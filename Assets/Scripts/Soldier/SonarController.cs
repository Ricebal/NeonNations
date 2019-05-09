using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SonarController : NetworkBehaviour
{
    // Prefab representing the sonar bullet
    public GameObject Prefab;
    // Amount of bullets spawned by sonar
    public float Rays;
    // Sonar rate in seconds
    public float Rate;
    // Amount of energy a sonar will consume
    public int Cost;
    // The next time the entity will be able to use the sonar, in seconds
    private float m_next;

    public bool CanSonar(int energy)
    {
        return Time.time > m_next && energy >= Cost;
    }

    public void Fire()
    {
        m_next = Time.time + Rate;
        CmdSonar();
    }

    [Command]
    public void CmdSonar()
    {
        GameObject prefab = Instantiate(Prefab, this.transform.position, Quaternion.Euler(0, 0, 0));
        prefab.transform.Translate(new Vector3(0, 0, this.transform.localScale.z / 2f), Space.Self);
        Sonar script = prefab.GetComponent<Sonar>();

        // Instanciate the bullet on the network for all players 
        NetworkServer.Spawn(prefab);
    }
}
