﻿using Assets.Scripts.Soldier.Bot.DStar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class DStarLite
{
    public Node[][] Map;

    private GameEnvironment m_environment;
    private Heap m_heap;
    private double m_travelledDistance;
    private List<Node> m_previousNodesToTraverse = new List<Node>();
    private Node m_goal;
    private Node m_previousGoal;
    
    private Node m_start;
    private Node m_previousStart;
    
    public DStarLite(GameEnvironment environment)
    {
        m_environment = environment;
    }

    /// <summary>
    /// Set's up the Algorithm.
    /// </summary>
    /// <param name="startX">The x of the start-position</param>
    /// <param name="startY">The y of the start-position</param>
    /// <param name="goalY">The y of the goal-position</param>
    /// <param name="goalX">The x of the goal-position</param>
    public void RunDStarLite(int startX, int startY, int goalX, int goalY)
    {
        Reset(startX, startY, goalX, goalY);
        // Calculate initial path
        ComputeShortestPath();
    }

    private void Reset(int startX, int startY, int goalX, int goalY)
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
        m_goal = Map[goalX][goalY];
        m_start = Map[startX][startY];
        m_previousStart = m_start;
        m_goal.Rhs = 0;
        m_heap.Insert(m_goal, CalculatePriority(m_goal));
        m_travelledDistance = 0;
    }

    /// <summary>
    /// Needs to be called before the RunDStarLite-function.
    /// Generates an empty map for the bot
    /// </summary>
    /// <param name="width">The amount of columns of the map</param>
    /// <param name="height">The amount of rows of the map</param>
    /// <param name="env">Any class that extends IDStarLiteEnvironment. Used for moving the entity and getting the surroundings</param>
    /// <param name="knowMap">Set to true if the algorithm should know the full map</param>
    public void GenerateNodeMap(bool knowMap)
    {
        int[][] completeMap = m_environment.GetMap();
        Map = new Node[completeMap.Length][];
        for (int i = 0; i < Map.Length; i++)
        {
            Map[i] = new Node[completeMap[0].Length];
        }
        for (int i = 0; i < Map.Length; i++)
        {
            for (int j = 0; j < Map[0].Length; j++)
            {
                Map[i][j] = new Node(this);
                Map[i][j].X = i;
                Map[i][j].Y = j;
                Map[i][j].CostFromStartingPoint = double.PositiveInfinity;
                Map[i][j].Rhs = double.PositiveInfinity;

                if (knowMap && completeMap[i][j] == 1)
                {
                    Map[i][j].Obstacle = true;
                }
            }
        }
    }

    public Node NextNodeToTraverse()
    {
        return NextMove(CheckForMapChanges());
    }

    private bool CheckForMapChanges()
    {
        // Check if bot sees new obstacles
        LinkedList<Coordinates> obstacleCoord = m_environment.GetObstaclesInVision();

        bool change = false;
        foreach (Coordinates c in obstacleCoord)
        {
            Node node = Map[c.X][c.Y];
            // If the obstacle was not previously known
            if (!node.Obstacle)
            {
                change = true;
                node.Obstacle = true;
                foreach (Node surroundingNode in node.GetSurroundingOpenSpaces())
                {
                    UpdateVertex(surroundingNode);
                }
            }
        }

        ComputeShortestPath();
        return change;
    }

    /// <summary>
    /// Returns a list of all the nodes that need to be traversed to reach the goal
    /// </summary>
    public List<Node> NodesToTraverse()
    {
        List<Node> nodesToTraverse = new List<Node>();
        // Check if the map has changed
        bool mapHasChanged = CheckForMapChanges();

        Node previousFirstNode = m_previousNodesToTraverse.FirstOrDefault();
        // If nothing has changed AND 
        // The bot is still in the same tile AND
        // The goal hasn't been reached THAN
        // The shortest path does not need to be recalculated.
        if(!mapHasChanged && m_start == previousFirstNode && m_goal == m_previousGoal)
        {
            return m_previousNodesToTraverse;
        }
        m_previousGoal = m_goal;
        do
        {
            nodesToTraverse.Add(NextMove(mapHasChanged));
        } while (nodesToTraverse[nodesToTraverse.Count-1] != m_goal);
        m_previousNodesToTraverse = nodesToTraverse;
        return nodesToTraverse;
    }

    /// <summary>
    /// Retrieve the new surrounding knowledge and applies this to the map.
    /// Calculate the (new) shortest path.
    /// Tells the entity where to move next.
    /// </summary>
    private Node NextMove(bool mapHasChanged)
    {
        // Update position of bot
        m_start = MinCostNode(m_start);

        if (mapHasChanged)
        {
            // Calculates a new TravelDistance
            m_travelledDistance += Heuristic(m_start, m_previousStart);
            // Saves the new start for calculating the Heuristics
            m_previousStart = m_start;
        }

        return m_start;
    }

    /// <summary>
    /// Set's the right position of the bot for m_start.
    /// </summary>
    /// <param name="botCoordinates">The current coordinates of the bot</param>
    public void SyncBotPosition(Coordinates botCoordinates)
    {
        m_start = Map[botCoordinates.X][botCoordinates.Y];
    }

    // --------------------------------------------------------------------------------------------
    // Debug functions
    // --------------------------------------------------------------------------------------------

    private void DebugHeap()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append('\n');
        builder.Append($"Goal = {m_goal.X}, {m_goal.Y}\n");
        builder.Append($"BotPosition = {m_start.X}, {m_start.Y}\n");
        builder.Append('\n');
        builder.Append(m_heap.ToString());
        Debug.Log(builder.ToString());
    }
    
    ///<summary>
    /// Calculates the key.
    /// Priority of a vertex = key.
    /// Key – vector with 2 components.
    /// k(s) = [ k1(s);  k2(s) ].
    /// k1(s) = min(g(s), rhs(s)) + h(s, sstart) + km.
    /// k2(s) = min(g(s), rhs(s)).
    ///</summary>
    private PriorityKey CalculatePriority(Node node)
    {
        return new PriorityKey(Math.Min(node.CostFromStartingPoint, node.Rhs) + Heuristic(node, m_start) + m_travelledDistance, Math.Min(node.CostFromStartingPoint, node.Rhs));
    }

    /// <summary>
    /// Calculates the Heuristics for traveling between two points
    /// </summary>
    private double Heuristic(Node pointA, Node pointB)
    {
        float xDistance = Math.Abs(pointA.X - pointB.X);
        float yDistance = Math.Abs(pointA.Y - pointB.Y);
        return xDistance + yDistance;
    }

    /// <summary>
    /// Re-evaluates the Rhs-value of a State
    /// </summary>
    private void UpdateVertex(Node node)
    {
        if (!node.Equals(m_goal))
        {
            node.Rhs = MinCost(node);
        }
        if (m_heap.Contains(node))   // To prevent any copies
        {
            m_heap.Remove(node);
        }
        if (node.CostFromStartingPoint != node.Rhs)
        {
            m_heap.Insert(node, CalculatePriority(node));
        }
    }

    /// <summary>
    /// Returns the node that has the lowest cost
    /// </summary>
    /// <returns>
    /// The lowest cost node
    /// </returns>
    private Node MinCostNode(Node node)
    {
        double min = double.PositiveInfinity;
        Node returnNode = null;
        foreach (Node s in node.GetSurroundingOpenSpaces())
        {
            double val = 1 + s.CostFromStartingPoint;
            if (val <= min)
            {
                min = val;
                returnNode = s;
            }
        }
        return returnNode;
    }

    /// <summary>
    /// Gets the minimum distance to travel to this node
    /// </summary>
    /// <returns>
    /// Double containing the minimum distance
    /// </returns>
    private double MinCost(Node node)
    {
        double min = double.PositiveInfinity;
        foreach (Node n in node.GetSurroundingOpenSpaces())
        {
            double val = 1 + n.CostFromStartingPoint; // Add's 1 to the CostFromStartingPoint since it's one more move
            if (val < min) // If the value is smaller than the minimum value found
            {
                min = val;
            }
        }
        return min;
    }

    private void ComputeShortestPath()
    {
        // While the top state of the Heap has a higher priority than the start state OR start.rhs is not the cost of start
        while (m_heap.TopKey().CompareTo(CalculatePriority(m_start)) < 0 || m_start.Rhs != m_start.CostFromStartingPoint)
        {
            // Gets the priority key op the top
            PriorityKey oldKey = m_heap.TopKey();
            // Gets the node of the top
            Node node = m_heap.Pop();
            if (node == null) break; // Heap is empty

            // Gets new key based on current botPosition
            PriorityKey newKey = CalculatePriority(node);
            if (oldKey.CompareTo(newKey) < 0) // The node has a lower priority than before
            {
                m_heap.Insert(node, newKey);
            }
            else if (node.CostFromStartingPoint > node.Rhs) // The g-value wasn't optimally calculated
            {
                node.CostFromStartingPoint = node.Rhs;
                foreach (Node surroundingState in node.GetSurroundingOpenSpaces())
                {
                    UpdateVertex(surroundingState);
                }
            }
            else
            {
                node.CostFromStartingPoint = double.PositiveInfinity;
                UpdateVertex(node);
                foreach (Node surroundingState in node.GetSurroundingOpenSpaces())
                {
                    UpdateVertex(surroundingState);
                }
            }
        }
    }
}