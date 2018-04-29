using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Steamworks;

namespace Prototype.NetworkLobby
{
    //Player entry in the lobby. Handle selecting color/setting name & getting ready for the game
    //Any LobbyHook can then grab it and pass those value to the game player prefab (see the Pong Example in the Samples Scenes)
    public class LobbyPlayer : NetworkLobbyPlayer
    {
        static Color[] Colors = new Color[] { Color.magenta, Color.red, Color.cyan, Color.blue, Color.green, Color.yellow };
        //used on server to avoid assigning the same color to two player
        static List<int> _colorInUse = new List<int>();

        public Image avatarImage;
        public Button colorButton;
        public InputField nameInput;
        public Button readyButton;
        public Button waitingPlayerButton;
        public Button removePlayerButton;

        public GameObject localIcone;
        public GameObject remoteIcone;

        //OnMyName function will be invoked on clients when server change the value of playerName
        [SyncVar(hook = "OnMyName")]
        public string playerName = "";
        [SyncVar(hook = "OnMyColor")]
        public Color playerColor = Color.white;
        [SyncVar(hook = "OnSteamID")]
        public ulong steamID;

        [SyncVar]
        public string frameId = "F1";

        [SyncVar]
        public string leftWeaponId = "W1";

        [SyncVar]
        public string rightWeaponId = "W1";


        public Color OddRowColor = new Color(250.0f / 255.0f, 250.0f / 255.0f, 250.0f / 255.0f, 1.0f);
        public Color EvenRowColor = new Color(180.0f / 255.0f, 180.0f / 255.0f, 180.0f / 255.0f, 1.0f);

        static Color JoinColor = new Color(255.0f/255.0f, 0.0f, 101.0f/255.0f,1.0f);
        static Color NotReadyColor = new Color(34.0f / 255.0f, 44 / 255.0f, 55.0f / 255.0f, 1.0f);
        static Color ReadyColor = new Color(0.0f, 204.0f / 255.0f, 204.0f / 255.0f, 1.0f);
        static Color TransparentColor = new Color(0, 0, 0, 0);

        //static Color OddRowColor = new Color(250.0f / 255.0f, 250.0f / 255.0f, 250.0f / 255.0f, 1.0f);
        //static Color EvenRowColor = new Color(180.0f / 255.0f, 180.0f / 255.0f, 180.0f / 255.0f, 1.0f);

        public override void OnClientEnterLobby()
        {
            base.OnClientEnterLobby();

            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(1);

            LobbyPlayerList._instance.AddPlayer(this);
            LobbyPlayerList._instance.DisplayDirectServerWarning(isServer && LobbyManager.s_Singleton.matchMaker == null);

            if (isLocalPlayer)
            {
                SetupLocalPlayer();
            }
            else
            {
                SetupOtherPlayer();
            }

            //Reset local rotation of player
            transform.localRotation = Quaternion.identity;

            //setup the player data on UI. The value are SyncVar so the player
            //will be created with the right value currently on server
            OnMyName(playerName);
            OnMyColor(playerColor);
            OnSteamID(steamID);
            OnFrame(frameId);
            OnLeftWeapon(leftWeaponId);
            OnRightWeapon(rightWeaponId);
        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            //if we return from a game, color of text can still be the one for "Ready"
            readyButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;

           SetupLocalPlayer();
        }

        void ChangeReadyButtonColor(Color c)
        {
            ColorBlock b = readyButton.colors;
            b.normalColor = c;
            b.pressedColor = c;
            b.highlightedColor = c;
            b.disabledColor = c;
            readyButton.colors = b;
        }

        void SetupOtherPlayer()
        {
            nameInput.interactable = false;
            removePlayerButton.interactable = NetworkServer.active;

            ChangeReadyButtonColor(NotReadyColor);

            readyButton.transform.GetChild(0).GetComponent<Text>().text = "...";
            readyButton.interactable = false;

            OnClientReady(false);
        }

        void SetupLocalPlayer()
        {
            nameInput.interactable = true;
            remoteIcone.gameObject.SetActive(false);
            localIcone.gameObject.SetActive(true);

            CheckRemoveButton();

            if (playerColor == Color.white)
                CmdColorChange();

            ChangeReadyButtonColor(JoinColor);

            readyButton.transform.GetChild(0).GetComponent<Text>().text = "JOIN";
            readyButton.interactable = true;

            //have to use child count of player prefab already setup as "this.slot" is not set yet
            if (playerName == "")
            {
                CmdNameChanged("Player" + (LobbyPlayerList._instance.playerListContentTransform.childCount - 1));
                CmdSteamIDChanged(SteamUser.GetSteamID().m_SteamID);
            }

            //we switch from simple name display to name input
            colorButton.interactable = true;
            nameInput.interactable = true;

            nameInput.onEndEdit.RemoveAllListeners();
            nameInput.onEndEdit.AddListener(OnNameChanged);

            colorButton.onClick.RemoveAllListeners();
            colorButton.onClick.AddListener(OnColorClicked);

            readyButton.onClick.RemoveAllListeners();
            readyButton.onClick.AddListener(OnReadyClicked);

            //when OnClientEnterLobby is called, the loval PlayerController is not yet created, so we need to redo that here to disable
            //the add button if we reach maxLocalPlayer. We pass 0, as it was already counted on OnClientEnterLobby
            //StartCoroutine(FetchAvatar(new CSteamID(steamID)));

            if (LobbyManager.s_Singleton != null)
            {
                LobbyManager.s_Singleton.OnPlayersNumberModified(0);
                //LobbyManager.s_Singleton.characterPanel.GetComponent<CharacterSelector>().OnConfirmCharacter();
            }
        }

        //This enable/disable the remove button depending on if that is the only local player or not
        public void CheckRemoveButton()
        {
            if (!isLocalPlayer)
                return;

            int localPlayerCount = 0;
            foreach (UnityEngine.Networking.PlayerController p in ClientScene.localPlayers)
                localPlayerCount += (p == null || p.playerControllerId == -1) ? 0 : 1;

            removePlayerButton.interactable = localPlayerCount > 1;
        }

        public override void OnClientReady(bool readyState)
        {
            if (readyState)
            {
                ChangeReadyButtonColor(TransparentColor);

                Text textComponent = readyButton.transform.GetChild(0).GetComponent<Text>();
                textComponent.text = "READY";
                textComponent.color = ReadyColor;
                readyButton.interactable = false;
                colorButton.interactable = false;
                nameInput.interactable = false;
            }
            else
            {
                ChangeReadyButtonColor(isLocalPlayer ? JoinColor : NotReadyColor);

                Text textComponent = readyButton.transform.GetChild(0).GetComponent<Text>();
                textComponent.text = isLocalPlayer ? "JOIN" : "...";
                textComponent.color = Color.white;
                readyButton.interactable = isLocalPlayer;
                colorButton.interactable = isLocalPlayer;
                nameInput.interactable = isLocalPlayer;
            }
        }

        public void OnPlayerListChanged(int idx)
        { 
            GetComponent<Image>().color = (idx % 2 == 0) ? EvenRowColor : OddRowColor;
        }

        ///===== callback from sync var

        public void OnMyName(string newName)
        {
            playerName = newName;
            nameInput.text = playerName;
        }

        public void OnMyColor(Color newColor)
        {
            playerColor = newColor;
            colorButton.GetComponent<Image>().color = newColor;
        }

        public void OnSteamID(ulong newSteamID)
        {
            if (!SteamManager.Initialized)
                return;
            steamID = newSteamID;
            playerName = SteamFriends.GetFriendPersonaName(new CSteamID(steamID));
            OnMyName(playerName);
            StartCoroutine(FetchAvatar(new CSteamID(steamID)));
            //colorButton.GetComponent<Image>().color = newColor;
        }

        public void OnFrame(string newFrameId)
        {
            frameId = newFrameId;
        }

        public void OnLeftWeapon(string newWeaponId)
        {
            leftWeaponId = newWeaponId;
        }

        public void OnRightWeapon(string newWeaponId)
        {
            rightWeaponId = newWeaponId;
        }

        //===== UI Handler

        //Note that those handler use Command function, as we need to change the value on the server not locally
        //so that all client get the new value throught syncvar
        public void OnColorClicked()
        {
            CmdColorChange();
        }

        public void OnReadyClicked()
        {
            SendReadyToBeginMessage();
        }

        public void OnNameChanged(string str)
        {
            CmdNameChanged(str);
        }

        public void OnRemovePlayerClick()
        {
            if (isLocalPlayer)
            {
                RemovePlayer();
            }
            else if (isServer)
                LobbyManager.s_Singleton.KickPlayer(connectionToClient);
                
        }

        public void OnNextMapClicked()
        {
            CmdNextMapClicked();
        }

        public void OnPreviousMapClicked()
        {
            CmdPreviousMapClicked();
        }

        public void ToggleJoinButton(bool enabled)
        {
            readyButton.gameObject.SetActive(enabled);
            waitingPlayerButton.gameObject.SetActive(!enabled);
        }

        [ClientRpc]
        public void RpcUpdateCountdown(int countdown)
        {
            LobbyManager.s_Singleton.countdownPanel.UIText.text = "Match Starting in " + countdown;
            LobbyManager.s_Singleton.countdownPanel.gameObject.SetActive(countdown != 0);
        }

        [ClientRpc]
        public void RpcUpdateRemoveButton()
        {
            CheckRemoveButton();
        }

        //====== Server Command

        [Command]
        public void CmdColorChange()
        {
            int idx = System.Array.IndexOf(Colors, playerColor);

            int inUseIdx = _colorInUse.IndexOf(idx);

            if (idx < 0) idx = 0;

            idx = (idx + 1) % Colors.Length;

            bool alreadyInUse = false;

            do
            {
                alreadyInUse = false;
                for (int i = 0; i < _colorInUse.Count; ++i)
                {
                    if (_colorInUse[i] == idx)
                    {//that color is already in use
                        alreadyInUse = true;
                        idx = (idx + 1) % Colors.Length;
                    }
                }
            }
            while (alreadyInUse);

            if (inUseIdx >= 0)
            {//if we already add an entry in the colorTabs, we change it
                _colorInUse[inUseIdx] = idx;
            }
            else
            {//else we add it
                _colorInUse.Add(idx);
            }

            playerColor = Colors[idx];
        }

        [Command]
        public void CmdNameChanged(string name)
        {
            playerName = name;
        }

        [Command]
        public void CmdSteamIDChanged(ulong newSteamID)
        {
            steamID = newSteamID;
        }

        [Command]
        public void CmdNextMapClicked()
        {
            int currentMapIndex = GetComponentInParent<MapSelector>().NextMap();
            RpcSetCurrentMap(currentMapIndex);
        }

        [Command]
        public void CmdPreviousMapClicked()
        {
            int currentMapIndex = GetComponentInParent<MapSelector>().PreviousMap();
            RpcSetCurrentMap(currentMapIndex);
        }

        [ClientRpc]
        public void RpcSetCurrentMap(int mapIndex)
        {
            GetComponentInParent<MapSelector>().SetShownMap(mapIndex);
        }

        [ClientRpc]
        public void RpcSetCharacter(string characterID, string leftWeaponID, string rightWeaponID)
        {
            CmdSetCharacter(characterID, leftWeaponID, rightWeaponID);
        }

        [Command]
        public void CmdSetCharacter(string characterID, string leftWeaponID, string rightWeaponID)
        {
            Debug.Log("SET CHARACTER");
            LobbyManager.s_Singleton.ServerSetCharacter(GetComponent<NetworkIdentity>().connectionToClient, characterID, leftWeaponID, rightWeaponID);
            frameId = characterID;
            leftWeaponId = leftWeaponID;
            rightWeaponId = rightWeaponID;
        }

        //Cleanup thing when get destroy (which happen when client kick or disconnect)
        public void OnDestroy()
        {
            LobbyPlayerList._instance.RemovePlayer(this);
            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(-1);

            int idx = System.Array.IndexOf(Colors, playerColor);

            if (idx < 0)
                return;

            for (int i = 0; i < _colorInUse.Count; ++i)
            {
                if (_colorInUse[i] == idx)
                {//that color is already in use
                    _colorInUse.RemoveAt(i);
                    break;
                }
            }
        }

        public override bool Equals(object other)
        {
            if (other == null)
                return false;
            if (this.GetType() != other.GetType())
                return false;
            LobbyPlayer o = (LobbyPlayer) other;
            if (o.GetInstanceID() != this.GetInstanceID())
                return false;
            else return true;
            //return base.Equals(other);
        }

        private int avatarInt;
        private uint width, height;
        private Texture2D downloadedAvatar;
        private Rect rect = new Rect(0, 0, 184, 184);
        private Vector2 pivot = new Vector2(0.5f, 0.5f);
        private IEnumerator FetchAvatar(CSteamID steamID)
        {
            avatarInt = SteamFriends.GetLargeFriendAvatar(steamID);
            while (avatarInt == -1)
            {
                yield return null;
            }

            if (avatarInt > 0)
            {
                Debug.Log("FETCH FINISH");
                SteamUtils.GetImageSize(avatarInt, out width, out height);
                if (width > 0 && height > 0)
                {
                    byte[] avatarStream = new byte[4 * (int)width * (int)height];

                    SteamUtils.GetImageRGBA(avatarInt, avatarStream, 4 * (int)width * (int)height);

                    downloadedAvatar = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
                    downloadedAvatar.LoadRawTextureData(avatarStream);
                    downloadedAvatar.Apply();

                    avatarImage.sprite = Sprite.Create(downloadedAvatar, rect, pivot);
                }
            }
        }
    }
}
