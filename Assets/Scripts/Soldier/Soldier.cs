using Mirror;
using UnityEngine;

public abstract class Soldier : NetworkBehaviour
{
    [SyncVar]
    public int Team;
    [SyncVar]
    public Color InitialColor;
    [SyncVar]
    public Score PlayerScore;
    // The speed of the entity
    public float Speed;
    // The respawn time of the soldier
    public float RespawnTime;
    public bool IsDead = false;

    protected float m_deathTime;
    protected Stats m_stats;

    private SphereCollider m_sphereCollider;
    private MeshRenderer m_meshRenderer;
    private Renderer m_renderer;

    public override void OnStartClient()
    {
        m_sphereCollider = GetComponent<SphereCollider>();
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_renderer = GetComponent<Renderer>();
        m_stats = GetComponent<Stats>();
    }

    protected void Update()
    {
        // If the Soldier is respawning, make him fade away
        if (IsDead)
        {
            float newAlpha = Mathf.Max(0, (RespawnTime - (Time.time - m_deathTime)) / RespawnTime);
            m_renderer.material.color = new Color(1, 0.39f, 0.28f, newAlpha);
        }

        if (isServer)
        {
            // If the soldier is able to respawn
            if (IsDead && Time.time - m_deathTime >= RespawnTime)
            {
                Vector2 spawnPoint = GameObject.Find("GameManager").GetComponent<BoardManager>().GetRandomFloorTile();
                RpcRespawn(spawnPoint);
            }
        }
    }

    [ClientRpc]
    private void RpcDead()
    {
        Die();
    }

    [ClientRpc]
    private void RpcRespawn(Vector2 spawnPoint)
    {
        Respawn(spawnPoint);
    }

    protected virtual void Die()
    {
        IsDead = true;
        m_sphereCollider.enabled = false;
        m_deathTime = Time.time;
        PlayerScore.Deaths++;

        DeathExplosion deathExplosion = GetComponentInChildren<DeathExplosion>();
        if (deathExplosion != null)
        {
            deathExplosion.Fire();
        }
    }

    public void SyncScore()
    {
        if (PlayerScore == null)
        {
            PlayerScore = new Score();
        }
        RpcSetScore(PlayerScore.Kills, PlayerScore.Deaths);
    }

    [ClientRpc]
    private void RpcSetScore(int kills, int deaths)
    {
        PlayerScore.Kills = kills;
        PlayerScore.Deaths = deaths;
    }

    protected virtual void Respawn(Vector2 spawnPoint)
    {
        transform.position = new Vector3(spawnPoint.x, 0, spawnPoint.y);
        m_sphereCollider.enabled = true;
        m_renderer.material.color = InitialColor;
        m_stats.Reset();
        IsDead = false;
    }

    public void SetInitialColor(Color color)
    {
        Color newColor = new Color(color.r, color.g, color.b, 1f);
        InitialColor = newColor;
        CmdColor(this.gameObject, newColor);
    }

    [ClientRpc]
    protected void RpcColor(GameObject obj, Color color)
    {
        obj.GetComponent<Renderer>().material.color = color;
        DeathExplosion deathExplosion = obj.GetComponentInChildren<DeathExplosion>();
        if (deathExplosion != null)
        {
            deathExplosion.SetColor(color);
        }
    }

    [Command]
    protected void CmdColor(GameObject obj, Color color)
    {
        RpcColor(obj, color);
    }

    protected void TakeDamage(int damage, string playerId)
    {
        RpcTakeDamage(damage);
        if (m_stats.GetCurrentHealth() <= 0)
        {
            // If the Soldier is not yet dead, the Soldier will die
            if (!IsDead)
            {
                RpcAddKill(playerId);
                RpcDead();
            }
        }
    }

    [ClientRpc]
    protected void RpcTakeDamage(int damage)
    {
        m_stats.TakeDamage(damage);
    }

    [ClientRpc]
    protected void RpcAddKill(string playerId)
    {
        GameObject.Find(playerId).GetComponent<Soldier>().PlayerScore.Kills++;
    }

    protected void OnTriggerEnter(Collider collider)
    {
        if (!isServer)
        {
            return;
        }

        if (collider.gameObject.tag == "Bullet")
        {
            Bullet bullet = collider.gameObject.GetComponent<Bullet>();
            Soldier shooter = GameObject.Find(bullet.ShooterId).GetComponent<Soldier>();
            if (bullet.ShooterId != transform.name && shooter.Team != this.Team)
            {
                TakeDamage(bullet.Damage, bullet.ShooterId);
            }
        }
    }

    protected void OnCollisionEnter(Collision collision)
    {
        // If the player collides with another player, freezes the other player locally to avoid being able to push him
        if (collision.gameObject.tag == "Player" && collision.gameObject != gameObject)
        {
            collision.rigidbody.isKinematic = true;
        }
    }

    protected void OnCollisionExit(Collision collision)
    {
        // If the player is not colliding with another player anymore, unfreezes the other player locally
        if (collision.gameObject.tag == "Player" && collision.gameObject != gameObject)
        {
            collision.rigidbody.isKinematic = false;
        }
    }

}
