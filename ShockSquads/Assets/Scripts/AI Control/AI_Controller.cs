using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Controller : ControllerMechanics {

    List<GameObject> ai_enemies = new List<GameObject>();

    List<GameObject> blue_spawns = new List<GameObject>();
    List<GameObject> red_spawns = new List<GameObject>();

	// Use this for initialization
	void Start()
    {
        blue_spawns.AddRange(GameObject.FindGameObjectsWithTag("Spawn_Blue"));
        print(blue_spawns.Count.ToString() + " blue spawns found.");
        red_spawns.AddRange(GameObject.FindGameObjectsWithTag("Spawn_Red"));
        print(red_spawns.Count.ToString() + " red spawns found.");

        //add any players to AI control
        ai_enemies.AddRange(GetAllPlayers());

        //DecideTeams();
        StartCoroutine(QuarterUpdate());
    }

    // SlowUpdate is called once per quarter second
    IEnumerator QuarterUpdate()
    {
        while (true)
        {
            //Decide each enemy's fate! (state)
            //print("ai enemies has " + ai_enemies.Count.ToString());

            foreach (GameObject enemy in ai_enemies.ToArray())
            {
                var ai = enemy.GetComponent<AI_Enemy>();
                if (ai)
                {
                    if (ai.active)
                    {
                        ai.DecideCurrentState();
                    }
                }
                else
                {
                    //probably a player
                }
            }

            yield return new WaitForSeconds(0.25f);
        }
    }

    void DecideTeams() //NOT ACTIVE
    {
        //Decide each enemy's team
        foreach (GameObject enemy in ai_enemies.ToArray())
        {
            var ai = enemy.GetComponent<AI_Enemy>();
            if (ai)
            {
                if (ai.active)
                {
                    switch (ai.currentteam)
                    {
                        case Team.blue:
                            //enemy.transform.position = blue_spawns[Random.Range(0, blue_spawns.Count)].transform.position;
                            break;
                        case Team.red:
                            //enemy.transform.position = red_spawns[Random.Range(0, red_spawns.Count)].transform.position;
                            break;
                        case Team.neutral:

                            continue;

                        default:
                            print(enemy.name.ToString() + " was not assigned a team!!!");
                            break;
                    }
                }
            }
            else
            {
                print("Couldn't find AI_Enemy Component");
            }
        }
    }

    #region Public List Control

    public void AddToList(GameObject ai_enemy)
    {
        ai_enemies.Add(ai_enemy);
        print("added " + ai_enemy.name.ToString() + " to list");
    }

    public void RemoveFromList(GameObject ai_enemy)
    {
        ai_enemies.Remove(ai_enemy);
        print("removed " + ai_enemy.name.ToString() + " from list");
    }

    public List<GameObject> GetAllEnemies()
    {
        return ai_enemies;
    }

    public List<GameObject> GetAllAI()
    {
        List<GameObject> ai = new List<GameObject>();
        ai.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));

        return ai;
    }

    public List<GameObject> GetAllPlayers()
    {
        List<GameObject> players = new List<GameObject>();
        players.AddRange(GameObject.FindGameObjectsWithTag("Player"));

        return players;
    }

    public List<GameObject> GetSpawns(string team)
    {
        switch (team)
        {
            case "blue":
                return blue_spawns;
            case "red":
                return red_spawns;
            //any other teams that are added

            default:
                return null;
        }
    }

    #endregion

}
