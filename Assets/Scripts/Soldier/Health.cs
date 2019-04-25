using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    private Soldier m_soldier;
    private int m_currentHealth;

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

    public void Reset()
    {
        m_currentHealth = (int)m_soldier.MaxHealth;
    }

    public int GetCurrentHealth()
    {
        return m_currentHealth;
    }
}
