using UnityEngine;
using UnityEngine.Networking;

public class Action : NetworkBehaviour
{
    private ShootController m_playerShoot;
    private SonarController m_playerSonar;
    private Stat m_playerEnergy;
    private DashController m_playerDash;

    private void Start()
    {
        m_playerShoot = GetComponent<ShootController>();
        m_playerSonar = GetComponent<SonarController>();
        Stat[] lol = GetComponents<Stat>();
        m_playerEnergy = GetComponents<Stat>()[1];
        m_playerDash = GetComponent<DashController>();
    }

    public void Sonar()
    {
        // If the cooldown is elapsed and the player has enough energy
        if (m_playerSonar.CanSonar(m_playerEnergy.GetCurrent()))
        {
            m_playerEnergy.ChangeCurrent(-m_playerSonar.Cost);
            m_playerSonar.Fire();
        }
    }

    public void Shoot()
    {
        // If the cooldown is elapsed and the player has enough energy
        if (m_playerShoot.CanShoot(m_playerEnergy.GetCurrent()))
        {
            m_playerEnergy.ChangeCurrent(-m_playerShoot.Cost);
            m_playerShoot.Fire();
        }

    }

    public void Dash()
    {
        if (m_playerDash.CanDash(m_playerEnergy.GetCurrent()))
        {
            m_playerEnergy.ChangeCurrent(-m_playerDash.Cost);
            m_playerDash.StartDash();
        }
    }
}
