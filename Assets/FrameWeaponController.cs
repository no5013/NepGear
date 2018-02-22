using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FrameWeaponController : NetworkBehaviour {

    private InputHandler ih;

    //Gun component
    [SerializeField]
    private FrameWeapon leftHand;
    [SerializeField]
    private FrameWeapon rightHand;

    public GameObject bullet;
    public GameObject muzzle;
    public float nextFire;

    //private Transform transform;

    // Use this for initialization
    void Start () {
        ih = GetComponent<InputHandler>();
        leftHand = GetComponentsInChildren<FrameWeapon>()[0];
        rightHand = GetComponentsInChildren<FrameWeapon>()[1];

        //transform = GetComponent<Transform>();
    }
	
	// Update is called once per frame
	void Update () {
        if(!isLocalPlayer)
        {
            return;
        }
        if (ih.fire1 > 0)
        {
            //Debug.Log("Called From Server : " + isServer.ToString() + " LeftHandNormal");
            //Debug.Log("Left Hand Fire by server? " + isServer + "Can fire ? " + leftHand.CanFire());
            if (nextFire < Time.time)
            {
                //Debug.Log("We're going to fucking fire the motherfucking shot.!");
                CmdLeftHandShoot(muzzle.transform.position, muzzle.transform.rotation);
                nextFire = Time.time + 1f / 3;
            }
        }
        if (ih.fire2 > 0)
        {
            /*rightHand.Shoot(isServer);
            CmdRightHandShoot();*/
        }
    }
       
    [Command]
    void CmdLeftHandShoot(Vector3 a, Quaternion b)
    {
        GameObject testBullet;
        leftHand.Shoot(out testBullet);

        if (testBullet != null)
        {
            GameObject newBullet = Instantiate(testBullet, a, b);
            newBullet.GetComponent<Bullet>().isServer = true;
            NetworkServer.Spawn(newBullet);
            testBullet.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    /*
    [ClientRpc]
    void RpcLeftHandSpawn()
    {
        Debug.Log("Called From Server : " + isServer.ToString() + " LeftHandRPC");
        leftHand.Shoot(isServer);
    }

    [Command]
    void CmdRightHandShoot()
    {
        rightHand.Shoot(isServer);
        RpcRightHandSpawn();
    }

    [ClientRpc]
    void RpcRightHandSpawn()
    {
        GameObject bullet = null;
        rightHand.Shoot(isServer);
    }
    */




}
