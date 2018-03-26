using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameManager : MonoBehaviour {

    private Transform mountGadgetL;
    private Transform mountGadgetR;

    private GameObject leftWeapon;
    private GameObject rightWeapon;

    public string leftWeaponId;
    public string rightWeaponId;

    public string inputL;
    public string inputR;

    private Animator animator;

    // Use this for initialization
    void Start () {
        mountGadgetL = GetComponentInChildren<MountGadgetL>().transform;
        mountGadgetR = GetComponentInChildren<MountGadgetR>().transform;
        animator = GetComponent<Animator>();

        EquipWeaponLeft(leftWeaponId);
        EquipWeaponRight(rightWeaponId);
    }
	
	// Update is called once per frame
	void Update () {
		if(inputL != leftWeaponId)
        {
            EquipWeaponLeft(inputL);
            leftWeaponId = inputL;
        }

        if (inputR != rightWeaponId)
        {
            EquipWeaponRight(inputR);
            rightWeaponId = inputR;
        }
    }

    void EquipWeaponLeft(string weaponId)
    {
        Destroy(leftWeapon);
        WeaponAbility leftWeaponAbility = Prototype.NetworkLobby.LobbyManager.s_Singleton.resourcesManager.GetWeapon(weaponId);
        if (leftWeaponAbility != null)
        {
            animator.SetTrigger("Equip");
            leftWeapon = Instantiate(leftWeaponAbility.gunPrefab, mountGadgetL);
        }
    }

    void EquipWeaponRight(string weaponId)
    {
        Destroy(rightWeapon);
        WeaponAbility rightWeaponAbility = Prototype.NetworkLobby.LobbyManager.s_Singleton.resourcesManager.GetWeapon(weaponId);
        if (rightWeaponAbility != null)
        {
            animator.SetTrigger("Equip");
            rightWeapon = Instantiate(rightWeaponAbility.gunPrefab, mountGadgetR);
        }
    }
}
