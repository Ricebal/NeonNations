/**
 * Authors: Nicander, Stella
 */

using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    private Stat m_healthStat;
    private Stat m_energyStat;
    private Slider m_healthSlider;
    private Slider m_energySlider;

    private void Start()
    {
        m_healthSlider = GameObject.Find("HealthSlider").GetComponent<Slider>();
        m_energySlider = GameObject.Find("EnergySlider").GetComponent<Slider>();
        Soldier soldier = GetComponent<Soldier>();
        m_healthStat = soldier.HealthStat;
        m_energyStat = soldier.EnergyStat;
    }

    public void UpdateHUD()
    {
        m_healthSlider.value = m_healthStat.GetValue();
        m_energySlider.value = m_energyStat.GetValue();
    }
}
