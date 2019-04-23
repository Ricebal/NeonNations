﻿using UnityEngine;
using UnityEngine.Networking;

public class ExplosionLight : NetworkBehaviour
{
    public float RangeMultiplier;
    public float IntensityMultiplier;
    public float Lifetime;
    public Light Light;
    private bool m_growing = true;
    private float m_timeCount = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        //Destroys explosion after certain time
        NetworkBehaviour.Destroy(gameObject, Lifetime * 2);
    }

    // Update is called once per frame
    void Update()
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
}