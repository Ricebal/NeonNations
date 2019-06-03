using Mirror;
using UnityEngine;

public class SyncRotation : NetworkBehaviour
{
    // Threshold in degrees
    [SerializeField] private float m_threshold = 1;
    // Interpolation factor
    [SerializeField] private float m_lerpRate = 15;
    // Non-local soldiers are instantly rotated when the difference between their current rotation and the real one is lower than this value
    [SerializeField] private float m_closeEnough = 1.5f;
    // Non-local soldiers are instantly rotated when the difference between their current rotation and the real one is higher than this value
    [SerializeField] private float m_tooFar = 90;
    // Whether or not the GameObject is a bot
    [SerializeField] private bool m_isBot = false;

    private float m_syncRotation;
    private float m_lastRotation;
    private Transform m_transform;

    private void Start()
    {
        m_transform = GetComponent<Transform>();
    }

    private void Update()
    {
        if (isLocalPlayer || isServer && m_isBot)
        {
            TransmitRotation();
        }
        else
        {
            LerpRotation();
        }
    }

    // Interpolate non-local soldier's rotation between his actual rotation and his synced rotation
    private void LerpRotation()
    {
        Quaternion newRotation = Quaternion.Euler(new Vector3(0, m_syncRotation, 0));

        // If the rotation has not yet been synchronized (when the soldier just joined) or the non-local soldier has reached his real rotation, do nothing
        if (newRotation == Quaternion.identity || m_transform.rotation == newRotation)
        {
            return;
        }

        // If the non-local soldier is close enough or too far from his real rotation, instantly rotate him
        float angle = Mathf.Abs(m_transform.localEulerAngles.y - m_syncRotation);
        if (angle < m_closeEnough || angle > m_tooFar)
        {
            m_transform.rotation = newRotation;
        }
        else
        {
            m_transform.rotation = Quaternion.Lerp(m_transform.rotation, newRotation, m_lerpRate * Time.deltaTime);
        }
    }

    private void TransmitRotation()
    {
        // If the local-player has rotated more than the threshold value, transmit his new Y rotation
        if (Mathf.Abs(m_transform.localEulerAngles.y - m_lastRotation) > m_threshold)
        {
            m_lastRotation = m_transform.localEulerAngles.y;
            CmdRotation(m_lastRotation);
        }
    }

    [Command]
    private void CmdRotation(float rotation)
    {
        RpcRotation(rotation);
    }

    [ClientRpc]
    private void RpcRotation(float rotation)
    {
        m_syncRotation = rotation;
    }

}
