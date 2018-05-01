using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UltimateAbility : ScriptableObject {

    public string uName = "New Ultimate";
    public string uID;
    //public Sprite aSprite;
    //public AudioClip aSound;
    public float duration;
    public float maxCharge;

    public abstract void Initialize(GameObject obj);
    public abstract void TriggerAbility();
    public abstract void TriggerAbilityEnd();
}
