using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Space]
    [Header("Camera Variables:")]
    public float lookSensitivity = 4;
    public float camXRotClamp = 45;
    public float camDistanceFromPlayer = 5;
    public float camDistanceFromObject = 0.05f;
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
    [Header("Referances:")]
    public Transform headPos;
    public Transform cameraParent;
    public Transform cameraPos;
    private CharacterController charController;

    [Space]
    [Header("Controls:")]
    public string horizInput = "Horizontal";
    public string vertiInput = "Vertical";
    public string mouseXInput = "Mouse X";
    public string mouseYInput = "Mouse Y";
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;

    enum MovementStates { walking, jumping, wallClimb, grappling };
    MovementStates moveState;

    private LayerMask playerLayer;

    void Start()
    {
        charController = GetComponent<CharacterController>();

        moveSpeed = walkSpeed;
        moveState = MovementStates.walking;
        playerLayer = LayerMask.GetMask("Player");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        moveDirection = GetMoveDirection();

        // change the move speed to either walk or sprint speed
        moveSpeed = Input.GetKey(sprintKey) ? sprintSpeed : walkSpeed;

        switch (moveState)
        {
            case MovementStates.walking:
                WalkMovement();

                if (Input.GetKeyDown(jumpKey) && charController.isGrounded)
                    moveState = MovementStates.jumping;
                break;

            case MovementStates.jumping:
                Jump();
                break;

            case MovementStates.wallClimb:
                WallClimbMovement();
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
                    if (Vector3.Angle(-transform.forward, wallClimbHit.normal) < startWallClimbAngle && Mathf.Approximately(wallClimbHit.normal.y, 0f))
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

        ApplyPhysics();
    }

    private void LateUpdate()
    {
        CameraMovement();
    }

    private void WalkMovement()
    {
        if (charController.isGrounded)
            charController.Move((moveDirection * moveSpeed + velocity * groundedVelocityMultiplier) * Time.deltaTime);
        else
            charController.Move(((moveDirection * moveSpeed * airControl) + velocity) * Time.deltaTime);
    }

    private void Jump()
    {
        if (!initialisedJump)
        {
            // initialise the jump
            velocity.y = jumpForce;
            if (!wallJumping)
                jumpDir = moveDirection;
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
                //jumpDir = GetMoveDirection();
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
            velocity = new Vector3(0, /*velocity.x + velocity.y + velocity.z +*/ wallClimbingForce, 0);

            initialisedWallClimb = true;
        }
        else
        {
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
                print("Should Jump Off wall");
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
}
