using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Projectile")]
public class Projectile : ScriptableObject {
    public string projectileId;

    public float damage;
    public float force;
    public float lifeTime;

    public GameObject projectilePrefab;
}
