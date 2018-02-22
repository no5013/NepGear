using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float damage;
    public float speed;
    public bool isServer;
	// Use this for initialization
	void Start () {
        Destroy(this.gameObject, 5f);
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
	}

    void OnCollisionEnter(Collision other) {
        Debug.Log("Points colliding: " + other.contacts.Length);
        Debug.Log("First normal of the point that collide: " + other.contacts[0].normal);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isServer)
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
        }
        Destroy(this.gameObject);
    }


}
