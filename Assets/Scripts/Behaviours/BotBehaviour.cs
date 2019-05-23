using UnityEngine;

public abstract class BotBehaviour : MonoBehaviour
{
    protected bool m_active;
    public void Activate()
    {
        m_active = true;
        Debug.Log("Activating behaviour");
    }

    public void Deactivate()
    {
        m_active = false;
    }
}
