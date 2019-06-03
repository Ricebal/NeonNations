using UnityEngine;

public class Sonar : MonoBehaviour
{
    [SerializeField] private Light m_lightForMap;
    [SerializeField] private Light m_lightForSoldiers;
    [SerializeField] private float m_maxIntensity;
    [SerializeField] private float m_maxRange;
    [SerializeField] private float m_maxRangeForSoldierLight;
    public float LifeSpan;
    private bool m_growing;

    private void Start()
    {
        Destroy(gameObject, LifeSpan);
        m_growing = true;
    }

    public void SetColor(Color color)
    {
        m_lightForMap.color = color;
        // color times three for intensity
        ParticleSystemRenderer particleSystemRenderer = GetComponentInChildren<ParticleSystemRenderer>();
        Material mat = Material.Instantiate(particleSystemRenderer.trailMaterial);
        mat.SetColor("_EmissionColor", color * 3f);
        particleSystemRenderer.trailMaterial = mat;
    }

    private void Update()
    {
        // Times two for half the lifespan
        float intensityAmount = Time.deltaTime / LifeSpan * m_maxIntensity * 2f;
        float rangeAmount = Time.deltaTime / LifeSpan * m_maxRange * 2f;

        if (m_lightForMap.intensity > m_maxIntensity && m_lightForMap.range > m_maxRange)
        {
            m_growing = false;
        }

        if (m_growing)
        {
            // Times two because it should grow faster
            m_lightForMap.intensity += intensityAmount * 2f;
            m_lightForMap.range += rangeAmount * 2f;
            m_lightForSoldiers.intensity += intensityAmount * 2f;
            m_lightForSoldiers.range += rangeAmount * 2f;
            if(m_lightForSoldiers.range > m_maxRangeForSoldierLight)
            {
                m_lightForSoldiers.range = m_maxRangeForSoldierLight;
            }
            
        }
        else
        {
            // Times 0.75 because it should shrink slower
            m_lightForMap.intensity -= intensityAmount * 0.75f;
            m_lightForMap.range -= rangeAmount * 0.75f;
            m_lightForSoldiers.intensity -= intensityAmount * 0.75f;
        }
    }
}
