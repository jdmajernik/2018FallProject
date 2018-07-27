using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnPlayer : NetworkBehaviour {
    //Written by John Majernik
    //This script sets up the player between the local (first person) and the server (third person) prefabs

    //public variables
    public enum playerLoadout { laser };

    //private variables
    [SerializeField] private GameObject localPrefabLaser;
    [SerializeField] private GameObject serverPrefabLaser;

    [SyncVar] public Vector3 localPlayerPos;

    public override void OnStartLocalPlayer()
    {
        
    }
    private void Start()
    {
        if(isLocalPlayer)
        {
            Instantiate(localPrefabLaser, transform);
        }
        else
        {
            Instantiate(serverPrefabLaser, transform);
        }
    }
    [Command] public void CmdUpdatePos(Vector3 newPos)
    {
        localPlayerPos = newPos;
    }
}
