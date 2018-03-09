using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {

    public float damage;

	// Use this for initialization
	void Start () {
        Destroy(this.gameObject, 5f);
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
            target.TakeDamage(10f);
            Rigidbody r = other.GetComponent<Rigidbody>();
            r.AddForce(transform.forward * 1000f);
        }
        Destroy(this.gameObject);
    }


}
