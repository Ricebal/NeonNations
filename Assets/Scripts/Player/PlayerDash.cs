using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    private const float MULTIPLIER = 10f;
    private const int COST = 20;
    private const float DURATION = 0.05f;
    private const float COOLDOWN = 1f;
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
        m_currentMultiplier = MULTIPLIER;
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
        return energy >= COST && Time.time > m_start + COOLDOWN;
    }

    public int GetCost()
    {
        return COST;
    }

    public float GetMultiplier()
    {
        return m_currentMultiplier;
    }

    public bool IsDashing()
    {
        return !(Time.time > m_start + DURATION);
    }

    private void FixedUpdate()
    {
        if (!IsDashing() && m_afterImageController.IsGenerating())
        {
            EndDash();
        }
    }
}
