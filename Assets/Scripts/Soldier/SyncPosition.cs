using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SyncPosition : NetworkBehaviour
{
    // Send a command to the server every Threshold meters
    public float Threshold;
    // Player's transform
    public Transform Transform;
    // Interpolation factors
    public float NormalLerpRate;
    public float FasterLerpRate;
    // This value is used to define if the player should 'leave in the past'
    // when lag occurres, seeing old other player's positions
    public bool UseHistoricalLerp;
    // This value is used to consider the position correct when the difference is inferior than this value
    public float CloseEnough;

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
        m_lerpRate = NormalLerpRate;
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
            if (UseHistoricalLerp)
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
            // If the player is close enough from the last position, remove the last position from the synced list
            if (Vector3.Distance(Transform.position, m_syncPosList[0]) < CloseEnough)
            {
                m_syncPosList.RemoveAt(0);
            }

            // If there are to many positions stored, the player is way too far in the past so we accelerate his movements
            if (m_syncPosList.Count > 10)
            {
                m_lerpRate = FasterLerpRate;
            }
            else
            {
                m_lerpRate = NormalLerpRate;
            }
        }
    }

    private void LerpPosition(Vector3 pos)
    {
        if (Vector3.Distance(Transform.position, pos) < CloseEnough)
        {
            Transform.position = pos;
        }
        else
        {
            Transform.position = Vector3.Lerp(Transform.position, pos, Time.deltaTime * m_lerpRate);
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
        if (isLocalPlayer && Vector3.Distance(Transform.position, m_lastPos) > Threshold)
        {
            m_lastPos = Transform.position;
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
