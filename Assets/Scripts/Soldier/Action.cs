using UnityEngine;
using UnityEngine.Networking;

public class Action : NetworkBehaviour
{
    private Shoot m_playerShoot;
    private Sonar m_playerSonar;
    private Stats m_playerStats;
    private Dash m_playerDash;

    private void Start()
    {
        m_playerShoot = GetComponent<Shoot>();
        m_playerSonar = GetComponent<Sonar>();
        m_playerStats = GetComponent<Stats>();
        m_playerDash = GetComponent<Dash>();
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

    public void Dash()
    {
        if (m_playerDash.CanDash(m_playerStats.GetCurrentEnergy()))
        {
            m_playerStats.AddEnergy(-m_playerDash.Cost);
            m_playerDash.StartDash();
        }
    }
}
