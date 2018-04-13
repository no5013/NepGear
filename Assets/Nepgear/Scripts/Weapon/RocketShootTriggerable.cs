using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketShootTriggerable : MonoBehaviour {

    [HideInInspector] public BlastRound rocket;
    [HideInInspector] public float cooldown;
    [HideInInspector] public float range;
    [HideInInspector] public string gunId;
    public Transform[] muzzles;

    public Transform eyePosition;

    public Transform parentOfMuzzle;
    private int fireIndex;

    private float fireCooldown;

    FrameWeaponController fwc;

	// Use this for initialization
	void Start () {
        fireCooldown = 0f;
        fwc = GetComponentInParent<FrameWeaponController>();
        //muzzle = GetComponentInParent<Transform>();
    }

    // Update is called once per frame
    void Update () {
        
        if(fireIndex != 0)
        {
            fireCooldown += Time.deltaTime;
            if (fireCooldown >= cooldown)
            {
                fireCooldown = 0f;
                ReloadRocket();
            }
        }  
    }

    private void ReloadRocket()
    {
       if(fireIndex > 0)
        {
            fireIndex--;
        }
    }

    public void Fire()
    {
        RaycastHit hit;
        if (CanFire() && Physics.Raycast(eyePosition.position, eyePosition.forward, out hit, range))
        {
            //Debug.Log("Lock rocket at " + hit.collider.gameObject.ToString());
            Transform muzzle = muzzles[fireIndex];
            //Quaternion rotation = muzzle.rotation;
            //rotation.eulerAngles = new Vector3(-90, 0, 0);
            //fireIndex++;
            fwc.CmdFireRocket(gunId, muzzle.up, muzzle.position, parentOfMuzzle.rotation);
            //GameObject rocketClone = Instantiate(rocket, muzzle.transform.position, Quaternion.identity);
            //HomingRocket rocketScript = rocketClone.GetComponent<HomingRocket>();
            //rocketScript.hitX = hit.point.x;
            //rocketScript.hitY = hit.point.y;
            //rocketScript.hitZ = hit.point.z;
            //fireIndex++;
        }
    }

    private bool CanFire()
    {
        return fireIndex < muzzles.Length;
    }
    
    //private void Shoot()
    //{
    //    GameObject rocketClone = Instantiate(rocket, muzzle.position, muzzle.rotation);
    //    HomingRocket rocketInstance = rocketClone.GetComponent<HomingRocket>();
    //    rocketInstance.damage = damage;
    //    rocketInstance.speed = speed;
    //    fireCooldown = cooldown;
    //}
}
