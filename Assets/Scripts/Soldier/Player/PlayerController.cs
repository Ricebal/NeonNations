﻿using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    private Rigidbody m_rigidbody;
    private Player m_player;
    private Action m_action;
    private Dash m_playerDash;
    private bool m_isEnabled;

    public void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        m_rigidbody = GetComponent<Rigidbody>();
        m_player = GetComponent<Player>();
        m_action = GetComponent<Action>();
        m_playerDash = GetComponent<Dash>();
        SetEnabled(true);
    }

    public void Update()
    {
        if (!isLocalPlayer || !m_isEnabled)
        {
            return;
        }

        // If a fire key is held down
        if (Input.GetButton("Fire1"))
        {
            m_action.Shoot();
        }

        // If the 'space' key is held down
        if (Input.GetButton("Jump"))
        {
            m_action.Sonar();
        }

        // If shift or mouse3 is pressed
        if (Input.GetButton("Fire3"))
        {
            m_action.Dash();
        }
    }

    public void FixedUpdate()
    {
        if (!isLocalPlayer || !m_isEnabled)
        {
            return;
        }

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        // If dashing normalize movement vector so you are always at max speed
        if (m_playerDash.IsDashing())
        {
            movement.Normalize();
        }

        m_rigidbody.velocity = movement * m_player.Speed * m_playerDash.GetMultiplier();
    }

    // Set player's speed to 0
    public void Freeze()
    {
        m_rigidbody.velocity = new Vector3(0, 0, 0);
    }

    // Enable / disable player's movements
    public void SetEnabled(bool isEnabled)
    {
        m_isEnabled = isEnabled;
    }

}
