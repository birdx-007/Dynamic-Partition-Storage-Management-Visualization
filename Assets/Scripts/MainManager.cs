using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : SingletonMonoBehaviour<MainManager>
{
    public Camera persistentCamera;
    private void Awake()
    {
        SceneManager.LoadScene("MainScene", LoadSceneMode.Additive);
        persistentCamera.gameObject.SetActive(false);
    }
}
