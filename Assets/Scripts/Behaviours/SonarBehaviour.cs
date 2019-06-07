using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Bot), typeof(Action))]
public class SonarBehaviour : BotBehaviour
{
    private Action m_action;
    private Bot m_bot;

    private void Start()
    {
        m_action = GetComponent<Action>();
        m_bot = GetComponent<Bot>();
    }

    private void FixedUpdate()
    {
        if (!isServer || !m_active || m_bot.IsDead)
        {
            return;
        }
        if (m_bot.EnergyStat.GetValue() == m_bot.EnergyStat.GetMaxValue() && Random.Range(1, 101) <= 1)
        {
            m_action.Sonar();
        }
    }
}
