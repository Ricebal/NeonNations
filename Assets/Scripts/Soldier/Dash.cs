﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour
{
    public float Multiplier = 10f;
    public int Cost = 20;
    public float Duration = 0.05f;
    public float Cooldown = 1f;
    private float m_start;
    private float m_currentMultiplier = 1f;
    private AfterImageController m_afterImageController;

    private void Start()
    {
        m_afterImageController = GetComponent<AfterImageController>();
    }

    // Start the dash, set speed multiplier and afterimages
    public void StartDash()
    {
        m_start = Time.time;
        m_currentMultiplier = Multiplier;
        m_afterImageController.StartAfterImages();
    }

    // End dash, reset the speed multiplier and start the afterimage fadeout
    private void EndDash()
    {
        m_currentMultiplier = 1f;
        m_afterImageController.StopAfterImages();
    }

    public bool CanDash(int energy)
    {
        return energy >= Cost && Time.time > m_start + Cooldown;
    }

    public float GetMultiplier()
    {
        return m_currentMultiplier;
    }

    public bool IsDashing()
    {
        return !(Time.time > m_start + Duration);
    }

    private void FixedUpdate()
    {
        if (!IsDashing() && m_afterImageController.IsGenerating())
        {
            EndDash();
        }
    }
}