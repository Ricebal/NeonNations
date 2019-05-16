using System;
using System.Collections.Generic;

public class Node
{
    // The cost it takes to get from the starting point to this position. (Also known as g-value)
    public double CostFromStartingPoint;
    // Some kind of potentially better estimated value than the g-value.
    public double Rhs;
    /// <summary>
    /// -1 = Unknown
    ///  0 = Open Space
    ///  1 = Wall
    ///  2 = Breakable Wall
    /// </summary>
    public int Content;
    
    public Node()
    {
        // Set the Content to Unknown by default.
        Content = -1;
        // Set CostFromStartingPoint and Rhs to PositiveInfinity by default.
        CostFromStartingPoint = double.PositiveInfinity;
        Rhs = double.PositiveInfinity;
    }

    /// <summary>
    /// Returns if a node contains an obstacle or not.
    /// </summary>
    public bool IsObstacle()
    {
        // 1 and 2 are obstacles
        return Content >= 1 && Content <= 2;
    }
}