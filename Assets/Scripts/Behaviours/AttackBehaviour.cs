using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Bot), typeof(Action))]
public class AttackBehaviour : BotBehaviour
{
    private Action m_action;
    private Bot m_bot;
    private TeamManager m_teamManager;

    private void Start()
    {
        m_action = GetComponent<Action>();
        m_bot = GetComponent<Bot>();
        m_teamManager = GameObject.Find("GameManager").GetComponent<TeamManager>();
    }

    private void Update()
    {
        if (!m_active)
        {
            return;
        }
        FireAtClosestEnemy();
    }

    private void FireAtClosestEnemy()
    {
        List<Soldier> enemies = m_teamManager.GetEnemiesByTeam(m_bot.Team);
        Vector2 targetPosition = FindClosestEnemyPosition(enemies);
        FireAtPosition(targetPosition);
    }

    private void FireAtPosition(Vector2 position)
    {
        m_bot.Aim(position);
        m_action.Shoot();

    }

    private Vector2 FindClosestEnemyPosition(List<Soldier> enemies)
    {
        Vector3 currentPosition = transform.position;
        float minDist = Mathf.Infinity;
        Vector3 closestEnemyPosition = Vector3.zero;
        enemies.ForEach(enemy =>
        {
            float dist = Vector3.Distance(currentPosition, enemy.transform.position);
            if (dist < minDist)
            {
                closestEnemyPosition = enemy.transform.position;
                minDist = dist;

            }
        });

        return new Vector2(closestEnemyPosition.x, closestEnemyPosition.z);
    }
}
