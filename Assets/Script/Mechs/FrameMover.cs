using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;

public class FrameMover : NetworkBehaviour
{
    public Transform target;
    public float maxSpeed = 10f;
    public float reachedSpeed = 10f;

    private bool reached = false;

    private Vector3 moveDir;

    private CharacterController character;
    private FirstPersonController firstPersonController;

    // Use this for initialization
    void Start()
    {
        character = GetComponent<CharacterController>();
        firstPersonController = GetComponent<FirstPersonController>();

        Vector3 targetPostition = new Vector3(target.position.x, transform.position.y, target.position.z);
        moveDir = CalculateMoveDirection();

        if(isLocalPlayer)
            transform.LookAt(targetPostition);
    }

    private Vector3 CalculateMoveDirection()
    {
        Vector3 destinationPosition = new Vector3(target.position.x, transform.position.y, target.position.z);
        Vector3 moveDirection = (destinationPosition - transform.position).normalized;

        return moveDirection;
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        Vector3 destinationPosition = new Vector3(target.position.x, transform.position.y, target.position.z);
        float distance = Vector3.Distance(transform.position, destinationPosition);

        if (distance < 10)
        {
            firstPersonController.enabled = true;
            reached = true;
        }

        if(reached && character.isGrounded)
        {
            this.enabled = false;
            GetComponent<PlayerBehaviorScript>().EnableControl();
        }

        Vector3 moveVector = new Vector3(moveDir.x * CalculateCurrentSpeed(), 0f, moveDir.z * CalculateCurrentSpeed());
        character.Move(moveVector * Time.fixedDeltaTime);
    }

    private float CalculateCurrentSpeed()
    {
        if (reached)
        {
            return reachedSpeed;
        }

        return maxSpeed;
    }
}
