using UnityEngine;

public class Bot : Soldier
{
    private Rigidbody m_rigidbody;

    private void Start()
    {
        if (!isServer)
        {
            return;
        }

        m_rigidbody = GetComponent<Rigidbody>();
    }

    protected new void Update()
    {
        base.Update();

        if (!isServer)
        {
            return;
        }

        // if the bot is dead, set its velocity to 0
        if (m_isDead)
        {
            m_rigidbody.velocity = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        if (!isServer)
        {
            return;
        }

        m_stats.AddEnergy(1);
    }

    // Should be called from the script that will control the bot
    public void Move(float horizontal, float vertical)
    {
        if (!isServer)
        {
            return;
        }

        if (!m_isDead)
        {
            Vector3 movement = new Vector3(horizontal, 0.0f, vertical);
            m_rigidbody.velocity = movement * Speed;
        }
    }

    // Aims the bot at the input vector in world space
    public void WorldAim(Vector2 position)
    {
        if (!isServer)
        {
            return;
        }

        Vector3 target = new Vector3(position.x, 0, position.y);
        Vector3 direction = target - transform.position;
        float rotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, rotation, 0);

    }

    // Aims the bot at the input vector in local space
    public void LocalAim(Vector2 position)
    {
        if (!isServer)
        {
            return;
        }

        Vector3 newDirection = new Vector3(position.x, transform.forward.y, position.y);
        // Change the rotation
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

}
