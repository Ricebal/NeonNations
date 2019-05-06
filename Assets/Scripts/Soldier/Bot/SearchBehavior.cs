using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SearchBehavior : MonoBehaviour, IDStarLiteEnvironment
{
    public Coordinates GoalCoordinates;
    public Coordinates StartCoordinates;
    public GameObject Bot;

    // Variables for debugging
    public int x;
    public int y;

    //These variables can be deleted after the map is integrated
    public const int Columns = 30;
    public const int Rows = 22;

    // This variable can later be private
    public bool[,] m_tileMap = new bool[Columns, Rows];

    private bool m_completed = false;
    private DStarLite m_dStarLite;
    private Bot m_bot;

    // Start is called before the first frame update
    void Start()
    {
        StartCoordinates = ConvertGameObjectToCoordinates(Bot.transform);
        GoalCoordinates = new Coordinates(29, 3);
        m_dStarLite = new DStarLite();
        m_dStarLite.Initialize(Columns, Rows, this);
        m_dStarLite.RunDStarLite(StartCoordinates.x, StartCoordinates.y, GoalCoordinates.x, GoalCoordinates.y);
        m_bot = GetComponent<Bot>();
        GenerateMap();    // used temperarly to get the right map-layout. This should later be replaced by a getMapLayout in the BoardManager or something like that
    }

    // Update is called once per frame
    void Update()
    {
        Coordinates currentCoordinates = ConvertGameObjectToCoordinates(Bot.transform);
        x = currentCoordinates.x;
        y = currentCoordinates.y;
        if (currentCoordinates.x != GoalCoordinates.x || currentCoordinates.y != GoalCoordinates.y)
        {
            m_dStarLite.NextMove();
        }
        else
        {
            bool found = false;
            int mapX = 0;
            int mapY = 0;
            System.Random r = new System.Random();
            while (!found)
            {
                mapX = r.Next(0, Columns);
                mapY = r.Next(0, Rows);
                if (m_tileMap[mapX, mapY])
                {
                    found = true;
                }
            }
            GoalCoordinates.x = mapX;
            GoalCoordinates.y = mapY;
            m_dStarLite.RunDStarLite(currentCoordinates.x, currentCoordinates.y, GoalCoordinates.x, GoalCoordinates.y);
        }
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

    void IDStarLiteEnvironment.MoveTo(Coordinates s)
    {
        //Coordinates botCoordinates = ConvertGameObjectToCoordinates(Bot.transform);
        float horizontal = s.x - /*botCoordinates.x;//*/ Bot.transform.position.x;
        float vertical = s.y - /*botCoordinates.y;//*/ Bot.transform.position.z;
        // The clamp normalizes the input to a value between -1 and 1 (To represent the players input)
        m_bot.Move(Mathf.Clamp((float)horizontal, -1f, 1f), Mathf.Clamp((float)vertical, -1f, 1f));
    }

    LinkedList<Coordinates> IDStarLiteEnvironment.GetObstaclesInVision()
    {
        // First get all coordinates that are illuminated
        LinkedList<Coordinates> currentCoordinatesInSight = GetIlluminatedCoordinates();
        // Than check the map-layout to see if the illuminated parts are walls or empty spaces
        FilterCoordinatesWithObstacle(ref currentCoordinatesInSight);
        // Return the result
        return currentCoordinatesInSight;
    }

    private void FilterCoordinatesWithObstacle(ref LinkedList<Coordinates> coordinates)
    {
        LinkedList<Coordinates> coordinatesToDelete = new LinkedList<Coordinates>();
        foreach (Coordinates coordinate in coordinates)
        {
            // If the coordinates are outside the map they shouldn't be checked (to prevent out of index exception)
            if(coordinate.x < 0 || coordinate.y < 0 || coordinate.x >= Columns || coordinate.y >= Rows)
            {
                coordinatesToDelete.AddLast(coordinate);
                continue;
            }
            // If the coordinates are inside the map, check if the coordinate contains an obstacle
            if(m_tileMap[coordinate.x, coordinate.y])
            {
                coordinatesToDelete.AddLast(coordinate);
            }
        }
        foreach (Coordinates coordinate in coordinatesToDelete)
        {
            coordinates.Remove(coordinate);
        }
    }

    private Coordinates ConvertGameObjectToCoordinates(Transform transform)
    {
        return new Coordinates((int)Math.Round(transform.position.x, 0, MidpointRounding.AwayFromZero), (int)Math.Round(transform.position.z, 0, MidpointRounding.AwayFromZero));
    }

    private LinkedList<Coordinates> GetIlluminatedCoordinates()
    {
        LinkedList<Coordinates> illuminatedCoordinates = GetCoordinatesAroundBot(); // Because of the spotlight around the bot
        GetCoordinatesAroundLights(ref illuminatedCoordinates); // All other lights visible on screen
        return illuminatedCoordinates;
    }
    private LinkedList<Coordinates> GetCoordinatesAroundBot()
    {
        LinkedList<Coordinates> currentCoordinatesInSight = new LinkedList<Coordinates>();
        Coordinates botCoordinate = ConvertGameObjectToCoordinates(Bot.transform);
        return GetCoordinatesInRange(botCoordinate, 2, currentCoordinatesInSight);
    }

    public Coordinates GetPosition()
    {
        return ConvertGameObjectToCoordinates(Bot.transform);
    }

    // Returns a linked list containing all map-coordinates that should be illuminated by the object
    private LinkedList<Coordinates> GetCoordinatesInRange(Coordinates objectCoordinates, int range, LinkedList<Coordinates> coordinatesInRange)
    {
        range = Mathf.Clamp(range, 0, 3);
        for (int i = -range; i < range; i++)
        {
            for (int j = -range; j < range; j++)
            {
                int x = objectCoordinates.x + i;
                int y = objectCoordinates.y + j;
                // Check if coordinate is already added to the list
                if (!coordinatesInRange.Any(coordinate => coordinate.x == x && coordinate.y == y))
                {
                    coordinatesInRange.AddLast(new Coordinates(x, y));
                }
            }
        }
        return coordinatesInRange;
    }

    private void GetCoordinatesAroundLights(ref LinkedList<Coordinates> illuminatedCoordinates)
    {
        List<Light> lightsInSight = GetLights();

        // Get all illuminated coordinates
        foreach(Light light in lightsInSight)
        {
            GetCoordinatesInRange(ConvertGameObjectToCoordinates(light.transform), (int)Math.Floor(light.range), illuminatedCoordinates);
        }
    }

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
