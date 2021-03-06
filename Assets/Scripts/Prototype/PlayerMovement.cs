using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController playerController;

    float currentSpeed;
    public float playerSpeed = 9f;
    public float crouchSpeed = 4f;
    public float sprintSpeed = 1.5f;
    public float jumpHeight;
    public float grav = -10f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 vel;

    public bool isGrounded;
    bool isCrouching;

    public Camera playerCamera;

    //Changeables
    public bool canSprint;
    public bool canCrouch;
    public bool canZoom;

    public Animator playerAnimation;


    void Start()
    {
        currentSpeed = playerSpeed;
        isCrouching = false;
    }


    void Update()
    {

        //Movement Code
        {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && vel.y < 0)
        {
            vel.y = -2f;
        }

        if (isGrounded)
            {
                playerAnimation.SetBool("hasLanded", true);
            }

        if (!isGrounded)
            {
                playerAnimation.SetBool("hasLanded", false);
            }

        float xMovement = Input.GetAxis("Horizontal");
        float zMovement = Input.GetAxis("Vertical");

        Vector3 move = transform.right * xMovement + transform.forward * zMovement;

        playerController.Move(move * currentSpeed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
                isGrounded = false;
            

            vel.y = Mathf.Sqrt(jumpHeight * -2f * grav);
        }

        vel.y += grav * Time.deltaTime;

        playerController.Move(vel * Time.deltaTime);
        }

        //Crouch Code
        if (canCrouch)
        {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (isCrouching)
            {
                isCrouching = false;
            } else if (!isCrouching)
            {
                isCrouching = true;
            }
        }

        if (isCrouching)
        {
            Crouching();
        } else if (!isCrouching)
        {
            Standing();
        }
        }

        //Zoom Code
        if (canZoom)
        { 
        
            if (Input.GetMouseButton(1))
            {
                ZoomIn();
            }
            else
            {
                ZoomOut();
            }
        
        }

        //Sprint Code
        if (canSprint)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                playerSpeed += sprintSpeed;
                isCrouching = false;
            } 
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                playerSpeed -= sprintSpeed;
            }
        }




        //Hacked in (delete if porting)
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
        {
            playerAnimation.SetBool("isMoving", true);
        }
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D))
        {
            playerAnimation.SetBool("isMoving", false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            playerAnimation.SetBool("isAttacking", true);
        }
        if (Input.GetMouseButtonUp(0))
        {
            playerAnimation.SetBool("isAttacking", false);
        }
    }



    void Crouching()
    {
        currentSpeed = crouchSpeed;
        if (playerController.height > 1.5f)
        {
            playerController.height -= 0.2f;
            playerController.center += new Vector3(0f, 0.1f, 0f);
        } else if (playerController.height < 1.5f)
        {
            playerController.height = 1.4f;
            playerController.center = new Vector3(0f, 0.5f, 0f);
        }
    }

    void Standing()
    {
        currentSpeed = playerSpeed;
        if (playerController.height < 3f)
        {
            playerController.height += 0.2f;
            playerController.center -= new Vector3(0f, 0.1f, 0f);
        }
        else if (playerController.height > 3f)
        {
            playerController.height = 3f;
            playerController.center = new Vector3(0f, 0f, 0f);
        }
    }

    void ZoomIn()
    {
        if (playerCamera.fieldOfView != 60)
        {
            playerCamera.fieldOfView -= 5;
        }
    }

    void ZoomOut()
    {
        if (playerCamera.fieldOfView != 90)
        {
            playerCamera.fieldOfView += 5;
        }
    }


}
