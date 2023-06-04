using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : SingletonMonoBehaviour<MainManager>
{
    public Camera persistentCamera;
    public const int maxMemorySize = 256;
    public const int OSMemorySize = 20;
    private void Awake()
    {
        SceneManager.LoadScene("MainScene", LoadSceneMode.Additive);
        persistentCamera.gameObject.SetActive(false);
    }
}
