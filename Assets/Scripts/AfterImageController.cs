﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AfterImageController : NetworkBehaviour
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
        // If the time since last image is greater than interval and generate images is true spawn an image
        if (Time.time > m_lastSpawnTime + INTERVAL && m_generateImages)
        {
            CmdSpawnImage();
            m_lastSpawnTime = Time.time;
        }
    }

    [Command]
    private void CmdSpawnImage()
    {
        GameObject afterImage = Instantiate(Prefab, SpawnLocation.position, SpawnLocation.rotation);
        NetworkServer.Spawn(afterImage);
    }

    public bool IsGenerating()
    {
        return m_generateImages;
    }
}