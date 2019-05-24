using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(Bot))]
public class SearchBehaviour : BotBehaviour
{
    private const float OFFSET_FOR_LINE_CALCULATION = .95f; // A little less than 1. This will prevent the bot from thinking it will collide with an obstacle directly next to it when moving parallel to ithat obstacle.
    private Vector2Int m_goalCoordinates = Vector2Int.zero;
    private Vector2Int m_previousFarthestNode = Vector2Int.zero;
    private GameEnvironment m_environment;
    private DStarLite m_dStarLite;
    private Bot m_bot;

    void Start()
    {
        m_environment = GameEnvironment.CreateInstance(GameObject.Find("GameManager").GetComponent<BoardManager>().GetTileMap(), new List<Tile>() { Tile.Wall, Tile.BreakableWall });
        m_dStarLite = new DStarLite(m_environment, false);
        Vector2Int startCoordinates = m_environment.ConvertGameObjectToCoordinates(gameObject.transform);
        GenerateNewDestination(startCoordinates);
        m_bot = GetComponent<Bot>();
    }

    void FixedUpdate()
    {
        if (!m_active)
        {
            return;
        }
        Vector2Int currentCoordinates = m_environment.ConvertGameObjectToCoordinates(gameObject.transform);
        // If the goal hasn't been reached
        if (currentCoordinates.x != m_goalCoordinates.x || currentCoordinates.y != m_goalCoordinates.y)
        {
            NextMove();
        }
        else
        {
            GenerateNewDestination(currentCoordinates);
        }
    }

    /// <summary>
    /// Makes the DStarLite calculate where the bot should move next and than moves the bot in that direction.
    /// </summary>
    private void NextMove()
    {
        Vector2Int botCoordinate = m_environment.ConvertGameObjectToCoordinates(gameObject.transform);
        List<Vector2Int> coordinatesToTraverse = m_dStarLite.CoordinatesToTraverse();

        Vector2Int farthestReachableNode = m_previousFarthestNode;
        if (m_dStarLite.CoordinatesToTraverseChanged || botCoordinate.Equals(m_previousFarthestNode)) // Perform pathsmoothing if either the path has changed, or if the bot is on the same position as the old farthest node.
        {
            farthestReachableNode = PathSmoothing.FarthestCoordinateToReach(new PointF(gameObject.transform.position.x, gameObject.transform.position.z), coordinatesToTraverse, m_dStarLite.Map, OFFSET_FOR_LINE_CALCULATION);
        }
        m_previousFarthestNode = farthestReachableNode;
        MoveTo(farthestReachableNode);
        m_dStarLite.SyncBotPosition(botCoordinate);
    }

    /// <summary>
    /// Finds a new target for the entity to move/explorer towards.
    /// </summary>
    /// <param name="currentPos">The current position of the entity</param>
    private void GenerateNewDestination(Vector2Int currentPos)
    {
        m_goalCoordinates = GameObject.Find("GameManager").GetComponent<BoardManager>().GetRandomFloorTile();
        m_dStarLite.RunDStarLite(currentPos, m_goalCoordinates);
    }

    /// <summary>
    /// Moves the bot to the next coordinates.
    /// </summary>
    /// <param name="coordinates">The coordinates the bot has to move to as Vector2Int</param>
    private void MoveTo(Vector2Int coordinates)
    {
        float horizontal = coordinates.x - gameObject.transform.position.x;
        float vertical = coordinates.y - gameObject.transform.position.z;

        Vector2 heading = new Vector2(horizontal, vertical);
        heading.Normalize();
        m_bot.Move(heading.x, heading.y);
    }

#if (UNITY_EDITOR)
    private void DebugMap(NavigationGraph map, Vector2Int nodeToReach, List<Vector2Int> coordinatesToTraverse)
    {
        Vector2Int botCoordinates = m_environment.ConvertGameObjectToCoordinates(gameObject.transform);
        StringBuilder builder = new StringBuilder();
        builder.Append('\n');
        for (int y = map.Map[0].Length - 1; y >= 0; y--)
        {
            for (int x = 0; x < map.Map.Length; x++)
            {
                if (map.Map[x][y].IsObstacle())
                {
                    builder.Append("#");
                    continue;
                }
                if (x == botCoordinates.x && y == botCoordinates.y)
                {
                    builder.Append("@");
                    continue;
                }
                if (x == m_goalCoordinates.x && y == m_goalCoordinates.y)
                {
                    builder.Append("X");
                    continue;
                }
                if (x == nodeToReach.x && y == nodeToReach.y)
                {
                    builder.Append("%");
                    continue;
                }
                bool foundInNodes = false;
                foreach (Vector2Int coordinates in coordinatesToTraverse)
                {
                    if (x == coordinates.x && y == coordinates.y)
                    {
                        foundInNodes = true;
                        builder.Append("Q");
                        continue;
                    }
                }
                if (map.Map[x][y].Content == Tile.Unknown)
                {
                    builder.Append("?");
                    continue;
                }
                if (foundInNodes)
                {
                    continue;
                }
                builder.Append("O");
            }
            builder.Append('\n');
        }
        string s = builder.ToString();
        Debug.Log(s);
    }
#endif
}
