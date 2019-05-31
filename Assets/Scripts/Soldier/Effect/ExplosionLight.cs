using UnityEngine;

public class ExplosionLight : MonoBehaviour
{
    public float RangeMultiplier;
    public float IntensityMultiplier;
    public float Lifetime;
    public Light Light;

    private bool m_growing = true;
    private float m_timeCount = 0.0f;

    private void Start()
    {
        //Destroys explosion after certain time
        Destroy(gameObject, Lifetime * 2);
    }

    private void Update()
    {
        m_timeCount += Time.deltaTime;
        if (m_timeCount > Lifetime && m_growing)
        {
            m_growing = false;
        }
        if (m_growing)
        {
            Light.range += RangeMultiplier * Time.deltaTime;
            Light.intensity += IntensityMultiplier * Time.deltaTime;
        }
        else
        {
            Light.range -= RangeMultiplier * Time.deltaTime;
            Light.intensity -= IntensityMultiplier * Time.deltaTime;
        }
    }

    public void SetColor(Color color)
    {
        Light.color = color;
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
