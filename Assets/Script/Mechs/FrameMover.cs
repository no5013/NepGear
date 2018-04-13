using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;

public class FrameMover : MonoBehaviour
{

    public float speed = 10f;
    public Transform target;

    public float drag = 0.9f;

    public bool reach = false;

    private Vector3 moveDir;

    private CharacterController character;
    private FirstPersonController firstPersonController;

    // Use this for initialization
    void Start()
    {
        character = GetComponent<CharacterController>();
        firstPersonController = GetComponent<FirstPersonController>();

        Vector3 targetPostition = new Vector3(target.position.x, transform.position.y, target.position.z);
        transform.LookAt(targetPostition);

        moveDir = CalculateMoveDirection();
    }

    private Vector3 CalculateMoveDirection()
    {
        Vector3 destinationPosition = new Vector3(target.position.x, transform.position.y, target.position.z);
        Vector3 moveDirection = (destinationPosition - transform.position).normalized;

        return moveDirection;
    }

    private void FixedUpdate()
    {
        Vector3 destinationPosition = new Vector3(target.position.x, transform.position.y, target.position.z);
        float distance = Vector3.Distance(transform.position, destinationPosition);

        if (distance < 10)
        {
            firstPersonController.enabled = true;
            reach = true;
        }

        if(reach && character.isGrounded)
        {
            this.enabled = false;
        }

        Vector3 moveVector = new Vector3(moveDir.x * speed, 0f, moveDir.z * speed);
        character.Move(moveVector * Time.fixedDeltaTime);
    }
}
