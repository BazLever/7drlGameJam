using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    public Animator loadingScreen;

    public float transitionTime = 1.6f;
    private float transitionTimer;
    private bool transitionToMainMenu = false;

    void Start()
    {
        transitionTimer = transitionTime;
    }

    void Update()
    {
        // transition to main menu
        if (transitionToMainMenu)
        {
            if (transitionTimer <= 0)
                SceneManager.LoadScene(1);
            else
                transitionTimer -= Time.deltaTime;
        }
    }

    public void MainMenuButton()
    {
        transitionToMainMenu = true;
        loadingScreen.SetBool("isLoadingFinished", true);
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
