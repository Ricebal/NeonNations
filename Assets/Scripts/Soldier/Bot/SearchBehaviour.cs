using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Soldier.Bot.DStar;
using System.Text;
using System.Drawing;

public class SearchBehaviour : MonoBehaviour
{
    public GameObject Bot;

    private const float m_offsetForLineCalculation = .95f; // A little less than 1. This will prevent the bot from thinking it will collide with an obstacle directly next to it when moving parallel to ithat obstacle.
    private Vector2Int GoalCoordinates = new Vector2Int();
    private GameEnvironment m_environment;
    private DStarLite m_dStarLite;
    private Bot m_bot;

    // Start is called before the first frame update
    void Start()
    {
        m_environment = new GameEnvironment(Bot);
        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
        m_dStarLite = new DStarLite(m_environment);
        m_dStarLite.GenerateNodeMap(false);
        Vector2Int startCoordinates = m_environment.ConvertGameObjectToCoordinates(Bot.transform);
        GenerateNewDestination(startCoordinates.x, startCoordinates.y);
        m_bot = GetComponent<Bot>();
    }
    
    void FixedUpdate()
    {
        Vector2Int currentCoordinates = m_environment.ConvertGameObjectToCoordinates(Bot.transform);
        // If the goal hasn't been reached
        if (currentCoordinates.x != GoalCoordinates.x || currentCoordinates.y != GoalCoordinates.y)
        {
            NextMove();
        }
        else
        {
            GenerateNewDestination(currentCoordinates.x, currentCoordinates.y);
        }
    }

    /// <summary>
    /// Makes the DStarLite calculate where the bot should move next and than moves the bot in that direction.
    /// </summary>
    private void NextMove()
    {
        List<Vector2Int> coordinatesToTraverse = new List<Vector2Int>();
        Vector2Int botCoordinate = m_environment.ConvertGameObjectToCoordinates(Bot.transform);
        coordinatesToTraverse.AddRange(m_dStarLite.CoordinatesToTraverse());

        Vector2Int farthestReachableNode = PathSmoothing.FarthestCoordinateToReach(new PointF(Bot.transform.position.x, Bot.transform.position.z), coordinatesToTraverse, m_dStarLite.Map, m_offsetForLineCalculation);
        MoveTo(farthestReachableNode);
        m_dStarLite.SyncBotPosition(botCoordinate);
    }

    /// <summary>
    /// Finds a new target for the entity to move/explorer towards.
    /// </summary>
    /// <param name="currentX">The current x-position of the entity</param>
    /// <param name="currentY">The current y-position of the entity</param>
    private void GenerateNewDestination(int currentX, int currentY)
    {
        bool found = false;
        int[][] map = m_environment.GetMap();
        int mapX = 0;
        int mapY = 0;
        while (!found)
        {
            mapX = UnityEngine.Random.Range(0, map.Length);
            mapY = UnityEngine.Random.Range(0, map[0].Length);
            if (map[mapX][mapY] == 0)
            {
                found = true;
            }
        }
        GoalCoordinates.x = mapX;
        GoalCoordinates.y = mapY;
        m_dStarLite.RunDStarLite(currentX, currentY, GoalCoordinates.x, GoalCoordinates.y);
    }
    
    // Moves the bot to the next Coordinate
    private void MoveTo(Vector2Int coordinates)
    {
        float horizontal = coordinates.x - Bot.transform.position.x;
        float vertical = coordinates.y - Bot.transform.position.z;

        Vector2 heading = new Vector2(horizontal, vertical);
        heading.Normalize();
        m_bot.Move(heading.x, heading.y);
    }

    private void DebugMap(NavigationGraph map, Vector2Int nodeToReach, List<Vector2Int> coordinatesToTraverse)
    {
        Vector2Int botCoordinates = m_environment.ConvertGameObjectToCoordinates(Bot.transform);
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
                if (x == GoalCoordinates.x && y == GoalCoordinates.y)
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
                foreach(Vector2Int coordinates in coordinatesToTraverse)
                {
                    if(x == coordinates.x && y == coordinates.y)
                    {
                        foundInNodes = true;
                        builder.Append("Q");
                        continue;
                    }
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
}
