using UnityEngine;
using UnityEngine.UI;

public class PlayerEnergy : MonoBehaviour
{
    private Slider m_energySlider;
    private int m_currentEnergy;

    void Start() {
        m_energySlider = GameObject.Find("EnergySlider").GetComponent<Slider>();
        Reset();
    }

    public void AddEnergy(int value) {
        m_currentEnergy += value;
        if (m_currentEnergy > 100) { 
            m_currentEnergy = 100;  
        }
        if (m_currentEnergy < 0) {
            m_currentEnergy = 0;
        }
        m_energySlider.value = m_currentEnergy;
    }

    public void Reset() {
        m_currentEnergy = (int) m_energySlider.maxValue;
        m_energySlider.value = m_currentEnergy;
    }

    public int GetCurrentEnergy() {
        return m_currentEnergy;
    }
}
