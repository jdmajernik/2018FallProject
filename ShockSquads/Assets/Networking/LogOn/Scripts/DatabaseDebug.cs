using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class DatabaseDebug : MonoBehaviour
{

    //Gui Public Variables
    public Image ConnectedImage;
    public Image LoggedInImage;

    public Sprite greenCheckmark;
    public Sprite redX;

    //Variables for Scripting
    private bool DebuggerActive = true;
    private GameObject NetworkDebugger;

    void Start()
    {
        NetworkDebugger = GameObject.FindGameObjectWithTag("DatabaseDebug");
        DontDestroyOnLoad(this.gameObject);//keeps this object between scenes to store global variables in
    }
    private void Update()
    {
        if (NetworkDebugger != null)
        {
            if (Input.GetButtonDown("NetworkDebug"))
            {
                DebuggerActive = !DebuggerActive;
                NetworkDebugger.SetActive(DebuggerActive); //I realize that this is a bit redundant
            }
        }
    }
    //All the button Functions
    public void ServerConnected()
    {
        ConnectedImage.sprite = greenCheckmark;
    }
    public void LoggedIn()
    {
        LoggedInImage.sprite = greenCheckmark;
    }
}
