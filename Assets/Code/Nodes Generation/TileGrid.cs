using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileGrid : MonoBehaviour {

    Node[,] grid;
    float nodeDiameter;
    int gridSizeX;
    int gridSizeY;

    public Vector2 gridWorldSize;
    public float nodeRadius;
    public LayerMask unwalkableMask;

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        GenerateGrid();
    }

    void GenerateGrid()
    {
        //GET WORLD LEFT BOTTOM POSITION;
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldLeftBottom = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for(int x = 0; x < gridSizeX; x++)
        {
            for(int y = 0; y < gridSizeY; y++)
            {
                //COMPUTE NODE POSITION FROM WORLD LEFT BOTTON;
                Vector3 nodePosition = worldLeftBottom + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                //CHECK IF NODE IS COLLIDING WITH UNWALKABLE OBSTACLE;
                bool walkable = !(Physics.CheckSphere(nodePosition, nodeRadius, unwalkableMask));

                grid[x, y] = new Node(walkable, nodePosition, x, y);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for(int x = -1; x <= 1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percenX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;  //GET X WORLD POSITION BETWEEN 0 (LEFT EDGE) AND 1 (RIGHT EDGE);
        float percenY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;  //GET Y WORLD POSITION BETWEEN 0 (DOWN EDGE) AND 1 (UP EDGE);
        percenX = Mathf.Clamp01(percenX);   //CLAMP01 FOR POSITIONS OUTSIDE THE WORLD SIZE;
        percenY = Mathf.Clamp01(percenY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percenX);  //SUBSTRACT 1 TO GRID SIZE TO NOT SEARCH OUTSIDE THE ARRAY LENGTH;
        int y = Mathf.RoundToInt((gridSizeY - 1) * percenY);

        return grid[x, y];
    }

    public List<Node> path;
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if(grid != null)
        {
            foreach(Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                if(path != null)
                    if (path.Contains(n))
                        Gizmos.color = Color.black;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }
}
