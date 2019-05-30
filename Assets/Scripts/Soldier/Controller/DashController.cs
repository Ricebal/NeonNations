using UnityEngine;

public class DashController : MonoBehaviour
{
    // Amount of energy a dash will consume
    public int Cost = 20;
    // Maximum speed multiplier
    public float Multiplier = 7.5f;

    [SerializeField] private float m_soundVolume;
    [SerializeField] private AudioClip m_dashSound;
    private AudioSource m_audioSource;
    // Dash duration in secondsd
    [SerializeField] private float m_duration = 0.1f;
    // Dash cooldown in seconds
    [SerializeField] private float m_cooldown = 1f;
    // Start time of the dash in seconds
    private float m_start;
    // Current speed multiplier
    private float m_currentMultiplier = 1f;
    private AfterImageController m_afterImageController;

    private void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        m_afterImageController = GetComponent<AfterImageController>();
    }

    private void FixedUpdate()
    {
        if (!IsDashing() && m_afterImageController.IsGenerating())
        {
            EndDash();
        }
    }

    // Start the dash, set speed multiplier and afterimages
    public void StartDash()
    {
        m_start = Time.time;
        m_currentMultiplier = Multiplier;
        m_afterImageController.StartAfterImages();
        m_audioSource.PlayOneShot(m_dashSound, m_soundVolume);
    }

    // End dash, reset the speed multiplier and start the afterimage fadeout
    private void EndDash()
    {
        m_currentMultiplier = 1f;
        m_afterImageController.StopAfterImages();
    }

    public bool CanDash(int energy)
    {
        return energy >= Cost && Time.time > m_start + m_cooldown;
    }

    public float GetMultiplier()
    {
        return m_currentMultiplier;
    }

    public bool IsDashing()
    {
        return Time.time <= m_start + m_duration;
    }

}
