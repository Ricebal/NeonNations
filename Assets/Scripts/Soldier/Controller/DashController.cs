using UnityEngine;

public class DashController : MonoBehaviour
{
    public bool IsDashing = false;
    public float Multiplier = 7.5f;
    public int Cost = 20;
    [SerializeField] private float m_duration = 0.1f;
    [SerializeField] private float m_cooldown = 1f;
    private float m_start;
    private float m_currentMultiplier = 1f;
    private AfterImageController m_afterImageController;

    private void Start()
    {
        m_afterImageController = GetComponent<AfterImageController>();
    }

    private void FixedUpdate()
    {
        if (IsDashing)
        {
            IsDashing = Time.time <= m_start + m_duration;
        }
        else if(m_afterImageController.IsGenerating())
        {
            EndDash();
        }
    }

    // Start the dash, set speed multiplier and afterimages
    public void StartDash()
    {
        m_start = Time.time;
        IsDashing = true;
        m_currentMultiplier = Multiplier;
        m_afterImageController.StartAfterImages();
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
}
