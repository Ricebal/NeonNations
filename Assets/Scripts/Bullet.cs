using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Bullet speed
    public float Speed;
    // Time in seconds before the bullet is destroyed
    public float LivingTime;
    // Damage done to a player on hit
    public int Damage;
    // The explosion on impact
    public GameObject HitPrefab;
    // Offset for spawning the lights
    public float WallOffset;

    // The player that shot the bullet
    private string m_shooterId;
    // Start position
    private Vector3 m_startPosition;

    public void Start()
    {
        m_startPosition = transform.position;
        Rigidbody rigidbody = GetComponent<Rigidbody>();

        // Move the bullet straight ahead with constant speed
        rigidbody.velocity = transform.forward * Speed;

        // Destroy the bullet when the LivingTime is elapsed
        Destroy(gameObject, LivingTime);
    }

    public void OnTriggerEnter(Collider collider)
    {
        // If the bullet does not collide with its shooter
        if (collider.transform.name != m_shooterId)
        {
            if (HitPrefab != null)
            {
                Vector3 impactPos = GetBulletImpactPosition(collider.GetInstanceID());
                // If an impact point has been found
                if (!impactPos.Equals(default))
                {
                    // Create explosion on impact
                    CreateExplosion(impactPos);
                }
            }

            // Decouple particle system from bullet to prevent the trail from disappearing
            Transform trail = transform.Find("Particle System");
            if (trail != null)
            {
                trail.parent = null;

                // Destroy the particles after 0.5 seconds, the max lifetime of a particle
                Destroy(trail.gameObject, 0.5f);
            }

            // The bullet is destroyed on collision
            Destroy(gameObject);
        }
    }

    private Vector3 GetBulletImpactPosition(int id)
    {
        RaycastHit[] hits = Physics.RaycastAll(m_startPosition, transform.forward, 80);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.GetInstanceID() == id)
            {
                return hit.point;
            }
        }
        return default;
    }

    private void CreateExplosion(Vector3 pos)
    {
        // Instantiate explosion
        GameObject explosion = Instantiate(HitPrefab, pos, transform.rotation);
        explosion.transform.Translate(0, 0, -WallOffset);
    }

    public void SetShooterId(string shooterId)
    {
        m_shooterId = shooterId;
    }

    public string GetShooterId()
    {
        return m_shooterId;
    }

}
