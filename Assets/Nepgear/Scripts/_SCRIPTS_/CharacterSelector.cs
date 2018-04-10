using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace Prototype.NetworkLobby
{
    public class CharacterSelector : NetworkBehaviour
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

        public LobbyPlayer lobbyPlayer;

        public RectTransform lobbyPanel;

        public Text characterText;
        public Text leftWeaponText;
        public Text rightWeaponText;
        private Dropdown[] dropdowns;

        public MechBase mechBase;

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

        private void OnChangeSetting()
        {
            if (mechBase != null)
            {
                mechBase.Initialize(characters[selectedCharacterRef].characterID, weapons[selectedLeftWeaponRef].aID, weapons[selectedRightWeaponRef].aID);
            }
        }

        public void OnCancelSelect()
        {
            mechBase.Initialize(lobbyPlayer.frameId, lobbyPlayer.leftWeaponId, lobbyPlayer.rightWeaponId);

            LobbyManager.s_Singleton.ChangeTo(lobbyPanel);
        }

        public void OnConfirmCharacter()
        {
            OnChangeSetting();
            if(isServer)
            {
                lobbyPlayer.RpcSetCharacter(characters[selectedCharacterRef].characterID, weapons[selectedLeftWeaponRef].aID, weapons[selectedRightWeaponRef].aID);
            }
            else
            {
                lobbyPlayer.CmdSetCharacter(characters[selectedCharacterRef].characterID, weapons[selectedLeftWeaponRef].aID, weapons[selectedRightWeaponRef].aID);
            }
            //// Instantiate Gun Object and Player Object
            //GameObject leftWeapon = Instantiate(selectedCharacter.leftWeaponPrefab);
            //GameObject rightWeapon = Instantiate(selectedCharacter.rightWeaponPrefab);
            //GameObject spawnPlayer = Instantiate(player, playerSpawnPosition, Quaternion.identity) as GameObject;

            //// Set Child to Camera
            //leftWeapon.transform.parent = spawnPlayer.GetComponentInChildren<Camera>().transform;
            //rightWeapon.transform.parent = spawnPlayer.GetComponentInChildren<Camera>().transform;

            //// VR
            //leftWeapon.transform.parent = spawnPlayer.GetComponentInChildren<LeftController>().transform;
            //rightWeapon.transform.parent = spawnPlayer.GetComponentInChildren<RightController>().transform;

            //// Set Local Position
            //leftWeapon.transform.localPosition = new Vector3(-0.185f, -0.04f, 0.2f);
            //rightWeapon.transform.localPosition = new Vector3(0.185f, -0.04f, 0.2f);

            //// Get Reference
            //FrameWeapon frameWeapon = spawnPlayer.GetComponentInChildren<FrameWeapon>();
            //PlayerBehaviorScript pbs = spawnPlayer.GetComponent<PlayerBehaviorScript>();
            //FrameWeaponController fwc = spawnPlayer.GetComponent<FrameWeaponController>();

            //// Initialize
            //pbs.Initialize(selectedCharacter);
            //fwc.Initialize(Instantiate(selectedCharacter.leftWeapon), leftWeapon, Instantiate(selectedCharacter.rightWeapon), rightWeapon);

            //characterSelectPanel.SetActive(false);
            //if (isServer)
            //    lobbyManager.RpcSetCharacter(selectedCharacter);
            //else
            //    lobbyManager.CmdSetCharacter(selectedCharacter);
            LobbyManager.s_Singleton.ChangeTo(lobbyPanel);
        }

        private void Initialize()
        {
            characters = LobbyManager.s_Singleton.resourcesManager.frames;
            weapons = LobbyManager.s_Singleton.resourcesManager.weaponAbilities;

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