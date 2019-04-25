using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BoardManager BoardScript;

    void Awake()
    {
        BoardScript = GetComponent<BoardManager>();
        InitGame();
    }

    void InitGame()
    {
        BoardScript.SetupScene();
    }
}
