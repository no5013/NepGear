using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullBurstShootTriggerable : MonoBehaviour {

    [HideInInspector] public Projectile projectile;
    [HideInInspector] public AudioClip gunSound;

    private AudioSource soundSource;
    public Transform[] bulletSpawns;

    FrameWeaponController fwc;

    public void Initialize()
    {
        soundSource = GetComponent<AudioSource>();
        fwc = GetComponentInParent<FrameWeaponController>();
        soundSource.clip = gunSound;
    }

    public void FreeFire()
    {
        for (int i = 0; i < bulletSpawns.Length; i++)
        {
            Transform bulletSpawn = bulletSpawns[i];
            fwc.CmdFireFullBurst(bulletSpawn.forward, bulletSpawn.position, bulletSpawn.rotation);
        }
        soundSource.Play();
    }

}
