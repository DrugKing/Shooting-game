using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleShoot : MonoBehaviour
{
    [Header("Firing")]
    public float damage = 10f;
    public float range = 50f;
    public float fireRate = 15f;
    private float nextTimeToFire = 0f;

    [Header("Shotgun")]
    public int pelletCount;
    public float spreadrange;
    public float shotPower = 100f;

    [Header("Ammo")]
    public int maxAmmo = 10;
    private int currentAmmo;
    public float reloadTime = 1f;
    public int maxAmmoStorage = 30;
    private int currentAmmoStorage;
    private bool reloading;
    private bool canshoot;

    [Header("GameObject")]
    public GameObject bulletPrefab;   
    public GameObject casingPrefab;
    public GameObject muzzleFlashPrefab;
    public GameObject hitEffect;
    private GameObject p;
    public Transform barrelLocation;
    public Transform casingExitLocation;
    public Camera fpsCamera;

    public Animator animator;
    List<Quaternion> pellets;

    void Start()
    {
        if (barrelLocation == null)
            barrelLocation = transform;
        currentAmmo = maxAmmo;
        currentAmmoStorage = 0;
        reloading = false;
        canshoot = true;
    }

    private void OnEnable()
    {
        reloading = false;
        animator.SetBool("Reload", false);
    }
    void Awake()
    {
        pellets = new List<Quaternion>(pelletCount);
        for (int i = 0; i < pelletCount; i++)
        {
            pellets.Add(Quaternion.Euler(Vector3.zero));
        }
    }
    void Update()
    {
        if (currentAmmo <= 0)
        {
            canshoot = false;
        }

        if (reloading)
        {
            return;
        }

        if (currentAmmo <=0 && currentAmmoStorage > 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetButtonDown("Fire1") && Time.time>= nextTimeToFire) 
        {
            nextTimeToFire = Time.time + (1f / fireRate);
            
            if (name == "Handgun" && canshoot)
            {
                GetComponent<Animator>().SetTrigger("Fire");
            }
            if (name == "Shotgun" && canshoot)
            {
                GetComponent<Animation>().Play("Reload");
                Shoot();
                CasingRelease();
            }            
        }        
    }

    void Shoot()
    {
        currentAmmo--;
        if (name == "Handgun")
        {
            GameObject bullet;
            bullet = Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(barrelLocation.forward * shotPower);

            GameObject tempFlash;
            //Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation).GetComponent<Rigidbody>().AddForce(barrelLocation.forward * shotPower);
            tempFlash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);

            Destroy(tempFlash, 0.5f);
            Destroy(bullet, 1f);
            //  Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation).GetComponent<Rigidbody>().AddForce(casingExitLocation.right * 100f);
            RaycastHit hit;
            if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, range))
            {
                Debug.Log(hit.transform.name);
                target Target = hit.transform.GetComponent<target>();
                if (Target != null)
                {
                    Target.damaged(damage);
                    Destroy(bullet);
                }

                GameObject Poof = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(Poof, 0.5f);
            }        
        }
        if(name == "Shotgun")
        {
            for (int i = 0; i < pelletCount; i++)
            {
                pellets[i] = Random.rotation;
                p = Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation);
                p.transform.rotation = Quaternion.RotateTowards(p.transform.rotation, pellets[i], spreadrange);
                p.GetComponent<Rigidbody>().AddForce(p.transform.right * shotPower);
                i++;
                Destroy(p, 0.5f);
            }
            GameObject tempFlash;
            tempFlash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);
            Destroy(tempFlash, 0.5f);
        }
        if (currentAmmo < 0)
        {
            currentAmmo = 0;
        }
    }

    void CasingRelease()
    {
        GameObject casing;
        casing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation) as GameObject;
        casing.GetComponent<Rigidbody>().AddExplosionForce(550f, (casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 1f);
        casing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(10f, 1000f)), ForceMode.Impulse);
        Destroy(casing, 0.5f);
    }
    //failed( might need some testing)
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Ammo"))
        {
            if(currentAmmo >0 && currentAmmoStorage < maxAmmoStorage)
            {
                currentAmmoStorage += maxAmmo;
                if(currentAmmoStorage > maxAmmoStorage)
                {
                    currentAmmoStorage = maxAmmoStorage;
                }
            }
            else
            {
                StartCoroutine(Reload());
            }
            Destroy(collision.collider.gameObject);
        }
    }

    IEnumerator Reload()
    {
        reloading = true;
        animator.SetBool("Reload", true);
        yield return new WaitForSeconds(reloadTime - 0.25f);
        animator.SetBool("Reload", false);
        yield return new WaitForSeconds(0.25f);
        if (currentAmmoStorage >0)
        {
            currentAmmo = currentAmmoStorage;
            currentAmmoStorage -= currentAmmo;
        }
        currentAmmo = maxAmmo;
        reloading = false;
    }
}
//Keep this script for zombie when they attack the player (might used)
/*if (_hit.collider.gameObject.CompareTag("Player"))
                {
                    _hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered,10f); 
                }   */