using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {

    static public GameManager instance;

    static public List<PlayerBehaviorScript> players = new List<PlayerBehaviorScript>();

    public float startDelay = 3f;           // The delay between the start of RoundStarting and RoundPlaying phases.
    public float endDelay = 3f;             // The delay between the end of RoundPlaying and RoundEnding phases.

    private WaitForSeconds m_StartWait;         // Used to have a delay whilst the round starts.
    private WaitForSeconds m_EndWait;           // Used to have a delay whilst the round or game ends.

    private PlayerBehaviorScript m_GameWinner;           // Reference to the winner of the game.  Used to make an announcement of who won.

    [HideInInspector]
    [SyncVar]
    public bool isFinished = false;

    [ServerCallback]
    private void Start()
    {
        // Create the delays so they only have to be made once.
        m_StartWait = new WaitForSeconds(startDelay);
        m_EndWait = new WaitForSeconds(endDelay);

        // Once the tanks have been created and the camera is using them as targets, start the game.
        StartCoroutine(GameLoop());
    }

    void Awake()
    {
        instance = this;
    }

    static public void AddPlayer(GameObject player)
    {
        Debug.Log("ADD PLAYER");

        PlayerBehaviorScript newPlayer = player.GetComponent<PlayerBehaviorScript>();
        players.Add(newPlayer);
    }

    // This is called from start and will run each phase of the game one after another. ONLY ON SERVER (as Start is only called on server)
    private IEnumerator GameLoop()
    {
        while (players.Count < 2)
            yield return null;

        //wait to be sure that all are ready to start
        yield return new WaitForSeconds(2.0f);

        // Start off by running the 'RoundStarting' coroutine but don't return until it's finished.
        yield return StartCoroutine(RoundStarting());


        // Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished.
        /*yield return StartCoroutine(RoundPlaying());

        // Once execution has returned here, run the 'RoundEnding' coroutine.
        yield return StartCoroutine(RoundEnding());

        // This code is not run until 'RoundEnding' has finished.  At which point, check if there is a winner of the game.
        if (m_GameWinner != null)
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

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return m_StartWait;
    }

    [ClientRpc]
    void RpcRoundStarting()
    {
        //EnablePlayers();
        DisablePlayers();
        Debug.Log("TEST");
        Debug.Log(players.Count);
        Debug.Log("ROUND STARTO");
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
            Debug.Log("SPECIFA");
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
