using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunnelTriggerable : MonoBehaviour {

    [HideInInspector] public Projectile projectile;
    [HideInInspector] public AudioClip gunSound;
    [HideInInspector] public float fireDelay;
    [HideInInspector] public float staminaDrain;

    public Transform bulletSpawn;

    private AudioSource audioSource;
    public bool isActivate;
    private bool isInitialize = false;
    private bool isFiring;
    private Renderer[] funnelRenderers;
    private float nextReadyFireTime;
    private PlayerBehaviorScript enemy;

    FrameWeaponController fwc;
    PlayerBehaviorScript pbs;


    public void Initailize()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = gunSound;
        fwc = GetComponentInParent<FrameWeaponController>();
        pbs = GetComponentInParent<PlayerBehaviorScript>();
        funnelRenderers = GetComponentsInChildren<Renderer>();
        //Debug.Log("Frame Weapon Controller" + fwc.ToString());
        nextReadyFireTime = 0f;
        isActivate = false;
        isInitialize = true;
        isFiring = false;
    }
	
	// Update is called once per frame
	void Update () {
	    if(!isInitialize)
        {
            return;
        }
        if(pbs.stamina <=0)
        {
            Deactivate();   
        }
        if(isActivate)
        {
            ActivateMechanic();
            pbs.stamina -= staminaDrain * Time.deltaTime;
            transform.LookAt(enemy.gameObject.transform);
            if (CanFire())
            {
                Fire();
                audioSource.Play();
            }
        }
        else
        {
            DeactivateMechanic();
        }
       
	}


    public void ActivateMechanic()
    {
        foreach(Renderer renderer in funnelRenderers)
        {
            renderer.enabled = true;
        }
        PlayerBehaviorScript[] players = GameObject.FindObjectsOfType<PlayerBehaviorScript>();
        foreach(PlayerBehaviorScript otherPlayer in players)
        {
            if(otherPlayer != pbs && otherPlayer.team != pbs.team)
            {
                enemy = otherPlayer;
                return;
            }
        }
        isFiring = true;
    }

    public void DeactivateMechanic()
    {
        isFiring = false;
        foreach (Renderer renderer in funnelRenderers)
        {
            renderer.enabled = false;
        }  
    }

    public void Activate()
    {
        if (isActivate)
            return;
        fwc.CmdActivateFunnel();
        pbs.shouldRegenStamina = false;
    }

    public void Deactivate()
    {
        if (!isActivate)
            return;
        fwc.CmdDeactivateFunnel();
        pbs.shouldRegenStamina = true;  
    }

    private bool CanFire()
    {
        return Time.time > nextReadyFireTime;
    }

    public void Fire()
    {
        fwc.CmdFireFunnel(bulletSpawn.forward, bulletSpawn.position, bulletSpawn.rotation);
        nextReadyFireTime = Time.time + fireDelay;
    }

}
