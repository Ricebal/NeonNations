using UnityEngine;
using UnityEngine.UI;

public class Stat : MonoBehaviour
{
    public int Max = 100;
    [SerializeField]
    private int m_current;

    void Start()
    {
        Reset();
    }

    public void ChangeCurrent(int amount)
    {
        m_current += amount;
        if (m_current > Max)
        {
            m_current = Max;
        }
        if (m_current < 0)
        {
            m_current = 0;
        }
    }

    public void Reset()
    {
        m_current = Max;
    }

    public int GetCurrent()
    {
        return m_current;
    }
}
