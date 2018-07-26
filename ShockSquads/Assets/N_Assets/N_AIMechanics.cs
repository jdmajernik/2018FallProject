using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class N_AIMechanics : N_ActorMechanics {

    private float ThoughtSpeed = 1f;

    private float P_Range = 999f;

    private Vector3 MyTarget = new Vector3(0, 0, 0);


	void Start() {
        StartCoroutine(Think());
	}
	
	IEnumerator Think() {
        while (true) {
            print("I want to shoot my pew pew!");
            FireWeapon(MyTarget);

            // ..and loop
            yield return new WaitForSeconds(ThoughtSpeed);
        }
    }

    protected virtual void FireWeapon(Vector3 target) {
        // Laser gun
        Vector3 hit_point;
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit ray_hit;
        if (Physics.Raycast(ray, out ray_hit, P_Range)) {
            hit_point = ray_hit.point;
        } else {
            hit_point = ray.origin + ray.direction * P_Range;
        }

        Collider collider = ray_hit.collider;
        if (collider != null) {
            print("found a collider! " + collider.gameObject.name);
        }
    }
}
