using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CharacterSelector : MonoBehaviour {

    public GameObject player;
    public Character[] characters;
    public Ability[] weapons;
    public GameObject[] weaponsPrefab;
    public Vector3 playerSpawnPosition = new Vector3(1, 1, 1);

    public GameObject characterSelectPanel;

    private Character selectedCharacter;
    private Dropdown[] dropdowns;

    void Start()
    {
        Initialize();
    }

    public void OnCharacterSelect(int characterChoice)
    {
        selectedCharacter = characters[characterChoice];
        //GameObject spawnPlayer = Instantiate(player, playerSpawnPosition, Quaternion.identity) as GameObject;
        //FrameWeapon frameWeapon = spawnPlayer.GetComponentInChildren<FrameWeapon>();

        //PlayerBehaviorScript pbs = player.GetComponent<PlayerBehaviorScript>();
        //pbs.Initialize(selectedCharacter);
    }

    public void OnLeftHandWeaponSelect(int weaponChoice)
    {
        if (!selectedCharacter)
        {
            return;
        }
        selectedCharacter.leftWeapon = weapons[weaponChoice];
        selectedCharacter.leftWeaponPrefab = weaponsPrefab[weaponChoice];
    }

    public void OnRightHandWeaponSelect(int weaponChoice)
    {
        if (!selectedCharacter)
        {
            return;
        }
        selectedCharacter.rightWeapon = weapons[weaponChoice];
        selectedCharacter.rightWeaponPrefab = weaponsPrefab[weaponChoice];
    }

    public void OnConfirmCharacter()
    {
        if(!selectedCharacter || !selectedCharacter.leftWeapon || !selectedCharacter.rightWeapon)
        {
            return;
        }
        GameObject leftWeapon = Instantiate(selectedCharacter.leftWeaponPrefab);
        GameObject rightWeapon = Instantiate(selectedCharacter.rightWeaponPrefab);
        GameObject spawnPlayer = Instantiate(player, playerSpawnPosition, Quaternion.identity) as GameObject;

        leftWeapon.transform.parent = spawnPlayer.GetComponentInChildren<Camera>().transform;
        rightWeapon.transform.parent = spawnPlayer.GetComponentInChildren<Camera>().transform;
        leftWeapon.transform.localPosition = new Vector3(-0.185f, -0.04f, 0.2f);
        rightWeapon.transform.localPosition = new Vector3(0.185f, -0.04f, 0.2f);

        FrameWeapon frameWeapon = spawnPlayer.GetComponentInChildren<FrameWeapon>();
        PlayerBehaviorScript pbs = spawnPlayer.GetComponent<PlayerBehaviorScript>();
        FrameWeaponController fwc = spawnPlayer.GetComponent<FrameWeaponController>();

        pbs.Initialize(selectedCharacter);
        fwc.Initialize(Instantiate(selectedCharacter.leftWeapon), leftWeapon, Instantiate(selectedCharacter.rightWeapon), rightWeapon);
        

        characterSelectPanel.SetActive(false);
    }

    private void Initialize()
    {
        dropdowns = characterSelectPanel.GetComponentsInChildren<Dropdown>();
        for (int i = 0; i < dropdowns.Length; i++)
        {
            dropdowns[i].ClearOptions();
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            if (i == 0)
            {
                foreach (Character character in characters)
                {
                    Dropdown.OptionData option = new Dropdown.OptionData();
                    option.text = character.characterName;
                    options.Add(option);
                }
            }
            else
            {
                foreach (Ability weapon in weapons)
                {
                    Dropdown.OptionData option = new Dropdown.OptionData();
                    option.text = weapon.aName;
                    options.Add(option);
                }
            }
            dropdowns[i].AddOptions(options);
        }
        selectedCharacter = characters[0];
        selectedCharacter.leftWeapon = weapons[0];
        selectedCharacter.leftWeaponPrefab = weaponsPrefab[0];
        selectedCharacter.rightWeapon = weapons[0];
        selectedCharacter.rightWeaponPrefab = weaponsPrefab[0];
    }
}
