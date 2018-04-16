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
    public ParticleSystem explosion;

    
    [SyncVar] [HideInInspector] public float hitX;
    private float hitY;
    [SyncVar] [HideInInspector] public float hitZ;

    /*public float damage;
    public float impactForce;
    //[SyncVar] [HideInInspector] public float risingSpeed;
    public float travelSpeed;
    public float lifeTime;
    public float blastDamage;
    public float blastRadius;
    public float blastForce;
    public GameObject impactPrefab;

    //[SyncVar] [HideInInspector] public float hitX;
    public float hitX;
    private float hitY;
    public float hitZ;
    //[SyncVar] [HideInInspector] public float hitZ;*/

    [SerializeField] private GameObject target;

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
    private const float timeBeforeHoming = 1f;

    // Use this for initialization
    void Start () {
        GetComponent<CapsuleCollider>().enabled = false;
        explosion.gameObject.GetComponent<destroyMe>().deathtimer = lifeTime + 2f;
        explosion.gameObject.GetComponent<destroyMe>().enabled = true;
        //cc.enabled = false;
        isActivated = false;
        hitY = 0f;
        StartCoroutine(EnableCollision());
        //if (target)
        //{
        //    hitX = target.transform.position.x;
        //    hitZ = target.transform.position.z;
        //}
        //Debug.Log(damage);
        //travelSpeed = 10f;
        //lifeTime = 10f;
        rb = GetComponent<Rigidbody>();
        Destroy(this.gameObject, lifeTime);
    }

    IEnumerator EnableCollision()
    {
        yield return new WaitForSeconds(timeBeforeHoming);
        GetComponent<CapsuleCollider>().enabled = true;
        findShootingAngle();
        ApplyVelocity();
        isActivated = true;
    }

    // Update is called once per frame
    void Update () {
        if (!isActivated)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * 10f);
        }
        else
        {
            Rotate();
        }

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
        float angleTurn2 = Mathf.Atan2(x, z) * Mathf.Rad2Deg;
        float offSetAngleDrop = -angleDrop;
        offSetAngleDrop = SetOffSetAngleDrop(offSetAngleDrop);
        transform.eulerAngles = new Vector3(offSetAngleDrop, angleTurn2, 0);
    }

    private float SetOffSetAngleDrop(float offSetAngleDrop)
    {
        if (offSetAngleDrop >= -180 && offSetAngleDrop <= -90)
        {
            float diff = offSetAngleDrop + 90;
            return -90 + Math.Abs(diff);
        }
        else if (offSetAngleDrop <= 180 && offSetAngleDrop >= 90)
        {
            float diff = offSetAngleDrop - 180;
            return Math.Abs(diff);
        }
        return offSetAngleDrop;
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

    [ServerCallback]
    private void OnCollisionEnter(Collision other)
    {
        PlayerBehaviorScript isPlayer = other.gameObject.GetComponentInParent<PlayerBehaviorScript>();
        if(isPlayer != null)
        {
         
            string dir = GetHitDir(other.transform);
            //parent.SendMessage("TakeDamage", damage);

            isPlayer.TakeDamage(damage);
            isPlayer.TickIndicator(dir);
            if (isPlayer.isDead())
            {
                //Rigidbody r = isPlayer.GetComponent<Rigidbody>();
                //r.AddForce(transform.forward * 100);
            }
            
        }
        else
        {
            Destructible target = other.transform.GetComponent<Destructible>();

            if (target != null)
            {
                target.TakeDamage(damage);
                Rigidbody r = other.gameObject.GetComponent<Rigidbody>();
                r.AddForce(transform.forward * impactForce);
            }
        }
        Explode();
        Explosion();
       
        //RpcExplosion(other.contacts[0].point, Quaternion.identity);
        Destroy(this.gameObject);
    }

    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);

        // Go through all the colliders...
        for (int i = 0; i < colliders.Length; i++)
        {
            // ... and find their rigidbody.
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

            // If they don't have a rigidbody, go on to the next collider.
            if (!targetRigidbody)
                continue;

            // Find the TankHealth script associated with the rigidbody.
            PlayerBehaviorScript targetHealth = targetRigidbody.GetComponent<PlayerBehaviorScript>();
            Destructible destructible = targetRigidbody.GetComponent<Destructible>();
            // If there is no TankHealth script attached to the gameobject, go on to the next collider.
            
            // Create a vector from the shell to the target.
            Vector3 explosionToTarget = targetRigidbody.position - transform.position;

            // Calculate the distance from the shell to the target.
            float explosionDistance = explosionToTarget.magnitude;

            // Calculate the proportion of the maximum distance (the explosionRadius) the target is away.
            float relativeDistance = (blastRadius - explosionDistance) / blastRadius;

            // Calculate damage as this proportion of the maximum possible damage.
            float damage = relativeDistance * blastDamage;

            // Make sure that the minimum damage is always 0.
            damage = Mathf.Max(0f, damage);

            if (targetHealth)
                targetHealth.TakeDamage(damage);
            if (destructible)
                destructible.TakeDamage(damage);
            // Deal this damage to the tank.
            targetRigidbody.AddExplosionForce(blastForce, transform.position, blastRadius);
        }
    }

    //[ClientRpc]
    //public void RpcExplosion(Vector3 position, Quaternion rotation)
    //{
    //    Debug.Log("Instantiate Explosion Impact Prefab");
    //    Instantiate(impactPrefab, position, rotation);
    //}
    
    public void Explosion()
    {
        explosion.transform.parent = null;
        explosion.Play();
        explosion.gameObject.GetComponent<AudioSource>().Play();
        //Instantiate(impactPrefab, position, rotation);
    }

    private string GetHitDir(Transform target)
    {
        Vector3 displacement = transform.position - target.position;

        float forwardAngle = Vector3.Angle(displacement, target.forward);
        float angle = 0;
        float rightAngle = Vector3.Angle(displacement, target.right);


        if (rightAngle >= 90) // Shot come from left side
        {
            forwardAngle = 360 - forwardAngle;
        }

        angle = forwardAngle;
        if ((angle >= 315 && angle <= 360) || (angle >= 0 && angle < 45))
        {
            return "front";
        }
        else if (angle >= 45 && angle < 135)
        {
            return "right";
        }
        else if (angle >= 135 && angle < 225)
        {
            return "back";
        }
        else if (angle >= 225 && angle < 315)
        {
            return "left";
        }
        return "";
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag.Equals("Player"))
    //        return;
    //    if (other.tag.Equals("Enemy"))
    //    {
    //        other.SendMessage("TakeDamage", damage);
    //        Destroy(this.gameObject);
    //    }
    //}
}
