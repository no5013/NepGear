using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using SocketIO;

public class MainMenuSceneManager : MonoBehaviour {

    public GameObject socket;
    public GameObject mainmenuScreen;
    public GameObject matchmakingScreen;
    private SocketIOComponent socketIOScript;
    private bool isMatchmaking = false;
    //private AssetBundle myLoadedAssetBundle;
    //private string[] scenePaths;
	// Use this for initialization
	void Start () {
        //myLoadedAssetBundle = AssetBundle.LoadFromFile("Assets/Scene");
        //scenePaths = myLoadedAssetBundle.GetAllScenePaths();
        socketIOScript = socket.GetComponent<SocketIOComponent>();
    }
	
	// Update is called once per frame
	void Update () {
	    if(isMatchmaking)
        {
            mainmenuScreen.SetActive(false);
            matchmakingScreen.SetActive(true);
        }
        else
        {
            matchmakingScreen.SetActive(false);
            mainmenuScreen.SetActive(true);
        }
	}
    public void LoadDemo()
    {
        //Debug.Log(scenePaths.ToString());
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
    }
    public void CancelMatchMaking()
    {
        socketIOScript.Emit("CancelMatchmaking");
        isMatchmaking = false;
    }
    public void Matchmaking()
    {
        socketIOScript.Emit("Matchmaking");
        isMatchmaking = true;

    }
}
