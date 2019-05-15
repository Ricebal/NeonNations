using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MapGenerator
{
    [SerializeField]
    private int m_mapWidth = 50;
    [SerializeField]
    private int m_mapHeight = 50;
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

    private int[][] m_tileMap;

    // --------------------------------------------------------------------------------------------
    // Generate maps
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Generates a map based on the content
    /// </summary>
    /// <param name="content">The tile to fill the map with</param>
    public int[][] GenerateEmptyMap(int content)
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
        return m_tileMap;
    }

    /// <summary>
    /// Generates the hardcoded test map
    /// </summary>
    public int[][] GenerateTestMap()
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
        return m_tileMap;
    }

    /// <summary>
    /// Generates a random map, based on the settings
    /// </summary>
    /// <param name="seed">Seed to be used with the generation</param>
    /// <returns>A jagged array of ints representing the tilemap</returns>
    public int[][] GenerateRandomMap(string seed)
    {
        // Create level with only walls
        GenerateEmptyMap(1);

        // Change seed of randomizer
        UnityEngine.Random.InitState(seed.GetHashCode());

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

        // Set seed to random again
        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);

        return m_tileMap;
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
        // for every tile in the small map
        for (int x = 0; x < smallMap.Length; x++)
        {
            for (int y = 0; y < smallMap[0].Length; y++)
            {
                // change the according tile on the big map
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
            if (TryPlacement(room))
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
    /// <returns>True if succeeded</returns>
    private bool TryPlacement(Room room)
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
            if (CanPlace(tempMap, room.Pos, entranceTiles, direction))
            {
                room.RoomMap = tempMap;
                return true;
            }
        }

        // for every length of the tunnel, this room doesn't fit. return false
        return false;
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
    /// <param name="direction">Direction of entrance</param>
    /// <returns>True if the map can indeed be placed inside the m_tilemap</returns>
    private bool CanPlace(int[][] map, Vector2 pos, List<Vector2> entranceTiles, Vector2 direction)
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
                    (!entranceTiles.Contains(new Vector2(x, y)) && // if the tile isn't an entrance
                    (m_tileMap[x + (int)pos.x][y + (int)pos.y] != 1 || // if so, check if the same position on map isn't a wall
                    m_tileMap[x + (int)pos.x][y + (int)pos.y + 1] != 1 || // and check all tiles around the position on the map, starting with north
                    m_tileMap[x + (int)pos.x + 1][y + (int)pos.y + 1] != 1 || // northeast
                    m_tileMap[x + (int)pos.x + 1][y + (int)pos.y] != 1 || // east
                    m_tileMap[x + (int)pos.x + 1][y + (int)pos.y - 1] != 1 || // southeast
                    m_tileMap[x + (int)pos.x][y + (int)pos.y - 1] != 1 || // south
                    m_tileMap[x + (int)pos.x - 1][y + (int)pos.y - 1] != 1 || // southwest
                    m_tileMap[x + (int)pos.x - 1][y + (int)pos.y] != 1 || // west
                    m_tileMap[x + (int)pos.x - 1][y + (int)pos.y + 1] != 1 // northwest
                    )) ||
                    (entranceTiles.Contains(new Vector2(x, y)) && // if the tile is an entrance
                    (m_tileMap[x + (int)pos.x][y + (int)pos.y] == 0 || // if so, check if the same position on map isn't a wall
                    (Math.Abs(direction.x) == 1 && m_tileMap[x + (int)pos.x][y + (int)pos.y + 1] != 1) || // and check all tiles around the position on the map, starting with north
                    (Math.Abs(direction.y) == 1 && m_tileMap[x + (int)pos.x + 1][y + (int)pos.y] != 1) || // east
                    (Math.Abs(direction.x) == 1 && m_tileMap[x + (int)pos.x][y + (int)pos.y - 1] != 1) || // south
                    (Math.Abs(direction.y) == 1 && m_tileMap[x + (int)pos.x - 1][y + (int)pos.y] != 1)))) // west
                {
                    return false;
                }
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
                    if (CanPlace(tunnel, pos, entranceTiles, direction))
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
