using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardFaceCamera : MonoBehaviour {

    [SerializeField] private GameObject mainCamera;
    private void Start()
    {
        //mainCamera = Camera.main.gameObject;
        Invoke("SelfDestruct", 0.8f);
        StartCoroutine("lookAtCamera"); //updates every second to look at camera
    }

    void SelfDestruct()
    {
        Destroy(gameObject);
    }

    private IEnumerator lookAtCamera()
    {
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
        }
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
        StartCoroutine("lookAtCamera");
	}
}
