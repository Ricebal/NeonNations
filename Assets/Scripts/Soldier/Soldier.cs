using UnityEngine;
using UnityEngine.Networking;

public class Soldier : NetworkBehaviour
{
    // The speed of the entity
    public float Speed;
    // The respawn time of the soldier
    public float RespawnTime;

    protected bool m_isDead = false;
    protected float m_deathTime;
    protected Color m_initialColor;
    protected Vector3 m_initialPosition;
    protected Stats m_stats;

    private SphereCollider m_sphereCollider;
    private MeshRenderer m_meshRenderer;
    private Renderer m_renderer;

    public override void OnStartClient()
    {
        m_sphereCollider = GetComponent<SphereCollider>();
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_renderer = GetComponent<Renderer>();
        m_initialColor = m_meshRenderer.material.color;
        m_initialPosition = transform.position;
        m_stats = GetComponent<Stats>();
    }

    protected void Update()
    {
        // If the Soldier is respawning, make him fade away
        if (m_isDead)
        {
            float newAlpha = (RespawnTime - (Time.time - m_deathTime)) / RespawnTime;
            m_renderer.material.color = new Color(1, 0.39f, 0.28f, newAlpha);
        }

        // If the Soldier's health is below or equal to 0
        if (isLocalPlayer && m_stats.GetCurrentHealth() <= 0)
        {
            // If the Soldier is not yet dead, the Soldier will die
            if (!m_isDead)
            {
                Die();
            }
            // If the Soldier is dead, but is able to respawn
            else if (Time.time - m_deathTime >= RespawnTime)
            {
                Respawn();
            }
        }
    }

    protected virtual void Die()
    {
        CmdSendDeathState(true);
    }

    protected virtual void Respawn()
    {
        CmdSendDeathState(false);
    }

    [Command]
    private void CmdSendDeathState(bool isDead)
    {
        RpcReceiveDeathState(isDead);
    }

    [ClientRpc]
    private void RpcReceiveDeathState(bool isDead)
    {
        m_isDead = isDead;
        m_deathTime = Time.time;

        if (isDead)
        {
            m_sphereCollider.enabled = false;
        }
        else
        {
            m_sphereCollider.enabled = true;
            m_renderer.material.color = m_initialColor;
            transform.position = m_initialPosition;
            m_stats.Reset();
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (!isServer)
        {
            return;
        }

        if (collider.gameObject.tag == "Bullet")
        {
            Bullet bullet = collider.gameObject.GetComponent<Bullet>();
            if (bullet.GetShooterId() != transform.name)
            {
                RpcTakeDamage(bullet.Damage);
            }
        }
    }

    [ClientRpc]
    private void RpcTakeDamage(int damage)
    {
        m_stats.TakeDamage(damage);
    }

}
