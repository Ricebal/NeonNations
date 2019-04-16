using UnityEngine;
using UnityEngine.UI;

public class PlayerEnergy : MonoBehaviour
{
    private Slider m_energySlider;
    private int m_startingEnergy;
    private int m_currentEnergy;

    void Start() {
        m_energySlider = GameObject.Find("EnergySlider").GetComponent<Slider>();
        m_startingEnergy = (int) m_energySlider.value;
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
        m_currentEnergy = m_startingEnergy;
        m_energySlider.value = m_currentEnergy;
    }

    public int GetCurrentEnergy() {
        return m_currentEnergy;
    }
}
