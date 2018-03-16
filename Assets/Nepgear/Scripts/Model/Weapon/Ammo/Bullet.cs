using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {

    [SyncVar] [HideInInspector] public float damage;
    [SyncVar] [HideInInspector] public float force;
    [SyncVar] [HideInInspector] public float lifeTime;

	// Use this for initialization
	void Start () {
        Destroy(this.gameObject, lifeTime);
	}
	
    void OnCollisionEnter(Collision other) {
        Debug.Log("Points colliding: " + other.contacts.Length);
        Debug.Log("First normal of the point that collide: " + other.contacts[0].normal);
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Bullet hit");
        if (other.tag.Equals("Player"))
        {
            other.SendMessage("TakeDamage", damage);

        }
        if (other.tag.Equals("Enemy"))
        {
            Debug.Log("Hit Enemy");
            other.SendMessage("TakeDamage", damage);
        }
        Destructible target = other.transform.GetComponent<Destructible>();

        if (target != null)
        {
            target.TakeDamage(damage);
            Rigidbody r = other.GetComponent<Rigidbody>();
            r.AddForce(transform.forward * force);
        }
        Destroy(this.gameObject);
    }


}
