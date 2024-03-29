/**
 * Authors: Stella, David, Nicander, Benji, Chiel
 */

using Mirror;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class Soldier : NetworkBehaviour
{
    [SyncVar] public Team Team;
    [SyncVar] public Color Color;
    [SyncVar] public Score Score = new Score();
    public float Speed;
    public string Username;
    public float RespawnTime;
    public bool IsDead = false;
    public Stat Health;
    public Stat Energy;

    [SerializeField] protected float m_energyReloadTime;
    [SerializeField] protected AudioClip m_hitSound;
    [SerializeField] protected float m_hitSoundVolume;
    [SerializeField] protected AudioClip m_deathSound;
    [SerializeField] protected float m_deathSoundVolume;
    [SerializeField] protected AudioSource m_defaultAudioSource;
    [SerializeField] protected AudioSource m_hitAudioSource;
    [SerializeField] protected GameObject m_spotLight;
    [SerializeField] protected int m_maxHealth = 100;
    [SerializeField] protected int m_maxEnergy = 100;
    protected HeadController m_headController;
    protected Gun m_gun;
    protected Renderer m_renderer;
    protected float m_deathTime;
    protected float m_lastEnergyReload;

    private void Awake()
    {
        Health = new Stat(0, m_maxHealth);
        Energy = new Stat(0, m_maxEnergy);
    }

    protected void Start()
    {
        if (isServer)
        {
            GameManager.AddPlayer(this);
        }
    }

    protected void OnDestroy()
    {
        // When a soldier leaves the game, OnCollisionExit is not triggered, this variable has to be reset manually
        foreach (Soldier soldier in TeamManager.GetAllPlayers())
        {
            if (soldier != null)
            {
                soldier.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            }
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
        if (Time.time - m_lastEnergyReload >= m_energyReloadTime)
        {
            m_lastEnergyReload = Time.time;
            Energy.Add(1);
        }
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
        Score.Deaths++;
        Team.AddDeath();

        DeathExplosion deathExplosion = GetComponentInChildren<DeathExplosion>();
        deathExplosion?.Fire();
        m_defaultAudioSource.PlayOneShot(m_deathSound, m_deathSoundVolume);
    }

    public virtual void StopMovement() { }

    protected virtual void Respawn()
    {
        CmdRespawn();
    }

    [Command]
    protected void CmdRespawn()
    {
        // Only able to respawn if the game isn't finished yet.
        if (!GameManager.Singleton.GameFinished)
        {
            Vector2Int spawnPoint = BoardManager.GetMap().GetSpawnPoint(Team);
            RpcRespawn(spawnPoint);
        }
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
        Health.Reset();
        Energy.Reset();
        IsDead = false;
    }

    public void SyncScore()
    {
        RpcSetScore(Score.Username, Score.Kills, Score.Deaths);
    }

    [ClientRpc]
    private void RpcSetScore(string username, int kills, int deaths)
    {
        Score.Username = username;
        Score.Kills = kills;
        Score.Deaths = deaths;
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
        Score.Username = username;
    }

    protected void TakeDamage(int damage, string playerId)
    {
        RpcTakeDamage(damage);
        // If the soldier has no remaining health and is not dead yet, he will die
        if (!IsDead && Health.GetValue() <= 0)
        {
            RpcAddKill(playerId);
            RpcDead();
            GetComponent<BotController>()?.DisableScripts();
        }
    }

    [ClientRpc]
    protected void RpcTakeDamage(int damage)
    {
        Health.Subtract(damage);
    }

    [ClientRpc]
    protected void RpcAddKill(string playerId)
    {
        Soldier otherSoldier = GameObject.Find(playerId).GetComponent<Soldier>();
        otherSoldier.Score.Kills++;
        otherSoldier.Team.AddKill();
    }

    [ClientRpc]
    private void RpcHitByBullet()
    {
        m_spotLight.SetActive(true); // Show the spotlight of the soldier that was hit by a bullet.
        m_spotLight.GetComponent<SpotLightScript>().SetLifeTime(ExplosionLight.LIFETIME, false);
        m_hitAudioSource.pitch = Random.Range(0.75f, 1.25f);
        m_hitAudioSource.PlayOneShot(m_hitSound, m_hitSoundVolume);
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
                RpcHitByBullet();
                if (shooter.Team != Team) // Don't take damage from friendly fire.
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
