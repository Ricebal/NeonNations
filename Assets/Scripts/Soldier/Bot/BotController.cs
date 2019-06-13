/**
 * Authors: Benji, Chiel, Nicander
 */

using System.Collections.Generic;
using UnityEngine;

public class BotController : MonoBehaviour
{
    private GameEnvironment m_environment;
    private SearchBehaviour m_searchBehaviour;
    private AttackBehaviour m_attackBehaviour;
    private SonarBehaviour m_sonarBehaviour;

    private void OnEnable()
    {
        m_environment = GameEnvironment.CreateInstance(BoardManager.GetMap(), new List<Tile>() { Tile.Wall, Tile.BreakableWall, Tile.Reflector });
        m_searchBehaviour = GetComponent<SearchBehaviour>();
        m_attackBehaviour = GetComponent<AttackBehaviour>();
        m_sonarBehaviour = GetComponent<SonarBehaviour>();

        m_searchBehaviour.Environment = m_environment;
        m_attackBehaviour.Environment = m_environment;
    }

    private void Start()
    {

        GameManager.Singleton.GameMode.OnGameFinished += DisableScripts;
    }

    public void EnableScripts()
    {
        m_searchBehaviour.enabled = true;
        m_attackBehaviour.enabled = true;
        m_sonarBehaviour.enabled = true;
    }

    public void DisableScripts()
    {
        m_searchBehaviour.enabled = false;
        m_attackBehaviour.enabled = false;
        m_sonarBehaviour.enabled = false;
    }
}
