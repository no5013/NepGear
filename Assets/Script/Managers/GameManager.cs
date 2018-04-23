using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class GameManager : NetworkBehaviour {

    static public GameManager instance;

    static public List<PlayerBehaviorScript> players = new List<PlayerBehaviorScript>();

    static public List<PlayerBehaviorScript> players_A = new List<PlayerBehaviorScript>();
    static public List<PlayerBehaviorScript> players_B = new List<PlayerBehaviorScript>();

    private float startDelay = 5f;           // The delay between the start of RoundStarting and RoundPlaying phases.
    private float endDelay = 10f;             // The delay between the end of RoundPlaying and RoundEnding phases.

    public static float playerLifeStock = 3f;

    private WaitForSeconds m_StartWait;         // Used to have a delay whilst the round starts.
    private WaitForSeconds m_EndWait;           // Used to have a delay whilst the round or game ends.

    private PlayerBehaviorScript gameWinner;           // Reference to the winner of the game.  Used to make an announcement of who won.

    [HideInInspector]
    [SyncVar]
    public bool isFinished = false;

    //private UIManager ui;

    private Camera mapCamera;

    public CatapultManager[] spawnPoints_A;
    public CatapultManager[] spawnPoints_B;

    //Startup
    private float matchCountdown = 4f;

    //Return to hangar
    private float returnCountdown = 4f;

    //Time that will stop when the winner is declared
    private float stopCountdown = 1f;

    //Text ui for player
    private string readyText = "READY";
    private string startText = "GO";

    //Time limit
    private float timeLimit = 300f;

    private void Start()
    {
        MapSetup();
        if (isServer)
        {
            // Create the delays so they only have to be made once.
            m_StartWait = new WaitForSeconds(startDelay);
            m_EndWait = new WaitForSeconds(endDelay);
            // Once the tanks have been created and the camera is using them as targets, start the game.
            StartCoroutine(GameLoop());
        }
        //ui = FindObjectOfType<UIManager>().GetComponent<UIManager>();
    }

    void Awake()
    {
        instance = this;
    }

    static public void AddPlayer(GameObject player, string team)
    {
        Debug.Log("ADD PLAYER");

        PlayerBehaviorScript newPlayer = player.GetComponent<PlayerBehaviorScript>();
        newPlayer.team = team;

        players.Add(newPlayer);

        if (team.Equals("A"))
        {
            players_A.Add(newPlayer);
        }
        else
        {
            players_B.Add(newPlayer);
        }

        newPlayer.DisablePlayer();
    }

    static public void AddPlayerAutoTeam(GameObject player)
    {
        string recommendedTeam = RecommendTeam();
        AddPlayer(player, recommendedTeam);
    }

    static public string RecommendTeam()
    {
        if(players_A.Count > players_B.Count)
        {
            return "B";
        }
        else
        {
            return "A";
        }
    }

    public static float GetTeamStock(string team)
    {
        float lifeStock = 10;
        if (team.Equals("A") && players_A.Count > 0)
        {
            lifeStock = players_A[0].lifeStock;
        }
        else if(team.Equals("B") && players_B.Count > 0)
        {
            lifeStock = players_B[0].lifeStock;
        }
        return lifeStock;
    }

    public void RemovePlayer(GameObject player)
    {
        PlayerBehaviorScript toRemove = null;
        foreach (var tmp in players)
        {
            if (tmp == player.GetComponent<PlayerBehaviorScript>())
            {
                toRemove = tmp;
                break;
            }
        }

        if (toRemove != null)
            players.Remove(toRemove);
    }

    /*public void TeamDie(PlayerBehaviorScript player)
    {
        string team = player.team;
        if (team.Equals("A") && players_A.Count > 0)
        {
            stock_A--;
        }
        else if (team.Equals("B") && players_B.Count > 0)
        {
            stock_B--;
        }
    }*/

    [Server]
    public void OnPlayerDie()
    {
        RpcUpdateTeamScore();
    }

    [ClientRpc]
    public void RpcUpdateTeamScore()
    {
        UpdateTeamScore();
    }

    public static void UpdateTeamScore()
    {
        foreach (PlayerBehaviorScript player in players)
        {
            UIManager playerUI = player.uiManager;
            if (playerUI != null)
                playerUI.SetStocks(player.lifeStock, GetTeamStock(GetEnemyTeam(player.team)), playerLifeStock);

        }
    }

    private static string GetEnemyTeam(string currentTeam)
    {
        if (currentTeam.Equals("A"))
        {
            return "B";
        }
        return "A";
    }

    public static float GetEnemyTeamStock(string currentTeam)
    {
        string enemyTeam = GetEnemyTeam(currentTeam);
        return GetTeamStock(enemyTeam);
    }

    /*public void PreparePlayers()
    {
        foreach (PlayerBehaviorScript player in players)
        {
            player.DisablePlayer();
        }
    }*/

    public void OnStockAChange()
    {

    }

    public void OnStockBChange()
    {

    }

    // This is called from start and will run each phase of the game one after another. ONLY ON SERVER (as Start is only called on server)
    private IEnumerator GameLoop()
    {
        while (players.Count < 2)
            yield return null;

        //wait to be sure that all are ready to start
        yield return new WaitForSeconds(2.0f);

        //yield return StartCoroutine(RoundSetup());

        // Start off by running the 'RoundStarting' coroutine but don't return until it's finished.
        yield return StartCoroutine(RoundStarting());

        // Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished.
        yield return StartCoroutine(RoundPlaying());

        // Once execution has returned here, run the 'RoundEnding' coroutine.
        yield return StartCoroutine(RoundEnding());

        yield return StartCoroutine(RoundClosing());

        //ui.HideResult();

        // This code is not run until 'RoundEnding' has finished.  At which point, check if there is a winner of the game.
        /*if (m_GameWinner != null)
        {// If there is a game winner, wait for certain amount or all player confirmed to start a game again
            isFinished = true;
            float leftWaitTime = 15.0f;
            bool allAreReady = false;
            int flooredWaitTime = 15;

            while (leftWaitTime > 0.0f && !allAreReady)
            {
                yield return null;

                allAreReady = true;
                foreach (var tmp in m_Tanks)
                {
                    allAreReady &= tmp.IsReady();
                }

                leftWaitTime -= Time.deltaTime;

                int newFlooredWaitTime = Mathf.FloorToInt(leftWaitTime);

                if (newFlooredWaitTime != flooredWaitTime)
                {
                    flooredWaitTime = newFlooredWaitTime;
                    string message = EndMessage(flooredWaitTime);
                    RpcUpdateMessage(message);
                }
            }

            Prototype.NetworkLobby.LobbyManager.s_Singleton.ServerReturnToLobby();
        }
        else
        {
            // If there isn't a winner yet, restart this coroutine so the loop continues.
            // Note that this coroutine doesn't yield.  This means that the current version of the GameLoop will end.
            StartCoroutine(GameLoop());
        }*/
    }

    private IEnumerator RoundStarting()
    {
        //we notify all clients that the round is starting
        RpcRoundStarting();

        RpcSetPlayerStateText(readyText);

        float remainingTime = matchCountdown;
        int floorTime = Mathf.FloorToInt(remainingTime);

        while (remainingTime > 0)
        {
            yield return null;

            remainingTime -= Time.deltaTime;
            int newFloorTime = Mathf.FloorToInt(remainingTime);

            if (newFloorTime != floorTime)
            {//to avoid flooding the network of message, we only send a notice to client when the number of plain seconds change.
                floorTime = newFloorTime;

                //To Set player ui
                if(floorTime > 0)
                {
                    RpcSetPlayerStateText((floorTime).ToString());
                }
            }
        }

        //To change player ui to go
        RpcSetPlayerStateText(startText);

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return null;
    }

    [ClientRpc]
    void RpcRoundStarting()
    {
        SetCanvasActive(false);
        //DisablePlayers();
        ResetPlayers();
        EnablePlayers();
        DisablePlayerControl();


        Debug.Log("ROUND STARTING");
    }

    private IEnumerator RoundPlaying()
    {
        //notify clients that the round is now started, they should allow player to move.
        RpcRoundPlaying();

        float remainingTime = timeLimit;
        int floorTime = Mathf.FloorToInt(remainingTime *10);

        while (!OnePlayerLeft() && remainingTime > 0f)
        {
            // ... return on the next frame.
            yield return null;

            remainingTime -= Time.deltaTime;
            int newFloorTime = Mathf.FloorToInt(remainingTime * 10);

            if (newFloorTime != floorTime)
            {//to avoid flooding the network of message, we only send a notice to client when the number of plain seconds change.
                floorTime = newFloorTime;

                RpcSetRemainingTime(floorTime);
            }
        }
    }

    private void ResetPlayers()
    {
        for (int i = 0; i < players.Count; i++)
        {
            Transform spawnPoint;
            if (players[i].team.Equals("A"))
            {
                players[i].SetCatapult(spawnPoints_A[0]);
                spawnPoints_A[0].SetupFrame(players[i].gameObject);
            }
            else
            {
                players[i].SetCatapult(spawnPoints_B[0]);
                spawnPoints_B[0].SetupFrame(players[i].gameObject);
            }
        }
    }

    [ClientRpc]
    void RpcRoundPlaying()
    {
        Debug.Log("START");

        launchFrames();
    }

    [ClientRpc]
    void RpcSetRemainingTime(float time)
    {
        SetPlayerRemainingTime(time);
    }

    [ClientRpc]
    void RpcSetPlayerStateText(string text)
    {
        if (text.Equals(startText))
        {
            SetPlayerStateText(text, true);
        }
        else
        {
            SetPlayerStateText(text);
        }
    }

    private void SetPlayerRemainingTime(float time)
    {
        for (int i = 0; i < players.Count; i++)
        {
            UIManager playerUI = players[i].uiManager;
            if (playerUI != null)
            {
                playerUI.SetTime(Mathf.FloorToInt(time/10), time%10);
            }
        }
    }

    private void SetPlayerStateText(string text)
    {
        for (int i = 0; i < players.Count; i++)
        {
            UIManager playerUI = players[i].uiManager;
            if(playerUI != null)
            {
                playerUI.SetStateText(text);
            }
        }
    }

    private void SetPlayerStateText(string text, bool fade)
    {
        for (int i = 0; i < players.Count; i++)
        {
            UIManager playerUI = players[i].uiManager;
            if (playerUI != null)
            {
                playerUI.SetStateText(text);
                if (fade)
                {
                    playerUI.FadeStateText();
                }
            }
        }
    }

    private IEnumerator RoundEnding()
    {
        // See if there is a winner now the round is over.
        
        //notify client they should disable tank control
        RpcRoundEnding();
        RpcSetTimeScale(0);

        float remainingTime = stopCountdown;
        int floorTime = Mathf.FloorToInt(remainingTime);
        while (remainingTime > 0)
        {
            yield return null;

            remainingTime -= Time.unscaledDeltaTime;
            int newFloorTime = Mathf.FloorToInt(remainingTime);

            if (newFloorTime != floorTime)
            {//to avoid flooding the network of message, we only send a notice to client when the number of plain seconds change.
                floorTime = newFloorTime;

                //To Set player ui
                if (floorTime > 0)
                {
                    Debug.Log("Time will run in " + floorTime);
                }
            }
        }
        RpcSetTimeScale(0.05f);
        //RpcSetFixedDeltaTime(0.2f);

        remainingTime = 2f;
        floorTime = Mathf.FloorToInt(remainingTime);
        while (remainingTime > 0)
        {
            yield return null;

            remainingTime -= Time.unscaledDeltaTime;
            int newFloorTime = Mathf.FloorToInt(remainingTime);

            if (newFloorTime != floorTime)
            {//to avoid flooding the network of message, we only send a notice to client when the number of plain seconds change.
                floorTime = newFloorTime;

                //To Set player ui
                if (floorTime > 0)
                {
                    Debug.Log("Time will speed in " + floorTime);
                }
            }
        }
        RpcSetTimeScale(1f);
        //RpcSetFixedDeltaTime(1f);

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return m_EndWait;
    }

    [ClientRpc]
    private void RpcRoundEnding()
    {
        gameWinner = GetRoundWinner();
        //DisablePlayerControl();

        Debug.Log("BATTLE OVER");
    }

    [ClientRpc]
    private void RpcSetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    [ClientRpc]
    private void RpcSetFixedDeltaTime(float scale)
    {
        Time.fixedDeltaTime = Time.timeScale * scale;
    }

    private void DeclareResult()
    {
        RpcDeclareResult();
    }

    [ClientRpc]
    private void RpcDeclareResult()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (gameWinner == players[i])
                players[i].WinGame();
            else
                players[i].LoseGame();
        }
    }

    private IEnumerator RoundClosing()
    {
        float remainingTime = returnCountdown;
        int floorTime = Mathf.FloorToInt(remainingTime);

        while (remainingTime > 0)
        {
            yield return null;

            remainingTime -= Time.deltaTime;
            int newFloorTime = Mathf.FloorToInt(remainingTime);

            if (newFloorTime != floorTime)
            {
                floorTime = newFloorTime;

                if (floorTime > 0)
                {
                    RpcSetPlayerStateText("Return to hangar in " + floorTime.ToString());
                    Debug.Log("Return to hangar in " + floorTime.ToString());
                }
            }
        }

        Prototype.NetworkLobby.LobbyManager.s_Singleton.SendReturnToLobby();

        yield return null;
    }

    [ClientRpc]
    private void RpcGameClosing()
    {
        /*string lobbyScene = FindObjectOfType<LobbyManager>().lobbyScene;
        FindObjectOfType<LobbyManager>().StopClient();
        Destroy(FindObjectOfType<LobbyManager>().gameObject);
        UnityEngine.SceneManagement.SceneManager.LoadScene(lobbyScene);*/
    }

    private bool OnePlayerLeft()
    {
        // Start the count of tanks left at zero.
        int numPlayersLeft = 0;

        // Go through all the tanks...
        for (int i = 0; i < players.Count; i++)
        {
            // ... and if they are active, increment the counter.
            if (!players[i].isOutOfStock())
                numPlayersLeft++;
        }

        // If there are one or fewer tanks remaining return true, otherwise return false.
        return numPlayersLeft <= 1;
    }

    // This function is to find out if there is a winner of the round.
    // This function is called with the assumption that 1 or fewer tanks are currently active.
    private PlayerBehaviorScript GetRoundWinner()
    {
        // Go through all the players...
        for (int i = 0; i < players.Count; i++)
        {
            // ... and if one of them is active, it is the winner so return it.
            if (!players[i].isOutOfStock())
                return players[i];
        }

        // If none of the tanks are active it is a draw so return null.
        return null;
    }

    private void EnablePlayers()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].EnablePlayer();
        }
    }

    private void DisablePlayers()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].DisablePlayer();
        }
    }

    private void EnablePlayerControl()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].EnableControl();
        }
    }

    private void DisablePlayerControl()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].DisableControl();
        }
    }

    private void ShowResult()
    {
        Debug.Log(gameWinner.characterID);
        //ui.SetResult(gameWinner.characterID);
        //ui.ShowResult();
    }

    void MapSetup()
    {
        mapCamera = Camera.main;

        spawnPoints_A = new CatapultManager[1];
        spawnPoints_A[0] = GameObject.FindGameObjectsWithTag("Spawn_A")[0].GetComponent<CatapultManager>();

        spawnPoints_B = new CatapultManager[1];
        spawnPoints_B[0] = GameObject.FindGameObjectsWithTag("Spawn_B")[0].GetComponent<CatapultManager>();
    }

    void SetCanvasActive(bool active)
    {
        Canvas mapCanvas = mapCamera.transform.parent.GetComponentInChildren<Canvas>();
        mapCanvas.enabled = active;
    }

    void launchFrames()
    {
        spawnPoints_A[0].launch();
        spawnPoints_B[0].launch();
    }
}
