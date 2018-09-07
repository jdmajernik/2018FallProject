using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMechanics : ActorMechanics {

    // Requires
    
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

    // this dictionary holds each of the currently worn body parts. The size of this
    // will never change, because you can only have 1 of each body part obvi. You only ever
    // need to set the values in this dictionary, the keys are there to access the right spots.
    private Dictionary<BodyPartType, BodyPart> bodyParts = new Dictionary<BodyPartType, BodyPart>()
    {
        { BodyPartType.Head, null },
        { BodyPartType.Torso, null },
        { BodyPartType.RightArm, null },
        { BodyPartType.LeftArm, null },
        { BodyPartType.RightLeg, null },
        { BodyPartType.LeftLeg, null }
    };

    private void Start()
    {
        AddBodyPart(new BP.RabbitLeg());
        AddBodyPart(new BP.PegLeg());
        AddBodyPart(new BP.Flail());
        AddBodyPart(new BP.MonsterHand());
        AddBodyPart(new BP.FatBelly());
        AddBodyPart(new BP.Bucket());

        // debugging to make sure that all body parts are filled with the test defaults
        foreach (BodyPart bodyPart in bodyParts.Values)
        {
            Debug.Log("Wearing part " + bodyPart.ToString());
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
        BodyPartType type = bodyPart.BodyPartType;
        bodyParts[type] = bodyPart;
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
