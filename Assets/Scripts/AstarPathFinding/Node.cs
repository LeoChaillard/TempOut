using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public Node parent;
    private List<Node> neighbours;
    private int heapIndex;

    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public int FCost 
    { 
        get { return gCost + hCost; }
    }

    public List<Node> Neighbours
    {
        get { return neighbours; }
        set { neighbours = value; }
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

    public Node(bool _walkable, Vector3 _worldPosition, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPosition;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int CompareTo(Node other)
    {
        int compare = FCost.CompareTo(other.FCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(other.hCost);
        }
        return -compare;
    }
}
