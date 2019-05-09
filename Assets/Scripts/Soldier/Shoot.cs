using UnityEngine;
using UnityEngine.Networking;

public class Shoot : NetworkBehaviour
{
    // Prefab representing the bullet
    public GameObject Prefab;
    // Transform object representing the bullets' spawn location
    public Transform Spawn;
    // Amount of energy a bullet will consume
    public int Cost;
    // Fire rate in seconds
    public float Rate;
    // The next time the entity will be able to shoot, in seconds
    private float m_next;
    // Bullet color
    private Color m_color;

    public bool CanShoot(int energy)
    {
        return Time.time > m_next && energy >= Cost;
    }

    public void Fire()
    {
        m_next = Time.time + Rate;
        CmdShoot(transform.name);
    }

    [Command]
    private void CmdShoot(string shooterId)
    {
        RpcShoot(shooterId);
    }

    [ClientRpc]
    private void RpcShoot(string shooterId)
    {
        GameObject prefab = Instantiate(Prefab, Spawn.position, Spawn.rotation);
        Bullet bullet = prefab.GetComponent<Bullet>();
        bullet.SetShooterId(shooterId);
        bullet.SetBulletColor();
    }
}
