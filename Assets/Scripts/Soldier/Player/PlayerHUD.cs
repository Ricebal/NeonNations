/**
 * Authors: Nicander, Stella
 */

using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    private Stat m_health;
    private Stat m_energy;
    private Slider m_healthSlider;
    private Slider m_energySlider;

    private void Start()
    {
        m_healthSlider = GameObject.Find("HealthSlider").GetComponent<Slider>();
        m_energySlider = GameObject.Find("EnergySlider").GetComponent<Slider>();
        Soldier soldier = GetComponent<Soldier>();
        m_health = soldier.Health;
        m_energy = soldier.Energy;
    }

    public void UpdateHUD()
    {
        m_healthSlider.value = m_health.GetValue();
        m_energySlider.value = m_energy.GetValue();
    }
}
