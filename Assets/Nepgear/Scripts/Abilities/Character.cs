using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character")]
public class Character : ScriptableObject
{

    public string characterName = "Default";
    public float maxHitPoint = 100f;
    public float maxStamina = 100f;
    public float maxUltimate = 100f;
    public float startingSpeed = 5f;

    public UltimateAbility ultimate;
    [HideInInspector] public WeaponAbility leftWeapon;
    [HideInInspector] public int leftWeaponRef;
    //[HideInInspector] public GameObject leftWeaponPrefab;
    [HideInInspector] public WeaponAbility rightWeapon;
    [HideInInspector] public int rightWeaponRef;
    //[HideInInspector] public GameObject rightWeaponPrefab;


}