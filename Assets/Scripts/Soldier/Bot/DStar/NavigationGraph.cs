using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Soldier.Bot.DStar
{
    public class NavigationGraph
    {
        public Node[][] Map;

        public NavigationGraph(int[][] completeMap, bool knowMap)
        {
            Map = new Node[completeMap.Length][];
            for (int i = 0; i < Map.Length; i++)
            {
                Map[i] = new Node[completeMap[0].Length];
            }
            for (int i = 0; i < Map.Length; i++)
            {
                for (int j = 0; j < Map[0].Length; j++)
                {
                    Map[i][j] = new Node();
                    Map[i][j].CostFromStartingPoint = double.PositiveInfinity;
                    Map[i][j].Rhs = double.PositiveInfinity;

                    if (knowMap && completeMap[i][j] == 1)
                    {
                        Map[i][j].Obstacle = true;
                    }
                }
            }
        }

        public int GetSize()
        {
            return Map.Length * Map[0].Length;
        }

        public void Reset()
        {
            for (int i = 0; i < Map.Length; i++)
            {
                for (int j = 0; j < Map[0].Length; j++)
                {
                    Map[i][j].CostFromStartingPoint = double.PositiveInfinity;
                    Map[i][j].Rhs = double.PositiveInfinity;
                }
            }
        }

        public Node GetNode(int x, int y)
        {
            return Map[x][y];
        }

        public Node GetNode(Vector2Int coordinate)
        {
            return Map[coordinate.x][coordinate.y];
        }

        /// <summary>
        /// Search for surrounding positions 
        /// </summary>
        public LinkedList<Vector2Int> GetSurroundingOpenSpaces(Vector2Int coordinates)
        {
            int x = coordinates.x;
            int y = coordinates.y;
            int width = Map.Length;
            int height = Map[0].Length;
            LinkedList<Vector2Int> openPositionsAroundEntity = new LinkedList<Vector2Int>();
            Node tempState;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (Math.Abs(i) == Math.Abs(j))
                    {
                        continue;
                    }
                    // Check if point is inside the map
                    if (x + i < width && x + i >= 0 && y + j < height && y + j >= 0)
                    {
                        tempState = Map[x + i][y + j];
                        // If it's no obstacle
                        if (!tempState.Obstacle)
                        {
                            //openPositionsAroundEntity.AddFirst(tempState);
                            openPositionsAroundEntity.AddFirst(new Vector2Int(x + i, y + j));
                        }
                    }
                }
            }
            return openPositionsAroundEntity;
        }
    }
}
