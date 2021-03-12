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
    public float walkSpeed = 10;

    [Space]
    [Header("Lunge Variables:")]
    public float lungeSpeed = 25;
    public float timeBetweenLunges = 2.5f;
    private float timerBetweenLunges = 0;
    public float lungePrepareTime = 0.75f;
    private float lungePrepareTimer;
    public float distanceFromPlayerToLunge = 4.5f;
    public float heightLevelDifferenceFromPlayerToLunge = 1.5f;
    public float lungeDistance = 6;
    private bool lunging = false;
    private bool initialisedLunge = false;

    [Space]
    [Header("Referances:")]
    public GameObject attackCollider;

    private RoundManager roundManager;
    private Transform playerPos;
    private Rigidbody rb;
    private NavMeshAgent agent;


    void Start()
    {
        roundManager = GameObject.FindGameObjectWithTag("RoundManager").GetComponent<RoundManager>();
        playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();

        agent = gameObject.GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;
        agent.enabled = false;

        attackCollider.SetActive(false);

        currentHealth = startingHealth;
    }

    void Update()
    {
        if (isDead)
        {
            roundManager.ModifyCurrentEnemyAmount(-1);
            Destroy(gameObject);
        }

        if (!agent.isOnNavMesh)
        {
            TryToEnableAgent();
        }


        if (agent != null && agent.isOnNavMesh)
        {

            if (!lunging)
            {
                // keep pathing toward the player
                if ((playerPos.position - transform.position).sqrMagnitude > distanceFromPlayerToLunge * distanceFromPlayerToLunge)
                    agent.SetDestination(playerPos.position);
                else
                    agent.SetDestination(transform.position);

                // check if player is within lunge distance
                if (timerBetweenLunges <= 0 && new Vector3(playerPos.position.x - transform.position.x, 0, playerPos.position.z - transform.position.z).sqrMagnitude <= lungeDistance * lungeDistance &&
                    Mathf.Abs(playerPos.position.y - transform.position.y) <= heightLevelDifferenceFromPlayerToLunge)
                {
                    lungePrepareTimer = lungePrepareTime;
                    lunging = true;
                }
                else if(timerBetweenLunges > 0)
                {
                    timerBetweenLunges -= Time.deltaTime;
                }
            }
            else
            {
                if (lungePrepareTimer <= 0)
                    Lunge();
                else
                {
                    // decrament the timer and stand still
                    lungePrepareTimer -= Time.deltaTime;
                    agent.speed = 0;
                }
            }
        }
    }

    private void Lunge()
    {
        if (!initialisedLunge)
        {
            // set the target to be behind the player
            agent.SetDestination(transform.position + new Vector3(playerPos.position.x - transform.position.x, 0, playerPos.position.z - transform.position.z).normalized * lungeDistance);

            agent.speed = lungeSpeed;

            attackCollider.SetActive(true);

            initialisedLunge = true;
        }
        else
        {
            // exit the lunge
            if (agent.remainingDistance <= 0.05f)
            {
                agent.speed = walkSpeed;

                // reset the lunge
                attackCollider.SetActive(false);
                timerBetweenLunges = timeBetweenLunges;
                lunging = false;
                initialisedLunge = false;
            }
        }
    }

    public void StopLunging()
    {
        agent.speed = walkSpeed;

        // reset the lunge
        attackCollider.SetActive(false);
        timerBetweenLunges = timeBetweenLunges;
        lunging = false;
        initialisedLunge = false;
    }

    private void TryToEnableAgent()
    {
        // place the agent on the nav mesh
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f, ~gameObject.layer))
        {
            rb.useGravity = false;

            transform.position = hit.point + new Vector3(0, 0.08334f, 0);
            agent.enabled = false;
            agent.enabled = true;

            if (agent.isOnNavMesh)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.constraints = RigidbodyConstraints.FreezeAll;
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
