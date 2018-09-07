using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkTools : MonoBehaviour {
    //This Goes in the GameController object and handles a lot of the overarching (or global) variables and functions
    //I realize that this isn't always the best approach, but for this instance it is certainly the most manageable
    [SerializeField]private string PlayerId;

    private string ServerIP;
    private int Port;

    public void SetPlayerID(string newPlayerID)
    {
        PlayerId = newPlayerID;
    }
    public void setServerIP(string newServerIP)
    {
        ServerIP = newServerIP;
    }
    public void SetPort (int newPort)
    {
        Port = newPort;
    }

    public string GetPlayerID ()
    {
        return PlayerId;
    }
    public string GetServerIP ()
    {
        return ServerIP;
    }
    public int GetPort ()
    {
        return Port;
    }
}
