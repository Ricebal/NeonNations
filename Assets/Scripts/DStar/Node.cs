public class Node
{
    // The cost it takes to get from the starting point to this position. (Also known as g-value)
    public double CostFromStartingPoint;
    // Some kind of potentially better estimated value than the g-value.
    public double Rhs;
    public Tile Content;

    private List<Tile> m_listOfObstacles = new List<Tile>();

    public Node(List<Tile> listOfObstacles)
    {
        m_listOfObstacles = listOfObstacles;

        // Set the Content to Unknown by default.
        Content = Tile.Unknown;
        // Set CostFromStartingPoint and Rhs to PositiveInfinity by default.
        CostFromStartingPoint = double.PositiveInfinity;
        Rhs = double.PositiveInfinity;
    }

    /// <summary>
    /// Returns if a node contains an obstacle or not.
    /// </summary>
    public bool IsObstacle()
    {
        return m_listOfObstacles.Contains(Content);
    }
}
