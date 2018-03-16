using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Abilities/ProjectileAbility")]
public class ProjectileAbility : WeaponAbility
{
    public float projectileForce;
    public Projectile projectile;
    public float reloadTime;

    private ProjectileShootTriggerable gun;

    public override void Initialize(GameObject obj)
    {
        gun = obj.GetComponent<ProjectileShootTriggerable>();
        gun.projectileForce = projectileForce;
        gun.projectile = projectile;
        gun.magazine = aMagazine;
        gun.gunSound = aGunSound;
        gun.reloadTime = reloadTime;
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
