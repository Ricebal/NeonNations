using UnityEngine;

public class Bot : Soldier
{
    private Rigidbody m_rigidbody;

    protected new void Start()
    {
        if (!isServer)
        {
            GetComponent<BotController>().DisableScripts();
            return;
        }

        m_rigidbody = GetComponent<Rigidbody>();

        Vector2Int spawnPoint = BoardManager.GetMap().GetSpawnPoint(Team);
        transform.position = new Vector3(spawnPoint.x, 0, spawnPoint.y);

        Username = ProfileMenu.GetRandomName() + " (Bot)";
        CmdUsername(Username);
    }

    protected new void Update()
    {
        base.Update();

        if (!isServer)
        {
            return;
        }

        if (IsDead)
        {
            // Set the bot velocity to 0 while it's dead
            m_rigidbody.velocity = Vector3.zero;

            // Respawn the bot if it's able to respawn
            if (Time.time - m_deathTime >= RespawnTime)
            {
                CmdRespawn();
                GetComponent<BotController>().EnableScripts();
            }

        }
    }

    private new void FixedUpdate()
    {
        if (!isServer)
        {
            return;
        }
        base.FixedUpdate();
    }

    // Should be called from the script that will control the bot
    public void Move(float horizontal, float vertical)
    {
        Vector3 movement = new Vector3(horizontal, 0.0f, vertical);
        m_rigidbody.velocity = movement * Speed;
    }

    public void AimAtMoveDirection()
    {
        LocalAim(new Vector2(m_rigidbody.velocity.x, m_rigidbody.velocity.z));
    }

    // Aims the bot at the input vector in world space
    public void WorldAim(Vector2 position)
    {
        Vector3 target = new Vector3(position.x, 0, position.y);
        Vector3 direction = target - transform.position;
        float rotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, rotation, 0);
    }

    // Aims the bot at the input vector in local space
    public void LocalAim(Vector2 position)
    {
        if (position == Vector2.zero)
        {
            return;
        }

        Vector3 newDirection = new Vector3(position.x, transform.forward.y, position.y);
        // Change the rotation
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    public override void StopMovement()
    {
        if (isServer)
        {
            m_rigidbody.velocity = Vector3.zero;
        }
    }
}
