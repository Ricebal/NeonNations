using Mirror;
using UnityEngine;

public class SonarController : NetworkBehaviour
{
    // Prefab representing the sonar
    [SerializeField] private GameObject m_prefab = null;
    // Sonar cooldown in seconds
    [SerializeField] private float m_cooldown = 0;
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

        GameObject spotLight = transform.Find("Spot_Light_For_Other_Players").gameObject;
        spotLight.SetActive(true); // Show the spotlight of the soldier that uses the sonar.
        spotLight.GetComponent<SpotLightScript>().SetLifeTime(Sonar.LIFETIME, true);

        Sonar sonarScript = sonarPrefab.GetComponent<Sonar>();
        Soldier soldier = GetComponent<Soldier>();
        sonarScript.SetColor(soldier.Color);
    }
}
