using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Abilities/RocketAbility")]
public class RocketAbility : WeaponAbility {

    public BlastRound rocket;
    public float range;
    public float reloadDelay;

    private RocketShootTriggerable rocketLuncher;

    public override void Initialize(GameObject obj)
    {
        rocketLuncher = obj.GetComponent<RocketShootTriggerable>();
        rocketLuncher.gunId = aID;
        rocketLuncher.rocket = rocket;
        rocketLuncher.range = range;
        rocketLuncher.cooldown = reloadDelay;
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
