using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShootTriggerable : MonoBehaviour {

    public static float autoReloadDelay = 2f;

    [HideInInspector] public Projectile projectile;
    public Transform[] bulletSpawns;
    [HideInInspector] public float projectileForce;
    [HideInInspector] public int magazine;
    [HideInInspector] public float reloadTime;
    [HideInInspector] public AudioClip gunSound;
    [HideInInspector] public string gunId;
    [HideInInspector] public float maxRecoil;
    [HideInInspector] public float recoilRate;
    [HideInInspector] public float chargeRate;
    [HideInInspector] public float maxCharge;
    public bool isChargable;

    public ParticleSystem[] chargeParticleSystems;

    public bool isFiring;
    private AudioSource soundSource;

    [HideInInspector] public int bulletLeft;
    [HideInInspector] public bool isReloading;
    private float recoil;
    private float recoilCooldown;
    private float reloadDelay;
    private float charge;

    private int lastParticleIndex;
    private float perCharge;

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
        charge = 0f;
        isFiring = false;
        if (isChargable)
        {
            lastParticleIndex = -1;
            FindPerCharge();
        }
        
    }

    private void FindPerCharge()
    {
        perCharge = 100f / chargeParticleSystems.Length;
    }

    private void Update()
    {
        if (recoilCooldown > 0)
        {
            recoilCooldown -= Time.deltaTime;
            if (recoilCooldown < 0)
            {
                isFiring = false;
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
        //if (bulletLeft < magazine && !isReloading)
        //{
        //    reloadDelay += Time.deltaTime;
        //    if (reloadDelay >= autoReloadDelay)
        //    {
        //        Debug.Log("AutoReload");
        //        Reload();
        //    }
        //}
        if (isChargable)
        {
            if (charge < maxCharge)
            {
                Charge();
            }
        }
    }

    private void Charge()
    {
        charge += Time.deltaTime * chargeRate;
        if (charge > maxCharge)
        {
            charge = maxCharge;
        }
       
      
        int index = Mathf.FloorToInt((charge / maxCharge * 100) / perCharge) - 1;
        if (index == lastParticleIndex)
        {
            return;
        }
        lastParticleIndex = index;
        ChangeChargeState(index);
    }

    private void ChangeChargeState(int index)
    {
        for (int i = 0; i< chargeParticleSystems.Length; i++)
        {
            if (i == index)
            {
                //chargeParticleSystems[i].enableEmission = true;
                if(!chargeParticleSystems[i].isPlaying)
                {
                    chargeParticleSystems[i].Play();
                }
            }
            else
            {
                //chargeParticleSystems[i].enableEmission = false;
                if (chargeParticleSystems[i].isPlaying)
                {
                    chargeParticleSystems[i].Stop();
                }
            }
        }
    }

    public void Fire()
    {
        if (isReloading)
        {
            return;
        }
        reloadDelay = 0f;
        if (CanFire())
        {
            isFiring = true;
            for (int i = 0; i< bulletSpawns.Length; i++)
            {
                if (bulletLeft <= 0)
                    return;
                bulletLeft--;
                Transform bulletSpawn = bulletSpawns[i];
                RandomBulletSpawnRotation(ref bulletSpawn);
                if (isChargable)
                {
                    float chargePercent = charge / maxCharge;
                    fwc.CmdFireProjectile(gunId, chargePercent, bulletSpawn.forward, bulletSpawn.position, bulletSpawn.rotation);
                    charge = 0f;
                }
                else
                {
                    fwc.CmdFireProjectile(gunId, 1f, bulletSpawn.forward, bulletSpawn.position, bulletSpawn.rotation);
                }
                Recoil();
                recoilCooldown = 2.0f;
            }
            soundSource.Play();
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
        if(isReloading)
        {
            return;
        }
        reloadDelay = 0f;
        StartCoroutine(Reloading());
    }
    public bool CanFire()
    {
        return !(isReloading || bulletLeft <= 0);
    }
    private void RandomBulletSpawnRotation(ref Transform bulletSpawn)
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
        fwc.ReloadSuccessful();
        //m_BulletText.text = bulletLeft + "/" + magazine;
    }
}
