using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponAbility : ScriptableObject {

    public string aName = "New Ability";
    //public Sprite aSprite;
    public AudioClip aGunSound;
    public int aMagazine;
    public float aFireDelay;

    public abstract void Initialize(GameObject obj);
    public abstract void TriggerAbility();
    public abstract void TriggerReload();
}