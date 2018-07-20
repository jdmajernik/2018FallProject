using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W_AR : WeaponMechanics {

    public Team good_team;

    bool isAI;

    bool automatic = true;

    float rof = 9f;
    float range = 250f;
    float damage = 23f;

    float bloom;
    float bloom_min = 3f;
    float bloom_max = 9f;
    float bloom_mult = 0.965f;
    float bloom_tick = 1.5f;
    bool bloom_cd = true;
    
    [SerializeField] Material mat_bullet;
    [SerializeField] float bullet_width = 0.15f;

    WeaponMechanics weapon_mechanics;

    GameObject weapon;
    GameObject barrel;

    void Start()
    {
        creator = gameObject;

        //Start weapon with max bloom to prevent the 100% accuracy issue
        bloom = bloom_max;

        if (tag == "Player")
        {
            isAI = false;
            good_team = gameObject.GetComponent<PlayerController>().currentteam;
        }
        else if (tag == "Enemy")
        {
            isAI = true;
            good_team = gameObject.GetComponent<AI_Enemy>().currentteam;
        }

        weapon = transform.Find("Weapon").gameObject;
        if (weapon != null)
        {
            barrel = weapon.transform.Find("Barrel").gameObject;
        }

        //define weapon type for WeaponMechanics
        weapon_mechanics = GetComponent<WeaponMechanics>();
        if (weapon_mechanics)
        {
            weapon_mechanics.weapon_type = this;
        }
    }

    void Update()
    {
        //bloom control
        if (canShoot)
        {
            if (bloom * bloom_mult > bloom_min)
            {
                bloom *= bloom_mult;
            }
            else
            {
                if (bloom != bloom_min)
                {
                    bloom = bloom_min;
                }
            }
        }
        //print("BLOOM : " + bloom.ToString());
    }

    private LineRenderer GetLineRenderer()
    {
        var lr = gameObject.GetComponent<LineRenderer>();
        if (!lr)
        {
            var new_lr = gameObject.AddComponent<LineRenderer>();

            //settings
            new_lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            new_lr.material = mat_bullet;
            new_lr.startWidth = 0f;
            new_lr.endWidth = bullet_width;
            new_lr.positionCount = 2;

            new_lr.numCapVertices = 1;

            return new_lr;
        }
        else
        {
            return lr;
        }
    }

    void ResetLine()
    {
        var lr = GetLineRenderer();
        lr.SetPosition(0, new Vector3(0,0,0));
        lr.SetPosition(1, new Vector3(0,0,0));
    }

    public override void Fire(Vector3 aimPosition)
    {
        if (canShoot)
        {
            Rof(rof);

            #region Add Bloom

            if (bloom + bloom_tick < bloom_max)
            {
                bloom += bloom_tick;
            }
            else
            {
                if (bloom != bloom_max)
                {
                    bloom = bloom_max;
                }
            }

            #endregion

            //Create direction for Raycast
            var direction = (aimPosition - barrel.transform.position).normalized;

            //Add bloom to bullet angle
            direction += AddNoiseOnAngle(bloom);

            //Create hit point for LineRenderer
            var hitpoint = aimPosition;

            //Raycast bullet
            RaycastHit hit;
            if (Physics.Raycast(barrel.transform.position, direction, out hit, range))
            {
                print("hit " + hit.collider.transform.root.gameObject.name);
                hitpoint = hit.point;

                if (hit.collider.transform.root.tag == "Player")
                {
                    if (hit.collider.transform.root.GetComponent<PlayerController>().currentteam != good_team)
                    {
                        hit.collider.transform.root.GetComponent<PlayerController>().TakeDamage(damage, transform.position);
                    }
                }
                if (hit.collider.transform.root.tag == "Enemy")
                {
                    if (hit.collider.transform.root.GetComponent<AI_Enemy>().currentteam != good_team)
                    {
                        hit.collider.transform.root.GetComponent<AI_Enemy>().TakeDamage(damage, transform.position);
                    }
                    else
                    {
                        //This is a teammate
                    }
                }
            }
            else
            {
                //Set hitpoint to the direction of mouse from camera
                hitpoint = barrel.transform.position + (direction * range);
            }

            //Draw line in game
            var lr = GetLineRenderer();
            lr.SetPosition(0, barrel.transform.position);
            lr.SetPosition(1, hitpoint);

            Invoke("ResetLine", ((1 / rof) / 2));
        }
    }
}
