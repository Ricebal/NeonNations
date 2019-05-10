using UnityEngine;

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
        // Life is bounded between 0 and MaxHealth
        m_currentHealth = Mathf.Max(0, Mathf.Min(m_currentHealth, MaxHealth));
    }

    public void AddEnergy(int value)
    {
        m_currentEnergy += value;
        // Energy is bounded between 0 and MaxEnergy
        m_currentEnergy = Mathf.Max(0, Mathf.Min(m_currentEnergy, MaxEnergy));
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
