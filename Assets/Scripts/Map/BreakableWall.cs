using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    protected Stat m_healthStat;

    protected void Start()
    {
        m_healthStat = GetComponent<Stat>();
        m_healthStat.Max = 50;
        m_healthStat.Reset();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_healthStat.GetCurrent() <= 0)
        {
            Destroy(gameObject);
        }
    }

    // If the BreakableWall gets hit by a bullet, it will take damage. Will return true if the collider was a Bullet and the BreakableWall took damage.
    protected bool OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Bullet")
        {
            m_healthStat.ChangeCurrent(-collider.gameObject.GetComponent<Bullet>().Damage);
            return true;
        }
        return false;
    }
}
