using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour {

    // Requires
    [SerializeField] private GameObject cameraEmpty;
    private CharacterController controller;
    private CapsuleCollider capsuleCollider;
    private CameraController cameraController;

    #region Global Player Movement Variables
    // Global Player Movement Variables
    private enum MovementStatus { Normal, Sprinting, Sneaking, Sliding }
    private MovementStatus MyMovementStatus = MovementStatus.Normal;

    private bool key_Sprint;

    // speeds for different movement status'
    private int speedSprint = 105; //120
    private int speedRun = 75; //90
    private int speedSneak = 30; //30
    
    private float slideControl = 0.025f; // slideControl is the amount of friction you have while sliding.
    private float slideDecay = 0.05f; // the exponential speed that the slide's speed decays at
    private float slideBoost = 2f; // movement speed boost from beginning the slide
    private bool slideReady = true;

    private float bulletBoost = 5f; // speed boost from jumping out of a slide
    private bool bulletReady = true;

    private float backpedalMultiplier = 0.75f; // multiplies your speed by this much when backpedaling. so, slower
    private float groundControl = 0.35f; // control over movement while on the ground

    private int jumpPower = 200; //200
    private float airControl = 0.035f; // control over movement while in the air

    private float gravityValue = 9.8f; // just a base number for gravity. could be a reason to change it to be Mars gravity or whatever
    private float gravityMultiplier = 1f; // multiplies gravity's force by this amount

    private float last_vspeed = 0;
    private int last_mspeed = 0;
    private Vector3 last_MovementVector;

    private Vector3 impulse_Vector;

    private bool grounded; // custom grounded
    #endregion

    private void Start() {

        //CameraEmpty = GameObject.FindGameObjectWithTag("CameraParent");
        cameraEmpty = DevTools.FindChildGameObjectWithTag(this.gameObject, "CameraParent");
        controller = GetComponent<CharacterController>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        //StartCoroutine(updatePos());
    }

    private void Update()
    {

        // Player movement inputs
        key_Sprint = Input.GetButton("Sprint");

        if (Input.GetButtonDown("Sprint")) { ChangeMovementStatus(MovementStatus.Sprinting); }
        if (Input.GetButtonUp("Sprint")) { ChangeMovementStatus(MovementStatus.Normal); }

        if (Input.GetButtonDown("Sneak"))
        {
            if (MyMovementStatus == MovementStatus.Sprinting)
            {
                if (Input.GetAxisRaw("Vertical") > 0)
                { // Can't slide backwards
                    ChangeMovementStatus(MovementStatus.Sliding);

                }
                else { ChangeMovementStatus(MovementStatus.Sneaking); }
            }
            else { ChangeMovementStatus(MovementStatus.Sneaking); }
        }
        if (Input.GetButtonUp("Sneak"))
        {
            if (key_Sprint) { ChangeMovementStatus(MovementStatus.Sprinting); }
            else { ChangeMovementStatus(MovementStatus.Normal); }
        }

        // Testing impluses
        if (Input.GetKeyDown(KeyCode.K)) { AddImpulse(new Vector3(25, 25, 25)); }
    }

    private void ChangeMovementStatus(MovementStatus NewMovementStatus)
    {
        switch (NewMovementStatus)
        {
            case MovementStatus.Sliding:
                if (slideReady)
                {
                    slideReady = false;
                    bulletReady = true;
                    MyMovementStatus = NewMovementStatus;
                    if (grounded)
                    {
                        last_MovementVector = last_MovementVector * slideBoost;
                    }
                }
                else { print("Can't slide now"); }
                break;
            default: MyMovementStatus = NewMovementStatus; break;
        }
    }

    // adds an impulse to your player. think impulse grenades from fortnite. we have that functionality :)
    public void AddImpulse(Vector3 impulse) { impulse_Vector = impulse; }

    private void FixedUpdate()
    {

        // Rotate body with camera
        transform.rotation = Quaternion.Euler(0, cameraEmpty.transform.localRotation.x, 0);

        Vector3 movementVector = Quaternion.Euler(0, cameraEmpty.transform.localRotation.eulerAngles.y, 0) *
            new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        //Vector3 ThisVelocity = Vector3.zero;

        // Gravity
        float PredictNext_vspeed = last_vspeed - ((gravityValue / 12) * gravityMultiplier);
        // above calculates the speed you will be at the end of the frame so it can determine
        // if you'll collide with the ground this frame. later in this code it will set your
        // actual speed to this speed, so it's an actual calculation not just a guess. this will
        // 100% be your speed at the end of this FixedUpdate().

        // fake grounded collision because CharacterController.isGrounded is a joke!
        RaycastHit groundHit;
        grounded = false;
        // sphereCast instead of a capsuleCast because it's faster, and we don't need to calculate if there is ground
        // below the head of the player but also above the feet.. that's an issue for another day.
        if (Physics.SphereCast(new Ray(transform.position, Vector3.down), 0.4f, out groundHit, 0.78f - (PredictNext_vspeed * Time.fixedDeltaTime)))
        {
            if (Vector3.Angle(groundHit.normal, Vector3.up) <= controller.slopeLimit)
            { // Slope handling
                grounded = true;
                if (slideReady != true) { slideReady = true; }
                transform.position = new Vector3(transform.position.x, groundHit.point.y + 1f + controller.skinWidth, transform.position.z);
                //print("Transform is being set to " + new Vector3(transform.position.x, GroundHit.point.y + 1f + Controller.skinWidth, transform.position.z));
                //print("Transform is currently set to " + transform.position);
            }
        }

        if (grounded)
        { // If grounded, apply MovementStatus speed to movement
            int thisSpeed;
            switch (MyMovementStatus)
            {
                case MovementStatus.Normal: thisSpeed = speedRun; break;
                case MovementStatus.Sprinting: thisSpeed = speedSprint; break;
                case MovementStatus.Sneaking: thisSpeed = speedSneak; break;
                default: thisSpeed = speedRun; break;
            }
            if (Input.GetAxisRaw("Vertical") < 0)
            { // Backpedaling penality
                if (MyMovementStatus == MovementStatus.Sliding) { ChangeMovementStatus(MovementStatus.Normal); }
                if (MyMovementStatus == MovementStatus.Sprinting) { thisSpeed = (int)(speedRun * backpedalMultiplier); } // Can't sprint backwards
                else { thisSpeed = (int)(thisSpeed * backpedalMultiplier); }
            }
            movementVector.x *= (thisSpeed / 10);
            movementVector.z *= (thisSpeed / 10);
            last_mspeed = thisSpeed;
        }
        else
        { // If not grounded, use the last applied MovementStatus speed to movement
            movementVector.x *= (last_mspeed / 10);
            movementVector.z *= (last_mspeed / 10);
        }

        if (Input.GetButton("Jump"))
        {
            if (grounded) // only jump if you're on the ground.. duh
            {
                movementVector.y = (jumpPower / 10);

                // sideways jump code

                if (MyMovementStatus == MovementStatus.Sliding)
                { // bullet jump is activated by jumping out of a slide.
                    if (bulletReady)
                    {
                        bulletReady = false;

                        // Between 0 and 1. aiming past 90* (forward) downwards will now clamp to 0
                        // number between 0 and 1 is % between forward and up
                        float ForwardThrust = Mathf.Clamp(cameraEmpty.transform.forward.y, 0, 1);
                        movementVector = new Vector3(
                            movementVector.x * ((1 - ForwardThrust) * bulletBoost), // More directional thrust based on the amount you /didn't/ aim up
                            movementVector.y + (ForwardThrust * (jumpPower / 25)), // Bullet jump can over double your jump height if you aim up
                            movementVector.z * ((1 - ForwardThrust) * bulletBoost)
                            );

                        print("BULLET " + ForwardThrust);
                    }
                    // if bullet jump wasn't ready, just jump like usual without the speed boost and thrust
                    if (key_Sprint)
                    {
                        ChangeMovementStatus(MovementStatus.Sprinting);
                    }
                    else { ChangeMovementStatus(MovementStatus.Normal); }
                }
            }
        }

        // this handles the fake impulse stuff. Essentially it just changes your movement
        // that used to be your movement input into a vector that was determined by the 
        // impulse. So of course right after the impulse is applied, control can be given right
        // back to the player.
        if (impulse_Vector != Vector3.zero)
        {
            movementVector = impulse_Vector;
            impulse_Vector = Vector3.zero;
        }
        else
        {
            if (grounded)
            {
                if (MyMovementStatus == MovementStatus.Sliding)
                {
                    movementVector = new Vector3(
                        last_MovementVector.x * (1 - slideControl) + (last_MovementVector.x * slideDecay * slideControl),
                        movementVector.y,
                        last_MovementVector.z * (1 - slideControl) + (last_MovementVector.z * slideDecay * slideControl));
                }
                else
                {
                    movementVector = new Vector3(
                        last_MovementVector.x * (1 - groundControl) + (movementVector.x * groundControl),
                        movementVector.y,
                        last_MovementVector.z * (1 - groundControl) + (movementVector.z * groundControl));
                }
            }
            else
            {
                if (MyMovementStatus == MovementStatus.Sliding)
                {
                    movementVector = new Vector3(
                        last_MovementVector.x * (1 - slideControl) + (movementVector.x * slideControl * slideDecay),
                        last_vspeed - ((gravityValue / 14) * gravityMultiplier),
                        last_MovementVector.z * (1 - slideControl) + (movementVector.z * slideControl * slideDecay));
                }
                else
                {
                    movementVector = new Vector3(
                    last_MovementVector.x * (1 - airControl) + (movementVector.x * airControl),
                    last_vspeed - ((gravityValue / 8) * gravityMultiplier),
                    last_MovementVector.z * (1 - airControl) + (movementVector.z * airControl));
                }
            }
        }
        // .Move actually causes the movement. may change this to a custom controller some day.
        controller.Move(movementVector * Time.fixedDeltaTime);
        // save off the movements to make nice smooth calculations later
        last_MovementVector = movementVector;
        last_vspeed = movementVector.y;
    }
}
