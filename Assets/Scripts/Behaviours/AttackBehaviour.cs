using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Bot), typeof(Action))]
public class AttackBehaviour : BotBehaviour
{
    private Action m_action;
    private Bot m_bot;

    private void Start()
    {
        m_action = GetComponent<Action>();
    }

    private void FireAtClosestEnemy()
    {
        FireAtPosition(FindClosestEnemyPosition());
    }

    private void FireAtPosition(Vector2 position)
    {
        m_bot.Aim(position);
    }

    private Vector2 FindClosestEnemyPosition()
    {
        return Vector2.zero;
    }
}
