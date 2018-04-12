using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {

    [SyncVar] [HideInInspector] public float damage;
    [SyncVar] [HideInInspector] public float force;
    [SyncVar] [HideInInspector] public float lifeTime;
    public GameObject impactPrefab;

	// Use this for initialization
	void Start () {
        Destroy(this.gameObject, lifeTime);
	}
	
    [ServerCallback]
    void OnCollisionEnter(Collision other) {

        //Debug.Log("Points colliding: " + other.contacts.Length);
        //Debug.Log("First normal of the point that collide: " + other.contacts[0].normal);
        Debug.Log("Collision Bullet hit");
        PlayerBehaviorScript isPlayer = other.gameObject.GetComponentInParent<PlayerBehaviorScript>();
        if (isPlayer != null)
        {
            GameObject parent = isPlayer.gameObject;
            if (parent.tag.Equals("Player"))
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
        }
        else
        {
            Destructible target = other.transform.GetComponent<Destructible>();

            if (target != null)
            {
                target.TakeDamage(damage);
                Rigidbody r = other.gameObject.GetComponent<Rigidbody>();
                r.AddForce(transform.forward * force);
            }
        }
        RpcImpactEffect(other.contacts[0].point, Quaternion.identity);
        Destroy(this.gameObject);
    }



    //[ServerCallback]
    //private void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log("Trigger Bullet hit");
    //    if (other.tag.Equals("Player"))
    //    {
    //        string dir = GetHitDir(other.transform);
    //        other.SendMessage("TakeDamage", damage);
    //        //other.SendMessage("ShowHitDir", dir);

    //    }
    //    if (other.tag.Equals("Enemy"))
    //    {
    //        Debug.Log("Hit Enemy");
    //        other.SendMessage("TakeDamage", damage);
    //    }
    //    Destructible target = other.transform.GetComponent<Destructible>();

    //    if (target != null)
    //    {
    //        target.TakeDamage(damage);
    //        Rigidbody r = other.GetComponent<Rigidbody>();
    //        r.AddForce(transform.forward * force);
    //    } 
    //   Destroy(this.gameObject);
    //}

    [ClientRpc]
    public void RpcImpactEffect(Vector3 position, Quaternion rotation)
    {
        Instantiate(impactPrefab, position, rotation);

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
        if((angle>= 315 && angle <=360) || (angle >= 0 && angle < 45))
        {
            return "front";
        }
        else if (angle>=45 && angle < 135)
        {
            return "right";
        }
        else if (angle>=135 && angle < 225)
        {
            return "back";
        }
        else if (angle>=225 && angle<315)
        {
            return "left";
        }
        return "";
    }


}
