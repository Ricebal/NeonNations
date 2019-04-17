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

    private Color m_initialColor;
    private Vector3 m_initialPosition;
    private PlayerController m_playerController;
    private PlayerAction m_playerAction;
    private PlayerEnergy m_playerEnergy;
    private PlayerHealth m_playerHealth;
    private CameraController m_cameraController;
    private EscapeMenu m_escapeMenu;
    private GameOverMenu m_gameOverMenu;

    void Start() {
        if (!isLocalPlayer) {
            return;
        }

        m_initialColor = GetComponent<MeshRenderer>().material.color;
        m_initialPosition = this.transform.position;
        m_playerController = GetComponent<PlayerController>();
        m_playerAction = GetComponent<PlayerAction>();
        m_playerEnergy = GetComponent<PlayerEnergy>();
        m_playerHealth = GetComponent<PlayerHealth>();
        m_cameraController = Camera.main.GetComponent<CameraController>();
        m_cameraController.setTarget(this.transform);
        m_escapeMenu = GameObject.Find("MenuCanvas").GetComponent<EscapeMenu>();
        m_gameOverMenu = GameObject.Find("GameOverCanvas").GetComponent<GameOverMenu>();
    }

    void Update() {
        if(!isLocalPlayer) {
            return;
        }

        // If the escape menu is displayed, stop player's movements and set its velocity to 0
        if(m_escapeMenu.IsPaused() || m_gameOverMenu.IsActive()) {
            m_playerController.SetEnabled(false);
            m_playerController.Freeze();
        } else {
            m_playerController.SetEnabled(true);
        }

        // If the player is dead...
        if (m_playerHealth.GetCurrentHealth() <= 0) {
            // ...and the game over menu is not already activated, the player just died
            if (!m_gameOverMenu.IsActive()) {
                Die();
            }
            // ...and the remaining time before respawn is not elapsed, the player is fading away
            else if(m_gameOverMenu.GetRemainingTime() > 0) {
                CmdFade();
            }
            // ...and the remaining time before respawn is elapsed, the player has to respawn
            else if (m_gameOverMenu.GetRemainingTime() <= 0) {
                Respawn();
            }
        }
    }

    void FixedUpdate() {
        if (!isLocalPlayer) {
            return;
        }

        m_playerEnergy.AddEnergy(1);
    }

    public void Shoot() {
        // If the cooldown is elapsed and the player has enough energy
        if (Time.time > m_nextFire && m_playerEnergy.GetCurrentEnergy() >= BulletCost) {
            m_nextFire = Time.time + FireRate;
            m_playerEnergy.AddEnergy(-BulletCost);
            m_playerAction.CmdShoot();
        }
    }

    public void Sonar() {
        // If the cooldown is elapsed and the player has enough energy
        if (Time.time > m_nextSonar && m_playerEnergy.GetCurrentEnergy() >= SonarCost) {
            m_nextSonar = Time.time + SonarRate;
            m_playerEnergy.AddEnergy(-SonarCost);
            m_playerAction.CmdSonar();
        }
    }

    private void Die() {
        m_gameOverMenu.SetActive(true);
    }

    [Command]
    private void CmdFade() {
        float alpha = m_gameOverMenu.GetRemainingTime() / m_gameOverMenu.RespawnDelay;
        GetComponent<MeshRenderer>().material.color = new Color(1, 0.39f, 0.28f, alpha);
    }

    private void Respawn() {
        m_gameOverMenu.SetActive(false);
        GetComponent<MeshRenderer>().material.color = m_initialColor;
        this.transform.position = m_initialPosition;
        m_playerHealth.Reset();
        m_playerEnergy.Reset();
    }

    // If the player is hit by a bullet, the player gets damaged
    void OnTriggerEnter(Collider collider) {
        if(!isLocalPlayer) {
            return;
        }

        if (collider.gameObject.tag == "Bullet" && collider.gameObject.GetComponent<Bullet>().GetShooter() != this.gameObject) {
            m_playerHealth.TakeDamage(collider.gameObject.GetComponent<Bullet>().Damage);
        }
    }

    void OnDestroy() {
        if (!isLocalPlayer) {
            return;
        }

        m_cameraController.setInactive();
        m_cameraController.PlayerTransform = null;
    }
}
