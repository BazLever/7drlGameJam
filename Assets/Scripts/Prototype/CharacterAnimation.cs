using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    public Animator playerAnimation;

    public float attackLength = 0.7f;


    float timer;

    private void Update()
    {
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

}
