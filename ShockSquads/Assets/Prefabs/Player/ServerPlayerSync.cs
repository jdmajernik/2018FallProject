using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerPlayerSync : NetworkBehaviour {


	
	// Update is called once per frame
	void Update () {
        transform.position = transform.parent.gameObject.GetComponent<SpawnPlayer>().localPlayerPos;
	}
}
