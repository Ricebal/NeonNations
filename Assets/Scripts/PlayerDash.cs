using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerDash : NetworkBehaviour
{
    public float DashSpeed = 1f;
    public int Cost = 20;
    private float m_dashFriction = 0.3f;
    private float m_dashRate = 0f;
    private float m_nextDash;

    public void StartDash()
    {
        DashSpeed = 5f;
        m_nextDash = Time.time + m_dashRate;
    }

    public bool CanDash(int energy)
    {
        return energy >= Cost && DashSpeed == 1f && Time.time > m_nextDash;
    }

    private void FixedUpdate()
    {
        if (DashSpeed > 1)
            DashSpeed -= m_dashFriction;
        else
            DashSpeed = 1;
    }
}
