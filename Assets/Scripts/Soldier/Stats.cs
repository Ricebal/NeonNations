using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    public int MaxHealth = 100;
    public int MaxEnergy = 100;
    private int m_currentHealth;
    private int m_currentEnergy;

    void Start()
    {
        Reset();
    }

    public void TakeDamage(int amount)
    {
        m_currentHealth -= amount;
        if (m_currentHealth > MaxHealth)
        {
            m_currentHealth = MaxHealth;
        }
        if (m_currentHealth < 0)
        {
            m_currentHealth = 0;
        }
    }

    public void AddEnergy(int value)
    {
        m_currentEnergy += value;
        if (m_currentEnergy > MaxEnergy)
        {
            m_currentEnergy = MaxEnergy;
        }
        if (m_currentEnergy < 0)
        {
            m_currentEnergy = 0;
        }
    }

    public void Reset()
    {
        m_currentHealth = MaxHealth;
        m_currentEnergy = MaxEnergy;
    }

    public int GetCurrentHealth()
    {
        return m_currentHealth;
    }

    public int GetCurrentEnergy()
    {
        return m_currentEnergy;
    }
}
