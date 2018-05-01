using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Abilities/UniqueAbilities/FunnelAbility")]
public class FunnelAbility : UniqueAbility {

    public Projectile projectile;
    public float fireDelay;
    public float staminaDrain;

    private FunnelTriggerable funnel;

    public override void DeTriggerAbility()
    {
        return;
    }

    public override void Initialize(GameObject obj)
    {
        funnel = obj.GetComponentInChildren<FunnelTriggerable>();
        funnel.projectile = projectile;
        funnel.gunSound = aSound;
        funnel.fireDelay = fireDelay;
        funnel.staminaDrain = staminaDrain;

        funnel.Initailize();
    }

    public override void TriggerAbility()
    {
        if (!funnel.isActivate)
        {
            funnel.Activate();
        }
        else
        {
            funnel.Deactivate();
        }
    }

    public override void TriggerReload()
    {
        return;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
