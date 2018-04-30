﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GrenadeBullet : NetworkBehaviour {

    [SyncVar] [HideInInspector] public float damage;
    [SyncVar] [HideInInspector] public float impactForce;
    //[SyncVar] [HideInInspector] public float risingSpeed;
    [SyncVar] [HideInInspector] public float travelSpeed;
    [SyncVar] [HideInInspector] public float lifeTime;
    [SyncVar] [HideInInspector] public float blastDamage;
    [SyncVar] [HideInInspector] public float blastRadius;
    [SyncVar] [HideInInspector] public float blastForce;
    [SyncVar] [HideInInspector] public float staggerDamage;
    [SyncVar] [HideInInspector] public float timeBeforeDestroy;
    public ParticleSystem explosion;

    private bool isCollideAlready;

    // Use this for initialization
    void Start () {
        DisableCollision();
        StartCoroutine(EnableCollision());
        Destroy(this.gameObject, lifeTime);
        //isCollideAlready = false;
    }
	
    IEnumerator EnableCollision()
    {
        yield return new WaitForSeconds(0.1f);
        GetComponent<Collider>().enabled = true;
    }

    [ServerCallback]
    private void OnCollisionEnter(Collision other)
    {
        PlayerBehaviorScript isPlayer = other.gameObject.GetComponentInParent<PlayerBehaviorScript>();
        ShieldTriggerable shield = other.gameObject.GetComponentInParent<ShieldTriggerable>();
        if (shield != null)
        {
            shield.TakeDamage(damage);
            Destroy(this.gameObject);
            return;
        }
        if (isPlayer != null && GetComponent<Collider>().enabled)
        {
            DisableCollision();
            string dir = GetHitDir(other.transform);
            isPlayer.TakeDamage(damage);
            isPlayer.Staggering(staggerDamage);
            isPlayer.TickIndicator(dir);
            
        }
        else
        {
            Destructible target = other.transform.GetComponent<Destructible>();

            if (target != null)
            {
                target.TakeDamage(damage);
                Rigidbody r = other.gameObject.GetComponent<Rigidbody>();
                r.AddForce(transform.forward * impactForce);
                //DisableCollision();
            }
        }
        Explode();
        //Explosion();
      
        //Destroy(this.gameObject);
    }
    private void DisableCollision()
    {
        GetComponent<Collider>().enabled = false;
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
            {
                continue;
            }

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
            float stagger = relativeDistance * staggerDamage;
            float force = relativeDistance * blastForce;
            // Make sure that the minimum damage is always 0.
            damage = Mathf.Max(0f, damage);
            stagger = Mathf.Max(0f, stagger);

            if (targetHealth)
            {
                targetHealth.TakeDamage(damage);
                targetHealth.Staggering(stagger);
                targetHealth.CmdAddImpact(explosionToTarget, force);
            }
            if (destructible)
            {
                destructible.TakeDamage(damage);
                targetRigidbody.AddExplosionForce(blastForce, transform.position, blastRadius);
            }
            // Deal this damage to the tank.
        }
        NetworkServer.Destroy(this.gameObject);
    }

    public override void OnNetworkDestroy()
    {
        //we spawn the explosion particle
        Explosion();
        //set the particle to be destroyed at the end of their lifetime
        base.OnNetworkDestroy();
    }

    private void Explosion()
    {
        explosion.transform.parent = null;
        explosion.gameObject.GetComponent<destroyMe>().deathtimer = 2f;
        explosion.gameObject.GetComponent<destroyMe>().enabled = true;
        explosion.Play();
        AudioSource explosionSound = explosion.gameObject.GetComponent<AudioSource>();
        if (explosionSound != null)
            explosionSound.Play();
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
}
