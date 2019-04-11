using UnityEngine;
using UnityEngine.Networking;

public class BulletMover : NetworkBehaviour
{

    public float Speed;
    // Time in seconds before the bullet is destroyed
    public float LivingTime;
    // Damage done to a player on hit
    public float Damage;

    private float m_spawnTime;

    public void Start() {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        // Move the bullet straight ahead with constant speed
        rigidbody.velocity = transform.forward * Speed;

        m_spawnTime = Time.time;
    }

    public void Update() {
        // Destroy the bullet when the LivingTime is elapsed
        if(Time.time - m_spawnTime > LivingTime) {
            Destroy(this.gameObject);
        }
    }

    public void OnTriggerEnter(Collider collider) {
        // The bullet is destroyed on collision
        Destroy(this.gameObject);
    }

}
