﻿using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Damage done to a player on hit
    public int Damage;
    // The player that shot the bullet
    public string ShooterId;

    // Bullet speed
    [SerializeField] private float Speed = 0;
    // Time in seconds before the bullet is destroyed
    [SerializeField] private float LivingTime = 0;
    // The explosion on impact
    [SerializeField] private GameObject HitPrefab = null;
    // The impact on a reflector
    [SerializeField] private GameObject ReflectorImpactPrefab = null;

    // Bullet's radius, used for sphere cast
    private float m_radius;
    private Rigidbody m_rigidbody;
    // Last position the bullet bounced off
    private Vector3 m_lastBouncePosition;
    // Has left player model
    private bool m_hasLeftPlayerCollider = false;
    // Last hit reflector
    private int m_lastReflector;
    private float m_startingTime;

    public void Start()
    {
        m_lastBouncePosition = transform.position;
        m_radius = GetComponent<CapsuleCollider>().radius;
        m_rigidbody = GetComponent<Rigidbody>();

        // Move the bullet straight ahead with constant speed
        m_rigidbody.velocity = transform.forward * Speed;

        m_startingTime = Time.time;
    }

    private void FixedUpdate()
    {
        // Destroy the bullet if the living time has been reached
        if (Time.time - m_startingTime > LivingTime)
        {
            DestroyBullet();
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        // Check if the bullet has left the players hitbox, so they don't shoot themself immediately
        if (collider.transform.name == ShooterId)
        {
            m_hasLeftPlayerCollider = true;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        // If the bullet hits a reflector and it's not the same as the last reflector bounce
        if (collider.tag == "Reflector" && collider.GetInstanceID() != m_lastReflector)
        {
            // Raycast from start or last bounce to collision
            RaycastHit contact = GetRaycastHit(collider.GetInstanceID());

            CreateReflectorImpact(contact.point, contact.normal);

            // If the bullet hits a mirror corner, destroy bullet to prevent bugs
            if (Vector3.Distance(m_lastBouncePosition, transform.position) < 0.1)
            {
                DestroyBullet();
            }
            // Get the current heading
            Vector3 currentDirection = transform.forward;
            // Set position to raycast position (if it's not default) to prevent bugs
            if (contact.point != Vector3.zero)
            {
                transform.position = contact.point;
            }
            else
            {
                DestroyBullet();
            }
            // Calculate the angle of reflection
            Vector3 newDirection = Vector3.Reflect(currentDirection, contact.normal);
            newDirection.Normalize();
            // Set the velocity to the speed var
            Vector3 newVelocity = newDirection * Speed;
            m_rigidbody.velocity = newVelocity;
            // Rotate the bullet so it faces the direction it's heading
            transform.rotation = Quaternion.LookRotation(newVelocity);
            // Set the last bounce position to current position for future raycasting
            m_lastBouncePosition = transform.position;
            // Set last hit reflector to the hit reflector
            m_lastReflector = collider.GetInstanceID();
        }

        // If the collider is not a reflector and has left the player hitbox
        if (collider.tag != "Reflector" && (m_hasLeftPlayerCollider || collider.transform.name != ShooterId))
        {
            DestroyBullet();
        }
    }

    private void DestroyBullet()
    {
        // Create explosion a bit behind to prevent explosions within walls
        CreateExplosion(transform.position - m_rigidbody.velocity * Time.fixedDeltaTime);

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

    private RaycastHit GetRaycastHit(int id)
    {
        RaycastHit[] hits = Physics.SphereCastAll(m_lastBouncePosition, m_radius, transform.forward, 80);
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

        // Set explosion light color to the player's color
        Color color = GameObject.Find(ShooterId).GetComponent<Soldier>().Color;
        ExplosionLight explosionLight = explosion.GetComponent<ExplosionLight>();
        explosionLight.SetColor(color);
    }

    private void CreateReflectorImpact(Vector3 pos, Vector3 normal)
    {
        //instantiate reflector impact
        Quaternion rotation;
        if (normal == Vector3.zero)
        {
            rotation = new Quaternion(0, 0, 0, 0);
        }
        else
        {
            rotation = Quaternion.LookRotation(normal);
        }
        GameObject impact = Instantiate(ReflectorImpactPrefab, pos, rotation);
        Destroy(impact, 1);

        // Set impact color to the player's color
        Color color = GameObject.Find(ShooterId).GetComponent<Soldier>().Color;
        Material mat;

        // Check if there is a TrailRenderer
        TrailRenderer[] trails = impact.GetComponentsInChildren<TrailRenderer>();
        if (trails.Length > 0)
        {
            TrailRenderer trail = trails[0];
            // Set the emission color of the trail to the new color times 3 for intensity
            mat = Material.Instantiate(trail.material);
            mat.SetColor("_EmissionColor", color * 3);
            trail.material = mat;
        }

        // Check if there is a Particlesystem
        ParticleSystemRenderer[] particleSystemRenderers = impact.GetComponentsInChildren<ParticleSystemRenderer>();
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

    public void SetBulletColor()
    {
        // Get the colour for the bullet
        Color color = GameObject.Find(ShooterId).GetComponent<Soldier>().Color;

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

}
