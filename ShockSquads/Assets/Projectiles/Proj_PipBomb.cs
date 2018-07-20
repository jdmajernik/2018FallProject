using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proj_PipBomb : MonoBehaviour
{
    GameObject overlord_empty;
    AI_Controller overlord;

    GameObject creator;
    public Team good_team;

    [SerializeField] GameObject explosion_prefab;

    float thrust = 40f;

    float damage_max = 70f;
    float damage_min = 30f;

    float range = 10f;

    float tickTime = 0.4f;
    int ticksBeforeBoom = 3;

    bool ticking = false;

    Rigidbody rb;

    void Start()
    {
        overlord_empty = GameObject.Find("EnemyOverlord_Empty");
        overlord = overlord_empty.GetComponent<AI_Controller>();

        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * thrust);
        Invoke("StartTicking", 2f);
    }

    void StartTicking()
    {
        if (!ticking)
        {
            ticking = true;
            StartCoroutine("TickingBomb");
        }
    }

    IEnumerator TickingBomb()
    {
        while (true)
        {
            yield return new WaitForSeconds(tickTime);

            ticksBeforeBoom--;

            if (ticksBeforeBoom <= 0)
            {
                Invoke("Explode", tickTime);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.root.tag != "Player" && collision.transform.root.tag != "Enemy")
        {
            StartTicking();
        }
    }

    void Explode()
    {
        //explosion particle empty
        var exp = Instantiate(explosion_prefab, transform.position, Quaternion.identity);

        //check for enemies in radius
        List<GameObject> ai_enemies = overlord.GetAllEnemies();
        foreach (GameObject enemy in ai_enemies.ToArray())
        {
            var distance = (enemy.transform.position - transform.position).magnitude;
            if (distance < range)
            {
                //Check for things blocking explosion

                var ReachesTarget = false;
                var direction = (enemy.transform.position - transform.position).normalized;

                Debug.DrawRay(transform.position, direction, Color.white, 0.3f);
                Color color = Color.green;

                RaycastHit hit;
                if (Physics.Raycast(transform.position + direction, direction, out hit, range))
                {
                    if (hit.collider.tag == "Player")
                    {
                        ReachesTarget = true;
                    }
                    else if (hit.collider.tag == "Enemy")
                    {
                        ReachesTarget = true;
                    }
                    else
                    {
                        color = Color.red;
                    }
                }
                else
                {
                    color = Color.red;
                }
                Debug.DrawRay(transform.position, direction * range, color, 0.3f);

                if (ReachesTarget)
                {
                    //Linear falloff
                    var damage = (1 - (distance / range)) * damage_max;
                    if (damage < damage_min)
                    {
                        damage = damage_min;
                    }
                    damage = Mathf.RoundToInt(damage);

                    //Apply damage
                    if (enemy.transform.root.tag == "Player")
                    {
                        if (enemy.transform.root.GetComponent<PlayerController>().currentteam != good_team)
                        {
                            enemy.transform.root.GetComponent<PlayerController>().TakeDamage(damage, transform.position);
                        }
                    }
                    if (enemy.transform.root.tag == "Enemy")
                    {
                        if (enemy.transform.root.GetComponent<AI_Enemy>().currentteam != good_team)
                        {
                            enemy.transform.root.GetComponent<AI_Enemy>().TakeDamage(damage, transform.position);
                        }
                    }
                }
            }
        }

        Destroy(gameObject);
    }
}