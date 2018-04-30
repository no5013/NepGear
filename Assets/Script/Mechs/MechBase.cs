using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.NetworkLobby;

public class MechBase : MonoBehaviour {

    public Transform spawnPosition;

    public float speed = 0f;

    public string mechId = "F1";
    public string leftWeaponId = "shotgunLvl1";
    public string rightWeaponId = "shotgunLvl1";

    private GameObject mech;

    public ResourcesManager resourcesManager;

    // Use this for initialization
    void Start () {
        resourcesManager.Init();
        Initialize(resourcesManager.frames[0].characterID, resourcesManager.weaponAbilities[0].aID, resourcesManager.weaponAbilities[0].aID);

        FindObjectOfType<LobbyManager>().characterPanel.GetComponent<CharacterSelector>().mechBase = this;
        FindObjectOfType<LobbyManager>().characterPanel.GetComponent<CharacterSelector>().OnChangeSetting();
    }

    public void Initialize(string mechId, string leftWeaponId, string rightWeaponId)
    {
        InitializeMech(mechId);
        EquipLeftWeaponToMech(leftWeaponId);
        EquipRightWeaponToMech(rightWeaponId);

        //ResetPosition();
    }

    public void InitializeMech(string mechId)
    {
        if (this.mechId.Equals(mechId))
            return;

        Character initializeMech = resourcesManager.GetCharacter(mechId);

        if (initializeMech != null)
        {
            Destroy(mech);

            mech = Instantiate(initializeMech.framePrefab);
            mech.transform.parent = transform;
            mech.transform.position = spawnPosition.position;
            mech.transform.rotation = spawnPosition.rotation;
            this.mechId = mechId;

            leftWeaponId = "";
            rightWeaponId = "";
            EquipLeftWeaponToMech(leftWeaponId);
            EquipRightWeaponToMech(rightWeaponId);
        }
    }

    public void EquipLeftWeaponToMech(string weaponId)
    {
        if (leftWeaponId.Equals(weaponId))
            return;

        WeaponAbility weaponAbility = resourcesManager.GetWeapon(weaponId);
        if(weaponAbility != null)
        {
            FrameManager fm = mech.GetComponent<FrameManager>();
            fm.EquipLeftWeapon(weaponAbility);
            leftWeaponId = weaponId;
        }        
    }

    public void EquipRightWeaponToMech(string weaponId)
    {
        if (rightWeaponId.Equals(weaponId))
            return;

        WeaponAbility weaponAbility = resourcesManager.GetWeapon(weaponId);
        if (weaponAbility != null)
        {
            FrameManager fm = mech.GetComponent<FrameManager>();
            fm.EquipRightWeapon(weaponAbility);
            rightWeaponId = weaponId;
        }
    }

    void ResetPosition()
    {
        //transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update () {
        transform.Rotate(Vector3.up, speed * Time.deltaTime);

        //Test only
        /*if (input != mechId)
        {
            InitializeMech(input);
            mechId = input;
        }

        if (inputL != leftWeaponId)
        {
            EquipLeftWeaponToMech(inputL);
            leftWeaponId = inputL;
        }

        if (inputR != rightWeaponId)
        {
            EquipRightWeaponToMech(inputR);
            rightWeaponId = inputR;
        }*/
    }
}
