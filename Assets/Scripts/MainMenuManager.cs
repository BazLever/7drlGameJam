using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    public GameObject credits;
    public GameObject mainMenu;

    public int gameScene;

    public LoadingScreenMenu loadingScreen;

    public Button creditsButton;
    public Button backButtonCredits;
    public Button playButton;
    public Button quitButton;

    public float transitionTime;

    bool transitionToCredits;
    bool transitionToMenu;
    bool transitionToGameScene;

    public float timer;

    void Start()
    {
        transitionToCredits = false;
        transitionToMenu = false;

        creditsButton.onClick.AddListener(CreditsOnClick);
        backButtonCredits.onClick.AddListener(BackOnClick);
        playButton.onClick.AddListener(PlayOnClick);
        quitButton.onClick.AddListener(QuitOnClick);
    }

    void Update()
    {

        if (timer > transitionTime)
        {
            if (transitionToCredits)
            {
                ChangeToCredits();
                loadingScreen.DoneLoading();
            }
            if (transitionToMenu)
            {
                ChangeToMenu();
                loadingScreen.DoneLoading();
            }
            if (transitionToGameScene)
            {
                ChangeToGameScene();
            }
        }


        if (transitionToCredits || transitionToMenu || transitionToGameScene)
        {
            timer += Time.deltaTime;
        }
    }

    public void ChangeToCredits()
    {
        credits.SetActive(true);
        mainMenu.SetActive(false);
        transitionToCredits = false;
        transitionToMenu = false;
        timer = 0;
    }

    public void ChangeToMenu()
    {
        credits.SetActive(false);
        mainMenu.SetActive(true);
        transitionToMenu = false;
        transitionToCredits = false;
        timer = 0;
    }


    public void ChangeToGameScene()
    {
        SceneManager.LoadScene(gameScene);
    }

    void CreditsOnClick()
    {
        loadingScreen.NowLoading();
        transitionToCredits = true;
    }

    void BackOnClick()
    {
        loadingScreen.NowLoading();
        transitionToMenu = true;
    }

    void PlayOnClick()
    {
        loadingScreen.NowLoading();
        transitionToGameScene = true;
    }

    void QuitOnClick()
    {
        Application.Quit();
    }


}
