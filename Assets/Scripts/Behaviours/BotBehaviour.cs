using Mirror;

public abstract class BotBehaviour : NetworkBehaviour
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
