﻿using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    private Rigidbody m_rigidbody;
    private Player m_player;
    private Action m_action;
    private DashController m_playerDash;
    private InputManager m_inputManager;

    void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        m_rigidbody = GetComponent<Rigidbody>();
        m_player = GetComponent<Player>();
        m_action = GetComponent<Action>();
        m_playerDash = GetComponent<DashController>();
        m_inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();

        Vector2 spawnPoint = GameObject.Find("GameManager").GetComponent<BoardManager>().GetRandomFloorTile();
        transform.position = new Vector3(spawnPoint.x, 0, spawnPoint.y);

        // By default, everything is freezed to avoid weird player collisions but the local player needs 
        // to move using the rigidbody velocity so his position is only frozen on Y.
        m_rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        // If a fire key is held down
        if (InputManager.GetKey("Shoot"))
        {
            m_action.Shoot();
        }

        // If the 'space' key is held down
        if (InputManager.GetKey("Sonar"))
        {
            m_action.Sonar();
        }

        // If shift is pressed
        if (InputManager.GetKeyDown("Dash"))
        {
            m_action.Dash();
        }
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        float moveHorizontal = m_inputManager.GetAxis("Horizontal");
        float moveVertical = m_inputManager.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);

        // If dashing, normalize movement vector so you are always at max speed
        if (m_playerDash.IsDashing())
        {
            movement.Normalize();
        }

        m_rigidbody.velocity = movement * m_player.Speed * m_playerDash.GetMultiplier();
    }

    void OnDisable()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        m_rigidbody.velocity = Vector3.zero;
    }

}
