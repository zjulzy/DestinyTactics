using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DestinyTactics.Cells;
using DestinyTactics.Characters;
using DestinyTactics.Systems;
using DestinyTactics.PathFinder;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace DestinyTactics.Players
{
    public class AIPlayer : MonoBehaviour
    {
        private Dictionary<Character, Character> targets;
        private List<Character> characters;
        public Systems.GridSystem gridSystem;
        public GameMode gameMode;
        private bool bMoving;
        [Header("ai索敌距离")] public int distince;

        public void Awake()
        {
            characters = new List<Character>();
            targets = new Dictionary<Character, Character>();
        }

        public void Start()
        {
            foreach (var character in FindObjectsOfType<Character>())
            {
                if (character.type == CharacterType.AI)
                {
                    //绑定ai棋子的各种事件
                    characters.Add(character);
                    targets.Add(character, null);
                    character.blockInput += () => { bMoving = true; };
                    character.allowInput += () => { bMoving = false; };
                    character.CharacterDead += ((c) =>
                    {
                        characters.Remove(c);
                        targets.Remove(c);
                    });
                }
                else
                {
                    character.CharacterDead += ((c) =>
                    {
                        for (int i = 0; i < targets.Count; i++)
                        {
                            if(targets.ElementAt(i).Value==c)
                                targets[targets.ElementAt(i).Key] = null;
                        }
                    });
                }
            }
        }

        public IEnumerator execute()
        {
            //为每个character索敌并移动
            List<Cell> availableCells = new List<Cell>();
            List<Character> enemies = new List<Character>();
            foreach (var character in characters)
            {
                availableCells.Clear();
                enemies.Clear();
                BFS.FindAttackRange(gridSystem.AdjacencyList, availableCells, character.correspondingCell, distince);
                availableCells.ForEach((a) =>
                {
                    if (a.correspondingCharacter && a.correspondingCharacter.type == CharacterType.Player)
                    {
                        enemies.Add(a.correspondingCharacter);
                    }
                });

                if (enemies.Count > 0)
                {
                    enemies.Sort((a, b) => { return a.Health - b.Health; });
                    targets[character] = enemies[0];
                    // 随机选择一种能力，并基于该能力的攻击距离向敌方移动
                    // 需要随机找到一个能够释放的技能，所以在每个敌人都应该有一个可以随意释放的普通攻击
                    // TODO: AI玩家智能选择技能3
                    var ability = character.abilities[Random.Range(0, character.abilities.Count)];
                    while (!ability.TryCommitCost())
                    {
                        ability = character.abilities[Random.Range(0, character.abilities.Count)];
                    }

                    //能打到就直接攻击，否则抵近后攻击
                    if (AStar.CalculateH(character.correspondingCell, targets[character].correspondingCell) <=
                        ability.attackRange)
                    {
                        bool res = ability.TryActivate(character, targets[character]);
                        // 如果激活失败则选择激活条件最简单的普通攻击技能
                        if (!res)
                        {
                            character.abilities[0].TryActivate(character, targets[character]);
                        }
                    }
                    else
                    {
                        BFS.FindAvailable(gridSystem.AdjacencyList, availableCells, character.correspondingCell,
                            character.AP);
                        var path = new List<Cell>();
                        AStar.CalcuPath(gridSystem.AdjacencyList, path, character.correspondingCell,
                            targets[character].correspondingCell);
                        var finalPath = new List<Cell>();
                        foreach (var cell in path)
                        {
                            if ((!availableCells.Contains(cell)) ||
                                (AStar.CalculateH(cell, targets[character].correspondingCell) < ability.attackRange))
                            {
                                break;
                            }

                            finalPath.Add(cell);
                        }

                        if (finalPath.Count > 0)
                        {
                            character.correspondingCell.correspondingCharacter = null;
                            finalPath[finalPath.Count - 1].correspondingCharacter = character;
                            character.Move(finalPath[finalPath.Count - 1], finalPath);

                        }

                        while (bMoving)
                        {
                            yield return null;
                        }

                        if (AStar.CalculateH(character.correspondingCell, targets[character].correspondingCell) <=
                            ability.attackRange)
                        {
                            bool res = ability.TryActivate(character, targets[character]);
                            // 如果激活失败则选择激活条件最简单的普通攻击技能
                            if (!res)
                            {
                                character.abilities[0].TryActivate(character, targets[character]);
                            }
                        }
                    }
                }
                else
                {
                    //随即找个方向移动
                    targets[character] = null;
                    availableCells.Clear();
                    BFS.FindAvailable(gridSystem.AdjacencyList, availableCells, character.correspondingCell,
                        character.AP);
                    Cell destination = availableCells[Random.Range(0, availableCells.Count)];
                    var path = new List<Cell>();
                    AStar.CalcuPath(gridSystem.AdjacencyList, path, character.correspondingCell, destination);
                    character.correspondingCell.correspondingCharacter = null;
                    destination.correspondingCharacter = character;
                    character.Move(destination, path);
                }

                yield return new WaitForSeconds(1);
            }

            gameMode.EndTurn();
        }

        public void OnCharacterDead(Character character)
        {
            characters.Remove(character);
            targets.Remove(character);
        }

        public void Update()
        {
            //查询己方character是否还有剩余，如果没有就直接进入胜利页面
            if (characters.Count == 0)
            {
                SceneManager.LoadSceneAsync("Scenes/VictoryScene");
            }
        }
    }
}