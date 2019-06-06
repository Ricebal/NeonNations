using UnityEngine;

public class Player : Soldier
{
    private PlayerController m_playerController;
    private PlayerHUD m_hud;
    private Aim m_aim;

    protected new void Start()
    {
        base.Start();

        if (!isLocalPlayer)
        {
            return;
        }

        m_hud = GetComponent<PlayerHUD>();
        m_aim = GetComponent<Aim>();
        m_playerController = GetComponent<PlayerController>();
        CameraController.SetTarget(transform);
        EscapeMenu.Singleton.OnPauseToggled += PauseToggled;
        GameOverMenu.Singleton.OnRespawnClick += Respawn;

        Username = ProfileMenu.GetUsername();
        CmdUsername(Username);
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        Cursor.visible = GameOverMenu.IsActive() || EscapeMenu.IsActive();

        EnergyStat.Add(1);
        m_hud.UpdateHUD();
        if (GameManager.Singleton.GameFinished)
        {
            if (GameOverMenu.IsActive())
            {
                GameOverMenu.Deactivate();
            }
        }
    }

    private void PauseToggled()
    {
        // Activate player controller if the player is alive and the escape menu is not activated and the game has not yet finished.
        if (!IsDead && !GameManager.Singleton.GameFinished)
        {
            m_playerController.enabled = !EscapeMenu.IsActive();
        }
    }

    protected override void Die()
    {
        if (isLocalPlayer)
        {
            if (!GameManager.Singleton.GameFinished)
            {
                GameOverMenu.Activate(RespawnTime);
            }
            m_playerController.enabled = false;
            m_aim.CanAim = false;
        }
        base.Die();
    }

    public override void StopMovement()
    {
        if (isLocalPlayer)
        {
            m_playerController.enabled = false;
        }
    }

    protected override void Respawn(Vector2Int respawnPoint)
    {
        if (isLocalPlayer)
        {
            GameOverMenu.Deactivate();
            // Activate player controller if the escape menu is not activated
            if (!EscapeMenu.IsActive())
            {
                m_playerController.enabled = true;
            }
            m_aim.CanAim = true;
        }
        base.Respawn(respawnPoint);
    }

    protected new void OnDestroy()
    {
        base.OnDestroy();

        if (!isLocalPlayer)
        {
            return;
        }

        CameraController.SetInactive();
        CameraController.Singleton.PlayerTransform = null;
        EscapeMenu.Singleton.OnPauseToggled -= PauseToggled;
        GameOverMenu.Singleton.OnRespawnClick -= Respawn;
    }
}
