﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Sonar : NetworkBehaviour
{
    [SerializeField] private Light m_light;
    [SerializeField] private float m_maxIntensity;
    [SerializeField] private float m_maxRange;
    [SerializeField] private float m_lifeSpan;
    private bool m_growing;

    // Start is called before the first frame update
    void Start()
    {
        // transform.Find("Particle System").parent = null;
        m_light.intensity = 0;
        m_light.range = 0;
        m_growing = true;
    }

    [ClientRpc]
    public void RpcSetColor(Color color)
    {
        m_light.color = color;
        GetComponentInChildren<ParticleSystemRenderer>().trailMaterial.SetColor("_EmissionColor", color * 3f);
    }

    // Update is called once per frame
    void Update()
    {
        float intensityAmount = Time.deltaTime / m_lifeSpan * m_maxIntensity * 2f;
        float rangeAmount = Time.deltaTime / m_lifeSpan * m_maxRange * 2f;

        if (m_light.intensity > m_maxIntensity && m_light.range > m_maxRange)
        {
            m_growing = false;
        }

        if (m_growing)
        {
            m_light.intensity += intensityAmount * 2f;
            m_light.range += rangeAmount * 2f;
        }
        else
        {
            m_light.intensity -= intensityAmount * 0.75f;
            m_light.range -= rangeAmount * 0.75f;
        }

        if (m_light.intensity <= 0 && m_light.range <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
