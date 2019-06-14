/**
 * Authors: David, Benji
 */

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Reflector : MonoBehaviour
{
    [SerializeField] private AudioClip m_hitSound = null;
    [SerializeField] private float m_hitSoundVolume = 0;
    [SerializeField] private AudioSource m_audioSource = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet")
        {
            m_audioSource.pitch = Random.Range(0.75f, 1.25f);
            m_audioSource.PlayOneShot(m_hitSound, m_hitSoundVolume);
        }
    }

}
