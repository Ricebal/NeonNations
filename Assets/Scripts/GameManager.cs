using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BoardManager boardScript;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        boardScript = GetComponent<BoardManager>();
        InitGame();
    }

    void InitGame()
    {
        boardScript.SetupScene();
    }
}
