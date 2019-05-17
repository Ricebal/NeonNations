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
            m_rigidbody.velocity = new Vector3(0, 0, 0);
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
        Vector3 movement = new Vector3(horizontal, 0.0f, vertical);

        m_rigidbody.velocity = movement * Speed;
    }

    // Should be called from the script that will control the bot
    public void Aim(float x, float z)
    {
        if (!isServer)
        {
            return;
        }

        Vector3 newDirection = new Vector3(x, transform.forward.y, z);
        // Change the rotation
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

}
