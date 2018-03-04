using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShootTriggerable : MonoBehaviour {

    [HideInInspector] public Rigidbody projectile;
    public Transform bulletSpawn;
    [HideInInspector] public float projectileForce = 250f;

    public void Fire()
    {
        Rigidbody cloneBullet = Instantiate(projectile, bulletSpawn.position, transform.rotation) as Rigidbody;

        cloneBullet.AddForce(bulletSpawn.transform.forward * projectileForce);
    }
}
