using UnityEngine;
using UnityEngine.Networking;

public class SyncPosition : NetworkBehaviour
{
    public float LerpRate = 15;
    public Transform Transform;

    [SyncVar]
    private Vector3 m_syncPos;

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

    [ClientCallback]
    private void TransmitPosition()
    {
        if(isLocalPlayer)
        {
            CmdProvidePositionToServer(Transform.position);
        }
    }

}
