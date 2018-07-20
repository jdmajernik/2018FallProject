using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardFaceCamera : MonoBehaviour {

    [SerializeField] private GameObject mainCamera;
    private void Start()
    {
        mainCamera = Camera.main.gameObject;
        Invoke("SelfDestruct", 0.8f);
    }

    void SelfDestruct()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
	}
}
