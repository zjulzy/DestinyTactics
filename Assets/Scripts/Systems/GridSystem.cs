using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using DestinyTactics.Characters;
using DestinyTactics.Cells;
using DestinyTactics.Characters.Abilities;
using DestinyTactics.PathFinder;
using DestinyTactics.UI;
using TMPro;
using UnityEngine.UI;

namespace DestinyTactics.Systems
{
    public enum ClickState
    {
        Unactivated,
        Activated,
        AbilitySelected
    }

    public static class CellColor
    {
        public static Color normal = Color.white;
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
        private AbilityUI _abilityUI;

        //所有节点和位置的映射
        private Dictionary<Vector3, Cell> Cells;
        private bool bInput;

        public Action<Character> ChangeDisplayerCharacterInfo;
        public Action<Character> OpenAbilityPanel;
        public Action CloseAbilityPanel;

        public void Awake()
        {
            _clickState = ClickState.Unactivated;
            Cells = new Dictionary<Vector3, Cell>();
            AdjacencyList = new Dictionary<Cell, List<WeightCell>>();
            _path = new List<Cell>();
            _availableCells = new List<Cell>();
            _attackCells = new List<Cell>();
            bInput = true;
            _abilityUI = FindObjectOfType<AbilityUI>();
            _abilityUI.SelectedAbility += OnAbilitySelected;
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
                if (currentCell.bIsObstacle) continue;

                if (Cells.ContainsKey(location + new Vector3(0, 0, -1)) &&
                    !Cells[location + new Vector3(0, 0, -1)].bIsObstacle)
                {
                    Cell nextCell = Cells[location + new Vector3(0, 0, -1)];
                    AdjacencyList[currentCell]
                        .Add(new WeightCell(nextCell, Math.Max(currentCell.aPConsume, nextCell.aPConsume)));
                }

                if (Cells.ContainsKey(location + new Vector3(0, 0, 1)) &&
                    !Cells[location + new Vector3(0, 0, 1)].bIsObstacle)
                {
                    Cell nextCell = Cells[location + new Vector3(0, 0, 1)];
                    AdjacencyList[currentCell]
                        .Add(new WeightCell(nextCell, Math.Max(currentCell.aPConsume, nextCell.aPConsume)));
                }

                if (Cells.ContainsKey(location + new Vector3(-1, 0, 0)) &&
                    !Cells[location + new Vector3(-1, 0, 0)].bIsObstacle)
                {
                    Cell nextCell = Cells[location + new Vector3(-1, 0, 0)];
                    AdjacencyList[currentCell]
                        .Add(new WeightCell(nextCell, Math.Max(currentCell.aPConsume, nextCell.aPConsume)));
                }

                if (Cells.ContainsKey(location + new Vector3(1, 0, 0)) &&
                    !Cells[location + new Vector3(1, 0, 0)].bIsObstacle)
                {
                    Cell nextCell = Cells[location + new Vector3(1, 0, 0)];
                    AdjacencyList[currentCell]
                        .Add(new WeightCell(nextCell, Math.Max(currentCell.aPConsume, nextCell.aPConsume)));
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
                    case ClickState.Unactivated:
                        if (ClickedCell.correspondingCharacter &&
                            ClickedCell.correspondingCharacter.type == CharacterType.Player)
                        {
                            //基于点击cell直接进行激活
                            Activate(ClickedCell);
                        }

                        break;
                    case ClickState.Activated:
                        if (ClickedCell == ActivatedCell) return;
                        if (ClickedCell.correspondingCharacter &&
                            ClickedCell.correspondingCharacter.type == CharacterType.Player)
                        {
                            //关闭原有的activatedcell并且重新激活
                            Unactivate();
                            Activate(ClickedCell);
                        }

                        //检查点击cell是否为可达cell，如果是移动
                        else if (_availableCells.Contains(ClickedCell))
                        {
                            if (ClickedCell.correspondingCharacter) break;

                            _path.Clear();
                            ActivatedCell.correspondingCharacter.AP -=
                                AStar.CalcuPath(AdjacencyList, _path, ActivatedCell, ClickedCell);
                            // ClickedCell.correspondingCharacter.Move(ClickedCell, _path);
                            ActivatedCell.correspondingCharacter.Move(ClickedCell, new List<Cell>(_path));
                            
                            //转换activatecell
                            ClickedCell.correspondingCharacter = ActivatedCell.correspondingCharacter;
                            ActivatedCell.correspondingCharacter = null;
                            ActivatedCell = ClickedCell;

                            //将_path和availablecells状态重置
                            //将之前的available cell的颜色清空并重新计算
                            _availableCells.ForEach((a) =>
                            {
                                a.transform.GetComponent<Renderer>().material.color = CellColor.normal;
                            });
                            _path.Clear();
                            FindAvailable(ClickedCell, ClickedCell.correspondingCharacter.AP);

                        }
                        else
                        {
                            //退出激活状态
                            Unactivate();
                        }

                        break;
                    case ClickState.AbilitySelected:
                        // 检查点击cell是否为可攻击cell，如果是直接攻击
                        if (_attackCells.Contains(ClickedCell) && ClickedCell.correspondingCharacter)
                        {
                            if (ClickedCell.correspondingCharacter.type != ActivatedCell.correspondingCharacter.type &&
                                ActivatedCell.correspondingCharacter.bCanAttack)
                            {
                                //攻击目标
                                Character target = ClickedCell.correspondingCharacter;
                                ActivatedCell.correspondingCharacter.abilities[_abilityUI.selectedID]
                                    .TryActivate(ActivatedCell.correspondingCharacter, target);

                                //退出激活模式
                                Unactivate();
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
                    case ClickState.Unactivated:
                        HoveredCell.GetComponent<Renderer>().material.color = CellColor.hover;
                        break;
                    
                    case ClickState.Activated:
                        if (HoveredCell == ActivatedCell) break;

                        //清空_path并将其颜色还原为available
                        _path.ForEach((a) =>
                        {
                            a.transform.GetComponent<Renderer>().material.color = CellColor.available;
                        });
                        
                        //检测是否可达，可达则计算路径
                        if (_availableCells.Contains(HoveredCell))
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
                    
                    case ClickState.AbilitySelected:
                        //检测是否可攻击，可攻击则显示准备攻击效果
                        if (_attackCells.Contains(HoveredCell) && HoveredCell.correspondingCharacter &&
                            _abilityUI.selectedID != -1)
                        {
                            if (HoveredCell.correspondingCharacter.type != ActivatedCell.correspondingCharacter.type)
                            {
                                var target = HoveredCell.correspondingCharacter;
                                target.GetComponent<Renderer>().material.color = CellColor.attackPrepared;
                                ActivatedCell.correspondingCharacter.AttackPrepare();
                            }
                        }
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
                    case ClickState.Unactivated:
                        UnhoveredCell.GetComponent<Renderer>().material.color = CellColor.normal;
                        break;
                    
                    case ClickState.Activated:
                        if (UnhoveredCell.correspondingCharacter)
                        {
                            
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
                    
                    case ClickState.AbilitySelected:
                        if (_attackCells.Contains(UnhoveredCell) && UnhoveredCell != ActivatedCell)
                        {
                            UnhoveredCell.correspondingCharacter.GetComponent<Renderer>().material
                                .DisableKeyword("_EMISSION");
                            UnhoveredCell.correspondingCharacter.GetComponent<Renderer>().material.color =
                                Color.white;
                        }
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
            _availableCells.ForEach(a => { a.transform.GetComponent<Renderer>().material.color = CellColor.normal; });
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
            if (!ActivatedCell) return;
            ClearAttackable();
            BFS.FindAttackRange(AdjacencyList, _attackCells, cell, attackRange);
            _attackCells.ForEach((a) =>
            {
                if (a.correspondingCharacter &&
                    a.correspondingCharacter.type != cell.correspondingCharacter.type)
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
            _availableCells.Clear();
            _path.Clear();
            ClearAttackable();

            _clickState = ClickState.Unactivated;
            if (ActivatedCell)
            {
                ActivatedCell.correspondingCharacter.GetComponent<Renderer>().material.DisableKeyword(
                    "_EMISSION");
            }

            CloseAbilityPanel();
            ActivatedCell = null;
        }

        protected void Activate(Cell activateCell)
        {
            ActivatedCell = activateCell;
            _clickState = ClickState.Activated;
            ActivatedCell.correspondingCharacter.GetComponent<Renderer>().material
                .EnableKeyword("_EMISSION");
            OpenAbilityPanel(activateCell.correspondingCharacter);
            FindAvailable(activateCell, activateCell.correspondingCharacter.AP);
        }

        protected void OnAbilitySelected(int id)
        {
            if (id != -1)
            {
                FindAttackable(ActivatedCell, id);
            }

            else
            {
                _clickState = ClickState.Activated;
                _attackCells.Clear();
            }


        }
        

        public void ResetTurn()
        {
            Unactivate();

            _clickState = ClickState.Unactivated;
            _path.ForEach((a) => { a.GetComponent<Renderer>().material.color = CellColor.normal; });
            _availableCells.ForEach((a) => { a.GetComponent<Renderer>().material.color = CellColor.normal; });
            _path.Clear();
            _availableCells.Clear();
        }

        public void ClearAttackable()
        {
            _attackCells.ForEach(a =>
            {
                if (a.correspondingCharacter &&
                    a.correspondingCharacter.type != ActivatedCell.correspondingCharacter.type)
                {
                    a.correspondingCharacter.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
                    a.correspondingCharacter.GetComponent<Renderer>().material.color = Color.white;
                }
            });
            _attackCells.Clear();
        }
    }
}