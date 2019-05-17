using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace Assets.Scripts.Soldier.Bot.DStar
{
    public class DStarLite
    {
        public NavigationGraph Map;
        public bool CoordinatesToTraverseChanged = true;

        private GameEnvironment m_environment;
        private MinHeap m_heap;
        private int m_travelledDistance;
        private List<Vector2Int> m_previousCoordinatesToTraverse = new List<Vector2Int>();
        private Vector2Int m_goal;
        private Vector2Int m_previousGoal;

        private Vector2Int m_start;
        private Vector2Int m_previousStart;

        public DStarLite(GameEnvironment environment, bool knowMap)
        {
            m_environment = environment;
            GenerateNodeMap(knowMap);
        }

        /// <summary>
        /// Sets up the Algorithm.
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
            m_heap = new MinHeap(Map.GetSize());
            Map.Reset();
            m_goal = new Vector2Int(goalX, goalY);
            m_start = new Vector2Int(startX, startY);
            m_previousStart = m_start;
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
            int[][] completeMap = m_environment.GetMap();
            Map = new NavigationGraph(completeMap, knowMap);
        }

        /// <summary>
        /// Returns if there are changes to the map or not.
        /// </summary>
        private bool CheckForMapChanges()
        {
            // Check if bot sees new obstacles
            LinkedList<Vector2Int> coordinatesInSight = m_environment.GetPositionsInVision();

            bool change = false;
            foreach (Vector2Int coordinates in coordinatesInSight)
            {
                // What the bot knows of the node
                Node knownNode = Map.GetNode(coordinates.x, coordinates.y);
                // What the node actually is
                int actualNodeContent = m_environment.GetNode(coordinates.x, coordinates.y);
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
            if (Map.GetNode(m_start).IsObstacle())  // Prevent from crashing when bot is inside a wall
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
            if (!mapChanged && m_start == previousFirstCoordinates && m_goal == m_previousGoal)
            {
                CoordinatesToTraverseChanged = false;
                return m_previousCoordinatesToTraverse;
            }
            m_previousGoal = m_goal;
            Vector2Int coordinateToTraverse = new Vector2Int();
            do
            {
                coordinateToTraverse = NextMove(mapChanged);
                CoordinatesToTraverse.Add(coordinateToTraverse);
            } while (!coordinateToTraverse.Equals(m_goal));
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
            m_start = MinCostCoordinates(m_start);

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
        /// Sets the right position of the bot for m_start.
        /// </summary>
        /// <param name="botCoordinates">The current coordinates of the bot</param>
        public void SyncBotPosition(Vector2Int botCoordinates)
        {
            m_start = botCoordinates;
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
            return new PriorityKey(Math.Min(node.CostFromStartingPoint, node.Rhs) + Heuristic(coordinates, m_start) + m_travelledDistance, Math.Min(node.CostFromStartingPoint, node.Rhs));
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

        private void ComputeShortestPath()
        {
            Node startNode = Map.GetNode(m_start);
            if (startNode.IsObstacle()) // Prevent from crashing when bot is inside a wall
            {
                return;
            }
            // While the top state of the Heap has a higher priority than the start state OR start.rhs is not the cost of start
            while (m_heap.TopKey().CompareTo(CalculatePriority(m_start)) < 0 || startNode.Rhs != startNode.CostFromStartingPoint)
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
            }
        }
    }
}