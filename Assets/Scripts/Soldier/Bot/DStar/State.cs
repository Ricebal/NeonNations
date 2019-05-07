using System.Collections.Generic;

public class State
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

    public State(DStarLite dStarLite)
    {
        m_dStarLite = dStarLite;
    }

    public bool Equals(State that)
    {
        if (this.X == that.X && this.Y == that.Y) return true;
        return false;
    }

    /// <summary>
    /// Get open spaces it can move to
    /// Will take in account that spaces diagonal to the current position, the spaces adjecent to the current space and the diagonal space should be empty too.
    /// This can be removed if we'd use path-smoothening and only 4 directions for the algorithm.
    /// </summary>
    public LinkedList<State> GetVisitablePositions()
    {
        LinkedList<State> s = new LinkedList<State>();
        int width = m_dStarLite.Map.GetLength(0);
        int height = m_dStarLite.Map.GetLength(1);
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                // Check if point is inside the map
                if (X + i < width && X + i >= 0 && Y + j < height && Y + j >= 0)
                {
                    // If the position is diagonal
                    if (!(i == 0 ^ j == 0))
                    {
                        // And the adjecent positions are no obstacles
                        if (m_dStarLite.Map[X + i, Y].Obstacle || m_dStarLite.Map[X, Y + j].Obstacle)
                        {
                            continue;
                        }
                    }
                    s.AddFirst(m_dStarLite.Map[X + i, Y + j]);
                }
            }
        }
        return s;
    }

    /// <summary>
    /// Search for surrounding positions 
    /// </summary>
    public LinkedList<State> GetSurroundingOpenSpaces()
    {
        int width = m_dStarLite.Map.GetLength(0);
        int height = m_dStarLite.Map.GetLength(1);
        LinkedList<State> openPositionsAroundEntity = new LinkedList<State>();
        State tempState;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                // Check if point is inside the map
                if (X + i < width && X + i >= 0 && Y + j < height && Y + j >= 0)
                {
                    tempState = m_dStarLite.Map[X + i, Y + j];
                    // If it's no obstacle
                    if (!tempState.Obstacle) openPositionsAroundEntity.AddFirst(tempState);
                }
            }
        }
        return openPositionsAroundEntity;
    }
}