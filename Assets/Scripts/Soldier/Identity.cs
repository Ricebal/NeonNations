using UnityEngine;
using UnityEngine.Networking;

public class Identity : NetworkBehaviour
{
    [SyncVar]
    private string m_playerUniqueIdentity;
    private NetworkInstanceId m_playerNetId;

    public override void OnStartLocalPlayer()
    {
        GetNetIdentity();
        SetIdentity();
    }

    void Update()
    {
        if(transform.name == "" ||transform.name == "PlayerPrefab(Clone)")
        {
            SetIdentity();
        }
    }

    [Client]
    private void GetNetIdentity()
    {
        m_playerNetId = GetComponent<NetworkIdentity>().netId;
        CmdSendIdentity(CreateUniqueIdentity());
    }

    private string CreateUniqueIdentity()
    {
        return "Player " + m_playerNetId.ToString();
    }

    private void SetIdentity()
    {
        if(isLocalPlayer)
        {
            transform.name = CreateUniqueIdentity();
        }
        else
        {
            transform.name = m_playerUniqueIdentity;
        }
    }

    [Command]
    private void CmdSendIdentity(string identity)
    {
        m_playerUniqueIdentity = identity;
    }
}
