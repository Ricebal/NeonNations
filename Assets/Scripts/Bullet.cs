using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour
{
    // Start position
    private Vector3 m_startPosition;
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
    [SyncVar]
    private GameObject m_shooter;
    private float m_spawnTime;

    public void Start()
    {
        m_startPosition = transform.position;
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        // Move the bullet straight ahead with constant speed
        rigidbody.velocity = transform.forward * Speed;

        m_spawnTime = Time.time;
    }

    public void Update()
    {
        // Destroy the bullet when the LivingTime is elapsed
        if (Time.time - m_spawnTime > LivingTime)
        {
            Destroy(this.gameObject);
        }
    }

    public void SetColor()
    {
        // Get the colour for the bullet
        Color color = m_shooter.GetComponent<Soldier>().InitialColor;

        // Change the light's colour
        Light light = GetComponentsInChildren<Light>()[0];
        light.color = color;

        // Check if there is a Meshrenderer
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        if (renderers.Length > 0)
        {
            MeshRenderer renderer = renderers[0];
            // Set the emission color of the renderer to the new color times 3 for intensity
            renderer.material.SetColor("_EmissionColor", color * 3);
        }

        // Check if there is a TrailRenderer
        TrailRenderer[] trails = GetComponentsInChildren<TrailRenderer>();
        if (trails.Length > 0)
        {
            TrailRenderer trail = trails[0];
            // Set the emission color of the trail to the new color times 3 for intensity
            trail.material.SetColor("_EmissionColor", color * 3);
        }

        // Check if there is a Particlesystem
        ParticleSystemRenderer[] particleSystemRenderers = GetComponentsInChildren<ParticleSystemRenderer>();
        if (particleSystemRenderers.Length > 0)
        {
            ParticleSystemRenderer particleSystemRenderer = particleSystemRenderers[0];
            // Set the emission color of the particle trail to the new color times 3 for intensity
            particleSystemRenderer.trailMaterial.SetColor("_EmissionColor", color * 3);
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (!isServer)
        {
            return;
        }

        if (collider.gameObject != m_shooter)
        {
            if (HitPrefab != null)
            {
                // Get tag of collider
                int id = collider.GetInstanceID();
                // Get impact-position
                RaycastHit trueHit = default(RaycastHit);
                RaycastHit[] hits = Physics.RaycastAll(m_startPosition, transform.forward, 80);
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.GetInstanceID() == id)
                    {
                        trueHit = hit;
                        break;
                    }
                }
                if (!trueHit.Equals(default(RaycastHit)))
                {
                    Vector3 pos = trueHit.point;
                    // Create explosion on impact
                    CmdCreateExplosion(pos);
                }
            }

            // Decouple particle system from bullet to prevent the trail from disappearing
            Transform trail = transform.Find("Particle System");
            if (trail != null)
            {
                trail.parent = null;

                // Destroy the particles after 0.5 seconds, the max lifetime of a particle
                NetworkBehaviour.Destroy(trail.gameObject, 0.5f);
            }

            // The bullet is destroyed on collision
            NetworkBehaviour.Destroy(gameObject);
        }
    }

    [Command]
    private void CmdCreateExplosion(Vector3 pos)
    {
        // Instantiate explosion
        GameObject explosion = Instantiate(HitPrefab, pos, gameObject.transform.rotation);
        explosion.transform.Translate(0, 0, -WallOffset);
        explosion.GetComponentsInChildren<Light>()[0].color = m_shooter.GetComponent<Soldier>().InitialColor;

        // Instanciate the explosion on the network for all players 
        NetworkServer.Spawn(explosion);
    }

    public void SetShooter(GameObject shooter)
    {
        m_shooter = shooter;
        SetColor();
    }

    public GameObject GetShooter()
    {
        return m_shooter;
    }

}
