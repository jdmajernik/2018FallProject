using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    //Requires
    protected Camera MyCamera;
    protected GameObject CameraEmpty;
    private CharacterController Controller;

    // Camera variables
    private enum CameraViewMode { First, Third };
    private CameraViewMode MyCameraViewMode;
    private bool CameraBobAndWeave = true;

    private float CamSensitivity = 100f;
    private float CamRotationX = 0f;
    private float CamRotationY = 0f;

    private float CameraRollMax = 1f; // angle that camera rolls on sideways movement
    private float CameraRollConstant = 0.1f; // smoothness essentially. lower is more smooth.
    private float LastCameraRoll = 0f;

    private float Trauma = 0f;
    private float TraumaDampen = 3f; // divide trauma value by this to get something that can be used for ranomization
    private float TraumaIncrement = 0.35f; // drop trauma by this amount every lateupdate
    private float TraumaMax = 10f;

    void Start () {

        MyCamera = Camera.main;
        CameraEmpty = this.transform.parent.gameObject;
        Controller = this.transform.parent.transform.parent.GetComponent<CharacterController>();
    }
	
	void Update () {

        // Lock & unlock camera
        if (Input.GetButtonDown("Camera Lock")) { ToggleCameraLock(false); }
        if (Input.GetButtonUp("Camera Lock")) { ToggleCameraLock(true); }

        // Change view type
        if (Input.GetButtonDown("Camera View")) { ToggleCameraView(); }

        // Debugging
        if (Input.GetKeyDown(KeyCode.J)) { AddTrauma(5f); }
    }

    private void LateUpdate() {

        // Camera follows mouse
        if (Cursor.lockState == CursorLockMode.Locked) {
            
            // Get mouse inputs
            float MousePosX = Input.GetAxis("Mouse X");
            float MousePosY = -Input.GetAxis("Mouse Y");

            CamRotationX += MousePosX * CamSensitivity * Time.fixedDeltaTime;
            CamRotationY += MousePosY * CamSensitivity * Time.fixedDeltaTime;
            CamRotationY = Mathf.Clamp(CamRotationY, -89, 89);

            // Camera roll
            float ThisCameraRoll = 0f;
            if (CameraBobAndWeave) {
                ThisCameraRoll = LastCameraRoll * (1 - CameraRollConstant) + (-Input.GetAxisRaw("Horizontal") * CameraRollConstant);
                LastCameraRoll = ThisCameraRoll;
            }

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
            if (CameraBobAndWeave) { /* Change CameraEmpty position to the position of the player's head */ }
        }
    }

    private void ToggleCameraView() {
        if (MyCameraViewMode == CameraViewMode.First) {
            MyCameraViewMode = CameraViewMode.Third;
            MyCamera.transform.localPosition = new Vector3(1, 1, -3);
        } else {
            MyCameraViewMode = CameraViewMode.First;
            MyCamera.transform.localPosition = new Vector3(0, 0, 0);
        }
    }

    private void ToggleCameraLock(bool b) {
        if (b) {
            if (Cursor.lockState == CursorLockMode.None) {
                Cursor.lockState = CursorLockMode.Locked;
            }
        } else {
            if (Cursor.lockState == CursorLockMode.Locked) {
                Cursor.lockState = CursorLockMode.None;
            }
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
