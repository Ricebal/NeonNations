﻿using System.Collections.Generic;
using UnityEngine;

public class BotController : MonoBehaviour
{
    private GameEnvironment m_environment;
    private SearchBehaviour m_searchBehaviour;
    private AttackBehaviour m_attackBehaviour;

    private void OnEnable()
    {
        m_environment = GameEnvironment.CreateInstance(BoardManager.GetMap(), new List<Tile>() { Tile.Wall, Tile.BreakableWall, Tile.Reflector });
        m_searchBehaviour = GetComponent<SearchBehaviour>();
        m_attackBehaviour = GetComponent<AttackBehaviour>();

        m_searchBehaviour.Environment = m_environment;
        m_attackBehaviour.Environment = m_environment;
    }

    private void Start()
    {

        GameManager.Singleton.GameMode.OnGameFinished += DisableBots;
        m_searchBehaviour.Activate();
        m_attackBehaviour.Activate();
    }

    private void DisableBots()
    {
        m_searchBehaviour.enabled = false;
        m_attackBehaviour.enabled = false;
    }
}
