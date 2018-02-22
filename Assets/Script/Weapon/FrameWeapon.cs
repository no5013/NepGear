using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class FrameWeapon : NetworkBehaviour {

    // Use this for initialization
    
    public virtual void Shoot(out GameObject bullet)
    {
        bullet = null;
        Debug.Log("Shoot");
    }

    [Command]
    public virtual void CmdShoot()
    {
        Debug.Log("CmdShoot");
    }

    public virtual bool CanFire()
    {
        return false;
    }
}
