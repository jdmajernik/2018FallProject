using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShockSquadsGUI;
using UnityEngine.Rendering;
using UnityEngine.PostProcessing;

public class GUItesting : MonoBehaviour {

    //variables for post-processing
    [SerializeField] PostProcessingProfile volume;

    public Color colorFull;
    public Color colorEmpty;
    public Color colorHalf;

    //postProcessing trauma variables
    float trauma = 0;
    float traumaDecrease = 0.05f;
    float hitTrauma = 1.5f;
    static float MAX_TRAUMA = 20f;
    bool traumaRunning = false;

    //testing variables
    float health;
    float damage = 15;

    //Variables for the GUI
    private MainGUI testGui;
    private GuiToolset guiTools;

    // Use this for initialization
    void Start() {

        volume = Resources.Load<PostProcessingProfile>("TestPostProcessing");
        colorHalf = (colorFull + colorEmpty)/2;

        int startingClips = 4;
        int bulletsPerClip = 12;
        int maxClips = 6;
        int maxHealth = 300;
        health = maxHealth;

        guiTools = GameObject.FindGameObjectWithTag("GUI").GetComponent<GuiToolset>();
        testGui = new MainGUI(maxClips, startingClips, bulletsPerClip, maxHealth, guiTools);

        PostProcessEnd();//makes sure postProcessing is deactivated on startup
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
            if(health >=0)
            {
                health -= damage;
            }
            testGui.updateHealthBar(health);
            //Trying to add a slightly more exponential trauma increase (TODO - Tweak to perfection)
            trauma += trauma + hitTrauma;
            if(trauma>MAX_TRAUMA)
            {
                //trauma cannot go past MAX_TRAUMA
                trauma = MAX_TRAUMA;
            }
            if(!traumaRunning)
            {
                StartCoroutine(Trauma());
            }
            Debug.Log("Player hit");
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
        float intensity = 1 - ((MAX_TRAUMA - trauma) / MAX_TRAUMA);//Returns a 0-1 scale for trauma over max trauma

        //pulling setting from postProcessing
        var bloom = volume.bloom.settings;
        var chroma = volume.chromaticAberration.settings;
        var vignette = volume.vignette.settings;
        var grain = volume.grain.settings;

        //modifying settings
        bloom.bloom.intensity = intensity*3;
        chroma.intensity = intensity*2;
        vignette.intensity = Mathf.Clamp(intensity*2,0,0.6f);
        grain.intensity = intensity*0.3f;

        //returning modified settings to postProcessing
        volume.bloom.settings = bloom;
        volume.chromaticAberration.settings = chroma;
        volume.vignette.settings = vignette;
        volume.grain.settings = grain;

    }
    public void PostProcessEnd()
    {
        //deactivates all trauma postProcessing after it's used
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
            trauma -= Mathf.Sqrt(trauma) * traumaDecrease; //this is my attempt to have an exponential decrease in trauma
            //I want the hard trauma to drop off pretty quick, but the soft stuff to linger just a bit
            yield return null;
        }
        trauma = 0;
        PostProcessEnd();
        //basic bool check for whether or not the coroutine is already running
        traumaRunning = false;
        yield return null;
    }
}
