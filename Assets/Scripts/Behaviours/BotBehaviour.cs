using Mirror;

public abstract class BotBehaviour : NetworkBehaviour
{
    protected Action m_action;
    protected Bot m_bot;

    protected void Start()
    {
        m_action = GetComponent<Action>();
        m_bot = GetComponent<Bot>();
    }
}
