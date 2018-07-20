using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAimCamera : MonoBehaviour {

    float sensitivity = 200f;

    float rotationY = 0f;
    float rotationX = 0f;

    CursorLockMode mode = CursorLockMode.Locked;

    bool canLook = true;

    private void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotationY = rot.y;
        rotationX = rot.x;

        Cursor.lockState = mode;
    }

    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (Cursor.lockState == mode)
            {
                print("UNLOCKING CURSOR");
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                print("LOCKING CURSOR");
                Cursor.lockState = CursorLockMode.Locked;
                return;
            }
        }

        if (Cursor.lockState == mode) //rotate camera with mouse
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = -Input.GetAxis("Mouse Y");

            rotationY += mouseX * sensitivity * Time.deltaTime;
            rotationX += mouseY * sensitivity * Time.deltaTime;

            rotationX = Mathf.Clamp(rotationX, -80f, 80f);

            Quaternion localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
            transform.rotation = localRotation;
        }
    }

    private void OnGUI()
    {
        
    }
}
