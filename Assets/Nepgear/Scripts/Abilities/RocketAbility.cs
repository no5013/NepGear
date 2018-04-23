using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Abilities/UniqueAbilities/RocketAbility")]
public class RocketAbility : UniqueAbility {

    public BlastRound rocket;
    public float range;
    public float reloadDelay;

    private RocketShootTriggerable rocketLuncher;

    public override void DeTriggerAbility()
    {
        return;
    }

    public override void Initialize(GameObject obj)
    {
        Debug.Log(obj.ToString());
        rocketLuncher = obj.GetComponentInChildren<RocketShootTriggerable>();
        rocketLuncher.gunId = aID;
        rocketLuncher.rocket = rocket;
        rocketLuncher.range = range;
        rocketLuncher.cooldown = reloadDelay;
        rocketLuncher.rocketSound = aSound;
    }

    public override void TriggerAbility()
    {
        rocketLuncher.Fire();
    }

    public override void TriggerReload()
    {
        return;
    }
}
