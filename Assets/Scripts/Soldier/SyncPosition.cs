using UnityEngine;
using UnityEngine.Networking;

public class SyncPosition : NetworkBehaviour
{
    // Send a command to the server every 0.5 meter
    public float Threshold = 0.5f;
    public float LerpRate = 15;
    public Transform Transform;

    [SyncVar]
    private Vector3 m_syncPos;
    private Vector3 m_lastPos;

    void FixedUpdate()
    {
        TransmitPosition();
        LerpPosition();
    }

    private void LerpPosition()
    {
        if(!isLocalPlayer)
        {
            Transform.position = Vector3.Lerp(Transform.position, m_syncPos, Time.deltaTime * LerpRate);
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
        if(isLocalPlayer && Vector3.Distance(Transform.position, m_lastPos) > Threshold)
        {
            CmdProvidePositionToServer(Transform.position);
            m_lastPos = Transform.position;
        }
    }

}
