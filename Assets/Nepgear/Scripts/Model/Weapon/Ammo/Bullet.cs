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
        Debug.Log("Bullet hit");
        if (other.gameObject.tag.Equals("Player"))
        {
            string dir = GetHitDir(other.transform);
            other.gameObject.SendMessage("TakeDamage", damage);
            //other.SendMessage("ShowHitDir", dir);

        }
        if (other.gameObject.tag.Equals("Enemy"))
        {
            Debug.Log("Hit Enemy");
            other.gameObject.SendMessage("TakeDamage", damage);
        }
        Destructible target = other.transform.GetComponent<Destructible>();

        if (target != null)
        {
            target.TakeDamage(damage);
            Rigidbody r = other.gameObject.GetComponent<Rigidbody>();
            r.AddForce(transform.forward * force);
        }
        RpcImpactEffect(other.contacts[0].point, Quaternion.identity);
        Destroy(this.gameObject);
    }



    //[ServerCallback]
    //private void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log("Bullet hit");
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

    //    Destroy(this.gameObject);
    //}

    [ClientRpc]
    public void RpcImpactEffect(Vector3 position, Quaternion rotation)
    {
        Instantiate(impactPrefab, position, rotation);

    }

    private string GetHitDir(Transform target)
    {
        //Vector3 vecBulletLocal = transform.InverseTransformDirection(target.position - transform.position);
        //Debug.Log("Local position in player" + vecBulletLocal.ToString());

        Vector3 displacement = transform.position - target.position;
        //Vector2 displacement = new Vector2(displacement3.x, displacement3.z);
        //Debug.Log(displacement.ToString());
        float forwardAngle = Vector3.Angle(displacement, target.forward);
        float angle = 0;
        float rightAngle = Vector3.Angle(displacement, target.right);
        //Debug.Log("Forward Dir = " + forwardAngle);
        //Debug.Log("Right Dir = " + rightAngle);

        if (rightAngle >= 90) // Shot come from left side
        {
            forwardAngle = 360 - forwardAngle;
        }

        angle = forwardAngle;
        //float angle = Vector3.Angle(new Vector3(0, 0, 1), vecBulletLocal) * Mathf.Sign(vecBulletLocal.x);
        //Vector2 u = new Vector2(target.position.x, target.position.z);
        //Vector2 v = new Vector2(transform.position.x, transform.position.z);
        //Debug.Log("Target vector is" + u.ToString());
        //Debug.Log("Bullet vector is" + v.ToString());
        //float angle = Vector2.Angle(u, v);
        ////float angle = Mathf.Acos((Vector2.Dot(u, v)) / (Vector2. * Vector2.Magnitude(v))) * Mathf.Rad2Deg;
        //Debug.Log("Hit dir angle is " + angle);
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
        ////Vector3 heading = transform.position - target.position;
        ////float angle = Vector3.Angle(heading, transform.forward);
        ////if ()

    }


}
