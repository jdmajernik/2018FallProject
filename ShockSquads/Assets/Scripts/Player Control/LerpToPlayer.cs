using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LerpToPlayer : NetworkBehaviour {

    [SerializeField]private GameObject mainCamera;


    public override void OnStartLocalPlayer()
    {
        mainCamera = Camera.main.transform.parent.gameObject;
        mainCamera.transform.parent = transform;
        mainCamera.transform.position = new Vector3(0, 0, 0);
    }

    void Update () {
        //transform.position = transform.position + (player.transform.position - transform.position) / 3;
        if (mainCamera != null)
        {
            mainCamera.transform.position = transform.position;
        }
    }
}
