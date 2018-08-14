using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using System.Net;

public class MatchLobbyGui : NetworkBehaviour {

    NetworkManager LobbyNetManager;
    NetworkTools networkInfo;

    private void Awake()
    {
        LobbyNetManager = gameObject.GetComponent<NetworkManager>();
        networkInfo = GameObject.FindGameObjectWithTag("GameController").GetComponent<NetworkTools>();

        LobbyNetManager.networkAddress = networkInfo.GetServerIP();
        //LobbyNetManager.client.
    }
}
