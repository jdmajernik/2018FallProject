using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using System.Net;

public class MatchLobbyGui : NetworkBehaviour {

    NetworkManager LobbyNetManager;
    NetworkTools networkInfo;

    [SerializeField] private int lobbyPort = 4180; //the port for the lobby manager

    private void Awake()
    {
        LobbyNetManager = gameObject.GetComponent<NetworkManager>();
        networkInfo = GameObject.FindGameObjectWithTag("GameController").GetComponent<NetworkTools>();

        networkInfo.SetPort(lobbyPort); //sets the port in networkInfo TEMPORARY, will probably change later

        LobbyNetManager.networkAddress = networkInfo.GetServerIP();
        LobbyNetManager.client.Connect(networkInfo.GetServerIP(), networkInfo.GetPort());
        //LobbyNetManager.client.
    }
}
