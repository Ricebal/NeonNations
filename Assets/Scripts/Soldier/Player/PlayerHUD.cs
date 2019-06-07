using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private Stat m_healthStat = null;
    [SerializeField] private Stat m_energyStat = null;
    private Slider m_healthSlider;
    private Slider m_energySlider;

    private void Start()
    {
        m_healthSlider = GameObject.Find("HealthSlider").GetComponent<Slider>();
        m_energySlider = GameObject.Find("EnergySlider").GetComponent<Slider>();
    }

    public void UpdateHUD()
    {
        m_healthSlider.value = m_healthStat.GetValue();
        m_energySlider.value = m_energyStat.GetValue();
    }
}
