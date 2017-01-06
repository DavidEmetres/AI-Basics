using UnityEngine;
using System.Collections;

public class Node {

    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost;   //DISTANCE TO START NODE;
    public int hCost;   //DISTANCE TO END NODE;
    public Node parent;

    public int fCost
    {
        get { return gCost + hCost; }
    }

    public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
    }
}
