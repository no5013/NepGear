using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Abilities/WeaponAbilities/ProjectileAbility")]
public class ProjectileAbility : WeaponAbility
{
    public float projectileForce;
    public Projectile projectile;
    public float reloadTime;

    public bool isSpread;
    public int palletCount;
    public float spreadFactor;

    public bool isChargable;
    public float chargeRate;
    public float maxCharge;

    private ProjectileShootTriggerable gun;

    public override void Initialize(GameObject obj)
    {
        gun = obj.GetComponent<ProjectileShootTriggerable>();
        gun.projectileForce = projectileForce;
        gun.projectile = projectile;
        gun.magazine = aMagazine;
        gun.gunSound = aGunSound;
        gun.reloadSound = aReloadSound;
        gun.reloadTime = reloadTime;
        gun.gunId = aID;
        gun.maxRecoil = maxRecoil;
        gun.recoilRate = recoilRate;
        gun.isChargable = isChargable;
        if (isChargable)
        {
            gun.chargeRate = chargeRate;
            gun.maxCharge = maxCharge;
        }
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
