using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Abilities/RaycastAbility")]
public class RaycastAbility : WeaponAbility {

    public float damage;
    public float range;
    public float reloadTime;
    public float force;

    private RaycastShootTriggerable gun;

    public override void Initialize(GameObject obj)
    {
        gun = obj.GetComponent<RaycastShootTriggerable>();
        gun.gunId = aID;
        gun.damage = damage;
        gun.range = range;
        gun.magazine = aMagazine;
        gun.gunSound = aGunSound;
        gun.reloadTime = reloadTime;
        gun.force = force;
        gun.maxRecoil = maxRecoil;
        gun.recoilRate = recoilRate;
        gun.Initialize();
    }

    public override void TriggerAbility()
    {
        gun.Fire();
    }

    public override void TriggerReload()
    {
        gun.Reload();
    }

}
