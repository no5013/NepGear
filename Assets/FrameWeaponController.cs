using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FrameWeaponController : NetworkBehaviour {

    private InputHandler ih;

    //Gun component
    //[SerializeField] private FrameWeapon leftHand;
    [SerializeField] private GameObject leftHand;
    [SerializeField] private Ability leftHandAbility;
    //[SerializeField] private FrameWeapon rightHand;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private Ability rightHandAbility;
    //public GameObject leftHand;
    //public Ability leftHandAbility;
    //public GameObject rightHand;
    //public Ability rightHandAbility;


    private float leftCooldown = 0;
    private float leftNextReadyFire = 0;
    private float leftCoolDownTimeLeft = 0;

    private float rightCooldown = 0;
    private float rightNextReadyFire = 0;
    private float rightCoolDownTimeLeft = 0;


    // Use this for initialization
    void Start ()
    {
        //Initialize(leftHandAbility, leftHand, rightHandAbility, rightHand);
        //Initialize(leftHandAbility, leftHand, rightHandAbility, rightHand);
    }

    public void Initialize(Ability selectedLeftHandAbility, GameObject leftHandWeapon, Ability selectedRightHandAbility, GameObject rightHandWeapon)
    {
        if(selectedLeftHandAbility.GetHashCode() == selectedLeftHandAbility.GetHashCode())
        {
            Debug.Log("same ref");
        }
        ih = GetComponent<InputHandler>();
        Debug.Log(ih.ToString());
        leftHandAbility = selectedLeftHandAbility;
        leftHand = leftHandWeapon;
        rightHandAbility = selectedRightHandAbility;
        rightHand = rightHandWeapon;

        leftCooldown = leftHandAbility.aBaseCoolDown;
        rightCooldown = rightHandAbility.aBaseCoolDown;

        leftHandAbility.Initialize(leftHand);
        rightHandAbility.Initialize(rightHand);
    }

    //public void Initialize()
    //{
    //    ih = GetComponent<InputHandler>();

    //    leftCooldown = leftHandAbility.aBaseCoolDown;
    //    rightCooldown = rightHandAbility.aBaseCoolDown;

    //    leftHandAbility.Initialize(leftHand);
    //    rightHandAbility.Initialize(rightHand);
    //}

    // Update is called once per frame
    void Update () {
        //if(!isLocalPlayer)
        //{
        //    return;
        //}
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

    //[Command]
    //void CmdLeftHandShoot(Vector3 a, Quaternion b)
    //{
    //    GameObject testBullet;
    //    leftHand.Shoot(out testBullet);

    //    if (testBullet != null)
    //    {
    //        GameObject newBullet = Instantiate(testBullet, a, b);
    //        newBullet.GetComponent<Bullet>().isServer = true;
    //        NetworkServer.Spawn(newBullet);
    //        testBullet.GetComponent<MeshRenderer>().enabled = false;
    //    }
    //}

    //[Command]
    //public void CmdFireProjectile(Vector3 forward, Vector3 position, Quaternion rotation)
    //{
    //    ProjectileGunBehavior pg = leftHand as ProjectileGunBehavior;
    //    GameObject projectileInstance = Instantiate(pg.bullet, position, rotation);
    //    NetworkServer.Spawn(projectileInstance);
    //}






}
