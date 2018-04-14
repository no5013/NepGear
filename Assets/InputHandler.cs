using System.Collections;
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
    }

    private void UpdateStates()
    {
        animator.SetFloat("Forward", vertical, 0.1f, Time.deltaTime);
        animator.SetFloat("Turn", horizontal, 0.1f, Time.deltaTime);
    }
}
