using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WalkingEnemy : MonoBehaviour
{
    [Header("Health Variables")]
    public float currentHealth;
    public float startingHealth = 35;
    public float maxHealth = 35;
    public bool isDead = false;

    [Space]
    [Header("Movement Variables:")]
    public float lowerSpeed = 5;

    public float walkSpeed = 15;

    [Space]
    [Header("Lunge Variables:")]
    public float lungeSpeed = 25;
    public float distanceFromPlayerToLunge = 3;
    public float heightLevelDifferenceFromPlayerToLunge = 1.5f;
    public float lungeDistance = 6;
    private bool lunging = false;
    private bool initialisedLunge = false;

    private NavMeshAgent agent;
    private PlayerController player;
    private Transform playerPos;

    void Start()
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        player = playerPos.gameObject.GetComponent<PlayerController>();

        currentHealth = startingHealth;
    }

    void Update()
    {
        if (isDead)
            Destroy(gameObject);

        if (agent != null && agent.isOnNavMesh)
        {
            print("WE'VE DONE IT");
        }

        // place the agent on the nav mesh
        RaycastHit hit;
        if (agent == null && Physics.Raycast(transform.position, Vector3.down, out hit, lowerSpeed * Time.deltaTime, ~gameObject.layer))
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
            agent.baseOffset = -0.09f;
            agent.speed = walkSpeed;

            if (!agent.isOnNavMesh)
            {
                agent.transform.position = hit.point;
                agent.enabled = false;
                agent.enabled = true;
            }
        }
        else if (agent == null)
        {
            transform.position -= Vector3.up * lowerSpeed * Time.deltaTime;
        }
        else if (!agent.isOnNavMesh && Physics.Raycast(transform.position, Vector3.down, out hit, 1, ~gameObject.layer))
        {
            agent.transform.position = hit.point;
            agent.enabled = false;
            agent.enabled = true;
        }

        if (agent != null && agent.isOnNavMesh)
        {
            if (!lunging)
            {
                // check if player is within lunge distance
                if (new Vector3(transform.position.x - playerPos.position.x, 0, transform.position.z - playerPos.position.z).sqrMagnitude <= lungeDistance * lungeDistance &&
                    Mathf.Abs(transform.position.y - playerPos.position.y) <= heightLevelDifferenceFromPlayerToLunge)
                {
                    lunging = true;
                }
                else
                {
                    // keep pathing toward the player
                    agent.SetDestination(playerPos.position);
                }
            }
            else
            {
                Lunge();
            }
        }
    }

    private void Lunge()
    {
        if (!initialisedLunge)
        {
            // set the target to be behind the player
            agent.SetDestination((transform.position - playerPos.position).normalized * lungeDistance);

            agent.speed = lungeSpeed;

            initialisedLunge = true;
        }
        else
        {
            // exit the lunge
            if (agent.remainingDistance <= 0.05f)
            {
                agent.speed = walkSpeed;
                lunging = false;
                initialisedLunge = false;
            }
        }
    }

    public void TakeDamage(float damage, Vector3 knockback)
    {
        // make sure the current health is not over the max
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        currentHealth -= damage;

        // become dead if there is no health left
        if (currentHealth <= 0)
            isDead = true;

        // add the knockback
        if (agent != null)
            agent.velocity += knockback;
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;

        // make sure the current health is not over the max
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }
}
