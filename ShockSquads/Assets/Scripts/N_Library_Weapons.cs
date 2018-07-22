using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public delegate void FireDelegate(Vector3 barrel, Vector3 aim);
public enum WeaponSlotType { Primary, Secondary };
public enum Class { Soldier, Sniper };

public class N_Library_Weapons : MonoBehaviour {

	public static Dictionary<WeaponSlotType, FireDelegate> GetClassWeapons(Class AskingClass)
    {
        Dictionary<WeaponSlotType, FireDelegate> FireDelegates = new Dictionary<WeaponSlotType, FireDelegate>();
        switch (AskingClass) {

            case Class.Soldier:
                FireDelegates.Add(WeaponSlotType.Primary, FireLaser);
                FireDelegates.Add(WeaponSlotType.Secondary, FireMiniLaser);
                break;

            case Class.Sniper:
                FireDelegates.Add(WeaponSlotType.Primary, FireMiniLaser);
                break;

            default: break;
        }
        return FireDelegates;
    }

    private static void FireLaser(Vector3 barrel, Vector3 aim)
    {
        print("fired laser at " + aim);
    }

    private static void FireMiniLaser(Vector3 barrel, Vector3 aim)
    {
        print("fired mini laser at " + aim);
    }
}
*/