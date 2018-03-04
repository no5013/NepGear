//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class ProjectileGunBehavior : FrameWeapon
//{

//    //public float damage;
//    public float range;
//    //public float speed;
//    public float fireRate;
//    public int magazine;
//    public float reloadTime;
//    public GameObject bullet;
//    public Transform muzzle;

//    private float nextFire;
//    private int bulletLeft;
//    private AudioSource gunSound;
//    private bool isReloading;

//    [SerializeField]
//    private Text m_BulletText;

//    private FrameWeaponController fwc;

//    // Use this for initialization
//    void Start()
//    {
//        nextFire = 0f;
//        bulletLeft = magazine;
//        isReloading = false;
//        gunSound = GetComponent<AudioSource>();
//        fwc = GetComponentInParent<FrameWeaponController>();
//    }

//    public void Initialize()
//    {
        
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (Input.GetButtonUp("Reload") || bulletLeft <= 0)
//        {
//            StartCoroutine(Reload());
//            return;
//        }
//        /*else if ((Input.GetButton("Fire1") || OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch) > 0))
//        {
//            Shoot();
//            ui.SetBullet(bulletLeft, magazine);
//        }*/

//        if (m_BulletText != null)
//        {
//            m_BulletText.text = bulletLeft + "/" + magazine;
//        }
//    }

//    public override void Shoot(out GameObject newBullet)
//    {
//        //Debug.Log("SHOOT FUNCTION");
//        newBullet = null;
//        if (!CanFire())
//        {
//            return;
//        }
//        nextFire = Time.time + 1f / fireRate;

//        bulletLeft--;
//        gunSound.Play();
//        //newBullet = Instantiate(bullet, muzzle.transform.position, muzzle.transform.rotation);

//        fwc.CmdFireProjectile(muzzle.forward, muzzle.position, muzzle.rotation);
//    }

//    public override bool CanFire()
//    {
//        return !(isReloading || Time.time < nextFire || bulletLeft <= 0);
//    }

//    IEnumerator Reload()
//    {
//        isReloading = true;
//        yield return new WaitForSeconds(reloadTime);

//        bulletLeft = magazine;
//        isReloading = false;
//        //m_BulletText.text = bulletLeft + "/" + magazine;
//    }
//}
