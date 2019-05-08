using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SyncRotation : NetworkBehaviour
{
    // Send a command to the server every Threshold degrees
    public float Threshold;
    // Player's transform
    public Transform Transform;
    // Interpolation factor
    public float LerpRate;
    // This value is used to define if the player should 'leave in the past'
    // when lag occurres, seeing old player's rotation
    public bool UseHistoricalInterpolation;
    // When UseHistoricalLerp is true, this value is used to consider that
    // the rotation is correct when the difference is inferior than this value
    public float CloseEnough;

    [SyncVar(hook = "OnRotationSynced")]
    private float m_syncRotation;
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
            if (UseHistoricalInterpolation)
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

    private void LerpRotation(float rotation)
    {
        Vector3 newRotation = new Vector3(0, rotation, 0);
        Transform.rotation = Quaternion.Lerp(Transform.rotation, Quaternion.Euler(newRotation), LerpRate * Time.deltaTime);
    }

    private void HistoricalLerping()
    {
        if(m_syncRotationList.Count > 0)
        {
            LerpRotation(m_syncRotationList[0]);
            // If the player's Y rotation is close enough from the last rotation, remove the last rotation from the synced list
            if (Mathf.Abs(Transform.localEulerAngles.y - m_syncRotationList[0]) < CloseEnough) {
                m_syncRotationList.RemoveAt(0);
            }
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
        if (isLocalPlayer && Mathf.Abs(Transform.localEulerAngles.y - m_lastRotation) > Threshold)
        {
            m_lastRotation = Transform.localEulerAngles.y;
            CmdProvideRotationToServer(m_lastRotation);
        }
    }

    [Client]
    private void OnRotationSynced(float latestRotation)
    {
        m_syncRotation = latestRotation;
        m_syncRotationList.Add(m_syncRotation);
    }

}
