/**
 * Authors: David, Benji
 */

using Mirror;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ShootController : NetworkBehaviour
{
    // Amount of energy a bullet will consume
    public int Cost;

    [SerializeField] private float m_soundVolume = 0;
    [SerializeField] private AudioClip m_shootSound = null;
    [SerializeField] private AudioSource m_audioSource = null;
    // Prefab representing the bullet
    [SerializeField] private GameObject m_prefab = null;
    // Transform object representing the bullets' spawn location
    [SerializeField] private Transform m_spawn = null;
    // Fire cooldown in seconds
    [SerializeField] private float m_cooldown = 0;
    // The next time the entity will be able to shoot, in seconds
    private float m_next;

    public bool CanShoot(int energy)
    {
        return Time.time > m_next && energy >= Cost;
    }

    public void Fire()
    {
        m_next = Time.time + m_cooldown;
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
        GameObject prefab = Instantiate(m_prefab, m_spawn.position, m_spawn.rotation);
        Bullet bullet = prefab.GetComponent<Bullet>();
        bullet.ShooterId = shooterId;
        bullet.SetBulletColor();
        bullet.GetComponent<Identity>().SetIdentity();
        m_audioSource.pitch = Random.Range(0.9f, 1f);
        m_audioSource.PlayOneShot(m_shootSound, m_soundVolume);
    }
}
