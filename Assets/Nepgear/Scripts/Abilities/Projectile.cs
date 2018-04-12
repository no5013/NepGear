using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Projectile/NormalRound")]
public class Projectile : ScriptableObject {
    public string projectileId;

    public float damage;
    public float force;
    public float lifeTime;
    public float speed;

    public GameObject projectilePrefab;
}
