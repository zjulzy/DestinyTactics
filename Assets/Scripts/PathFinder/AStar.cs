using System;
using System.Collections.Generic;
using DestinyTactics.Cells;
using DestinyTactics.Characters;
using DestinyTactics.Systems;

namespace DestinyTactics.PathFinder
{
    public struct AStarNode
    {
        public int g;
        public int h;
        public int f;

        public AStarNode(int g_value, int h_value, int f_value)
        {
            g = g_value;
            h = h_value;
            f = f_value;
        }
    }

    public static class AStar
    {
        public static int CalcuPath(Dictionary<Cell, List<WeightCell>> adjacencyList, List<Cell> cellPath, Cell start,
            Cell destination)
        {
            //通过Astar算法生成路径
            cellPath.Clear();
            // cellPath.Add(start);
            List<Cell> openList = new List<Cell>();
            List<Cell> closeList = new List<Cell>();
            Character currentCharacter = start.correspondingCharacter;

            Dictionary<Cell, AStarNode> AstarNodes = new Dictionary<Cell, AStarNode>();
            foreach (var cell in adjacencyList.Keys)
            {
                AstarNodes.Add(cell, new AStarNode(-1, -1, -1));
            }

            AstarNodes[start] = new AStarNode(0, CalculateH(start,destination), CalculateH(start,destination));
            
            openList.Add(start);
            Cell currentCell = start;
            
            while(openList.Count>0 && currentCell!=destination)
            {
                //找到f值最小的节点
                int minF = int.MaxValue;
                foreach (var cell in openList)
                {
                    if (AstarNodes[cell].f < minF)
                    {
                        minF = AstarNodes[cell].f;
                        currentCell = cell;
                    }
                }
                openList.Remove(currentCell);
                closeList.Add(currentCell);
                //遍历邻接节点
                foreach (var weightCell in adjacencyList[currentCell])
                {
                    //去除已经遍历或者被占据的节点
                    if (closeList.Contains(weightCell.cell)||(weightCell.cell.correspondingCharacter&&weightCell.cell!=destination))
                    {
                        continue;
                    }
                    if (!openList.Contains(weightCell.cell))
                    {
                        openList.Add(weightCell.cell);
                    }
                    int g = AstarNodes[currentCell].g + weightCell.weight;
                    int h = CalculateH(weightCell.cell, destination);
                    int f = g + h;
                    if (AstarNodes[weightCell.cell].f == -1 || AstarNodes[weightCell.cell].f > f)
                    {
                        AstarNodes[weightCell.cell] = new AStarNode(g, h, f);
                    }
                }
            }
            //根据astarNodes生成最终路径结果
            while (currentCell != start)
            {
                cellPath.Add(currentCell);
                foreach (var weightCell in adjacencyList[currentCell])
                {
                    if (AstarNodes[weightCell.cell].g == AstarNodes[currentCell].g - weightCell.weight)
                    {
                        currentCell = weightCell.cell;
                        break;
                    }
                }
            }
            
            cellPath.Reverse();
            return AstarNodes[destination].g;
        }

        public static int CalculateH(Cell current, Cell destination)
        {
            //曼哈顿距离计算H
            return Math.Abs((int)(current.transform.position.x) - (int)(destination.transform.position.x)) +
                   Math.Abs((int)(current.transform.position.z) - (int)(destination.transform.position.z));
        }
    }
}