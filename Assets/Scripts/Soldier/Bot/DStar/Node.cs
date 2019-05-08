using System;
using System.Collections.Generic;

public class Node
{
    public int X;
    public int Y;
    // The cost it takes to get from the starting point to this position. (Also known as g-value)
    public double CostFromStartingPoint;
    // Some kind of potentially better estimated value than the g-value.
    public double Rhs;
    // If this position contains an obstacle that can't be passed (A wall for example)
    public bool Obstacle;

    private DStarLite m_dStarLite;

    public Node(DStarLite dStarLite)
    {
        m_dStarLite = dStarLite;
    }

    public bool Equals(Node that)
    {
        if (this.X == that.X && this.Y == that.Y) return true;
        return false;
    }

    /// <summary>
    /// Search for surrounding positions 
    /// </summary>
    public LinkedList<Node> GetSurroundingOpenSpaces()
    {
        int width = m_dStarLite.Map.Length;
        int height = m_dStarLite.Map[0].Length;
        LinkedList<Node> openPositionsAroundEntity = new LinkedList<Node>();
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
                if (X + i < width && X + i >= 0 && Y + j < height && Y + j >= 0)
                {
                    tempState = m_dStarLite.Map[X + i][Y + j];
                    // If it's no obstacle
                    if (!tempState.Obstacle) openPositionsAroundEntity.AddFirst(tempState);
                }
            }
        }
        return openPositionsAroundEntity;
    }
}