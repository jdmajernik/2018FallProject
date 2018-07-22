using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Class { Soldier };

public class N_C_Soldier : N_PlayerMechanics {

    protected Class MyClass = Class.Soldier;

    // Primary weapon variables
    private bool    Primary_Automatic       = true;
    private float   Primary_RateOfFire      = 3f;
    private float   Primary_ReloadTime      = 1.5f;
    private bool    Primary_Reloading       = false;
    private bool    Primary_CanFire         = true;
    private int     Primary_ClipSize        = 100;
    private int     Primary_Ammo            = 100;

    // Secondary weapon variables
    private bool    Secondary_Automatic     = false;
    private float   Secondary_RateOfFire    = 3f;
    private float   Secondary_RechargeRate  = 5f; //per sec
    private bool    Secondary_CanFire       = true;
    private int     Secondary_ClipSize      = 15;
    private int     Secondary_Ammo          = 15;

    protected override void FireWeapon() {
        switch (MySelectedWeapon) {

            case SelectedWeapon.Primary:
                if (Primary_CanFire) {
                    if (Primary_Automatic && Fire_BeingPressed) {
                        FirePrimary();
                    } else if (!Primary_Automatic && Fire_JustPressed) {
                        FirePrimary();
                    }
                } break;

            case SelectedWeapon.Secondary:
                if (Secondary_Automatic && Fire_BeingPressed) {
                        FireSecondary();
                } else if (Fire_JustPressed && Fire_JustPressed) {
                        FireSecondary();
                } break;

            default:
                print("You don't have a weapon for that slot");
                break;
        }
    }

    protected override void ReloadWeapon() {
        switch (MySelectedWeapon) {

            case SelectedWeapon.Primary:
                if (!Primary_Reloading && Primary_CanFire) {
                    StartCoroutine(PrimaryReload());
                } else {
                    print("Can't reload right now");
                } break;
            case SelectedWeapon.Secondary:
                if (!Primary_Reloading && Primary_CanFire) {
                    StartCoroutine(PrimaryReload());
                } else {
                    print("Can't reload right now");
                } break;
            default:
                print("Can't reload that type of weapon");
                break;
        }
    }

    protected override void SwitchWeapon() {
        switch (MySelectedWeapon) {
            case SelectedWeapon.Primary: MySelectedWeapon = SelectedWeapon.Secondary; break;
            case SelectedWeapon.Secondary: MySelectedWeapon = SelectedWeapon.Primary; break;
            default: break;
        }
        print("Switched weapons to " + MySelectedWeapon.ToString());
    }

    #region Primary Weapon
    private void FirePrimary() {
        if (Primary_CanFire && !Primary_Reloading) {

            StartCoroutine(PrimaryRof());

            if (Fire_JustPressed) {
                // special effect for when you first start firing?
            }

            // Laser gun
            var ray = MyCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit ray_hit;
            if (Physics.Raycast(ray, out ray_hit)) {
                if (Primary_Ammo > 0) {
                    print("Shot laser at " + ray_hit.point + ". Ammo: " + Primary_Ammo);
                    Primary_Ammo -= 1;
                } else {
                    StartCoroutine(PrimaryReload());
                }
            }
        }
    }

    IEnumerator PrimaryReload() {
        print("reloading primary weapon");
        Primary_Reloading = true;
        yield return new WaitForSeconds(1 / Primary_ReloadTime);
        Primary_Reloading = false;
    }
    #endregion

    #region Secondary Weapon
    private void FireSecondary() {
        print("fizzed secondary weapon");
    }
    #endregion

    IEnumerator PrimaryRof() {
        Primary_CanFire = false;
        yield return new WaitForSeconds(1 / Primary_RateOfFire);
        Primary_CanFire = true;
    }
    
    IEnumerator SecondaryRof() {
        Secondary_CanFire = false;
        yield return new WaitForSeconds(1 / Secondary_RateOfFire);
        Secondary_CanFire = true;
    }

    public Class GetClass() {
        return MyClass;
    }
}
