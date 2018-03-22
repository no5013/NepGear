using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class GameManager : NetworkBehaviour {

    static public GameManager instance;

    static public List<PlayerBehaviorScript> players = new List<PlayerBehaviorScript>();

    public float startDelay = 3f;           // The delay between the start of RoundStarting and RoundPlaying phases.
    public float endDelay = 3f;             // The delay between the end of RoundPlaying and RoundEnding phases.

    public float playerLifeStock = 3f;

    private WaitForSeconds m_StartWait;         // Used to have a delay whilst the round starts.
    private WaitForSeconds m_EndWait;           // Used to have a delay whilst the round or game ends.

    private PlayerBehaviorScript gameWinner;           // Reference to the winner of the game.  Used to make an announcement of who won.

    [HideInInspector]
    [SyncVar]
    public bool isFinished = false;

    public Transform[] spawnPoint_A;
    public Transform[] spawnPoint_B;

    private void Start()
    {
        if (isServer)
        {
            // Create the delays so they only have to be made once.
            m_StartWait = new WaitForSeconds(startDelay);
            m_EndWait = new WaitForSeconds(endDelay);

            RpcMapSetup();

            // Once the tanks have been created and the camera is using them as targets, start the game.
            StartCoroutine(GameLoop());
        }
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
        yield return StartCoroutine(RoundPlaying());

        // Once execution has returned here, run the 'RoundEnding' coroutine.
        yield return StartCoroutine(RoundEnding());

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

        LobbyManager.s_Singleton.ServerReturnToLobby();
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
        DisablePlayers();
        RpcResetPlayers();
        Debug.Log("ROUND STARTING");
    }

    private IEnumerator RoundPlaying()
    {
        //notify clients that the round is now started, they should allow player to move.
        RpcRoundPlaying();

        // While there is not one tank left...
        while (!OnePlayerLeft())
        {
            // ... return on the next frame.
            yield return null;
        }
    }

    private void RpcResetPlayers()
    {
        for (int i = 0; i < players.Count; i++)
        {
            Transform spawnPoint;
            if (i % 2 == 0)
            {
                spawnPoint = spawnPoint_A[0];
            }
            else
            {
                spawnPoint = spawnPoint_B[0];
            }

            players[i].transform.position = spawnPoint.position;
            players[i].transform.rotation = spawnPoint.rotation;
        }
    }

    [ClientRpc]
    void RpcRoundPlaying()
    {
        Debug.Log("START");
        // As soon as the round begins playing let the players control the tanks.
        EnablePlayers();
    }

    private IEnumerator RoundEnding()
    {
        // See if there is a winner now the round is over.
        gameWinner = GetRoundWinner();

        //notify client they should disable tank control
        RpcRoundEnding();

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return m_EndWait;
    }

    [ClientRpc]
    private void RpcRoundEnding()
    {
        DisablePlayers();
        Debug.Log("ROUND ENDING");
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

    [ClientRpc]
    private void RpcMapSetup()
    {
        GameObject[] o_SpawnPoint_A = GameObject.FindGameObjectsWithTag("Spawn_A");
        spawnPoint_A = Utils.gameObjectsToTransforms(o_SpawnPoint_A);

        GameObject[] o_SpawnPoint_B = GameObject.FindGameObjectsWithTag("Spawn_B");
        spawnPoint_B = Utils.gameObjectsToTransforms(o_SpawnPoint_B);
    }
}
