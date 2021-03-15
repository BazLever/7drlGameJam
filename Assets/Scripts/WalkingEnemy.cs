using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WalkingEnemy : MonoBehaviour
{
    public bool isAnElite = false;
    public AttributeTypes attributeType = AttributeTypes.nullAttribute;
    public GameObject attributeUpgradeBody = null;

    [Space]
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
    public float timeBetweenLunges = 2f;
    private float timerBetweenLunges = 0;
    public float lungePrepareTime = 0.4f;
    private float lungePrepareTimer;
    public float distanceFromPlayerToLunge = 4.5f;
    public float heightLevelDifferenceFromPlayerToLunge = 1.5f;
    public float lungeDistance = 7;
    private bool lunging = false;
    private bool initialisedLunge = false;

    [Space]
    [Header("Referances:")]
    public SkinnedMeshRenderer headRenderer;
    public SkinnedMeshRenderer torsoRenderer;
    public SkinnedMeshRenderer legsRenderer;
    public GameObject deathParticle;

    public GameObject attackCollider;

    private RoundManager roundManager;
    private Transform playerPos;
    private Rigidbody rb;
    private Animator anim;
    private NavMeshAgent agent;


    void Start()
    {
        roundManager = GameObject.FindGameObjectWithTag("RoundManager").GetComponent<RoundManager>();
        playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        agent = GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;
        agent.enabled = false;

        attackCollider.SetActive(false);

        currentHealth = startingHealth;
    }

    void Update()
    {
        // attach the agent to the navmesh
        if (!agent.isOnNavMesh)
        {
            TryToEnableAgent();
        }

        // kill the enemy
        if (isDead)
        {
            roundManager.ModifyCurrentEnemyAmount(-1);

            // spawn the player attribute upgrade body
            if (isAnElite)
            {
                GameObject spawnedAttributeUpgradeBody = Instantiate(attributeUpgradeBody, transform.position, transform.rotation);

                PlayerAttributeUpgradePieces attributeUpgradePieces = spawnedAttributeUpgradeBody.GetComponent<PlayerAttributeUpgradePieces>();

                attributeUpgradePieces.attributeType = attributeType;
                attributeUpgradePieces.headRenderer.material = headRenderer.material;
                attributeUpgradePieces.torsoRenderer.material = torsoRenderer.material;
                attributeUpgradePieces.legsRenderer.material = legsRenderer.material;
            }
            else // spawn the body parts particle effect
            {
                Instantiate(deathParticle, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }

        // tell the animator if the enemy is grounded
        anim.SetBool("OnGround", Physics.Raycast(transform.position, Vector3.down, 0.25f, ~gameObject.layer));

        if (agent != null && agent.isOnNavMesh)
        {

            if (!lunging)
            {
                // keep pathing toward the player
                if ((playerPos.position - transform.position).sqrMagnitude > distanceFromPlayerToLunge * distanceFromPlayerToLunge)
                {
                    agent.SetDestination(playerPos.position);

                    // start the walking animation
                    anim.SetBool("IsWalking", agent.velocity != Vector3.zero);
                }
                else
                {
                    agent.SetDestination(transform.position);

                    // stop the walking animation
                    anim.SetBool("IsWalking", false);
                }

                // check if player is within lunge distance
                if (timerBetweenLunges <= 0 && new Vector3(playerPos.position.x - transform.position.x, 0, playerPos.position.z - transform.position.z).sqrMagnitude <= lungeDistance * lungeDistance &&
                    Mathf.Abs(playerPos.position.y - transform.position.y) <= heightLevelDifferenceFromPlayerToLunge)
                {
                    // start the punch wind up animation
                    anim.SetTrigger("BeginPunch");

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

            // start the sprinting animation
            //anim.SetBool("IsSprinting", true);

            // start the punch follow through animation
            anim.SetTrigger("FollowThroughPunch");

            attackCollider.SetActive(true);

            initialisedLunge = true;
        }
        else
        {
            // exit the lunge
            if (agent.remainingDistance <= 0.05f)
            {
                agent.speed = walkSpeed;

                // stop the sprint animation
                anim.SetBool("IsSprinting", false);

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

        // stop the sprint animation
        anim.SetBool("IsSprinting", false);

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

    /// <summary>
    /// damages and knocks back this enemy by the specified amount, will also set isDead to true if health is below zero
    /// </summary>
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

        gameObject.GetComponent<AudioSource>().Play();

    }

    /// <summary>
    /// heals this enemy by the specified amount
    /// </summary>
    public void Heal(float healAmount)
    {
        currentHealth += healAmount;

        // make sure the current health is not over the max
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }
}
