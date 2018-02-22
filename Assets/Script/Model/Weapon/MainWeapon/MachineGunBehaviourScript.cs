using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGunBehaviourScript : FrameWeapon {

    public float damage = 10f;
    public float range = 100f;
    private float nextFire;
    public float fireRate = 1f;
    public float impactForce = 30f;

    public Transform fpsCam;
    public GameObject impactEffect;
    private AudioSource gunSound;

    // Use this for initialization
    void Start () {
        nextFire = 0f;
        gunSound = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update ()
    {

    }

    public override void Shoot(out GameObject bullet)
    {
        nextFire = Time.time + 1f / fireRate;
        gunSound.Play();
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform);
            Destructible target = hit.transform.GetComponent<Destructible>();
            if(target != null)
            {
                target.TakeDamage(damage);
            }
            if(hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }
            GameObject i = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(i, 0.5f);
        }
        bullet = null;
    }
}
