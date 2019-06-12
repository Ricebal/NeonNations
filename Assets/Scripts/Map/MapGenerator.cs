using System;
using System.Collections.Generic;
using UnityEngine;

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
    private int m_reflectorAreaSize;
    private int m_spawnAreaMinSize;

    private const int MAX_PLACE_ATTEMPTS = 10;
    private const int MAX_BUILD_ATTEMPTS = 250;
    private const int MAX_SHORTCUT_ATTEMPTS = 250;
    private const int MAX_WALL_ATTEMPTS = 250;
    private const int MINIMUM_ROOM_LENGTH = 1;
    private const int MINIMUM_MAP_SIDE = 20;
    private const int MAXIMUM_MAP_SIDE = 400;

    private Map m_map;

    // --------------------------------------------------------------------------------------------
    // Generate maps
    // --------------------------------------------------------------------------------------------
    public MapGenerator(int mapWidth, int mapHeight, int maxRoomAmount, int maxShortcutAmount, int minRoomLength, int maxRoomLength, int minTunnelLength, int maxTunnelLength, int tunnelWidth, int breakableTunnelChance, int shortcutMinSkipDistance, int reflectorAreaSize)
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
        m_reflectorAreaSize = reflectorAreaSize;
        m_spawnAreaMinSize = Math.Min(m_tunnelWidth * m_maxTunnelLength, (int)Math.Pow(Math.Min(mapWidth, mapHeight), 2));
    }

    /// <summary>
    /// Generates the hardcoded test map
    /// </summary>
    /// <returns>A Map object containing a test map</returns>
    public Map GenerateTestMap()
    {
        m_map = Map.GenerateEmptyMap(Tile.Wall, m_mapWidth, m_mapHeight, m_spawnAreaMinSize);

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
                    m_map.TileMap[x][y] = Tile.Floor;
                }
            }
        }
        return m_map;
    }

    /// <summary>
    /// Generates a random map, based on the settings
    /// </summary>
    /// <param name="seed">Seed to be used with the generation</param>
    /// <returns>A Map object containing the random map</returns>
    public Map GenerateRandomMap(string seed)
    {
        // Check if settings can be used
        CheckSettings();

        // Create level with only walls
        m_map = Map.GenerateEmptyMap(Tile.Wall, m_mapWidth, m_mapHeight, m_spawnAreaMinSize);

        // Change seed of randomizer
        UnityEngine.Random.InitState(seed.GetHashCode());

        // Generate first room (in the middle) and add it to the map
        Map room = GenerateRandomRoom();
        room.Pos = new Vector2Int(m_mapWidth / 2 - room.TileMap.Length / 2, m_mapHeight / 2 - room.TileMap[0].Length / 2);
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

        // Add reflectors
        AddReflectors();

        // Set seed to random again
        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);

        return m_map;
    }

    // --------------------------------------------------------------------------------------------
    // Random generation functions
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Generates a random square room based on the settings
    /// </summary>
    /// <returns>A Map object containing the room</returns>
    private Map GenerateRandomRoom()
    {
        int width = UnityEngine.Random.Range(m_minRoomLength, Math.Min(m_maxRoomLength, m_mapWidth));
        int height = UnityEngine.Random.Range(m_minRoomLength, Math.Min(m_maxRoomLength, m_mapHeight));

        return Map.GenerateEmptyMap(Tile.Floor, width, height, m_spawnAreaMinSize);
    }

    /// <summary>
    /// Adds the room to the map
    /// </summary>
    /// <param name="room">Room to be added to the map</param>
    private void AddRoom(Map room)
    {
        m_map.PasteTileMap(room.Pos, room.TileMap);
    }

    /// <summary>
    /// Try to place the room within the map
    /// </summary>
    /// <param name="room">Room to be added to the map</param>
    /// <returns>True if successful</returns>
    private bool PlaceRoom(Map room)
    {
        for (int i = 1; i <= MAX_PLACE_ATTEMPTS; i++)
        {
            Map tryMap = TryPlacement(room);
            if (tryMap != null)
            {
                // add room to the map
                AddRoom(tryMap);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Generates a random position on the map to add a room, and sees if the room fits there
    /// </summary>
    /// <param name="room">Room to be added</param>
    /// <returns>Returns a valid map if succeeded, returns null when failed</returns>
    private Map TryPlacement(Map room)
    {
        // get random direction to attach the room to the map
        Vector2Int direction = Map.Directions[UnityEngine.Random.Range(0, 4)]; // max is exclusive, so this will generate number between 0 and 3

        // get random walltile for that direction
        Vector2Int wallTile = m_map.GetRandomWallTile(direction, m_tunnelWidth);
        if (wallTile.x == -1 && wallTile.y == -1)
        {
            return null;
        }

        // get a random tile within the room, to attach the corridor to
        Vector2Int entrance = new Vector2Int(UnityEngine.Random.Range(0, room.TileMap.Length + 1 - m_tunnelWidth), UnityEngine.Random.Range(0, room.TileMap[0].Length + 1 - m_tunnelWidth));
        if (direction == Vector2Int.down)
        {
            entrance.y = room.TileMap[0].Length - 1;
        }
        else if (direction == Vector2Int.up)
        {
            entrance.y = 0;
        }
        else if (direction == Vector2Int.left)
        {
            entrance.x = room.TileMap.Length - 1;
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
            Map tempMap = room.AddTunnelToMap(i, entrance, direction, m_tunnelWidth);
            tempMap.Pos = new Vector2Int(room.Pos.x, room.Pos.y);
            Vector2Int tempEntrance = new Vector2Int(entrance.x, entrance.y);

            // adjust position and entrance of the room based on the length of the tunnel
            if (direction == Vector2Int.down)
            {
                tempMap.Pos.y = wallTile.y + 1 - tempMap.TileMap[0].Length;
                tempEntrance.y += i;
            }
            else if (direction == Vector2Int.left)
            {
                tempMap.Pos.x = wallTile.x + 1 - tempMap.TileMap.Length;
                tempEntrance.x += i;
            }

            // calculate all entrance tiles
            List<Vector2Int> entranceTiles = new List<Vector2Int>();
            for (int j = 0; j < m_tunnelWidth; j++)
            {
                entranceTiles.Add(new Vector2Int(tempEntrance.x + Math.Abs(direction.y) * j, tempEntrance.y + Math.Abs(direction.x) * j));
            }

            // if it can be placed, return the room
            if (CanPlace(tempMap, entranceTiles, direction))
            {
                return tempMap;
            }
        }

        // for every length of the tunnel, this room doesn't fit. return null
        return null;
    }

    /// <summary>
    /// Checks if a map can be added to the m_map
    /// </summary>
    /// <param name="map">Small map to be added</param>
    /// <param name="entranceTiles">Tiles that should be excluded of checking</param>
    /// <param name="direction">Direction of entrance</param>
    /// <returns>True if the map can indeed be placed inside the m_map</returns>
    private bool CanPlace(Map map, List<Vector2Int> entranceTiles, Vector2Int direction)
    {
        // check out of bounds
        if (m_map.CheckOutOfBounds(map.Pos, new Vector2Int(map.Pos.x + map.TileMap.Length, map.Pos.y + map.TileMap[0].Length)))
        {
            return false;
        }

        // check overlap
        for (int x = 0; x < map.TileMap.Length; x++)
        {
            for (int y = 0; y < map.TileMap[0].Length; y++)
            {
                if (map.TileMap[x][y] == Tile.Floor && // check if position in room is a floor tile
                    (!entranceTiles.Contains(new Vector2Int(x, y)) && // if the tile isn't an entrance
                    (m_map.TileMap[x + map.Pos.x][y + map.Pos.y] != Tile.Wall || // if so, check if the same position on map isn't a wall
                    m_map.TileMap[x + map.Pos.x][y + map.Pos.y + 1] != Tile.Wall || // and check all tiles around the position on the map, starting with north
                    m_map.TileMap[x + map.Pos.x + 1][y + map.Pos.y + 1] != Tile.Wall || // northeast
                    m_map.TileMap[x + map.Pos.x + 1][y + map.Pos.y] != Tile.Wall || // east
                    m_map.TileMap[x + map.Pos.x + 1][y + map.Pos.y - 1] != Tile.Wall || // southeast
                    m_map.TileMap[x + map.Pos.x][y + map.Pos.y - 1] != Tile.Wall || // south
                    m_map.TileMap[x + map.Pos.x - 1][y + map.Pos.y - 1] != Tile.Wall || // southwest
                    m_map.TileMap[x + map.Pos.x - 1][y + map.Pos.y] != Tile.Wall || // west
                    m_map.TileMap[x + map.Pos.x - 1][y + map.Pos.y + 1] != Tile.Wall // northwest
                    )) ||
                    (entranceTiles.Contains(new Vector2Int(x, y)) && // if the tile is an entrance
                    (m_map.TileMap[x + map.Pos.x][y + map.Pos.y] != Tile.Wall || // if so, check if the same position on map isn't a wall
                    (Math.Abs(direction.x) == 1 && m_map.TileMap[x + map.Pos.x][y + map.Pos.y + 1] != Tile.Wall) || // and check certain tiles around the position on the map, based on direction, starting with north
                    (Math.Abs(direction.y) == 1 && m_map.TileMap[x + map.Pos.x + 1][y + map.Pos.y] != Tile.Wall) || // east
                    (Math.Abs(direction.x) == 1 && m_map.TileMap[x + map.Pos.x][y + map.Pos.y - 1] != Tile.Wall) || // south
                    (Math.Abs(direction.y) == 1 && m_map.TileMap[x + map.Pos.x - 1][y + map.Pos.y] != Tile.Wall)))) // west
                {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Add shortcuts to m_map based on the settings
    /// </summary>
    private void AddShortcuts()
    {
        int currentShortcutAmount = 0;

        while (currentShortcutAmount < m_maxShortcutAmount)
        {
            // get list of all wall tiles and their direction
            bool shortcutPlaced = false;
            List<KeyValuePair<Vector2Int, Vector2Int>> list = m_map.GetAllWallTiles(1, m_tunnelWidth);

            for (int i = 0; i < list.Count; i++)
            {
                Vector2Int wallTile = list[i].Key;
                Vector2Int otherWallTile = Vector2Int.zero;
                Vector2Int direction = list[i].Value;

                for (int j = m_minTunnelLength - 1; j < m_maxTunnelLength - 1; j++)
                {
                    otherWallTile = new Vector2Int(wallTile.x + direction.x * (j - 1), (wallTile.y + direction.y * (j - 1)));
                    if (!(otherWallTile.x <= 1 || otherWallTile.x >= m_mapWidth - 1 || otherWallTile.y <= 1 || otherWallTile.y >= m_mapHeight - 1)
                        && m_map.CheckWallTile(otherWallTile, new Vector2Int(direction.x * -1, direction.y * -1), m_tunnelWidth))
                    {
                        // generate tunnel
                        Map tunnel = new Map(new Tile[(m_tunnelWidth * Math.Abs(direction.y) + j * Math.Abs(direction.x))][], m_spawnAreaMinSize);

                        for (int k = 0; k < tunnel.TileMap.Length; k++)
                        {
                            tunnel.TileMap[k] = new Tile[(m_tunnelWidth * Math.Abs(direction.x) + j * Math.Abs(direction.y))];
                        }

                        // generate position of tunnel
                        tunnel.Pos = new Vector2Int();
                        if (direction == Vector2Int.right || direction == Vector2Int.up)
                        {
                            tunnel.Pos = wallTile;
                        }
                        else
                        {
                            tunnel.Pos = otherWallTile;
                        }

                        // set entrances
                        List<Vector2Int> entranceTiles = new List<Vector2Int>();
                        for (int k = 0; k < m_tunnelWidth; k++)
                        {
                            Vector2Int absDirection = new Vector2Int(Math.Abs(direction.x), Math.Abs(direction.y));
                            entranceTiles.Add(new Vector2Int(absDirection.y * k, absDirection.x * k));
                            entranceTiles.Add(new Vector2Int(absDirection.y * k + absDirection.x * (tunnel.TileMap.Length - 1), absDirection.y * (tunnel.TileMap[0].Length - 1) + absDirection.x * k));
                        }

                        // check if placeable
                        if (CanPlace(tunnel, entranceTiles, direction) && CheckIfUsefulToPlace(wallTile, otherWallTile, direction))
                        {
                            // if breakable, make entrance tiles a breakable block
                            if (IsBreakable())
                            {
                                for (int k = 0; k < entranceTiles.Count; k++)
                                {
                                    // change the according tile on the map
                                    tunnel.TileMap[entranceTiles[k].x][entranceTiles[k].y] = Tile.BreakableWall;
                                }
                            }

                            m_map.PasteTileMap(tunnel.Pos, tunnel.TileMap);
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
        return UnityEngine.Random.Range(1, 101) <= m_breakableTunnelChance;
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
        tilesToCalculateFrom[1] = new Vector2Int(wallTile.x - direction.x + Math.Abs(direction.y) * m_tunnelWidth, wallTile.y - direction.y + Math.Abs(direction.x) * m_tunnelWidth);
        tilesToCalculateTo[0] = new Vector2Int(otherWallTile.x + direction.x, otherWallTile.y + direction.y);
        tilesToCalculateTo[1] = new Vector2Int(otherWallTile.x + direction.x + Math.Abs(direction.y) * m_tunnelWidth, otherWallTile.y + direction.y + Math.Abs(direction.x) * m_tunnelWidth);

        // run DStarLite twice
        GameEnvironment ge = GameEnvironment.CreateInstance(m_map, new List<Tile>() { Tile.Wall, Tile.Reflector });
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

    /// <summary>
    /// Adds reflectors to m_map
    /// </summary>
    private void AddReflectors()
    {
        // setup
        Vector2Int startingPos = m_map.GetRandomFloorTile();
        Queue<Vector2Int> positionsToCheck = new Queue<Vector2Int>();
        positionsToCheck.Enqueue(startingPos);
        List<Vector2Int> positionsChecked = new List<Vector2Int>();

        // floodfill algorithm
        while (positionsToCheck.Count != 0 && positionsChecked.Count < m_reflectorAreaSize)
        {
            Vector2Int current = positionsToCheck.Dequeue();
            for (int i = 0; i < Map.Directions.Length; i++)
            {
                Vector2Int nextToCurrent = current + Map.Directions[i];
                // if the tile is a wall, convert to reflector
                if (m_map.TileMap[nextToCurrent.x][nextToCurrent.y] == Tile.Wall)
                {
                    m_map.TileMap[nextToCurrent.x][nextToCurrent.y] = Tile.Reflector;
                }

                // if the tile hasn't been checked and is not in the list to be checked, and it's either a floor or breakable wall, add to check list
                if (!positionsChecked.Contains(nextToCurrent) && !positionsToCheck.Contains(nextToCurrent)
                    && (m_map.TileMap[nextToCurrent.x][nextToCurrent.y] == Tile.Floor || m_map.TileMap[nextToCurrent.x][nextToCurrent.y] == Tile.BreakableWall))
                {
                    positionsToCheck.Enqueue(nextToCurrent);
                }
            }

            positionsChecked.Add(current);
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
}
