using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Corpa : AIMechanics
{

    private string EntityName = "Corpa";

    protected AIState MyState = AIState.Idle;



    public override void StartAI()
    {
        Activate();
        HiveMind.CheckForUnit(this);
    }

    protected void ActivateState()
    {
        switch (MyState)
        {
            case AIState.Idle: State_Idle(); break;
            case AIState.Combat: State_Combat(); break;
            default: break;
        }
    }

    //> STATES
    //> STATES
    //> STATES

    protected void State_Idle()
    {
        //print("State_Idle .. " + EntityName);
        if (HasPatrolTarget())
        {
            if (HasReachedPatrolTarget())
            {
                List<AIMechanics> PotentialUnit = HiveMind.CheckForUnit(this);
                if (PotentialUnit != null)
                {
                    // Found a unit to join. is now part of that unit

                }
                else { FindNewPatrol(); }
            }
            else
            {
                if (Agent.destination != PatrolTarget) { MoveTo(PatrolTarget); }
            }
        }
        else { FindNewPatrol(); }

    }

    private void State_Combat()
    {

    }

    //> ACTIONS
    //> ACTIONS
    //> ACTIONS



    private PlayerMechanics CheckForEnemies()
    {
        return null;
    }

    private void SearchForNewTarget()
    {

    }

    //> OVERRIDES
    //> OVERRIDES
    //> OVERRIDES

    public override bool HasReachedPatrolTarget()
    {
        if (Vector3.Distance(transform.position, PatrolTarget) <= 5f)
        {
            //PassDrone();
            return true;
        }
        else
        {
            return false;
        }
    }

    public override string GetName()
    {
        return EntityName;
    }

    public override void TriggerThought()
    {
        if (!GetPass())
        {
            //print(gameObject.name + " is thinking");
            if (IsActivated())
            {
                ActivateState();
            }
            else
            {
                Activate();
                TriggerThought();
            }
        }
        else { SetPass(false); }
    }

    protected override void MoveTo(Vector3 pos)
    {
        Agent.destination = pos;
    }

    protected override void RotateAt(Quaternion q)
    {
        Agent_UpdateRotation(false);
        Agent_UpdateRotation(q);
    }

    public override PlayerMechanics VisionCheck()
    {
        return CheckForEnemies();
    }
}
