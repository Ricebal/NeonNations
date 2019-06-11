using UnityEngine;

public class Reflector : MonoBehaviour
{
    [SerializeField] private AudioClip m_hitSound = null;
    [SerializeField] private float m_hitSoundVolume = 0;
    private AudioSource m_audioSource;

    private void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet")
        {
            m_audioSource.pitch = Random.Range(0.75f, 1.25f);
            m_audioSource.PlayOneShot(m_hitSound, m_hitSoundVolume);
        }
    }

}
