using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Bot), typeof(Action))]
public class AttackBehaviour : BotBehaviour
{
    private Action m_action;
    private Bot m_bot;
    private TeamManager m_teamManager;
    private const float VISION_RANGE = 7.5f;
    private const float INITIAL_ACCURACY = 2f;
    private const float ACCURACY_MODIFIER = 0.01f;
    private float m_accuracy = 1f;
    private Vector3 m_lastShotPosition;

    private void Start()
    {
        m_action = GetComponent<Action>();
        m_bot = GetComponent<Bot>();
        m_teamManager = GameObject.Find("GameManager").GetComponent<TeamManager>();
    }

    private void FixedUpdate()
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
        Vector3 aimTarget;
        Vector3 targetPosition = FindClosestEnemyPosition(enemies);
        Vector3 rayCastTarget = targetPosition - transform.position;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, rayCastTarget, VISION_RANGE);
        RaycastHit closestHit = new RaycastHit();
        float minDist = Mathf.Infinity;
        foreach (RaycastHit hit in hits)
        {
            float dist = Vector3.Distance(transform.position, hit.point);
            if (dist < minDist)
            {
                closestHit = hit;
                minDist = dist;
            }
        }

        if (!(closestHit.collider != null && closestHit.collider.tag == "Player"))
        {
            return;
        }

        aimTarget = JitterAim(closestHit.point);
        // Debug.DrawLine(transform.position, aimTarget);
        FireAtPosition(aimTarget);
        m_lastShotPosition = targetPosition;
    }

    private Vector3 JitterAim(Vector3 position)
    {
        if (m_lastShotPosition != null)
        {
            float movedDistance = Vector3.Distance(m_lastShotPosition, position);
            if (movedDistance > 0.52 || movedDistance < 0.48)
            {
                m_accuracy = INITIAL_ACCURACY;
            }
            else
            {
                m_accuracy = Mathf.Max(0, m_accuracy - ACCURACY_MODIFIER);
            }
        }
        return new Vector3(position.x + Random.Range(-m_accuracy, m_accuracy), position.y, position.z + Random.Range(-m_accuracy, m_accuracy));
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
