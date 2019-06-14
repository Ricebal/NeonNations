/**
 * Authors: Chiel
 */

using UnityEngine;

[RequireComponent(typeof(Bot), typeof(Action))]
public class SonarBehaviour : BotBehaviour
{

    private void FixedUpdate()
    {
        if (m_bot.Energy.GetValue() == m_bot.Energy.GetMaxValue() && Random.Range(1, 101) <= 1)
        {
            m_action.Sonar();
        }
    }
}
