using UnityEngine;

public class Stat : MonoBehaviour
{
    [SerializeField] private int m_minValue = 0;
    [SerializeField] private int m_maxValue = 0;
    [SerializeField] private int m_currentValue;

    private void Start()
    {
        Reset();
    }

    public void Add(int value)
    {
        m_currentValue += value;
        m_currentValue = Mathf.Max(m_minValue, Mathf.Min(m_currentValue, m_maxValue));
    }

    public void Subtract(int value)
    {
        m_currentValue -= value;
        m_currentValue = Mathf.Max(m_minValue, Mathf.Min(m_currentValue, m_maxValue));
    }

    public void Reset()
    {
        m_currentValue = m_maxValue;
    }

    public int GetValue()
    {
        return m_currentValue;
    }

    public int GetMaxValue()
    {
        return m_maxValue;
    }
}
