using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Ultimates/OverclockAbility")]
public class OverclockAbility : UltimateAbility {
    
    private PlayerBehaviorScript frame;

    public override void Initialize(GameObject obj)
    {
        frame = obj.GetComponent<PlayerBehaviorScript>();
    }

    public override void TriggerAbility()
    {
        frame.hitPoint *= 2;
        frame.maxHitPoint *= 2;
        frame.stamina *= 2;
        frame.maxStamina *= 2;
    }

    public override void TriggerAbilityEnd()
    {
        frame.hitPoint /= 2;
        frame.maxHitPoint /= 2;
        frame.stamina /= 2;
        frame.maxStamina /= 2;
    }
}
