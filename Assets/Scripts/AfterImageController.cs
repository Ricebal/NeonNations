﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AfterImageController : NetworkBehaviour
{
    [SerializeField] private GameObject m_prefab;
    [SerializeField] private float m_distance = 1f;
    [SerializeField] private float m_linger = 0.1f;
    private float m_endTime;
    private Vector3 m_lastPosition;
    private bool m_generateImages;
    [SyncVar] private Color m_color;

    private void Start()
    {
        Color color = GetComponent<Soldier>().InitialColor;
        m_color = new Color(color.r, color.g, color.b, 0.5f);
    }

    public void StartAfterImages()
    {
        m_generateImages = true;
        m_lastPosition = transform.position;
    }

    public void StopAfterImages()
    {
        m_generateImages = false;
        m_endTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // Exit if not generating images and linger time is over
        if (!(m_generateImages || Time.time < m_endTime + m_linger))
        {
            return;
        }

        // If the player moved a certain distance from the last image spawn a new one
        if (Vector3.Distance(transform.position, m_lastPosition) > m_distance || m_lastPosition == null)
        {
            CmdSpawnImage(transform.position, transform.rotation);
            m_lastPosition = transform.position;
        }
    }

    [Command]
    private void CmdSpawnImage(Vector3 position, Quaternion rotation)
    {
        RpcSpawnImage(position, rotation);
    }

    [ClientRpc]
    private void RpcSpawnImage(Vector3 position, Quaternion rotation)
    {
        GameObject afterImage = Instantiate(m_prefab, position, rotation);
        afterImage.GetComponent<AfterImage>().SetColor(m_color);
    }

    public bool IsGenerating()
    {
        return m_generateImages;
    }
}
