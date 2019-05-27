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
    // Last position the bullet bounced off
    private Vector3 m_lastBouncePosition;
    // Has left player model
    private bool m_hasLeftPlayerCollider = false;
    // Last hit mirror
    private int m_lastMirror;

    public void Start()
    {
        m_lastBouncePosition = transform.position;
        Rigidbody rigidbody = GetComponent<Rigidbody>();

        // Move the bullet straight ahead with constant speed
        rigidbody.velocity = transform.forward * Speed;

        // Destroy the bullet when the LivingTime is elapsed
        Destroy(gameObject, LivingTime);
    }

    private void OnTriggerExit(Collider collider)
    {
        // Check if the bullet has left the players hitbox, so they don't shoot themself immediately
        if (collider.transform.name == m_shooterId)
        {
            m_hasLeftPlayerCollider = true;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        // If the bullet hits a mirror and it's not the same as the last mirror bounce
        if (collider.tag == "Mirror" && collider.GetInstanceID() != m_lastMirror)
        {
            // Get the current heading
            Vector3 currentDirection = transform.TransformDirection(Vector3.forward);
            // Raycast from start or last bounce to collision
            RaycastHit contact = GetRaycastHit(collider.GetInstanceID());
            // Calculate the angle of reflection
            Vector3 newDirection = Vector3.Reflect(currentDirection, contact.normal);
            newDirection.Normalize();
            // Set the velocity to the speed var
            Vector3 newVelocity = newDirection * Speed;
            GetComponent<Rigidbody>().velocity = newVelocity;
            // Rotate the bullet so it faces the direction it's heading
            transform.rotation = Quaternion.LookRotation(newVelocity);
            // Set the last bounce position to current position for future raycasting
            m_lastBouncePosition = transform.position;
            // Set last hit mirror to the hit mirror
            m_lastMirror = collider.GetInstanceID();
        }

        // If the collider is not a mirror and has left the player hitbox
        if (collider.tag != "Mirror" && (m_hasLeftPlayerCollider || collider.transform.name != m_shooterId))
        {
            if (HitPrefab != null)
            {
                Vector3 impactPos = GetRaycastHit(collider.GetInstanceID()).point;
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

    private RaycastHit GetRaycastHit(int id)
    {
        RaycastHit[] hits = Physics.RaycastAll(m_lastBouncePosition, transform.forward, 80);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.GetInstanceID() == id)
            {
                return hit;
            }
        }
        return default;
    }

    private void CreateExplosion(Vector3 pos)
    {
        // Instantiate explosion
        GameObject explosion = Instantiate(HitPrefab, pos, transform.rotation);
        explosion.transform.Translate(0, 0, -WallOffset);

        // Set explosion light color to the player's color
        Color color = GameObject.Find(m_shooterId).GetComponent<Soldier>().InitialColor;
        ExplosionLight explosionLight = explosion.GetComponent<ExplosionLight>();
        explosionLight.SetColor(color);
    }

    public void SetBulletColor()
    {
        // Get the colour for the bullet
        Color color = GameObject.Find(m_shooterId).GetComponent<Soldier>().InitialColor;

        // Make new material
        Material mat;

        // Change the light's colour
        Light light = GetComponentsInChildren<Light>()[0];
        light.color = color;

        // Check if there is a Meshrenderer
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        if (renderers.Length > 0)
        {
            MeshRenderer renderer = renderers[0];
            // Set the emission color of the renderer to the new color times 3 for intensity
            mat = Material.Instantiate(renderer.material);
            mat.SetColor("_EmissionColor", color * 3);
            renderer.material = mat;
        }

        // Check if there is a TrailRenderer
        TrailRenderer[] trails = GetComponentsInChildren<TrailRenderer>();
        if (trails.Length > 0)
        {
            TrailRenderer trail = trails[0];
            // Set the emission color of the trail to the new color times 3 for intensity
            mat = Material.Instantiate(trail.material);
            mat.SetColor("_EmissionColor", color * 3);
            trail.material = mat;
        }

        // Check if there is a Particlesystem
        ParticleSystemRenderer[] particleSystemRenderers = GetComponentsInChildren<ParticleSystemRenderer>();
        if (particleSystemRenderers.Length > 0)
        {
            ParticleSystemRenderer particleSystemRenderer = particleSystemRenderers[0];
            // Set the emission color of the particle trail to the new color times 3 for intensity
            mat = Material.Instantiate(particleSystemRenderer.material);
            mat.SetColor("_EmissionColor", color * 3);
            particleSystemRenderer.material = mat;
            particleSystemRenderer.trailMaterial = mat;
        }
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
