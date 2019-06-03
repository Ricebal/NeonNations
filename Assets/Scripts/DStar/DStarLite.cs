using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DStarLite
{
    public NavigationGraph Map;
    public Vector2Int Start;
    public bool CoordinatesToTraverseChanged = true;

    private GameEnvironment m_environment;
    private MinHeap m_heap;
    private int m_travelledDistance;
    private List<Vector2Int> m_previousCoordinatesToTraverse = new List<Vector2Int>();
    private Vector2Int m_goal;
    private Vector2Int m_previousGoal;
    private Vector2Int m_previousStart;

    public DStarLite(GameEnvironment environment, bool knowMap)
    {
        m_environment = environment;
        GenerateNodeMap(knowMap);
    }

    /// <summary>
    /// Sets up the Algorithm.
    /// </summary>
    /// <param name="start">The start-position</param>
    /// <param name="goal">The goal-position</param>
    public void RunDStarLite(Vector2Int start, Vector2Int goal)
    {
        Reset(start, goal);
        // Calculate initial path
        ComputeShortestPath();
    }

    /// <summary>
    /// Resets the algorithm for a new location.
    /// </summary>
    private void Reset(Vector2Int start, Vector2Int goal)
    {
        //Creates an heap the size of the map
        m_heap = new MinHeap(Map.GetSize());
        Map.Reset();
        m_goal = goal;
        Start = start;
        m_previousStart = Start;
        Map.GetNode(m_goal).Rhs = 0;
        m_heap.Insert(m_goal, CalculatePriority(m_goal));
        m_travelledDistance = 0;
    }

    /// <summary>
    /// Needs to be called before the RunDStarLite-function.
    /// Generates an empty map for the bot
    /// </summary>
    /// <param name="knowMap">Set to true if the algorithm should know the full map</param>
    private void GenerateNodeMap(bool knowMap)
    {
        Tile[][] completeMap = m_environment.GetMap();
        Map = new NavigationGraph(completeMap, knowMap, m_environment.GetList());
    }

    /// <summary>
    /// Returns if there are changes to the map or not.
    /// </summary>
    private bool CheckForMapChanges()
    {
        // Check if bot sees new obstacles
        LinkedList<Vector2Int> coordinatesInSight = m_environment.GetIlluminatedCoordinates(Start);

        bool change = false;
        foreach (Vector2Int coordinates in coordinatesInSight)
        {
            // What the bot knows of the node
            Node knownNode = Map.GetNode(coordinates.x, coordinates.y);
            // What the node actually is
            Tile actualNodeContent = m_environment.GetNode(coordinates);
            // If the obstacle was not previously known or the obstacle has been removed
            if (knownNode.Content != actualNodeContent)
            {
                change = true;
                knownNode.Content = actualNodeContent;
                foreach (Vector2Int surroundingCoordinates in Map.GetSurroundingOpenSpaces(coordinates))
                {
                    UpdateVertex(surroundingCoordinates);
                }
            }
        }

        ComputeShortestPath();
        return change;
    }

    /// <summary>
    /// Returns a list of all the nodes that need to be traversed to reach the goal
    /// </summary>
    public List<Vector2Int> CoordinatesToTraverse()
    {
        if (Map.GetNode(Start).IsObstacle())  // Prevent from crashing when bot is inside a wall
        {
            return new List<Vector2Int>();
        }
        List<Vector2Int> CoordinatesToTraverse = new List<Vector2Int>();

        // If the map has changed or not
        bool mapChanged = CheckForMapChanges();

        Vector2Int previousFirstCoordinates = m_previousCoordinatesToTraverse.FirstOrDefault();

        CoordinatesToTraverseChanged = true;
        // If the map has not been changed AND 
        // The bot is still in the same tile AND
        // The goal hasn't been reached THEN
        // The shortest path does not need to be recalculated.
        if (!mapChanged && Start == previousFirstCoordinates && m_goal == m_previousGoal)
        {
            CoordinatesToTraverseChanged = false;
            return m_previousCoordinatesToTraverse;
        }
        m_previousGoal = m_goal;
        Vector2Int coordinateToTraverse = new Vector2Int();
        for (int i = 0; i < Map.Map.Length + Map.Map[0].Length; i++)
        {
            coordinateToTraverse = NextMove(mapChanged);
            CoordinatesToTraverse.Add(coordinateToTraverse);

            // If the goal coordinates have been reached
            if (coordinateToTraverse.Equals(m_goal))
            {
                break;
            }
        }
        m_previousCoordinatesToTraverse = CoordinatesToTraverse;
        return CoordinatesToTraverse;
    }

    /// <summary>
    /// Retrieve the new surrounding knowledge and applies this to the map.
    /// Calculate the (new) shortest path.
    /// Tells the entity where to move next.
    /// </summary>
    private Vector2Int NextMove(bool mapHasChanged)
    {
        // Update position of bot
        Start = MinCostCoordinates(Start);

        if (mapHasChanged)
        {
            // Calculates a new TravelDistance
            m_travelledDistance += Heuristic(Start, m_previousStart);
            // Saves the new start for calculating the Heuristics
            m_previousStart = Start;
        }

        return Start;
    }

    /// <summary>
    /// Sets the right position of the bot for m_start.
    /// </summary>
    /// <param name="botCoordinates">The current coordinates of the bot</param>
    public void SyncBotPosition(Vector2Int botCoordinates)
    {
        Start = botCoordinates;
    }

    ///<summary>
    /// Calculates the key.
    /// Priority of a vertex = key.
    /// Key – vector with 2 components.
    /// k(s) = [ k1(s);  k2(s) ].
    /// k1(s) = min(g(s), rhs(s)) + h(s, sstart) + km.
    /// k2(s) = min(g(s), rhs(s)).
    ///</summary>
    private PriorityKey CalculatePriority(Vector2Int coordinates)
    {
        Node node = Map.GetNode(coordinates);
        return new PriorityKey(Math.Min(node.CostFromStartingPoint, node.Rhs) + Heuristic(coordinates, Start) + m_travelledDistance, Math.Min(node.CostFromStartingPoint, node.Rhs));
    }

    /// <summary>
    /// Calculates the Heuristics for traveling between two points
    /// </summary>
    private int Heuristic(Vector2Int pointA, Vector2Int pointB)
    {
        int xDistance = Math.Abs(pointA.x - pointB.x);
        int yDistance = Math.Abs(pointA.y - pointB.y);
        return xDistance + yDistance;
    }

    /// <summary>
    /// Re-evaluates the Rhs-value of a State
    /// </summary>
    private void UpdateVertex(Vector2Int coordinates)
    {
        Node node = Map.GetNode(coordinates);
        if (!coordinates.Equals(m_goal))
        {
            node.Rhs = MinCost(coordinates);
        }
        if (m_heap.Contains(coordinates))   // To prevent any copies
        {
            m_heap.Remove(coordinates);
        }
        if (node.CostFromStartingPoint != node.Rhs)
        {
            m_heap.Insert(coordinates, CalculatePriority(coordinates));
        }
    }

    /// <summary>
    /// Returns the node that has the lowest cost
    /// </summary>
    /// <returns>
    /// The lowest cost node
    /// </returns>
    private Vector2Int MinCostCoordinates(Vector2Int coordinates)
    {
        double min = double.PositiveInfinity;
        Vector2Int returnCoordinates = new Vector2Int();
        foreach (Vector2Int surroundingCoordinates in Map.GetSurroundingOpenSpaces(coordinates))
        {
            Node node = Map.GetNode(surroundingCoordinates);
            double val = 1 + node.CostFromStartingPoint;    // Adds 1 to the CostFromStartingPoint since it is one more move to reach this point from a point next to it.
            if (val <= min)
            {
                min = val;
                returnCoordinates = surroundingCoordinates;
            }
        }
        return returnCoordinates;
    }

    /// <summary>
    /// Gets the minimum distance to travel to this node
    /// </summary>
    /// <returns>
    /// Double containing the minimum distance
    /// </returns>
    private double MinCost(Vector2Int coordinates)
    {
        double min = double.PositiveInfinity;
        foreach (Vector2Int surroundingCoordinates in Map.GetSurroundingOpenSpaces(coordinates))
        {
            Node node = Map.GetNode(surroundingCoordinates);
            double val = 1 + node.CostFromStartingPoint; // Adds 1 to the CostFromStartingPoint since it is one more move to reach this point from a point next to it.
            if (val < min) // If the value is smaller than the minimum value found
            {
                min = val;
            }
        }
        return min;
    }

    /// <summary>
    /// Computes the shortest path from the current location to the goal location.
    /// </summary>
    private void ComputeShortestPath()
    {
        Node startNode = Map.GetNode(Start);
        if (startNode.IsObstacle()) // Prevent from crashing when bot is inside a wall
        {
            return;
        }

        for (int i = 0; i < Map.Map.Length * Map.Map[0].Length; i++)
        {
            // Gets the priority key from the top
            PriorityKey oldKey = m_heap.TopKey();
            // Gets the node of the top
            Vector2Int coordinates = m_heap.Pop();
            if (coordinates == null)
            {
                break; // Heap is empty
            }
            Node node = Map.GetNode(coordinates);

            // Gets new key based on current position that is being calculated
            PriorityKey newKey = CalculatePriority(coordinates);
            if (oldKey.CompareTo(newKey) < 0) // The node has a lower priority than before
            {
                m_heap.Insert(coordinates, newKey);
            }
            else if (node.CostFromStartingPoint > node.Rhs) // The g-value wasn't optimally calculated
            {
                node.CostFromStartingPoint = node.Rhs;      // Set the g-value to the calculated Rhs-value
                foreach (Vector2Int surroundingCoordinates in Map.GetSurroundingOpenSpaces(coordinates))    // Update the Rhs-value of the surrounding nodes
                {
                    UpdateVertex(surroundingCoordinates);
                }
            }
            else  // Re-calculate the Rhs-value of the current node and its surrounding nodes
            {
                node.CostFromStartingPoint = double.PositiveInfinity;
                UpdateVertex(coordinates);
                foreach (Vector2Int surroundingNodes in Map.GetSurroundingOpenSpaces(coordinates))  // Update the Rhs-value of the surrounding nodes
                {
                    UpdateVertex(surroundingNodes);
                }
            }
            // íf the top state of the Heap does not have a higher priority than the start state AND start.rhs is the cost of start
            if (!(m_heap.TopKey().CompareTo(CalculatePriority(Start)) < 0 || startNode.Rhs != startNode.CostFromStartingPoint))
            {
                break;
            }
        }
    }
}
