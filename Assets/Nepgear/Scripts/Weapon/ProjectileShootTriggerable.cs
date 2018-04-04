using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShootTriggerable : MonoBehaviour {

    [HideInInspector] public Projectile projectile;
    public Transform bulletSpawn;
    [HideInInspector] public float projectileForce;
    [HideInInspector] public int magazine;
    [HideInInspector] public float reloadTime;
    [HideInInspector] public AudioClip gunSound;
    [HideInInspector] public string gunId;
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
            RandomBulletSpawnRotation();
            fwc.CmdFireProjectile(gunId, bulletSpawn.forward, bulletSpawn.position, bulletSpawn.rotation);
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
    private void RandomBulletSpawnRotation()
    {
        float x = Random.Range(-1.5f, 1.5f);
        float y = Random.Range(-1.5f, 1.5f);
        float z = Random.Range(-1.5f, 1.5f);
        bulletSpawn.localRotation = Quaternion.Euler(x,y,z);
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
