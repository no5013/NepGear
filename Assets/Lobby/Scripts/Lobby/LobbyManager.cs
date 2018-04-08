using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using System.Collections;
using System.Collections.Generic;

namespace Prototype.NetworkLobby
{
    public class LobbyManager : NetworkLobbyManager 
    {
        static short MsgKicked = MsgType.Highest + 1;

        static public LobbyManager s_Singleton;


        [Header("Unity UI Lobby")]
        [Tooltip("Time in second between all players ready & match start")]
        public float prematchCountdown = 5.0f;

        [Space]
        [Header("UI Reference")]
        public LobbyTopPanel topPanel;

        public RectTransform mainMenuPanel;
        public RectTransform lobbyPanel;
        public RectTransform characterPanel;

        public LobbyInfoPanel infoPanel;
        public LobbyCountdownPanel countdownPanel;
        public GameObject addPlayerButton;

        protected RectTransform currentPanel;

        public Button backButton;
        public Button editButton;

        public Text statusInfo;
        public Text hostInfo;

        //Client numPlayers from NetworkManager is always 0, so we count (throught connect/destroy in LobbyPlayer) the number
        //of players, so that even client know how many player there is.
        [HideInInspector]
        public int _playerNumber = 0;

        //used to disconnect a client properly when exiting the matchmaker
        [HideInInspector]
        public bool _isMatchmaking = false;

        protected bool _disconnectServer = false;
        
        protected ulong _currentMatchID;

        protected LobbyHook _lobbyHooks;

        private Dictionary<int, string[]> currentPlayers;

        public ResourcesManager resourcesManager;

        void Start()
        {
            s_Singleton = this;
            currentPlayers = new Dictionary<int, string[]>();
            resourcesManager.Init();
            _lobbyHooks = GetComponent<Prototype.NetworkLobby.LobbyHook>();
            currentPanel = mainMenuPanel;

            backButton.gameObject.SetActive(false);
            editButton.gameObject.SetActive(false);
            GetComponent<Canvas>().enabled = true;

            DontDestroyOnLoad(gameObject);

            SetServerInfo("Offline", "None");
        }

        public override void OnLobbyClientSceneChanged(NetworkConnection conn)
        {
            if (SceneManager.GetSceneAt(0).name == lobbyScene)
            {
                if (topPanel.isInGame)
                {
                    ChangeTo(lobbyPanel);
                    if (_isMatchmaking)
                    {
                        if (conn.playerControllers[0].unetView.isServer)
                        {
                            backDelegate = StopHostClbk;
                        }
                        else
                        {
                            backDelegate = StopClientClbk;
                        }
                    }
                    else
                    {
                        if (conn.playerControllers[0].unetView.isClient)
                        {
                            backDelegate = StopHostClbk;
                        }
                        else
                        {
                            backDelegate = StopClientClbk;
                        }
                    }
                }
                else
                {
                    ChangeTo(mainMenuPanel);
                }

                topPanel.ToggleVisibility(true);
                topPanel.isInGame = false;
            }
            else
            {
                ChangeTo(null);

                Destroy(GameObject.Find("MainMenuUI(Clone)"));

                //backDelegate = StopGameClbk;
                topPanel.isInGame = true;
                topPanel.ToggleVisibility(false);
            }
        }

        public void ChangeTo(RectTransform newPanel)
        {
            if (currentPanel != null)
            {
                currentPanel.gameObject.SetActive(false);
            }

            if (newPanel != null)
            {
                newPanel.gameObject.SetActive(true);
            }

            currentPanel = newPanel;

            if (currentPanel == characterPanel)
            {
                LobbyPlayer localPlayer = LobbyPlayerList._instance.FindLocalPlayer();
                characterPanel.gameObject.GetComponent<CharacterSelector>().lobbyPlayer = LobbyPlayerList._instance.FindLocalPlayer();
            }
            if (currentPanel != mainMenuPanel)
            {
                backButton.gameObject.SetActive(true);
                editButton.gameObject.SetActive(true);
            }
            else
            {
                backButton.gameObject.SetActive(false);
                editButton.gameObject.SetActive(false);
                SetServerInfo("Offline", "None");
                _isMatchmaking = false;
            }
        }

        public void DisplayIsConnecting()
        {
            var _this = this;
            infoPanel.Display("Connecting...", "Cancel", () => { _this.backDelegate(); });
        }

        public void SetServerInfo(string status, string host)
        {
            statusInfo.text = status;
            hostInfo.text = host;
        }


        public delegate void BackButtonDelegate();
        public BackButtonDelegate backDelegate;
        public void GoBackButton()
        {
            backDelegate();
			topPanel.isInGame = false;
        }
        public void OnCharacterEditButton()
        {
            ChangeTo(characterPanel);
            backDelegate = SimpleBackClbk;
        }

        // ----------------- Server management

        public void AddLocalPlayer()
        {
            TryToAddPlayer();
        }

        public void RemovePlayer(LobbyPlayer player)
        {
            player.RemovePlayer();
        }

        public void SimpleBackClbk()
        {
            ChangeTo(mainMenuPanel);
        }
                 
        public void StopHostClbk()
        {
            if (_isMatchmaking)
            {
				matchMaker.DestroyMatch((NetworkID)_currentMatchID, 0, OnDestroyMatch);
				_disconnectServer = true;
            }
            else
            {
                StopHost();
            }

            
            ChangeTo(mainMenuPanel);
        }

        public void StopClientClbk()
        {
            StopClient();

            if (_isMatchmaking)
            {
                StopMatchMaker();
            }

            ChangeTo(mainMenuPanel);
        }

        public void StopServerClbk()
        {
            StopServer();
            ChangeTo(mainMenuPanel);
        }

        class KickMsg : MessageBase { }
        public void KickPlayer(NetworkConnection conn)
        {
            conn.Send(MsgKicked, new KickMsg());
        }

        public void KickedMessageHandler(NetworkMessage netMsg)
        {
            infoPanel.Display("Kicked by Server", "Close", null);
            netMsg.conn.Disconnect();
        }

        //===================

        public override void OnStartHost()
        {
            base.OnStartHost();

            ChangeTo(lobbyPanel);
            backDelegate = StopHostClbk;
            SetServerInfo("Hosting", networkAddress);
        }

        public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId)
        {
            // There should be some problem because I set char selector to be a Network Behavior but there no Network identity on it (not that i need it)
            // any other thing else should work...

            string[] characterInfo = currentPlayers[conn.connectionId];

            Debug.Log(characterInfo[0]);
            Debug.Log(characterInfo[1]);
            Debug.Log(characterInfo[2]);
            //// Instantiate Gun Object and Player Object

            //1.) Get what frame id player select
            //2.) Get scriptable object from frame id
            //3.) Get player prefab from character scripable object

            Character character = resourcesManager.GetCharacter(characterInfo[0]);
            GameObject spawnPlayer = Instantiate(character.characterPrefab, new Vector3(1, 1, 1), Quaternion.identity) as GameObject;

            /*//// Set Child to Camera
            GameObject leftWeapon = Instantiate(weaponPrefabs[character.leftWeaponRef]);
            GameObject rightWeapon = Instantiate(weaponPrefabs[character.rightWeaponRef]);

            leftWeapon.transform.parent = spawnPlayer.GetComponentInChildren<Camera>().transform;
            rightWeapon.transform.parent = spawnPlayer.GetComponentInChildren<Camera>().transform;

            //// Set Local Position
            leftWeapon.transform.localPosition = new Vector3(-0.185f, -0.04f, 0.2f);
            rightWeapon.transform.localPosition = new Vector3(0.185f, -0.04f, 0.2f);*/

            //// VR
            //leftWeapon.transform.parent = spawnPlayer.GetComponentInChildren<LeftController>().transform;
            //rightWeapon.transform.parent = spawnPlayer.GetComponentInChildren<RightController>().transform;

            //// Get Reference
            FrameWeapon frameWeapon = spawnPlayer.GetComponentInChildren<FrameWeapon>();
            PlayerBehaviorScript pbs = spawnPlayer.GetComponent<PlayerBehaviorScript>();
            FrameWeaponController fwc = spawnPlayer.GetComponent<FrameWeaponController>();
            pbs.characterID = characterInfo[0];
            fwc.leftWeaponID = characterInfo[1];
            fwc.rightWeaponID = characterInfo[2];

            //// Initialize
            //pbs.Initialize(character);
            //pbs.SetFrame(character);
            //fwc.SetLeftAbility(Instantiate(weapons[character.leftWeaponRef]));
            //fwc.SetRightAbility(Instantiate(weapons[character.rightWeaponRef]));

            return spawnPlayer;
            //return base.OnLobbyServerCreateGamePlayer(conn, playerControllerId);
        }

        public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
		{
			base.OnMatchCreate(success, extendedInfo, matchInfo);
            _currentMatchID = (System.UInt64)matchInfo.networkId;
		}

		public override void OnDestroyMatch(bool success, string extendedInfo)
		{
			base.OnDestroyMatch(success, extendedInfo);
			if (_disconnectServer)
            {
                StopMatchMaker();
                StopHost();
            }
        }

        //allow to handle the (+) button to add/remove player
        public void OnPlayersNumberModified(int count)
        {
            _playerNumber += count;

            int localPlayerCount = 0;
            foreach (UnityEngine.Networking.PlayerController p in ClientScene.localPlayers)
                localPlayerCount += (p == null || p.playerControllerId == -1) ? 0 : 1;

            addPlayerButton.SetActive(localPlayerCount < maxPlayersPerConnection && _playerNumber < maxPlayers);
        }

        // ----------------- Server callbacks ------------------

        //we want to disable the button JOIN if we don't have enough player
        //But OnLobbyClientConnect isn't called on hosting player. So we override the lobbyPlayer creation
        public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
        {
            string[] character = { "F1", "W1", "W1" };
            //Character character = characters[0];
            //character.leftWeaponRef = 0;
            //character.leftWeapon = weapons[character.leftWeaponRef];
            //character.rightWeaponRef = 0;
            //character.rightWeapon = weapons[character.rightWeaponRef];
            if (!currentPlayers.ContainsKey(conn.connectionId))
            {
                currentPlayers.Add(conn.connectionId, character);
            }

            GameObject obj = Instantiate(lobbyPlayerPrefab.gameObject) as GameObject;
            LobbyPlayer newPlayer = obj.GetComponent<LobbyPlayer>();
            newPlayer.ToggleJoinButton(numPlayers + 1 >= minPlayers);


            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

                if (p != null)
                {
                    p.RpcUpdateRemoveButton();
                    p.ToggleJoinButton(numPlayers + 1 >= minPlayers);
                }
            }

            return obj;
        }

        public void ServerSetCharacter(NetworkConnection conn, string characterID, string leftWeaponID, string rightWeaponID)
        {
            string[] character = { characterID, leftWeaponID, rightWeaponID };
            //Character character = characters[characterRef];
            //character.leftWeapon = weapons[leftWeaponRef];
            //character.leftWeaponRef = leftWeaponRef;
            //character.rightWeapon = weapons[rightWeaponRef];
            //character.rightWeaponRef = rightWeaponRef;

            if(currentPlayers.ContainsKey(conn.connectionId))
            {
                currentPlayers[conn.connectionId] = character;
            }
        }

        public override void OnLobbyServerPlayerRemoved(NetworkConnection conn, short playerControllerId)
        {
            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

                if (p != null)
                {
                    p.RpcUpdateRemoveButton();
                    p.ToggleJoinButton(numPlayers + 1 >= minPlayers);
                }
            }
        }

        public override void OnLobbyServerDisconnect(NetworkConnection conn)
        {
            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

                if (p != null)
                {
                    p.RpcUpdateRemoveButton();
                    p.ToggleJoinButton(numPlayers >= minPlayers);
                }
            }

        }

        public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
        {
            //This hook allows you to apply state data from the lobby-player to the game-player
            //just subclass "LobbyHook" and add it to the lobby object.

            if (_lobbyHooks)
                _lobbyHooks.OnLobbyServerSceneLoadedForPlayer(this, lobbyPlayer, gamePlayer);

            return true;
        }

        // --- Countdown management

        public override void OnLobbyServerPlayersReady()
        {
			bool allready = true;
			for(int i = 0; i < lobbySlots.Length; ++i)
			{
				if(lobbySlots[i] != null)
					allready &= lobbySlots[i].readyToBegin;
			}

			if(allready)
				StartCoroutine(ServerCountdownCoroutine());
        }

        public IEnumerator ServerCountdownCoroutine()
        {
            float remainingTime = prematchCountdown;
            int floorTime = Mathf.FloorToInt(remainingTime);

            while (remainingTime > 0)
            {
                yield return null;

                remainingTime -= Time.deltaTime;
                int newFloorTime = Mathf.FloorToInt(remainingTime);

                if (newFloorTime != floorTime)
                {//to avoid flooding the network of message, we only send a notice to client when the number of plain seconds change.
                    floorTime = newFloorTime;

                    for (int i = 0; i < lobbySlots.Length; ++i)
                    {
                        if (lobbySlots[i] != null)
                        {//there is maxPlayer slots, so some could be == null, need to test it before accessing!
                            (lobbySlots[i] as LobbyPlayer).RpcUpdateCountdown(floorTime);
                        }
                    }
                }
            }

            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                if (lobbySlots[i] != null)
                {
                    (lobbySlots[i] as LobbyPlayer).RpcUpdateCountdown(0);
                }
            }

            /*Character Select should be executed before ServerChangeScene
            Having Character Select Manager with countdown (Send signal to that script to let it start its work)
            When Character Select Get Confirm from both player send signal back to lobby manager to start the server
            Still don't clear about On hook but will manage somehow.
             */
            //ChangeTo(characterSelectPanel);

            MapSelector mapSelector = GetComponentInChildren<MapSelector>();

            if (mapSelector == null)
                ServerChangeScene(playScene);
            else
                ServerChangeScene(mapSelector.GetCurrentMap().mapScene.name);
        }

        // ----------------- Client callbacks ------------------

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            infoPanel.gameObject.SetActive(false);

            conn.RegisterHandler(MsgKicked, KickedMessageHandler);

            if (!NetworkServer.active)
            {//only to do on pure client (not self hosting client)
                ChangeTo(lobbyPanel);
                backDelegate = StopClientClbk;
                SetServerInfo("Client", networkAddress);
            }
        }


        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            ChangeTo(mainMenuPanel);
        }

        public override void OnClientError(NetworkConnection conn, int errorCode)
        {
            ChangeTo(mainMenuPanel);
            infoPanel.Display("Cient error : " + (errorCode == 6 ? "timeout" : errorCode.ToString()), "Close", null);
        }

    }
}
