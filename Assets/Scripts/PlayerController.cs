using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public float Speed;

    private Rigidbody m_rigidBody;
    private Vector3 m_movement;

    public void Start() {
        m_rigidBody = GetComponent<Rigidbody>();
    }

    public void FixedUpdate() {
        if(!isLocalPlayer)
            return;

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        MovePlayer(moveHorizontal, moveVertical);
    }

    public void MovePlayer(float horizontal, float vertical) {
        m_movement.Set(horizontal, 0, vertical);

        if (horizontal != 0 || vertical != 0) {
            m_rigidBody.MoveRotation(Quaternion.LookRotation(m_movement));
        }

        m_movement = m_movement.normalized * Speed * Time.deltaTime;
        m_rigidBody.MovePosition(transform.position + m_movement);
    }
}
