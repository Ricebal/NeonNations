using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DStarLite
{
    public static Heap U;
    public static State[,] S;
    public static double km;
    public static State sgoal;
    public static State sstart;
    private static State slast;
    private IDStarLiteEnvironment env;

    // sx and sy are the start coordinates and gx and gy are the goal coordinates
    public void RunDStarLite(int sx, int sy, int gx, int gy)
    {
        sstart = new State();
        sstart.x = sx;
        sstart.y = sy;
        sgoal = new State();
        sgoal.x = gx;
        sgoal.y = gy;
        slast = sstart;
        OldInit();
        ComputeShortestPath();
    }

    private void OldInit()
    {
        U = new Heap(10000);
        for(int i = 0; i < S.GetLength(0); i++)
        {
            for (int j = 0; j<S.GetLength(1); j++)
            {
                S[i, j].g = double.PositiveInfinity;
                S[i, j].rhs = double.PositiveInfinity;
            }
        }
        sgoal = S[sgoal.x, sgoal.y];
        sstart = S[sstart.x, sstart.y];
        sgoal.rhs = 0;
        km = 0;
        U.Insert(sgoal, CalculateKey(sgoal));
    }

    public void NextMove()
    {
        // if(sstart.g.isInfinity) then there is no known path
        sstart = MinSuccState(sstart);
        env.MoveTo(new Coordinates(sstart.x, sstart.y));
        LinkedList<Coordinates> obstacleCoord = env.GetObstaclesInVision();
        double oldkm = km;
        State oldslast = slast;
        km += Heuristic(sstart, slast);
        slast = sstart;
        bool change = false;
        foreach (Coordinates c in obstacleCoord)
        {
            State s = S[c.x, c.y];
            if (s.obstacle) continue;// is already known
            change = true;
            s.obstacle = true;
            foreach (State p in s.GetPred())
            {
                UpdateVertex(p);
            }
        }
        if (!change)
        {
            km = oldkm;
            slast = oldslast;
        }
        //DebugMap(S);
        ComputeShortestPath();
        Coordinates botCoordinates = env.GetPosition();
        sstart = S[botCoordinates.x, botCoordinates.y];
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
                if (map[x, y].obstacle)
                {
                    builder.Append("#");
                    continue;
                }
                if(x == botCoordinates.x && y == botCoordinates.y)
                {
                    builder.Append("@");
                    continue;
                }
                if (x == sstart.x && y == sstart.y)
                {
                    builder.Append("%");
                    continue;
                }
                if (x == sgoal.x && y == sgoal.y)
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

    // calculates the key
    /*
    Priority of a vertex = key
    Key – vector with 2 components
        k(s) = [ k1(s);  k2(s) ]
    k1(s) = min(g(s), rhs(s)) + h(s, sstart) + km
    k2(s) = min(g(s), rhs(s)) 
    */
    static K CalculateKey(State s)
    {
        return new K(min(s.g, s.rhs) + Heuristic(s, sstart) + km, min(s.g, s.rhs));
    }

    static double Heuristic(State a, State b)
    {
        return Math.Sqrt(Math.Pow(Math.Abs(a.x - b.x), 2) + Math.Pow(Math.Abs(a.y - b.y), 2));
    }
    
    public void Initialize(int x, int y, IDStarLiteEnvironment env)
    {
        S = new State[x, y];
        this.env = env;
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                S[i, j] = new State();
                S[i, j].x = i;
                S[i, j].y = j;
                S[i, j].g = double.PositiveInfinity;
                S[i, j].rhs = double.PositiveInfinity;
            }
        }
    }

    static void UpdateVertex(State u)
    {
        if (!u.Equals(sgoal))
        {
            u.rhs = MinSucc(u);
        }
        if (U.Contains(u))
        {
            U.Remove(u);
        }
        if (u.g != u.rhs)
        {
            U.Insert(u, CalculateKey(u));
        }
    }

    static State MinSuccState(State u)
    {
        double min = double.PositiveInfinity;
        State n = null;
        foreach (State s in u.GetSucc())
        {
            double val = 1 + s.g;
            if (val <= min && !s.obstacle)
            {
                min = val;
                n = s;
            }
        }
        return n;
    }

    // finds the succesor s' with the min (c(u,s')+g(s'))
    // where cost from u to s' is 1 and returns the value
    static double MinSucc(State u)
    {
        double min = double.PositiveInfinity;
        foreach (State s in u.GetSucc())
        {
            double val = 1 + s.g;
            if (val < min && !s.obstacle) min = val;
        }
        return min;
    }

    static void ComputeShortestPath()
    {
        while (U.TopKey().CompareTo(CalculateKey(sstart)) < 0 || sstart.rhs != sstart.g)
        {
            K kold = U.TopKey();
            State u = U.Pop();
            if (u == null) break;
            if (kold.CompareTo(CalculateKey(u)) < 0)
            {
                U.Insert(u, CalculateKey(u));
            }
            else if (u.g > u.rhs)
            {
                u.g = u.rhs;
                foreach (State s in u.GetPred())
                {
                    UpdateVertex(s);
                }
            }
            else
            {
                u.g = double.PositiveInfinity;
                UpdateVertex(u);
                foreach (State s in u.GetPred())
                {
                    UpdateVertex(s);
                }
            }
        }
    }

    static double min(double a, double b)
    {
        if (b < a) return b;
        return a;
    }
}