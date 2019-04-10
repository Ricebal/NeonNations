using UnityEngine;

public class BulletMover : MonoBehaviour
{

    public float Speed;
    // Time in seconds before the bullet is destroyed
    public float LivingTime;

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

    public void OnCollisionEnter(Collision collision) {
        // Destroy the bullet when it hits a wall
        if (collision.gameObject.name == "Wall") {
            Destroy(this.gameObject);
        }
    }

}
