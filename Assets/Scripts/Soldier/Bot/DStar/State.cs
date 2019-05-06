using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class State
{
    public int x;
    public int y;
    public double g;
    public double rhs;
    public bool obstacle;

    public bool Equals(State that)
    {
        if (this.x == that.x && this.y == that.y) return true;
        return false;
    }

    public LinkedList<State> GetSucc()
    {
        LinkedList<State> s = new LinkedList<State>();
        int width = DStarLite.S.GetLength(0);
        int height = DStarLite.S.GetLength(1);
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                // Check if point is inside the map
                if (i == 0 && j == 0)
                {
                    continue;
                }
                if (x + i < width && x + i >= 0 && y + j < height && y + j >= 0)
                {
                    if (!(i == 0 ^ j == 0))
                    {
                        if (DStarLite.S[x + i, y].obstacle || DStarLite.S[x, y + j].obstacle)
                        {
                            continue;
                        }
                    }
                    s.AddFirst(DStarLite.S[x + i, y + j]);
                }
            }
        }
        return s;
    }

    public LinkedList<State> GetPred()
    {
        int width = DStarLite.S.GetLength(0);
        int height = DStarLite.S.GetLength(1);
        LinkedList<State> s = new LinkedList<State>();
        State tempState;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                // Check if point is inside the map
                if (i == 0 && j == 0)
                {
                    continue;
                }
                if (x + i < width && x + i >= 0 && y + j < height && y + j >= 0)
                {
                    tempState = DStarLite.S[x + i, y + j];
                    if (!tempState.obstacle) s.AddFirst(tempState);
                }
            }
        }
        return s;
    }
}