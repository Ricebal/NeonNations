using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SyncRotation : NetworkBehaviour
{
    // Send a command to the server every m_threshold degrees
    [SerializeField]
    private float m_threshold;
    // Player's transform
    [SerializeField]
    private Transform m_transform;
    // Interpolation factor
    [SerializeField]
    private float m_lerpRate;
    // This value is used to define if the player should 'leave in the past'
    // when lag occurres, seeing old player's rotation
    // Deprecated [SerializeField]
    private bool m_useHistoricalLerp;
    // This value is used to consider the rotation correct when the difference is inferior than this value
    [SerializeField]
    private float m_closeEnough;

    // Player's rotation
    [SyncVar(hook = "OnRotationSynced")]
    private float m_syncRotation;
    // Last player's rotation synced across the network
    private float m_lastRotation;
    private List<float> m_syncRotationList = new List<float>();

    void Update()
    {
        LerpRotation();
    }

    void FixedUpdate()
    {
        TransmitRotation();
    }

    private void LerpRotation()
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
        LerpRotation(m_syncRotation);
    }

    private void HistoricalLerping()
    {
        if (m_syncRotationList.Count > 0)
        {
            LerpRotation(m_syncRotationList[0]);
            // If the Y rotation is close enough from the last rotation, remove the it from the synced list
            if (Mathf.Abs(m_transform.localEulerAngles.y - m_syncRotationList[0]) < m_closeEnough)
            {
                m_syncRotationList.RemoveAt(0);
            }
        }
    }

    private void LerpRotation(float rotation)
    {
        Quaternion newRotation = Quaternion.Euler(new Vector3(0, rotation, 0));
        if (Mathf.Abs(m_transform.localEulerAngles.y - rotation) < m_closeEnough)
        {
            m_transform.rotation = newRotation;
        }
        else
        {
            m_transform.rotation = Quaternion.Lerp(m_transform.rotation, newRotation, m_lerpRate * Time.deltaTime);
        }
    }

    [Command]
    private void CmdProvideRotationToServer(float rotation)
    {
        m_syncRotation = rotation;
    }

    [Client]
    private void TransmitRotation()
    {
        // If the player's has rotated more than the threshold value, send his new Y rotation through the network
        if (isLocalPlayer && Mathf.Abs(m_transform.localEulerAngles.y - m_lastRotation) > m_threshold)
        {
            m_lastRotation = m_transform.localEulerAngles.y;
            CmdProvideRotationToServer(m_lastRotation);
        }
    }

    [Client]
    private void OnRotationSynced(float latestRotation)
    {
        m_syncRotation = latestRotation;
        // Deprecated m_syncRotationList.Add(m_syncRotation);
    }

}
