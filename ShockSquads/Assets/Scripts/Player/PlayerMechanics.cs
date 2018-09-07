using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMechanics : ActorMechanics {

    // Requires
    
    //[SerializeField] protected string PlayerName;
    private string EntityName = "Tester114";
    
    // Weapon variables
    protected enum SelectedWeapon { Primary, Secondary };
    protected SelectedWeapon MySelectedWeapon = SelectedWeapon.Primary;
    protected bool Fire_JustPressed;
    protected bool Fire_BeingPressed;
    
    private void Start()
    {

    }
    
    private void Update() {
    
        //transform.parent.position = transform.position - transform.localPosition;
        // Fire weapon inputs
        Fire_BeingPressed = Input.GetMouseButton(0);
        Fire_JustPressed = Input.GetMouseButtonDown(0);
        
        // Overriden weapon function inputs
        if (Fire_JustPressed || Fire_BeingPressed) { FireWeapon(); }
        if (Input.GetButtonDown("Reload")) { ReloadWeapon(); }
        if (Input.GetButtonDown("Switch Weapon")) { SwitchWeapon(); }
    }

    //Overriden weapon functions
    protected virtual void SwitchWeapon() { }
    protected virtual void ReloadWeapon() { }
    protected virtual void FireWeapon() { }
    
    public string GetName() {
        return EntityName;
    }
}
