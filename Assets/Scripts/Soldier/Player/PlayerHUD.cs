using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    private Slider m_healthSlider;
    private Slider m_energySlider;
    private Health m_playerHealth;
    private Energy m_playerEnergy;

    // Start is called before the first frame update
    void Start()
    {
        m_playerHealth = GetComponent<Health>();
        m_playerEnergy = GetComponent<Energy>();

        m_healthSlider = GameObject.Find("HealthSlider").GetComponent<Slider>();
        m_energySlider = GameObject.Find("EnergySlider").GetComponent<Slider>();
    }

    public void UpdateHUD()
    {
        m_healthSlider.value = m_playerHealth.GetCurrentHealth();
        m_energySlider.value = m_playerEnergy.GetCurrentEnergy();
    }
}
