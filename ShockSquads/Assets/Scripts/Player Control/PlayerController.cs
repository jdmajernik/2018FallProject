using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerController : ControllerMechanics {

    private NavMeshAgent agent;
    private GameObject movementEmpty;
    private GameObject cameraEmpty;
    private CharacterController controller;

    bool grounded = false;
    float movespeed = 8f;
    float jumppower = 25f;
    float gravity = 1.5f; //raw grav value
    float gravityscale = 1f; //multiplier

    float lastvspeed;

    Rigidbody rb;
    GameObject weapon;
    GameObject barrel;

    [SerializeField] Text respawn_timer;
    int time_left = 0;

    public void ShowRespawnTimer()
    {
        time_left = 10;
        StartCoroutine(TimerTick());
    }

    IEnumerator TimerTick()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            respawn_timer.text = "Respawning in .." + time_left;
            time_left--;
        }
    }

    public void Restart()
    {
        respawn_timer.text = "";
        Start();
    }

    void Start ()
    {
        agent = GetComponent<NavMeshAgent>();
        movementEmpty = GameObject.Find("Movement_Empty");
        cameraEmpty = GameObject.Find("Camera_Empty");
        controller = GetComponent<CharacterController>();

        //controller_mechanics = GetComponent<ControllerMechanics>();

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

        //set team
        currentteam = Team.red;
        good_team = currentteam;
    }

    void Update ()
    {
        //Debug.DrawRay(barrel.transform.position, barrel.transform.forward);

        if (Input.GetMouseButton(0)) //AUTOMATIC FIRE
        {
            var weapon_mechanics = GetComponent<WeaponMechanics>();
            if (weapon_mechanics)
            {
                var weapon = weapon_mechanics.weapon_type;
                if (weapon)
                {
                    //var aimPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
                    //aimPosition = Camera.main.ScreenToWorldPoint(aimPosition);

                    //raycast cam to mouse pos
                    /*
                    RaycastHit hit;
                    if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 250f, 1 << 0))
                    {
                        //use hit point to raycast from barrel
                        RaycastHit barrel_hit;
                        if (Physics.Raycast(barrel.transform.position, hit.point, out barrel_hit, 250f, 1 << 0))
                        {
                            //if barrel raycast is blocked then display no aim indicator
                            print("BARREL BLOCKED");
                            weapon.Fire(barrel_hit.point);
                        }
                        else
                        {
                            //barrel is unblocked
                            weapon.Fire(barrel_hit.point);
                        }
                    }

                    */
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition); //parameter could be a vector3 like soldier 76 target on gos

                    RaycastHit ray_hit;
                    if (Physics.Raycast(ray, out ray_hit))
                    {
                        //Debug.DrawLine(barrel.transform.position, ray_hit.point, Color.red, 3f, false);

                        weapon.Fire(ray_hit.point);

                        /*
                        RaycastHit hit;
                        if (Physics.Raycast(barrel.transform.position, ray_hit.point, out hit))
                        {
                            print("HIT POINT IS " + hit.point.ToString());
                        }
                        else
                        {

                        }
                        */
                    }
                }
            }
            else
            {
                print("Could't find weapon mechs");
            }
        }
    }

    void FixedUpdate ()
    {
        //Rotate to face with camera
        transform.rotation = Quaternion.Euler(0f, cameraEmpty.transform.localRotation.eulerAngles.y, 0f);


        //Define movement vectors
        Vector3 wantedPosition = Quaternion.Euler(0f, cameraEmpty.transform.localRotation.eulerAngles.y, 0f)
            * new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        wantedPosition.x = wantedPosition.x * movespeed;
        wantedPosition.z = wantedPosition.z * movespeed;


        //Jump        
        if (Input.GetKey(KeyCode.Space))
        {
            if (controller.isGrounded)
            {
                wantedPosition.y = jumppower;
            }
        }

        //Gravity
        if (!controller.isGrounded)
        {
            wantedPosition.y = lastvspeed - (gravity * gravityscale);
        }


        //Movement
        controller.Move(wantedPosition * Time.deltaTime);
        lastvspeed = wantedPosition.y;
    }

    public void TakeDamage(float amount, Vector3 pos)
    {
        TakeDamageThroughController(amount);
    }
}
