using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemyController : MonoBehaviour
{
    public PlayerController player;
    public NavMeshAgent agent;
    public Transform target;



    [Header("Enemy Stats")]
    public int health = 5;
    public int maxHealth = 10;
    public int damageGiven = 1;
    public int damageReceived = 1;
    public float pushBackForce = 5;

    // Start is called before the first frame update
    void Start()
    {
       player = GameObject.Find("Player").GetComponent<PlayerController>();
       agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        target = GameObject.Find("Player").transform;

        agent.destination = target.position;

    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Bullet")
        {
            health -= damageReceived;
            Destroy(collision.gameObject);
        }


        if (collision.gameObject.tag == "Player" && !player.takenDamage)
        {
                player.takenDamage = true;
                player.health -= damageGiven;
                player.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * pushBackForce);
                player.StartCoroutine("cooldownDamage");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            agent.isStopped = false;
            agent.destination = target.position;
        }
    }
}
