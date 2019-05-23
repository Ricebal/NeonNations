using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Bot), typeof(Action))]
public class AttackBehaviour : BotBehaviour
{
    private Action m_action;
    private Bot m_bot;
    private TeamManager m_teamManager;
    private const float VISION_RANGE = 5;

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
        Vector3 targetPosition = FindClosestEnemyPosition(enemies);
        Vector3 rayCastTarget = targetPosition - transform.position;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, rayCastTarget, VISION_RANGE);
        bool canFire = true;
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.tag != "Player" && hit.collider.tag != "Bullet")
            {
                canFire = false;
                return;
            }
        }
        if (!canFire)
        {
            return;
        }
        FireAtPosition(targetPosition);
    }

    private void FireAtPosition(Vector3 position)
    {
        m_bot.Aim(new Vector2(position.x, position.z));
        m_action.Shoot();
    }

    private Vector3 FindClosestEnemyPosition(List<Soldier> enemies)
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

        return closestEnemyPosition;
    }
}
