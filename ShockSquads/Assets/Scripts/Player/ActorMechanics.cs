using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Affiliation { Neutral };

public class ActorMechanics : MonoBehaviour {

    [SerializeField] protected Affiliation MyAffiliation;

    private int health_max;
    private int health_current;

    public void TakeDamage(float damage) {
        int health_potential = (int)(health_current - damage);
        if (health_potential <= 0) {
            Death();
        } else {
            health_current = health_potential;
        }
    }

    protected virtual void Death() {
        health_current = 0;
        print("..died");
    }

    public int GetHealth() {
        return health_current;
    }

}
