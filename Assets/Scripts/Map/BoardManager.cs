using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class BoardManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_mapPrefab;
    [SerializeField]
    private GameObject m_breakableWallPrefab;
    [SerializeField]
    private int m_mapWidth = 50;
    [SerializeField]
    private int m_mapHeight = 50;
    [SerializeField]
    private int m_outerWallWidth = 14;
    [SerializeField]
    private int m_maxRoomAmount = 100;
    [SerializeField]
    private int m_maxShortcutAmount = 10;
    [SerializeField]
    private int m_maxRoomSize = 80;
    [SerializeField]
    private int m_minRoomLength = 6;
    [SerializeField]
    private int m_maxPlaceAttempts = 10;
    [SerializeField]
    private int m_maxBuildAttempts = 250;
    [SerializeField]
    private int m_maxShortcutAttempts = 250;
    [SerializeField]
    private int m_minTunnelLength = 1;
    [SerializeField]
    private int m_maxTunnelLength = 7;
    [SerializeField]
    private int m_tunnelWidth = 2;
    [SerializeField]
    private int m_chanceForBreakableTunnel = 20;


    private string m_seed;
    private int[][] m_tileMap;
    private GameObject m_map;
    private List<GameObject> m_mapParts = new List<GameObject>();
    // so that a position is in the middle of a tile
    private const float MAP_OFFSET = -0.5f;

    // --------------------------------------------------------------------------------------------
    // Public functions
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Creates a random map object 
    /// </summary>
    public void SetupScene(string seed)
    {
        m_seed = seed;
        GenerateRandomMap();
        LoadFloor();
        LoadMap();
        CreateOuterWalls();
        CombineAllMeshes();
    }

    /// <summary>
    /// Gets the tilemap
    /// </summary>
    /// <returns>A jagged array of chars containing the map data</returns>
    public int[][] GetTileMap()
    {
        return m_tileMap;
    }

    /// <summary>
    /// Generate a random seed
    /// </summary>
    /// <returns>A string containing the random seed</returns>
    public string GenerateSeed()
    {
        StringBuilder builder = new StringBuilder();
        char ch;
        for (int i = 0; i < 20; i++)
        {
            ch = (char)UnityEngine.Random.Range('a', 'z');
            builder.Append(ch);
        }
        return builder.ToString();
    }

    /// <summary>
    /// Gets a random floor tile
    /// </summary>
    /// <returns>Vector2 containing the position of the floortile</returns>
    public Vector2 GetRandomFloorTile()
    {
        while (true)
        {
            // get a random tile in the map
            Vector2 randomTile = new Vector2(UnityEngine.Random.Range(1, m_mapWidth - 1), UnityEngine.Random.Range(1, m_mapHeight - 1));

            if (m_tileMap[(int)randomTile.x][(int)randomTile.y] == 0)
            {
                return randomTile;
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // Generate maps
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Generates a map based on the content
    /// </summary>
    /// <param name="content">The tile to fill the map with</param>
    private void GenerateEmptyMap(int content)
    {
        m_tileMap = new int[m_mapWidth][];
        for (int x = 0; x < m_mapWidth; x++)
        {
            m_tileMap[x] = new int[m_mapHeight];
            for (int y = 0; y < m_mapHeight; y++)
            {
                m_tileMap[x][y] = content;
            }
        }
    }

    /// <summary>
    /// Generates the hardcoded test map
    /// </summary>
    private void GenerateTestMap()
    {
        GenerateEmptyMap(1);

        for (int x = 0; x < m_mapWidth; x++)
        {
            for (int y = 0; y < m_mapHeight; y++)
            {
                if (// rooms
                    x == 0 && y == 0
                    || x >= 0 && x <= 9 && y >= 16 && y <= 21
                    || x >= 1 && x <= 7 && y >= 0 && y <= 5
                    || x >= 11 && x <= 15 && y >= 4 && y <= 10
                    || x >= 16 && x <= 20 && y >= 13 && y <= 18
                    || x >= 23 && x <= 29 && y >= 3 && y <= 10
                    || x >= 24 && x <= 27 && y >= 17 && y <= 21
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
                    m_tileMap[x][y] = 0;
                }
            }
        }
    }

    /// <summary>
    /// Generates a random map, based on the settings
    /// </summary>
    private void GenerateRandomMap()
    {
        // Create level with only walls
        GenerateEmptyMap(1);

        // Display seed on the hud
        GameObject hud = GameObject.FindGameObjectWithTag("HUD");
        TextMeshProUGUI text = hud.GetComponent<TextMeshProUGUI>();
        text.text = m_seed;

        // Change seed of randomizer
        UnityEngine.Random.InitState(m_seed.GetHashCode());

        // Generate first room (in the middle) and add it to the map
        Room room = GenerateRandomRoom();
        room.Pos = new Vector2(m_mapWidth / 2 - room.RoomMap.Length / 2, m_mapHeight / 2 - room.RoomMap[0].Length / 2);
        AddRoom(room);

        int currentRoomAmount = 1;
        int currentBuildAttempt = 0;

        while (currentBuildAttempt < m_maxBuildAttempts && currentRoomAmount < m_maxRoomAmount)
        {
            // Generate a room
            room = GenerateRandomRoom();
            // Try to place the room
            bool placed = PlaceRoom(room);
            // If succeeded
            if (placed)
            {
                currentRoomAmount++;
            }
            currentBuildAttempt++;
        }
        // Add shortcuts
        AddShortcuts();
        DebugMap(m_tileMap);

        // Set seed to random again
        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
    }

    // --------------------------------------------------------------------------------------------
    // Random generation functions
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Generates a random square room based on the settings
    /// </summary>
    /// <returns>A Room object</returns>
    private Room GenerateRandomRoom()
    {
        int width = UnityEngine.Random.Range(m_minRoomLength, m_maxRoomSize / m_minRoomLength);
        int height = UnityEngine.Random.Range(m_minRoomLength, m_maxRoomSize / width);

        int[][] roomMap = new int[width][];
        for (int x = 0; x < width; x++)
        {
            roomMap[x] = new int[height];
        }
        return new Room(roomMap);
    }

    /// <summary>
    /// Adds the room to the map
    /// </summary>
    /// <param name="room">Room to be added to the map</param>
    private void AddRoom(Room room)
    {
        PasteTileMap(room.RoomMap, m_tileMap, room.Pos);
    }

    /// <summary>
    /// Add a small tilemap to a big tilemap
    /// </summary>
    /// <param name="smallMap">Small tile map to be added</param>
    /// <param name="bigMap">Big tilemap to be added to</param>
    /// <param name="pos">Position of where the small map should be pasted within the big map</param>
    private void PasteTileMap(int[][] smallMap, int[][] bigMap, Vector2 pos)
    {
        // for every tile in the room
        for (int x = 0; x < smallMap.Length; x++)
        {
            for (int y = 0; y < smallMap[0].Length; y++)
            {
                // change the according tile on the map
                if (smallMap[x][y] != 1)
                {
                    bigMap[x + (int)pos.x][y + (int)pos.y] = smallMap[x][y];
                }
            }
        }
    }

    /// <summary>
    /// Try to place the room within the map
    /// </summary>
    /// <param name="room">Room to be added to the map</param>
    /// <returns>True if successful</returns>
    private bool PlaceRoom(Room room)
    {
        for (int i = 1; i <= m_maxPlaceAttempts; i++)
        {
            room = TryPlacement(room);
            if (room.Pos.x != -1 && room.Pos.y != -1)
            {
                // add room to the map
                AddRoom(room);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Generates a random position on the map to add a room, and sees if the room fits there
    /// </summary>
    /// <param name="room">Room to be added</param>
    /// <returns>The room, position is (-1,-1) if this attempt didn't succeed</returns>
    private Room TryPlacement(Room room)
    {
        // get random direction to attach the room to the map
        Vector2 direction = GenerateRandomDirection();

        // get random walltile for that direction
        Vector2 wallTile = GetRandomWallTile(direction);

        // get a random tile within the room, to attach the corridor to
        Vector2 entrance = new Vector2(UnityEngine.Random.Range(0, room.RoomMap.Length + 1 - m_tunnelWidth), UnityEngine.Random.Range(0, room.RoomMap[0].Length + 1 - m_tunnelWidth));
        if (direction.y == -1)
        {
            entrance.y = room.RoomMap[0].Length - 1;
        }
        else if (direction.y == 1)
        {
            entrance.y = 0;
        }
        else if (direction.x == -1)
        {
            entrance.x = room.RoomMap.Length - 1;
        }
        else if (direction.x == 1)
        {
            entrance.x = 0;
        }

        // determine the position of the room within the map, based on the walltile and the tile within the room
        room.Pos = new Vector2(wallTile.x - entrance.x, wallTile.y - entrance.y);

        // generate random ordered list of tunnel lengths
        IEnumerable<int> tunnelLengths = UniqueRandom(m_minTunnelLength, m_maxTunnelLength);

        foreach (int i in tunnelLengths)
        {
            // create temporary values for this length
            int[][] tempMap = AddTunnelToMap(room.RoomMap, i, entrance, direction);
            Vector2 tempEntrance = new Vector2(entrance.x, entrance.y);

            // adjust position and entrance of the room based on the length of the tunnel
            if (direction.y == -1)
            {
                room.Pos.y = wallTile.y + 1 - tempMap[0].Length;
                tempEntrance.y += i;
            }
            else if (direction.x == -1)
            {
                room.Pos.x = wallTile.x + 1 - tempMap.Length;
                tempEntrance.x += i;
            }

            // calculate all entrance tiles
            List<Vector2> entranceTiles = new List<Vector2>();
            for (int j = 0; j < m_tunnelWidth; j++)
            {
                entranceTiles.Add(new Vector2((int)tempEntrance.x + Math.Abs((int)direction.y) * j, (int)tempEntrance.y + Math.Abs((int)direction.x) * j));
            }

            // if it can be placed, return the room
            if (CanPlace(tempMap, room.Pos, entranceTiles))
            {
                room.RoomMap = tempMap;
                return room;
            }
        }

        // for every length of the tunnel, this room doesn't fit. set the pos to -1,-1 and return
        room.Pos = new Vector2(-1, -1);
        return room;
    }

    /// <summary>
    /// Generate a random direction
    /// </summary>
    /// <returns>Vector2 which contains a random direction</returns>
    private Vector2 GenerateRandomDirection()
    {
        return GetDirection(UnityEngine.Random.Range(1, 5)); // max is exclusive, so this will generate number between 1 and 4
    }

    /// <summary>
    /// Gets a direction based on an int
    /// </summary>
    /// <param name="directionInt">int indicating the direction (1 = north, 2, = east, 3 = south, 4 = west)</param>
    /// <returns>Vector2 which contains the direction</returns>
    private Vector2 GetDirection(int directionInt)
    {
        Vector2 direction = new Vector2();
        switch (directionInt)
        {
            case 1:
                direction = new Vector2(0, 1);
                break;
            case 2:
                direction = new Vector2(1, 0);
                break;
            case 3:
                direction = new Vector2(0, -1);
                break;
            case 4:
                direction = new Vector2(-1, 0);
                break;
        }
        return direction;
    }

    /// <summary>
    /// Gets a random wall tile based on a direction
    /// </summary>
    /// <param name="direction">Direction the tunnel will face if added to the wall</param>
    /// <returns>A Vector2 containing the position of the wall</returns>
    private Vector2 GetRandomWallTile(Vector2 direction)
    {
        while (true)
        {
            // get a random tile in the map
            Vector2 randomTile = new Vector2(UnityEngine.Random.Range(1, m_mapWidth - m_tunnelWidth), UnityEngine.Random.Range(1, m_mapHeight - m_tunnelWidth));

            if (CheckWallTile(randomTile, direction))
            {
                return randomTile;
            }
        }
    }

    /// <summary>
    /// Checks if the given tile is a wall in the given Direction
    /// </summary>
    /// <param name="randomTile">Tile to be checked</param>
    /// <param name="direction">Direction the tunnel will face if added to the wall</param>
    /// <returns>True if tile is indeed a wall based on the direction</returns>
    private bool CheckWallTile(Vector2 randomTile, Vector2 direction)
    {
        // check if the surrounding tiles are suitable for the width of the tunnel
        for (int i = 0; i < m_tunnelWidth; i++)
        {
            if (!(m_tileMap[(int)(randomTile.x + i * Math.Abs(direction.y))][(int)(randomTile.y + i * Math.Abs(direction.x))] == 1
                    && m_tileMap[(int)(randomTile.x + i * Math.Abs(direction.y) + direction.x)][(int)(randomTile.y + i * Math.Abs(direction.x) + direction.y)] == 1
                    && m_tileMap[(int)(randomTile.x + i * Math.Abs(direction.y) - direction.x)][(int)(randomTile.y + i * Math.Abs(direction.x) - direction.y)] == 0))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Adds a tunnel to a given room
    /// </summary>
    /// <param name="map">Map the tunnel should be added to</param>
    /// <param name="tunnelLength">Length of the tunnel</param>
    /// <param name="entrance">The tile within the room that the tunnel will be adjacent to</param>
    /// <param name="direction">Direction of the tunnel</param>
    /// <returns>The new map with the tunnel added to it</returns>
    private int[][] AddTunnelToMap(int[][] map, int tunnelLength, Vector2 entrance, Vector2 direction)
    {
        // create new map with right size
        int[][] newMap = new int[map.Length + (int)Math.Abs(direction.x) * tunnelLength][];
        for (int x = 0; x < newMap.Length; x++)
        {
            newMap[x] = new int[map[0].Length + (int)Math.Abs(direction.y) * tunnelLength];
            for (int y = 0; y < newMap[0].Length; y++)
            {
                newMap[x][y] = 1;
            }
        }

        // copy roommap into new map
        PasteTileMap(map, newMap, new Vector2((int)Math.Abs(Math.Max(0, direction.x)) * tunnelLength, (int)Math.Abs(Math.Max(0, direction.y)) * tunnelLength));

        // generate tunnel
        int[][] tunnel = new int[(int)(m_tunnelWidth * Math.Abs(direction.y) + tunnelLength * Math.Abs(direction.x))][];

        for (int x = 0; x < tunnel.Length; x++)
        {
            tunnel[x] = new int[(int)(m_tunnelWidth * Math.Abs(direction.x) + tunnelLength * Math.Abs(direction.y))];
        }

        // generate position within new map
        Vector2 pos = new Vector2(entrance.x + Math.Abs(Math.Max(0, direction.x)) * tunnelLength, entrance.y + Math.Abs(Math.Max(0, direction.y)) * tunnelLength);
        if (direction.x == 1)
        {
            pos.x -= tunnelLength;
        }
        else if (direction.x == -1)
        {
            pos.x += 1;
        }
        else if (direction.y == 1)
        {
            pos.y -= tunnelLength;
        }
        else if (direction.y == -1)
        {
            pos.y += 1;
        }

        // add tunnel to new map
        PasteTileMap(tunnel, newMap, pos);

        // return new map
        return newMap;
    }

    /// <summary>
    /// Checks if a map can be added to the m_tilemap
    /// </summary>
    /// <param name="map">Small map to be added</param>
    /// <param name="pos">Position of the small map within the m_tilemap</param>
    /// <param name="entranceTiles">Tiles that should be excluded of checking</param>
    /// <returns>True if the map can indeed be placed inside the m_tilemap</returns>
    private bool CanPlace(int[][] map, Vector2 pos, List<Vector2> entranceTiles)
    {
        // check out of bounds
        if (pos.x <= 0 || pos.x > m_mapWidth - map.Length - 1 || pos.y <= 0 || pos.y > m_mapHeight - map[0].Length - 1)
        {
            return false;
        }

        // check overlap
        for (int x = 0; x < map.Length; x++)
        {
            for (int y = 0; y < map[0].Length; y++)
            {
                if (map[x][y] == 0 && // check if position in room equals zero
                    !entranceTiles.Contains(new Vector2(x, y)) && // check if the tile isn't an entrance
                    (m_tileMap[x + (int)pos.x][y + (int)pos.y] == 0 || // if so, check if the same position on map is zero
                    m_tileMap[x + (int)pos.x][y + (int)pos.y + 1] == 0 || // and check all tiles around the position on the map is zero, starting with north
                    m_tileMap[x + (int)pos.x + 1][y + (int)pos.y + 1] == 0 || // northeast
                    m_tileMap[x + (int)pos.x + 1][y + (int)pos.y] == 0 || // east
                    m_tileMap[x + (int)pos.x + 1][y + (int)pos.y - 1] == 0 || // southeast
                    m_tileMap[x + (int)pos.x][y + (int)pos.y - 1] == 0 || // south
                    m_tileMap[x + (int)pos.x - 1][y + (int)pos.y - 1] == 0 || // southwest
                    m_tileMap[x + (int)pos.x - 1][y + (int)pos.y] == 0 || // west
                    m_tileMap[x + (int)pos.x - 1][y + (int)pos.y + 1] == 0 // northwest
                    ))
                    return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Add shortcuts to m_tilemap based on the settings
    /// </summary>
    private void AddShortcuts()
    {
        int currentShortcutAttempt = 0;
        int currentShortcutAmount = 0;
        while (currentShortcutAttempt < m_maxShortcutAttempts && currentShortcutAmount < m_maxShortcutAmount)
        {
            // get random direction to create a shortcut
            Vector2 direction = GenerateRandomDirection();

            // get random walltile for that direction
            Vector2 wallTile = GetRandomWallTile(direction);
            Vector2 otherWallTile = Vector2.zero;

            for (int i = m_minTunnelLength; i < m_maxTunnelLength; i++)
            {
                otherWallTile = new Vector2(wallTile.x + direction.x * (i - 1), (int)(wallTile.y + direction.y * (i - 1)));
                if (!(otherWallTile.x <= 1 || otherWallTile.x >= m_mapWidth - 1 || otherWallTile.y <= 1 || otherWallTile.y >= m_mapHeight - 1) && CheckWallTile(otherWallTile, new Vector2(direction.x * -1, direction.y * -1)))
                {
                    // generate tunnel
                    int[][] tunnel = new int[(int)(m_tunnelWidth * Math.Abs(direction.y) + i * Math.Abs(direction.x))][];

                    for (int j = 0; j < tunnel.Length; j++)
                    {
                        tunnel[j] = new int[(int)(m_tunnelWidth * Math.Abs(direction.x) + i * Math.Abs(direction.y))];
                    }

                    // generate position of tunnel
                    Vector2 pos = new Vector2();
                    if (direction.x > 0 || direction.y > 0)
                    {
                        pos = wallTile;
                    }
                    else
                    {
                        pos = otherWallTile;
                    }

                    // set entrances
                    List<Vector2> entranceTiles = new List<Vector2>();
                    for (int j = 0; j < m_tunnelWidth; j++)
                    {
                        entranceTiles.Add(new Vector2(direction.y * j, direction.x * j));
                        // TODO check this
                        entranceTiles.Add(new Vector2(direction.y * j + direction.x * (tunnel[0].Length - 1), direction.y * (tunnel[0].Length - 1) + direction.x * j));
                    }

                    // check if placeable
                    if (CanPlace(tunnel, pos, entranceTiles))
                    {
                        // if breakable, make entrance tiles a breakable block
                        if (IsBreakable())
                        {
                            for (int j = 0; j < entranceTiles.Count; j++)
                            {
                                // change the according tile on the map
                                tunnel[(int)entranceTiles[j].x][(int)entranceTiles[j].y] = 2;
                            }
                        }

                        PasteTileMap(tunnel, m_tileMap, pos);
                        currentShortcutAmount++;
                    }
                }
            }
            currentShortcutAttempt++;
        }
    }

    private bool IsBreakable()
    {
        int i = UnityEngine.Random.Range(1, 101);
        return i <= m_chanceForBreakableTunnel;
    }

    // --------------------------------------------------------------------------------------------
    // Create map functions
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Loads the floor of the map
    /// </summary>
    private void LoadFloor()
    {
        m_map = new GameObject();
        m_map.name = "Map";
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);

        floor.transform.position = new Vector3((float)m_mapWidth / 2, MAP_OFFSET, (float)m_mapHeight / 2);
        floor.transform.localScale = new Vector3((float)(m_mapWidth + m_outerWallWidth * 2) / 10, 1, (float)(m_mapHeight + m_outerWallWidth * 2) / 10);
        floor.transform.SetParent(m_map.transform);
    }

    /// <summary>
    /// Uses m_tilemap to load in the map
    /// </summary>
    private void LoadMap()
    {
        for (int i = 0; i < m_tileMap.Length; i++)
        {
            for (int j = 0; j < m_tileMap[0].Length; j++)
            {
                if (m_tileMap[i][j] == 1)
                { // if 1, build wall
                    GameObject instance = GameObject.CreatePrimitive(PrimitiveType.Cube);

                    // calculate walls
                    Walls walls = Walls.None;
                    if (j + 1 < m_tileMap[0].Length && m_tileMap[i][j + 1] != 1)
                    {
                        walls = walls | Walls.Up;
                    }
                    if (i + 1 < m_tileMap.Length && m_tileMap[i + 1][j] != 1)
                    {
                        walls = walls | Walls.Right;
                    }
                    if (j - 1 >= 0 && m_tileMap[i][j - 1] != 1)
                    {
                        walls = walls | Walls.Down;
                    }
                    if (i - 1 >= 0 && m_tileMap[i - 1][j] != 1)
                    {
                        walls = walls | Walls.Left;
                    }

                    instance.transform.GetComponent<MeshFilter>().sharedMesh = generateNewMesh(walls);

                    instance.transform.position = new Vector3(i + MAP_OFFSET, 0f, j + MAP_OFFSET);
                    instance.transform.SetParent(m_map.transform);
                }
                // if 2, build breakable wall
                else if (m_tileMap[i][j] == 2)
                {
                    GameObject instance = Instantiate(m_breakableWallPrefab, new Vector3(i, 0f, j), Quaternion.identity);
                    instance.name = "BreakableWall";
                    instance.transform.SetParent(m_map.transform);
                }
            }
        }
    }

    /// <summary>
    /// Creates the outer walls for the map
    /// </summary>
    private void CreateOuterWalls()
    {
        for (int i = -m_outerWallWidth; i < m_tileMap.Length + m_outerWallWidth; i++)
        {
            for (int j = -m_outerWallWidth; j < m_tileMap[0].Length + m_outerWallWidth; j++)
            {
                if (i < 0 || i >= m_tileMap.Length || j < 0 || j >= m_tileMap[0].Length)
                {
                    GameObject instance = GameObject.CreatePrimitive(PrimitiveType.Cube);

                    instance.transform.GetComponent<MeshFilter>().sharedMesh = generateNewMesh(Walls.None);

                    instance.transform.position = new Vector3(i + MAP_OFFSET, 0f, j + MAP_OFFSET);
                    instance.transform.SetParent(m_map.transform);
                }
            }
        }
    }

    /// <summary>
    /// Creates a new mesh based on the walls given
    /// </summary>
    /// <param name="walls">Used to see which vertices should be generated</param>
    /// <returns>The new mesh</returns>
    private Mesh generateNewMesh(Walls walls)
    {
        // all the vertices needed for the different faces
        // vertices can't be shared between the triangles, because the shading will be wrong then
        Vector3[] vertices = {
                        new Vector3 (1, 0.5f, 0),
                        new Vector3 (0, 0.5f, 0),
                        new Vector3 (0, 0.5f, 1),
                        new Vector3 (1, 0.5f, 1),

                        new Vector3 (1, 0.5f, 1),
                        new Vector3 (0, 0.5f, 1),
                        new Vector3 (0, -0.5f, 1),
                        new Vector3 (1, -0.5f, 1),

                        new Vector3 (1, -0.5f, 0),
                        new Vector3 (1, 0.5f, 0),
                        new Vector3 (1, 0.5f, 1),
                        new Vector3 (1, -0.5f, 1),

                        new Vector3 (0, -0.5f, 0),
                        new Vector3 (1, 0.5f, 0),
                        new Vector3 (1, -0.5f, 0),
                        new Vector3 (0, 0.5f, 0),

                        new Vector3 (0, -0.5f, 0),
                        new Vector3 (0, -0.5f, 1),
                        new Vector3 (0, 0.5f, 1),
                        new Vector3 (0, 0.5f, 0)
                    };

        int[] faceTop = { 0, 1, 2, 0, 2, 3 };
        int[] faceUp = { 4, 5, 6, 4, 6, 7 };
        int[] faceRight = { 8, 9, 10, 8, 10, 11 };
        int[] faceDown = { 12, 13, 14, 12, 15, 13 };
        int[] faceLeft = { 16, 17, 18, 16, 18, 19 };

        List<int> triangles = new List<int>();

        AddMultipleInts(triangles, faceTop);

        if (walls.HasFlag(Walls.Up))
        {
            AddMultipleInts(triangles, faceUp);
        }
        if (walls.HasFlag(Walls.Right))
        {
            AddMultipleInts(triangles, faceRight);
        }
        if (walls.HasFlag(Walls.Down))
        {
            AddMultipleInts(triangles, faceDown);
        }
        if (walls.HasFlag(Walls.Left))
        {
            AddMultipleInts(triangles, faceLeft);
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }

    /// <summary>
    /// Adds an array of ints to a list of ints
    /// </summary>
    /// <param name="list">List of ints</param>
    /// <param name="ints">Array of ints that should be added to the list</param>
    private void AddMultipleInts(List<int> list, int[] ints)
    {
        for (int i = 0; i < ints.Length; i++)
        {
            list.Add(ints[i]);
        }
    }

    /// <summary>
    /// Combines all the meshes of the map
    /// </summary>
    private void CombineAllMeshes()
    {
        // initialize
        int vertexLimit = 30000;
        int verticesSoFar = 0;
        List<MeshFilter> meshFilters = new List<MeshFilter>();
        foreach (Transform child in m_map.transform)
        {
            if (child.gameObject.name != "BreakableWall")
            {
                meshFilters.Add(child.GetComponent<MeshFilter>());
            }
        }

        List<CombineInstance> combiners = new List<CombineInstance>();
        CombineInstance combine = new CombineInstance();

        // for all the objects within the map
        for (int i = 0; i < meshFilters.Count; i++)
        {
            // if the amount of vertices till now would be over the limit
            if (verticesSoFar + meshFilters[i].mesh.vertexCount > vertexLimit)
            {
                // send list to be combined
                CreateCombinedMeshes(combiners);
                combiners.Clear();
                verticesSoFar = 0;
            }

            // add to the list
            combine.subMeshIndex = 0;
            combine.mesh = meshFilters[i].sharedMesh;
            combine.transform = meshFilters[i].transform.localToWorldMatrix;
            combiners.Add(combine);
            verticesSoFar += combine.mesh.triangles.Length;
        }

        // call the mesh combiner one last time for the last few objects in the list
        CreateCombinedMeshes(combiners);

        // destroy all the objects the map was made up of
        foreach (Transform child in m_map.transform)
        {
            if (child.gameObject.name != "BreakableWall")
            {
                Destroy(child.gameObject);
            }
        }

        // add all the map parts to the map
        foreach (GameObject mapPart in m_mapParts)
        {
            mapPart.transform.SetParent(m_map.transform);
        }
    }

    /// <summary>
    /// Adds a mappart based on the mesh data
    /// </summary>
    /// <param name="meshDataList">Data of the meshes used to create the mappart</param>
    private void CreateCombinedMeshes(List<CombineInstance> meshDataList)
    {
        // create the combined mesh
        Mesh newMesh = new Mesh();
        newMesh.CombineMeshes(meshDataList.ToArray());

        // create new map object to hold part of the map
        GameObject mapPart = Instantiate(m_mapPrefab, Vector3.zero, Quaternion.identity) as GameObject;

        // handle new map object
        mapPart.transform.GetComponent<MeshFilter>().sharedMesh = newMesh;
        var collider = mapPart.GetComponent<MeshCollider>();
        collider.sharedMesh = newMesh;

        // add part to list
        m_mapParts.Add(mapPart);
        mapPart.name = "MapPart " + m_mapParts.Count;
    }

    // --------------------------------------------------------------------------------------------
    // Helper functions
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Gets a list of ints in a random sequence
    /// </summary>
    /// <param name="min">Lowest number in sequence</param>
    /// <param name="max">Highest number in sequence</param>
    /// <returns>List of ints in random sequence</returns>
    private IEnumerable<int> UniqueRandom(int min, int max)
    {
        List<int> candidates = new List<int>();
        for (int i = min; i <= max; i++)
        {
            candidates.Add(i);
        }
        while (candidates.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, candidates.Count);
            yield return candidates[index];
            candidates.RemoveAt(index);
        }
    }

    // --------------------------------------------------------------------------------------------
    // Debug functions
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Prints the map given
    /// </summary>
    /// <param name="map">Map to print</param>
    private void DebugMap(int[][] map)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append('\n');
        for (int y = map[0].Length - 1; y >= 0; y--)
        {
            for (int x = 0; x < map.Length; x++)
            {
                builder.Append(map[x][y]);
            }
            builder.Append('\n');
        }
        string s = builder.ToString();
        Debug.Log(s);
    }

    /// <summary>
    /// Prints the adding of a roommap to a map
    /// </summary>
    /// <param name="map">Map to print</param>
    /// <param name="roomMap">Room to add to the map</param>
    /// <param name="pos">Position of the room within the map</param>
    private void DebugAddingRoom(int[][] map, int[][] roomMap, Vector2 pos)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append('\n');
        for (int y = map[0].Length - 1; y >= 0; y--)
        {
            for (int x = 0; x < map.Length; x++)
            {
                if (x >= pos.x && x < pos.x + roomMap.Length && y >= pos.y && y < pos.y + roomMap[0].Length)
                {
                    builder.Append(roomMap[x - (int)pos.x][y - (int)pos.y] + 2);
                }
                else
                {
                    builder.Append(map[x][y]);
                }
            }
            builder.Append('\n');
        }
        string s = builder.ToString();
        Debug.Log(s);
    }
}

public class Room
{
    public int[][] RoomMap;
    public Vector2 Pos;

    public Room(int[][] roomMap)
    {
        RoomMap = roomMap;
    }
}

// Walls is used for loading the map
[Flags]
public enum Walls
{
    // Decimal     // Binary
    None = 0,    // 000000
    Up = 1,    // 000001
    Right = 2,    // 000010
    Down = 4,    // 000100
    Left = 8    // 001000
}
