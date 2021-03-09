using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public Animator pauseMenuAnim;
    bool isPaused;

    void Start()
    {
        isPaused = false;   
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                pauseMenuAnim.SetBool("isPaused", true);
                isPaused = true;
                Time.timeScale = 0;
            } else if (isPaused)
            {
                pauseMenuAnim.SetBool("isPaused", false);
                isPaused = false;
                Time.timeScale = 1;
            }
            
        }
    }
}
