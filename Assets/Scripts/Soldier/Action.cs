using UnityEngine.Networking;

public class Action : NetworkBehaviour
{
    private ShootController m_playerShoot;
    private SonarController m_playerSonar;
    private DashController m_playerDash;
    private Stats m_playerStats;

    private void Start()
    {
        m_playerShoot = GetComponent<ShootController>();
        m_playerSonar = GetComponent<SonarController>();
        m_playerDash = GetComponent<DashController>();
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

    public void Dash()
    {
        if (m_playerDash.CanDash(m_playerStats.GetCurrentEnergy()))
        {
            m_playerStats.AddEnergy(-m_playerDash.Cost);
            m_playerDash.StartDash();
        }
    }
}
