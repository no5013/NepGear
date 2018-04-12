using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HomingRocket : NetworkBehaviour {

    [SyncVar] [HideInInspector] public float damage;
    [SyncVar] [HideInInspector] public float impactForce;
    //[SyncVar] [HideInInspector] public float risingSpeed;
    [SyncVar] [HideInInspector] public float travelSpeed;
    [SyncVar] [HideInInspector] public float lifeTime;
    [SyncVar] [HideInInspector] public float blastDamage;
    [SyncVar] [HideInInspector] public float blastRadius;
    [SyncVar] [HideInInspector] public float blastForce;
    public GameObject impactPrefab;

    //public GameObject target;

    [SyncVar] [HideInInspector] public float hitX;
    private float hitY;
    [SyncVar] [HideInInspector] public float hitZ;

    private float distance_x;
    private float distance_y;
    private float distance_z;
    private float gravity = Physics.gravity.magnitude;
    private float usedAngle = 0f;
    private float velocityZ = 0f;
    private Rigidbody rb;
    private CapsuleCollider cc;
    private Vector3 velocity;
    private bool isActivated;
    private float timeBeforeHoming = 1f;

    // Use this for initialization
    void Start () {
        cc = GetComponent<CapsuleCollider>();
        cc.enabled = false;
        isActivated = false;
        hitY = 0f;
        //timeBeforeHoming = 1f;
        rb = GetComponent<Rigidbody>();
        Destroy(this.gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update () {
        if (!isActivated)
        {
            //transform.Translate(Vector3.up * Time.deltaTime * risingSpeed);
            timeBeforeHoming -= Time.deltaTime;
            if (timeBeforeHoming <= 0f)
            {
                isActivated = true;
                Activate();
                findShootingAngle();
                ApplyVelocity();
            }
        }
        else
        {
            Rotate();
        }

    }

    private void Activate()
    {
        cc.enabled = true;
    }

    private void ApplyVelocity()
    {
        float velocityX = travelSpeed * Mathf.Cos((usedAngle) * Mathf.Deg2Rad);
        float velocityY = travelSpeed * Mathf.Sin((usedAngle) * Mathf.Deg2Rad);
        if (distance_x < 0)
            velocityX = -velocityX;
        if (velocityZ != velocityZ)
            velocityZ = 0;
        if (distance_x == 0)
        {
            if (distance_z > 0)
                velocityX = -velocityX;
            velocity = new Vector3(0, velocityY, velocityX);
        }
        else 
            velocity = new Vector3(velocityX, velocityY, -velocityZ);
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = velocity;
        rb.useGravity = true;
    }

    private void Rotate()
    {
        float x = rb.velocity.x;
        float y = rb.velocity.y;
        float z = rb.velocity.z;
        float angleDrop = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
        float angleTurn = Mathf.Atan2(x, z) * Mathf.Rad2Deg;
        float offSetAngleDrop = 90 - angleDrop;
        float offSetAngleTurn = 90 - angleTurn;
        if (offSetAngleDrop > 0)
            offSetAngleDrop = -offSetAngleDrop;
        transform.eulerAngles = new Vector3(0, -offSetAngleTurn, offSetAngleDrop);
    }

    private void findShootingAngle()
    {
        if (hitX > transform.position.x)
        {
            distance_x = hitX - transform.position.x;
        }
        else
        {
            distance_x = transform.position.x - hitX;
            distance_x = -distance_x;
        }
       

        if (hitY > transform.position.y)
        {
            distance_y = hitY - transform.position.y;
        }
        else
        {
            distance_y = transform.position.y - hitY;
        }
        if (hitZ > transform.position.z)
        {
            distance_z = hitZ - transform.position.z;
            distance_z = -distance_z;
        }
        else
        {
            distance_z = transform.position.z - hitZ;
        }
        if (distance_x == 0 && distance_z ==0)
        {
            usedAngle = 90;
            return;
        }
        float distance_x_in_projectile;
        if (distance_x == 0)
            distance_x_in_projectile = distance_z;
        else
            distance_x_in_projectile = distance_x;


        float a = gravity * (distance_x_in_projectile * distance_x_in_projectile) / (2 * (travelSpeed * travelSpeed));
        float b = -Math.Abs(distance_x_in_projectile);
        float c = a - distance_y;

        float x1 = (-b + Mathf.Sqrt((b * b) - (4 * a * c))) / (2 * a);
        float x2 = (-b - Mathf.Sqrt((b * b) - (4 * a * c))) / (2 * a);

        float angle1 = Mathf.Atan(x1) * Mathf.Rad2Deg;
        float angle2 = Mathf.Atan(x2) * Mathf.Rad2Deg;

        
        if (angle1 != angle1 && angle2 != angle2)
        {
            usedAngle = 45;
        }
        else if (angle2 < angle1 && angle2 > 0)
        {
            usedAngle = angle2;
        }
        else
        {
            usedAngle = angle1;
        }
        if (distance_x == 0)
        {
            return;
        }
        float time = Mathf.Abs(distance_x / (travelSpeed * Mathf.Cos((usedAngle) * Mathf.Deg2Rad)));
       
        velocityZ = distance_z / time;
        float realVelocityX = travelSpeed * Mathf.Cos((usedAngle) * Mathf.Deg2Rad);
        float turnAngle = Math.Abs(Mathf.Atan2(realVelocityX, velocityZ) * Mathf.Rad2Deg);
        float maxVelocityZ = travelSpeed * Mathf.Cos((turnAngle) * Mathf.Deg2Rad);
        if (Mathf.Abs(velocityZ) > Math.Abs(maxVelocityZ)) 
        {
            if (velocityZ > 0)
                velocityZ = Math.Abs(travelSpeed * Mathf.Cos((turnAngle) * Mathf.Deg2Rad));
            else
                velocityZ = -Math.Abs(travelSpeed * Mathf.Cos((turnAngle) * Mathf.Deg2Rad));
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
            return;
        if (other.tag.Equals("Enemy"))
        {
            other.SendMessage("TakeDamage", damage);
            Destroy(this.gameObject);
        }
    }
}
