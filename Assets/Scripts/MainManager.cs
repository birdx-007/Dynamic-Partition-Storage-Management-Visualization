using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : SingletonMonoBehaviour<MainManager>
{
    public Camera persistentCamera;
    public EventSystem persistentEventSystem;
    private void Awake()
    {
        SceneManager.LoadScene("MainScene", LoadSceneMode.Additive);
        persistentCamera.gameObject.SetActive(false);
        persistentEventSystem.gameObject.SetActive(false);
    }

    public void PauseOrResume()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
