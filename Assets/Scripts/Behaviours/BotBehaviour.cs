using UnityEngine;

public abstract class BotBehaviour : MonoBehaviour
{
    protected bool m_active = false;
    public void Activate()
    {
        m_active = true;
    }

    public void Deactivate()
    {
        m_active = false;
    }
}
