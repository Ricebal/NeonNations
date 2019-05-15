using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SyncPosition : NetworkBehaviour
{
    // Send a command to the server every m_threshold meters
    [SerializeField]
    private float m_threshold;
    // Player's transform
    [SerializeField]
    private Transform m_transform;
    // Interpolation factors
    [SerializeField]
    private float m_normalLerpRate;
    [SerializeField]
    private float m_fasterLerpRate;
    // This value is used to define if the player should 'leave in the past'
    // when lag occurres, seeing old other player's positions
    // Deprecated [SerializeField]
    private bool m_useHistoricalLerp;
    // This value is used to consider the position correct when the difference is inferior than this value
    [SerializeField]
    private float m_closeEnough;

    // Current interpolation factor
    private float m_lerpRate;
    // Player's position
    [SyncVar(hook = "OnPositionSynced")]
    private Vector3 m_syncPos;
    // Last player's position synced across the Network
    private Vector3 m_lastPos;
    private List<Vector3> m_syncPosList = new List<Vector3>();

    void Start()
    {
        m_lerpRate = m_normalLerpRate;
    }

    void Update()
    {
        LerpPosition();
    }

    void FixedUpdate()
    {
        TransmitPosition();
    }

    private void LerpPosition()
    {
        if (!isLocalPlayer)
        {
            if (m_useHistoricalLerp)
            {
                HistoricalLerping();
            }
            else
            {
                OrdinaryLerping();
            }
        }
    }

    private void OrdinaryLerping()
    {
        LerpPosition(m_syncPos);
    }

    private void HistoricalLerping()
    {
        if (m_syncPosList.Count > 0)
        {
            LerpPosition(m_syncPosList[0]);
            // If the player is close enough from the last position, remove it from the synced list
            if (Vector3.Distance(m_transform.position, m_syncPosList[0]) < m_closeEnough)
            {
                m_syncPosList.RemoveAt(0);
            }

            // If there are to many positions stored, the player is way too far in the past so we accelerate his movements
            if (m_syncPosList.Count > 10)
            {
                m_lerpRate = m_fasterLerpRate;
            }
            else
            {
                m_lerpRate = m_normalLerpRate;
            }
        }
    }

    private void LerpPosition(Vector3 pos)
    {
        if (Vector3.Distance(m_transform.position, pos) < m_closeEnough)
        {
            m_transform.position = pos;
        }
        else
        {
            m_transform.position = Vector3.Lerp(m_transform.position, pos, Time.deltaTime * m_lerpRate);
        }
    }

    [Command]
    private void CmdProvidePositionToServer(Vector3 pos)
    {
        m_syncPos = pos;
    }

    [Client]
    private void TransmitPosition()
    {
        // If the player's has moved more than the threshold value, send his new position through the network
        if (isLocalPlayer && Vector3.Distance(m_transform.position, m_lastPos) > m_threshold)
        {
            m_lastPos = m_transform.position;
            CmdProvidePositionToServer(m_lastPos);
        }
    }

    [Client]
    private void OnPositionSynced(Vector3 latestPos)
    {
        m_syncPos = latestPos;
        //m_syncPosList.Add(m_syncPos);
    }

}
