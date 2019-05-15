using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class Explosion : MonoBehaviour
{
    [SerializeField] private Light m_light;
    private const float LIFESPAN = 1.5f;
    private const float DELAY = 1f;
    private const float MAX_INTENSITY = 3;
    private const float MAX_RANGE = 32;
    private bool m_growing = true;
    private float m_start;

    public void ActivateLight()
    {
        m_light.gameObject.SetActive(true);
        m_light.intensity = 0;
        m_light.range = 0;
        m_growing = true;
        m_start = Time.time;
    }

    public void SetColor(Color color)
    {
        m_light.color = color;
    }

    private void Update()
    {
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
            // Times two because it should grow faster
            m_light.intensity += intensityAmount;
            m_light.range += rangeAmount;
        }
        else
        {
            // Times 0.75 because it should shrink slower
            m_light.intensity -= intensityAmount;
            m_light.range -= rangeAmount;
        }
    }
}
