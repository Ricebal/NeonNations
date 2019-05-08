using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DStarLite
{
    public State[][] Map;

    private Heap m_heap;
    private double m_km;
    private State m_goal;
    private State m_start;
    private State m_last;
    private IDStarLiteEnvironment m_environment;

    /// <summary>
    /// Set's up the Algorithm.
    /// </summary>
    /// <param name="startX">The x of the start-position</param>
    /// <param name="startY">The y of the start-position</param>
    /// <param name="goalY">The y of the goal-position</param>
    /// <param name="goalX">The x of the goal-position</param>
    public void RunDStarLite(int startX, int startY, int goalX, int goalY)
    {
        m_start = new State(this);
        m_start.X = startX;
        m_start.Y = startY;
        m_goal = new State(this);
        m_goal.X = goalX;
        m_goal.Y = goalY;
        m_last = m_start;
        Reset();
        ComputeShortestPath();
    }

    private void Reset()
    {
        //Creates an heap the size of the map
        m_heap = new Heap(Map.Length * Map[0].Length);
        for(int i = 0; i < Map.Length; i++)
        {
            for (int j = 0; j < Map[0].Length; j++)
            {
                Map[i][j].CostFromStartingPoint = double.PositiveInfinity;
                Map[i][j].Rhs = double.PositiveInfinity;
            }
        }
        m_goal = Map[m_goal.X][m_goal.Y];
        m_start = Map[m_start.X][m_start.Y];
        m_goal.Rhs = 0;
        m_km = 0;
        m_heap.Insert(m_goal, CalculatePriority(m_goal));
    }

    /// <summary>
    /// Needs to be called before the RunDStarLite-function.
    /// Generates an empty map for the bot
    /// </summary>
    /// <param name="width">The amount of columns of the map</param>
    /// <param name="height">The amount of rows of the map</param>
    /// <param name="env">Any class that extends IDStarLiteEnvironment. Used for moving the entity and getting the surroundings</param>
    public void GenerateEmptyMap(int width, int height, IDStarLiteEnvironment env)
    {
        Map = new State[width][];
        for (int i = 0; i < width; i++)
        {
            Map[i] = new State[height];
        }
        this.m_environment = env;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Map[i][j] = new State(this);
                Map[i][j].X = i;
                Map[i][j].Y = j;
                Map[i][j].CostFromStartingPoint = double.PositiveInfinity;
                Map[i][j].Rhs = double.PositiveInfinity;
            }
        }
    }

    /// <summary>
    /// Retrieve the new surrounding knowledge and applies this to the map.
    /// Calculate the (new) shortest path.
    /// Tells the entity where to move next.
    /// </summary>
    public void NextMove()
    {
        // if(sstart.g.isInfinity) then there is no known path
        m_start = MinSuccState(m_start);
        m_environment.MoveTo(new Coordinates(m_start.X, m_start.Y));
        LinkedList<Coordinates> obstacleCoord = m_environment.GetObstaclesInVision();
        double oldkm = m_km;
        State oldslast = m_last;
        m_km += Heuristic(m_start, m_last);
        m_last = m_start;
        bool change = false;
        foreach (Coordinates c in obstacleCoord)
        {
            State state = Map[c.X][c.Y];
            if (state.Obstacle)// The obstacle is already known
            {
                continue;
            }
            change = true;
            state.Obstacle = true;
            foreach (State surroundingState in state.GetSurroundingOpenSpaces())
            {
                UpdateVertex(surroundingState);
            }
        }
        if (!change)
        {
            m_km = oldkm;
            m_last = oldslast;
        }
        DebugMap(Map);
        ComputeShortestPath();
        Coordinates botCoordinates = m_environment.GetPosition();
        m_start = Map[botCoordinates.X][botCoordinates.Y];
    }

    // --------------------------------------------------------------------------------------------
    // Debug functions
    // --------------------------------------------------------------------------------------------

    private void DebugMap(State[][] map)
    {
        Coordinates botCoordinates = m_environment.GetPosition();
        StringBuilder builder = new StringBuilder();
        builder.Append('\n');
        for (int y = map[0].Length - 1; y >= 0; y--)
        {
            for (int x = 0; x < map.Length; x++)
            {
                if (map[x][y].Obstacle)
                {
                    builder.Append("#");
                    continue;
                }
                if(x == botCoordinates.X && y == botCoordinates.Y)
                {
                    builder.Append("@");
                    continue;
                }
                if (x == m_start.X && y == m_start.Y)
                {
                    builder.Append("%");
                    continue;
                }
                if (x == m_goal.X && y == m_goal.Y)
                {
                    builder.Append("X");
                    continue;
                }
                builder.Append("O");
            }
            builder.Append('\n');
        }
        string s = builder.ToString();
        Debug.Log(s);
    }
    
    ///<summary>
    /// calculates the key.
    /// Priority of a vertex = key.
    /// Key – vector with 2 components.
    /// k(s) = [ k1(s);  k2(s) ].
    /// k1(s) = min(g(s), rhs(s)) + h(s, sstart) + km.
    /// k2(s) = min(g(s), rhs(s)).
    ///</summary>
    public PriorityKey CalculatePriority(State s)
    {
        return new PriorityKey(SmallestValue(s.CostFromStartingPoint, s.Rhs) + Heuristic(s, m_start) + m_km, SmallestValue(s.CostFromStartingPoint, s.Rhs));
    }

    /// <summary>
    /// Calculates the Heuristics for traveling between two points
    /// </summary>
    public double Heuristic(State pointA, State pointB)
    {
        return Math.Sqrt(Math.Pow(Math.Abs(pointA.X - pointB.X), 2) + Math.Pow(Math.Abs(pointA.Y - pointB.Y), 2));
    }

    /// <summary>
    /// Re-evaluates the Rhs-value of a State
    /// </summary>
    public void UpdateVertex(State state)
    {
        if (!state.Equals(m_goal))
        {
            state.Rhs = MinSucc(state);
        }
        if (m_heap.Contains(state))   // To prevent any copies
        {
            m_heap.Remove(state);
        }
        if (state.CostFromStartingPoint != state.Rhs)
        {
            m_heap.Insert(state, CalculatePriority(state));
        }
    }

    /// <summary>
    /// Returns the position that has the lowest cost
    /// </summary>
    public State MinSuccState(State state)
    {
        double min = double.PositiveInfinity;
        State n = null;
        foreach (State s in state.GetVisitablePositions())
        {
            double val = 1 + s.CostFromStartingPoint;
            if (val <= min && !s.Obstacle)
            {
                min = val;
                n = s;
            }
        }
        return n;
    }

    /// <summary>
    /// Finds the succesor s' with the minium value of (c(state,s')+CostFromStartingPoint(s')).
    /// Where cost from state to s' is 1 and returns the value
    /// </summary>
    /// <returns>
    /// The minimum value of the surrounding tile(s) after adding 1 (for being the next move)
    /// </returns>
    public double MinSucc(State state)
    {
        double min = double.PositiveInfinity;
        foreach (State s in state.GetVisitablePositions())
        {
            double val = 1 + s.CostFromStartingPoint; // Add's 1 to the CostFromStartingPoint since it's one more move
            if (val < min && !s.Obstacle) // If the value is smaller than the minimum value found
            {
                min = val;
            }
        }
        return min;
    }

    public void ComputeShortestPath()
    {
        //While the top state of the Heap has a higher priority than the start state OR start.rhs is not the cost of start
        while (m_heap.TopKey().CompareTo(CalculatePriority(m_start)) < 0 || m_start.Rhs != m_start.CostFromStartingPoint)
        {
            PriorityKey oldKey = m_heap.TopKey();
            State state = m_heap.Pop();
            PriorityKey newKey = CalculatePriority(state);
            if (state == null) break; // Heap is empty
            if (oldKey.CompareTo(newKey) < 0) // The state has a lower priority than before
            {
                m_heap.Insert(state, newKey);
            }
            else if (state.CostFromStartingPoint > state.Rhs)
            {
                state.CostFromStartingPoint = state.Rhs;
                foreach (State surroundingState in state.GetSurroundingOpenSpaces())
                {
                    UpdateVertex(surroundingState);
                }
            }
            else
            {
                state.CostFromStartingPoint = double.PositiveInfinity;
                UpdateVertex(state);
                foreach (State surroundingState in state.GetSurroundingOpenSpaces())
                {
                    UpdateVertex(surroundingState);
                }
            }
        }
    }

    /// <summary>
    /// Returns the smallest of two values
    /// </summary>
    public double SmallestValue(double valueA, double valueB)
    {
        if (valueB < valueA) return valueB;
        return valueA;
    }
}