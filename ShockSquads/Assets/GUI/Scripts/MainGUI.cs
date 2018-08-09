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
        GuiToolset GuiTools;
        
        [SerializeField] private GameObject ammoBar;

        [SerializeField] private GameObject guiClip;
        private List<GameObject> guiClips = new List<GameObject>();


        private float maxClips; //the max amount of clips
        private float bulletsPerClip; //the amount of bullets in a clip

        private float totalBullets; //the total amount of bullets in all clips
        private float totalClips; //REMOVABLE the current amount of clips

        // Use this for initialization
        public MainGUI(int newMaxClips, int startingClips, int newBulletsPerClip, GuiToolset newGuiTools)
        {
            //sets variables from inputs
            maxClips = newMaxClips;
            bulletsPerClip = newBulletsPerClip;
            GuiTools = newGuiTools;

            //sets the other private variables
            ammoBar = GameObject.FindGameObjectWithTag("AmmoBar");

            guiClip = Resources.Load<GameObject>("AmmoClip"); //TODO - DELETE this is just to test the bar decreasing as you fire

            //calculates the total number of bullets
            //adds starting clips to the GUI
            AddClips(startingClips);
            totalBullets = totalClips * bulletsPerClip;
            ReCenterClips();//alligns all the GUI elements
        }
        public void Fire()
        {
            //defaults to one bullet fired per firing call
            float bulletPercentage = (totalBullets - (bulletsPerClip * (totalClips - 1))) / bulletsPerClip;
           
            if (bulletPercentage >= 0)
            {
                Debug.Log("Removing a bullet - " + bulletPercentage);
                totalBullets--;
                guiClips[guiClips.Count - 1].transform.GetChild(0).gameObject.GetComponent<Image>().fillAmount = bulletPercentage; //this is a long one, isn't it?
            }
        }
        public void Fire(float bulletsFired)
        {
            //takes input for how many bullets fired per firing call
            float bulletPercentage = (totalBullets - (bulletsPerClip * (totalClips - 1))) / bulletsPerClip;
           
            if (bulletPercentage >= 0)
            {
                Debug.Log("Removing a bullet - " + bulletPercentage);
                totalBullets -= bulletsFired;
                guiClips[guiClips.Count - 1].transform.GetChild(0).gameObject.GetComponent<Image>().fillAmount = bulletPercentage; //this is a long one, isn't it?
            }
        }
        public void Reload()
        {
            GuiTools.ReloadAnimation(guiClips[guiClips.Count - 1]);
            guiClips.RemoveAt(guiClips.Count - 1);
            totalClips--;
            totalBullets = totalClips * bulletsPerClip;
        }
        public void AddClip()
        {
            //adds a clip to the ammo bar
            if(totalClips<maxClips)
            {
                guiClips.Insert(0, GuiTools.CreateObject(guiClip, ammoBar));
                totalClips = guiClips.Count;
                totalBullets += bulletsPerClip; //just adding one clip
            }
            
        }
        public void AddClips(float numClips)
        {
            //adds a set amount of clips to the ammo bar
            if (totalClips != maxClips)
            {
                if (numClips + totalClips > maxClips)
                {
                    numClips = maxClips - totalClips;
                }
                for (int a = 0; a < numClips; a++)
                {
                    guiClips.Insert(0, GuiTools.CreateObject(guiClip, ammoBar));
                }
                totalClips = guiClips.Count;
                totalBullets += numClips * bulletsPerClip;
            }
        }
        public void ReCenterClips ()
        {
            //This re-centers the clips int the middle of the AmmoBar gameObject
            float buffer = 9; //the buffer between the gui widths(since it's diagonal, the actual width is off)
            float totalClips = guiClips.Count;
            float clipWidth = guiClip.GetComponent<RectTransform>().rect.width - buffer; //the width of the gui object

            Vector3 newPos = new Vector3(0, 0, 0);
            Debug.Log("Total Clips - " + totalClips);
            for(int a = 0; a<guiClips.Count; a++)
            {
                GameObject thisClip = guiClips[a];
                float aa = a;// I do this BECAUSE INT AND FLOAT CALCULATIONS DON'T ALWAYS WORK!!!!!!! 
                newPos.x = (aa * (clipWidth)) - ((clipWidth * totalClips) / 2);
                Debug.Log("new position - [" + aa + "]" + newPos.x);
                thisClip.transform.position = newPos + thisClip.transform.parent.position;//updates position
            }
        }
    }
   
}
