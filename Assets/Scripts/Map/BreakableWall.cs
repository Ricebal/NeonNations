using Mirror;
using UnityEngine;

public class BreakableWall : NetworkBehaviour
{
    public delegate void OnWallDestroyedDelegate(Vector2Int coordinates);
    public event OnWallDestroyedDelegate WallDestroyedHandler;

    [SerializeField] protected Stat m_healthStat;
    [SerializeField] private AudioClip m_hitSound;
    [SerializeField] private float m_hitSoundVolume;
    [SerializeField] private AudioClip m_destroySound;
    [SerializeField] private float m_destroySoundVolume;
    private AudioSource m_audioSource;

    private void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
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
                m_audioSource.pitch = Random.Range(0.75f, 1.25f);
                m_audioSource.PlayOneShot(m_hitSound, m_hitSoundVolume);
            }
            return true;
        }
        return false;
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
        Destroy(audioSource, audioSource.clip.length);
    }
}
