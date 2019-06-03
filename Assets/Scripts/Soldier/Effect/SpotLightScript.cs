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
    private float m_lifeTime;
    private float m_timeAlive;
    private bool m_increasing;
    private const float MIN_RANGE = 3.5f;
    private const float MAX_RANGE = 4.5f;

    private void OnEnable()
    {
        m_spotLight.range = MIN_RANGE;
        m_increasing = true;
        m_timeAlive = 0;
    }
    private void Update()
    {
        m_timeAlive += Time.deltaTime;
        if (m_increasing && m_spotLight.range < MAX_RANGE)
        {
            m_spotLight.range += m_increment;
        }
        else
        {
            m_spotLight.range -= m_increment;
        }
        if (m_timeAlive > m_lifeTime/2)
        {
            m_increasing = false;
        }
        if(m_timeAlive > m_lifeTime)
        {
            m_spotLight.gameObject.SetActive(false); // Deactivate the spotLight;
        }
    }

    public void SetLifeTime(float lifeTime)
    {
        m_lifeTime = lifeTime;
        m_increasing = true;
        m_timeAlive = 0;
    }
}