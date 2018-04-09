using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechBase : MonoBehaviour {

    public Transform spawnPosition;

    public float speed = 10f;

    public string mechId = "F1";
    public string leftWeaponId = "shotgunLvl1";
    public string rightWeaponId = "shotgunLvl1";

    private GameObject mech;

    public ResourcesManager resourcesManager;

    private void Awake()
    {
        resourcesManager.Init();
    }

    // Use this for initialization
    void Start () {
        Initialize(resourcesManager.frames[0].characterID, resourcesManager.weaponAbilities[0].aID, resourcesManager.weaponAbilities[0].aID); 
    }

    public void Initialize(string mechId, string leftWeaponId, string rightWeaponId)
    {
        InitializeMech(mechId);
        EquipLeftWeaponToMech(leftWeaponId);
        EquipRightWeaponToMech(rightWeaponId);

        ResetPosition();
    }

    public void InitializeMech(string mechId)
    {
        Character initializeMech = resourcesManager.GetCharacter(mechId);

        if (initializeMech != null)
        {
            Destroy(mech);

            mech = Instantiate(initializeMech.characterPrefab);
            mech.transform.parent = transform;
            mech.transform.position = spawnPosition.position;
            mech.transform.rotation = spawnPosition.rotation;

            EquipLeftWeaponToMech(leftWeaponId);
            EquipRightWeaponToMech(rightWeaponId);
        }
    }

    public void EquipLeftWeaponToMech(string weaponId)
    {
        WeaponAbility weaponAbility = resourcesManager.GetWeapon(weaponId);
        if(weaponAbility != null)
        {
            FrameManager fm = mech.GetComponent<FrameManager>();
            fm.EquipLeftWeapon(weaponAbility);
        }        
    }

    public void EquipRightWeaponToMech(string weaponId)
    {
        WeaponAbility weaponAbility = resourcesManager.GetWeapon(weaponId);
        if (weaponAbility != null)
        {
            FrameManager fm = mech.GetComponent<FrameManager>();
            fm.EquipRightWeapon(weaponAbility);
        }
    }

    void ResetPosition()
    {
        transform.rotation = Quaternion.identity;
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
