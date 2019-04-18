using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour
{
    // Bullet speed
    public float Speed;
    // Time in seconds before the bullet is destroyed
    public float LivingTime;
    // Damage done to a player on hit
    public int Damage;
    // The explosion on impact
    public GameObject HitPrefab;
    // Value for offset from wall to prevent light from going through the walls
    public float wallOffset;

    // The player that shot the bullet
    [SyncVar]
    private GameObject m_shooter;
    private float m_spawnTime;

    public void Start() {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        // Move the bullet straight ahead with constant speed
        rigidbody.velocity = transform.forward * Speed;

        m_spawnTime = Time.time;
    }

    public void Update() {
        // Destroy the bullet when the LivingTime is elapsed
        if (Time.time - m_spawnTime > LivingTime) {
            Destroy(this.gameObject);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (!isServer)
        {
            return;
        }

        // To prevent the player from hitting themselves
        if (collision.gameObject != m_shooter)
        {
            // Get impact-position
            ContactPoint contact = collision.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 pos = contact.point;

            // Create explosion on impact
            if (HitPrefab != null)
            {
                var hitFvx = Instantiate(HitPrefab, pos, gameObject.transform.rotation);
                hitFvx.transform.Translate(0, 0, -wallOffset);
            }

            // The bullet is destroyed on collision
            NetworkBehaviour.Destroy(gameObject);
        }
    }
    public void OnTriggerEnter(Collider collider)
    {
        if (!isServer)
        {
            return;
        }

        // To prevent the player from hitting themselves
        if (collider.gameObject != m_shooter)
        {
            // The bullet is destroyed on collision
            NetworkBehaviour.Destroy(gameObject);
        }
    }


    public void SetShooter(GameObject shooter) {
        m_shooter = shooter;
    }

    public GameObject GetShooter() {
        return m_shooter;
    }

}
