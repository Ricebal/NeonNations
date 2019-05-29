﻿using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class BoardManager : NetworkBehaviour
{
    // Walls is used for loading the map
    [Flags]
    private enum Walls
    {
        // Decimal     // Binary
        None = 0, // 000000
        Up = 1, // 000001
        Right = 2, // 000010
        Down = 4, // 000100
        Left = 8 // 001000
    }

    [SerializeField]
    private GameObject m_mapPrefab;
    [SerializeField]
    private GameObject m_breakableWallPrefab;
    [SerializeField]
    private GameObject m_reflectorPrefab;

    // So that a position is in the middle of a tile
    private const float MAP_OFFSET = -0.5f;
    private const int VERTEX_LIMIT = 30000;
    private const string MAP = "Map";
    private const string MAP_PART = "Map Part";
    private const string BREAKABLE_WALLS = "Breakable Walls";
    private const string BREAKABLE_WALL = "Breakable Wall";
    private const string REFLECTORS = "Reflectors";
    private const string REFLECTOR = "Reflector";

    private Map m_map;
    private int m_outerWallWidth;
    private GameObject m_mapObject;
    private GameObject m_breakableWalls;
    private GameObject m_reflectors;
    private List<GameObject> m_mapParts = new List<GameObject>();

    private readonly List<Tile> m_permanentObstacles = new List<Tile>() { Tile.Wall, Tile.Reflector };

    // --------------------------------------------------------------------------------------------
    // Public functions
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Creates a random map object 
    /// </summary>
    public void SetupScene(Map map, int outerWallWidth)
    {
        m_map = map;
        m_outerWallWidth = outerWallWidth;
        m_mapObject = new GameObject();
        m_mapObject.name = MAP;

        LoadFloor();
        LoadMap();
        CreateOuterWalls();
        CombineAllMeshes();
    }

    /// <summary>
    /// Gets the map
    /// </summary>
    /// <returns>A map</returns>
    public Map GetMap()
    {
        return m_map;
    }

    // --------------------------------------------------------------------------------------------
    // Create map functions
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Loads the floor of the map
    /// </summary>
    private void LoadFloor()
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);

        floor.transform.position = new Vector3((float)m_map.TileMap.Length / 2 + MAP_OFFSET, MAP_OFFSET, (float)m_map.TileMap[0].Length / 2 + MAP_OFFSET);
        floor.transform.localScale = new Vector3((float)(m_map.TileMap.Length) / 10, 1, (float)(m_map.TileMap[0].Length) / 10);
        floor.transform.SetParent(m_mapObject.transform);
    }

    /// <summary>
    /// Uses m_tilemap to load in the map
    /// </summary>
    private void LoadMap()
    {
        // init
        m_breakableWalls = new GameObject();
        m_breakableWalls.name = BREAKABLE_WALLS;
        m_breakableWalls.transform.SetParent(m_mapObject.transform);
        m_reflectors = new GameObject();
        m_reflectors.name = REFLECTORS;
        m_reflectors.transform.SetParent(m_mapObject.transform);
        List<KeyValuePair<Vector2Int,Vector2Int>> mirrorList = new List<KeyValuePair<Vector2Int, Vector2Int>>();

        for (int i = 0; i < m_map.TileMap.Length; i++)
        {
            for (int j = 0; j < m_map.TileMap[0].Length; j++)
            {
                if (m_map.TileMap[i][j] == Tile.Wall)
                {
                    GameObject instance = GameObject.CreatePrimitive(PrimitiveType.Cube);

                    // calculate walls
                    Walls walls = Walls.None;
                    if (j + 1 < m_map.TileMap[0].Length && !m_permanentObstacles.Contains(m_map.TileMap[i][j + 1]))
                    {
                        walls = walls | Walls.Up;
                    }
                    if (i + 1 < m_map.TileMap.Length && !m_permanentObstacles.Contains(m_map.TileMap[i + 1][j]))
                    {
                        walls = walls | Walls.Right;
                    }
                    if (j - 1 >= 0 && !m_permanentObstacles.Contains(m_map.TileMap[i][j - 1]))
                    {
                        walls = walls | Walls.Down;
                    }
                    if (i - 1 >= 0 && !m_permanentObstacles.Contains(m_map.TileMap[i - 1][j]))
                    {
                        walls = walls | Walls.Left;
                    }

                    instance.transform.GetComponent<MeshFilter>().sharedMesh = GenerateNewMesh(walls);

                    instance.transform.position = new Vector3(i + MAP_OFFSET, 0f, j + MAP_OFFSET);
                    instance.transform.SetParent(m_mapObject.transform);
                }
                else if (m_map.TileMap[i][j] == Tile.BreakableWall && isServer)
                {
                    GameObject instance = Instantiate(m_breakableWallPrefab, new Vector3(i, 0f, j), Quaternion.identity);
                    instance.name = BREAKABLE_WALL;
                    instance.transform.SetParent(m_breakableWalls.transform);
                    NetworkServer.Spawn(instance);
                }
                else if (m_map.TileMap[i][j] == Tile.Reflector)
                {
                    GameObject instance;
                    Vector2Int[] directions = new Vector2Int[] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
                    for (int k = 0; k < directions.Length; k++)
                    {
                        if (!mirrorList.Contains(new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i, j), directions[k])))
                        {
                            List<KeyValuePair<Vector2Int, Vector2Int>> currentList = new List<KeyValuePair<Vector2Int, Vector2Int>>();
                            GetAdjacentMirrors(currentList, new Vector2Int(i, j), directions[k]);
                            if (currentList.Count > 0)
                            {
                                instance = Instantiate(m_reflectorPrefab, new Vector3(i, 0f, j), Quaternion.identity);
                                instance.name = REFLECTOR;
                                instance.transform.Translate(new Vector3(Math.Abs(directions[k].y) * ((float)(0.5 * currentList.Count) - 0.5f) + directions[k].x*0.5f, 0f, Math.Abs(directions[k].x) * ((float)(0.5 * currentList.Count) - 0.5f) + directions[k].y * 0.5f));
                                instance.transform.rotation = Quaternion.AngleAxis((Math.Abs(directions[k].y) * ((directions[k].y + 1) * 90)) + (Math.Abs(directions[k].x) * (90 + (directions[k].x + 1) * 90)), Vector3.up);
                                instance.transform.localScale = new Vector3(currentList.Count, 1, 1);
                                instance.transform.SetParent(m_reflectors.transform);

                                mirrorList.AddRange(currentList);
                                currentList.Clear();
                            }
                        }
                    }

                    instance = GameObject.CreatePrimitive(PrimitiveType.Cube);

                    // calculate walls
                    Walls walls = Walls.None;

                    instance.transform.GetComponent<MeshFilter>().sharedMesh = GenerateNewMesh(walls);

                    instance.transform.position = new Vector3(i + MAP_OFFSET, 0f, j + MAP_OFFSET);
                    instance.transform.SetParent(m_mapObject.transform);
                }
            }
        }
    }

    /// <summary>
    /// Creates the outer walls for the map
    /// </summary>
    private void CreateOuterWalls()
    {
        // since (0,0) in the tilemap is (0,0) in unity, the down left corner of the map is (-outerwallwidth,-outerwallwidth) and the up right corner is ((maplength-1)+outerwallwidth,(maplength-1)+outerwallwidth)
        for (int i = -m_outerWallWidth; i < m_map.TileMap.Length + m_outerWallWidth; i++)
        {
            for (int j = -m_outerWallWidth; j < m_map.TileMap[0].Length + m_outerWallWidth; j++)
            {
                if (i < 0 || i >= m_map.TileMap.Length || j < 0 || j >= m_map.TileMap[0].Length)
                {
                    GameObject instance = GameObject.CreatePrimitive(PrimitiveType.Cube);

                    instance.transform.GetComponent<MeshFilter>().sharedMesh = GenerateNewMesh(Walls.None);

                    instance.transform.position = new Vector3(i + MAP_OFFSET, 0f, j + MAP_OFFSET);
                    instance.transform.SetParent(m_mapObject.transform);
                }
            }
        }
    }

    /// <summary>
    /// Creates a new mesh based on the walls given
    /// </summary>
    /// <param name="walls">Used to see which vertices should be generated</param>
    /// <returns>The new mesh</returns>
    private Mesh GenerateNewMesh(Walls walls)
    {
        // all the vertices needed for the different faces
        // vertices can't be shared between the triangles, because the shader can't handle that
        Vector3[] vertices = {
            new Vector3(1, 0.5f, 0),
            new Vector3(0, 0.5f, 0),
            new Vector3(0, 0.5f, 1),
            new Vector3(1, 0.5f, 1),

            new Vector3(1, 0.5f, 1),
            new Vector3(0, 0.5f, 1),
            new Vector3(0, -0.5f, 1),
            new Vector3(1, -0.5f, 1),

            new Vector3(1, -0.5f, 0),
            new Vector3(1, 0.5f, 0),
            new Vector3(1, 0.5f, 1),
            new Vector3(1, -0.5f, 1),

            new Vector3(0, -0.5f, 0),
            new Vector3(1, 0.5f, 0),
            new Vector3(1, -0.5f, 0),
            new Vector3(0, 0.5f, 0),

            new Vector3(0, -0.5f, 0),
            new Vector3(0, -0.5f, 1),
            new Vector3(0, 0.5f, 1),
            new Vector3(0, 0.5f, 0)
        };

        int[] faceTop = { 0, 1, 2, 0, 2, 3 };
        int[] faceUp = { 4, 5, 6, 4, 6, 7 };
        int[] faceRight = { 8, 9, 10, 8, 10, 11 };
        int[] faceDown = { 12, 13, 14, 12, 15, 13 };
        int[] faceLeft = { 16, 17, 18, 16, 18, 19 };

        List<int> triangles = new List<int>();

        triangles.AddRange(faceTop);

        if (walls.HasFlag(Walls.Up))
        {
            triangles.AddRange(faceUp);
        }
        if (walls.HasFlag(Walls.Right))
        {
            triangles.AddRange(faceRight);
        }
        if (walls.HasFlag(Walls.Down))
        {
            triangles.AddRange(faceDown);
        }
        if (walls.HasFlag(Walls.Left))
        {
            triangles.AddRange(faceLeft);
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }

    /// <summary>
    /// Combines all the meshes of the map
    /// </summary>
    private void CombineAllMeshes()
    {
        // initialize
        int verticesSoFar = 0;
        List<MeshFilter> meshFilters = new List<MeshFilter>();
        foreach (Transform child in m_mapObject.transform)
        {
            if (child.gameObject.name != BREAKABLE_WALLS && child.gameObject.name != REFLECTORS)
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
            if (verticesSoFar + meshFilters[i].mesh.vertexCount > VERTEX_LIMIT)
            {
                // send list to be combined
                CreateCombinedMesh(combiners);
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
        CreateCombinedMesh(combiners);

        // destroy all the objects the map was made up of
        foreach (Transform child in m_mapObject.transform)
        {
            if (child.gameObject.name != BREAKABLE_WALLS && child.gameObject.name != REFLECTORS)
            {
                Destroy(child.gameObject);
            }
        }

        // add all the map parts to the map
        foreach (GameObject mapPart in m_mapParts)
        {
            mapPart.transform.SetParent(m_mapObject.transform);
        }
    }

    /// <summary>
    /// Adds a mappart based on the mesh data
    /// </summary>
    /// <param name="meshDataList">Data of the meshes used to create the mappart</param>
    private void CreateCombinedMesh(List<CombineInstance> meshDataList)
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
        mapPart.name = MAP_PART + " " + m_mapParts.Count;
    }

    private List<KeyValuePair<Vector2Int, Vector2Int>> GetAdjacentMirrors(List<KeyValuePair<Vector2Int, Vector2Int>> list, Vector2Int pos, Vector2Int direction)
    {
        if (m_map.TileMap[pos.x][pos.y] == Tile.Reflector)
        {
            if (direction.x == 1)
            {
                if (pos.x + 1 < m_map.TileMap.Length && !m_permanentObstacles.Contains(m_map.TileMap[pos.x + 1][pos.y]))
                {
                    list.Add(new KeyValuePair<Vector2Int, Vector2Int>(pos, direction));
                    GetAdjacentMirrors(list, new Vector2Int(pos.x, pos.y + 1), direction);
                }
            }
            else if (direction.x == -1)
            {
                if (pos.x - 1 >= 0 && !m_permanentObstacles.Contains(m_map.TileMap[pos.x - 1][pos.y]))
                {
                    list.Add(new KeyValuePair<Vector2Int, Vector2Int>(pos, direction));
                    GetAdjacentMirrors(list, new Vector2Int(pos.x, pos.y + 1), direction);
                }
            }
            else if (direction.y == 1)
            {
                if (pos.y + 1 < m_map.TileMap[0].Length && !m_permanentObstacles.Contains(m_map.TileMap[pos.x][pos.y + 1]))
                {
                    list.Add(new KeyValuePair<Vector2Int, Vector2Int>(pos, direction));
                    GetAdjacentMirrors(list, new Vector2Int(pos.x + 1, pos.y), direction);
                }
            }
            else if (direction.y == -1)
            {
                if (pos.y - 1 >= 0 && !m_permanentObstacles.Contains(m_map.TileMap[pos.x][pos.y - 1]))
                {
                    list.Add(new KeyValuePair<Vector2Int, Vector2Int>(pos, direction));
                    GetAdjacentMirrors(list, new Vector2Int(pos.x + 1, pos.y), direction);
                }
            }
        }
        return list;
    }
}
