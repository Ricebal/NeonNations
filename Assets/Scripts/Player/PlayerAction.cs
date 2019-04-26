using UnityEngine;
using UnityEngine.Networking;

public class PlayerAction : NetworkBehaviour
{
    // Prefab representing the bullet
    public GameObject Bullet;
    // Prefab representing the sonar bullet
    public GameObject SonarBullet;
    // Transform object representing the bullets' spawn location
    public Transform BulletSpawn;
    // Amount of bullets spawned by sonar
    public float SonarRays;
    public PlayerDash m_playerDash;
    public PlayerEnergy m_playerEnergy;

    private void Start()
    {
        m_playerDash = GetComponent<PlayerDash>();
        m_playerEnergy = GetComponent<PlayerEnergy>();
    }

    [Command]
    public void CmdShoot()
    {
        GameObject bullet = Instantiate(Bullet, BulletSpawn.position, BulletSpawn.rotation);
        bullet.GetComponent<Bullet>().SetShooter(this.gameObject);

        // Instanciate the bullet on the network for all players 
        NetworkServer.Spawn(bullet);
    }

    [Command]
    public void CmdSonar()
    {
        float amount = 360 / SonarRays;
        for (int i = 0; i < 360; i += (int) amount)
        {
            GameObject sonarBullet = Instantiate(SonarBullet, this.transform.position, Quaternion.Euler(0, i, 0));
            sonarBullet.transform.Translate(new Vector3(0, 0, this.transform.localScale.z / 2f), Space.Self);
            sonarBullet.GetComponent<Bullet>().SetShooter(this.gameObject);

            // Instanciate the bullet on the network for all players 
            NetworkServer.Spawn(sonarBullet);
        }
    }

    public void Dash()
    {
        if (m_playerDash.CanDash(m_playerEnergy.GetCurrentEnergy()))
        {
            m_playerEnergy.AddEnergy(-m_playerDash.Cost);
            m_playerDash.StartDash();
        }
    }
}
