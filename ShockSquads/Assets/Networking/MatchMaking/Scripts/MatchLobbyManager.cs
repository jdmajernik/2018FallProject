using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Nat;

public class MatchLobbyManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        NatUtility.StartDiscovery();
        INatDevice device = 
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
