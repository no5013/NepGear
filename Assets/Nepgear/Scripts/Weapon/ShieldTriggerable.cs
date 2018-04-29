using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShieldTriggerable : MonoBehaviour {

    [HideInInspector] public float maxShieldHealth;
    [HideInInspector] public float shieldDecayRate;
    [HideInInspector] public float shieldRegenRate;
    private Collider shieldCollider;
    private Renderer shieldRenderer;
    public bool isActivate;
    private bool isInitialize = false;
    private Text shieldHealthText;
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
        shieldHealthText = GetComponentInChildren<Text>();
        isActivate = false;
        isInitialize = true;
    }
	// Update is called once per frame
	void Update () {
        if(!isInitialize)
        {
            return;
        }
        if (shieldHealth <= 0f)
        {
            shieldHealth = 0f;
            Deactivate();
        }
        if (isActivate)
        {
            ActivateMechanic();
            shieldHealth -= shieldDecayRate * Time.deltaTime;
            shieldHealthText.text = (shieldHealth / maxShieldHealth * 100).ToString("#.##") + " %";
            //if ((shieldHealth / maxShieldHealth * 100) < 30f)
            //{
            //    shieldHealthText.color = new Color(255, 62, 27, 1);
            //} else
            //{
            //    shieldHealthText.color = new Color(255, 255, 255, 1);
            //}
        }
        else
        {
            DeactivateMechanic();
            if (shieldHealth < maxShieldHealth)
            {
                shieldHealth += shieldRegenRate * Time.deltaTime;
                if (shieldHealth >= maxShieldHealth)
                {
                    shieldHealth = maxShieldHealth;
                }
            }
            shieldHealthText.text = "";
        }
        Debug.Log(shieldHealth);
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
