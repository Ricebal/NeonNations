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
        m_escapeMenu.OnPauseToggled += PauseToggled;
        m_gameOverMenu = GameObject.Find("GameOverMenu").GetComponent<GameOverMenu>();
        m_gameOverMenu.OnRespawnClick += CmdRespawn;
        m_hud = GetComponent<PlayerHUD>();
    }

    private void FixedUpdate()
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

    protected override void Respawn(Vector2 respawnPoint)
    {
        if (isLocalPlayer)
        {
            m_gameOverMenu.Deactivate();
            m_playerController.enabled = true;
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

        m_escapeMenu.OnPauseToggled -= PauseToggled;
        m_gameOverMenu.OnRespawnClick -= CmdRespawn;
    }
}
