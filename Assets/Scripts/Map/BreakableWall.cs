/**
 * Authors: David, Benji, Chiel, Nicander
 */

using Mirror;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BreakableWall : NetworkBehaviour
{
    public delegate void OnWallDestroyedDelegate(Vector2Int coordinates);
    public event OnWallDestroyedDelegate WallDestroyedHandler;

    [SerializeField] private Stat m_healthStat = null;
    [SerializeField] private AudioClip m_hitSound = null;
    [SerializeField] private float m_hitSoundVolume = 0;
    [SerializeField] private AudioClip m_destroySound = null;
    [SerializeField] private float m_destroySoundVolume = 0;
    [SerializeField] private AudioSource m_audioSource = null;
    [SerializeField] private int m_maxHealth = 0;

    private void Awake()
    {
        m_healthStat = new Stat(0, m_maxHealth);
    }

    // If the BreakableWall gets hit by a bullet, it will take damage. Will return true if the collider was a Bullet and the BreakableWall took damage.
    protected bool OnTriggerEnter(Collider collider)
    {
        if (!isServer)
        {
            return false;
        }

        if (collider.gameObject.tag == "Bullet")
        {
            m_healthStat.Subtract(collider.gameObject.GetComponent<Bullet>().Damage);

            if (m_healthStat.GetValue() <= 0)
            {
                RpcDestroyWall();
            }
            else // Play hit-sound.
            {
                RpcHitSound();
            }
            return true;
        }
        return false;
    }

    [ClientRpc]
    private void RpcHitSound()
    {
        m_audioSource.pitch = Random.Range(0.75f, 1.25f);
        m_audioSource.PlayOneShot(m_hitSound, m_hitSoundVolume);
    }
    [ClientRpc]
    private void RpcDestroyWall()
    {
        PlayDestroySound();

        if (WallDestroyedHandler != null)
        {
            Vector3 position = gameObject.transform.position;
            Vector2Int coordinates = new Vector2Int((int)position.x, (int)position.z);
            WallDestroyedHandler(coordinates);
        }

        // Destroy wall instantly.
        Destroy(gameObject);
    }

    private void PlayDestroySound()
    {
        GameObject soundObject = Instantiate(new GameObject(), transform.position, Quaternion.identity);
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.maxDistance = m_audioSource.maxDistance;
        audioSource.minDistance = m_audioSource.minDistance;
        audioSource.spatialBlend = m_audioSource.spatialBlend;
        audioSource.rolloffMode = m_audioSource.rolloffMode;
        audioSource.clip = m_destroySound;
        audioSource.volume = m_destroySoundVolume;
        audioSource.Play();
        Destroy(soundObject, audioSource.clip.length);
    }
}
