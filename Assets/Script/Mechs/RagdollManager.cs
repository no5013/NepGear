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
    private void Awake()
    {
        parentCollider = GetComponent<Collider>();
        parentRigidbody = GetComponent<Rigidbody>();

        ragdollColliders = GetComponentsInChildren<Collider>();
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();

        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        DisableRagdoll();
    }

    public void DisableRagdoll()
    {
        //foreach (Collider collider in ragdollColliders)
        //{
        //    if(parentCollider != collider)
        //    {
        //        collider.isTrigger = true;
        //    }
        //}

        foreach (Rigidbody rigidbody in ragdollRigidbodies)
        {
            if(parentRigidbody != rigidbody)
            {
                rigidbody.isKinematic = true;
            }
        }

        if(parentCollider != null)
            parentCollider.enabled = true;

        if (animator != null)
            animator.enabled = true;
    }

    public void EnableRagdoll()
    {
        //foreach (Collider collider in ragdollColliders)
        //{
        //    collider.isTrigger = false;
        //}

        foreach (Rigidbody rigidbody in ragdollRigidbodies)
        {
            rigidbody.isKinematic = false;
        }

        if (parentCollider != null)
            parentCollider.enabled = false;

        if (animator != null)
            animator.enabled = false;
    }
}