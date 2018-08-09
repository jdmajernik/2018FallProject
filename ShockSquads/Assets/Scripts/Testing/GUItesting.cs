using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShockSquadsGUI;

public class GUItesting : MonoBehaviour {

    private MainGUI testGui;
    [SerializeField] private GameObject guiClip;
	// Use this for initialization
	void Start () {
        int startingClips = 4;
        int bulletsPerClip = 12;
        int maxClips = 6;

        guiClip = Resources.Load("AmmoClip") as GameObject;

        testGui = new MainGUI(maxClips, startingClips, bulletsPerClip, guiClip);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.I))
        {
            testGui.Fire();
            Debug.Log("Firing");
        }
	}
}
