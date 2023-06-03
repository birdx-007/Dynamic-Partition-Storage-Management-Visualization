using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum NodeType
{
    H = 0, // 空闲区
    P // 进程
}

public class IdleLinkedListNode
{
    public NodeType type;
    public int start;
    public int length;

    public IdleLinkedListNode(NodeType type, int start, int length)
    {
        this.type = type;
        this.start = start;
        this.length = length;
    }
}

public class IdleLinkedListController : MonoBehaviour
{
    public LinkedList<IdleLinkedListNode> IdleLinkedList;
}
