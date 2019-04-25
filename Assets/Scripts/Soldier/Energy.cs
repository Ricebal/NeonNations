using UnityEngine;
using UnityEngine.UI;

public class Energy : MonoBehaviour
{
    private Soldier m_soldier;
    private int m_currentEnergy;

    void Start()
    {
        m_soldier = GetComponent<Soldier>();
        Reset();
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
        m_currentEnergy = (int)m_soldier.MaxEnergy;
    }

    public int GetCurrentEnergy()
    {
        return m_currentEnergy;
    }
}
