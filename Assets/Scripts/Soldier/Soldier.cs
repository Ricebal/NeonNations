using UnityEngine;
using UnityEngine.Networking;

public class Soldier : NetworkBehaviour
{
    // The speed of the entity
    public float Speed;
    // Amount of energy a bullet will consume
    public int BulletCost;
    // Amount of energy a sonar will consume
    public int SonarCost;
    // Fire rate in seconds
    public float FireRate;
    // Sonar rate in seconds
    public float SonarRate;
    // Maximum amount of health
    public int MaxHealth;
    // Maximum amount of energy
    public int MaxEnergy;

    // The next time the entity will be able to shoot, in seconds
    protected float m_nextFire;
    // The next time the entity will be able to use the sonar, in seconds
    protected float m_nextSonar;

    protected Color m_initialColor;
    protected Vector3 m_initialPosition;
    protected Action m_playerAction;
    protected Energy m_energy;
    protected Health m_health;

    protected void Start()
    {
        m_initialColor = GetComponent<MeshRenderer>().material.color;
        m_initialPosition = this.transform.position;
        m_playerAction = GetComponent<Action>();
        m_energy = GetComponent<Energy>();
        m_health = GetComponent<Health>();
    }

    public void Shoot()
    {
        // If the cooldown is elapsed and the player has enough energy
        if (Time.time > m_nextFire && m_energy.GetCurrentEnergy() >= BulletCost)
        {
            m_nextFire = Time.time + FireRate;
            m_energy.AddEnergy(-BulletCost);
            m_playerAction.CmdShoot();
        }
    }

    public void Sonar()
    {
        // If the cooldown is elapsed and the player has enough energy
        if (Time.time > m_nextSonar && m_energy.GetCurrentEnergy() >= SonarCost)
        {
            m_nextSonar = Time.time + SonarRate;
            m_energy.AddEnergy(-SonarCost);
            m_playerAction.CmdSonar();
        }
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

    protected void Respawn()
    {
        CmdColor(this.gameObject, m_initialColor);
        this.transform.position = m_initialPosition;
        m_health.Reset();
        m_energy.Reset();
    }
}
