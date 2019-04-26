using UnityEngine;
using UnityEngine.Networking;

public class PlayerAction : NetworkBehaviour
{

    private PlayerShoot m_playerShoot;
    private PlayerSonar m_playerSonar;
    private PlayerEnergy m_playerEnergy;

    private void Start()
    {
        m_playerShoot = GetComponent<PlayerShoot>();
        m_playerSonar = GetComponent<PlayerSonar>();
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

}
