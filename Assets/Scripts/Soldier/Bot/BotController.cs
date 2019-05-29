using UnityEngine;

public class BotController : MonoBehaviour
{
    private SearchBehaviour m_searchBehaviour;
    private AttackBehaviour m_attackBehaviour;
    private GameManager m_gameManager;

    private void Start()
    {
        m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        m_searchBehaviour = GetComponent<SearchBehaviour>();
        m_attackBehaviour = GetComponent<AttackBehaviour>();

        m_gameManager.GameMode.OnGameFinished += DisableBots;
        m_searchBehaviour.Activate();
        m_attackBehaviour.Activate();
    }

    private void DisableBots()
    {
        m_searchBehaviour.enabled = false;
        m_attackBehaviour.enabled = false;
    }
}
