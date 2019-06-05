using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    [SerializeField] protected Stat m_healthStat;
    [SerializeField] private AudioClip m_hitSound;
    [SerializeField] private float m_hitSoundVolume;
    [SerializeField] private AudioClip m_destroySound;
    [SerializeField] private float m_destroySoundVolume;
    private GameObject m_audioObject;
    private AudioSource m_audioSource;

    private void Start()
    {
        m_audioObject = new GameObject();
        m_audioObject.transform.parent = transform;
        m_audioObject.transform.position = transform.position;
        m_audioSource = m_audioObject.AddComponent<AudioSource>();
        m_audioSource.maxDistance = 15;
        m_audioSource.minDistance = 1;
        m_audioSource.spatialBlend = 1;
        m_audioSource.rolloffMode = AudioRolloffMode.Linear;
    }

    // If the BreakableWall gets hit by a bullet, it will take damage. Will return true if the collider was a Bullet and the BreakableWall took damage.
    protected bool OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Bullet")
        {
            m_healthStat.Subtract(collider.gameObject.GetComponent<Bullet>().Damage);

            if (m_healthStat.GetValue() <= 0)
            {
                // Play destroy-sound.
                m_audioSource.PlayOneShot(m_destroySound, m_destroySoundVolume);
                m_audioObject.transform.parent = null;
                Destroy(m_audioObject, 1); // Destroy audio after 1 second.
                Destroy(gameObject); // Destroy wall instantly.
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
}
