using UnityEngine;

public class BotController : MonoBehaviour
{
    private SearchBehaviour m_searchBehaviour;
    private AttackBehaviour m_attackBehaviour;

    private void Start()
    {
        m_searchBehaviour = GetComponent<SearchBehaviour>();
        m_attackBehaviour = GetComponent<AttackBehaviour>();

        GameManager.Singleton.GameMode.OnGameFinished += DisableBots;
        //m_searchBehaviour.Activate();
        //m_attackBehaviour.Activate();
    }

    private void DisableBots()
    {
        m_searchBehaviour.enabled = false;
        m_attackBehaviour.enabled = false;
    }
}
