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

    private PlayerAction m_playerAction;
    private PlayerEnergy m_playerEnergy;
    private PlayerHealth m_playerHealth;

    void Start() {
        if (!isLocalPlayer) {
            return;
        }
        m_playerAction = GetComponent<PlayerAction>();
        m_playerEnergy = GetComponent<PlayerEnergy>();
        m_playerHealth = GetComponent<PlayerHealth>();
    }

    void FixedUpdate() {
        if (!isLocalPlayer) {
            return;
        }
        m_playerEnergy.AddEnergy(1);
    }

    public void Shoot() {
        // If the cooldown is elapsed and the player has enough energy
        if (Time.time > m_playerAction.getNextFire() && m_playerEnergy.CurrentEnergy >= BulletCost) {
            m_playerEnergy.AddEnergy(-BulletCost);
            m_playerAction.CmdShoot();
        }
    }

    public void Sonar() {
        // If the cooldown is elapsed and the player has enough energy
        if (Time.time > m_playerAction.getNextSonar() && m_playerEnergy.CurrentEnergy >= SonarCost) {
            m_playerEnergy.AddEnergy(-SonarCost);
            m_playerAction.CmdSonar();
        }
    }

    // If the player is hit by a bullet, the player gets damaged
    public void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Bullet") {
            if (m_playerHealth.GetCurrentHealth() > 0) {
                m_playerHealth.TakeDamage(collision.gameObject.GetComponent<BulletMover>().Damage);
            }
        }
    }
}
