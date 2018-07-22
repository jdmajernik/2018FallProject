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
    private int           Speed_Sprint = 120;
    private int              Speed_Run = 90;
    private int            Speed_Sneak = 30;
    private float Backpedal_Multiplier = 0.5f;

    private int             Jump_Power = 250;

    private float        Gravity_Value = 9.8f;
    private float   Gravity_Multiplier = 1f;

    private float          Last_vspeed = 0;
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
        CameraEmpty = this.transform.Find("CameraEmpty").gameObject;
        Controller = GetComponent<CharacterController>();

    }

    private void Update() {

        // Fire weapon inputs
        Fire_BeingPressed = Input.GetMouseButton(0);
        Fire_JustPressed = Input.GetMouseButtonDown(0);
        
        // Camera follows mouse
        if (Cursor.lockState == mode) {
            float MousePosX = Input.GetAxis("Mouse X");
            float MousePosY = -Input.GetAxis("Mouse Y");

            CamRotationX += MousePosX * CamSensitivity * Time.deltaTime;
            CamRotationY += MousePosY * CamSensitivity * Time.deltaTime;
            CamRotationY = Mathf.Clamp(CamRotationY, -89f, 89f);

            Quaternion NewCameraRotation = Quaternion.Euler(CamRotationY, CamRotationX, 0f);
            CameraEmpty.transform.rotation = NewCameraRotation;
        }

        // Lock & unlock camera
        if (Input.GetKeyDown(KeyCode.Tab)) { ToggleCameraLock(); }

        // Overriden weapon function inputs
        if (Fire_JustPressed || Fire_BeingPressed) { FireWeapon(); }
        if (Input.GetKeyDown(KeyCode.R)) { ReloadWeapon(); }
        if (Input.GetKeyDown(KeyCode.Q)) { SwitchWeapon(); }
    }
    
    //Overriden weapon functions
    protected virtual void SwitchWeapon() { }
    protected virtual void ReloadWeapon() { }
    protected virtual void FireWeapon() { }

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

        // Movement
        Vector3 MovementVector = Quaternion.Euler(0f, CameraEmpty.transform.localRotation.eulerAngles.y, 0f) * 
            new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        MovementVector.x = MovementVector.x * (Speed_Run / 10);
        MovementVector.z = MovementVector.z * (Speed_Run / 10);
     
        if (Input.GetKey(KeyCode.Space)) {
            if (Controller.isGrounded) {
                MovementVector.y = (Jump_Power / 10);
            }
        }
        
        if (!Controller.isGrounded) {
            MovementVector.y = Last_vspeed - ((Gravity_Value / 10) * Gravity_Multiplier);
        }
        
        Controller.Move(MovementVector * Time.deltaTime);
        Last_vspeed = MovementVector.y;
    }

    public string GetPlayerName() {
        return PlayerName;
    }
}