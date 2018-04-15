using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

[RequireComponent(typeof(Animator))]

public class IKControl : NetworkBehaviour
{

    protected Animator animator;

    public bool ikActive = false;
    public Transform leftHandObj = null;
    public Transform rightHandObj = null;
    public Transform headRotation = null;
    public Transform lookObj = null;

    private Quaternion m_HeadTargetRot;

    private float smoothTime = 1000f;

    private float MinimumX = -45F;
    private float MaximumX = 45F;

    private float MinimumY = -90F;
    private float MaximumY = 90F;

    private float MinimumZ = -30F;
    private float MaximumZ = 30F;

    void Start()
    {
        animator = GetComponent<Animator>();

        m_HeadTargetRot = animator.GetBoneTransform(HumanBodyBones.Head).rotation;
    }

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if (animator)
        {

            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive)
            {

                // Set the look target position, if one has been assigned
                if (lookObj != null)
                {
                    //animator.SetLookAtWeight(1);
                    //animator.SetLookAtPosition(lookObj.position);
                }

                // Set the right hand target position and rotation, if one has been assigned
                if (rightHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
                }

                if (leftHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
                }

            }

            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                animator.SetLookAtWeight(0);
            }
        }
    }

    private void LateUpdate()
    {
        if (animator && isLocalPlayer)
        {
            if (headRotation != null)
            {
                Transform transform = animator.GetBoneTransform(HumanBodyBones.Head);
                transform.rotation = Quaternion.Slerp(transform.rotation, ClampRotationAroundXAxis(transform.rotation * headRotation.rotation), smoothTime * Time.deltaTime);
            }
        }
    }

    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        float angleY = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.y);
        angleY = Mathf.Clamp(angleY, MinimumY, MaximumY);
        q.y = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleY);

        float angleZ = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.z);
        angleZ = Mathf.Clamp(angleZ, MinimumZ, MaximumZ);
        q.z = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleZ);

        return q;
    }
}