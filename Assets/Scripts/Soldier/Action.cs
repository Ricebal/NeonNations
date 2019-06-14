/**
 * Authors: Nicander
 */

using Mirror;
using UnityEngine;

[RequireComponent(typeof(ShootController), typeof(SonarController), typeof(DashController))]
public class Action : NetworkBehaviour
{
    private Stat m_energy;
    private ShootController m_shootController;
    private SonarController m_sonarController;
    private DashController m_dashController;

    private void Start()
    {
        m_shootController = GetComponent<ShootController>();
        m_sonarController = GetComponent<SonarController>();
        m_dashController = GetComponent<DashController>();
        m_energy = GetComponent<Soldier>().Energy;
    }

    public void Sonar()
    {
        // If the cooldown is elapsed and the player has enough energy
        if (m_sonarController.CanSonar(m_energy.GetValue()))
        {
            m_energy.Subtract(m_sonarController.Cost);
            m_sonarController.Fire();
        }
    }

    public bool Shoot()
    {
        // If the cooldown is elapsed and the player has enough energy
        if (m_shootController.CanShoot(m_energy.GetValue()))
        {
            m_energy.Subtract(m_shootController.Cost);
            m_shootController.Fire();
            return true;
        }

        return false;
    }

    public void Dash()
    {
        if (m_dashController.CanDash(m_energy.GetValue()))
        {
            m_energy.Subtract(m_dashController.Cost);
            m_dashController.StartDash();
        }
    }
}
