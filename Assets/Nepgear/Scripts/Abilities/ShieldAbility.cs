using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/UniqueAbilities/ShieldAbility")]
public class ShieldAbility : UniqueAbility {

    public float maxShieldHealth;
    public float shieldDecayRate;
    public float shieldRegenRate;

    private ShieldTriggerable shield;

    public override void DeTriggerAbility()
    {
        shield.Deactivate();
    }

    public override void Initialize(GameObject obj)
    {
        shield = obj.GetComponent<ShieldTriggerable>();
        shield.maxShieldHealth = maxShieldHealth;
        shield.shieldDecayRate = shieldDecayRate;
        shield.shieldRegenRate = shieldRegenRate;
        
        shield.Initialize();
    }

    public override void TriggerAbility()
    {
        shield.Activate();
    }

    public override void TriggerReload()
    {
        return;
    }
}
