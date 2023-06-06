using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ProcessMemoryBlock : MonoBehaviour
{
    private MemoryController memoryController;
    public Image fillImage;
    private RectTransform parentRectTransform;
    private RectTransform rectTransform;
    private float frameWidth;
    private float frameHeight;
    public float existTime;
    private bool isExisting = false;
    private float leftExistTime;
    private int start;
    private int size;
    private void Awake()
    {
        memoryController = GameObject.FindWithTag("GameController").GetComponent<MemoryController>();
        rectTransform = GetComponent<RectTransform>();
        parentRectTransform = rectTransform.parent.GetComponentInParent<RectTransform>();
        frameWidth = parentRectTransform.rect.width;
        frameHeight = parentRectTransform.rect.height;
    }

    private void Update()
    {
        if (isExisting && existTime > 0)
        {
            leftExistTime -= Time.deltaTime;
            if (leftExistTime <= 0)
            {
                leftExistTime = 0;
                isExisting = false;
                memoryController.Release(start,size);
                Destroy(gameObject);
            }
            fillImage.fillAmount = leftExistTime / existTime;
        }
    }

    public void Set(int start,int size)
    {
        this.start = start;
        this.size = size;
        rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, frameHeight * start / MemoryController.maxMemorySize, frameHeight * size / MemoryController.maxMemorySize);
    }

    public void StartExisting(float time)
    {
        leftExistTime = existTime = time;
        isExisting = true;
    }
}
