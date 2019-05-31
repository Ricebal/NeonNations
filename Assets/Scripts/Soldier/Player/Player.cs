﻿using UnityEngine;

public class Player : Soldier
{
    private PlayerController m_playerController;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        m_playerController = GetComponent<PlayerController>();
        CameraController.SetTarget(this.transform);
        EscapeMenu.Singleton.OnPauseToggled += PauseToggled;
        GameOverMenu.Singleton.OnRespawnClick += CmdRespawn;

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

        m_energyStat.Add(1);
        PlayerHUD.UpdateHUD();
    }

    private void PauseToggled()
    {
        // Activate player controller if the player is alive and the escape menu is not activated
        if (!IsDead)
        {
            m_playerController.enabled = !EscapeMenu.IsActive();
        }
    }

    protected override void Die()
    {
        if (isLocalPlayer)
        {
            GameOverMenu.Activate(RespawnTime);
            m_playerController.enabled = false;
        }

        base.Die();
    }

    protected override void Respawn(Vector2 respawnPoint)
    {
        if (isLocalPlayer)
        {
            GameOverMenu.Deactivate();
            // Activate player controller if the escape menu is not activated
            if (!EscapeMenu.IsActive())
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

        CameraController.SetInactive();
        CameraController.Singleton.PlayerTransform = null;
        EscapeMenu.Singleton.OnPauseToggled -= PauseToggled;
        GameOverMenu.Singleton.OnRespawnClick -= CmdRespawn;
    }
}
