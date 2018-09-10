using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMechanics : ActorMechanics {

    // this will boil down to just being the player's name
    private string entityName;
    public string EntityName {
        get { return entityName; }
        set { entityName = value; }
    }

    // Weapon variables
    protected enum SelectedWeapon { Primary, Secondary };
    protected SelectedWeapon MySelectedWeapon = SelectedWeapon.Primary;
    protected bool fire_JustPressed;
    protected bool fire_BeingPressed;

    // this is a parallel array system. Because an array can't hold two different types
    // of variables for each column, we will use this. A dictionary cannot have two of
    // the same key either, which is something we need to do (two legs, two arms).
    private BodyPartType[] bodyPartTypes = new BodyPartType[6]
    {
        BodyPartType.Head,
        BodyPartType.Torso,
        BodyPartType.Arm,
        BodyPartType.Arm,
        BodyPartType.Leg,
        BodyPartType.Leg
    };

    private BodyPart[] bodyParts = new BodyPart[6]
    {
        null,
        null,
        null,
        null,
        null,
        null
    };

    private void Start()
    {

        AddBodyPart(BodyPartFactory.Lookup("Rabbit Leg"));
        AddBodyPart(BodyPartFactory.Lookup("Bucket Head")); // doesn't exist
        

        // debugging to make sure that all body parts are filled with the test defaults

        for (int i = 0; i < bodyPartTypes.Length; i++)
        {
            if (bodyParts[i] != null)
            {
                Debug.Log("In slot " + bodyPartTypes[i].ToString() + " you're wearing " + bodyParts[i].name);
            }
            else
            {
                Debug.Log("In slot " + bodyPartTypes[i].ToString() + " you're not wearing anything");
            }
        }

    }
    
    private void Update()
    {
        //transform.parent.position = transform.position - transform.localPosition;
        // Fire weapon inputs
        fire_BeingPressed = Input.GetMouseButton(0);
        fire_JustPressed = Input.GetMouseButtonDown(0);
        
        // Overriden weapon function inputs
        if (fire_JustPressed || fire_BeingPressed) { FireWeapon(); }
        if (Input.GetButtonDown("Reload")) { ReloadWeapon(); }
        if (Input.GetButtonDown("Switch Weapon")) { SwitchWeapon(); }
    }

    // automatically finds which slot this bodyPart should go into and replaces it.
    public void AddBodyPart(BodyPart bodyPart)
    {
        if (bodyPart != null)
        {
            BodyPartType type = bodyPart.BodyPartType;
            int slot = 0;
            switch (type)
            {
                case BodyPartType.Head: slot = 0; break;
                case BodyPartType.Torso: slot = 1; break;
                case BodyPartType.Arm:
                    if (bodyParts[2] != null)
                    {
                        slot = 3;
                    }
                    else
                    {
                        slot = 2;
                    }
                    break;
                case BodyPartType.Leg:
                    if (bodyParts[4] != null)
                    {
                        slot = 5;
                    }
                    else
                    {
                        slot = 4;
                    }
                    break;
                default: break;
            }
            bodyParts[slot] = bodyPart;
        }
        else
        {
            Debug.Log("Not a valid bodyPart. was null. can't equip");
        }
    }

    #region Weapon functions

    //Overriden weapon functions
    protected virtual void SwitchWeapon()
    {

    }

    protected virtual void ReloadWeapon()
    {

    }

    protected virtual void FireWeapon()
    {

    }
    #endregion

}
