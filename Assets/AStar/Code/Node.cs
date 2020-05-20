using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost = Int32.MaxValue;
    public int hCost = Int32.MaxValue;

    public Node parent = null;

    private int heapIndex;

    public Node(bool _walkAbel, Vector3 _worldPos, int _gridX, int _gridY)
    {
        this.walkable = _walkAbel;
        this.worldPosition = _worldPos;
        this.gridX = _gridX;
        this.gridY = _gridY;
    }

    public int fCost
    {
        get { return gCost + hCost; }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }

        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node other)
    {
        int compare = fCost.CompareTo(other.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(other.hCost);
        }
        return -compare;
    }
}