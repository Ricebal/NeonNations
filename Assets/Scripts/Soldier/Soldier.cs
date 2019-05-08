using UnityEngine;
using UnityEngine.Networking;

public class Soldier : NetworkBehaviour
{
    [SyncVar] public int Team;
    // The speed of the entity
    public float Speed;
    // The respawn time of the soldier
    public float RespawnTime;

    protected bool m_isDead = false;
    protected float m_remainingRespawnTime;

    [SyncVar] protected Color m_initialColor;
    protected Vector3 m_initialPosition;
    protected Stats m_stats;

    protected void Start()
    {
        m_initialPosition = this.transform.position;
        m_stats = GetComponent<Stats>();
    }

    protected void Update()
    {
        // If the Soldier's health is below or equal to 0...
        if (m_stats.GetCurrentHealth() <= 0)
        {
            // ...and the Soldier is not yet dead, the Soldier will die
            if (!m_isDead)
            {
                Die();
            }
            // Make the Soldier fade away
            else if (m_remainingRespawnTime > 0)
            {
                m_remainingRespawnTime -= Time.deltaTime;
                float alpha = m_remainingRespawnTime / RespawnTime;
                Color color = new Color(1, 0.39f, 0.28f, alpha);
                CmdColor(this.gameObject, color);
            }
            // If the Soldier is dead, but is able to respawn
            else
            {
                Respawn();
            }
        }
    }

    protected virtual void Die()
    {
        m_isDead = true;
        m_remainingRespawnTime = RespawnTime;
        CmdLayer(gameObject, 11);
    }

    public void SetInitialColor(Color color)
    {
        Color newColor = new Color(color.r, color.g, color.b, 1f);
        m_initialColor = newColor;
        CmdColor(this.gameObject, newColor);
    }

    [ClientRpc]
    protected void RpcColor(GameObject obj, Color color)
    {
        obj.GetComponent<Renderer>().material.color = color;
    }

    [Command]
    protected void CmdColor(GameObject obj, Color color)
    {
        RpcColor(obj, color);
    }

    [ClientRpc]
    void RpcLayer(GameObject obj, int layer)
    {
        obj.layer = layer;
    }

    [Command]
    void CmdLayer(GameObject obj, int layer)
    {
        RpcLayer(obj, layer);
    }

    protected virtual void Respawn()
    {
        m_isDead = false;
        Debug.Log("Soldier died, returning to default color \n" + m_initialColor);
        CmdColor(this.gameObject, m_initialColor);
        this.transform.position = m_initialPosition;
        m_stats.Reset();
        CmdLayer(gameObject, 10);
    }

    // If the Soldier gets hit by a bullet, it will take damage. Will return true if the collider was a Bullet and the Soldier took damage.
    protected bool OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Bullet")
        {
            Bullet bullet = collider.gameObject.GetComponent<Bullet>();
            if (bullet.GetShooter() != this.gameObject && bullet.GetShooter().GetComponent<Soldier>().Team != this.Team)
            {
                m_stats.TakeDamage(collider.gameObject.GetComponent<Bullet>().Damage);
                return true;
            }
        }
        return false;
    }
}
