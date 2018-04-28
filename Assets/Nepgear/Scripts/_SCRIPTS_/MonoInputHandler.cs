﻿using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;

public class MonoInputHandler : MonoBehaviour
{

    public float horizontal;
    public float vertical;

    public float yRot;
    public float xRot;

    public float fire1;
    public float fire2;
    public bool fire3;

    public float reload1;
    public float reload2;

    public bool ultimate;

    public bool jumping;
    public bool dashing;

    public bool any;

    //private Animator animator;

    private void Start()
    {
        //animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        //UpdateStates();
        HandleInput();
    }

    private void HandleInput()
    {
        horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        vertical = CrossPlatformInputManager.GetAxis("Vertical");

        yRot = CrossPlatformInputManager.GetAxis("Mouse X");
        xRot = CrossPlatformInputManager.GetAxis("Mouse Y");

        fire1 = CrossPlatformInputManager.GetAxis("Fire1");
        if (fire1 <= 0)
        {
            //fire1 = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch);
            fire1 = CrossPlatformInputManager.GetAxis("VRFire1");
        }

        fire2 = CrossPlatformInputManager.GetAxis("Fire2");
        if (fire2 <= 0)
        {
            //fire2 = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
            fire2 = CrossPlatformInputManager.GetAxis("VRFire2");
        }
        fire3 = CrossPlatformInputManager.GetButton("Fire3");
        reload1 = CrossPlatformInputManager.GetAxis("Reload1");
        reload2 = CrossPlatformInputManager.GetAxis("Reload2");
        //if (fire3 <= 0)
        //{
        //    //fire3 = CrossPlatformInputManager.GetAxis("VRFire3");
        //}

        jumping = CrossPlatformInputManager.GetButton("Ascending");
        dashing = CrossPlatformInputManager.GetButton("Dash");
        ultimate = CrossPlatformInputManager.GetButton("UltimateHalfLeft") && CrossPlatformInputManager.GetButton("UltimateHalfRight");

        any = fire1 > 0 || fire2 > 0 || fire3 || reload1 > 0 || reload2 > 0 || jumping || dashing || ultimate;
    }

    private void UpdateStates()
    {
        /*animator.SetFloat("Forward", vertical, 0.1f, Time.deltaTime);
        animator.SetFloat("Turn", yRot, 0.1f, Time.deltaTime);*/
    }
}
