using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour
{
    private BoardManager m_boardManager;
    private TeamManager m_teamManager;

    void Awake()
    {
        m_boardManager = GetComponent<BoardManager>();
        m_teamManager = GetComponent<TeamManager>();
        InitGame();
    }

    void InitGame()
    {
        m_boardManager.SetupScene();
    }

    public void AddPlayer(Player player)
    {
        player.Team = m_teamManager.AddPlayer();
        player.SetInitialColor(m_teamManager.GetColor(player.Team));
    }
}
