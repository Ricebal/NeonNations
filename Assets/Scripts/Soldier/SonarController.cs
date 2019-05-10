using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SonarController : NetworkBehaviour
{
    // Prefab representing the sonar
    [SerializeField] private GameObject m_prefab;
    // Sonar cooldown in seconds
    [SerializeField] private float m_cooldown;
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
        m_next = Time.time + m_cooldown;
        CmdSonar(transform.name);
    }

    [Command]
    private void CmdSonar(string shooterId)
    {
        RpcSonar(shooterId);
    }

    [ClientRpc]
    private void RpcSonar(string shooterId)
    {
        GameObject prefab = Instantiate(m_prefab, transform.position, Quaternion.identity);

        Sonar script = prefab.GetComponent<Sonar>();
        Soldier soldier = GetComponent<Soldier>();
        script.SetColor(soldier.InitialColor);
    }
}
