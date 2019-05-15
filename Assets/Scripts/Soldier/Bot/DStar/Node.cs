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

    public Node(int x, int y)
    {
        X = x;
        Y = y;
    }

    public bool Equals(Node that)
    {
        if (this.X == that.X && this.Y == that.Y) return true;
        return false;
    }
}