using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShockSquadsGUI;

public class GUItesting : MonoBehaviour {

    private MainGUI testGui;

	// Use this for initialization
	void Start () {
        int startingClips = 4;
        int bulletsPerClip = 12;
        int maxClips = 6;

        testGui = new MainGUI(maxClips, startingClips, bulletsPerClip);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.I))
        {
            testGui.Fire();
            Debug.Log("Firing");
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            testGui.Reload();
            Debug.Log("Reloading");
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            testGui.AddClip();
            testGui.ReCenterClips();
            Debug.Log("adding Clip");
        }
    }
}
