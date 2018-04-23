using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ultimates/SatelliteCannonAbility")]
public class SatelliteCannonAbility : UltimateAbility {

    public Projectile projectile;
    public float fireDelay;
    public float startHeight;
   
    private UltimateControllerScript ucs;

    public override void Initialize(GameObject obj)
    {
        ucs = obj.GetComponent<UltimateControllerScript>();
    }

    public override void TriggerAbility()
    {
        ucs.SatelliteCannon(startHeight, fireDelay);        
    }

    public override void TriggerAbilityEnd()
    {
        ucs.StopSatelliteCannon();
    }
}
