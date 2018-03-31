using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollManager : MonoBehaviour
{

    private Collider[] ragdollColliders;
    private Rigidbody[] ragdollRigidbodies;

    private Collider parentCollider;
    private Rigidbody parentRigidbody;

    private Animator animator;

    // Use this for initialization
    void Start()
    {
        ragdollColliders = GetComponentsInChildren<Collider>();
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();

        parentCollider = GetComponent<Collider>();
        parentRigidbody = GetComponent<Rigidbody>();

        animator = GetComponentInChildren<Animator>();

        //DisableRagdoll();
        //EnableRagdoll();
    }

    public void DisableRagdoll()
    {
        foreach (Collider collider in ragdollColliders)
        {
            collider.isTrigger = true;
        }

        foreach (Rigidbody rigidbody in ragdollRigidbodies)
        {
            rigidbody.isKinematic = true;
        }

        parentCollider.enabled = true;
        parentRigidbody.isKinematic = false;
        animator.enabled = true;
    }

    public void EnableRagdoll()
    {
        foreach (Collider collider in ragdollColliders)
        {
            collider.isTrigger = false;
        }

        foreach (Rigidbody rigidbody in ragdollRigidbodies)
        {
            rigidbody.isKinematic = false;
        }

        parentCollider.enabled = false;
        animator.enabled = false;
    }
}