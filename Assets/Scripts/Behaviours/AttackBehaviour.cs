using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Bot), typeof(Action))]
public class AttackBehaviour : BotBehaviour
{
    public GameEnvironment Environment;

    private Action m_action;
    private Bot m_bot;
    private Vector3 m_lastShotPosition;
    
    // Accuracy handicap
    private const float ACCURACY_MODIFIER = 1.3f;
    // Accuracy between 0 and 1 where 1 is most accurate
    private const float ACCURACY = 0.75f;
    // How far the player has to move before readjusting bot aim
    private const float AIM_THRESHOLD = 3f;

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
        FireAtClosestEnemy();
    }

    private void FireAtClosestEnemy()
    {
        // Get all players that aren't on the bot's team
        List<Soldier> enemies = TeamManager.GetAliveEnemiesByTeam(m_bot.Team.Id);
        enemies = Environment.GetIlluminatedEnemies(m_bot, enemies);

        // If the closest enemy is in line of sight, shoot at it
        if (FindClosestEnemy(enemies, out Soldier closestEnemy))
        {
            Vector3 targetPosition = closestEnemy.transform.position;

            // Make bot less accurate to make it more fair for players
            Vector3 aimTarget = JitterAim(targetPosition);

            // If it fired successfully set last shot position to aimtarget
            if (FireAtPosition(aimTarget))
            {
                m_lastShotPosition = aimTarget;
            }
        }
        else
        {
            // If there is nothing to fire at aim at movement direction
            m_bot.AimAtMoveDirection();
        }
    }

    private Vector3 JitterAim(Vector3 position)
    {
        Vector3 result;
        // If the last position is not set or the player has moved in worldspace and in localspace shoot at target with an offset based on distance
        if (m_lastShotPosition == null || (Vector3.Distance(m_lastShotPosition, position) > AIM_THRESHOLD && Vector3.Distance(m_lastShotPosition - transform.position, position - transform.position) > AIM_THRESHOLD))
        {
            float distance = Vector3.Distance(position, transform.position);
            result = Quaternion.Euler(0, Random.Range(-distance * ACCURACY_MODIFIER, distance * ACCURACY_MODIFIER), 0) * position;
        }
        else
        {
            // Lerp between last and current position based on accuracy between 0-1 where 1 is the most accurate and 0 is the least
            result = Vector3.Lerp(m_lastShotPosition, position, ACCURACY);
            // Make bot predict movement
            result = PredictMovement(result);
        }

        return result;
    }

    private Vector3 PredictMovement(Vector3 position)
    {
        Vector3 prediction = position - m_lastShotPosition;

        return position + prediction;
    }

    private bool FireAtPosition(Vector3 position)
    {
        m_bot.WorldAim(new Vector2(position.x, position.z));
        return m_action.Shoot();
    }

    private bool FindClosestEnemy(List<Soldier> enemies, out Soldier closestEnemy)
    {
        Vector3 currentPosition = transform.position;
        float minDist = Mathf.Infinity;
        bool found = false;
        Soldier enemyFound = null;
        // Loop through the enemies and find the closest one in line of sight
        enemies.ForEach(enemy =>
        {
            float dist = Vector3.Distance(currentPosition, enemy.transform.position);
            if (dist < minDist)
            {
                // Worldspace -> localspace
                Vector3 rayCastTarget = enemy.transform.position - transform.position;
                // Raycast to the target
                Physics.Raycast(transform.position, rayCastTarget, out RaycastHit closestHit);
                // If the closest raycast object is a player and is not on the same team as the bot make the closest enemy the current enemy
                if (closestHit.collider != null && closestHit.collider.tag == "Player")
                {
                    enemyFound = enemy;
                    minDist = dist;
                    found = true;
                }
            }
        });

        closestEnemy = enemyFound;

        return found;
    }
}
