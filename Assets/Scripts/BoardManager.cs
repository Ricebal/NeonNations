using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class BoardManager : MonoBehaviour
{
    // this can be used with random map generation
    //[Serializable]
    //public class Count
    //{
    //    public int minimum;
    //    public int maximum;

    //    public Count (int min, int max)
    //    {
    //        minimum = min;
    //        maximum = max;
    //    }
    //}

    public int Columns = 30;
    public int Rows = 22;
    public int OuterWallWidth = 14;
    public GameObject Map;

    private bool[,] m_tileMap;
    private GameObject m_map;

    void GenerateMap() {
        m_tileMap = new bool[Columns, Rows];

        for (int x = 0; x < Columns; x++) {
            for (int y = 0; y < Rows; y++) {
                if (// rooms
                    x == 0 && y == 0
                    || x >= 0 && x <= 9 && y >= 16 && y <= 21
                    || x >= 1 && x <= 7 && y >= 0 && y <= 5
                    || x >= 11 && x <= 15 && y >= 4 && y <= 10
                    || x >= 16 && x <= 20 && y >= 13 && y <= 18
                    || x >= 23 && x <= 29 && y >= 3 && y <= 10
                    || x >= 24 && x <= 27 && y >= 17 && y <= 21
                    || x >= 0 && x <= 20 && y >= 7 && y <= 20
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
                    || x >= 25 && x <= 26 && y >= 11 && y <= 16) {
                    m_tileMap[x, y] = true;
                }
            }
        }
    }

    void GenerateFloor() {
        m_map = Instantiate(Map, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.transform.position = new Vector3((float)Columns / 2 - 0.5f, -0.5f, (float)Rows / 2 - 0.5f);
        floor.transform.localScale = new Vector3((float)(Columns + OuterWallWidth * 2) / 10, 1, (float)(Rows + OuterWallWidth * 2) / 10);
        floor.transform.SetParent(m_map.transform);
    }

    void LoadMap() {
        for (int i = 0; i < m_tileMap.GetLength(0); i++) {
            for (int j = 0; j < m_tileMap.GetLength(1); j++) {
                if (!m_tileMap[i, j]) { // if false, build wall
                    GameObject instance = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    instance.transform.position = new Vector3(i, 0f, j);
                    instance.transform.SetParent(m_map.transform);
                }
            }
        }
    }

    void GenerateOuterWall() {
        for (int i = -OuterWallWidth; i < m_tileMap.GetLength(0) + OuterWallWidth; i++) {
            for (int j = -OuterWallWidth; j < m_tileMap.GetLength(1) + OuterWallWidth; j++) {
                if (i < 0 && j != 0 || i >= m_tileMap.GetLength(0) || j < 0 || j >= m_tileMap.GetLength(1)) {
                    GameObject instance = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    instance.transform.position = new Vector3(i, 0f, j);
                    instance.transform.SetParent(m_map.transform);
                }
            }
        }
    }

    void CombineMeshes() {
        MeshFilter[] meshFilters = m_map.GetComponentsInChildren<MeshFilter>();
        Mesh finalMesh = new Mesh();
        CombineInstance[] combiners = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++) {
            combiners[i].subMeshIndex = 0;
            combiners[i].mesh = meshFilters[i].sharedMesh;
            combiners[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }

        finalMesh.CombineMeshes(combiners);
        m_map.transform.GetComponent<MeshFilter>().sharedMesh = finalMesh;
        m_map.transform.gameObject.SetActive(true);

        var collider = m_map.GetComponent<MeshCollider>();
        collider.sharedMesh = finalMesh;

        foreach (Transform child in m_map.transform) {
            Destroy(child.gameObject);
        }
    }

    public void SetupScene() {
        GenerateMap();
        GenerateFloor();
        LoadMap();
        GenerateOuterWall();
        CombineMeshes();
    }
}
