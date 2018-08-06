using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkTools : MonoBehaviour {
    //This Goes in the GameController object and handles a lot of the overarching (or global) variables and functions
    private string PlayerId;

    private string ServerIP;
    private string Port;

    public void SetPlayerID(string newPlayerID)
    {
        PlayerId = newPlayerID;
    }
}
