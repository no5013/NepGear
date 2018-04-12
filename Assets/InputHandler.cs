using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR;

public class InputHandler : NetworkBehaviour {

    public float horizontal;
    public float vertical;

    public float fire1;
    public float fire2;

    public float leftGrip;
    public float rightGrip;

    public float dash;

    private Animator animator;

    private Quaternion characterTargetRot;

    public SteamVR_TrackedObject leftHandController;
    public SteamVR_TrackedObject rightHandController;

    private SteamVR_Controller.Device mDevice;


    private void Start()
    {
        animator = GetComponent<Animator>();

        characterTargetRot = transform.rotation;
    }

    private void FixedUpdate()
    {
        /*if (!isLocalPlayer)
        {
            return;
        }*/
        UpdateStates();
        HandleInput();
    }

    private void HandleInput()
    {
        horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        vertical = CrossPlatformInputManager.GetAxis("Vertical");

        string model = UnityEngine.XR.XRDevice.model != null ? UnityEngine.XR.XRDevice.model : "";

        fire1 = CrossPlatformInputManager.GetAxis("Fire2");
        if (fire1 <= 0 && model.IndexOf("Rift") >= 0)
        {
            fire1 = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch);
        }
        else if (fire1 <= 0 && leftHandController != null)
        {
            mDevice = SteamVR_Controller.Input((int)leftHandController.index);
            fire1 = mDevice.GetAxis(EVRButtonId.k_EButton_SteamVR_Trigger).x;
        }

        fire2 = CrossPlatformInputManager.GetAxis("Fire2");
        if (fire2 <= 0 && model.IndexOf("Rift") >= 0)
        {
            fire2 = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
        }
        else if (fire2 <= 0 && rightHandController != null)
        {
            mDevice = SteamVR_Controller.Input((int)rightHandController.index);
            fire2 = mDevice.GetAxis(EVRButtonId.k_EButton_SteamVR_Trigger).x;
        }


        //Handle Rotation
        /*if (model.IndexOf("Rift") >= 0)
        {
           
        }
        else
        {
            mDevice = SteamVR_Controller.Input((int)leftHandController.index);
            bool leftClicked = mDevice.GetPress(EVRButtonId.k_EButton_Grip);
            if (leftClicked)
            {
                leftGrip = 1;
            }
            else
            {
                leftGrip = 0;
            }

            mDevice = SteamVR_Controller.Input((int)rightHandController.index);
            bool rightClicked = mDevice.GetPress(EVRButtonId.k_EButton_Grip);
            if (rightClicked)
            {
                rightGrip = 1;
            }
            else
            {
                rightGrip = 0;
            }
        }*/
    }

    private void UpdateStates()
    {
        animator.SetFloat("Forward", vertical, 0.1f, Time.deltaTime);
        animator.SetFloat("Turn", horizontal, 0.1f, Time.deltaTime);
    }
}
