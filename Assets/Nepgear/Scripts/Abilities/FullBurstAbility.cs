using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Ultimates/FullBurstAbility")]
public class FullBurstAbility : UltimateAbility {

    public FullBurstGunAbility fullBurstProjectileAbility;
    
    public float fireDelay;

    private UltimateControllerScript ucs;

    public override void Initialize(GameObject obj)
    {
        FullBurstShootTriggerable[] additionalGuns = obj.GetComponentsInChildren<FullBurstShootTriggerable>();
        foreach(FullBurstShootTriggerable gun in additionalGuns)
        {
            fullBurstProjectileAbility.Initialize(gun.gameObject);
        }
        ucs = obj.GetComponent<UltimateControllerScript>();
    }

    public override void TriggerAbility()
    {
        ucs.FullBurst(fireDelay);
    }

    public override void TriggerAbilityEnd()
    {
        ucs.StopFullBurst();
    }


}
