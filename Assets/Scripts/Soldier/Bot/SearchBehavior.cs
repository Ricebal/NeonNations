using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SearchBehavior : MonoBehaviour, IDStarLiteEnvironment
{
    public GameObject Bot;

    //These variables can be deleted after the map is integrated
    public const int Columns = 30;
    public const int Rows = 22;
    
    private Coordinates GoalCoordinates = new Coordinates();
    private Coordinates StartCoordinates;
    private bool[,] m_tileMap = new bool[Columns, Rows];
    private bool m_completed = false;
    private DStarLite m_dStarLite;
    private Bot m_bot;

    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();    // used temperarly to get the right map-layout. This should later be replaced by a getMapLayout in the BoardManager or something like that
        UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);
        m_dStarLite = new DStarLite();
        m_dStarLite.GenerateEmptyMap(Columns, Rows, this);
        StartCoordinates = ConvertGameObjectToCoordinates(Bot.transform);
        FindNewDestination(StartCoordinates.X, StartCoordinates.Y);
        m_bot = GetComponent<Bot>();
        m_dStarLite.RunDStarLite(StartCoordinates.X, StartCoordinates.Y, GoalCoordinates.X, GoalCoordinates.Y);
    }

    // Update is called once per frame
    void Update()
    {
        Coordinates currentCoordinates = ConvertGameObjectToCoordinates(Bot.transform);
        // If the goal hasn't been reached
        if (currentCoordinates.X != GoalCoordinates.X || currentCoordinates.Y != GoalCoordinates.Y)
        {
            m_dStarLite.NextMove();
        }
        else
        {
            FindNewDestination(currentCoordinates.X, currentCoordinates.Y);
        }
    }

    /// <summary>
    /// Finds a new target for the entity to move/explorer towards.
    /// </summary>
    /// <param name="currentX">The current x-position of the entity</param>
    /// <param name="currentY">The current y-position of the entity</param>
    private void FindNewDestination(int currentX, int currentY)
    {
        bool found = false;
        int mapX = 0;
        int mapY = 0;
        while (!found)
        {
            mapX = UnityEngine.Random.Range(0, Columns);
            mapY = UnityEngine.Random.Range(0, Rows);
            if (m_tileMap[mapX, mapY])
            {
                found = true;
            }
        }
        GoalCoordinates.X = mapX;
        GoalCoordinates.Y = mapY;
        m_dStarLite.RunDStarLite(currentX, currentY, GoalCoordinates.X, GoalCoordinates.Y);
    }

    void GenerateMap()
    {
        m_tileMap = new bool[Columns, Rows];

        for (int x = 0; x < Columns; x++)
        {
            for (int y = 0; y < Rows; y++)
            {
                if (// rooms
                    x == 0 && y == 0
                    || x >= 0 && x <= 9 && y >= 16 && y <= 21
                    || x >= 1 && x <= 7 && y >= 0 && y <= 5
                    || x >= 11 && x <= 15 && y >= 4 && y <= 10
                    || x >= 16 && x <= 20 && y >= 13 && y <= 18
                    || x >= 23 && x <= 29 && y >= 3 && y <= 10
                    || x >= 24 && x <= 27 && y >= 17 && y <= 21
                    //|| x >= 0 && x <= 20 && y >= 7 && y <= 20
                    // corridors
                    || x >= 1 && x <= 2 && y >= 12 && y <= 15
                    || x >= 3 && x <= 6 && y >= 12 && y <= 13
                    || x >= 5 && x <= 6 && y >= 6 && y <= 13
                    || x >= 7 && x <= 10 && y >= 8 && y <= 9
                    || x >= 8 && x <= 24 && y >= 0 && y <= 1
                    || x >= 10 && x <= 15 && y >= 17 && y <= 18
                    || x >= 13 && x <= 14 && y >= 2 && y <= 3
                    || x >= 16 && x <= 22 && y >= 8 && y <= 9
                    || x >= 17 && x <= 18 && y >= 19 && y <= 21
                    || x >= 17 && x <= 18 && y >= 10 && y <= 12
                    || x >= 19 && x <= 25 && y >= 20 && y <= 21
                    || x >= 23 && x <= 24 && y == 2
                    || x >= 25 && x <= 26 && y >= 11 && y <= 16)
                {
                    m_tileMap[x, y] = true;
                }
            }
        }
    }

    // Moves the bot to the next Coordinate
    void IDStarLiteEnvironment.MoveTo(Coordinates s)
    {
        float horizontal = s.X - Bot.transform.position.x;
        float vertical = s.Y - Bot.transform.position.z;
        // The clamp normalizes the input to a value between -1 and 1 (To represent the players input)
        m_bot.Move(Mathf.Clamp((float)horizontal, -1f, 1f), Mathf.Clamp((float)vertical, -1f, 1f));
    }

    // Return the obstacles the bot should be able to see
    LinkedList<Coordinates> IDStarLiteEnvironment.GetObstaclesInVision()
    {
        // First get all coordinates that are illuminated
        LinkedList<Coordinates> currentCoordinatesInSight = GetIlluminatedCoordinates();
        // Than check the map-layout to see if the illuminated parts are walls or empty spaces
        FilterCoordinatesWithObstacle(ref currentCoordinatesInSight);
        // Return the result
        return currentCoordinatesInSight;
    }

    /// <summary>
    /// Checks if the Coordinates in sight are walls or empty spaces.
    /// </summary>
    /// <param name="coordinates">The Coordinates that should be checked</param>
    private void FilterCoordinatesWithObstacle(ref LinkedList<Coordinates> coordinates)
    {
        LinkedList<Coordinates> coordinatesToDelete = new LinkedList<Coordinates>();
        foreach (Coordinates coordinate in coordinates)
        {
            // If the coordinates are outside the map they shouldn't be checked (to prevent out of index exception)
            if(coordinate.X < 0 || coordinate.Y < 0 || coordinate.X >= Columns || coordinate.Y >= Rows)
            {
                coordinatesToDelete.AddLast(coordinate);
                continue;
            }
            // If the coordinates are inside the map, check if the coordinate contains an obstacle
            if(m_tileMap[coordinate.X, coordinate.Y])
            {
                coordinatesToDelete.AddLast(coordinate);
            }
        }
        // Delete the coordintes that contain no obstacles
        foreach (Coordinates coordinate in coordinatesToDelete)
        {
            coordinates.Remove(coordinate);
        }
    }

    /// <summary>
    /// Converts the transform of a gameobject to coordinates on the map
    /// </summary>
    /// <param name="transform">The transform of the gameobject from which you want the coordinates</param>
    private Coordinates ConvertGameObjectToCoordinates(Transform transform)
    {
        return new Coordinates((int)Math.Round(transform.position.x, 0, MidpointRounding.AwayFromZero), (int)Math.Round(transform.position.z, 0, MidpointRounding.AwayFromZero));
    }

    /// <summary>
    /// Get all Coordinates that are illuminated by light on the bot's screen
    /// </summary>
    private LinkedList<Coordinates> GetIlluminatedCoordinates()
    {
        LinkedList<Coordinates> illuminatedCoordinates = GetCoordinatesAroundBot(); // Because of the spotlight around the bot
        GetCoordinatesAroundLights(ref illuminatedCoordinates); // All other lights visible on screen
        return illuminatedCoordinates;
    }

    /// <summary>
    /// Get the Coordinates that are illuminated by the spotlight on the bot
    /// </summary>
    private LinkedList<Coordinates> GetCoordinatesAroundBot()
    {
        LinkedList<Coordinates> currentCoordinatesInSight = new LinkedList<Coordinates>();
        Coordinates botCoordinate = ConvertGameObjectToCoordinates(Bot.transform);
        return GetCoordinatesInRange(botCoordinate, 2, currentCoordinatesInSight);
    }

    /// <summary>
    /// returns the positino of the bot
    /// </summary>
    public Coordinates GetPosition()
    {
        return ConvertGameObjectToCoordinates(Bot.transform);
    }

    /// <summary>
    /// Returns a linked list containing all map-coordinates that should be illuminated by the object
    /// </summary>
    /// <param name="objectCoordinates">The Coordinates of the object you want to check the light from</param>
    /// <param name="range">The range of the light that's emitted by the object</param>
    /// <param name="coordinatesInRange">The Coordinates that have to be checked</param>
    private LinkedList<Coordinates> GetCoordinatesInRange(Coordinates objectCoordinates, int range, LinkedList<Coordinates> coordinatesInRange)
    {
        range = Mathf.Clamp(range, 0, 3);
        for (int i = -range; i < range; i++)
        {
            for (int j = -range; j < range; j++)
            {
                int x = objectCoordinates.X + i;
                int y = objectCoordinates.Y + j;
                // Check if coordinate is already added to the list
                if (!coordinatesInRange.Any(coordinate => coordinate.X == x && coordinate.Y == y))
                {
                    coordinatesInRange.AddLast(new Coordinates(x, y));
                }
            }
        }
        return coordinatesInRange;
    }

    /// <summary>
    /// Get coordinates that are illuminated by lights
    /// </summary>
    /// <param name="illuminatedCoordinates">LinkedListist to which the illuminated Coordinates should be added</param>
    private void GetCoordinatesAroundLights(ref LinkedList<Coordinates> illuminatedCoordinates)
    {
        List<Light> lightsInSight = GetLights();

        // Get all illuminated coordinates
        foreach(Light light in lightsInSight)
        {
            GetCoordinatesInRange(ConvertGameObjectToCoordinates(light.transform), (int)Math.Floor(light.range), illuminatedCoordinates);
        }
    }

    /// <summary>
    /// Get all lights in view of the bot
    /// </summary>
    private List<Light> GetLights()
    {
        // Get's all lights in the scene 
        Light[] lights = FindObjectsOfType<Light>();
        // Filter lights by point light and check if the position is inside the view
        List<Light> pointLights = lights.Where(light => light.type == UnityEngine.LightType.Point && CheckIfPositionIsInsideView(light.transform.position)/* && light.transform.parent.tag != "Player"*/).ToList();
        // Remove point lights of the players and bots
        for (int i = 0; i < pointLights.Count(); i++)
        {
            string tag = pointLights[i].transform.parent.tag;
            Console.WriteLine(tag);//
        }
        return pointLights.Where(light => light.transform.parent.tag != "Player").ToList();
    }

    /// <summary>
    /// Checks if a position is inside the view of the bot
    /// </summary>
    /// <param name="position">The position you want to check</param>
    private bool CheckIfPositionIsInsideView(Vector3 position)
    {
        float viewLeft = Bot.transform.position.x - 6;
        float viewUp = Bot.transform.position.z + 12;
        float viewRight = viewLeft + 12;
        float viewDown = viewUp - 24;
        float posX = position.x;
        float posZ = position.z;
        return (viewLeft <= posX && posX <= viewRight && viewDown <= posZ && posZ <= viewUp);
    }
}
