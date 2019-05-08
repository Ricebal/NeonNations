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

        base.Start();
        m_playerController = GetComponent<PlayerController>();
        m_playerController.enabled = true;
        m_cameraController = Camera.main.GetComponent<CameraController>();
        m_cameraController.SetTarget(this.transform);
        m_escapeMenu = GameObject.Find("MenuCanvas").GetComponent<EscapeMenu>();
        m_escapeMenu.EventPauseToggled += PauseToggled;
        m_gameOverMenu = GameObject.Find("GameOverCanvas").GetComponent<GameOverMenu>();
        m_hud = GetComponent<PlayerHUD>();
    }

    void OnDisable()
    {
        m_escapeMenu.EventPauseToggled -= PauseToggled;
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        base.Update();
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
        m_gameOverMenu.Activate(RespawnTime);
        m_playerController.enabled = false;
        base.Die();
    }

    protected override void Respawn()
    {
        m_gameOverMenu.Deactivate();
        m_playerController.enabled = true;
        m_hud.UpdateHUD();
        base.Respawn();
    }

    // If the player is hit by a bullet, the player gets damaged
    void OnTriggerEnter(Collider collider)
    {
        if (!isLocalPlayer)
        {
            return;
        }

        // If the player is hit by a bullet, update the HUD to show the new HP
        if (base.OnTriggerEnter(collider))
        {
            m_hud.UpdateHUD();
        }
    }

    public float GetRemainingSpawnTime()
    {
        return m_remainingRespawnTime;
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
