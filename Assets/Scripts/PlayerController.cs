using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Space]
    [Header("Health Variables:")]
    public float currentHealth;
    public float startingHealth = 100;
    public float maxHealth = 100;
    public float invulnerableAfterHitTime = 0.1f;
    private float invulnerableAfterHitTimer;
    private float healOverTimeTimer = 0;
    public bool isDead = false;
    private bool initialisedDeath = false;

    [Space]
    [Header("Camera Variables:")]
    public float lookSensitivity = 4;
    public float camXRotClamp = 45;
    public float camDistanceFromPlayer = 5;
    public float camDistanceFromObject = 0.05f;
    public bool canMoveCamera = true;
    private float pitch = 0;
    private float yaw = 0;

    [Header("Movement Variables:")]
    public float walkSpeed = 12;
    public float sprintSpeed = 18;
    private float moveSpeed;
    public float airControl = 0.75f;
    public float drag = 5;
    public float gravity = 35f;
    public float groundedVelocityMultiplier = 0.5f;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 velocity = Vector3.zero;

    [Space]
    [Header("Jump Variables:")]
    public int numberOfJumps = 1;
    private int numberOfJumpsLeft;
    public float jumpForce = 20;
    public float jumpDirectionMultiplier = 1;
    private bool initialisedJump = false;
    private Vector3 jumpDir;

    [Space]
    [Header("Wall Climb Variables:")]
    [Range(0, 90)]
    public float startWallClimbAngle = 45;
    public float distanceToStartWallClimb = 0.15f;
    public float timeBetweenWallClimbs = 0.1f;
    private float timerBetweenWallClimbs = 0;
    public float wallClimbWallDistance = 0.1f;
    public float wallClimbStrafingMultiplier = 1;
    public float wallClimbingForce = 30;
    public float wallClimbingForceFalloff = 6;
    public float wallClimbJumpOffForce = 12;
    private bool initialisedWallClimb = false;
    private bool wallJumping = false;
    private RaycastHit wallClimbHit;

    [Space]
    [Header("Attack Variables:")]
    public float baseDamage = 15;
    public float knockback = 15;
    [Space]
    public float timeBetweenAttacks = 0.3f;
    private float timerBetweenAttacks = 0;
    public float attackDistanceToLockOn = 2f;
    public float attackLockOnDistance = 1.3f;
    public float attackStepForce = 35;
    public float attackStepFalloff = 5;
    public float attackRegainControlSpeed = 0.15f;
    private float attackStepForceMultiplier;
    private int attackComboLength = 2; // <-- this is the amount of attack animations that are in attack combo
    private int sequentialAttackCount;
    private bool initialisedAttack = false;
    private bool attackingForward = false;
    private bool attackLockedOn = false;
    private List<GameObject> hitEnemies;
    private Vector3 attackDirection = Vector3.zero;


    [Space]
    [Header("References:")]
    public Transform headPos;
    public Transform cameraParent;
    public Transform cameraPos;
    public Slider healthBar;
    public GameObject deathMenu;
    private Animator anim;
    private CharacterController charController;
    private PlayerAttributes playerAttributes;

    public ParticleSystem headDeathParticle;
    public ParticleSystem torsoDeathParticle;
    public ParticleSystem legsDeathParticle;

    // character model referances
    public Transform characterModel;
    private float desiredCharModelYRot = 0;
    private float charModelYRot = 0;

    [Space]
    [Header("Controls:")]
    public string horizInput = "Horizontal";
    public string vertiInput = "Vertical";
    public string mouseXInput = "Mouse X";
    public string mouseYInput = "Mouse Y";
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode attackKey = KeyCode.Mouse0;
    public KeyCode interactKey = KeyCode.E;

    enum MovementStates { walking, jumping, wallClimb, attacking, grappling };
    private MovementStates moveState;

    private LayerMask playerLayer;

    private bool gamePaused = false;


    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        charController = GetComponent<CharacterController>();
        playerAttributes = GetComponent<PlayerAttributes>();

        currentHealth = startingHealth;
        moveSpeed = walkSpeed;
        moveState = MovementStates.walking;
        playerLayer = LayerMask.GetMask("Player");
        invulnerableAfterHitTimer = invulnerableAfterHitTime;
        healthBar.maxValue = maxHealth;
        numberOfJumpsLeft = numberOfJumps;

        hitEnemies = new List<GameObject>();

        deathMenu.SetActive(false);
    }

    void Update()
    {
        // go to main menu as soon as you die - [NOTE: this needs to change]
        if (isDead)
        {
            if (!initialisedDeath)
            {
                // unlock and reveal the cursor
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                // play the death particle effects
                headDeathParticle.Play();
                torsoDeathParticle.Play();
                legsDeathParticle.Play();

                // deactivate the character model
                characterModel.gameObject.SetActive(false);

                canMoveCamera = false;

                deathMenu.SetActive(true);
                deathMenu.GetComponent<Animator>().SetBool("isPaused", true);

                initialisedDeath = true;
            }
            //SceneManager.LoadScene(0);
        }
        else
        {
            moveDirection = GetMoveDirection();

            // set the animation variables
            anim.SetBool("OnGround", Physics.Raycast(transform.position, Vector3.down, charController.height * 0.5f + 0.1f, ~playerLayer));
            anim.SetBool("IsWalking", moveDirection != Vector3.zero);
            anim.SetBool("IsSprinting", Input.GetKey(sprintKey));
            anim.SetBool("IsWallClimbing", moveState == MovementStates.wallClimb);

            switch (moveState)
            {
                case MovementStates.walking:
                    WalkMovement();
                    if (timerBetweenAttacks <= 0)
                    {
                        if (Input.GetKeyDown(attackKey))
                            moveState = MovementStates.attacking;
                    }
                    else
                        timerBetweenAttacks -= Time.deltaTime;
                    break;

                case MovementStates.jumping:
                    Jump();
                    break;

                case MovementStates.wallClimb:
                    WallClimbMovement();
                    break;

                case MovementStates.attacking:
                    Attack();
                    break;

                case MovementStates.grappling:
                    break;
            }

            // check to start the wall run
            if (velocity.y > 0 && (moveState == MovementStates.walking || moveState == MovementStates.jumping))
            {
                // make sure the timer is zero
                if (timerBetweenWallClimbs <= 0)
                {
                    // raycast to check for a wall infront of the player
                    if (Physics.Raycast(transform.position, moveDirection, out wallClimbHit, charController.radius + distanceToStartWallClimb, ~playerLayer))
                    {
                        // make sure the angle on the wall is under the threshold
                        if (Vector3.Angle(-moveDirection, wallClimbHit.normal) < startWallClimbAngle && Mathf.Approximately(wallClimbHit.normal.y, 0f))
                        {
                            // begin wall climb
                            moveState = MovementStates.wallClimb;
                            timerBetweenWallClimbs = timeBetweenWallClimbs;
                            initialisedJump = false;
                        }
                    }
                }
                else
                    timerBetweenWallClimbs -= Time.deltaTime;
            }

            // character model rotation
            charModelYRot = Mathf.LerpAngle(charModelYRot, desiredCharModelYRot, 15f * Time.deltaTime);
            characterModel.localRotation = Quaternion.Euler(0, -pitch + charModelYRot, 0);//Quaternion.Lerp(characterModel.localRotation, Quaternion.Euler(0, desiredCharModelYRot, 0), 15f * Time.deltaTime);

            // make the character model face in the movement direction
            if (moveState == MovementStates.walking || moveState == MovementStates.jumping)
            {
                if (moveDirection != Vector3.zero)
                    desiredCharModelYRot = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            }

            // decrement the invulnerability timer
            if (invulnerableAfterHitTimer > 0)
                invulnerableAfterHitTimer -= Time.deltaTime;

            // heal the player every second by the attribute upgrades amount
            if (healOverTimeTimer <= 0)
            {
                Heal(playerAttributes.GetAttributeModifier(AttributeTypes.healOverTime));
                healOverTimeTimer = 1f;
            }
            else
                healOverTimeTimer -= Time.deltaTime;

            ApplyPhysics();
        }
    }

    private void LateUpdate()
    {
        if (canMoveCamera && !gamePaused)
            CameraMovement();
    }

    private void WalkMovement()
    {
        // change the move speed to either walk or sprint speed
        // this ain't very optimised but that's fine for now
        moveSpeed = (Input.GetKey(sprintKey) ? sprintSpeed : walkSpeed) + playerAttributes.GetAttributeModifier(AttributeTypes.movementSpeed);

        // movement
        if (charController.isGrounded)
            charController.Move((moveDirection * moveSpeed + velocity * groundedVelocityMultiplier) * Time.deltaTime);
        else
            charController.Move(((moveDirection * moveSpeed * airControl) + velocity) * Time.deltaTime);

        // go to jump state if possible
        if (Input.GetKeyDown(jumpKey) && charController.isGrounded)
        {
            numberOfJumpsLeft = numberOfJumps + (int)playerAttributes.GetAttributeModifier(AttributeTypes.numberOfJumps);
            moveState = MovementStates.jumping;
        }
    }

    private void Jump()
    {
        if (!initialisedJump)
        {
            // initialise the jump
            velocity.y = jumpForce + playerAttributes.GetAttributeModifier(AttributeTypes.jumpHeight);

            // get the initial jump direction
            if (!wallJumping)
                jumpDir = moveDirection;

            // start the jump animation
            anim.SetTrigger("Jump");

            numberOfJumpsLeft--;

            initialisedJump = true;
        }
        else
        {
            if (moveDirection != Vector3.zero)
            {
                // calculate the new move direction
                Vector3 movementVec = (moveDirection + jumpDir * jumpDirectionMultiplier) * moveSpeed * airControl + velocity;

                // movement
                charController.Move(movementVec * Time.deltaTime);

                // reset jump direction
                jumpDir = GetMoveDirection();
            }
            else
                charController.Move((jumpDir * jumpDirectionMultiplier * moveSpeed * airControl + velocity) * Time.deltaTime);

            // if player hit their head exit the jump early
            if (IsObjectAbove())
            {
                velocity.y = 0f;
                moveState = MovementStates.walking;
                wallJumping = false;
                initialisedJump = false;
            }

            // check if the player can do more jumps
            if (Input.GetKeyDown(jumpKey) && numberOfJumpsLeft > 0)
            {
                initialisedJump = false;
            }

            // exit jump
            if (velocity.y <= 0f && charController.isGrounded)
            {
                moveState = MovementStates.walking;
                wallJumping = false;
                initialisedJump = false;
            }
        }
    }

    private void WallClimbMovement()
    {
        if (!initialisedWallClimb)
        {
            // set the new velocity
            velocity = new Vector3(0, wallClimbingForce + playerAttributes.GetAttributeModifier(AttributeTypes.wallClimbHeight), 0);

            initialisedWallClimb = true;
        }
        else
        {
            // rotate the character model
            desiredCharModelYRot = Mathf.Atan2(-wallClimbHit.normal.x, -wallClimbHit.normal.z) * Mathf.Rad2Deg;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, -wallClimbHit.normal, out hit, charController.radius + distanceToStartWallClimb, ~playerLayer))
            {
                if (!Mathf.Approximately(hit.normal.y, 0f))
                {
                    // exit the wall climb if the wall is slanted
                    moveState = MovementStates.walking;
                    initialisedWallClimb = false;
                }
            }
            else
            {
                // exit the wall climb if there's no wall
                moveState = MovementStates.walking;
                initialisedWallClimb = false;
            }

            // make the velocity falloff
            velocity.y = Mathf.Lerp(velocity.y, 0, wallClimbingForceFalloff * Time.deltaTime);

            // move the character
            Vector3 movementVec = Vector3.Cross(hit.normal, Vector3.up) * Vector3.Dot(moveDirection, Vector3.Cross(hit.normal, Vector3.up)) * wallClimbStrafingMultiplier + velocity;
            charController.Move(movementVec * Time.deltaTime);


            // jump off wall and exit wall climb
            if (Input.GetKeyDown(jumpKey))
            {
                // add force away from the wall
                if (Vector3.Dot(moveDirection, hit.normal) >= 0.5f)
                {
                    AddForce(moveDirection * wallClimbJumpOffForce);
                    jumpDir = moveDirection;
                }
                else
                {
                    AddForce(hit.normal * wallClimbJumpOffForce);
                    jumpDir = new Vector3(hit.normal.x, 0, hit.normal.z).normalized;
                }


                // exit the wall climb and enter jumping
                moveState = MovementStates.jumping;
                numberOfJumpsLeft = numberOfJumps + (int)playerAttributes.GetAttributeModifier(AttributeTypes.numberOfJumps);
                wallJumping = true;
                initialisedJump = false;
                initialisedWallClimb = false;
            }
            else if (velocity.y < 0.1f || Vector3.Dot(moveDirection, hit.normal) > 0.5f) 
            {
                // exit wall climb if you are no longer ascending or if the move direction is going away from the wall
                moveState = MovementStates.walking;
                initialisedWallClimb = false;
            }
        }
    }

    private void Attack()
    {
        if (!initialisedAttack)
        {
            // initialise the attack
            if (moveDirection != Vector3.zero)
            {
                attackDirection = moveDirection;
                attackingForward = false;
            }
            else
            {
                // attack in the cameras direction if standing still
                attackDirection = transform.forward;

                attackingForward = true;
            }

            // check to lock on attack
            attackLockedOn = false;
            GameObject nearestEnemy = GetNearestEnemy();
            if (nearestEnemy != null && (nearestEnemy.transform.position - transform.position).sqrMagnitude <= (attackDistanceToLockOn * attackDistanceToLockOn))
            {
                Vector3 toEnemy = new Vector3(nearestEnemy.transform.position.x - transform.position.x, 0, nearestEnemy.transform.position.z - transform.position.z).normalized;
                attackDirection = toEnemy;
                charController.Move((nearestEnemy.transform.position - transform.position) - toEnemy * (attackLockOnDistance + charController.radius));
                attackStepForceMultiplier = 0;

                Debug.Log("Locked On");
                attackLockedOn = true;
            }
            else
                attackStepForceMultiplier = attackStepForce;

            sequentialAttackCount++;

            // attack animation
            if (sequentialAttackCount == 1)
                anim.SetTrigger("AttackLeft");
            else if (sequentialAttackCount == 2)
                anim.SetTrigger("AttackRight");

            // reset the attack timer
            timerBetweenAttacks = timeBetweenAttacks;

            hitEnemies.Clear();

            initialisedAttack = true;
        }
        else
        {
            // rotate the character model
            desiredCharModelYRot = Mathf.Atan2(attackDirection.x, attackDirection.z) * Mathf.Rad2Deg;

            // make the attack step falloff
            attackStepForceMultiplier = Mathf.Lerp(attackStepForceMultiplier, 0, attackStepFalloff * Time.deltaTime);

            // actually move in the attack direction
            charController.Move((attackDirection * attackStepForceMultiplier + velocity) * Time.deltaTime);

            // exit attck if the attack step has stopped
            if (attackStepForceMultiplier <= attackRegainControlSpeed || (attackStepForceMultiplier <= moveSpeed * 0.5f && moveDirection != Vector3.zero))
            {
                moveState = MovementStates.walking;
                sequentialAttackCount = 0;
                timerBetweenAttacks = timeBetweenAttacks - playerAttributes.GetAttributeModifier(AttributeTypes.attackSpeed);
                initialisedAttack = false;
            }
            else if (Input.GetKeyDown(attackKey) && timerBetweenAttacks <= 0) // reset the initialiser if the player continues the attack combo
            {
                // reset the sequentialAttackCount if it's reached the end of the combo
                if (sequentialAttackCount >= attackComboLength)
                    sequentialAttackCount = 0;
                initialisedAttack = false;
            }
            else if (timerBetweenAttacks > 0)
                timerBetweenAttacks -= Time.deltaTime;

            // actually attacking the enemies
            RaycastHit[] rayHits = Physics.SphereCastAll(transform.position + attackDirection * 1f, 0.5f, attackDirection, 0f, ~playerLayer);
            if (rayHits.Length > 0)
            {
                float damage = baseDamage + playerAttributes.GetAttributeModifier(AttributeTypes.damage);
                for (int i = 0; i < rayHits.Length; ++i)
                {
                    if (rayHits[i].transform.CompareTag("Enemy"))
                    {
                        // check if the enemy has been hit already
                        bool isNewEnemy = true;
                        for (int j = 0; j < hitEnemies.Count; ++j)
                        {
                            if (rayHits[i].transform.gameObject == hitEnemies[j])
                            {
                                isNewEnemy = false;
                                break;
                            }
                        }

                        // damage the enemy
                        if (isNewEnemy)
                        {
                            WalkingEnemy hitEnemy = rayHits[i].transform.GetComponent<WalkingEnemy>();
                            hitEnemy.TakeDamage(damage, attackDirection * knockback);

                            // lifesteal
                            if (hitEnemy.isDead)
                                Heal(playerAttributes.GetAttributeModifier(AttributeTypes.lifeSteal));

                            hitEnemies.Add(rayHits[i].transform.gameObject);
                            Debug.Log("Player hit enemy");
                        }
                    }
                }
            }
        }
    }

    private void CameraMovement()
    {
        // apply mouse movement to the desired camera rotation
        pitch += Input.GetAxis(mouseXInput) * lookSensitivity;
        yaw -= Input.GetAxis(mouseYInput) * lookSensitivity;

        // dont allow the cameras rotation to look to high or to low
        yaw = Mathf.Clamp(yaw, -camXRotClamp, camXRotClamp);

        // apply pitch to players y rotation
        transform.rotation = Quaternion.Euler(0, pitch, 0);
        // apply yaw to cameras parent x rotation
        headPos.localRotation = Quaternion.Euler(yaw, headPos.localRotation.y, headPos.localRotation.z);

        // stop camera from cliping into walls
        RaycastHit hit;
        if (Physics.Raycast(headPos.position, cameraParent.position - headPos.position, out hit, camDistanceFromPlayer + camDistanceFromObject, ~playerLayer))
            cameraParent.localPosition = new Vector3(0, 0, -(hit.distance - camDistanceFromObject));
        else
            cameraParent.localPosition = new Vector3(0, 0, -(camDistanceFromPlayer - camDistanceFromObject));
    }

    /// <summary>
    /// gets the players desired movement direction based on the players input
    /// </summary>
    private Vector3 GetMoveDirection()
    {
        return (transform.forward * Input.GetAxisRaw(vertiInput) + transform.right * Input.GetAxisRaw(horizInput)).normalized;
    }

    /// <summary>
    /// applies gravity and drag to the velocity then updates the controllers position
    /// </summary>
    private void ApplyPhysics()
    {
        // apply gravity
        if (moveState != MovementStates.wallClimb)
        {
            if (!charController.isGrounded && velocity.y > -gravity)
                velocity.y -= gravity * Time.deltaTime;
            else if (charController.isGrounded)
                velocity.y = Mathf.MoveTowards(velocity.y, 0, drag * Time.deltaTime);
        }

        // apply drag
        velocity.x = Mathf.Lerp(velocity.x, 0, drag * Time.deltaTime);
        velocity.z = Mathf.Lerp(velocity.z, 0, drag * Time.deltaTime);
    }

    /// <summary>
    /// returns true if there is an object above the character controller
    /// </summary>
    private bool IsObjectAbove()
    {
        float rayLength = transform.localScale.y + charController.skinWidth * 1.1f;
        return (
            // center check
            Physics.Raycast(transform.position, Vector3.up, rayLength, ~playerLayer) ||

            // forward and backward checks
            Physics.Raycast(transform.position + new Vector3(0f, 0f, charController.radius), Vector3.up, rayLength, ~playerLayer) ||
            Physics.Raycast(transform.position + new Vector3(0f, 0f, -charController.radius), Vector3.up, rayLength, ~playerLayer) ||

            // right and left checks
            Physics.Raycast(transform.position + new Vector3(charController.radius, 0f, 0f), Vector3.up, rayLength, ~playerLayer) ||
            Physics.Raycast(transform.position + new Vector3(-charController.radius, 0f, 0f), Vector3.up, rayLength, ~playerLayer) ||

            // forward right and forward left checks
            Physics.Raycast(transform.position + new Vector3(charController.radius * 0.5f, 0f, charController.radius * 0.5f), Vector3.up, rayLength, ~playerLayer) ||
            Physics.Raycast(transform.position + new Vector3(-charController.radius * 0.5f, 0f, charController.radius * 0.5f), Vector3.up, rayLength, ~playerLayer) ||

            // backward right and backward left checks
            Physics.Raycast(transform.position + new Vector3(charController.radius * 0.5f, 0f, -charController.radius * 0.5f), Vector3.up, rayLength, ~playerLayer) ||
            Physics.Raycast(transform.position + new Vector3(-charController.radius * 0.5f, 0f, -charController.radius * 0.5f), Vector3.up, rayLength, ~playerLayer));
    }
    
    /// <summary>
    /// adds a force to the velocity
    /// </summary>
    public void AddForce(Vector3 force)
    {
        velocity += force;
    }

    /// <summary>
    /// applies damage to the health and the knockback to the players velocity
    /// </summary>
    public void TakeDamage(float damage, Vector3 knockback)
    {
        // don't do the damage calculations if the player has dodged the attack
        if (Random.Range(0.1f, 100f) < playerAttributes.GetAttributeModifier(AttributeTypes.chanceToBlockDamage))
            return;

        if (invulnerableAfterHitTimer <= 0)
        {
            // make sure the current health is not over the max
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;

            currentHealth -= damage;

            // become dead if there is no health left
            if (currentHealth <= 0)
                isDead = true;

            // add the enemies knockback
            velocity += knockback;

            // reset the invulnerable timer
            invulnerableAfterHitTimer = invulnerableAfterHitTime;
        }

        // set the health bar to the players health
        healthBar.value = currentHealth;
    }

    /// <summary>
    /// heals the player by the given heal amount
    /// </summary>
    public void Heal(float healAmount)
    {
        currentHealth += healAmount;

        // make sure the current health is not over the max
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        // set the health bar to the players health
        healthBar.value = currentHealth;
    }

    /// <summary>
    /// sets if the game is paused for this object
    /// </summary>
    public void SetGamePaused(bool pauseState)
    {
        gamePaused = pauseState;
    }

    /// <summary>
    /// retruns the nearest enemy, or null if there are no enemies
    /// </summary>
    private GameObject GetNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // return null if there's no enemies
        if (enemies.Length == 0)
            return null;

        GameObject closestEnemy = enemies[0];

        for (int i = 1; i < enemies.Length; ++i)
        {
            if ((enemies[i].transform.position - transform.position).sqrMagnitude < (closestEnemy.transform.position - transform.position).sqrMagnitude)
            {
                closestEnemy = enemies[i];
            }
        }

        return closestEnemy;
    }

}
