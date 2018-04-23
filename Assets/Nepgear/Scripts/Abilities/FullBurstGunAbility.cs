using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/WeaponAbilities/FullBurstAbility")]
public class FullBurstGunAbility : WeaponAbility {


    public Projectile projectile;

    private FullBurstShootTriggerable gun;

    public override void Initialize(GameObject obj)
    {
        gun = obj.GetComponent<FullBurstShootTriggerable>();
        gun.projectile = projectile;
        gun.gunSound = aGunSound;
        gun.Initialize();
    }

    public override void TriggerAbility()
    {
        return;
    }

    public override void TriggerReload()
    {
        return;
    }
}
