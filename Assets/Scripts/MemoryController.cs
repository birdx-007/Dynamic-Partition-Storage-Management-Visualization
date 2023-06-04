using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions.Must;

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

public class MemoryController : MonoBehaviour
{
    public LinkedList<IdleLinkedListNode> idleLinkedList;
    private LinkedListNode<IdleLinkedListNode> latestAllocated;
    private void Awake()
    {
        idleLinkedList = new LinkedList<IdleLinkedListNode>();
        //IdleLinkedListNode nodeOS = new IdleLinkedListNode(NodeType.P, 0, MainManager.OSMemorySize);
        IdleLinkedListNode node = new IdleLinkedListNode(NodeType.H, MainManager.OSMemorySize, MainManager.maxMemorySize - MainManager.OSMemorySize);
        //idleLinkedList.AddLast(nodeOS);
        idleLinkedList.AddLast(node);
        latestAllocated = idleLinkedList.First;
    }
    public bool Allocate_First_Fit(int requiredMemorySize)
    {
        var current = idleLinkedList.First;
        while (current != null)
        {
            if (current.Value.type == NodeType.H && current.Value.size >= requiredMemorySize)
            {
                if(current.Value.size == requiredMemorySize)
                {
                    current.Value.type = NodeType.P;
                    return true;
                }
                IdleLinkedListNode nodeH = new IdleLinkedListNode(NodeType.H, current.Value.start + requiredMemorySize, current.Value.size - requiredMemorySize);
                idleLinkedList.AddAfter(current, nodeH);
                current.Value.type = NodeType.P;
                current.Value.size = requiredMemorySize;
                return true;
            }
            current = current.Next;
        }
        Debug.LogWarning("Allocate failed.");
        return false;
    }
    public bool Allocate_Next_Fit(int requiredMemorySize)
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
                    return true;
                }
                IdleLinkedListNode nodeH = new IdleLinkedListNode(NodeType.H, current.Value.start + requiredMemorySize, current.Value.size - requiredMemorySize);
                idleLinkedList.AddAfter(current, nodeH);
                current.Value.type = NodeType.P;
                current.Value.size = requiredMemorySize;
                latestAllocated = current.Next;
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
        return false;
    }
    public bool Allocate_Best_Fit(int requiredMemorySize)
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
            return true;
        }
        Debug.LogWarning("Allocate failed.");
        return false;
    }
    public bool Allocate_Worst_Fit(int requiredMemorySize)
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
                return true;
            }
            IdleLinkedListNode nodeH = new IdleLinkedListNode(NodeType.H, worst.Value.start + requiredMemorySize, worst.Value.size - requiredMemorySize);
            idleLinkedList.AddAfter(worst, nodeH);
            worst.Value.type = NodeType.P;
            worst.Value.size = requiredMemorySize;
            return true;
        }
        Debug.LogWarning("Allocate failed.");
        return false;
    }
}
