using UnityEngine;

public class Player : Soldier
{
    private PlayerController m_playerController;
    private CameraController m_cameraController;
    private EscapeMenu m_escapeMenu;
    private GameOverMenu m_gameOverMenu;
    private PlayerHUD m_hud;

    void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        m_playerController = GetComponent<PlayerController>();
        m_cameraController = Camera.main.GetComponent<CameraController>();
        m_cameraController.SetTarget(this.transform);
        m_escapeMenu = GameObject.Find("MenuCanvas").GetComponent<EscapeMenu>();
        m_escapeMenu.EventPauseToggled += PauseToggled;
        m_gameOverMenu = GameObject.Find("GameOverCanvas").GetComponent<GameOverMenu>();
        m_hud = GetComponent<PlayerHUD>();
    }

    void OnDisable()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        m_escapeMenu.EventPauseToggled -= PauseToggled;
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        m_stats.AddEnergy(1);
        m_hud.UpdateHUD();
    }

    private void PauseToggled()
    {
        m_playerController.enabled = !m_playerController.enabled;
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

    protected override void Respawn()
    {
        if (isLocalPlayer)
        {
            m_gameOverMenu.Deactivate();
            m_playerController.enabled = true;
        }
        base.Respawn();
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
