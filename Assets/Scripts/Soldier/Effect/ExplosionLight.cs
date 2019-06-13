/**
 * Authors: Benji
 */

using UnityEngine;

public class ExplosionLight : MonoBehaviour
{
    [SerializeField] private float m_rangeMultiplier = 0;
    [SerializeField] private float m_intensityMultiplier = 0;
    [SerializeField] private float m_maxRangeForSoldierLight = 0;
    [SerializeField] private Light m_lightForMap = null;
    [SerializeField] private Light m_lightForSoldiers = null;

    public const float LIFETIME = 0.5f;

    private bool m_growing = true;
    private float m_timeAlive = 0.0f;

    private void Start()
    {
        // Destroys explosion after certain time.
        Destroy(gameObject, LIFETIME);
    }

    private void Update()
    {
        m_timeAlive += Time.deltaTime;
        if (m_growing)
        {
            if (m_timeAlive > LIFETIME / 2)
            {
                m_growing = false;
            }
            m_lightForMap.range += m_rangeMultiplier * Time.deltaTime;
            m_lightForMap.intensity += m_intensityMultiplier * Time.deltaTime;
            m_lightForSoldiers.range += m_rangeMultiplier * Time.deltaTime;
            m_lightForSoldiers.intensity += m_intensityMultiplier * Time.deltaTime;
            if (m_lightForSoldiers.range > m_maxRangeForSoldierLight)
            {
                m_lightForSoldiers.range = m_maxRangeForSoldierLight;
            }
        }
        else
        {
            m_lightForMap.range -= m_rangeMultiplier * Time.deltaTime;
            m_lightForMap.intensity -= m_intensityMultiplier * Time.deltaTime;
            m_lightForSoldiers.intensity -= m_intensityMultiplier * Time.deltaTime;
        }
    }

    public void SetColor(Color color)
    {
        m_lightForMap.color = color;
        GetComponentsInChildren<ParticleSystemRenderer>()[0].trailMaterial.SetColor("_EmissionColor", color * 3); ParticleSystemRenderer[] particleSystemRenderers = GetComponentsInChildren<ParticleSystemRenderer>();

        // Make new material.
        Material mat;
        // Change color of particle system.
        if (particleSystemRenderers.Length > 0)
        {
            ParticleSystemRenderer particleSystemRenderer = particleSystemRenderers[0];
            // Set the emission color of the particle trail to the new color times 3 for intensity.
            mat = Material.Instantiate(particleSystemRenderer.material);
            mat.SetColor("_EmissionColor", color * 3);
            particleSystemRenderer.material = mat;
            particleSystemRenderer.trailMaterial = mat;
        }
    }
}
