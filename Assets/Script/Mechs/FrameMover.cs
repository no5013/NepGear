using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class FrameMover : MonoBehaviour
{

    public float speed = 10f;
    public Transform target;

    public float drag = 0.9f;

    public bool reach = false;

    private Rigidbody rb;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Vector3 targetPostition = new Vector3(target.position.x, transform.position.y, target.position.z);
        transform.LookAt(targetPostition);
    }

    private void FixedUpdate()
    {
        Vector3 currentPosition = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 destinationPosition = new Vector3(target.position.x, 0f, target.position.z);

        Vector3 moveDirection = (destinationPosition - currentPosition).normalized;

        float distance = Vector3.Distance(currentPosition, destinationPosition);

        Debug.Log(distance);

        if (distance < 0.5)
        {
            reach = true;
            this.enabled = false;
        }
        else if (!reach)
        {
            rb.velocity = new Vector3(moveDirection.x, 0, moveDirection.z) * speed;
        }
    }
}
