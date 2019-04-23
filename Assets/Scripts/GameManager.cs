using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BoardManager boardScript;

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
