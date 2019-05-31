using Mirror;
using UnityEngine;

public class SyncPosition : NetworkBehaviour
{
    // Threshold in meters
    [SerializeField] private float m_threshold = 0.025f;
    // Interpolation factor
    [SerializeField] private float m_lerpRate = 7;
    // Non-local soldiers are teleported when the difference between their current position and the real one is lower than this value
    [SerializeField] private float m_closeEnough = 0.01f;
    // Non-local soldiers are teleported when the difference between their current position and the real one is higher than this value
    [SerializeField] private float m_tooFar = 10;
    // Whether or not the GameObject is a bot
    [SerializeField] private bool m_isBot = false;

    private Vector3 m_syncPos;
    private Vector3 m_lastPos;
    private Transform m_transform;

    private void Start()
    {
        m_transform = GetComponent<Transform>();
    }

    private void Update()
    {
        if (isLocalPlayer || isServer && m_isBot)
        {
            TransmitPosition();
        }
        else
        {
            LerpPosition();
        }
    }

    // Interpolate non-local soldier's position between his actual position and his synced position
    private void LerpPosition()
    {
        // If the position has not yet been synchronized (when the soldier just joined) or the non-local soldier has reached his real position, do nothing
        if (m_syncPos == Vector3.zero || m_transform.position == m_syncPos)
        {
            return;
        }

        // If the non-local soldier is close enough or too far from his real position, teleport him
        float distance = Vector3.Distance(m_transform.position, m_syncPos);
        if (distance < m_closeEnough || distance > m_tooFar)
        {
            m_transform.position = m_syncPos;
        }
        else
        {
            m_transform.position = Vector3.Lerp(m_transform.position, m_syncPos, Time.deltaTime * m_lerpRate);
        }
    }

    private void TransmitPosition()
    {
        // If the local-player has moved more than the threshold value, transmit his new position
        if (Vector3.Distance(m_transform.position, m_lastPos) > m_threshold)
        {
            m_lastPos = m_transform.position;
            CmdPosition(m_lastPos);
        }
    }

    [Command]
    private void CmdPosition(Vector3 pos)
    {
        RpcPosition(pos);
    }

    [ClientRpc]
    private void RpcPosition(Vector3 pos)
    {
        m_syncPos = pos;
    }

}
