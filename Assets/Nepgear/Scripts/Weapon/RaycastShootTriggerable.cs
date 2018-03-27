using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastShootTriggerable : MonoBehaviour {
    [HideInInspector] public int magazine;
    public Transform bulletSpawn;
    [HideInInspector] public float reloadTime;
    [HideInInspector] public AudioClip gunSound;
    [HideInInspector] public float damage;
    [HideInInspector] public float range;
    [HideInInspector] public string gunId;
    [HideInInspector] public float force;
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
        if (isReloading)
        {
            return;
        }
        if (CanFire())
        {
            bulletLeft--;
            soundSource.Play();
            fwc.CmdFireRaycast(gunId, damage, force, range, bulletSpawn.forward, bulletSpawn.position, bulletSpawn.rotation);
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
