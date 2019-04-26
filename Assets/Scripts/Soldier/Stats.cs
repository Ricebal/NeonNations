using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    private Soldier m_soldier;
    private int m_currentHealth;
    private int m_currentEnergy;

    void Start()
    {
        m_soldier = GetComponent<Soldier>();
        Reset();
    }

    public void TakeDamage(int amount)
    {
        m_currentHealth -= amount;
        if (m_currentHealth > 100)
        {
            m_currentHealth = 100;
        }
        if (m_currentHealth < 0)
        {
            m_currentHealth = 0;
        }
    }

    public void AddEnergy(int value)
    {
        m_currentEnergy += value;
        if (m_currentEnergy > 100)
        {
            m_currentEnergy = 100;
        }
        if (m_currentEnergy < 0)
        {
            m_currentEnergy = 0;
        }
    }

    public void Reset()
    {
        m_currentHealth = (int)m_soldier.MaxHealth;
        m_currentEnergy = (int)m_soldier.MaxEnergy;
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
