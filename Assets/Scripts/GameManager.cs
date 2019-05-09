using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour
{
    [SyncVar]
    public string Seed;
    public BoardManager BoardScript;

    void Start()
    {
        BoardScript = GetComponent<BoardManager>();
        InitGame();
    }

    void InitGame()
    {
        if (isServer)
        {
            // set seed to be used by generation
            Seed = BoardScript.GenerateSeed();
        }
        BoardScript.SetupScene(Seed);
    }
}
