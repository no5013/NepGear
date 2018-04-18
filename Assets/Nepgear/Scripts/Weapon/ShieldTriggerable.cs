using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldTriggerable : MonoBehaviour {

    [HideInInspector] public float maxShieldHealth;
    [HideInInspector] public float shieldDecayRate;
    [HideInInspector] public float shieldRegenRate;
    private Collider shieldCollider;
    private Renderer shieldRenderer;
    private bool isActivate;

    private float shieldHealth;

	// Use this for initialization
    public void Initialize()
    {
        shieldCollider = GetComponentInChildren<Collider>();
        shieldRenderer = GetComponentInChildren<Renderer>();
        Debug.Log(shieldCollider.ToString());
        Debug.Log(shieldRenderer.ToString());

        shieldHealth = maxShieldHealth;
        isActivate = false;
    }
	// Update is called once per frame
	void Update () {
        if (shieldHealth <=0)
        {
            shieldHealth = 0f;
            Deactivate();
        }
        if (isActivate)
        {
            shieldHealth -= shieldDecayRate * Time.deltaTime;
        }
        else
        {
            shieldHealth += shieldRegenRate * Time.deltaTime;
        }
    }

    public void TakeDamage(float damage)
    {
        shieldHealth -= damage;
    }

    public void Activate()
    {
        if (isActivate)
            return;
        shieldCollider.enabled = true;
        shieldRenderer.enabled = true;
        isActivate = true;
    }

    public void Deactivate()
    {
        if (!isActivate)
            return;
        shieldCollider.enabled = false;
        shieldRenderer.enabled = false;
        isActivate = false;
    }
}
