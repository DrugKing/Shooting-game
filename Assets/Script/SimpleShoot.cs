using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleShoot : MonoBehaviour
{
    public float damage = 10f;
    public float range = 50f;
    public float fireRate = 15f;
    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public GameObject muzzleFlashPrefab;
    public Transform barrelLocation;
    public Transform casingExitLocation;
    public Camera fpsCamera;
    public GameObject hitEffect;

    public float shotPower = 100f;
    private float nextTimeToFire = 0f;

    void Start()
    {
        if (barrelLocation == null)
            barrelLocation = transform;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time>= nextTimeToFire) 
        {
            nextTimeToFire = Time.time + (1f / fireRate);
            GetComponent<Animator>().SetTrigger("Fire");
        }
    }

    void Shoot()
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
            if(Target!= null)
            {
                Target.damaged(damage);
                Destroy(bullet);
            }

            GameObject Poof =Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(Poof, 0.5f);
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


}
//Keep this script for zombie when they attack the player (might used)
/*if (_hit.collider.gameObject.CompareTag("Player"))
                {
                    _hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered,10f); 
                }    */