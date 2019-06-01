using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum Tile { Unknown = -1, Floor, Wall, BreakableWall }

public class MapGenerator
{
    private int m_mapWidth;
    private int m_mapHeight;
    private int m_maxRoomAmount;
    private int m_maxShortcutAmount;
    private int m_minRoomLength;
    private int m_maxRoomLength;
    private int m_minTunnelLength;
    private int m_maxTunnelLength;
    private int m_tunnelWidth;
    private int m_breakableTunnelChance;
    private int m_shortcutMinSkipDistance;

    private const int MAX_PLACE_ATTEMPTS = 10;
    private const int MAX_BUILD_ATTEMPTS = 250;
    private const int MAX_SHORTCUT_ATTEMPTS = 250;
    private const int MAX_WALL_ATTEMPTS = 250;
    private const int MINIMUM_ROOM_LENGTH = 1;
    private const int MINIMUM_MAP_SIDE = 20;
    private const int MAXIMUM_MAP_SIDE = 400;

    private Tile[][] m_tileMap;

    // --------------------------------------------------------------------------------------------
    // Generate maps
    // --------------------------------------------------------------------------------------------
    public MapGenerator(int mapWidth, int mapHeight, int maxRoomAmount, int maxShortcutAmount, int minRoomLength, int maxRoomLength, int minTunnelLength, int maxTunnelLength, int tunnelWidth, int breakableTunnelChance, int shortcutMinSkipDistance)
    {
        m_mapWidth = mapWidth;
        m_mapHeight = mapHeight;
        m_maxRoomAmount = maxRoomAmount;
        m_maxShortcutAmount = maxShortcutAmount;
        m_minRoomLength = minRoomLength;
        m_maxRoomLength = maxRoomLength;
        m_minTunnelLength = minTunnelLength;
        m_maxTunnelLength = maxTunnelLength;
        m_tunnelWidth = tunnelWidth;
        m_breakableTunnelChance = breakableTunnelChance;
        m_shortcutMinSkipDistance = shortcutMinSkipDistance;
    }

    /// <summary>
    /// Generates a map based on the content
    /// </summary>
    /// <param name="content">The tile to fill the map with</param>
    /// <returns>A jagged array of Tiles representing the tilemap</returns>
    public Tile[][] GenerateEmptyMap(Tile content)
    {
        m_tileMap = new Tile[m_mapWidth][];
        for (int x = 0; x < m_mapWidth; x++)
        {
            m_tileMap[x] = new Tile[m_mapHeight];
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
    /// <returns>A jagged array of Tiles representing the tilemap</returns>
    public Tile[][] GenerateTestMap()
    {
        GenerateEmptyMap(Tile.Wall);

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
                    m_tileMap[x][y] = Tile.Floor;
                }
            }
        }
        return m_tileMap;
    }

    /// <summary>
    /// Generates a random map, based on the settings
    /// </summary>
    /// <param name="seed">Seed to be used with the generation</param>
    /// <returns>A jagged array of Tiles representing the tilemap</returns>
    public Tile[][] GenerateRandomMap(string seed)
    {
        // Check if settings can be used
        CheckSettings();

        // Create level with only walls
        GenerateEmptyMap(Tile.Wall);

        // Change seed of randomizer
        UnityEngine.Random.InitState(seed.GetHashCode());

        // Generate first room (in the middle) and add it to the map
        Room room = GenerateRandomRoom();
        room.Pos = new Vector2Int(m_mapWidth / 2 - room.RoomMap.Length / 2, m_mapHeight / 2 - room.RoomMap[0].Length / 2);
        AddRoom(room);

        int currentRoomAmount = 1;
        int currentBuildAttempt = 0;
        while (currentBuildAttempt < MAX_BUILD_ATTEMPTS && currentRoomAmount < m_maxRoomAmount)
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
        int width = UnityEngine.Random.Range(m_minRoomLength, Math.Min(m_maxRoomLength, m_mapWidth));
        int height = UnityEngine.Random.Range(m_minRoomLength, Math.Min(m_maxRoomLength, m_mapHeight));

        Tile[][] roomMap = new Tile[width][];
        for (int x = 0; x < width; x++)
        {
            roomMap[x] = new Tile[height];
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
    private void PasteTileMap(Tile[][] smallMap, Tile[][] bigMap, Vector2Int pos)
    {
        // for every tile in the small map
        for (int x = 0; x < smallMap.Length; x++)
        {
            for (int y = 0; y < smallMap[0].Length; y++)
            {
                // change the according tile on the big map
                if (smallMap[x][y] != Tile.Wall)
                {
                    bigMap[x + pos.x][y + pos.y] = smallMap[x][y];
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
        for (int i = 1; i <= MAX_PLACE_ATTEMPTS; i++)
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
        Vector2Int direction = GenerateRandomDirection();

        // get random walltile for that direction
        Vector2Int wallTile = GetRandomWallTile(direction);
        if (wallTile.x == -1 && wallTile.y == -1)
        {
            return false;
        }

        // get a random tile within the room, to attach the corridor to
        Vector2Int entrance = new Vector2Int(UnityEngine.Random.Range(0, room.RoomMap.Length + 1 - m_tunnelWidth), UnityEngine.Random.Range(0, room.RoomMap[0].Length + 1 - m_tunnelWidth));
        if (direction == Vector2Int.down)
        {
            entrance.y = room.RoomMap[0].Length - 1;
        }
        else if (direction == Vector2Int.up)
        {
            entrance.y = 0;
        }
        else if (direction == Vector2Int.left)
        {
            entrance.x = room.RoomMap.Length - 1;
        }
        else if (direction == Vector2Int.right)
        {
            entrance.x = 0;
        }

        // determine the position of the room within the map, based on the walltile and the tile within the room
        room.Pos = new Vector2Int(wallTile.x - entrance.x, wallTile.y - entrance.y);

        // generate random ordered list of tunnel lengths
        IEnumerable<int> tunnelLengths = UniqueRandom(m_minTunnelLength, m_maxTunnelLength);

        foreach (int i in tunnelLengths)
        {
            // create temporary values for this length
            Tile[][] tempMap = AddTunnelToMap(room.RoomMap, i, entrance, direction);
            Vector2Int tempEntrance = new Vector2Int(entrance.x, entrance.y);
            Vector2Int tempPos = new Vector2Int(room.Pos.x, room.Pos.y);

            // adjust position and entrance of the room based on the length of the tunnel
            if (direction == Vector2Int.down)
            {
                tempPos.y = wallTile.y + 1 - tempMap[0].Length;
                tempEntrance.y += i;
            }
            else if (direction == Vector2Int.left)
            {
                tempPos.x = wallTile.x + 1 - tempMap.Length;
                tempEntrance.x += i;
            }

            // calculate all entrance tiles
            List<Vector2Int> entranceTiles = new List<Vector2Int>();
            for (int j = 0; j < m_tunnelWidth; j++)
            {
                entranceTiles.Add(new Vector2Int(tempEntrance.x + Math.Abs(direction.y) * j, tempEntrance.y + Math.Abs(direction.x) * j));
            }

            // if it can be placed, return the room
            if (CanPlace(tempMap, tempPos, entranceTiles, direction))
            {
                room.RoomMap = tempMap;
                room.Pos = tempPos;
                return true;
            }
        }

        // for every length of the tunnel, this room doesn't fit. return false
        return false;
    }

    /// <summary>
    /// Generate a random direction
    /// </summary>
    /// <returns>Vector2Int which contains a random direction</returns>
    private Vector2Int GenerateRandomDirection()
    {
        return GetDirection(UnityEngine.Random.Range(1, 5)); // max is exclusive, so this will generate number between 1 and 4
    }

    /// <summary>
    /// Gets a direction based on an int
    /// </summary>
    /// <param name="directionInt">int indicating the direction (1 = north, 2, = east, 3 = south, 4 = west)</param>
    /// <returns>Vector2Int which contains the direction</returns>
    private Vector2Int GetDirection(int directionInt)
    {
        switch (directionInt)
        {
            case 1:
                return Vector2Int.up;
            case 2:
                return Vector2Int.right;
            case 3:
                return Vector2Int.down;
            case 4:
                return Vector2Int.left;
            default:
                return Vector2Int.zero;
        }
    }

    /// <summary>
    /// Gets a random wall tile based on a direction
    /// </summary>
    /// <param name="direction">Direction the tunnel will face if added to the wall</param>
    /// <returns>A Vector2Int containing the position of the wall</returns>
    private Vector2Int GetRandomWallTile(Vector2Int direction)
    {
        List<Vector2Int> list = GetAllWallTilesInDirection(direction, 1).ToList();
        if (list.Count > 0)
        {
            int r = UnityEngine.Random.Range(0, list.Count);
            return list[r];
        }

        return new Vector2Int(-1, -1);
    }

    /// <summary>
    /// Gets a list of all the wall tiles in a certain direction
    /// </summary>
    /// <param name="direction">Direction that should be searched</param>
    /// <param name="outerWidth">The distance between the edge of the map and the area that should be searched</param>
    /// <returns></returns>
    private IEnumerable<Vector2Int> GetAllWallTilesInDirection(Vector2Int direction, int outerWidth)
    {
        for (int i = outerWidth; i < m_tileMap.Length - m_tunnelWidth - outerWidth; i++)
        {
            for (int j = outerWidth; j < m_tileMap[0].Length - m_tunnelWidth - outerWidth; j++)
            {
                Vector2Int tile = new Vector2Int(i, j);
                if (CheckWallTile(tile, direction))
                {
                    yield return (tile);
                }
            }
        }
    }

    /// <summary>
    /// Gets a list of all the wall tiles in the map, and what their direction is
    /// </summary>
    /// <param name="outerWidth">The distance between the edge of the map and the area that should be searched</param>
    /// <returns>List containing all the wall tiles</returns>
    private List<KeyValuePair<Vector2Int, Vector2Int>> GetAllWallTiles(int outerWidth)
    {
        List<KeyValuePair<Vector2Int, Vector2Int>> result = new List<KeyValuePair<Vector2Int, Vector2Int>>();

        for (int i = 1; i <= 4; i++)
        {
            Vector2Int direction = GetDirection(i);
            List<Vector2Int> list = GetAllWallTilesInDirection(direction, outerWidth).ToList();
            for (int j = 0; j < list.Count; j++)
            {
                result.Add(new KeyValuePair<Vector2Int, Vector2Int>(list[j], direction));
            }
        }

        Shuffle(result);
        return result;
    }

    /// <summary>
    /// Checks if the given tile is a wall in the given Direction
    /// </summary>
    /// <param name="randomTile">Tile to be checked</param>
    /// <param name="direction">Direction the tunnel will face if added to the wall</param>
    /// <returns>True if tile is indeed a wall based on the direction</returns>
    private bool CheckWallTile(Vector2Int randomTile, Vector2Int direction)
    {
        // check if the surrounding tiles are suitable for the width of the tunnel
        for (int i = 0; i < m_tunnelWidth; i++)
        {
            Vector2Int wallTile = new Vector2Int(randomTile.x + i * Math.Abs(direction.y), randomTile.y + i * Math.Abs(direction.x));
            if (!(m_tileMap[wallTile.x][wallTile.y] == Tile.Wall
                    //&& m_tileMap[wallTile.x + direction.x][wallTile.y + direction.y] == Tile.Wall
                    && m_tileMap[wallTile.x - direction.x][wallTile.y - direction.y] == Tile.Floor))
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
    private Tile[][] AddTunnelToMap(Tile[][] map, int tunnelLength, Vector2Int entrance, Vector2Int direction)
    {
        // create new map with right size
        Tile[][] newMap = new Tile[map.Length + Math.Abs(direction.x) * tunnelLength][];
        for (int x = 0; x < newMap.Length; x++)
        {
            newMap[x] = new Tile[map[0].Length + Math.Abs(direction.y) * tunnelLength];
            for (int y = 0; y < newMap[0].Length; y++)
            {
                newMap[x][y] = Tile.Wall;
            }
        }

        // copy roommap into new map
        PasteTileMap(map, newMap, new Vector2Int(Math.Abs(Math.Max(0, direction.x)) * tunnelLength, Math.Abs(Math.Max(0, direction.y)) * tunnelLength));

        // generate tunnel
        Tile[][] tunnel = new Tile[(m_tunnelWidth * Math.Abs(direction.y) + tunnelLength * Math.Abs(direction.x))][];

        for (int x = 0; x < tunnel.Length; x++)
        {
            tunnel[x] = new Tile[(m_tunnelWidth * Math.Abs(direction.x) + tunnelLength * Math.Abs(direction.y))];
        }

        // generate position within new map
        Vector2Int pos = new Vector2Int(entrance.x + Math.Abs(Math.Max(0, direction.x)) * tunnelLength, entrance.y + Math.Abs(Math.Max(0, direction.y)) * tunnelLength);
        if (direction == Vector2Int.right)
        {
            pos.x -= tunnelLength;
        }
        else if (direction == Vector2Int.left)
        {
            pos.x += 1;
        }
        else if (direction == Vector2Int.up)
        {
            pos.y -= tunnelLength;
        }
        else if (direction == Vector2Int.down)
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
    private bool CanPlace(Tile[][] map, Vector2Int pos, List<Vector2Int> entranceTiles, Vector2Int direction)
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
                if (map[x][y] == Tile.Floor && // check if position in room is a floor tile
                    (!entranceTiles.Contains(new Vector2Int(x, y)) && // if the tile isn't an entrance
                    (m_tileMap[x + pos.x][y + pos.y] != Tile.Wall || // if so, check if the same position on map isn't a wall
                    m_tileMap[x + pos.x][y + pos.y + 1] != Tile.Wall || // and check all tiles around the position on the map, starting with north
                    m_tileMap[x + pos.x + 1][y + pos.y + 1] != Tile.Wall || // northeast
                    m_tileMap[x + pos.x + 1][y + pos.y] != Tile.Wall || // east
                    m_tileMap[x + pos.x + 1][y + pos.y - 1] != Tile.Wall || // southeast
                    m_tileMap[x + pos.x][y + pos.y - 1] != Tile.Wall || // south
                    m_tileMap[x + pos.x - 1][y + pos.y - 1] != Tile.Wall || // southwest
                    m_tileMap[x + pos.x - 1][y + pos.y] != Tile.Wall || // west
                    m_tileMap[x + pos.x - 1][y + pos.y + 1] != Tile.Wall // northwest
                    )) ||
                    (entranceTiles.Contains(new Vector2Int(x, y)) && // if the tile is an entrance
                    (m_tileMap[x + pos.x][y + pos.y] != Tile.Wall || // if so, check if the same position on map isn't a wall
                    (Math.Abs(direction.x) == 1 && m_tileMap[x + pos.x][y + pos.y + 1] != Tile.Wall) || // and check all tiles around the position on the map, starting with north
                    (Math.Abs(direction.y) == 1 && m_tileMap[x + pos.x + 1][y + pos.y] != Tile.Wall) || // east
                    (Math.Abs(direction.x) == 1 && m_tileMap[x + pos.x][y + pos.y - 1] != Tile.Wall) || // south
                    (Math.Abs(direction.y) == 1 && m_tileMap[x + pos.x - 1][y + pos.y] != Tile.Wall)))) // west
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
        int currentShortcutAmount = 0;

        while (currentShortcutAmount < m_maxShortcutAmount)
        {
            // get list of all wall tiles and their direction
            bool shortcutPlaced = false;
            List<KeyValuePair<Vector2Int, Vector2Int>> list = GetAllWallTiles(1);

            for (int i = 0; i < list.Count; i++)
            {
                Vector2Int wallTile = list[i].Key;
                Vector2Int otherWallTile = Vector2Int.zero;
                Vector2Int direction = list[i].Value;

                for (int j = m_minTunnelLength - 1; j < m_maxTunnelLength - 1; j++)
                {
                    otherWallTile = new Vector2Int(wallTile.x + direction.x * (j - 1), (wallTile.y + direction.y * (j - 1)));
                    if (!(otherWallTile.x <= 1 || otherWallTile.x >= m_mapWidth - 1 || otherWallTile.y <= 1 || otherWallTile.y >= m_mapHeight - 1)
                        && CheckWallTile(otherWallTile, new Vector2Int(direction.x * -1, direction.y * -1)))
                    {
                        // generate tunnel
                        Tile[][] tunnel = new Tile[(m_tunnelWidth * Math.Abs(direction.y) + j * Math.Abs(direction.x))][];

                        for (int k = 0; k < tunnel.Length; k++)
                        {
                            tunnel[k] = new Tile[(m_tunnelWidth * Math.Abs(direction.x) + j * Math.Abs(direction.y))];
                        }

                        // generate position of tunnel
                        Vector2Int pos = new Vector2Int();
                        if (direction == Vector2Int.right || direction == Vector2Int.up)
                        {
                            pos = wallTile;
                        }
                        else
                        {
                            pos = otherWallTile;
                        }

                        // set entrances
                        List<Vector2Int> entranceTiles = new List<Vector2Int>();
                        for (int k = 0; k < m_tunnelWidth; k++)
                        {
                            Vector2Int absDirection = new Vector2Int(Math.Abs(direction.x), Math.Abs(direction.y));
                            entranceTiles.Add(new Vector2Int(absDirection.y * k, absDirection.x * k));
                            entranceTiles.Add(new Vector2Int(absDirection.y * k + absDirection.x * (tunnel.Length - 1), absDirection.y * (tunnel[0].Length - 1) + absDirection.x * k));
                        }

                        // check if placeable
                        if (CanPlace(tunnel, pos, entranceTiles, direction) && CheckIfUsefulToPlace(wallTile, otherWallTile, direction))
                        {
                            // if breakable, make entrance tiles a breakable block
                            if (IsBreakable())
                            {
                                for (int k = 0; k < entranceTiles.Count; k++)
                                {
                                    // change the according tile on the map
                                    tunnel[entranceTiles[k].x][entranceTiles[k].y] = Tile.BreakableWall;
                                }
                            }

                            PasteTileMap(tunnel, m_tileMap, pos);
                            shortcutPlaced = true;
                            currentShortcutAmount++;
                            break;
                        }
                    }
                }
                if (shortcutPlaced)
                {
                    break;
                }
            }
            if (!shortcutPlaced)
            {
                break;
            }
        }
    }

    /// <summary>
    /// Determines if a shortcut should be secret or not
    /// </summary>
    /// <returns>True if secret</returns>
    private bool IsBreakable()
    {
        int i = UnityEngine.Random.Range(1, 101);
        return i <= m_breakableTunnelChance;
    }

    /// <summary>
    /// Calculates if shortcut should be placed. 
    /// </summary>
    /// <param name="wallTile"></param>
    /// <param name="otherWallTile"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    private bool CheckIfUsefulToPlace(Vector2Int wallTile, Vector2Int otherWallTile, Vector2Int direction)
    {
        // calculate tiles to run pathfinding on
        Vector2Int[] tilesToCalculateFrom = new Vector2Int[2];
        Vector2Int[] tilesToCalculateTo = new Vector2Int[2];
        double[] costs = new double[2];
        tilesToCalculateFrom[0] = new Vector2Int(wallTile.x - direction.x, wallTile.y - direction.y);
        tilesToCalculateFrom[1] = new Vector2Int(wallTile.x - direction.x + direction.y * m_tunnelWidth, wallTile.y - direction.y + direction.x * m_tunnelWidth);
        tilesToCalculateTo[0] = new Vector2Int(otherWallTile.x + direction.x, otherWallTile.y + direction.y);
        tilesToCalculateTo[1] = new Vector2Int(otherWallTile.x + direction.x + direction.y * m_tunnelWidth, otherWallTile.y + direction.y + direction.x * m_tunnelWidth);

        // run DStarLite twice
        GameEnvironment ge = GameEnvironment.CreateInstance(m_tileMap, new List<Tile>() { Tile.Wall });
        DStarLite dStarLite = new DStarLite(ge, true);
        dStarLite.RunDStarLite(tilesToCalculateFrom[0], tilesToCalculateTo[0]);
        costs[0] = dStarLite.Map.GetNode(tilesToCalculateFrom[0].x, tilesToCalculateFrom[0].y).CostFromStartingPoint;
        dStarLite.RunDStarLite(tilesToCalculateFrom[1], tilesToCalculateTo[1]);
        costs[1] = dStarLite.Map.GetNode(tilesToCalculateFrom[1].x, tilesToCalculateFrom[1].y).CostFromStartingPoint;

        // see if the distance between the two points without a shortcut is larger than the minimum distance
        if (costs[0] >= m_shortcutMinSkipDistance && costs[1] >= m_shortcutMinSkipDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
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

    /// <summary>
    /// Shuffles a list using the Fisher-Yates shuffle
    /// </summary>
    /// <param name="list">List to be shuffled</param>
    private void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    /// <summary>
    /// Checks if all the settings are correct, and corrects them if necessary
    /// </summary>
    private void CheckSettings()
    {
        // check map width
        if (m_mapWidth < MINIMUM_MAP_SIDE)
        {
            Debug.LogWarning("Map width can't be smaller than " + MINIMUM_MAP_SIDE + ", using " + MINIMUM_MAP_SIDE);
            m_mapWidth = MINIMUM_MAP_SIDE;
        }
        else if (m_mapWidth > MAXIMUM_MAP_SIDE)
        {
            Debug.LogWarning("Map width can't be bigger than " + MAXIMUM_MAP_SIDE + ", using " + MAXIMUM_MAP_SIDE);
            m_mapWidth = MAXIMUM_MAP_SIDE;
        }

        // check map height
        if (m_mapHeight < MINIMUM_MAP_SIDE)
        {
            Debug.LogWarning("Map height can't be smaller than " + MINIMUM_MAP_SIDE + ", using " + MINIMUM_MAP_SIDE);
            m_mapHeight = MINIMUM_MAP_SIDE;
        }
        else if (m_mapHeight > MAXIMUM_MAP_SIDE)
        {
            Debug.LogWarning("Map height can't be bigger than " + MAXIMUM_MAP_SIDE + ", using " + MAXIMUM_MAP_SIDE);
            m_mapHeight = MAXIMUM_MAP_SIDE;
        }

        // check minimum room length
        if (m_minRoomLength > Math.Min(m_mapWidth, m_mapHeight) - 2)
        {
            Debug.LogWarning("Minimum room length is too high for this map, using maximum size possible");
            m_minRoomLength = Math.Min(m_mapWidth, m_mapHeight) - 2;
        }
        else if (m_minRoomLength < MINIMUM_ROOM_LENGTH)
        {
            Debug.LogWarning("Minimum room length is too low, using " + MINIMUM_ROOM_LENGTH);
            m_minRoomLength = MINIMUM_ROOM_LENGTH;
        }

        // check maximum room length
        if (m_maxRoomLength < m_minRoomLength)
        {
            Debug.LogWarning("Maximum room length can't be smaller than minimum room length, using minimum length as maximum");
            m_maxRoomLength = m_minRoomLength;
        }
        else if (m_maxRoomLength > Math.Max(m_mapWidth, m_mapHeight) - 2)
        {
            Debug.LogWarning("Maximum room length is too big for this map size, using maximum length possible");
            m_maxRoomLength = Math.Max(m_mapWidth, m_mapHeight) - 2;
        }

        // check minimum tunnel length
        if (m_minTunnelLength > Math.Min(m_mapWidth, m_mapHeight) / 2 - m_maxRoomLength * 2)
        {
            Debug.LogWarning("Minimum tunnel length is too high for this map size, using minimum size possible");
            m_minTunnelLength = Math.Min(m_mapWidth, m_mapHeight) / 2 - m_maxRoomLength * 2;
        }
        if (m_minTunnelLength < 1)
        {
            Debug.LogWarning("Minimum tunnel length can't be lower than 1, using 1");
            m_minTunnelLength = 1;
        }

        // check maximum tunnel length
        if (m_maxTunnelLength < m_minTunnelLength)
        {
            Debug.LogWarning("Maximum tunnel length can't be lower than minimum tunnel length, using minimum length as maximum");
            m_maxTunnelLength = m_minTunnelLength;
        }
        else if (m_maxTunnelLength > Math.Max(m_mapWidth, m_mapHeight) - m_minRoomLength * 2)
        {
            Debug.LogWarning("Maximum tunnel length can't be higher than the highest of mapwidth and height, minus twice the minimum room length");
            m_maxTunnelLength = Math.Max(m_mapWidth, m_mapHeight) - m_minRoomLength * 2;
        }

        // check tunnel length
        if (m_tunnelWidth < 1)
        {
            Debug.LogWarning("Tunnel width can't be lower than 1, using 1");
            m_tunnelWidth = 1;
        }
        else if (m_tunnelWidth > m_minRoomLength)
        {
            Debug.LogWarning("Tunnel width can't be higher than minimum room length, using maximum");
            m_tunnelWidth = m_minRoomLength;
        }

        return;
    }


    // --------------------------------------------------------------------------------------------
    // Debug functions
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Prints the map given
    /// </summary>
    /// <param name="map">Map to print</param>
    private void DebugMap(Tile[][] map)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append('\n');
        for (int y = map[0].Length - 1; y >= 0; y--)
        {
            for (int x = 0; x < map.Length; x++)
            {
                builder.Append((int)map[x][y]);
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
    private void DebugAddingRoom(Tile[][] map, Tile[][] roomMap, Vector2Int pos)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append('\n');
        for (int y = map[0].Length - 1; y >= 0; y--)
        {
            for (int x = 0; x < map.Length; x++)
            {
                if (x >= pos.x && x < pos.x + roomMap.Length && y >= pos.y && y < pos.y + roomMap[0].Length)
                {
                    builder.Append((int)roomMap[x - pos.x][y - pos.y]);
                }
                else
                {
                    builder.Append((int)map[x][y]);
                }
            }
            builder.Append('\n');
        }
        string s = builder.ToString();
        Debug.Log(s);
    }

    private class Room
    {
        public Tile[][] RoomMap;
        public Vector2Int Pos;

        public Room(Tile[][] roomMap)
        {
            RoomMap = roomMap;
        }
    }
}
