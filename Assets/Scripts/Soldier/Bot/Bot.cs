using UnityEngine;
using UnityEngine.Networking;

public class Bot : Soldier
{
    // The speed of the entity
    public float Speed;
    // The respawn time of the bot
    private float m_respawnTime = 5;
    // The time left untill the bot will respawn
    private float m_remainingRespawnTime;
    
    private PlayerController m_playerController;
    private bool m_isDead = false;
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

        // if the bot is dead, /*disable bot's movements and */set its velocity to 0
        if (m_isDead)
        {
            m_rigidbody.velocity = new Vector3(0, 0, 0);
        }

        // If the bot's health is below or equal to 0...
        if (m_health.GetCurrentHealth() <= 0)
        {
            // ...and the bot is not yet dead, the bot will die
            if (!m_isDead)
            {
                Die();
            }
            // Make the bot fade away
            else if (m_remainingRespawnTime > 0)
            {
                float alpha = m_remainingRespawnTime / m_respawnTime;
                Color color = new Color(1, 0.39f, 0.28f, alpha);
                CmdColor(this.gameObject, color);
            }
            // If the bot is dead, but is able to respawn
            else
            {
                Respawn();
            }
        }
    }

    void FixedUpdate()
    {
        if (!isServer)
        {
            return;
        }

        m_energy.AddEnergy(1);
    }

    // Should be called from the script that will controll the bot
    public void Move(float horizontal, float vertical)
    {
        Vector3 movement = new Vector3(horizontal, 0.0f, vertical);

        m_rigidbody.velocity = movement * Speed;
    }

    // Should be called from the script that will controll the bot
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

    private void Die()
    {
        m_isDead = true;
    }

    private void Respawn()
    {
        m_isDead = false;
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
            m_health.TakeDamage(collider.gameObject.GetComponent<Bullet>().Damage);
        }
    }

    //This function seems unnessecairy for now, but will probably still be used later
    void OnDestroy()
    {
        if (!isServer)
        {
            return;
        }
    }
}
