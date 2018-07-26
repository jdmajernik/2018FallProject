using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Class { Soldier };

public class N_C_Soldier : N_PlayerMechanics {

    protected Class MyClass = Class.Soldier;

    // Primary weapon variables
    private bool    P_Automatic       = true;
    private float   P_RateOfFire      = 3f;
    private float   P_ReloadTime      = 1.5f;
    private bool    P_Reloading       = false;
    private bool    P_CanFire         = true;
    private int     P_ClipSize        = 100;
    private int     P_Ammo            = 100;
    private int     P_Range           = 999;

    // Secondary weapon variables
    private bool    S_Automatic       = false;
    private float   S_RateOfFire      = 3f;
    private float   S_RechargeRate    = 5f; //per sec
    private bool    S_CanFire         = true;
    private int     S_ClipSize        = 15;
    private int     S_Ammo            = 15;
    private int     S_Range           = 999;

    protected override void FireWeapon() {
        switch (MySelectedWeapon) {

            case SelectedWeapon.Primary:
                if (P_CanFire) {
                    if (P_Automatic && Fire_BeingPressed) {
                        FirePrimary();
                    } else if (!P_Automatic && Fire_JustPressed) {
                        FirePrimary();
                    }
                } break;

            case SelectedWeapon.Secondary:
                if (S_Automatic && Fire_BeingPressed) {
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
                if (!P_Reloading && P_CanFire) {
                    StartCoroutine(PrimaryReload());
                } else {
                    print("Can't reload right now");
                } break;
            case SelectedWeapon.Secondary:
                if (!P_Reloading && P_CanFire) {
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
        if (P_CanFire && !P_Reloading) {

            StartCoroutine(PrimaryRof());

            if (Fire_JustPressed) {
                // special effect for when you first start firing?
            }

            // Laser gun
            Vector3 hit_point;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit ray_hit;
            if (Physics.Raycast(ray, out ray_hit, P_Range)) {
                hit_point = ray_hit.point;
            } else {
                hit_point = ray.origin + ray.direction * P_Range;
            }

            if (hit_point != null) {
                if (P_Ammo > 0) {
                    print("Shot laser at " + hit_point + ". Ammo: " + P_Ammo);
                    P_Ammo -= 1;
                } else {
                    StartCoroutine(PrimaryReload());
                }
            }

        }
    }

    IEnumerator PrimaryReload() {
        print("reloading primary weapon");
        P_Reloading = true;
        yield return new WaitForSeconds(1 / P_ReloadTime);
        P_Reloading = false;
    }
    #endregion

    #region Secondary Weapon
    private void FireSecondary() {
        print("fizzed secondary weapon");
    }
    #endregion

    IEnumerator PrimaryRof() {
        P_CanFire = false;
        yield return new WaitForSeconds(1 / P_RateOfFire);
        P_CanFire = true;
    }
    
    IEnumerator SecondaryRof() {
        S_CanFire = false;
        yield return new WaitForSeconds(1 / S_RateOfFire);
        S_CanFire = true;
    }

    public Class GetClass() {
        return MyClass;
    }
}
