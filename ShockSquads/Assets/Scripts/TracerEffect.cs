using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TracerEffect : MonoBehaviour {

    private float StartTime;
    private float JourneyLength;

    private float Speed = 250f;

    private Vector3 Target;
    private Vector3 Start;

	public void ShootToHere(Vector3 point) {

        Start = this.transform.position;
        Target = point;

        StartTime = Time.time;
        JourneyLength = Vector3.Distance(Start, Target);
    }
    
    private void Update() {

        float DistanceCovered = (Time.time - StartTime) * Speed;
        float JourneyFraction = DistanceCovered / JourneyLength;
        transform.position = Vector3.Lerp(Start, Target, JourneyFraction);

        if (transform.position == Target) { Destroy(this.gameObject); }
    }
}
