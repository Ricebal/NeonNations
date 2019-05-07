using UnityEngine;
using UnityEngine.Networking;

public class SyncRotation : NetworkBehaviour
{
    public float LerpRate = 15;
    public Transform Transform;

    [SyncVar]
    private Quaternion m_syncPlayerRotation;

    void FixedUpdate()
    {
        TransmitRotation();
        LerpRotation();
    }

    private void LerpRotation()
    {
        if(!isLocalPlayer)
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
        if(isLocalPlayer)
        {
            CmdProvideRotationToServer(Transform.rotation);
        }
    }
}
