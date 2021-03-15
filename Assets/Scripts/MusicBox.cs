using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicBox : MonoBehaviour
{
    public AudioClip[] musicSelection;
    public AudioSource backgroundMusic;

    int songPlaying = 0;

    bool gameHasLoaded;

    void Start()
    {
        gameHasLoaded = false;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (backgroundMusic.isPlaying == false)
        {
            if (songPlaying == 0)
            {
                songPlaying = 1;
                backgroundMusic.clip = musicSelection[songPlaying];
            } else if (songPlaying == 1)
            {
                songPlaying = 2;
                backgroundMusic.clip = musicSelection[songPlaying];
            }
            else if (songPlaying == 2)
            {
                songPlaying = 0;
                backgroundMusic.clip = musicSelection[songPlaying];
            }
            backgroundMusic.Play();

        }
        if (gameHasLoaded == false)
        {
            LoadMenuScene();
        }
    }

    void LoadMenuScene()
    {
        SceneManager.LoadScene(1);
        gameHasLoaded = true;
    }
}
