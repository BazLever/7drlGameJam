using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public Animator pauseMenuAnim;
    public Animator loadingScreen;

    private PlayerController playerController;
    private PieceSelectionUI pieceSelection;
    private bool isPaused = false;

    public float transitionTime = 1.6f;
    private float transitionTimer;
    private bool transitionToMainMenu = false;

    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        pieceSelection = GameObject.FindGameObjectWithTag("PieceSelection").GetComponent<PieceSelectionUI>();

        transitionTimer = transitionTime;

        // lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            if (!isPaused)
            {
                // unlock and reveal the cursor
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                pauseMenuAnim.SetBool("isPaused", true);
                isPaused = true;
                Time.timeScale = 0;
            } else if (isPaused)
            {
                // lock and hide the cursor if the part selection menu is not open
                if (!pieceSelection.IsOpen())
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }

                pauseMenuAnim.SetBool("isPaused", false);
                isPaused = false;
                Time.timeScale = 1;
            }

            // tell the player and piece selection menu if the game is paused or not
            playerController.SetGamePaused(isPaused);
            pieceSelection.SetGamePaused(isPaused);
        }

        // transition to main menu
        if (transitionToMainMenu)
        {
            if (transitionTimer <= 0)
                SceneManager.LoadScene(1);
            else
                transitionTimer -= Time.deltaTime;
        }

    }

    public void ResumeButton()
    {
        // lock and hide the cursor if the part selection menu is not open
        if (!pieceSelection.IsOpen())
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        pauseMenuAnim.SetBool("isPaused", false);
        isPaused = false;
        Time.timeScale = 1;

        // tell the player and piece selection menu if the game is paused or not
        playerController.SetGamePaused(isPaused);
        pieceSelection.SetGamePaused(isPaused);
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
