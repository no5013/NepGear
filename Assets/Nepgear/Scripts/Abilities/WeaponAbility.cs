using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponAbility : ScriptableObject {

    public string aName = "New Ability";
    public string aID;
    //public Sprite aSprite;
    public AudioClip aGunSound;
    public AudioClip aReloadSound;
    public int aMagazine;
    public float aFireDelay;
    public float maxRecoil;
    public float recoilRate;

    public GameObject gunPrefab;
    public GameObject muzzleFlash;

    public abstract void Initialize(GameObject obj);
    public abstract void TriggerAbility();
    public abstract void TriggerReload();
}