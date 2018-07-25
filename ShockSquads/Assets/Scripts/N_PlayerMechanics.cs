using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class N_PlayerMechanics : N_ActorMechanics {

    // Requires
    protected Camera MyCamera;
    protected GameObject CameraEmpty;
    private CharacterController Controller;

    //[SerializeField] protected string PlayerName;
    private string PlayerName = "Tester114";

    #region Global Player Movement Variables
    // Global Player Movement Variables
    private enum MovementStatus { Normal, Sprinting, Sneaking }
    private MovementStatus MyMovementStatus = MovementStatus.Normal;

    private KeyCode         Key_Sprint = KeyCode.LeftShift;
    private bool   Sprint_BeingPressed = false;
    private KeyCode          Key_Sneak = KeyCode.C;
    private bool    Sneak_BeingPressed = false;

    private int           Speed_Sprint = 120;
    private int              Speed_Run = 90;
    private int            Speed_Sneak = 30;
    private float Backpedal_Multiplier = 0.75f;
    private float       Ground_Control = 0.35f;

    private int             Jump_Power = 250;
    private float          Air_Control = 0.035f;

    private float        Gravity_Value = 9.8f;
    private float   Gravity_Multiplier = 1f;

    private float          Last_vspeed = 0;
    private float          Last_mspeed = 0;
    private Vector3        Last_MovementVector;
    #endregion
    
    // Weapon variables
    protected enum SelectedWeapon { Primary, Secondary };
    protected SelectedWeapon MySelectedWeapon = SelectedWeapon.Primary;
    protected bool Fire_JustPressed;
    protected bool Fire_BeingPressed;
    
    // Camera variables
    CursorLockMode mode = CursorLockMode.Locked;
    float CamSensitivity = 100f;
    float CamRotationX = 0f;
    float CamRotationY = 0f;

    private void Start() {

        MyCamera = Camera.main;
        //CameraEmpty = this.transform.Find("CameraEmpty").gameObject;
        Controller = GetComponent<CharacterController>();
    }

    private void Update() {

        // Fire weapon inputs
        Fire_BeingPressed = Input.GetMouseButton(0);
        Fire_JustPressed = Input.GetMouseButtonDown(0);
        Sprint_BeingPressed = Input.GetKey(KeyCode.LeftShift);

        // Lock & unlock camera
        if (Input.GetKeyDown(KeyCode.Tab)) { ToggleCameraLock(); }

        // Overriden weapon function inputs
        if (Fire_JustPressed || Fire_BeingPressed) { FireWeapon(); }
        if (Input.GetKeyDown(KeyCode.R)) { ReloadWeapon(); }
        if (Input.GetKeyDown(KeyCode.Q)) { SwitchWeapon(); }

        // Player movement inputs
        if (Input.GetKeyDown(Key_Sprint)) { ChangeMovementStatus(MovementStatus.Sprinting); }
        if (Input.GetKeyUp(Key_Sprint)) { ChangeMovementStatus(MovementStatus.Normal); }
        if (Input.GetKeyDown(Key_Sneak)) { ChangeMovementStatus(MovementStatus.Sneaking); }
        if (Input.GetKeyUp(Key_Sneak)) { ChangeMovementStatus(MovementStatus.Normal); }
    }
    
    //Overriden weapon functions
    protected virtual void SwitchWeapon() { }
    protected virtual void ReloadWeapon() { }
    protected virtual void FireWeapon() { }

    private void ChangeMovementStatus(MovementStatus NewMovementStatus) {
        MyMovementStatus = NewMovementStatus;
    }

    private void ToggleCameraLock() {
        if (Cursor.lockState == mode) {
            print("UNLOCKING CURSOR");
            Cursor.lockState = CursorLockMode.None;
        } else {
            print("LOCKING CURSOR");
            Cursor.lockState = CursorLockMode.Locked;
            return;
        }
    }

    private void FixedUpdate() {

        // Camera follows mouse
        if (Cursor.lockState == mode) {
            float MousePosX = Input.GetAxis("Mouse X");
            float MousePosY = -Input.GetAxis("Mouse Y");

            CamRotationX += MousePosX * CamSensitivity * Time.deltaTime;
            CamRotationY += MousePosY * CamSensitivity * Time.deltaTime;
            CamRotationY = Mathf.Clamp(CamRotationY, -89, 89);

            Quaternion NewCameraRotation = Quaternion.Euler(CamRotationY, CamRotationX, 0);
            //CameraEmpty.transform.rotation = NewCameraRotation;
            transform.rotation = Quaternion.Euler(0, CamRotationX, 0); // Rotate body with camera
        }
        
        // Movement
        Vector3 MovementVector = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, 0) *
            new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        if (Controller.isGrounded) { // If grounded, apply MovementStatus speed to movement
            int ThisSpeed;
            switch (MyMovementStatus) {
                case MovementStatus.Normal: ThisSpeed = Speed_Run; break;
                case MovementStatus.Sprinting: ThisSpeed = Speed_Sprint; break;
                case MovementStatus.Sneaking: ThisSpeed = Speed_Sneak; break;
                default: ThisSpeed = Speed_Run; break;
            }
            if (Input.GetAxisRaw("Vertical") < 0) { // Backpedaling penality
                if (MyMovementStatus == MovementStatus.Sprinting) { ThisSpeed = (int)(Speed_Run * Backpedal_Multiplier); } // Can't sprint backwards
                else { ThisSpeed = (int)(ThisSpeed * Backpedal_Multiplier); }
            } 
            MovementVector.x = MovementVector.x * (ThisSpeed / 10);
            MovementVector.z = MovementVector.z * (ThisSpeed / 10);
            Last_mspeed = ThisSpeed;
        } else { // If not grounded, use the last applied MovementStatus speed to movement
            MovementVector.x = MovementVector.x * (Last_mspeed / 10);
            MovementVector.z = MovementVector.z * (Last_mspeed / 10);
        }
     
        // Jump
        if (Input.GetKey(KeyCode.Space)) {
            if (Controller.isGrounded) {
                MovementVector.y = (Jump_Power / 10);
            }
        }
        
        if (Controller.isGrounded) {
            // Ground Control
            Vector3 GroundMovementVector = new Vector3(
        /* x */ Last_MovementVector.x * (1 - Ground_Control) + (MovementVector.x * Ground_Control),
        /* y */ MovementVector.y, // Ready for jump input
        /* z */ Last_MovementVector.z * (1 - Ground_Control) + (MovementVector.z * Ground_Control)
            );
            Controller.Move(GroundMovementVector * Time.deltaTime);
            Last_MovementVector = GroundMovementVector;
            Last_vspeed = GroundMovementVector.y;
        } else {
            // Air Control
            Vector3 AirMovementVector = new Vector3(
        /* x */ Last_MovementVector.x * (1 - Air_Control) + (MovementVector.x * Air_Control),
        /* y */ Last_vspeed - ((Gravity_Value / 10) * Gravity_Multiplier), // Gravity
        /* z */ Last_MovementVector.z * (1 - Air_Control) + (MovementVector.z * Air_Control)
            );
            Controller.Move(AirMovementVector * Time.deltaTime);
            Last_MovementVector = AirMovementVector;
            Last_vspeed = AirMovementVector.y;
        }
    }

    public string GetPlayerName() {
        return PlayerName;
    }
}