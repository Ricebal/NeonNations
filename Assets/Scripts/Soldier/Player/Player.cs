using UnityEngine;
using UnityEngine.Networking;

public class Player : Soldier
{
    private PlayerController m_playerController;
    private CameraController m_cameraController;
    private EscapeMenu m_escapeMenu;
    private GameOverMenu m_gameOverMenu;

    void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        base.Start();
        m_playerController = GetComponent<PlayerController>();
        m_cameraController = Camera.main.GetComponent<CameraController>();
        m_cameraController.SetTarget(this.transform);
        m_escapeMenu = GameObject.Find("MenuCanvas").GetComponent<EscapeMenu>();
        m_gameOverMenu = GameObject.Find("GameOverCanvas").GetComponent<GameOverMenu>();
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        // If the escape menu or the game over menu is displayed, disable player's movements and set his velocity to 0
        if (m_escapeMenu.IsPaused() || m_gameOverMenu.IsActive())
        {
            m_playerController.SetEnabled(false);
            m_playerController.Freeze();
        }
        else
        {
            m_playerController.SetEnabled(true);
        }

        // If the player is dead...
        if (m_health.GetCurrentHealth() <= 0)
        {
            // ...and the game over menu is not already activated, the player just died
            if (!m_gameOverMenu.IsActive())
            {
                Die();
            }
            // ...and the remaining time before respawn is not elapsed, the player is fading away
            else if (m_gameOverMenu.GetRemainingTime() > 0)
            {
                float alpha = m_gameOverMenu.GetRemainingTime() / m_gameOverMenu.RespawnDelay;
                Color color = new Color(1, 0.39f, 0.28f, alpha);
                CmdColor(this.gameObject, color);
            }
            // ...and the remaining time before respawn is elapsed, the player has to respawn
            else if (m_gameOverMenu.GetRemainingTime() <= 0)
            {
                Respawn();
            }
        }
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        m_energy.AddEnergy(1);
    }

    private void Die()
    {
        m_gameOverMenu.SetActive(true);
    }

    private void Respawn()
    {
        m_gameOverMenu.SetActive(false);
        base.Respawn();
    }

        // If the player is hit by a bullet, the player gets damaged
        void OnTriggerEnter(Collider collider)
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (collider.gameObject.tag == "Bullet" && collider.gameObject.GetComponent<Bullet>().GetShooter() != this.gameObject)
        {
            m_health.TakeDamage(collider.gameObject.GetComponent<Bullet>().Damage);
        }
    }

    void OnDestroy()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        m_cameraController.SetInactive();
        m_cameraController.PlayerTransform = null;
    }
}
