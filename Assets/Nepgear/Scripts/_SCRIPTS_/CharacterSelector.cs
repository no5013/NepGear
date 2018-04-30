using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace Prototype.NetworkLobby
{
    public class CharacterSelector : MonoBehaviour
    {

        //public LobbyManager lobbyManager;

        public GameObject player;
        [SerializeField] private Character[] characters;
        [SerializeField] private WeaponAbility[] weapons;
        [SerializeField] private GameObject[] weaponsPrefab;
        public Vector3 playerSpawnPosition = new Vector3(1, 1, 1);

        public GameObject characterSelectPanel;

        private int selectedCharacterRef;
        private int selectedLeftWeaponRef;
        private int selectedRightWeaponRef;

        private int currentSelectedCharacter = 0;
        private int currentSelectedLeft = 0;
        private int currentSelectedRight = 0;

        public LobbyPlayer lobbyPlayer;

        public RectTransform lobbyPanel;

        public Text characterText;
        public Text leftWeaponText;
        public Text rightWeaponText;
        private Dropdown[] dropdowns;

        public MechBase mechBase;

        [SerializeField]
        private ResourcesManager resourcesManager;

        void Start()
        {
            Initialize();
        }

        public void OnCharacterSelect(int characterChoice)
        {
            if(characterChoice > 0)
            {
                selectedCharacterRef++;
                if (selectedCharacterRef >= characters.Length)
                {
                    selectedCharacterRef = 0;
                }
            }
            else
            {
                selectedCharacterRef--;
                if(selectedCharacterRef < 0)
                {
                    selectedCharacterRef = characters.Length - 1;
                }
            }
            characterText.text = characters[selectedCharacterRef].characterName;
            //selectedCharacter = characters[characterChoice];

            OnChangeSetting();
        }

        public void OnLeftHandWeaponSelect(int weaponChoice)
        {
            if(weaponChoice > 0)
            {
                selectedLeftWeaponRef++;
                if (selectedLeftWeaponRef >= weapons.Length)
                {
                    selectedLeftWeaponRef = 0;
                }

            }
            else
            {
                selectedLeftWeaponRef--;
                if(selectedLeftWeaponRef < 0)
                {
                    selectedLeftWeaponRef = weapons.Length - 1;
                }
            }
            leftWeaponText.text = weapons[selectedLeftWeaponRef].aName;
            //selectedCharacter.leftWeapon = weapons[weaponChoice];

            //selectedCharacter.leftWeaponPrefab = weaponsPrefab[weaponChoice];

            OnChangeSetting();
        }

        public void OnRightHandWeaponSelect(int weaponChoice)
        {
            if (weaponChoice > 0)
            {
                selectedRightWeaponRef++;
                if (selectedRightWeaponRef >= weapons.Length)
                {
                    selectedRightWeaponRef = 0;
                }
            }
            
            else
            {
                selectedRightWeaponRef--;
                if (selectedRightWeaponRef < 0)
                {
                    selectedRightWeaponRef = weapons.Length - 1;
                }
            }
            rightWeaponText.text = weapons[selectedRightWeaponRef].aName;

            OnChangeSetting();
        }

        public void OnChangeSetting()
        {
            if (mechBase != null)
            {
                mechBase.Initialize(characters[selectedCharacterRef].characterID, weapons[selectedLeftWeaponRef].aID, weapons[selectedRightWeaponRef].aID);
            }
        }

        public void OnCancelSelect()
        {
            if(lobbyPlayer != null)
            {
                mechBase.Initialize(lobbyPlayer.frameId, lobbyPlayer.leftWeaponId, lobbyPlayer.rightWeaponId);
            }
            else
            {
                selectedCharacterRef = currentSelectedCharacter;
                selectedLeftWeaponRef = currentSelectedLeft;
                selectedRightWeaponRef = currentSelectedRight;
                OnChangeSetting();
            }


            LobbyManager.s_Singleton.ChangeTo(lobbyPanel);
        }

        public void OnConfirmCharacter()
        {
            currentSelectedCharacter = selectedCharacterRef;
            currentSelectedLeft = selectedLeftWeaponRef;
            currentSelectedRight = selectedRightWeaponRef;

            OnChangeSetting();

            if(LobbyPlayerList._instance == null)
            {
                return;
            }

            lobbyPlayer = LobbyPlayerList._instance.FindLocalPlayer();

            if (lobbyPlayer != null)
            {
                if (lobbyPlayer.isServer)
                {
                    lobbyPlayer.RpcSetCharacter(characters[selectedCharacterRef].characterID, weapons[selectedLeftWeaponRef].aID, weapons[selectedRightWeaponRef].aID);
                }
                else
                {
                    lobbyPlayer.CmdSetCharacter(characters[selectedCharacterRef].characterID, weapons[selectedLeftWeaponRef].aID, weapons[selectedRightWeaponRef].aID);
                }
                LobbyManager.s_Singleton.ChangeTo(lobbyPanel);
            }
        }

        public void OnConfirmCharacter(LobbyPlayer lobbyPlayer)
        {
            currentSelectedCharacter = selectedCharacterRef;
            currentSelectedLeft = selectedLeftWeaponRef;
            currentSelectedRight = selectedRightWeaponRef;

            OnChangeSetting();
       
                if (lobbyPlayer.isServer)
                {
                    lobbyPlayer.RpcSetCharacter(characters[selectedCharacterRef].characterID, weapons[selectedLeftWeaponRef].aID, weapons[selectedRightWeaponRef].aID);
                }
                else
                {
                    lobbyPlayer.CmdSetCharacter(characters[selectedCharacterRef].characterID, weapons[selectedLeftWeaponRef].aID, weapons[selectedRightWeaponRef].aID);
                }

            LobbyManager.s_Singleton.ChangeTo(lobbyPanel);
        }

        private void Initialize()
        {
            resourcesManager.Init();

            characters = resourcesManager.frames;
            weapons = resourcesManager.weaponAbilities;

            dropdowns = GetComponentsInChildren<Dropdown>();
            for (int i = 0; i < dropdowns.Length; i++)
            {
                dropdowns[i].ClearOptions();
                List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
                Dropdown.OptionData option = new Dropdown.OptionData();
                if (i == 0)
                {
                    foreach (Character character in characters)
                    {
                        option.text = character.characterName;
                        options.Add(option);
                    }
                }
                else
                {
                    foreach (WeaponAbility weapon in weapons)
                    {
                        option.text = weapon.aName;
                        options.Add(option);
                    }
                }
                dropdowns[i].AddOptions(options);
            }

            selectedCharacterRef = 0;
            selectedLeftWeaponRef = 0;
            selectedRightWeaponRef = 0;


            characterText.text = characters[selectedCharacterRef].characterName;
            leftWeaponText.text = weapons[selectedLeftWeaponRef].aName;
            rightWeaponText.text = weapons[selectedRightWeaponRef].aName;
            //Text[] texts = GetComponentsInChildren<Text>();
            //texts[0].text = characters[selectedCharacterRef].characterName;
            //texts[1].text = selectedCharacter.leftWeapon.aName;
            //texts[2].text = selectedCharacter.rightWeapon.aName;

        }
    }

}