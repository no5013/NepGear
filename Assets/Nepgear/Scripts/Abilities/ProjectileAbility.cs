using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Abilities/ProjectileAbility")]
public class ProjectileAbility : Ability
{
    public float projectileForce;
    public Rigidbody projectile;

    private ProjectileShootTriggerable gun;

    public override void Initialize(GameObject obj)
    {
        gun = obj.GetComponent<ProjectileShootTriggerable>();
        gun.projectileForce = projectileForce;
        gun.projectile = projectile;
    }

    public override void TriggerAbility()
    {
        gun.Fire();
    }
}
