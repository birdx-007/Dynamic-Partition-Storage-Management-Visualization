using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProcessMemoryBlock : MonoBehaviour
{
    public RectTransform parentRectTransform;
    private RectTransform rectTransform;
    public float frameWidth;
    public float frameHeight;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentRectTransform = rectTransform.parent.GetComponentInParent<RectTransform>();
        frameWidth = parentRectTransform.rect.width;
        frameHeight = parentRectTransform.rect.height;
    }
    public void Set(int start,int size)
    {
        rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, frameHeight * start / MainManager.maxMemorySize, frameHeight * size / MainManager.maxMemorySize);
    }
}
