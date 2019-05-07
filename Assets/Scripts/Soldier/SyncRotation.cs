using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(channel = 0, sendInterval = 0.033f)]
public class SyncRotation : NetworkBehaviour
{
    // Send a command to the server every 5 degrees
    public float Threshold = 5;
    public float LerpRate = 15;
    public Transform Transform;

    [SyncVar]
    private Quaternion m_syncPlayerRotation;
    private Quaternion m_lastPlayerRotation;

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
            Transform.rotation = Quaternion.Lerp(Transform.rotation, m_syncPlayerRotation, Time.deltaTime * LerpRate);
        }
    }

    [Command]
    private void CmdProvideRotationToServer(Quaternion playerRotation)
    {
        m_syncPlayerRotation = playerRotation;
    }

    [Client]
    private void TransmitRotation()
    {
        if (isLocalPlayer && Quaternion.Angle(Transform.rotation, m_lastPlayerRotation) > Threshold)
        {
            CmdProvideRotationToServer(Transform.rotation);
            m_lastPlayerRotation = Transform.rotation;
        }
    }
}
