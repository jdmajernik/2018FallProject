using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class N_CameraController : MonoBehaviour {

    //Requires
    protected Camera MyCamera;
    protected GameObject CameraEmpty;
    private CharacterController Controller;

    // Camera variables
    CursorLockMode mode = CursorLockMode.Locked;
    float CamSensitivity = 100f;
    float CamRotationX = 0f;
    float CamRotationY = 0f;
    
    float CameraRollMax = 1f; // angle that camera rolls on sideways movement
    float CameraRollConstant = 0.1f; // smoothness essentially. lower is more smooth.
    float LastCameraRoll = 0f;

    float Trauma = 0f;
    float TraumaDampen = 3f; // divide trauma value by this to get something that can be used for ranomization
    float TraumaIncrement = 0.35f; // drop trauma by this amount every lateupdate
    float TraumaMax = 10f;

    void Start () {

        MyCamera = Camera.main;
        CameraEmpty = this.transform.parent.gameObject;
        Controller = this.transform.parent.transform.parent.GetComponent<CharacterController>();
    }
	
	void Update () {

        // Lock & unlock camera
        if (Input.GetKeyDown(KeyCode.Tab)) { ToggleCameraLock(); }

        if (Input.GetKeyDown(KeyCode.J)) { AddTrauma(5f); }
    }

    private void LateUpdate() {

        // Camera follows mouse
        if (Cursor.lockState == mode) {
            
            // Get mouse inputs
            float MousePosX = Input.GetAxis("Mouse X");
            float MousePosY = -Input.GetAxis("Mouse Y");

            CamRotationX += MousePosX * CamSensitivity * Time.deltaTime;
            CamRotationY += MousePosY * CamSensitivity * Time.deltaTime;
            CamRotationY = Mathf.Clamp(CamRotationY, -89, 89);

            // Camera roll
            float ThisCameraRoll = LastCameraRoll * (1 - CameraRollConstant) + (-Input.GetAxisRaw("Horizontal") * CameraRollConstant);
            LastCameraRoll = ThisCameraRoll;

            // Trauma
            Vector3 RotationAmount = new Vector3(0,0,0);
            if (Trauma > 0) {
                RotationAmount = Random.insideUnitSphere * Mathf.Pow(Trauma/TraumaDampen, 2);
                RotationAmount.z = 0f; // z is reserved for roll. also looks weird in shake
                if (Trauma <= TraumaIncrement) {
                    Trauma = 0;
                } else {
                    Trauma -= TraumaIncrement;
                }
            }

            MyCamera.transform.localRotation = Quaternion.Euler(RotationAmount.x, RotationAmount.y, ThisCameraRoll * CameraRollMax);
            Quaternion NewCameraRotation = Quaternion.Euler(CamRotationY, CamRotationX, 0);
            CameraEmpty.transform.rotation = NewCameraRotation;
        }
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

    public void AddTrauma(float amount) {
        float PotentialTrauma = Trauma + amount;
        if (PotentialTrauma > TraumaMax) {
            Trauma = TraumaMax;
        } else {
            Trauma = PotentialTrauma;
        }
    }
}
