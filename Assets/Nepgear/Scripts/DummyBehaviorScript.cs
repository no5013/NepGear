using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyBehaviorScript : MonoBehaviour {

	public float Health;
    public float speed;
   
	// Use this for initialization
	void Start () {
   
	}

    // Update is called once per frame
    void Update()
    {
    }

	public void TakeDamage(float damage) {
		this.Health -= damage;
        if (Health <= 0)
        {
            Destroy(this.gameObject);
        }
        Debug.Log (this.Health + " Left");
	}
   
    //private void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log("Hit by" + other.tag);
    //    if(other.tag.Equals("Bullet"))
    //    {
    //        Bullet bullet = other.GetComponent<Bullet>();
    //        TakeDamage(bullet.damage);
    //        Destroy(other.gameObject);

    //    }
    //}
}
