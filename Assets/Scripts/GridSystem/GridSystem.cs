using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using DestinyTactics.Characters;
using DestinyTactics.Cells;
using DestinyTactics.GridSystem;
using DestinyTactics.PathFinder;
using TMPro;
using UnityEngine.UI;

namespace DestinyTactics.GridSystem
{
    public enum ClickState
    {
        unactivated,
        activated,
    }

    public static class CellColor
    {
        public static Color normal = Color.gray;
        public static Color hover = Color.magenta;
        public static Color available = Color.green;
        public static Color clicked = Color.blue;
        public static Color onPath = Color.cyan;
        public static Color attackable = Color.yellow;
        public static Color attackPrepared = Color.red;
    }

    public struct WeightCell
    {
        public Cell cell;
        public int weight;

        public WeightCell(Cell c, int w)
        {
            cell = c;
            weight = w;
        }
    }


    public class GridSystem : MonoBehaviour
    {
        //邻接矩阵
        public Dictionary<Cell, List<WeightCell>> AdjacencyList;
        public TextMeshProUGUI infoText;
        private ClickState _clickState;

        //可达和可攻击节点
        private List<Cell> _availableCells;
        private List<Cell> _attackCells;
        private List<Cell> _path;
        private Cell ActivatedCell;

        //所有节点和位置的映射
        private Dictionary<Vector3, Cell> Cells;
        private bool bInput;

        public Action<Character> ChangeDisplayerCharacterInfo;

        public void Awake()
        {
            _clickState = ClickState.unactivated;
            Cells = new Dictionary<Vector3, Cell>();
            AdjacencyList = new Dictionary<Cell, List<WeightCell>>();
            _path = new List<Cell>();
            _availableCells = new List<Cell>();
            _attackCells = new List<Cell>();
            bInput = true;
        }

        public void Start()
        {
            foreach (var componentsInChild in GetComponentsInChildren<Cell>())
            {
                Debug.Log(componentsInChild.name);
                componentsInChild.OnCellClick += OnCellClicked;
                componentsInChild.OnCellHover += OnCellHovered;
                componentsInChild.OnCellUnHover += OnCellUnhovered;

                Cells.Add(componentsInChild.GetLocation(), componentsInChild);
            }

            foreach (var componentsInChild in GetComponentsInChildren<Character>())
            {
                componentsInChild.allowInput += OnAllowInput;
                componentsInChild.blockInput += OnBlockInput;
                componentsInChild.correspondingCell.correspondingCharacter = componentsInChild;
            }

            //初始化邻接表
            foreach (var location in Cells.Keys)
            {
                Cell currentCell = Cells[location];
                AdjacencyList.Add(Cells[location], new List<WeightCell>());
                if (Cells.ContainsKey(location + new Vector3(0, 0, -1)))
                {
                    AdjacencyList[currentCell]
                        .Add(new WeightCell(Cells[location + new Vector3(0, 0, -1)], currentCell.aPConsume));
                }

                if (Cells.ContainsKey(location + new Vector3(0, 0, 1)))
                {
                    AdjacencyList[currentCell]
                        .Add(new WeightCell(Cells[location + new Vector3(0, 0, 1)], currentCell.aPConsume));
                }

                if (Cells.ContainsKey(location + new Vector3(-1, 0, 0)))
                {
                    AdjacencyList[currentCell]
                        .Add(new WeightCell(Cells[location + new Vector3(-1, 0, 0)], currentCell.aPConsume));
                }

                if (Cells.ContainsKey(location + new Vector3(1, 0, 0)))
                {
                    AdjacencyList[currentCell]
                        .Add(new WeightCell(Cells[location + new Vector3(1, 0, 0)], currentCell.aPConsume));
                }
            }

            bInput = true;
        }

        public void OnAllowInput()
        {
            bInput = true;
        }

        public void OnBlockInput()
        {
            bInput = false;
        }

        # region InputProcess

        //########################################################################################################################
        //处理用户的各种输入事件
        public void OnCellClicked(Cell ClickedCell)
        {
            if (bInput)
            {
                switch (_clickState)
                {
                    case ClickState.activated:
                        if (ClickedCell == ActivatedCell) return;
                        // 检查点击cell是否为可攻击cell，如果是直接攻击
                        if (_attackCells.Contains(ClickedCell) && ClickedCell.correspondingCharacter)
                        {
                            if (ClickedCell.correspondingCharacter.type != ActivatedCell.correspondingCharacter.type &&
                                ActivatedCell.correspondingCharacter.bCanAttack)
                            {
                                //攻击目标
                                Character target = ClickedCell.correspondingCharacter;
                                ActivatedCell.correspondingCharacter.Attack(target);

                                //退出激活模式
                                Unactivate();
                            }
                        }
                        //检查点击cell是否为可达cell，如果是移动
                        else if (_availableCells.Contains(ClickedCell))
                        {
                            if (ClickedCell.correspondingCharacter) break;

                            _path.Clear();
                            ActivatedCell.correspondingCharacter.AP -=
                                AStar.CalcuPath(AdjacencyList, _path, ActivatedCell, ClickedCell);
                            // ClickedCell.correspondingCharacter.Move(ClickedCell, _path);
                            StartCoroutine(
                                ActivatedCell.correspondingCharacter.Move(ClickedCell, new List<Cell>(_path)));
                            //将之前的available cell的颜色清空并重新计算
                            _availableCells.ForEach((a) =>
                            {
                                a.transform.GetComponent<Renderer>().material.color = CellColor.normal;
                            });

                            //转换activatecell
                            ClickedCell.correspondingCharacter = ActivatedCell.correspondingCharacter;
                            ActivatedCell.correspondingCharacter = null;
                            ActivatedCell = ClickedCell;

                            //将_path和availablecells状态重置
                            _path.Clear();
                            FindAvailable(ClickedCell, ClickedCell.correspondingCharacter.AP);

                            //查询可被攻击的敌人
                            if (ActivatedCell.correspondingCharacter.bCanAttack)
                            {
                                FindAttackable(ClickedCell, ClickedCell.correspondingCharacter.attackRange);
                            }
                        }
                        else
                        {
                            //退出激活状态
                            Unactivate();
                        }

                        break;
                    case ClickState.unactivated:
                        if (ClickedCell.correspondingCharacter &&
                            ClickedCell.correspondingCharacter.type == CharacterType.Player)
                        {
                            _clickState = ClickState.activated;
                            ActivatedCell = ClickedCell;
                            ActivatedCell.correspondingCharacter.GetComponent<Renderer>().material
                                .EnableKeyword("_EMISSION");
                            FindAvailable(ClickedCell, ClickedCell.correspondingCharacter.AP);
                            if (ActivatedCell.correspondingCharacter.bCanAttack)
                            {
                                FindAttackable(ClickedCell, ClickedCell.correspondingCharacter.attackRange);
                            }
                        }

                        break;
                }
            }
        }

        public void OnCellHovered(Cell HoveredCell)
        {
            if (HoveredCell.correspondingCharacter)
            {
                ChangeDisplayerCharacterInfo(HoveredCell.correspondingCharacter);
            }

            if (bInput)
            {
                switch (_clickState)
                {
                    case ClickState.activated:
                        if (HoveredCell == ActivatedCell) break;

                        //清空_path并将其颜色还原为available
                        _path.ForEach((a) =>
                        {
                            a.transform.GetComponent<Renderer>().material.color = CellColor.available;
                        });

                        //检测是否可攻击，可攻击则显示准备攻击效果
                        if (_attackCells.Contains(HoveredCell) && HoveredCell.correspondingCharacter)
                        {
                            if (HoveredCell.correspondingCharacter.type != ActivatedCell.correspondingCharacter.type)
                            {
                                var target = HoveredCell.correspondingCharacter;
                                target.GetComponent<Renderer>().material.color = CellColor.attackPrepared;
                                ActivatedCell.correspondingCharacter.AttackPrepare();
                            }
                        }
                        //检测是否可达，可达则计算路径
                        else if (_availableCells.Contains(HoveredCell))
                        {
                            if (HoveredCell.correspondingCharacter)
                            {
                                _path.ForEach((a) =>
                                {
                                    a.transform.GetComponent<Renderer>().material.color = CellColor.available;
                                });
                                _path.Clear();
                            }
                            else
                            {
                                FindPath(ActivatedCell, HoveredCell);
                            }
                        }
                        else
                        {
                            _path.ForEach((a) =>
                            {
                                a.transform.GetComponent<Renderer>().material.color = CellColor.available;
                            });
                            _path.Clear();
                        }

                        break;
                    case ClickState.unactivated:
                        HoveredCell.GetComponent<Renderer>().material.color = CellColor.hover;
                        break;
                }
            }
        }

        public void OnCellUnhovered(Cell UnhoveredCell)
        {
            if (bInput)
            {
                switch (_clickState)
                {
                    case ClickState.activated:
                        if (UnhoveredCell.correspondingCharacter)
                        {
                            if (_attackCells.Contains(UnhoveredCell) && UnhoveredCell != ActivatedCell)
                            {
                                UnhoveredCell.correspondingCharacter.GetComponent<Renderer>().material
                                    .DisableKeyword("_EMISSION");
                                UnhoveredCell.correspondingCharacter.GetComponent<Renderer>().material.color =
                                    Color.white;
                            }
                        }
                        else
                        {
                            if (_availableCells.Contains(UnhoveredCell))
                            {
                                UnhoveredCell.GetComponent<Renderer>().material.color = CellColor.available;
                            }
                            else
                            {
                                UnhoveredCell.GetComponent<Renderer>().material.color = CellColor.normal;
                            }
                        }

                        break;
                    case ClickState.unactivated:
                        UnhoveredCell.GetComponent<Renderer>().material.color = CellColor.normal;
                        break;
                }
            }

            if (UnhoveredCell.correspondingCharacter && ActivatedCell)
            {
                ChangeDisplayerCharacterInfo(ActivatedCell.correspondingCharacter);
            }
        }

        //########################################################################################################################

        #endregion

        #region findpath

        public void FindAvailable(Cell start, int AP)
        {
            _availableCells.Clear();
            BFS.FindAvailable(AdjacencyList, _availableCells, start, AP);
            _availableCells.ForEach((a) =>
            {
                a.transform.GetComponent<Renderer>().material.color = CellColor.available;
            });
        }

        public void FindPath(Cell start, Cell destination)
        {
            _path.Clear();

            AStar.CalcuPath(AdjacencyList, _path, start, destination);
            _path.ForEach((a) => { a.transform.GetComponent<Renderer>().material.color = CellColor.onPath; });
        }

        public void FindAttackable(Cell cell, int attackRange)
        {
            _attackCells.Clear();
            BFS.FindAttackRange(AdjacencyList, _attackCells, cell, attackRange);
            _attackCells.ForEach((a) =>
            {
                if (a.correspondingCharacter && a.correspondingCharacter.type != cell.correspondingCharacter.type)
                {
                    a.correspondingCharacter.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
                }
            });
        }

        #endregion


        protected void Unactivate()
        {
            _availableCells.ForEach((a) => { a.transform.GetComponent<Renderer>().material.color = CellColor.normal; });
            _path.ForEach(a => { a.transform.GetComponent<Renderer>().material.color = CellColor.normal; });
            _attackCells.ForEach(a =>
            {
                if (a.correspondingCharacter &&
                    a.correspondingCharacter.type != ActivatedCell.correspondingCharacter.type)
                {
                    a.correspondingCharacter.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
                    a.correspondingCharacter.GetComponent<Renderer>().material.color = Color.white;
                }
            });
            _availableCells.Clear();
            _attackCells.Clear();
            _attackCells.Clear();

            _clickState = ClickState.unactivated;
            ActivatedCell.correspondingCharacter.GetComponent<Renderer>().material.DisableKeyword(
                "_EMISSION");
            ActivatedCell = null;
        }

        public void ResetTurn()
        {
            _attackCells.Clear();
            ActivatedCell = null;
            _clickState = ClickState.unactivated;
            _path.ForEach((a) => { a.GetComponent<Renderer>().material.color = CellColor.normal; });
            _availableCells.ForEach((a) => { a.GetComponent<Renderer>().material.color = CellColor.normal; });
            _path.Clear();
            _availableCells.Clear();
        }
    }
}