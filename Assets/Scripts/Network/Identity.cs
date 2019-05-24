using Mirror;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class Identity : NetworkBehaviour
{
    private string m_uniqueIdentity;

    public void SetIdentity()
    {
        if (transform.name != m_uniqueIdentity)
        {
            uint netId = GetComponent<NetworkIdentity>().netId;
            m_uniqueIdentity = CreateUniqueIdentity(netId);
            transform.name = m_uniqueIdentity;
        }
    }

    private void Update()
    {
        SetIdentity();
    }

    private string CreateUniqueIdentity(uint netId)
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
