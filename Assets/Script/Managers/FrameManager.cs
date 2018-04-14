using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameManager : MonoBehaviour {

    private Transform mountGadgetL;
    private Transform mountGadgetR;

    private GameObject leftWeapon;
    private GameObject rightWeapon;

    private Animator animator;

    public string leftWeaponId;
    public string rightWeaponId;

    // Use this for initialization

    private void Awake()
    {
        mountGadgetL = GetComponentInChildren<LeftController>().transform;
        mountGadgetR = GetComponentInChildren<RightController>().transform;

        animator = GetComponent<Animator>();

        /*EquipWeaponLeft(leftWeaponId);
        EquipWeaponRight(rightWeaponId);*/
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

    public void EquipLeftWeapon(WeaponAbility weaponAbility)
    {
        Destroy(leftWeapon);
        if (weaponAbility != null)
        {
            animator.SetTrigger("Equip");
            leftWeapon = Instantiate(weaponAbility.gunPrefab, mountGadgetL);
        }
    }

    public void EquipRightWeapon(WeaponAbility weaponAbility)
    {
        Destroy(rightWeapon);
        if (weaponAbility != null)
        {
            animator.SetTrigger("Equip");
            rightWeapon = Instantiate(weaponAbility.gunPrefab, mountGadgetR);
        }
    }
}
