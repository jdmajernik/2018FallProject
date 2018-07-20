using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

    OUTDATED OUTDATED OUTDATED OUTDATED OUTDATEDOUTDATED

    */

public class PlayerMovement : MonoBehaviour {

    private UnityEngine.GameObject player;
    private UnityEngine.GameObject cameraEmpty;

    Rigidbody rb;

	void Start()
    {
        player = UnityEngine.GameObject.FindGameObjectWithTag("Player");
        cameraEmpty = UnityEngine.GameObject.Find("Camera_Empty");

        rb = gameObject.GetComponent<Rigidbody>();
	}
	
    /*
	void FixedUpdate()
    {
        Vector3 wantedPosition = Quaternion.Euler(0f, cameraEmpty.transform.localRotation.eulerAngles.y, 0f)
            * new Vector3(Mathf.RoundToInt(Input.GetAxis("Horizontal")), 0, Mathf.RoundToInt(Input.GetAxis("Vertical")));

        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("JUMP");
            rb.AddForce(0, 10f, 0);
        }

        transform.position = player.transform.position + wantedPosition;
        player.transform.rotation = Quaternion.Euler(0f, cameraEmpty.transform.localRotation.eulerAngles.y, 0f);
	}
    */
}
