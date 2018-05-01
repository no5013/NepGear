using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Projectile/BlastRound")]
public class BlastRound : Projectile {

    public float blastForce;
    public float blastRadius;
    public float blastDamage;
    public float timeBeforeDestroy;
    public float risingSpeed = 0;
}
