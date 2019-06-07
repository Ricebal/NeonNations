using UnityEngine;

[RequireComponent(typeof(Bot), typeof(Action))]
public class SonarBehaviour : BotBehaviour
{

    private void FixedUpdate()
    {
        if (m_bot.EnergyStat.GetValue() == m_bot.EnergyStat.GetMaxValue() && Random.Range(1, 101) <= 1)
        {
            m_action.Sonar();
        }
    }
}
