using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character")]
public class Character : ScriptableObject
{

    public string characterName = "Default";
    public string characterID;
    public float maxHitPoint = 100f;
    public float maxStamina = 100f;
    public float startingSpeed = 5f;
    //public float runningSpeed;
    public float jumpForce;
    public float boostSpeed;
    public float dodgeForce;
    public float mass;
    public float staggerLimit;
    public float staggerRecovery;
    public GameObject characterPrefab;

    public GameObject framePrefab;

    public UltimateAbility ultimate;
    public UniqueAbility uniqueWeaponAbility;

    [HideInInspector] public WeaponAbility leftWeapon;
    [HideInInspector] public string leftWeaponID;
    //[HideInInspector] public GameObject leftWeaponPrefab;
    [HideInInspector] public WeaponAbility rightWeapon;
    [HideInInspector] public string rightWeaponID;
    //[HideInInspector] public GameObject rightWeaponPrefab;


}
