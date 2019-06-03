using Mirror;
using UnityEngine;
using UnityEngine.Rendering;

public class NetworkLobbyManagerCustom : NetworkLobbyManager
{
    /*
        This code below is to demonstrate how to do a Start button that only appears for the Host player
        showStartButton is a local bool that's needed because OnLobbyServerPlayersReady is only fired when
        all players are ready, but if a player cancels their ready state there's no callback to set it back to false
        Therefore, allPlayersReady is used in combination with showStartButton to show/hide the Start button correctly.
        Setting showStartButton false when the button is pressed hides it in the game scene since NetworkLobbyManager
        is set as DontDestroyOnLoad = true.
    */

    private bool m_showStartButton;

    public override void OnLobbyServerPlayersReady()
    {
        // calling the base method calls ServerChangeScene as soon as all players are in Ready state.
        if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null && startOnHeadless)
        {
            base.OnLobbyServerPlayersReady();
        }
        else
        {
            m_showStartButton = true;
        }
    }

    public override void OnGUI()
    {
        base.OnGUI();

        if (allPlayersReady && m_showStartButton && GUI.Button(new Rect(150, 300, 120, 20), "START GAME"))
        {
            // set to false to hide it in the game scene
            m_showStartButton = false;

            ServerChangeScene(GameplayScene);
        }
    }
}
