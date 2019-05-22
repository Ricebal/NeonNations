using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    protected Stats m_stats;

    protected void Start()
    {
        m_stats = GetComponent<Stats>();
        m_stats.MaxHealth = 50;
        m_stats.Reset();
    }

    // If the BreakableWall gets hit by a bullet, it will take damage. Will return true if the collider was a Bullet and the BreakableWall took damage.
    protected bool OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Bullet")
        {
            m_stats.TakeDamage(collider.gameObject.GetComponent<Bullet>().Damage);

            if (m_stats.GetCurrentHealth() <= 0)
            {
                Destroy(gameObject);
            }
            return true;
        }
        return false;
    }
}
