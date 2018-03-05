using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerLobbyHook : Prototype.NetworkLobby.LobbyHook
{
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        Debug.Log("TEST ON LOBBY SERVER SCENE LOADED");

        if (lobbyPlayer == null)
            return;

        Prototype.NetworkLobby.LobbyPlayer lp = lobbyPlayer.GetComponent<Prototype.NetworkLobby.LobbyPlayer>();

        /*if (lp != null)
            GameManager.AddPlayer(gamePlayer);*/
    }
}
