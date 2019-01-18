using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    PlayerController player;
    float bulletSpeed = 12.0f;
    float homingRotation = 10.0f;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        //Homing Function
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(player.transform.position - transform.position), homingRotation * Time.deltaTime);

        //Forward Momentum
        transform.position = transform.position + (transform.forward.normalized * bulletSpeed * Time.deltaTime); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.GetComponent<PlayerController>().HitByBullet();
        }

        if (other.tag != "Enemy")
        {
            Destroy(this.gameObject); //To be destroyed but not on self
        }
    }
}
