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
    // Transform object representing the bullets' spawn location
    public Transform BulletSpawn;

    [Command]
    public void CmdShoot() {
        GameObject bullet = Instantiate(Bullet, BulletSpawn.position, BulletSpawn.rotation) as GameObject;
        bullet.GetComponent<BulletMover>().SetShooter(this.gameObject);

        // Instanciate the bullet on the network for all players 
        NetworkServer.Spawn(bullet);
    }

    [Command]
    public void CmdSonar() {
        for (int i = 0; i < 360; i += 2) {
            GameObject sonarBullet = Instantiate(SonarBullet, this.transform.position, BulletSpawn.rotation * Quaternion.Euler(0.0f, i, 0.0f)) as GameObject;
            sonarBullet.GetComponent<BulletMover>().SetShooter(this.gameObject);

            // Instanciate the bullet on the network for all players 
            NetworkServer.Spawn(sonarBullet);
        }
    }

}
