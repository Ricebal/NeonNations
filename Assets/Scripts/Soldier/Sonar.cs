using UnityEngine;
using UnityEngine.Networking;

public class Sonar : NetworkBehaviour
{
    // Prefab representing the sonar bullet
    public GameObject Prefab;
    // Amount of bullets spawned by sonar
    public float Rays;
    // Sonar rate in seconds
    public float Rate;
    // Amount of energy a sonar will consume
    public int Cost;
    // The next time the entity will be able to use the sonar, in seconds
    private float m_next;

    public bool CanSonar(int energy)
    {
        return Time.time > m_next && energy >= Cost;
    }

    public void Fire()
    {
        m_next = Time.time + Rate;
        CmdSonar(transform.name);
    }

    [Command]
    public void CmdSonar(string shooterId)
    {
        RpcSonar(shooterId);
    }

    [ClientRpc]
    private void RpcSonar(string shooterId)
    {
        float amount = 360 / Rays;
        for (int i = 0; i < 360; i += (int)amount)
        {
            GameObject prefab = Instantiate(Prefab, transform.position, Quaternion.Euler(0, i, 0));
            prefab.transform.Translate(new Vector3(0, 0, transform.localScale.z / 2f), Space.Self);

            Bullet bullet = prefab.GetComponent<Bullet>();
            bullet.SetShooterId(shooterId);
            bullet.SetBulletColor();
        }
    }
}
