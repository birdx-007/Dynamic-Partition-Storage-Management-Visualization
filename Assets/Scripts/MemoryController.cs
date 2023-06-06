using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public enum AllocationType
{
    First_Fit = 0,
    Next_Fit,
    Best_Fit,
    Worst_Fit
}
public enum NodeType
{
    H = 0, // 空闲区
    P // 进程
}

public class IdleLinkedListNode
{
    public NodeType type;
    public int start;
    public int size;

    public IdleLinkedListNode(NodeType type, int start, int size)
    {
        this.type = type;
        this.start = start;
        this.size = size;
    }
}

public class MemoryController : SingletonMonoBehaviour<MemoryController>
{
    public GameObject processMemoryPrefab;
    public RectTransform memoryFrameRectTransform;
    public AllocationType allocationType;
    public Slider allocationTypeSlider;
    public LinkedList<IdleLinkedListNode> idleLinkedList;
    private LinkedListNode<IdleLinkedListNode> latestAllocated;
    private void Awake()
    {
        idleLinkedList = new LinkedList<IdleLinkedListNode>();
        //IdleLinkedListNode nodeOS = new IdleLinkedListNode(NodeType.P, 0, MainManager.OSMemorySize);
        //idleLinkedList.AddLast(nodeOS);
        InstantiateMemory(0, MainManager.OSMemorySize,-1);
        IdleLinkedListNode node = new IdleLinkedListNode(NodeType.H, MainManager.OSMemorySize, MainManager.maxMemorySize - MainManager.OSMemorySize);
        idleLinkedList.AddLast(node);
        latestAllocated = idleLinkedList.First;
    }
    public void SetAllocationType()
    {
        allocationType = (AllocationType)allocationTypeSlider.value;
    }

    public void TryAllocate(int size, float time = 8f)
    {
        bool canAllocate;
        int start;
        switch (allocationType)
        {
            case AllocationType.First_Fit:
                canAllocate = Allocate_First_Fit(size,out start);
                break;
            case AllocationType.Next_Fit:
                canAllocate = Allocate_Next_Fit(size,out start);
                break;
            case AllocationType.Best_Fit:
                canAllocate = Allocate_Best_Fit(size,out start);
                break;
            case AllocationType.Worst_Fit:
                canAllocate = Allocate_Worst_Fit(size,out start);
                break;
            default:
                Debug.LogWarning($"No AllocationType {allocationType}.");
                return;
        }
        if (canAllocate)
        {
            InstantiateMemory(start, size, time);
        }
    }
    public void InstantiateMemory(int start, int size,float time)
    {
        GameObject memory = Instantiate(processMemoryPrefab, memoryFrameRectTransform);
        ProcessMemoryBlock controller = memory.GetComponent<ProcessMemoryBlock>();
        controller.Set(start, size);
        controller.StartExisting(time);
    }
    public void MergeAllAdjacentIdlePartitions()
    {
        var current = idleLinkedList.First;
        while (current != null)
        {
            if (current.Value.type == NodeType.H)
            {
                while (current.Next != null && current.Next.Value.type == NodeType.H)
                {
                    current.Value.size += current.Next.Value.size;
                    idleLinkedList.Remove(current.Next);
                }
            }
            current = current.Next;
        }
    }
    public void Release(int start, int size)
    {
        var toBeReleased = new IdleLinkedListNode(NodeType.P,start,size);
        var target =  idleLinkedList.Find(toBeReleased);
        if (target != null)
        {
            target.Value.type = NodeType.H;
            if (target.Previous != null && target.Previous.Value.type == NodeType.H)
            {
                target.Value.size += target.Previous.Value.size;
                target.Value.start = target.Previous.Value.start;
                idleLinkedList.Remove(target.Previous);
            }
            if (target.Next != null && target.Next.Value.type == NodeType.H)
            {
                target.Value.size += target.Next.Value.size;
                idleLinkedList.Remove(target.Next);
            }
        }
    }
    public bool Allocate_First_Fit(int requiredMemorySize,out int start)
    {
        var current = idleLinkedList.First;
        while (current != null)
        {
            if (current.Value.type == NodeType.H && current.Value.size >= requiredMemorySize)
            {
                if(current.Value.size == requiredMemorySize)
                {
                    current.Value.type = NodeType.P;
                    start = current.Value.start;
                    return true;
                }
                IdleLinkedListNode nodeH = new IdleLinkedListNode(NodeType.H, current.Value.start + requiredMemorySize, current.Value.size - requiredMemorySize);
                idleLinkedList.AddAfter(current, nodeH);
                current.Value.type = NodeType.P;
                current.Value.size = requiredMemorySize;
                start = current.Value.start;
                return true;
            }
            current = current.Next;
        }
        Debug.LogWarning("Allocate failed.");
        start = -1;
        return false;
    }
    public bool Allocate_Next_Fit(int requiredMemorySize, out int start)
    {
        var current = latestAllocated;
        do
        {
            if (current.Value.type == NodeType.H && current.Value.size >= requiredMemorySize)
            {
                if (current.Value.size == requiredMemorySize)
                {
                    current.Value.type = NodeType.P;
                    latestAllocated = current.Next;
                    start = current.Value.start;
                    return true;
                }
                IdleLinkedListNode nodeH = new IdleLinkedListNode(NodeType.H, current.Value.start + requiredMemorySize, current.Value.size - requiredMemorySize);
                idleLinkedList.AddAfter(current, nodeH);
                current.Value.type = NodeType.P;
                current.Value.size = requiredMemorySize;
                latestAllocated = current.Next;
                start = current.Value.start;
                return true;
            }
            current = current.Next;
            if (current == null)
            {
                current = idleLinkedList.First;
            }
        }
        while (current != latestAllocated);
        Debug.LogWarning("Allocate failed.");
        start = -1;
        return false;
    }
    public bool Allocate_Best_Fit(int requiredMemorySize,out int start)
    {
        var current = idleLinkedList.First;
        LinkedListNode<IdleLinkedListNode> best = null;
        int bestDeltaSize = MainManager.maxMemorySize - MainManager.OSMemorySize;
        while (current != null)
        {
            if(current.Value.type==NodeType.H && current.Value.size >= requiredMemorySize)
            {
                if (current.Value.size == requiredMemorySize) // already best
                {
                    current.Value.type = NodeType.P;
                    start = current.Value.start;
                    return true;
                }
                else
                {
                    if(current.Value.size - requiredMemorySize < bestDeltaSize)
                    {
                        best = current;
                        bestDeltaSize = best.Value.size - requiredMemorySize;
                    }
                }
            }
            current = current.Next;
        }
        if(best != null)
        {
            IdleLinkedListNode nodeH = new IdleLinkedListNode(NodeType.H, best.Value.start + requiredMemorySize, best.Value.size - requiredMemorySize);
            idleLinkedList.AddAfter(best, nodeH);
            best.Value.type = NodeType.P;
            best.Value.size = requiredMemorySize;
            start = best.Value.start;
            return true;
        }
        Debug.LogWarning("Allocate failed.");
        start = -1;
        return false;
    }
    public bool Allocate_Worst_Fit(int requiredMemorySize,out int start)
    {
        var current = idleLinkedList.First;
        LinkedListNode<IdleLinkedListNode> worst = null;
        int worstDeltaSize = -1;
        while (current != null)
        {
            if (current.Value.type == NodeType.H && current.Value.size >= requiredMemorySize)
            {
                if (current.Value.size - requiredMemorySize > worstDeltaSize)
                {
                    worst = current;
                    worstDeltaSize = worst.Value.size - requiredMemorySize;
                }
            }
            current = current.Next;
        }
        if (worst != null)
        {
            if(worstDeltaSize == 0)
            {
                worst.Value.type = NodeType.P;
                start = worst.Value.start;
                return true;
            }
            IdleLinkedListNode nodeH = new IdleLinkedListNode(NodeType.H, worst.Value.start + requiredMemorySize, worst.Value.size - requiredMemorySize);
            idleLinkedList.AddAfter(worst, nodeH);
            worst.Value.type = NodeType.P;
            worst.Value.size = requiredMemorySize;
            start = worst.Value.start;
            return true;
        }
        Debug.LogWarning("Allocate failed.");
        start = -1;
        return false;
    }
}
