using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FrameWeaponController : NetworkBehaviour {

    private InputHandler ih;

    //Gun component
    //[SerializeField] private FrameWeapon leftHand;
    [SerializeField] private GameObject leftHand;
    [SerializeField] private WeaponAbility leftHandAbility;
    //[SerializeField] private FrameWeapon rightHand;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private WeaponAbility rightHandAbility;
    //public GameObject leftHand;
    //public Ability leftHandAbility;
    //public GameObject rightHand;
    //public Ability rightHandAbility;

    [SyncVar]
    public string leftWeaponID;
    [SyncVar]
    public string rightWeaponID;

    public WeaponResourcesManager wrm;

    private float leftCooldown = 0;
    private float leftNextReadyFire = 0;
    private float leftCoolDownTimeLeft = 0;

    private float rightCooldown = 0;
    private float rightNextReadyFire = 0;
    private float rightCoolDownTimeLeft = 0;



    // Use this for initialization
    void Start ()
    {
        wrm.Init();

        // Remove when complete Char select
        //Initialize(Instantiate(leftHandAbility), leftHand, Instantiate(rightHandAbility), rightHand);

        ih = GetComponent<InputHandler>();
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

        leftCooldown = leftHandAbility.aFireDelay;
        rightCooldown = rightHandAbility.aFireDelay;

        leftHandAbility.Initialize(leftWeapon);
        rightHandAbility.Initialize(rightWeapon);
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
        if(!isLocalPlayer)
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
    }

    [Command]
    public void CmdFireProjectile(string projectileId, float projectileForce, Vector3 forward, Vector3 position, Quaternion rotation)
    {
        Projectile projectile = Prototype.NetworkLobby.LobbyManager.s_Singleton.resourcesManager.GetProjectile(projectileId);
        GameObject projectileInstance = Instantiate(projectile.projectilePrefab, position, rotation);

        Rigidbody projectileRigidBody = projectileInstance.GetComponent<Rigidbody>();
        projectileRigidBody.velocity = forward * 5f;

        Bullet b = projectileInstance.GetComponent<Bullet>();
        b.damage = projectile.damage;
        b.lifeTime = projectile.lifeTime;
        b.force = projectile.force;

        NetworkServer.Spawn(projectileRigidBody.gameObject);
    }

    [Command]
    public void CmdFireRaycast(string gunId, float damage, float force, float range, Vector3 forward, Vector3 position, Quaternion rotation)
    {
        RaycastHit hit;
        if (Physics.Raycast(position, forward, out hit, range))
        {
            if (hit.collider.tag.Equals("Player"))
            {
                hit.collider.SendMessage("TakeDamage", damage);
            }
            Destructible target = hit.collider.transform.GetComponent<Destructible>();
            if (target != null)
            {
                target.TakeDamage(damage);
                Rigidbody r = hit.collider.GetComponent<Rigidbody>();
                //r.AddForce(transform.forward * force);
            }
        }
        
    }

    //[ClientRpc]
    //public void RpcRaycastHit(RaycastHit hit, float damage)
    //{
    //    if (hit.collider.gameObject.tag == "player")
    //    {
    //        Debug.Log("Hit Player");
    //    }
    //    else
    //    {
    //        Debug.Log("Hit Smthing");
    //    }
    //}
}
