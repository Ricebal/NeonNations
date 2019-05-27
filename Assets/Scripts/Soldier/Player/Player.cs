using UnityEngine;

public class Player : Soldier
{
    private PlayerController m_playerController;
    private CameraController m_cameraController;
    private EscapeMenu m_escapeMenu;
    private GameOverMenu m_gameOverMenu;
    private PlayerHUD m_hud;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        m_playerController = GetComponent<PlayerController>();
        m_cameraController = Camera.main.GetComponent<CameraController>();
        m_cameraController.SetTarget(this.transform);
        m_escapeMenu = GameObject.Find("EscapeMenu").GetComponent<EscapeMenu>();
        m_escapeMenu.EventPauseToggled += PauseToggled;
        m_gameOverMenu = GameObject.Find("GameOverMenu").GetComponent<GameOverMenu>();
        m_hud = GetComponent<PlayerHUD>();
    }

    private void OnDisable()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        m_escapeMenu.EventPauseToggled -= PauseToggled;
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        Cursor.visible = m_gameOverMenu.IsActive() || m_escapeMenu.IsActive();

        m_stats.AddEnergy(1);
        m_hud.UpdateHUD();
    }

    private void PauseToggled()
    {
        // Activate player controller if the player is alive and the escape menu is not activated
        if (!IsDead)
        {
            m_playerController.enabled = !m_escapeMenu.IsActive();
        }
    }

    protected override void Die()
    {
        if (isLocalPlayer)
        {
            m_gameOverMenu.Activate(RespawnTime);
            m_playerController.enabled = false;
        }

        base.Die();
    }

    protected override void Respawn(Vector2 respawnPoint)
    {
        if (isLocalPlayer)
        {
            m_gameOverMenu.Deactivate();
            // Activate player controller if the escape menu is not activated
            if (!m_escapeMenu.IsActive())
            {
                m_playerController.enabled = true;
            }
        }

        base.Respawn(respawnPoint);
    }

    private void OnDestroy()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        m_cameraController.SetInactive();
        m_cameraController.PlayerTransform = null;
    }
}
