using UnityEngine;
using UnityEngine.Networking;

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
        m_cameraController = Camera.main.GetComponent<CameraController>();
        m_cameraController.SetTarget(this.transform);
        m_escapeMenu = GameObject.Find("MenuCanvas").GetComponent<EscapeMenu>();
        m_gameOverMenu = GameObject.Find("GameOverCanvas").GetComponent<GameOverMenu>();
        m_hud = GetComponent<PlayerHUD>();
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        // If the escape menu or the game over menu is displayed, disable player's movements
        if (m_escapeMenu.IsPaused() || m_gameOverMenu.IsActive())
        {
            m_playerController.SetEnabled(false);
        }
        else
        {
            m_playerController.SetEnabled(true);
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

    protected override void Die()
    {
        m_gameOverMenu.Activate(RespawnTime);
        base.Die();
    }

    protected override void Respawn()
    {
        m_gameOverMenu.Deactivate();
        base.Respawn();
        m_hud.UpdateHUD();
    }

    // If the player is hit by a bullet, the player gets damaged
    void OnTriggerEnter(Collider collider)
    {
        if (!isLocalPlayer)
        {
            return;
        }

        // If the player is hit by a bullet, update the HUD to show the new HP
        if (base.OnTriggerEnter(collider)){
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
