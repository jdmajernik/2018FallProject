using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W_PipBomb : WeaponMechanics {

    bool isAI;

    bool automatic = true;

    float rof = 1.5f;

    [SerializeField] GameObject prefab;

    WeaponMechanics weapon_mechanics;

    GameObject weapon;
    GameObject barrel;

    void Start()
    {
        creator = gameObject;

        weapon = transform.Find("Weapon").gameObject;
        if (weapon != null)
        {
            barrel = weapon.transform.Find("Barrel").gameObject;
            if (barrel == null)
            {
                print("COULD NOT FIND BARREL. FATAL ERROR");
            }
        }
        else
        {
            print("COULD NOT FIND WEAPON. FATAL ERROR");
        }

        //define weapon type for WeaponMechanics
        weapon_mechanics = GetComponent<WeaponMechanics>();
        if (weapon_mechanics)
        {
            weapon_mechanics.weapon_type = this;
        }
        else
        {
            print("Couldn't find WeaponMechanics");
        }

        if (gameObject.tag == "Player")
        {
            isAI = false;
        }
        else
        {
            isAI = true;
        }
    }

    public override void Fire(Vector3 aimPosition)
    {
        if (base.canShoot)
        {
            base.Rof(rof);

            var bullet = Instantiate(prefab, barrel.transform.position, Quaternion.LookRotation(aimPosition - barrel.transform.position, Vector3.up));
            if (isAI)
            {
                bullet.GetComponent<Proj_PipBomb>().good_team = gameObject.GetComponent<AI_Enemy>().currentteam;
            }
            else
            {
                bullet.GetComponent<Proj_PipBomb>().good_team = gameObject.GetComponent<PlayerController>().currentteam;
            }
        }
    }
}
