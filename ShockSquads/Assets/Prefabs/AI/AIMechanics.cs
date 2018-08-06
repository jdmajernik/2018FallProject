using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // necessary 

public enum AIState { Idle, Combat }

[RequireComponent(typeof(NavMeshAgent))]
public class AIMechanics : ActorMechanics
{

    // Requires
    protected GameObject CameraEmpty;
    protected NavMeshAgent Agent;
    protected AIController HiveMind;

    protected bool DroneActivated;

    public bool PassMe = false;

    // AI
    public Vector3 PatrolTarget = Vector3.zero;
    protected float PatrolSpeed;

    private void Start()
    {
        CameraEmpty = StaticTools.FindChildGameObjectWithTag(this.gameObject, "CameraParent");
        Agent = GetComponent<NavMeshAgent>();

        PatrolSpeed = Agent.speed;

        ConnectToHiveMind();
    }

    public virtual void StartAI()
    {
        print("No override func found for StartAI in " + this.gameObject.name);
    }

    private void ConnectToHiveMind()
    {
        // Add self to hive mind
        HiveMind = GameObject.FindGameObjectWithTag("AIController").GetComponent<AIController>();
        if (HiveMind)
        {
            HiveMind.AddToNetwork(this);
        }
        else
        {
            // Create the hive mind
            StaticTools.CreateAIController();
            ConnectToHiveMind();
        }
    }

    //> ACTIONS
    //> ACTIONS
    //> ACTIONS



    //> PUBS
    //> PUBS
    //> PUBS

    public bool IsActivated() { return DroneActivated; }
    public void Activate() { print("Activated " + gameObject.name); DroneActivated = true; }
    public void CommandMovement(Vector3 pos) { MoveTo(pos); }
    public void CommandRotation(Vector3 rot) { RotateAt(Quaternion.LookRotation(rot.normalized, Vector3.up)); }
    public void CommandRotation(Quaternion q) { RotateAt(q); }
    public bool HasPatrolTarget() { if (PatrolTarget == null || PatrolTarget == Vector3.zero) { return false; } else { return true; } }
    public void FindNewPatrol() { PatrolTarget = HiveMind.GetPatrolPoint(this); if (PatrolTarget == Vector3.zero) { SetPass(true); } }
    public Vector3 GetFindNewPatrol() { FindNewPatrol(); return PatrolTarget; }
    public Vector3 GetMyPatrolTarget() { return PatrolTarget; }
    public void SetNewPatrol(Vector3 point) { PatrolTarget = point; Debug.DrawLine(transform.position, point, Color.magenta, 2f); }
    //public bool PassCheckSet() { if (PassMe) { PassMe = false; print("Passed " + gameObject.name); return true; } else { return false; } }
    public void Agent_UpdateRotation(bool b) { if (b == true) { Agent.updateRotation = true; } else { Agent.updateRotation = false; } }
    public void Agent_UpdateRotation(Quaternion q) { transform.rotation = q; }
    //public void PassDrone() { PassMe = true; }
    public void SetPatrolSpeed(float speed) { PatrolSpeed = speed; }
    public float GetPatrolSpeed() { return PatrolSpeed; }

    public bool GetPass() { return PassMe; }
    public void SetPass(bool b) { PassMe = b; print("set PassMe to " + b + " on " + gameObject.name); }

    //> VIRTS
    //> VIRTS
    //> VIRTS

    public virtual string GetName() { return "null_string"; }
    public virtual void TriggerThought() { print("no thought found in " + this.gameObject.name); }
    protected virtual void MoveTo(Vector3 pos) { print(this.gameObject.name + " has no override for MoveTo()"); }
    protected virtual void RotateAt(Quaternion q) { print(this.gameObject.name + " has no override for RotateAt()"); }
    public virtual PlayerMechanics VisionCheck() { print("no vision to check on " + this.gameObject.name); return null; }
    public virtual bool HasReachedPatrolTarget() { if (Vector3.Distance(transform.position, PatrolTarget) <= 5) { return true; } else { return false; } }

    protected override void Death()
    {
        base.Death();
        if (HiveMind)
        {
            HiveMind.RemoveFromNetwork(this);
        }
    }

    /*
    protected virtual void FireWeapon(Vector3 target)
    {
        // Laser gun
        Vector3 hit_point;
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit ray_hit;
        if (Physics.Raycast(ray, out ray_hit, P_Range))
        {
            hit_point = ray_hit.point;
        }
        else
        {
            hit_point = ray.origin + ray.direction * P_Range;
        }

        Collider collider = ray_hit.collider;
        if (collider != null)
        {
            print("found a collider! " + collider.gameObject.name);
        }
    } */
}
