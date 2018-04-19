using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ShieldTriggerable : NetworkBehaviour {

    [HideInInspector] public float maxShieldHealth;
    [HideInInspector] public float shieldDecayRate;
    [HideInInspector] public float shieldRegenRate;
    private Collider shieldCollider;
    private Renderer shieldRenderer;
    public bool isActivate;
    private bool isInitialize = false;
    //[SyncVar(hook = "OnChangeStatus")] public bool isActivate;

    private float shieldHealth;

    FrameWeaponController frm;

	// Use this for initialization
    public void Initialize()
    {
        shieldCollider = GetComponentInChildren<Collider>();
        shieldRenderer = GetComponentInChildren<Renderer>();
        frm = GetComponentInParent<FrameWeaponController>();
        shieldHealth = maxShieldHealth;
        isActivate = false;
        isInitialize = true;
    }
	// Update is called once per frame
	void Update () {
        if(!isInitialize)
        {
            return;
        }
        if (shieldHealth <=0)
        {
            shieldHealth = 0f;
            Deactivate();
        }
        if (isActivate)
        {
            ActivateMechanic();
            shieldHealth -= shieldDecayRate * Time.deltaTime;
        }
        else
        {
            DeactivateMechanic();
            shieldHealth += shieldRegenRate * Time.deltaTime;
        }
    }

    public void TakeDamage(float damage)
    {
        shieldHealth -= damage;
    }

    public void ActivateMechanic()
    {
        shieldCollider.enabled = true;
        shieldRenderer.enabled = true;
    }

    public void DeactivateMechanic()
    {
        shieldCollider.enabled = false;
        shieldRenderer.enabled = false;
    }

    public void Activate()
    {
        if (isActivate)
            return;
        frm.CmdActivateShield();
        //shieldCollider.enabled = true;
        //shieldRenderer.enabled = true;
        //isActivate = true;
    }

    public void Deactivate()
    {
        if (!isActivate)
            return;
        frm.CmdDeactivateShield();
        //shieldCollider.enabled = false;
        //shieldRenderer.enabled = false;
        //isActivate = false;
    }

    /*void OnChangeStatus(bool status)
    {
        Debug.Log("Change status to " + status);
        shieldRenderer.enabled = status;
        shieldCollider.enabled = status;
    }
    [ClientRpc]
    public void RpcChangeStatus(bool status)
    {
        Debug.Log("Change shield status to" + status);
        isActivate = status;
        shieldCollider.enabled = status;
        shieldRenderer.enabled = status;
    }*/

}
