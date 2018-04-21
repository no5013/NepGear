using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Ultimates/OverclockAbility")]
public class OverclockAbility : UltimateAbility {

    public float multiplier;

    private UltimateControllerScript ultimateController;

    public override void Initialize(GameObject obj)
    {
        ultimateController = obj.GetComponent<UltimateControllerScript>();
    }

    public override void TriggerAbility()
    {
        ultimateController.Overclock(multiplier);
    }

    public override void TriggerAbilityEnd()
    {
        ultimateController.StopOverclock(multiplier);
    }
}
