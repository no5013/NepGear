using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingRocket : MonoBehaviour {

    public float damage;
    public float speed;
    private bool isHoming;
    private float timeBeforeHoming;
    private GameObject target;

	// Use this for initialization
	void Start () {
        isHoming = false;
        timeBeforeHoming = 1f;
        Destroy(this, 5f);
    }
	
	// Update is called once per frame
	void Update () {
		if(isHoming)
        {
            if(target == null)
            {
                target = GameObject.FindGameObjectWithTag("Enemy");
                speed *= 2;
            }
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.up * Time.deltaTime * speed);
            timeBeforeHoming -= Time.deltaTime;
            if (timeBeforeHoming <= 0f)
            {
                isHoming = true;
            }
        }
	}

    public float GetDamage()
    {
        return damage;
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
