using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class N_Tools : MonoBehaviour {

    static Vector3 AddNoiseOnAngle(float bloom)
    {
        float x_noise = Random.Range(-bloom, bloom);
        float y_noise = Random.Range(-bloom, bloom);

        Vector3 noise = new Vector3(
            Mathf.Sin(2f * Mathf.PI * x_noise / 360),
            Mathf.Sin(2f * Mathf.PI * y_noise / 360),
            0f);

        return noise;
    }

}
