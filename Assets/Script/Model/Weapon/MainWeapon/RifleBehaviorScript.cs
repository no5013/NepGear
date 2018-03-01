using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleBehaviorScript : MonoBehaviour
{

    public float damage;
    public float range;
    public float speed;
    public float fireRate;
    public int magazine;
    public float reloadTime;
    public GameObject bullet;

    private float nextFire;
    private int bulletLeft;
    private AudioSource gunSound;
    private bool isReloading;
    public GameObject muzzle;

    UIManager ui;
    // Use this for initialization
    void Start()
    {
        nextFire = 0f;
        bulletLeft = magazine;
        isReloading = false;
        gunSound = GetComponent<AudioSource>();
        ui = FindObjectOfType<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isReloading)
        {
            return;
        }
        if (Input.GetButtonUp("Reload") || bulletLeft <= 0)
        {
            StartCoroutine(Reload());
            return;
        }
        else if ((Input.GetButton("Fire2") || OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) > 0) && Time.time >= nextFire && bulletLeft > 0)
        {
            nextFire = Time.time + 1f / fireRate;
            Shoot();
            ui.SetBullet(bulletLeft, magazine);
        }
    }

    private void Shoot()
    {
        /*GameObject bulletClone = Instantiate(bullet, muzzle.transform.position, muzzle.transform.rotation);
        Bullet bulletInstance = bulletClone.GetComponent<Bullet>();
        bulletInstance.damage = damage;
        bulletInstance.speed = speed;
        bulletLeft--;
        gunSound.Play();*/
    }

    IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);

        bulletLeft = magazine;
        isReloading = false;
        ui.SetBullet(bulletLeft, magazine);
        //Implement BEST PRACTICE reload
    }
}