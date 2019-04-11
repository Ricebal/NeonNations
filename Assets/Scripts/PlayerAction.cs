using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerAction : NetworkBehaviour
{
    // Prefab representing the bullet
    public GameObject Bullet;
    // Prefab representing the sonar bullet
    public GameObject SonarBullet;
    public Transform BulletSpawn;
    // Fire rate in seconds
    public float FireRate;
    // Sonar rate in seconds
    public float SonarRate;

    // The next time the entity will be able to shoot, in seconds
    private float m_nextFire;
    // The next time the entity will be able to use the sonar, in seconds
    private float m_nextSonar;

    [Command]
    public void CmdShoot() {
        m_nextFire = Time.time + FireRate;

        GameObject bullet = Instantiate(Bullet, BulletSpawn.position, BulletSpawn.rotation) as GameObject;
        bullet.GetComponent<BulletMover>().SetShooter(this.gameObject);

        // Instanciate the bullet on the network for all players 
        NetworkServer.Spawn(bullet);
    }

    [Command]
    public void CmdSonar() {
        m_nextSonar = Time.time + SonarRate;

        for (int i = 0; i < 360; i += 2) {
            GameObject sonarBullet = Instantiate(SonarBullet, this.transform.position, BulletSpawn.rotation * Quaternion.Euler(0.0f, i, 0.0f)) as GameObject;
            sonarBullet.GetComponent<BulletMover>().SetShooter(this.gameObject);

            // Instanciate the bullet on the network for all players 
            NetworkServer.Spawn(sonarBullet);
        }
    }

    public float getNextFire() {
        return m_nextFire;
    }

    public float getNextSonar() {
        return m_nextSonar;
    }

}
