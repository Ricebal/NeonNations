using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerDash : NetworkBehaviour
{
    private const float SPEED = 100f;
    private const int COST = 20;
    private const float DURATION = 0.05f;
    private const float COOLDOWN = 1f;
    [SyncVar]
    private float m_start;
    private AfterImageController m_afterImageController;

    private void Start()
    {
        m_afterImageController = GetComponent<AfterImageController>();
    }

    // Start the dash, set speed multiplier and afterimages
    [Command]
    public void CmdDash()
    {
        m_start = Time.time;
        m_afterImageController.StartAfterImages();
    }

    // End dash, reset the speed multiplier and start the afterimage fadeout
    private void EndDash()
    {
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

    public float GetSpeed()
    {
        return SPEED;
    }

    public bool IsDashing()
    {
        return !(Time.time > m_start + DURATION);
    }

    private void FixedUpdate()
    {
        if (!IsDashing())
        {
            EndDash();
        }
    }
}
