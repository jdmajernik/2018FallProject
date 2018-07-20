using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Team { neutral, red, blue};

public class ControllerMechanics : MonoBehaviour {

    float health = 100;
    float shield = 100;

    int respawn_time = 10;

    public Team currentteam;
    public Team good_team;

    AI_Controller ai_controller;
    [SerializeField] GameObject hit_prefab;

    void Start ()
    {

    }

    #region Public Gets

    

    #endregion

    #region Public Manual Changes

    public void ChangeHealth(float new_value)
    {
        if (new_value > 0)
        {
            health = new_value;
        }
    }

    public void ChangeShield(float new_value)
    {
        if (new_value > 0)
        {
            shield = new_value;
        }
    }

    #endregion

    void DisplayDamage(float amount)
    {
        var hit = Instantiate(hit_prefab, transform.position, Quaternion.identity);
        hit.transform.Find("Canvas").transform.Find("Text").GetComponent<Text>().text = amount.ToString();
    }

    public virtual void TakeDamageThroughController(float amount)
    {
        var damage_left = amount;

        if (shield > 0)
        {
            if (shield - damage_left > 0)
            {
                DisplayDamage(damage_left);
                shield -= damage_left;
                damage_left = 0;
            }
            else
            {
                DisplayDamage(damage_left - shield);
                damage_left -= shield;
                shield = 0;
            }
        }
        if (damage_left > 0)
        {
            if (health > 0)
            {
                if (health - damage_left > 0)
                {
                    DisplayDamage(damage_left);
                    health -= damage_left;
                    damage_left = 0;
                }
                else
                {
                    DisplayDamage(damage_left - health);
                    health = 0;
                    Death();
                    return;
                }
            }
        }

        if (health <= 0)
        {
            Death();
        }

        //print("health(" + health.ToString() + ") shield(" + shield.ToString() + ")");
    }

    public virtual void Death()
    {
        ai_controller = GameObject.Find("EnemyOverlord_Empty").GetComponent<AI_Controller>();
        if (ai_controller)
        {
            ai_controller.RemoveFromList(gameObject);
        }

        //deactivate enemy
        gameObject.SetActive(false);
        AI_Enemy enm = gameObject.GetComponent<AI_Enemy>();
        if (enm)
        {
            enm.active = false;
        }
        else
        {
            gameObject.GetComponent<PlayerController>().ShowRespawnTimer();
        }

        //start timer for respawn
        Invoke("Respawn", respawn_time);
    }

    public virtual void Respawn()
    {
        //find spawn
        Vector3 new_spawn = new Vector3(0,0,0);

        ai_controller = GameObject.Find("EnemyOverlord_Empty").GetComponent<AI_Controller>();
        if (ai_controller)
        {
            List<GameObject> good_spawns = ai_controller.GetSpawns(good_team.ToString());
            new_spawn = good_spawns[Random.Range(0, good_spawns.Count)].transform.position;
        }

        //move there
        if (new_spawn != new Vector3(0,0,0))
        {
            gameObject.transform.position = new_spawn;
        }

        //put them back in the system if they are ai
        PlayerController pc = gameObject.GetComponent<PlayerController>();
        if (pc)
        {
            ai_controller.AddToList(gameObject);
        }

        health = 100;
        shield = 100;

        gameObject.SetActive(true);
        AI_Enemy enm = gameObject.GetComponent<AI_Enemy>();
        if (enm)
        {
            enm.active = true;
            enm.Restart();
        }
        else
        {
            gameObject.GetComponent<PlayerController>().Restart();
        }
    }
}
