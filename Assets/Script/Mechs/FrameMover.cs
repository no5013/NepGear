using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class FrameMover : MonoBehaviour {

    public float speed = 10f;
    public Transform destinationPoint;
    public Rigidbody rb;
    public bool mouseDown;

    public float boostSpeed = 5f;

    public float floatSpeed = 10f;

    public float drag = 0.5f;

    public bool reach = false;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 currentPosition = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 destinationPosition = new Vector3(destinationPoint.position.x, 0f, destinationPoint.position.z);

        Vector3 moveDirection = (destinationPosition - currentPosition).normalized;

        float distance = Vector3.Distance(currentPosition, destinationPosition);

        Debug.Log(distance);

        if (distance < 0.5)
        {
            reach = true;

            rb.useGravity = true;

            Vector3 vel = rb.velocity;
            vel.x *= drag;
            vel.z *= drag;

            rb.velocity = vel;

            //this.enabled = false;
        }
        else if(!reach)
        {
            rb.velocity = moveDirection * speed;
        }




        /*float fire1 = CrossPlatformInputManager.GetAxis("Fire1");
        if(fire1 > 0 && mouseDown == false)
        {
            Debug.Log("MOUSE DOWN");
            mouseDown = true;
            //Boost();
        }
        else if (fire1 > 0 && mouseDown == true)
        {
            Debug.Log("LONG");
            Boost();
        }
        else if(fire1 <= 0 && mouseDown == true){
            Debug.Log("MOUSE UP");
            mouseDown = false;
        }*/
    }

    void Dodge()
    {
        rb.velocity = transform.forward * speed;
    }

    void Boost()
    {
        rb.velocity = transform.forward * boostSpeed;
    }

    private void FixedUpdate()
    {
        /*if (Vector3.Distance(transform.position, destinationPoint.position) <= 1f)
        {
            //this.enabled = false;
            Debug.Log("FUCK");
        }
        else
        {
            rb.MovePosition(destinationPoint.position + transform.forward * Time.deltaTime);
            rb.velocity = (transform.forward * speed);
            Debug.Log(Vector3.Distance(transform.position, destinationPoint.position));
        }*/
    }
}
