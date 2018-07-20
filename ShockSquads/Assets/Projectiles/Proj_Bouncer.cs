using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proj_Bouncer : MonoBehaviour
{
    GameObject creator;
    public Team good_team;

    float thrust = 100f;
    float damage = 26f;
    
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * thrust);
        Invoke("DestroySelf", 3f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.root.tag == "Player")
        {
            if (collision.transform.root.GetComponent<PlayerController>().currentteam != good_team)
            {
                collision.transform.root.GetComponent<PlayerController>().TakeDamage(damage, transform.position);
                Destroy(gameObject);
            }
        }
        if (collision.transform.root.tag == "Enemy")
        {
            if (collision.transform.root.GetComponent<AI_Enemy>().currentteam != good_team)
            {
                collision.transform.root.GetComponent<AI_Enemy>().TakeDamage(damage, transform.position);
                Destroy(gameObject);
            }
        }
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}