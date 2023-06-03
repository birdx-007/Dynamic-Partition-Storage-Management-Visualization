using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T: Component
{
    public static T Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
        }
        else
        {
            Destroy(Instance);
            Debug.LogError($"There is already one SingletonMonoBehaviour {name} !");
        }
    }

    public void DestroyInstance()
    {
        Instance = null;
    }
}
