using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Slider HealthSlider;

    private int m_startingHealth;
    private int m_currentHealth; 
    private bool m_isDead;                            // Whether the player is dead

    void Start() {
        HealthSlider = GameObject.Find("HealthSlider").GetComponent<Slider>();
        m_startingHealth = (int) HealthSlider.value;
        m_currentHealth = m_startingHealth;
        m_isDead = false;
    }

    public void TakeDamage(int amount) {
        m_currentHealth -= amount;
        HealthSlider.value = m_currentHealth;

        if (m_currentHealth <= 0) {
            m_isDead = true;
        }
    }

    public int GetCurrentHealth() {
        return m_currentHealth;
    }
}
