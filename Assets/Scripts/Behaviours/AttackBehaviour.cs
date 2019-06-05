using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Bot), typeof(Action))]
public class AttackBehaviour : BotBehaviour
{
    private Action m_action;
    private Bot m_bot;
    private Vector3 m_lastShotPosition;

    // How far the bot can see
    private const float VISION_RANGE = 7.5f;
    // Accuracy handicap in degrees
    private const float ACCURACY_MODIFIER = 1.3f;
    private const float ACCURACY = 0.75f;
    private const float MOVE_THRESHOLD = 1f;
    private const float AIM_THRESHOLD = 3f;

    private void Start()
    {
        m_action = GetComponent<Action>();
        m_bot = GetComponent<Bot>();
    }

    private void FixedUpdate()
    {
        if (!m_active || m_bot.IsDead)
        {
            return;
        }
        FireAtClosestEnemy();
    }

    private void FireAtClosestEnemy()
    {
        // Get all players that aren't on the bot's team
        List<Soldier> enemies = TeamManager.GetAliveEnemiesByTeam(m_bot.Team.Id);
        Soldier closestEnemy = FindClosestEnemy(enemies);
        Vector3 targetPosition = closestEnemy.transform.position;
        // Worldspace -> localspace
        Vector3 rayCastTarget = targetPosition - transform.position;
        // Raycast to the target
        Physics.Raycast(transform.position, rayCastTarget, out RaycastHit closestHit, VISION_RANGE);
        // If the closest raycast object is not a player or is the same team as the bot, return
        if (closestHit.collider == null || closestHit.collider.tag != "Player" || closestHit.collider.GetComponent<Soldier>().Team == m_bot.Team)
        {
            m_bot.AimAtMoveDirection();
            return;
        }

        // Make bot less accurate to make it more fair for players
        Vector3 aimTarget = JitterAim(targetPosition);

        // Make bot predict movement
        aimTarget = PredictMovement(aimTarget, closestEnemy);
        if (FireAtPosition(aimTarget))
        {
            m_lastShotPosition = aimTarget;
        }
    }

    private Vector3 JitterAim(Vector3 position)
    {
        Vector3 result;
        if (m_lastShotPosition == null || (Vector3.Distance(m_lastShotPosition, position) > AIM_THRESHOLD && Vector3.Distance(m_lastShotPosition - transform.position, position - transform.position) > AIM_THRESHOLD))
        {
            float distance = Vector3.Distance(position, transform.position);
            result = Quaternion.Euler(0, Random.Range(-distance * ACCURACY_MODIFIER, distance * ACCURACY_MODIFIER), 0) * position;
        }
        else
        {
            Vector3 direction = position - transform.position;
            Quaternion aimRotation = Quaternion.Euler(0, Mathf.Atan2(direction.x, direction.z), 0);
            Vector3 lastDirection = m_lastShotPosition - transform.position;
            Quaternion lastAimRotation = Quaternion.Euler(0, Mathf.Atan2(lastDirection.x, lastDirection.z), 0);
            result = Vector3.Lerp(m_lastShotPosition, position, ACCURACY);
        }

        return result;
    }

    private Vector3 PredictMovement(Vector3 position, Soldier target)
    {
        if (target.GetComponent<Rigidbody>().velocity.magnitude < MOVE_THRESHOLD)
        {
            return position;
        }
        Vector3 prediction = position - m_lastShotPosition;
        Debug.Log($"Position: {position}");
        Debug.Log($"Last position: {m_lastShotPosition}");
        Debug.Log($"Prediction: {prediction}");
        Debug.Log($"Position + prediction: {position + prediction}");
        return position + prediction;
    }

    private bool FireAtPosition(Vector3 position)
    {
        m_bot.WorldAim(new Vector2(position.x, position.z));
        return m_action.Shoot();
    }

    private Soldier FindClosestEnemy(List<Soldier> enemies)
    {
        Vector3 currentPosition = transform.position;
        float minDist = Mathf.Infinity;
        Soldier closestEnemy = null;
        enemies.ForEach(enemy =>
        {
            float dist = Vector3.Distance(currentPosition, enemy.transform.position);
            if (dist < minDist)
            {
                closestEnemy = enemy;
                minDist = dist;
            }
        });

        return closestEnemy;
    }
}
