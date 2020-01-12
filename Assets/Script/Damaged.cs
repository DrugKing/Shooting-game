using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Damaged : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Image healthBar;

    public float startHealth = 100;
    public float health;

    // Start is called before the first frame update
    void Start()
    {
        health = startHealth;
        healthBar.fillAmount = health / startHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log(health);
        healthBar.fillAmount = health / startHealth;

        if (health <= 0f)
        {
            //Die
            Die();
        }
    }
    void Die()
    {
        if (photonView.IsMine)
        {
            PlayerManager.instance.LeaveRoom();
        }
    }
}
