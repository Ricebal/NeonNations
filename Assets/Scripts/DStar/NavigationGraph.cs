using System;
using System.Collections.Generic;
using UnityEngine;

public class NavigationGraph
{
    public Node[][] Map;

    public NavigationGraph(Tile[][] completeMap, bool knowMap, List<Tile> listOfObstacles)
    {
        Map = new Node[completeMap.Length][];
        for (int i = 0; i < Map.Length; i++)
        {
            Map[i] = new Node[completeMap[0].Length];
            for (int j = 0; j < Map[0].Length; j++)
            {
                Map[i][j] = new Node(listOfObstacles);
                if (knowMap)
                {
                    Map[i][j].Content = completeMap[i][j];
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

    /// <summary>
    /// Returns the Node that corresponds to a specific x- and y-value.
    /// </summary>
    public Node GetNode(int x, int y)
    {
        // Out of bounds check
        if (x >= 0 && x < Map.Length && y >= 0 && y < Map[0].Length)
        {
            return Map[x][y];
        }
        // return default
        return new Node(null);
    }

    /// <summary>
    /// Returns the Node that corresponds to a specific coordinate.
    /// </summary>
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

        // For all positions around the coordinates.
        // -1, -1 - 0, -1 - 1, -1
        // -1, 0  - 0, 0  - 1, 0
        // -1, 1  - 0, 1  - 1, 1
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
                    if (!tempState.IsObstacle())
                    {
                        openPositionsAroundEntity.AddFirst(new Vector2Int(x + i, y + j));
                    }
                }
            }
        }
        return openPositionsAroundEntity;
    }
}
