﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShootTriggerable : MonoBehaviour {

    [HideInInspector] public Rigidbody projectile;
    public Transform bulletSpawn;
    [HideInInspector] public float projectileForce;
    [HideInInspector] public int magazine;
    [HideInInspector] public float reloadTime;
    [HideInInspector] public AudioClip gunSound;
    private AudioSource soundSource;

    private int bulletLeft;
    private bool isReloading;

    FrameWeaponController fwc;

    public void Initialize()
    {
        soundSource = GetComponent<AudioSource>();
        fwc = GetComponentInParent<FrameWeaponController>();
        soundSource.clip = gunSound;
        bulletLeft = magazine;
        isReloading = false;
    }

    public void Fire()
    {
        if(isReloading)
        {
            return;
        }
        if(CanFire())
        {
            /*Rigidbody cloneBullet = Instantiate(projectile, bulletSpawn.position, transform.rotation) as Rigidbody;
            cloneBullet.AddForce(bulletSpawn.transform.forward * projectileForce);*/
            bulletLeft--;
            soundSource.Play();
            fwc.CmdFireProjectile("p1", projectileForce, bulletSpawn.forward, bulletSpawn.position, bulletSpawn.rotation);
        }
        else
        {
            Reload();
        }
    }
    public void Reload()
    {
        StartCoroutine(Reloading());
    }
    public bool CanFire()
    {
        return !(isReloading || bulletLeft <= 0);
    }
    
    IEnumerator Reloading()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);

        bulletLeft = magazine;
        isReloading = false;
        //m_BulletText.text = bulletLeft + "/" + magazine;
    }
}
