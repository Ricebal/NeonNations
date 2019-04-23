using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private Slider m_healthSlider;
    private int m_currentHealth; 

    void Start() {
        m_healthSlider = GameObject.Find("HealthSlider").GetComponent<Slider>();
        Reset();
    }

    public void TakeDamage(int amount) {
        m_currentHealth -= amount;
        if(m_currentHealth > 100) {
            m_currentHealth = 100;
        }
        if (m_currentHealth < 0) {
            m_currentHealth = 0;
        }
        m_healthSlider.value = m_currentHealth;
    }

    public void Reset() {
        m_currentHealth = (int) m_healthSlider.maxValue;
        m_healthSlider.value = m_currentHealth;
    }

    public int GetCurrentHealth() {
        return m_currentHealth;
    }
}
