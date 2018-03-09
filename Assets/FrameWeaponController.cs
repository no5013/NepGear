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


    private float leftCooldown = 0;
    private float leftNextReadyFire = 0;
    private float leftCoolDownTimeLeft = 0;

    private float rightCooldown = 0;
    private float rightNextReadyFire = 0;
    private float rightCoolDownTimeLeft = 0;


    // Use this for initialization
    void Start ()
    {

        // Remove when complete Char select
        Initialize(Instantiate(leftHandAbility), leftHand, Instantiate(rightHandAbility), rightHand);
    }

    public void Initialize(WeaponAbility selectedLeftHandAbility, GameObject leftHandWeapon, WeaponAbility selectedRightHandAbility, GameObject rightHandWeapon)
    {
        ih = GetComponent<InputHandler>();
        leftHandAbility = selectedLeftHandAbility;
        leftHand = leftHandWeapon;
        rightHandAbility = selectedRightHandAbility;
        rightHand = rightHandWeapon;

        leftCooldown = leftHandAbility.aFireDelay;
        rightCooldown = rightHandAbility.aFireDelay;

        leftHandAbility.Initialize(leftHand);
        rightHandAbility.Initialize(rightHand);
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
    public void CmdFireProjectile(int spawnableID, float projectileForce, Vector3 forward, Vector3 position, Quaternion rotation)
    {
        //ProjectileAbility pa = leftHandAbility as ProjectileAbility;
        GameObject projectile = NetworkManager.singleton.spawnPrefabs[spawnableID];
        GameObject projectileInstance = Instantiate(projectile, position, rotation);

        Rigidbody projectileRigidBody = projectileInstance.GetComponent<Rigidbody>();
        //projectileRigidBody.AddForce(forward * projectileForce);
        projectileRigidBody.velocity = forward * 5f;
        NetworkServer.Spawn(projectileRigidBody.gameObject);
    }
}
