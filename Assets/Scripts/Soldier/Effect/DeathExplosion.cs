using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class DeathExplosion : MonoBehaviour
{
    [SerializeField] private Light m_light;
    private const float LIFESPAN = 1.5f;
    private const float DELAY = 1f;
    private const float MAX_INTENSITY = 3;
    private const float MAX_RANGE = 32;
    private bool m_growing = true;
    private float m_start;

    public void Fire()
    {
        m_light.gameObject.SetActive(true);
        m_light.intensity = 0;
        m_light.range = 0;
        m_growing = true;
        m_start = Time.time;
        GetComponentInChildren<ParticleSystem>().Play();
    }

    public void SetColor(Color color)
    {
        m_light.color = color;
        ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < particleSystems.Length; i++)
        {
            var main = particleSystems[i].main;
            main.startColor = color;
        }

        ParticleSystemRenderer[] particleSystemRenderers = GetComponentsInChildren<ParticleSystemRenderer>();
        for (int i = 0; i < particleSystemRenderers.Length; i++)
        {
            if (particleSystemRenderers[i].trailMaterial != null)
            {
                Material mat = Material.Instantiate(particleSystemRenderers[i].trailMaterial);
                mat.SetColor("_EmissionColor", color * 3);
                particleSystemRenderers[i].trailMaterial = mat;
            }
        }

    }

    private void Update()
    {
        // If the effect hasn't started yet or is over, exit the method
        if (Time.time > m_start + LIFESPAN + DELAY || Time.time < m_start + DELAY)
        {
            return;
        }

        // Times two for half the lifespan
        float intensityAmount = Time.deltaTime / LIFESPAN * MAX_INTENSITY * 2f;
        float rangeAmount = Time.deltaTime / LIFESPAN * MAX_RANGE * 2f;

        if (m_light.intensity > MAX_INTENSITY && m_light.range > MAX_RANGE)
        {
            m_growing = false;
        }

        if (m_growing)
        {
            m_light.intensity += intensityAmount;
            m_light.range += rangeAmount;
        }
        else
        {
            m_light.intensity -= intensityAmount;
            m_light.range -= rangeAmount;
        }
    }
}
