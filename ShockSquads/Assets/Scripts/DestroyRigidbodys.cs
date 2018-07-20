using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyRigidbodys : MonoBehaviour {

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Projectile")
        {
            Destroy(collision.gameObject);
        }
    }
}
