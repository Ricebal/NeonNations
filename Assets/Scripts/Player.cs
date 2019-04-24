using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{

    // Amount of energy a bullet will consume
    public int BulletCost;
    // Amount of energy a sonar will consume
    public int SonarCost;
    // Fire rate in seconds
    public float FireRate;
    // Sonar rate in seconds
    public float SonarRate;

    // The next time the entity will be able to shoot, in seconds
    private float m_nextFire;
    // The next time the entity will be able to use the sonar, in seconds
    private float m_nextSonar;

    private PlayerAction m_playerAction;
    private PlayerEnergy m_playerEnergy;
    private PlayerHealth m_playerHealth;
    private PlayerDash m_playerDash;

    void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        m_playerAction = GetComponent<PlayerAction>();
        m_playerEnergy = GetComponent<PlayerEnergy>();
        m_playerHealth = GetComponent<PlayerHealth>();
        m_playerDash = GetComponent<PlayerDash>();
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        m_playerEnergy.AddEnergy(1);
    }

    // Dash, if player can dash deduct energy and start dashing
    public void Dash()
    {
        if (!m_playerDash.CanDash(m_playerEnergy.GetCurrentEnergy()))
            return;

        m_playerEnergy.AddEnergy(-m_playerDash.GetCost());
        m_playerDash.StartDash();
    }

    public bool IsDashing()
    {
        return m_playerDash.IsDashing();
    }

    public float DashMultiplier()
    {
        return m_playerDash.GetMultiplier();
    }

    public void Shoot()
    {
        // If the cooldown is elapsed and the player has enough energy
        if (Time.time > m_nextFire && m_playerEnergy.GetCurrentEnergy() >= BulletCost)
        {
            m_nextFire = Time.time + FireRate;
            m_playerEnergy.AddEnergy(-BulletCost);
            m_playerAction.CmdShoot();
        }
    }

    public void Sonar()
    {
        // If the cooldown is elapsed and the player has enough energy
        if (Time.time > m_nextSonar && m_playerEnergy.GetCurrentEnergy() >= SonarCost)
        {
            m_nextSonar = Time.time + SonarRate;
            m_playerEnergy.AddEnergy(-SonarCost);
            m_playerAction.CmdSonar();
        }
    }

    // If the player is hit by a bullet, the player gets damaged
    public void OnTriggerEnter(Collider collider)
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (collider.gameObject.tag == "Bullet" && collider.gameObject.GetComponent<Bullet>().GetShooter() != this.gameObject)
        {
            m_playerHealth.TakeDamage(collider.gameObject.GetComponent<Bullet>().Damage);
        }
    }
}
