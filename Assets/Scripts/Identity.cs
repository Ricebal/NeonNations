using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkIdentity))]
public class Identity : NetworkBehaviour
{
    private string m_uniqueIdentity;

    void Update()
    {
        if (transform.name != m_uniqueIdentity)
        {
            NetworkInstanceId netId = GetComponent<NetworkIdentity>().netId;
            m_uniqueIdentity = CreateUniqueIdentity(netId);
            transform.name = m_uniqueIdentity;
        }
    }

    private string CreateUniqueIdentity(NetworkInstanceId netId)
    {
        string name = transform.name;
        if (name == "")
        {
            return "GameObject(" + netId.ToString() + ")";
        }
        else if (name.Contains("Clone"))
        {
            return name.Replace("Clone", netId.ToString());
        }
        else
        {
            return name + "(" + netId.ToString() + ")";
        }
    }

}