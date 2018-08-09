using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShockSquadsGUI;

public class GUItesting : MonoBehaviour {

    private MainGUI testGui;
    private GuiToolset guiTools;

	// Use this for initialization
	void Start () {
        int startingClips = 4;
        int bulletsPerClip = 12;
        int maxClips = 6;

        guiTools = GameObject.FindGameObjectWithTag("GUI").GetComponent<GuiToolset>();
        testGui = new MainGUI(maxClips, startingClips, bulletsPerClip, guiTools);
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
