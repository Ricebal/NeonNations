using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DStarLite
{
    public Heap Heap;
    public State[,] Map;
    public double Km;
    public State SGoal;
    public State SStart;
    private State SLast;
    private IDStarLiteEnvironment env;

    /// <summary>
    /// Set's up the Algorithm.
    /// </summary>
    /// <param name="startX">The x of the start-position</param>
    /// <param name="startY">The y of the start-position</param>
    /// <param name="goalY">The y of the goal-position</param>
    /// <param name="goalX">The x of the goal-position</param>
    public void RunDStarLite(int startX, int startY, int goalX, int goalY)
    {
        SStart = new State(this);
        SStart.X = startX;
        SStart.Y = startY;
        SGoal = new State(this);
        SGoal.X = goalX;
        SGoal.Y = goalY;
        SLast = SStart;
        Initialize();
        ComputeShortestPath();
    }

    private void Initialize()
    {
        //Creates an heap the size of the map
        Heap = new Heap(Map.GetLength(0)*Map.GetLength(1));
        for(int i = 0; i < Map.GetLength(0); i++)
        {
            for (int j = 0; j<Map.GetLength(1); j++)
            {
                Map[i, j].CostFromStartingPoint = double.PositiveInfinity;
                Map[i, j].Rhs = double.PositiveInfinity;
            }
        }
        SGoal = Map[SGoal.X, SGoal.Y];
        SStart = Map[SStart.X, SStart.Y];
        SGoal.Rhs = 0;
        Km = 0;
        Heap.Insert(SGoal, CalculateKey(SGoal));
    }

    /// <summary>
    /// Needs to be called before the RunDStarLite-function.
    /// Generates an empty map for the bot
    /// </summary>
    /// <param name="x">The amount of columns of the map</param>
    /// <param name="y">The amount of rows of the map</param>
    /// <param name="env">Any class that extends IDStarLiteEnvironment. Used for moving the entity and getting the surroundings</param>
    public void GenerateEmptyMap(int x, int y, IDStarLiteEnvironment env)
    {
        Map = new State[x, y];
        this.env = env;
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                Map[i, j] = new State(this);
                Map[i, j].X = i;
                Map[i, j].Y = j;
                Map[i, j].CostFromStartingPoint = double.PositiveInfinity;
                Map[i, j].Rhs = double.PositiveInfinity;
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
        SStart = MinSuccState(SStart);
        env.MoveTo(new Coordinates(SStart.X, SStart.Y));
        LinkedList<Coordinates> obstacleCoord = env.GetObstaclesInVision();
        double oldkm = Km;
        State oldslast = SLast;
        Km += Heuristic(SStart, SLast);
        SLast = SStart;
        bool change = false;
        foreach (Coordinates c in obstacleCoord)
        {
            State s = Map[c.X, c.Y];
            if (s.Obstacle)// The obstacle is already known
            {
                continue;
            }
            change = true;
            s.Obstacle = true;
            foreach (State p in s.GetSurroundingOpenSpaces())
            {
                UpdateVertex(p);
            }
        }
        if (!change)
        {
            Km = oldkm;
            SLast = oldslast;
        }
        DebugMap(Map);
        ComputeShortestPath();
        Coordinates botCoordinates = env.GetPosition();
        SStart = Map[botCoordinates.X, botCoordinates.Y];
    }

    // --------------------------------------------------------------------------------------------
    // Debug functions
    // --------------------------------------------------------------------------------------------

    private void DebugMap(State[,] map)
    {
        Coordinates botCoordinates = env.GetPosition();
        StringBuilder builder = new StringBuilder();
        builder.Append('\n');
        for (int y = map.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                if (map[x, y].Obstacle)
                {
                    builder.Append("#");
                    continue;
                }
                if(x == botCoordinates.X && y == botCoordinates.Y)
                {
                    builder.Append("@");
                    continue;
                }
                if (x == SStart.X && y == SStart.Y)
                {
                    builder.Append("%");
                    continue;
                }
                if (x == SGoal.X && y == SGoal.Y)
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
    /// calculates the key
    /// Priority of a vertex = key
    /// Key – vector with 2 components
    /// k(s) = [ k1(s);  k2(s) ]
    /// k1(s) = min(g(s), rhs(s)) + h(s, sstart) + km
    /// k2(s) = min(g(s), rhs(s)) 
    ///</summary>
    public Key CalculateKey(State s)
    {
        return new Key(SmallestValue(s.CostFromStartingPoint, s.Rhs) + Heuristic(s, SStart) + Km, SmallestValue(s.CostFromStartingPoint, s.Rhs));
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
        if (!state.Equals(SGoal))
        {
            state.Rhs = MinSucc(state);
        }
        if (Heap.Contains(state))
        {
            Heap.Remove(state);
        }
        if (state.CostFromStartingPoint != state.Rhs)
        {
            Heap.Insert(state, CalculateKey(state));
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
    /// finds the succesor s' with the min (c(u,s')+g(s'))
    /// where cost from u to s' is 1 and returns the value
    /// </summary>
    public double MinSucc(State state)
    {
        double min = double.PositiveInfinity;
        foreach (State s in state.GetVisitablePositions())
        {
            double val = 1 + s.CostFromStartingPoint;
            if (val < min && !s.Obstacle) min = val;
        }
        return min;
    }

    public void ComputeShortestPath()
    {
        while (Heap.TopKey().CompareTo(CalculateKey(SStart)) < 0 || SStart.Rhs != SStart.CostFromStartingPoint)
        {
            Key oldKey = Heap.TopKey();
            State state = Heap.Pop();
            if (state == null) break;
            if (oldKey.CompareTo(CalculateKey(state)) < 0)
            {
                Heap.Insert(state, CalculateKey(state));
            }
            else if (state.CostFromStartingPoint > state.Rhs)
            {
                state.CostFromStartingPoint = state.Rhs;
                foreach (State s in state.GetSurroundingOpenSpaces())
                {
                    UpdateVertex(s);
                }
            }
            else
            {
                state.CostFromStartingPoint = double.PositiveInfinity;
                UpdateVertex(state);
                foreach (State s in state.GetSurroundingOpenSpaces())
                {
                    UpdateVertex(s);
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