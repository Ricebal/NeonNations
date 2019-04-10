using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed;

    public GameObject Shot;
    public Transform ShotSpawn;
    // Fire rate in seconds
    public float FireRate;

    private Rigidbody m_rigidBody;
    // The next time the entity will be able to shoot in seconds
    private float m_nextFire;

    public void Start() {
        m_rigidBody = GetComponent<Rigidbody>();
    }

    public void Update() {
        // If the 'ctrl' key is held down and the entity is able to shoot
        if(Input.GetButton("Fire1") && Time.time > m_nextFire) {
            m_nextFire = Time.time + FireRate;
            Instantiate(Shot, ShotSpawn.position, ShotSpawn.rotation);
        }
    }

    public void FixedUpdate() {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        m_rigidBody.velocity = movement * Speed;
    }
}
