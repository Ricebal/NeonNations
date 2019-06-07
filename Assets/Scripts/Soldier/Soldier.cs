using Mirror;
using UnityEngine;

public abstract class Soldier : NetworkBehaviour
{
    [SyncVar] public Team Team;
    [SyncVar] public Color Color;
    [SyncVar] public Score PlayerScore = new Score();
    public float Speed;
    public string Username;
    // The respawn time of the soldier
    public float RespawnTime;
    public bool IsDead = false;
    public Stat EnergyStat;

    [SerializeField] protected Stat m_healthStat;

    protected HeadController m_headController;
    protected Gun m_gun;
    [SerializeField] protected GameObject m_spotLight;
    protected Renderer m_renderer;
    protected float m_deathTime;
    protected int m_updateCount;

    protected void Start()
    {
        if (isServer)
        {
            GameManager.AddPlayer(this);
        }
        m_updateCount = 0;
    }

    protected void OnDestroy()
    {
        if (isServer)
        {
            GameManager.RemovePlayer(this);
        }
    }

    public override void OnStartClient()
    {
        m_renderer = GetComponent<Renderer>();
        m_headController = GetComponentInChildren<HeadController>();
        m_gun = GetComponentInChildren<Gun>();
    }

    protected void FixedUpdate()
    {
        if (m_updateCount % 4 == 0)
        {
            EnergyStat.Add(1);
        }
        m_updateCount++;
    }

    protected void Update()
    {
        // If the Soldier is respawning, make him fade away
        if (IsDead)
        {
            float newAlpha = Mathf.Max(0, (RespawnTime - (Time.time - m_deathTime)) / RespawnTime);
            Color newColor = new Color(Color.r, Color.g, Color.b, newAlpha);
            m_renderer.material.color = newColor;
            m_headController?.SetColor(newColor);
            m_gun?.SetColor(newColor);
        }
    }

    [ClientRpc]
    private void RpcDead()
    {
        Die();
    }

    protected virtual void Die()
    {
        IsDead = true;
        gameObject.layer = 9; // DeadPlayers layer;
        m_deathTime = Time.time;
        PlayerScore.Deaths++;
        Team.AddDeath();

        DeathExplosion deathExplosion = GetComponentInChildren<DeathExplosion>();
        deathExplosion?.Fire();
    }

    public virtual void StopMovement() { }

    protected virtual void Respawn()
    {
        CmdRespawn();
    }

    [Command]
    protected void CmdRespawn()
    {
        Vector2Int spawnPoint = BoardManager.GetMap().GetSpawnPoint();
        RpcRespawn(spawnPoint);
    }

    [ClientRpc]
    private void RpcRespawn(Vector2Int spawnPoint)
    {
        Respawn(spawnPoint);
    }

    protected virtual void Respawn(Vector2Int spawnPoint)
    {
        transform.position = new Vector3(spawnPoint.x, 0, spawnPoint.y);
        gameObject.layer = 8; // Players layer
        m_renderer.material.color = Color;
        m_headController?.SetColor(Color);
        m_gun?.SetColor(Color);
        m_healthStat.Reset();
        EnergyStat.Reset();
        IsDead = false;
    }

    public void SyncScore()
    {
        RpcSetScore(PlayerScore.Username, PlayerScore.Kills, PlayerScore.Deaths);
    }

    [ClientRpc]
    private void RpcSetScore(string username, int kills, int deaths)
    {
        PlayerScore.Username = username;
        PlayerScore.Kills = kills;
        PlayerScore.Deaths = deaths;
    }

    public void SetInitialColor(Color color)
    {
        Color newColor = new Color(color.r, color.g, color.b, 1f);
        Color = newColor;
        CmdColor(gameObject, newColor);
    }

    [Command]
    protected void CmdColor(GameObject obj, Color color)
    {
        RpcColor(obj, color);
    }

    [ClientRpc]
    protected void RpcColor(GameObject obj, Color color)
    {
        obj.GetComponent<Renderer>().material.color = color;
        DeathExplosion deathExplosion = obj.GetComponentInChildren<DeathExplosion>();
        deathExplosion?.SetColor(color);

        m_headController?.SetColor(color);
        m_gun?.SetColor(color);
    }

    [Command]
    protected void CmdUsername(string username)
    {
        RpcUsername(username);
    }

    [ClientRpc]
    protected void RpcUsername(string username)
    {
        Username = username;
        PlayerScore.Username = username;
    }

    protected void TakeDamage(int damage, string playerId)
    {
        RpcTakeDamage(damage);
        // If the soldier has no remaining health and is not dead yet, he will die
        if (!IsDead && m_healthStat.GetValue() <= 0)
        {
            RpcAddKill(playerId);
            RpcDead();
        }
    }

    [ClientRpc]
    protected void RpcTakeDamage(int damage)
    {
        m_healthStat.Subtract(damage);
    }

    [ClientRpc]
    protected void RpcAddKill(string playerId)
    {
        Soldier otherSoldier = GameObject.Find(playerId).GetComponent<Soldier>();
        otherSoldier.PlayerScore.Kills++;
        otherSoldier.Team.AddKill();
    }

    [ClientRpc]
    private void RpcShowSpotLight()
    {
        m_spotLight.SetActive(true); // Show the spotlight of the soldier that was hit by a bullet.
        m_spotLight.GetComponent<SpotLightScript>().SetLifeTime(ExplosionLight.LIFETIME, false);
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
            if (bullet.ShooterId != transform.name) // Don't light up when you're hit by your own bullet.
            {
                RpcShowSpotLight();
                if (shooter.Team != this.Team) // Don't take damage from friendly fire.
                {
                    TakeDamage(bullet.Damage, bullet.ShooterId);
                }
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
