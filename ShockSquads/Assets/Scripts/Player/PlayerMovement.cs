using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour {

    // Requires
    [SerializeField] private GameObject CameraEmpty;
    private CharacterController Controller;
    private CapsuleCollider CapsuleCollider;
    private CameraController CameraController;

    //[SerializeField] protected string PlayerName;

    #region Global Player Movement Variables
    // Global Player Movement Variables
    private enum MovementStatus { Normal, Sprinting, Sneaking, Sliding }
    private MovementStatus MyMovementStatus = MovementStatus.Normal;

    private bool Key_Sprint;

    private int Speed_Sprint = 105; //120
    private int Speed_Run = 75; //90
    private int Speed_Sneak = 30; //30

    private float Slide_Control = 0.025f; // 0.025f
    private float Slide_Decay = 0.05f; // 0.05f
    private float Slide_Boost = 2f; // 2f
    private bool Slide_Ready = true;

    private float Bullet_Boost = 5f; // 5f
    private bool Bullet_Ready = true;

    private float Backpedal_Multiplier = 0.75f; //0.75f
    private float Ground_Control = 0.35f; //0.35f

    private int Jump_Power = 200; //200
    private float Air_Control = 0.035f; //0.035f

    private float Gravity_Value = 9.8f; //9.8f
    private float Gravity_Multiplier = 1f; //1f

    private float Last_vspeed = 0;
    private int Last_mspeed = 0;
    private Vector3 Last_MovementVector;

    private Vector3 Impulse_Vector;

    private bool Grounded;
    #endregion

    private void Start() {

        //CameraEmpty = GameObject.FindGameObjectWithTag("CameraParent");
        CameraEmpty = DevTools.FindChildGameObjectWithTag(this.gameObject, "CameraParent");
        Controller = GetComponent<CharacterController>();
        CapsuleCollider = GetComponent<CapsuleCollider>();
        CameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        //StartCoroutine(updatePos());
    }

    private void Update() {

        // Player movement inputs
        Key_Sprint = Input.GetButton("Sprint");

        if (Input.GetButtonDown("Sprint")) { ChangeMovementStatus(MovementStatus.Sprinting); }
        if (Input.GetButtonUp("Sprint")) { ChangeMovementStatus(MovementStatus.Normal); }

        if (Input.GetButtonDown("Sneak")) {
            if (MyMovementStatus == MovementStatus.Sprinting) {
                if (Input.GetAxisRaw("Vertical") > 0) { // Can't slide backwards
                    ChangeMovementStatus(MovementStatus.Sliding);
                    
                } else { ChangeMovementStatus(MovementStatus.Sneaking); }
            } else { ChangeMovementStatus(MovementStatus.Sneaking); }
        }
        if (Input.GetButtonUp("Sneak")) {
            if (Key_Sprint) { ChangeMovementStatus(MovementStatus.Sprinting); }
            else { ChangeMovementStatus(MovementStatus.Normal); }
        }

        // Testing impluses
        if (Input.GetKeyDown(KeyCode.K)) { AddImpulse(new Vector3(25, 25, 25)); }
    }

    private void ChangeMovementStatus(MovementStatus NewMovementStatus) {
        switch (NewMovementStatus) {
            case MovementStatus.Sliding:
                if (Slide_Ready) {
                    print("SLIDE");
                    Slide_Ready = false;
                    Bullet_Ready = true;
                    MyMovementStatus = NewMovementStatus;
                    if (Grounded) {
                        Last_MovementVector = Last_MovementVector * Slide_Boost;
                    }
                } else { print("Can't slide now"); }
                break;
            default: MyMovementStatus = NewMovementStatus; break;
        }
    }

    public void AddImpulse(Vector3 impulse) { Impulse_Vector = impulse; }

    private void FixedUpdate() {

        // Rotate body with camera
        transform.rotation = Quaternion.Euler(0, CameraEmpty.transform.localRotation.x, 0);
        
        Vector3 MovementVector = Quaternion.Euler(0, CameraEmpty.transform.localRotation.eulerAngles.y, 0) * 
            new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        //Vector3 ThisVelocity = Vector3.zero;

        // Gravity
        float PredictNext_vspeed = Last_vspeed - ((Gravity_Value / 12) * Gravity_Multiplier);

        RaycastHit GroundHit;
        Grounded = false;
        if (Physics.SphereCast(new Ray(transform.position, Vector3.down), 0.4f, out GroundHit, 0.78f - (PredictNext_vspeed * Time.fixedDeltaTime))) {
            if (Vector3.Angle(GroundHit.normal, Vector3.up) <= Controller.slopeLimit) { // Slope handling
                Grounded = true;
                if (Slide_Ready != true) { Slide_Ready = true; }
                transform.position = new Vector3(transform.position.x, GroundHit.point.y + 1f + Controller.skinWidth, transform.position.z);
                //print("Transform is being set to " + new Vector3(transform.position.x, GroundHit.point.y + 1f + Controller.skinWidth, transform.position.z));
                //print("Transform is currently set to " + transform.position);
            }
        }

        if (Grounded) { // If grounded, apply MovementStatus speed to movement
            int ThisSpeed;
            switch (MyMovementStatus) {
                case MovementStatus.Normal: ThisSpeed = Speed_Run; break;
                case MovementStatus.Sprinting: ThisSpeed = Speed_Sprint; break;
                case MovementStatus.Sneaking: ThisSpeed = Speed_Sneak; break;
                default: ThisSpeed = Speed_Run; break;
            } if (Input.GetAxisRaw("Vertical") < 0) { // Backpedaling penality
                if (MyMovementStatus == MovementStatus.Sliding) { ChangeMovementStatus(MovementStatus.Normal); }
                if (MyMovementStatus == MovementStatus.Sprinting) { ThisSpeed = (int)(Speed_Run * Backpedal_Multiplier); } // Can't sprint backwards
                else { ThisSpeed = (int)(ThisSpeed * Backpedal_Multiplier); }
            }
            MovementVector.x *= (ThisSpeed / 10);
            MovementVector.z *= (ThisSpeed / 10);
            Last_mspeed = ThisSpeed;
        } else { // If not grounded, use the last applied MovementStatus speed to movement
            MovementVector.x *= (Last_mspeed / 10);
            MovementVector.z *= (Last_mspeed / 10);
        }
        
        if (Input.GetButton("Jump")) {
            if (Grounded) {
                MovementVector.y = (Jump_Power / 10);
                if (MyMovementStatus == MovementStatus.Sliding) {
                    if (Bullet_Ready) {
                        Bullet_Ready = false;

                        // Between 0 and 1. aiming past 90* (forward) downwards will now clamp to 0
                        // number between 0 and 1 is % between forward and up
                        float ForwardThrust = Mathf.Clamp(CameraEmpty.transform.forward.y, 0, 1); 
                        MovementVector = new Vector3(
                            MovementVector.x * ((1 - ForwardThrust) * Bullet_Boost), // More directional thrust based on the amount you /didn't/ aim up
                            MovementVector.y + (ForwardThrust * (Jump_Power / 25)), // Bullet jump can over double your jump height if you aim up
                            MovementVector.z * ((1 - ForwardThrust) * Bullet_Boost)
                            );
                        
                        print("BULLET " + ForwardThrust);
                    }
                    if (Key_Sprint) { ChangeMovementStatus(MovementStatus.Sprinting);
                    } else { ChangeMovementStatus(MovementStatus.Normal); }
                }
            }
        }

        if (Impulse_Vector != Vector3.zero) {
            MovementVector = Impulse_Vector;
            Impulse_Vector = Vector3.zero;
        } else {
            if (Grounded) {
                if (MyMovementStatus == MovementStatus.Sliding) {
                    MovementVector = new Vector3(
                        Last_MovementVector.x * (1 - Slide_Control) + (Last_MovementVector.x * Slide_Decay * Slide_Control),
                        MovementVector.y,
                        Last_MovementVector.z * (1 - Slide_Control) + (Last_MovementVector.z * Slide_Decay * Slide_Control));
                } else {
                    MovementVector = new Vector3(
                        Last_MovementVector.x * (1 - Ground_Control) + (MovementVector.x * Ground_Control),
                        MovementVector.y,
                        Last_MovementVector.z * (1 - Ground_Control) + (MovementVector.z * Ground_Control));
                }
            } else {
                if (MyMovementStatus == MovementStatus.Sliding) {
                    MovementVector = new Vector3(
                        Last_MovementVector.x * (1 - Slide_Control) + (MovementVector.x * Slide_Control * Slide_Decay),
                        Last_vspeed - ((Gravity_Value / 14) * Gravity_Multiplier),
                        Last_MovementVector.z * (1 - Slide_Control) + (MovementVector.z * Slide_Control * Slide_Decay));
                } else {
                    MovementVector = new Vector3(
                    Last_MovementVector.x * (1 - Air_Control) + (MovementVector.x * Air_Control),
                    Last_vspeed - ((Gravity_Value / 8) * Gravity_Multiplier),
                    Last_MovementVector.z * (1 - Air_Control) + (MovementVector.z * Air_Control));
                }
            }
        }
        Controller.Move(MovementVector * Time.fixedDeltaTime);
        Last_MovementVector = MovementVector;
        Last_vspeed = MovementVector.y;
    }
}
