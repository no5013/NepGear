﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;
using System;

public class FrameWeaponController : NetworkBehaviour {

    private UIManager uiManager;
    private InputHandler ih;

    //Gun component
    //[SerializeField] private FrameWeapon leftHand;
    [SerializeField] private GameObject leftHand;
    [SerializeField] private WeaponAbility leftHandAbility;
    private ProjectileShootTriggerable leftHandTrigger;

    //[SerializeField] private FrameWeapon rightHand;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private WeaponAbility rightHandAbility;
    private ProjectileShootTriggerable rightHandTrigger;

    [SerializeField] private GameObject uniqueWeapon;
    [SerializeField] private UniqueAbility uniqueAbility;


    public Transform eye;
    //public GameObjec unique
    //[SerializeField] private WeaponAbility eyeAbility;
    //public GameObject leftHand;
    //public Ability leftHandAbility;
    //public GameObject rightHand;
    //public Ability rightHandAbility;

    [SyncVar]
    public string leftWeaponID;
    [SyncVar]
    public string rightWeaponID;
    
    private ResourcesManager wrm;
    private Animator animator;

    private float leftCooldown = 0;
    private float leftNextReadyFire = 0;
    private float leftCoolDownTimeLeft = 0;

    private float rightCooldown = 0;
    private float rightNextReadyFire = 0;
    private float rightCoolDownTimeLeft = 0;

    private float uniqueCooldown = 0;
    private float uniqueNextReadyFire = 0;
    private float uniqueCoolDownTimeLeft = 0;



    // Use this for initialization
    void Start ()
    {
        wrm = LobbyManager.s_Singleton.resourcesManager;

        // Remove when complete Char select
        //Initialize(Instantiate(leftHandAbility), leftHand, Instantiate(rightHandAbility), rightHand);

        ih = GetComponent<InputHandler>();
        animator = GetComponent<Animator>();
        uiManager = GetComponent<PlayerBehaviorScript>().uiManager;

        Initialize(Instantiate(wrm.GetWeapon(leftWeaponID)), Instantiate(wrm.GetWeapon(rightWeaponID)));
    }


    //public void Initialize(WeaponAbility selectedLeftHandAbility, GameObject leftHandWeapon, WeaponAbility selectedRightHandAbility, GameObject rightHandWeapon)
    //{
    //    leftHandAbility = selectedLeftHandAbility;
    //    leftHand = leftHandWeapon;
    //    rightHandAbility = selectedRightHandAbility;
    //    rightHand = rightHandWeapon;

    //    leftCooldown = leftHandAbility.aFireDelay;
    //    rightCooldown = rightHandAbility.aFireDelay;

    //    leftHandAbility.Initialize(leftHand);
    //    rightHandAbility.Initialize(rightHand);
    //}

    public void Initialize(WeaponAbility selectedLeftHandAbility,WeaponAbility selectedRightHandAbility)
    {
        //// Set Child to Camera
        GameObject leftWeapon = Instantiate(selectedLeftHandAbility.gunPrefab);
        GameObject rightWeapon = Instantiate(selectedRightHandAbility.gunPrefab);

        leftWeapon.transform.parent = GetComponentInChildren<LeftController>().transform;
        rightWeapon.transform.parent = GetComponentInChildren<RightController>().transform;

        leftWeapon.transform.localPosition = Vector3.zero;
        rightWeapon.transform.localPosition = Vector3.zero;

        leftWeapon.transform.localRotation = Quaternion.identity;
        rightWeapon.transform.localRotation = Quaternion.identity;

        leftHandAbility = selectedLeftHandAbility;
        rightHandAbility = selectedRightHandAbility;

        leftHandTrigger = leftWeapon.GetComponent<ProjectileShootTriggerable>();
        rightHandTrigger = rightWeapon.GetComponent<ProjectileShootTriggerable>();

        leftCooldown = leftHandAbility.aFireDelay;
        rightCooldown = rightHandAbility.aFireDelay;


        leftHandAbility.Initialize(leftWeapon);
        rightHandAbility.Initialize(rightWeapon);

        if(uniqueAbility != null)
        {
            Debug.Log("Initializing UniqueAbility");
            uniqueCooldown = uniqueAbility.triggerDelay;
            uniqueAbility.Initialize(uniqueWeapon);
        }

        if(uiManager != null)
        {
            uiManager.SetLeftWeaponText(leftHandAbility.aMagazine + "/" + leftHandAbility.aMagazine);
            uiManager.SetRightWeaponText(rightHandAbility.aMagazine + "/" + rightHandAbility.aMagazine);
        }
    }

    public void SetLeftAbility(WeaponAbility la)
    {
        leftHandAbility = la;
    }

    public void SetRightAbility(WeaponAbility ra)
    {
        rightHandAbility = ra;
    }

    // Update is called once per frame
    void Update () {
        if(!isLocalPlayer || !ih.enabled)
        {
            return;
        }

        if (ih.fire1 > 0)
        {
            //Debug.Log("Called From Server : " + isServer.ToString() + " LeftHandNormal");
            //Debug.Log("Left Hand Fire by server? " + isServer + "Can fire ? " + leftHand.CanFire());
            if (Time.time > leftNextReadyFire)
            {
                //Debug.Log("We're going to fucking fire the motherfucking shot.!");
                /*CmdLeftHandShoot(muzzle.transform.position, muzzle.transform.rotation);
                nextFire = Time.time + 1f / 3;*/
                LeftButtonTriggered();
                
                //GameObject testBullet;
                //leftHand.Shoot(out testBullet);
           
            }
        }
        if (ih.fire2 > 0)
        {
            if (Time.time > rightNextReadyFire)
            {
                //Debug.Log("We're going to fucking fire the motherfucking shot.!");
                /*CmdLeftHandShoot(muzzle.transform.position, muzzle.transform.rotation);
                nextFire = Time.time + 1f / 3;*/
                RightButtonTriggered();
                //GameObject testBullet;
                //leftHand.Shoot(out testBullet);

            }
        }

        if (ih.fire2 > 0)
        {
            if (Time.time > uniqueNextReadyFire)
            {
                UniqueButtonTriggered();
            }
        }
        else
        {
            UniqueButtonDeTriggered();    
        }

    }

    private void UniqueButtonTriggered()
    {
        if(uniqueAbility != null)
        {
            uniqueNextReadyFire = uniqueCooldown + Time.time;
            uniqueCoolDownTimeLeft = uniqueCooldown;
            uniqueAbility.TriggerAbility();
        }
    }
    private void UniqueButtonDeTriggered()
    {
        if (uniqueAbility != null)
        {
            uniqueAbility.DeTriggerAbility();
        }
    }

    private void LeftButtonTriggered()
    {
        leftNextReadyFire = leftCooldown + Time.time;
        leftCoolDownTimeLeft = leftCooldown;
        //darkMask.enabled = true;
        //coolDownTextDisplay.enabled = true;

        //abilitySource.clip = ability.aSound;
        //abilitySource.Play();
        leftHandAbility.TriggerAbility();
        animator.SetTrigger("RecoilLeft");

        if (uiManager)
        {
            uiManager.SetLeftWeaponText(leftHandTrigger.bulletLeft + "/" + leftHandAbility.aMagazine);
        }
    }

    private void RightButtonTriggered()
    {
        rightNextReadyFire = rightCooldown + Time.time;
        rightCoolDownTimeLeft = rightCooldown;
        //darkMask.enabled = true;
        //coolDownTextDisplay.enabled = true;

        //abilitySource.clip = ability.aSound;
        //abilitySource.Play();
        rightHandAbility.TriggerAbility();
        animator.SetTrigger("RecoilRight");

        if (uiManager)
        {
            uiManager.SetRightWeaponText(rightHandTrigger.bulletLeft + "/" + rightHandAbility.aMagazine);
        }
    }

    [Command]
    public void CmdFireProjectile(string gunId, float charge, Vector3 forward, Vector3 position, Quaternion rotation)
    {
        ProjectileAbility gun = (ProjectileAbility)Prototype.NetworkLobby.LobbyManager.s_Singleton.resourcesManager.GetWeapon(gunId);
        Projectile projectile = gun.projectile;
        float initialCharge = 0f;
        if (charge <= 0f)
        {
            initialCharge = 1f;
        }
        if (gun.isSpread)
        {
            GameObject pallet;
            Rigidbody palletRigidBody;
            Bullet b;
            for (int i = 0; i< gun.palletCount; i++)
            {
                Quaternion projectileRotation = rotation;
                 
                float x = UnityEngine.Random.Range(-gun.spreadFactor, gun.spreadFactor);
                float y = UnityEngine.Random.Range(-gun.spreadFactor, gun.spreadFactor);
                projectileRotation *= Quaternion.Euler(new Vector3(x, y, 0));

                pallet = Instantiate(projectile.projectilePrefab, position, projectileRotation);

                palletRigidBody = pallet.GetComponent<Rigidbody>();
                palletRigidBody.velocity = pallet.transform.forward * (projectile.speed * charge + initialCharge);

                b = pallet.GetComponent<Bullet>();
                b.damage = projectile.damage * charge + initialCharge;
                b.lifeTime = projectile.lifeTime;
                b.force = projectile.force * charge + initialCharge;
                b.staggerDamage = projectile.staggerDamage * charge + initialCharge;

                NetworkServer.Spawn(palletRigidBody.gameObject);
            }
        }
        else
        {
            GameObject projectileInstance = Instantiate(projectile.projectilePrefab, position, rotation);

            Rigidbody projectileRigidBody = projectileInstance.GetComponent<Rigidbody>();
            projectileRigidBody.velocity = (forward) * (projectile.speed * charge + initialCharge);

            Bullet b = projectileInstance.GetComponent<Bullet>();
            if(b != null)
            {
                b.damage = projectile.damage * charge + initialCharge;
                b.lifeTime = projectile.lifeTime;
                b.force = projectile.force * charge + initialCharge;
                b.staggerDamage = projectile.staggerDamage * charge + initialCharge;
            }
            GrenadeBullet g = projectileInstance.GetComponent<GrenadeBullet>();
            if(g != null)
            {
                BlastRound blastRound = (BlastRound) projectile;
                g.damage = blastRound.damage * charge + initialCharge;
                g.lifeTime = blastRound.lifeTime;
                g.impactForce = blastRound.force * charge + initialCharge;
                g.blastDamage = blastRound.blastDamage * charge + initialCharge;
                g.blastForce = blastRound.blastForce * charge + initialCharge;
                g.blastRadius = blastRound.blastRadius * charge + initialCharge;
                g.travelSpeed = blastRound.speed * charge + initialCharge;
                g.staggerDamage = blastRound.staggerDamage * charge + initialCharge;
            }
         

            NetworkServer.Spawn(projectileRigidBody.gameObject);
        }
        RpcMuzzleFlash(gunId, position, rotation);
    }

    [Command]
    public void CmdActivateShield()
    {
        //ShieldTriggerable shield = uniqueWeapon.GetComponent<ShieldTriggerable>();
        //shield.isActivate = true;
        //shield.RpcChangeStatus(true);
        RpcChangeShieldStatus(true);
    }

    [ClientRpc]
    public void RpcChangeShieldStatus(bool status)
    {
        ShieldTriggerable shield = uniqueWeapon.GetComponent<ShieldTriggerable>();
        shield.isActivate = status;
    }


    [Command]
    public void CmdDeactivateShield()
    {
        //ShieldTriggerable shield = uniqueWeapon.GetComponent<ShieldTriggerable>();
        //shield.isActivate = true;
        //shield.RpcChangeStatus(true);
        RpcChangeShieldStatus(false);
    }

    /*[ClientRpc]
    public void RpcChangeShieldStatus(bool status)
    {
        // Disable or Enable Exactly shield for every client.
        
        //uniqueWeapon.id
    }

    [Command]
    public void CmdDeactivateShield()
    {

        //ShieldTriggerable shield = uniqueWeapon.GetComponent<ShieldTriggerable>();
        //shield.isActivate = false;
        ShieldTriggerable shield = uniqueWeapon.GetComponent<ShieldTriggerable>();
        shield.RpcChangeStatus(false);
    }*/

    [Command]
    public void CmdFireRocket(string gunId, Vector3 forward, Vector3 position, Quaternion rotation)
    {
        RocketAbility gun = (RocketAbility)uniqueAbility;
        RaycastHit hit;
        if (Physics.Raycast(eye.position, eye.forward, out hit, gun.range))
        {
            BlastRound rocket = gun.rocket;
            GameObject rocketInstance = Instantiate(rocket.projectilePrefab, position, rotation);

            Rigidbody rocketRigidBody = rocketInstance.GetComponent<Rigidbody>();
            //rocketRigidBody.velocity = (forward) * rocket.risingSpeed;

            HomingRocket r = rocketInstance.GetComponent<HomingRocket>();
            r.damage = rocket.damage;
            r.travelSpeed = rocket.speed;
            r.impactForce = rocket.force;
            r.blastForce = rocket.blastForce;
            r.blastRadius = rocket.blastRadius;
            r.blastDamage = rocket.blastDamage;
            r.staggerDamage = rocket.staggerDamage;
            r.hitX = hit.point.x;
            r.hitZ = hit.point.z;
            r.lifeTime = rocket.lifeTime;

            //RpcMuzzleFlash();
            NetworkServer.Spawn(rocketRigidBody.gameObject);
        }

         
    }

    [ClientRpc]
    public void RpcMuzzleFlash(string gunId, Vector3 position, Quaternion rotation)
    {
        //find the right muzzle flash
        WeaponAbility gun = Prototype.NetworkLobby.LobbyManager.s_Singleton.resourcesManager.GetWeapon(gunId);
        Instantiate(gun.muzzleFlash, position, rotation);
    }

    [Command]
    public void CmdFireRaycast(string gunId, Vector3 forward, Vector3 position, Quaternion rotation)
    {
        RaycastAbility gun = (RaycastAbility) Prototype.NetworkLobby.LobbyManager.s_Singleton.resourcesManager.GetWeapon(gunId);
        
        RaycastHit hit;
        if (Physics.Raycast(position, forward, out hit, gun.range))
        {
            if (hit.collider.tag.Equals("Player"))
            {
                hit.collider.SendMessage("TakeDamage", gun.damage);
            }
            Destructible target = hit.collider.transform.GetComponent<Destructible>();
            if (target != null)
            {
                target.TakeDamage(gun.damage);
                Rigidbody r = hit.collider.GetComponent<Rigidbody>();
                r.AddForce(transform.forward * gun.force);
            }
        }   
    }

}
