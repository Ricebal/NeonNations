using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    [SerializeField] protected Stat m_healthStat;

    // If the BreakableWall gets hit by a bullet, it will take damage. Will return true if the collider was a Bullet and the BreakableWall took damage.
    protected bool OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Bullet")
        {
            m_healthStat.Subtract(collider.gameObject.GetComponent<Bullet>().Damage);

            if (m_healthStat.GetValue() <= 0)
            {
                Destroy(gameObject);
            }
            return true;
        }
        return false;
    }
}
