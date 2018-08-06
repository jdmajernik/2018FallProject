using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

// AI Controller is DIFFERENT than AIMechanics. AI Controller controls all AI in the scene and is necessary for hive mind/ collab integration

public class AIController : MonoBehaviour
{

    private List<PlayerMechanics> Players = new List<PlayerMechanics>(); // Understand basic player
    private List<AIMechanics> MyAI = new List<AIMechanics>(); // Control the hive's minds

    private List<List<AIMechanics>> Units = new List<List<AIMechanics>>(); // Units are groups of AI

    private enum FormationShape { Doubles, Duo, Triangle, Square, House }
    private Dictionary<FormationShape, List<Vector3>> Formations = new Dictionary<FormationShape, List<Vector3>> {
        { FormationShape.Duo, new List<Vector3> { new Vector3(1,0,0), new Vector3(-1,0,0) } },
        { FormationShape.Triangle, new List<Vector3> { new Vector3(0,0,1), new Vector3(-1,0,-1), new Vector3(1,0,-1) } },
        { FormationShape.Square, new List<Vector3> { new Vector3(-1,0,1), new Vector3(1,0,1), new Vector3(-1,0,-1), new Vector3(1,0,-1) } },
        { FormationShape.House, new List<Vector3> { new Vector3(0,0,2), new Vector3(-1,0,0), new Vector3(1,0,0), new Vector3(-1,0,-2), new Vector3(1,0,-2) } }
    };

    private float ThoughtSpeed = 5f; // Time it takes to cycle through all AI thought connected to hive mind. use caution

    private List<Vector3> PatrolPoints = new List<Vector3>
    {
        new Vector3(-35,0,25),
        new Vector3(35,0,-25),
        new Vector3(15,0,35),
        //*/
    };

    //> CODE CODE CODE CODE CODE CODE CODE CODE CODE CODE
    //> CODE CODE CODE CODE CODE CODE CODE CODE CODE CODE
    //> CODE CODE CODE CODE CODE CODE CODE CODE CODE CODE
    //> CODE CODE CODE CODE CODE CODE CODE CODE CODE CODE
    //> CODE CODE CODE CODE CODE CODE CODE CODE CODE CODE

    void Start()
    {
        // Debugging. these will throw print errors 
        DebugPatrolPoints();

        StartCoroutine(Think());
    }

    void Update()
    {

    }

    IEnumerator Think()
    {
        while (true)
        {
            //print("HIVE MIND is thinking");
            TriggerAllAI(); // triggers thought for all connected AI

            yield return new WaitForSeconds(ThoughtSpeed);
        }
    }

    //> ACTIONS
    //> ACTIONS
    //> ACTIONS

    private void StartAllDrones()
    {
        if (HasDrones())
        {
            foreach (AIMechanics drone in MyAI)
            {
                if (!drone.IsActivated())
                {
                    drone.StartAI();
                }
            }
        }
    }

    private void TriggerAllAI()
    {
        if (HasDrones())
        {
            // Divide Solos from Units
            List<AIMechanics> Solos = new List<AIMechanics>(MyAI);
            if (HasUnits())
            {
                foreach (List<AIMechanics> unit in Units)
                {
                    foreach (AIMechanics drone in unit) { Solos.Remove(drone); }
                }
            }
            float RatioOfSolosToRest = Solos.Count / MyAI.Count;
            // Think for Solos
            for (int i = 0; i < Solos.Count; i++)
            {
                if (!Solos[i].IsActivated()) { Solos[i].StartAI(); }
                StartCoroutine(DelayedThought(Solos[i], i * (ThoughtSpeed / MyAI.Count)));
            }
            // Think for Units
            for (int i = 0; i < Units.Count; i++)
            {
                foreach (AIMechanics drone in Units[i]) { if (!drone.IsActivated()) { drone.StartAI(); } }
                StartCoroutine(DelayedThought(Units[i], RatioOfSolosToRest + (i * (ThoughtSpeed / Units.Count))));
            }
        }
    }

    IEnumerator DelayedThought(AIMechanics drone, float timer)
    {
        yield return new WaitForSeconds(timer);
        // Make sure I'm still solo before thinking like an idiot

        drone.TriggerThought();
        Debug.DrawRay(drone.transform.position, Vector3.up * 3f, Color.blue, ThoughtSpeed);
    }

    IEnumerator DelayedThought(List<AIMechanics> unit, float timer)
    {
        yield return new WaitForSeconds(timer);
        Unit_TriggerThought(unit);
        foreach (AIMechanics drone in unit) { Debug.DrawRay(drone.transform.position, Vector3.up * 3f, Color.white, ThoughtSpeed); }
    }

    private void AdjustAndContinuePatrol(List<AIMechanics> unit, Vector3 point)
    {
        Unit_FormFacing(unit, Vector3.zero, point);
        Unit_SetPass(unit, true);
    }

    private void AdjustAndContinuePatrol(AIMechanics drone, Vector3 point)
    {
        drone.CommandRotation(point);
        drone.SetPass(true);
    }

    //> DEBUG
    //> DEBUG
    //> DEBUG

    private void DebugPatrolPoints()
    {
        for (int i = 0; i < PatrolPoints.Count; i++)
        {
            NavMeshHit NavHit;
            if (!NavMesh.SamplePosition(PatrolPoints[i], out NavHit, 10f, NavMesh.AllAreas))
            {
                throw new System.Exception("A patrol point is not valid. " + PatrolPoints[i]);
            }
            else
            {
                if (NavHit.position != PatrolPoints[i])
                {
                    PatrolPoints[i] = NavHit.position;
                    print("Changed a patrol point.");
                }
            }
        }
    }

    /*
    IEnumerator AdjustAndContinuePatrol(List<AIMechanics> unit, Vector3 point) {
        Unit_FormFacing(unit, Vector3.zero, point);
        yield return new WaitForSeconds(ThoughtSpeed);
        Unit_Patrol(unit);
    }

    IEnumerator AdjustAndContinuePatrol(AIMechanics drone, Vector3 point) {
        drone.CommandRotation(point);
        yield return new WaitForSeconds(ThoughtSpeed);
        drone.CommandMovement(point);
    }
    */

    private bool HasDrones()
    {
        if (MyAI.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool HasUnits()
    {
        if (Units.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void CleanUnits() // Trigger this after any changes to units, this will prevent units from being empty or contain only 1 drone
    {
        foreach (List<AIMechanics> unit in Units)
        {
            if (unit.Count < 2) { Units.Remove(unit); }
        }
    }

    //> UNITS
    //> UNITS
    //> UNITS

    private void Unit_TriggerThought(List<AIMechanics> unit)
    {
        AIMechanics leader = unit[0];

        if (leader.GetPass()) { Unit_SetPass(unit, false); return; }

        // Debugging
        print("We're thinking ok!");
        foreach (AIMechanics drone in unit)
        {
            foreach (AIMechanics other in unit)
            {
                if (drone != other) { Debug.DrawLine(drone.transform.position, other.transform.position, Color.white, ThoughtSpeed); }
            }
        }

        // Unit copies the leader. Lots of movement coordination mostly. All drones are vision-aware
        PlayerMechanics PotentialTarget = null;
        foreach (AIMechanics drone in unit)
        {
            PotentialTarget = drone.VisionCheck();
            if (PotentialTarget) { break; }
        }
        if (PotentialTarget)
        { // Unit found a target

        }
        else
        { // No target
            if (leader.HasPatrolTarget())
            {
                // Already patrolling
                //Debug.DrawLine(leader.transform.position, leader.GetMyPatrolTarget(), Color.red, ThoughtSpeed);
                if (leader.HasReachedPatrolTarget())
                {
                    if (!leader.GetPass())
                    {
                        //print(leader.gameObject.name + " PASS TEST");
                        Unit_FindNewPatrol(unit);
                    }
                    else
                    {
                        //print(leader.gameObject.name + " FAIL TEST");
                        leader.SetPass(false);
                    }
                }
                else { Unit_Patrol(unit); }
            }
            else
            {
                Unit_FindNewPatrol(unit);
            }
        }
    }

    private void Unit_FindNewPatrol(List<AIMechanics> unit)
    {
        print("Find new patrol!");
        // Find new patrol
        AIMechanics leader = unit[0];

        Vector3 PatrolPoint = Vector3.zero;
        PatrolPoint = leader.GetFindNewPatrol();

        if (PatrolPoint != null || PatrolPoint != Vector3.zero)
        {
            foreach (AIMechanics drone in unit)
            {
                if (drone != leader)
                {
                    Vector3 MyOffset = drone.transform.position - leader.transform.position;
                    drone.SetNewPatrol(PatrolPoint + MyOffset);
                }
            }
        }
        else
        {
            print("GetFindNewPatrol failed on leader of unit.");
        }
        AdjustAndContinuePatrol(unit, PatrolPoint);
    }

    private void Unit_FormFacing(List<AIMechanics> unit, Vector3 center, Vector3 direction)
    {
        // Debugging
        print("Unit_FormFacing!");
        foreach (AIMechanics drone in unit)
        {
            foreach (AIMechanics other in unit)
            {
                if (drone != other) { Debug.DrawLine(drone.transform.position, other.transform.position, Color.black, ThoughtSpeed); }
            }
        }

        // Unit forms an appropriate shape facing this direction
        int UnitSize = unit.Count;
        FormationShape Shape;
        List<Vector3> Offsets = new List<Vector3>();
        switch (UnitSize)
        {
            // Forms are saved in a dictionary. "Formations
            case 2: Shape = FormationShape.Duo; Offsets = Formations[Shape]; break;
            case 3: Shape = FormationShape.Triangle; Offsets = Formations[Shape]; break;
            case 4: Shape = FormationShape.Square; Offsets = Formations[Shape]; break;
            case 5: Shape = FormationShape.House; Offsets = Formations[Shape]; break;
            default:
                Shape = FormationShape.Doubles; // This form wont work well because it's not centered yet
                if (UnitSize % 2 != 0)
                {
                    // Add tip drone if uneven
                    Offsets.Add(new Vector3(0, 0, -2));
                }
                int Row = 0;
                for (int i = 0; i < UnitSize; i++)
                { // Organize rest of group into duos
                    if (i % 2 == 0)
                    { // Even
                        Offsets.Add(new Vector3(1, 0, Row * 2));
                    }
                    else
                    { // Odd
                        Offsets.Add(new Vector3(-1, 0, Row * 2));
                        Row++;
                    }
                }
                Offsets.Reverse();
                break;
        }

        Vector3 CenterVector;
        if (center == Vector3.zero)
        {
            // Get average position from drones in unit
            Vector3 MeanVector = Vector3.zero;
            foreach (AIMechanics drone in unit) { MeanVector += drone.gameObject.transform.position; }
            CenterVector = MeanVector / UnitSize;
        }
        else { CenterVector = center; } // Else use passed value

        int UnitIndex = 0;
        foreach (AIMechanics drone in unit)
        {

            // Rotate to prepare for entering formation
            drone.CommandRotation(unit[0].transform.forward);

            // Rotate point around pivot
            Vector3 D = (Offsets[UnitIndex] + CenterVector) - CenterVector; // Direction of given offset from center point
            D = Quaternion.FromToRotation(drone.transform.forward, direction) * D; // Rotate the direction by the amount necessary to face "direction
            Vector3 DronePosition = D + CenterVector; // Create the actual position by adding center and the newly rotated offset
            //*/

            //Vector3 DronePosition = Offsets[UnitIndex] + CenterVector;
            //print("hi, my offset is " + Offsets[UnitIndex] + ", the center is " + CenterVector + ", and I decided to move to " + DronePosition);

            // Move to correct position
            drone.CommandMovement(DronePosition);

            UnitIndex++;
        }
    }

    private void Unit_Patrol(List<AIMechanics> unit)
    {
        foreach (AIMechanics drone in unit)
        {
            drone.CommandMovement(drone.GetMyPatrolTarget());
        }
    }

    private List<AIMechanics> CreateUnit(List<AIMechanics> new_unit)
    {
        List<AIMechanics> AddedUnit = new List<AIMechanics> { };
        Units.Add(AddedUnit);
        foreach (AIMechanics drone in new_unit) { JoinUnit(drone, AddedUnit); }
        return new_unit;
    }

    private void JoinUnit(AIMechanics drone, List<AIMechanics> unit)
    {
        print(drone.gameObject.name + " joined a unit " + (unit.Count + 1) + " big");
        if (Units.Contains(unit))
        {
            unit.Add(drone);
            Debug.DrawLine(drone.transform.position, unit[0].transform.position, Color.green, ThoughtSpeed);
            PassUnit(unit);
            Unit_PatrolSpeedSet(unit);
        }
    }

    private void Unit_PatrolSpeedSet(List<AIMechanics> unit)
    {
        // Get lowest movespeed in unit, this is the new patrol speed of the unit
        float SpeedToBeat = Mathf.Infinity;
        foreach (AIMechanics drone in unit)
        {
            float ThisSpeed = drone.GetPatrolSpeed();
            if (ThisSpeed < SpeedToBeat) { SpeedToBeat = ThisSpeed; }
        }
        unit[0].SetPatrolSpeed(SpeedToBeat);
    }

    private void Unit_SetPass(List<AIMechanics> unit, bool b)
    {
        foreach (AIMechanics drone in unit)
        {
            drone.SetPass(b);
        }
    }

    private void PassUnit(List<AIMechanics> unit)
    {
        foreach (AIMechanics drone in unit)
        {
            drone.SetPass(true);
        }
    }

    //> PUBS
    //> PUBS
    //> PUBS

    public List<AIMechanics> CheckForUnit(AIMechanics drone)
    {
        // Look for pre-built units
        Vector3 Position = drone.transform.position;
        List<AIMechanics> Solos = MyAI;
        foreach (List<AIMechanics> unit in Units)
        {
            if (unit.Count < 6)
            {
                foreach (AIMechanics unit_drone in unit)
                {
                    if (Vector3.Distance(unit_drone.transform.position, Position) <= 5)
                    {
                        JoinUnit(drone, unit);
                        Unit_FormFacing(unit, unit[0].transform.position, unit[0].transform.forward);
                        return unit;
                    }
                    else
                    {
                        Solos.Remove(unit_drone);
                    }
                }
            }
        }

        // Look for other solos
        foreach (AIMechanics solo_drone in MyAI)
        {
            if (solo_drone != drone)
            {
                if (Vector3.Distance(solo_drone.transform.position, Position) <= 5)
                {
                    List<AIMechanics> NewUnit = new List<AIMechanics> { drone, solo_drone };
                    NewUnit = CreateUnit(NewUnit);
                    Unit_FormFacing(NewUnit, Vector3.zero, drone.transform.forward);
                    return NewUnit;
                }
            }
        }

        // No pre-made unit found
        return null;
    }

    public void AddToNetwork(AIMechanics drone)
    {
        // Check and make sure it doesn't already exist in the network
        foreach (AIMechanics ai in MyAI)
        {
            if (ai == drone)
            {
                return; // Found it, get outta here
            }
        }
        // If nothing matches, go ahead and add it
        MyAI.Add(drone);
        print("Added " + drone.name + " to the hive");
    }

    public void RemoveFromNetwork(AIMechanics drone)
    {
        // See if it exists
        foreach (AIMechanics ai in MyAI)
        {
            if (ai == drone)
            {
                MyAI.Remove(ai);
                print("Removed " + drone.name + " from the hive");
            }
        }
    }

    public void ConnectAsPlayer(PlayerMechanics player)
    {
        // Check and make sure it doesn't already exist in the network
        foreach (PlayerMechanics players in Players)
        {
            if (players == player)
            {
                return; // Found it, get outta here
            }
        }
        // If nothing matches, go ahead and add it
        Players.Add(player);
        print("Added " + player.name + " to the hive's players");
    }

    public void RemoveAsPlayer(PlayerMechanics player)
    {
        // See if it exists
        foreach (PlayerMechanics players in Players)
        {
            if (players == player)
            {
                Players.Remove(players);
                print("Removed " + player.name + " from the hive");
            }
        }
    }

    public Vector3 GetPatrolPoint(AIMechanics drone)
    {
        return GetPatrolPoint(drone, 0);
    }

    public Vector3 GetPatrolPoint(AIMechanics drone, int tries)
    {
        if (tries >= 25) { print("Couldn't find patrol point. big bad"); return Vector3.zero; }

        Vector3 ChosenPatrol = PatrolPoints[Random.Range(0, PatrolPoints.Count)];

        // Try until a /different/ patrol point is found
        if (ChosenPatrol == drone.GetMyPatrolTarget()) { ChosenPatrol = GetPatrolPoint(drone, tries + 1); }

        Debug.DrawLine(drone.transform.position, ChosenPatrol, Color.red, ThoughtSpeed);
        AdjustAndContinuePatrol(drone, ChosenPatrol);
        return ChosenPatrol;
    }
}
