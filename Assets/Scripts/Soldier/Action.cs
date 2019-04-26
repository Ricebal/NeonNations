using UnityEngine;
using UnityEngine.Networking;

public class Action : NetworkBehaviour
{

    private PlayerShoot m_playerShoot;
    private PlayerSonar m_playerSonar;
    private Stats m_playerStats;

    private void Start()
    {
        m_playerShoot = GetComponent<PlayerShoot>();
        m_playerSonar = GetComponent<PlayerSonar>();
        m_playerStats = GetComponent<Stats>();
    }

    public void Sonar()
    {
        // If the cooldown is elapsed and the player has enough energy
        if (m_playerSonar.CanSonar(m_playerStats.GetCurrentEnergy()))
        {
            m_playerStats.AddEnergy(-m_playerSonar.Cost);
            m_playerSonar.Fire();
        }
    }

    public void Shoot()
    {
        // If the cooldown is elapsed and the player has enough energy
        if (m_playerShoot.CanShoot(m_playerStats.GetCurrentEnergy()))
        {
            m_playerStats.AddEnergy(-m_playerShoot.Cost);
            m_playerShoot.Fire();
        }

    }

}
