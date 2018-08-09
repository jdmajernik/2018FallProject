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
        public MainGUI(int newMaxClips, int startingClips, int newBulletsPerClip)
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
            ReCenterClips();//alligns all the GUI elements
        }
        public void Fire()
        {
            //defaults to one bullet fired per firing call
            float bulletPercentage = (totalBullets - (bulletsPerClip * (totalClips - 1))) / bulletsPerClip;
            Debug.Log("Removing a bullet - " + bulletPercentage);
            if (bulletPercentage > 0)
            {
                totalBullets--;
                guiClips[guiClips.Count - 1].transform.GetChild(0).gameObject.GetComponent<Image>().fillAmount = bulletPercentage; //this is a long one, isn't it?
            }
        }
        public void Fire(float bulletsFired)
        {
            //takes input for how many bullets fired per firing call
            float bulletPercentage = (totalBullets - (bulletsPerClip * (totalClips - 1))) / bulletsPerClip;
            Debug.Log("Removing a bullet - " + bulletPercentage);
            if (bulletPercentage > 0)
            {
                totalBullets-= bulletsFired;
                guiClips[guiClips.Count - 1].transform.GetChild(0).gameObject.GetComponent<Image>().fillAmount = bulletPercentage; //this is a long one, isn't it?
            }
        }
        public void Reload()
        {
            GuiTools.removeObject(guiClips[guiClips.Count - 1]);
            guiClips.RemoveAt(guiClips.Count - 1);
            totalClips--;
            totalBullets = totalClips * bulletsPerClip;
        }
        public void AddClip()
        {
            //adds a clip to the ammo bar
            if(totalClips<maxClips)
            {
                guiClips.Insert(0, GuiTools.createObject(guiClip, ammoBar));
            }
        }
        public void AddClips(float numClips)
        {
            //adds a set amount of clips to the ammo bar
            if(numClips + totalClips > maxClips)
            {
                numClips = maxClips - totalClips;
            }
            for (int a = 0; a<numClips; a++)
            {
                guiClips.Insert(0,GuiTools.createObject(guiClip, ammoBar));
            }
        }
        public void ReCenterClips ()
        {
            //This re-centers the clips int the middle of the AmmoBar gameObject
            float buffer = 9; //the buffer between the gui widths(since it's diagonal, the actual width is off)
            float totalClips = guiClips.Count;
            float clipWidth = guiClip.GetComponent<RectTransform>().rect.width - buffer; //the width of the gui object

            Vector3 newPos = new Vector3(0, 0, 0);

            for(int a = 0; a<guiClips.Count; a++)
            {
                float aa = a;// I do this BECAUSE INT AND FLOAT CALCULATIONS DON'T ALWAYS WORK!!!!!!! 
                newPos.x = (aa * (clipWidth)) - ((clipWidth * totalClips) / 2);
                guiClips[a].transform.position += newPos;//updates position
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
        public void removeObject (GameObject removedObject)
        {
            Destroy(removedObject);
        }
    }
}
