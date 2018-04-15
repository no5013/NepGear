using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelRotate : MonoBehaviour {


    private float rotateSpeed;
    private const float maxRotateSpeed = 1.5f;
    private ProjectileShootTriggerable ps;
    private Animator animator;
	// Use this for initialization
	void Start () {
        rotateSpeed = 0f;
        ps = GetComponent<ProjectileShootTriggerable>();
        animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        if(ps.isFiring)
        {
            IncreaseSpeed();
        }
        else
        {
            DecreaseSpeed();
        }
    }

    private void LateUpdate()
    {
        //Debug.Log("Rotate Speed" + rotateSpeed);
        animator.speed = rotateSpeed;

    }

    private void DecreaseSpeed()
    {
        if(rotateSpeed > 0)
        {
            rotateSpeed -= 0.1f;
            if(rotateSpeed < 0)
            {
                rotateSpeed = 0;
            }
        }
    }

    private void IncreaseSpeed()
    {
        if(rotateSpeed < maxRotateSpeed)
        {
            //Debug.Log("Increase Speed");
            rotateSpeed += 0.1f;
            if(rotateSpeed > maxRotateSpeed)
            {
                rotateSpeed = maxRotateSpeed;
            }
        }
    }
}
