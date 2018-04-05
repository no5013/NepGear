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
    [HideInInspector] public float maxRecoil;
    [HideInInspector] public float recoilRate;
    private AudioSource soundSource;

    private int bulletLeft;
    private bool isReloading;
    private float recoil;
    private float recoilCooldown;

    FrameWeaponController fwc;

    public void Initialize()
    {
        soundSource = GetComponent<AudioSource>();
        fwc = GetComponentInParent<FrameWeaponController>();
        soundSource.clip = gunSound;
        bulletLeft = magazine;
        isReloading = false;
        recoil = 0f;
        recoilCooldown = 0f;
    }

    private void Update()
    {
        if (recoilCooldown > 0)
        {
            recoilCooldown -= Time.deltaTime;
            if (recoilCooldown < 0)
            {
                recoilCooldown = 0f;
            }
        }
        else 
        {
            recoil -= 0.1f;
            if(recoil < 0)
            {
                recoil = 0f;
            }
        }
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
            Recoil();
            recoilCooldown = 2.0f;
        }
        else
        {
            Reload();
        }
    }
    private void Recoil()
    {
        recoil += recoilRate;
        if(recoil > maxRecoil)
        {
            recoil = maxRecoil;
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
        if (recoil == 0)
        {
            bulletSpawn.localRotation = Quaternion.Euler(0, 0, 0);
        }
        float x = RandomRecoil();
        float y = RandomRecoil();
        float z = RandomRecoil();
        bulletSpawn.localRotation = Quaternion.Euler(x,y,z);
    }

    private float RandomRecoil()
    {
        return Random.Range(-recoil, recoil);
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
