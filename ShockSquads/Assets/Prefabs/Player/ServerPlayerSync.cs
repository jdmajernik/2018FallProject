using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerPlayerSync : NetworkBehaviour {


	
	// Update is called once per frame
	void Update () {
        transform.position = Vector3.Lerp(transform.position,transform.parent.gameObject.GetComponent<SpawnPlayer>().localPlayerPos,.5f);
	}
}
