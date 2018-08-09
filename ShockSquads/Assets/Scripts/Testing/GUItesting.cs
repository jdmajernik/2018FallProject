using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShockSquadsGUI;
using UnityEngine.Rendering;
using UnityEngine.PostProcessing;

public class GUItesting : MonoBehaviour {

    //variables for post-processing
    [SerializeField] PostProcessingProfile volume;
    float trauma = 0;
    float traumaDecrease = 0.05f;
    float hitTrauma = 1.5f;
    static float MAX_TRAUMA = 20f;
    bool traumaRunning = false;

    //Variables for the GUI
    private MainGUI testGui;
    private GuiToolset guiTools;

    // Use this for initialization
    void Start() {
        

        int startingClips = 4;
        int bulletsPerClip = 12;
        int maxClips = 6;

        guiTools = GameObject.FindGameObjectWithTag("GUI").GetComponent<GuiToolset>();
        testGui = new MainGUI(maxClips, startingClips, bulletsPerClip, guiTools);

    }

    // Update is called once per frame
    void Update() {
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
        if(Input.GetKeyDown(KeyCode.J))
        {
            trauma += trauma + hitTrauma;
            if(trauma>MAX_TRAUMA)
            {
                trauma = MAX_TRAUMA;
            }
            if(!traumaRunning)
            {
                StartCoroutine(Trauma());
            }
        }
    }
    public void PostProcessStartup()
    {
        volume.bloom.enabled = true;
        volume.chromaticAberration.enabled = true;
        volume.vignette.enabled = true;
        volume.grain.enabled = true;
    }
    public void PostProcessHit()
    {
        float intensity = 1 - ((MAX_TRAUMA - trauma) / MAX_TRAUMA);
        Debug.Log("Intensity - " + intensity);

        var bloom = volume.bloom.settings;
        var chroma = volume.chromaticAberration.settings;
        var vignette = volume.vignette.settings;
        var grain = volume.grain.settings;

        bloom.bloom.intensity = intensity*3;
        chroma.intensity = intensity;
        vignette.intensity = Mathf.Clamp(intensity,0,0.63f);
        grain.intensity = intensity;

        volume.bloom.settings = bloom;
        volume.chromaticAberration.settings = chroma;
        volume.vignette.settings = vignette;
        volume.grain.settings = grain;

    }
    public void PostProcessEnd()
    {
        volume.bloom.enabled = false;
        volume.chromaticAberration.enabled = false;
        volume.vignette.enabled = false;
        volume.grain.enabled = false;
    }
    public IEnumerator Trauma()
    {
        //basic trauma system with a set decrease per cycle
        traumaRunning = true;
        PostProcessStartup();
        while(trauma>0)
        {
            PostProcessHit();
            trauma -= Mathf.Sqrt(trauma) * traumaDecrease;
            yield return null;
        }
        trauma = 0;
        PostProcessEnd();
        traumaRunning = false;
        yield return null;
    }
}
