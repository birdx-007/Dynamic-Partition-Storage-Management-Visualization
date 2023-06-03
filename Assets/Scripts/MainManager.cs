using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : SingletonMonoBehaviour<MainManager>
{
    private void Awake()
    {
        SceneManager.LoadScene("Persistent", LoadSceneMode.Additive);
    }
}
