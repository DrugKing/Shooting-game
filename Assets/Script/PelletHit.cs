using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PelletHit : MonoBehaviour
{
    public float damage = 10f;
    public GameObject pellet;
    public GameObject hitEffect;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Enemy"))
        {
            target Target = collision.transform.GetComponent<target>();
            if (Target != null)
            {
                Target.damaged(damage);
                Destroy(pellet);
            }
            GameObject Poof = Instantiate(hitEffect, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
            Destroy(Poof, 0.5f);
        }        
    }
}
