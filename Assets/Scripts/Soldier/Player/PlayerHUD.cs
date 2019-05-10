using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    private Slider m_healthSlider;
    private Slider m_energySlider;
    private Stat m_healthStat;
    private Stat m_energyStat;

    // Start is called before the first frame update
    void Start()
    {
        m_healthStat = GetComponents<Stat>()[0];
        m_energyStat = GetComponents<Stat>()[1];
        m_healthSlider = GameObject.Find("HealthSlider").GetComponent<Slider>();
        m_energySlider = GameObject.Find("EnergySlider").GetComponent<Slider>();
    }

    public void UpdateHUD()
    {
        m_healthSlider.value = m_healthStat.GetCurrent();
        m_energySlider.value = m_energyStat.GetCurrent();
    }
}
