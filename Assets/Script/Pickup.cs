using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public int Ammo_Capacity;
    public int Healthpack;
    public string WeaponAmmo;
    public string FirstKit;
    // Start is called before the first frame update
    void Start()
    {
        Ammo_Capacity = Random.Range(3, 15);
        Healthpack = Random.Range(25, 50);
    }
}
