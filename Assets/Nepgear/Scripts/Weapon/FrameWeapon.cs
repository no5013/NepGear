using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class FrameWeapon : MonoBehaviour {

    // Use this for initialization
    
    public virtual void Shoot(out GameObject bullet)
    {
        bullet = null;
        Debug.Log("Shoot");
    }

    public virtual bool CanFire()
    {
        return false;
    }
}
