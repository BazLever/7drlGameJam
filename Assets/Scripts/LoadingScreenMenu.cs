using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenMenu : MonoBehaviour
{
    public Animator loadingScreen;

    void Start()
    {
        
    }
    void Update()
    {
        
    }

    public void NowLoading()
    {
        loadingScreen.SetBool("isLoadingFinished", true);
    }

    public void DoneLoading()
    {
        loadingScreen.SetBool("isLoadingFinished", false);
    }

}
