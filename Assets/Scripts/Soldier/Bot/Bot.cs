using UnityEngine;
using UnityEngine.Networking;

public class Bot : Soldier
{
    private Rigidbody m_rigidbody;

    void Start()
    {
        if (!isServer)
        {
            return;
        }
        base.Start();
        m_rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!isServer)
        {
            return;
        }

        // if the bot is dead, set its velocity to 0
        if (m_isDead)
        {
            m_rigidbody.velocity = new Vector3(0, 0, 0);
        }
        base.Update();
    }

    void FixedUpdate()
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

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(x,0,z));
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            Vector3 target = ray.GetPoint(distance);
            Vector3 direction = target - transform.position;
            float rotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, rotation, 0);
        }
    }

    private void Respawn()
    { 
        base.Respawn();
    }

    // If the player is hit by a bullet, the player gets damaged
    void OnTriggerEnter(Collider collider)
    {
        if (!isServer)
        {
            return;
        }

        if (collider.gameObject.tag == "Bullet" && collider.gameObject.GetComponent<Bullet>().GetShooter() != this.gameObject)
        {
            m_stats.TakeDamage(collider.gameObject.GetComponent<Bullet>().Damage);
        }
    }
}
