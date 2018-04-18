using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UniqueAbility : ScriptableObject {

    public string aName = "New Unique Ability";
    public string aID;
    public AudioClip aSound;
    public float triggerDelay;


    public abstract void Initialize(GameObject obj);

    public abstract void TriggerAbility();

    public abstract void DeTriggerAbility();

    public abstract void TriggerReload();

}
