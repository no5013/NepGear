using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class MapSelector : MonoBehaviour {

    private Map[] maps;

    public Text currentMapText;
    
    public int currentMapIndex;

    public RectTransform mapSelectionPanel;

	// Use this for initialization

	void Start () {
        maps = LobbyManager.s_Singleton.resourcesManager.maps;

        currentMapIndex = 0;
        SetShownMap(currentMapIndex);
	}
	
	public void SetShownMap(int mapIndex)
    {
        currentMapText.text = maps[mapIndex].mapName;
    }

    public void OnClickNextMap()
    {
        LobbyPlayer localPlayer = GetComponent<LobbyPlayerList>().FindLocalPlayer();
        localPlayer.OnNextMapClicked();
    }

    public void OnClickPreviousMap()
    {
        LobbyPlayer localPlayer = GetComponent<LobbyPlayerList>().FindLocalPlayer();
        localPlayer.OnNextMapClicked();
    }

    public int NextMap()
    {
        if(currentMapIndex + 1 >= maps.Length)
        {
            currentMapIndex = 0;
        }
        else
        {
            currentMapIndex++;
        }

        return currentMapIndex;
    }

    public int PreviousMap()
    {
        if (currentMapIndex - 1 < 0)
        {
            currentMapIndex = maps.Length-1;
        }
        else
        {
            currentMapIndex--;
        }

        return currentMapIndex;
    }

    public Map GetCurrentMap()
    {
        return maps[currentMapIndex];
    }

}
