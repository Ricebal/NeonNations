using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerDash : NetworkBehaviour
{
    private float m_multiplier = 1f;
    private const int COST = 20;
    private const float DURATION = 0.05f;
    private const float COOLDOWN = 1f;
    private float m_start;
    private const float MULTIPLIER_AMOUNT = 7.5f;
    private AfterImagePool m_imagePool;

    private void Start() {
        m_imagePool = GetComponent<AfterImagePool>();
    }

    // Start the dash, set speed multiplier and afterimages
    public void StartDash()
    {
        m_multiplier = MULTIPLIER_AMOUNT;
        m_start = Time.time;
        m_imagePool.CreatePool();
        m_imagePool.ShowImages = true;
    }


    // End dash, reset the speed multiplier and start the afterimage fadeout
    private void EndDash()
    {
        m_multiplier = 1f;
        m_imagePool.ShowImages = false;
    }

    public bool CanDash(int energy)
    {
        return energy >= COST && Time.time > m_start + COOLDOWN;
    }

    public int GetCost() 
    {
        return COST;
    }

    public float GetMultiplier()
    {
        return m_multiplier;
    }

    private void FixedUpdate()
    {
        if(Time.time > m_start + DURATION) 
        {
            EndDash();
        }
    }
}
