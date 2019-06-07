using UnityEngine;

public class AfterImage : MonoBehaviour
{
    [SerializeField] private Renderer m_renderer = null;
    [SerializeField] private Light m_light = null;
    [SerializeField] private float m_lifespan = 1f;
    private float m_initialIntensity;

    private void Start()
    {
        m_initialIntensity = m_light.intensity;
        Destroy(this.gameObject, m_lifespan);
    }

    private void Update()
    {
        // Set opacity between 0.5 and 0 based on lifespan
        Color oldColor = m_renderer.material.color;
        float newAlpha = oldColor.a - ((Time.deltaTime / m_lifespan) / 2);
        m_renderer.material.color = new Color(oldColor.r, oldColor.g, oldColor.b, newAlpha);

        // Set light intensity between 0.5 and 0 based on lifespan
        float newIntensity = m_light.intensity - ((Time.deltaTime / m_lifespan) * m_initialIntensity);
        m_light.intensity = newIntensity;
    }

    public void SetColor(Color color)
    {
        m_renderer.material.color = color;
        m_light.color = new Color(color.r, color.g, color.b, 1);
    }
}
