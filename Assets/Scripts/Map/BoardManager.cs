using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class BoardManager : NetworkBehaviour
{
    [SerializeField]
    private GameObject m_mapPrefab;
    [SerializeField]
    private GameObject m_breakableWallPrefab;
    [SerializeField]
    private int m_outerWallWidth = 14;

    private int[][] m_tileMap;
    private GameObject m_map;
    private List<GameObject> m_mapParts = new List<GameObject>();

    // So that a position is in the middle of a tile
    private const float MAP_OFFSET = -0.5f;

    // --------------------------------------------------------------------------------------------
    // Public functions
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Creates a random map object 
    /// </summary>
    public void SetupScene(int[][] tileMap)
    {
        m_tileMap = tileMap;
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
    /// Gets a random floor tile
    /// </summary>
    /// <returns>Vector2 containing the position of the floortile</returns>
    public Vector2 GetRandomFloorTile()
    {
        while (true)
        {
            // get a random tile in the map
            Vector2 randomTile = new Vector2(UnityEngine.Random.Range(1, m_tileMap.Length - 1), UnityEngine.Random.Range(1, m_tileMap[0].Length - 1));

            if (m_tileMap[(int)randomTile.x][(int)randomTile.y] == 0)
            {
                return randomTile;
            }
        }
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

        floor.transform.position = new Vector3((float)m_tileMap.Length / 2, MAP_OFFSET, (float)m_tileMap[0].Length / 2);
        floor.transform.localScale = new Vector3((float)(m_tileMap.Length + m_outerWallWidth * 2) / 10, 1, (float)(m_tileMap[0].Length + m_outerWallWidth * 2) / 10);
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
                else if (m_tileMap[i][j] == 2 && isServer)
                {
                    GameObject instance = Instantiate(m_breakableWallPrefab, new Vector3(i, 0f, j), Quaternion.identity);
                    instance.name = "BreakableWall";
                    instance.transform.SetParent(m_map.transform);
                    NetworkServer.Spawn(instance);
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
