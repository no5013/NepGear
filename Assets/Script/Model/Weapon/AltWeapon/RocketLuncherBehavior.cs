using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLuncherBehavior : MonoBehaviour {

    public GameObject rocket;
    public float damage;
    public float impactForce;
    public float cooldown;
    public float speed;
    private Transform muzzle;

    private float fireCooldown;

	// Use this for initialization
	void Start () {
        fireCooldown = 0f;
        muzzle = GetComponentInParent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonUp("Fire3") && fireCooldown <= 0f)
        {
            Debug.Log("Rocket Fire");
            Shoot();
        }
        if (fireCooldown > 0f)
        {
            fireCooldown -= Time.deltaTime;
        }
    }
    
    private void Shoot()
    {
        GameObject rocketClone = Instantiate(rocket, muzzle.position, muzzle.rotation);
        HomingRocket rocketInstance = rocketClone.GetComponent<HomingRocket>();
        rocketInstance.damage = damage;
        rocketInstance.speed = speed;
        fireCooldown = cooldown;
    }
}
