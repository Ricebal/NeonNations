using UnityEngine;
using UnityEngine.Networking;

public class PlayerAction : NetworkBehaviour
{
    private PlayerShoot m_playerShoot;
    private PlayerSonar m_playerSonar;
    private PlayerDash m_playerDash;
    private PlayerEnergy m_playerEnergy;

    private void Start()
    {
        m_playerShoot = GetComponent<PlayerShoot>();
        m_playerSonar = GetComponent<PlayerSonar>();
        m_playerDash = GetComponent<PlayerDash>();
        m_playerEnergy = GetComponent<PlayerEnergy>();
    }

    public void Sonar()
    {
        // If the cooldown is elapsed and the player has enough energy
        if (m_playerSonar.CanSonar(m_playerEnergy.GetCurrentEnergy()))
        {
            m_playerEnergy.AddEnergy(-m_playerSonar.Cost);
            m_playerSonar.Fire();
        }
    }

    public void Shoot()
    {
        // If the cooldown is elapsed and the player has enough energy
        if (m_playerShoot.CanShoot(m_playerEnergy.GetCurrentEnergy()))
        {
            m_playerEnergy.AddEnergy(-m_playerShoot.Cost);
            m_playerShoot.Fire();
        }

    }

    public void Dash()
    {
        if (m_playerDash.CanDash(m_playerEnergy.GetCurrentEnergy()))
        {
            m_playerEnergy.AddEnergy(-m_playerDash.Cost);
            m_playerDash.StartDash();
        }
    }
}
