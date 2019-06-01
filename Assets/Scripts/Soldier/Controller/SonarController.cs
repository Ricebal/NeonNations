using Mirror;
using UnityEngine;

public class SonarController : NetworkBehaviour
{
    // Amount of energy a sonar will consume
    public int Cost;

    [SerializeField] private float m_soundVolume;
    [SerializeField] private AudioClip m_sonarSound;
    private AudioSource m_audioSource;
    // Prefab representing the sonar
    [SerializeField] private GameObject m_prefab;
    // Sonar cooldown in seconds
    [SerializeField] private float m_cooldown;
    // The next time the entity will be able to use the sonar, in seconds
    private float m_next;

    private void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
    }

    public bool CanSonar(int energy)
    {
        return Time.time > m_next && energy >= Cost;
    }

    public void Fire()
    {
        m_next = Time.time + m_cooldown;
        CmdSonar();
    }

    [Command]
    private void CmdSonar()
    {
        RpcSonar();
    }

    [ClientRpc]
    private void RpcSonar()
    {
        GameObject prefab = Instantiate(m_prefab, transform.position, Quaternion.identity);

        Sonar script = prefab.GetComponent<Sonar>();
        Soldier soldier = GetComponent<Soldier>();
        script.SetColor(soldier.Color);
        m_audioSource.PlayOneShot(m_sonarSound, m_soundVolume);
    }
}
