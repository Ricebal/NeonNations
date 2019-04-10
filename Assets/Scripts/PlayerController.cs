using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed;

    private Rigidbody m_rigidBody;

    // Start is called before the first frame update
    public void Start() {
        m_rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    public void FixedUpdate() {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        m_rigidBody.velocity = movement * Speed;
    }
}
