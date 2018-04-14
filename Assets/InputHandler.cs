﻿using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;
using UnityEngine.Networking;

public class InputHandler : NetworkBehaviour {

    public float horizontal;
    public float vertical;

    public float fire1;
    public float fire2;

    public float xRot;
    public float yRot;

    public bool jumping;
    public bool dashing;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        UpdateStates();
        HandleInput();
    }

    private void HandleInput()
    {
        horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        vertical = CrossPlatformInputManager.GetAxis("Vertical");

        yRot = CrossPlatformInputManager.GetAxis("Mouse X");
        xRot = CrossPlatformInputManager.GetAxis("Mouse Y");
        float handR = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch);
        float handL = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch);
        if (handR > 0 && handL > 0)
        {

        }
        else if (handR > 0)
        {
            yRot = handR;
        }
        else if (handL > 0)
        {
            yRot = -(handL);
        }

        fire1 = CrossPlatformInputManager.GetAxis("Fire1");
        if(fire1 <= 0)
        {
            fire1 = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch);
        }

        fire2 = CrossPlatformInputManager.GetAxis("Fire2");
        if (fire2 <= 0)
        {
            fire2 = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
        }

        jumping = CrossPlatformInputManager.GetButton("Jump");
        dashing = CrossPlatformInputManager.GetButton("Dash");
    }

    private void UpdateStates()
    {
        animator.SetFloat("Forward", vertical, 0.1f, Time.deltaTime);
        animator.SetFloat("Turn", horizontal, 0.1f, Time.deltaTime);
    }
}
