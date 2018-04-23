﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateControllerScript : MonoBehaviour {

    private PlayerBehaviorScript pbs;
    private FrameWeaponController fwc;

    private float satelliteCannonDuration;
    private bool canSatelliteFire;
    private bool isFullBurst;
    
	// Use this for initialization
	void Start () {
        pbs = this.gameObject.GetComponent<PlayerBehaviorScript>();
        fwc = this.gameObject.GetComponent<FrameWeaponController>();
        canSatelliteFire = false;
        isFullBurst = false;
    }



    public void Overclock(float multiplier)
    {
        pbs.CmdOverclock(multiplier);
    }

    public void StopOverclock(float multiplier)
    {
        pbs.CmdStopOverClock(multiplier);
    }

    public void SatelliteCannon(float startHeight, float fireDelay)
    {
        canSatelliteFire = true;
        StartCoroutine(FiringSatellite(startHeight, fireDelay));
        //StartCoroutine(CountDownSatellite(duration));
        //FiringSatellite(startHeight, fireDelay);
        //CountDownSatellite(duration);    

    }

    public void StopSatelliteCannon()
    {
        canSatelliteFire = false;
    }

    public void FullBurst(float fireDelay)
    {
        Debug.Log("Fullburst Activate");
        isFullBurst = true;
        fwc.CmdFullBurst();
        pbs.CmdFullBurst();
        // ค้าง จารย์ มันค้าง!!!!
        StartCoroutine(FiringFullBurst(fireDelay));
    }

    public void StopFullBurst()
    {
        isFullBurst = false;
        fwc.CmdStopFullBurst();
        pbs.CmdStopFullBurst();
    }

    //IEnumerator CountDownSatellite(float duration)
    //{
    //    yield return new WaitForSeconds(duration);
    //    canSatelliteFire = false;
    //}

    IEnumerator FiringFullBurst(float fireDelay)
    {
        while(isFullBurst)
        {
            FullBurstShootTriggerable[] allGuns = GetComponentsInChildren<FullBurstShootTriggerable>();
            foreach(FullBurstShootTriggerable gun in allGuns)
            {
                gun.FreeFire();
            }
            yield return new WaitForSeconds(fireDelay);
        }
       
    }

    IEnumerator FiringSatellite(float startHeight, float fireDelay)
    {
        while (canSatelliteFire)
        {
            PlayerBehaviorScript[] players = FindObjectsOfType<PlayerBehaviorScript>();
            PlayerBehaviorScript myself = GetComponent<PlayerBehaviorScript>();
            Quaternion rotation = Quaternion.identity;
            foreach (PlayerBehaviorScript otherPlayer in players)
            {
                if (otherPlayer != myself && !otherPlayer.team.Equals(myself.team))
                {
                    if (otherPlayer.isDead())
                        continue;
                    //position = new Vector3(otherPlayer.transform.position.x, startHeight, otherPlayer.transform.position.z);
                    rotation.SetLookRotation(Vector3.down);
                    fwc.CmdFireSatelliteCannon(Vector3.down, new Vector3(otherPlayer.transform.position.x, startHeight, otherPlayer.transform.position.z), rotation);
                }
            }
            yield return new WaitForSeconds(fireDelay);
        }
       
    }
}
