using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SpotLightScript : MonoBehaviour
{
    [SerializeField] private Light m_spotLight;
    [SerializeField] private float m_increment;
    private const float MIN_RANGE = 3.5f;
    private const float MAX_RANGE = 4.5f;
    private float m_lifeTime;
    private float m_timeAlive;
    private bool m_growing;
    private bool m_sonar; // Because the sonar fades away slower, it needs to know if it's a sonar or not.

    private void OnEnable()
    {
        m_spotLight.range = MIN_RANGE;
        m_growing = true;
        m_timeAlive = 0;
    }
    private void Update()
    {
        m_timeAlive += Time.deltaTime;
        if (m_growing)
        {
            if(m_spotLight.range < MAX_RANGE)
            {
                m_spotLight.range += m_increment;
            }
            if (m_timeAlive > m_lifeTime / 2)
            {
                m_growing = false;
            }
        }
        else
        {
            if (m_sonar)
            {
                m_spotLight.range -= (float)(m_increment * 0.5); // Because the sonar fades away slower.
            }
            else
            {
                m_spotLight.range -= m_increment;
            }
        }
        if (m_timeAlive > m_lifeTime)
        {
            m_spotLight.gameObject.SetActive(false); // Deactivate the spotLight;
        }
    }

    public void SetLifeTime(float lifeTime, bool sonar)
    {
        m_lifeTime = lifeTime;
        m_growing = true;
        m_timeAlive = 0;
        m_sonar = sonar;
    }
}