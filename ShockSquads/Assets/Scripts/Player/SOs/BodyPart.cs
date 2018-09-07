using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void AbilityDelegate();
// delegates are essentially variables that hold a function. In this case, we use a delegate to hold
// code for an ability. You have to initialize it with a name cuz you can't just use delegate. delegate is a value type.

public enum AbilityType { None, Jump, Fire }
public enum BodyPartType { Head, Torso, RightArm, LeftArm, RightLeg, LeftLeg }

public class BodyPart {

    // name
    protected string bodyPartName;
    public string BodyPartName
    {
        get { return bodyPartName; }
        set { bodyPartName = value; }
    }

    // type
    protected BodyPartType bodyPartType;
    public BodyPartType BodyPartType
    {
        get { return bodyPartType; }
        set { bodyPartType = value; }
    }

    // abilities
    // here we use a dictionary to store any abilities that we may have. It's a dictionary so we can store a 
    // cooresponding 'key' to each delegate 'value'. The key is what type of ability it is. If the dictionary is 
    // empty, then you know this item doesn't have any abilities. If it has multiple, boom, no problem.
    protected Dictionary<AbilityType, AbilityDelegate> myAbilities = new Dictionary<AbilityType, AbilityDelegate>();
    public Dictionary<AbilityType, AbilityDelegate> MyAbilities
    {
        get { return myAbilities; }
        set { /* Never set this list! */ }
    }

    // this is a constructor that redirects to a virtual function
    // that way we can override the virtual function in anything that
    // inherits from this class.
    public BodyPart() { Constructor(); }
    public virtual void Constructor() { }
    
}
