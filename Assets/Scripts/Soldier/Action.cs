using Mirror;
using UnityEngine;

[RequireComponent(typeof(ShootController), typeof(SonarController), typeof(DashController))]
[RequireComponent(typeof(Stats))]
public class Action : NetworkBehaviour
{
    private ShootController m_shootController;
    private SonarController m_sonarController;
    private DashController m_dashController;
    private Stats m_stats;

    private void Start()
    {
        m_shootController = GetComponent<ShootController>();
        m_sonarController = GetComponent<SonarController>();
        m_dashController = GetComponent<DashController>();
        m_stats = GetComponent<Stats>();
    }

    public void Sonar()
    {
        // If the cooldown is elapsed and the player has enough energy
        if (m_sonarController.CanSonar(m_stats.GetCurrentEnergy()))
        {
            m_stats.AddEnergy(-m_sonarController.Cost);
            m_sonarController.Fire();
        }
    }

    public void Shoot()
    {
        // If the cooldown is elapsed and the player has enough energy
        if (m_shootController.CanShoot(m_stats.GetCurrentEnergy()))
        {
            m_stats.AddEnergy(-m_shootController.Cost);
            m_shootController.Fire();
        }

    }

    public void Dash()
    {
        if (m_dashController.CanDash(m_stats.GetCurrentEnergy()))
        {
            m_stats.AddEnergy(-m_dashController.Cost);
            m_dashController.StartDash();
        }
    }
}
