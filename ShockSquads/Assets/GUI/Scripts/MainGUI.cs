using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShockSquadsGUI
{
    public class MainGUI {
        /// <summary>
        /// Controls the main GUI for the GAMEPLAY
        /// </summary>
        GuiToolset GuiTools = new GuiToolset();
        
        [SerializeField] private GameObject ammoBar;

        [SerializeField] private GameObject guiClip;
        private List<GameObject> guiClips = new List<GameObject>();


        private float maxClips; //the max amount of clips
        private float bulletsPerClip; //the amount of bullets in a clip

        private float totalBullets; //the total amount of bullets in all clips
        private float totalClips; //REMOVABLE the current amount of clips

        // Use this for initialization
        public MainGUI(int newMaxClips, int startingClips, int newBulletsPerClip, GameObject newGuiClip)
        {
            //sets variables from inputs
            maxClips = newMaxClips;
            bulletsPerClip = newBulletsPerClip;

            //sets the other private variables
            ammoBar = GameObject.FindGameObjectWithTag("AmmoBar");

            guiClip = Resources.Load<GameObject>("AmmoClip"); //TODO - DELETE this is just to test the bar decreasing as you fire

            //calculates the total number of bullets
            totalBullets = bulletsPerClip * startingClips;
            totalClips = startingClips;//REMOVABLE I can calculate this later
            //adds starting clips to the GUI
            AddClips(startingClips);
        }
        public void Fire()
        {
            //defaults to one bullet fired per firing call
            float bulletPercentage = (totalBullets - (bulletsPerClip * (totalClips - 1))) / bulletsPerClip;
            Debug.Log("Removing a bullet - " + bulletPercentage);
            totalBullets--;
            guiClip.transform.GetChild(0).gameObject.GetComponent<Image>().fillAmount = (totalBullets - (bulletsPerClip * (totalClips - 1)))/bulletsPerClip;
        }
        public void Fire(float bulletsFired)
        {
            //takes input for how many bullets fired per firing call
        }
        public void AddClip()
        {
            //adds a clip to the ammo bar
        }
        public void AddClips(int numClips)
        {
            //adds a set amount of clips to the ammo bar
            for (int a = 0; a<numClips; a++)
            {
                guiClips.Add(GuiTools.createObject(guiClip, ammoBar));
            }
        }
    }
    public class GuiToolset : MonoBehaviour
    {
        //Exists to utilize the MonoBehaviour specific functions (cough, cough, Instantiate, cough) while being able to have a constructor
        //Whats that? Have my cake? Eat it too??? Don't mind if I do!!!
        public GameObject createObject (GameObject newObject, GameObject parent)
        {
            return Instantiate(newObject , parent.transform);
        }
    }
}
