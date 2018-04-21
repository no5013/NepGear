using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;
using UnityEngine.Networking;

public class InputHandler : NetworkBehaviour {

    public float horizontal;
    public float vertical;

    public float yRot;
    public float xRot;

    public float fire1;
    public float fire2;
    public bool fire3;

    public bool ultimate;

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
        if(fire1 <= 0)
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
        //if (fire3 <= 0)
        //{
        //    //fire3 = CrossPlatformInputManager.GetAxis("VRFire3");
        //}

        jumping = CrossPlatformInputManager.GetButton("Ascending");
        dashing = CrossPlatformInputManager.GetButton("Dash");
        ultimate = CrossPlatformInputManager.GetButton("UltimateHalfLeft") && CrossPlatformInputManager.GetButton("UltimateHalfRight");
    }

    private void UpdateStates()
    {
        /*animator.SetFloat("Forward", vertical, 0.1f, Time.deltaTime);
        animator.SetFloat("Turn", yRot, 0.1f, Time.deltaTime);*/
    }
}
