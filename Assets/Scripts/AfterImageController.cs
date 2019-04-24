using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImageController : MonoBehaviour
{
    public GameObject Prefab;
    public Transform SpawnLocation;
    private const float INTERVAL = 0.005f;
    private float m_lastSpawnTime;
    private bool m_generateImages;

    public void StartAfterImages()
    {
        m_generateImages = true;
    }

    public void StopAfterImages()
    {
        m_generateImages = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > m_lastSpawnTime + INTERVAL && m_generateImages)
        {
            SpawnImage();
            m_lastSpawnTime = Time.time;
        }
    }

    private void SpawnImage()
    {
        Instantiate(Prefab, SpawnLocation.position, SpawnLocation.rotation);
    }
}
