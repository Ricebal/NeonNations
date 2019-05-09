using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Soldier.Bot.DStar;

public class SearchBehaviour : MonoBehaviour
{
    public GameObject Bot;
    
    private Coordinates GoalCoordinates = new Coordinates();
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
        Coordinates startCoordinates = m_environment.ConvertGameObjectToCoordinates(Bot.transform);
        GenerateNewDestination(startCoordinates.X, startCoordinates.Y);
        m_bot = GetComponent<Bot>();
    }

    // Update is called once per frame
    void Update()
    {
        Coordinates currentCoordinates = m_environment.ConvertGameObjectToCoordinates(Bot.transform);
        // If the goal hasn't been reached
        if (currentCoordinates.X != GoalCoordinates.X || currentCoordinates.Y != GoalCoordinates.Y)
        {
            NextMove();
        }
        else
        {
            GenerateNewDestination(currentCoordinates.X, currentCoordinates.Y);
        }
    }

    /// <summary>
    /// Makes the DStarLite calculate where the bot should move next and than moves the bot in that direction.
    /// </summary>
    private void NextMove()
    {
        MoveTo(m_dStarLite.NextMove());
        m_dStarLite.SyncBotPosition(m_environment.ConvertGameObjectToCoordinates(Bot.transform));
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
        GoalCoordinates.X = mapX;
        GoalCoordinates.Y = mapY;
        m_dStarLite.RunDStarLite(currentX, currentY, GoalCoordinates.X, GoalCoordinates.Y);
    }
    
    // Moves the bot to the next Coordinate
    private void MoveTo(Coordinates s)
    {
        float horizontal = s.X - Bot.transform.position.x;
        float vertical = s.Y - Bot.transform.position.z;
        // The clamp normalizes the input to a value between -1 and 1 (To represent the players input)
        m_bot.Move(Mathf.Clamp(horizontal, -1f, 1f), Mathf.Clamp(vertical, -1f, 1f));
    }
}
