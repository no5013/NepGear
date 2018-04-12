using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketShootTriggerable : MonoBehaviour {

    [HideInInspector] public Projectile rocket;
    [HideInInspector] public float damage;
    [HideInInspector] public float cooldown;
    [HideInInspector] public float range;
    public Transform[] muzzles;

    public Transform eyePosition;

    private int fireIndex;

    private float fireCooldown;

	// Use this for initialization
	void Start () {
        fireCooldown = 0f;
        //muzzle = GetComponentInParent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyUp(KeyCode.F) && fireCooldown <= 0f)
        {
            Debug.Log("Rocket Fire");
            Fire();
        }
        if (fireCooldown > 0f)
        {
            fireCooldown -= Time.deltaTime;
        }
    }

    public void Fire()
    {
        RaycastHit hit;
        Transform muzzle = muzzles[fireIndex];
        if (CanFire() && Physics.Raycast(eyePosition.position, eyePosition.forward, out hit, range))
        {
            /*GameObject rocketClone = Instantiate(rocket, muzzle.transform.position, Quaternion.identity);
            HomingRocket rocketScript = rocketClone.GetComponent<HomingRocket>();
            rocketScript.hitX = hit.point.x;
            rocketScript.hitY = hit.point.y;
            rocketScript.hitZ = hit.point.z;
            fireIndex++;*/
        }
    }

    private bool CanFire()
    {
        return fireIndex < muzzles.Length;
    }
    
    //private void Shoot()
    //{
    //    GameObject rocketClone = Instantiate(rocket, muzzle.position, muzzle.rotation);
    //    HomingRocket rocketInstance = rocketClone.GetComponent<HomingRocket>();
    //    rocketInstance.damage = damage;
    //    rocketInstance.speed = speed;
    //    fireCooldown = cooldown;
    //}
}
