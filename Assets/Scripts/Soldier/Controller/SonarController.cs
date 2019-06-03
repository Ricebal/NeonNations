using Mirror;
using UnityEngine;

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
        CmdSonar();
    }

    [Command]
    private void CmdSonar()
    {
        RpcSonar();
    }

    [ClientRpc]
    private void RpcSonar()
    {
        GameObject sonarPrefab = Instantiate(m_prefab, transform.position, Quaternion.identity);
        Sonar sonarScript = sonarPrefab.GetComponent<Sonar>();
        
        GameObject spotLight = transform.GetChild(3).gameObject;
        spotLight.SetActive(true); // Show the spotlight of the soldier that uses the sonar.
        spotLight.GetComponent<SpotLightScript>().SetLifeTime(sonarScript.LifeSpan);


        Soldier soldier = GetComponent<Soldier>();
        sonarScript.SetColor(soldier.InitialColor);
    }
}
