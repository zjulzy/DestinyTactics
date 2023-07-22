using System.Collections.Generic;
using DestinyTactics.Cells;
using DestinyTactics.Systems;

namespace DestinyTactics.PathFinder
{
    public static class BFS
    {
        public static void FindAvailable(Dictionary<Cell, List<WeightCell>> adjacencyList, List<Cell> available,
            Cell start, int AP)
        {
            //基于广度优先搜索找到所有可达节点
            available.Clear();
            available.Add(start);
            var candidate = new Queue<KeyValuePair<Cell, int>>();
            var visitedCells = new List<Cell>();
            candidate.Enqueue(new KeyValuePair<Cell, int>(start, AP));
            while (candidate.Count > 0)
            {
                var pair = candidate.Dequeue();
                Cell currentCell = pair.Key;
                int currentAP = pair.Value;
                adjacencyList[currentCell].ForEach((weightCell =>
                {
                    //友方或者敌方会阻挡路径
                    if (weightCell.weight <= currentAP && !visitedCells.Contains(weightCell.cell)&&!weightCell.cell.correspondingCharacter)
                    {
                        candidate.Enqueue(new KeyValuePair<Cell, int>(weightCell.cell, currentAP - weightCell.weight));
                        visitedCells.Add(weightCell.cell);
                        available.Add(weightCell.cell);
                    }
                }));
            }
        }

        public static void FindAttackRange(Dictionary<Cell, List<WeightCell>> adjacencyList, List<Cell> attackList,
            Cell start, int attackRange)
        {
            //基于广度优先搜索找到所有可攻击节点
            attackList.Clear();
            attackList.Add(start);
            var candidate = new Queue<KeyValuePair<Cell, int>>();
            var visitedCells = new List<Cell>();
            candidate.Enqueue(new KeyValuePair<Cell, int>(start, attackRange));
            while (candidate.Count > 0)
            {
                var pair = candidate.Dequeue();
                Cell currentCell = pair.Key;
                int currentattackRange = pair.Value;
                adjacencyList[currentCell].ForEach((weightCell =>
                {
                    if (1 <= currentattackRange && !visitedCells.Contains(weightCell.cell))
                    {
                        candidate.Enqueue(new KeyValuePair<Cell, int>(weightCell.cell, currentattackRange - 1));
                        visitedCells.Add(weightCell.cell);
                        attackList.Add(weightCell.cell);
                    }
                }));
            }
        }
    }
}