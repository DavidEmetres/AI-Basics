using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour {

    TileGrid grid;

    public Transform seeker, target;

    void Awake()
    {
        grid = GetComponent<TileGrid>();
    }

    void Update()
    {
        FindPath(seeker.position, target.position);
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        //CREATE OPEN AND CLOSED LIST;
        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        openList.Add(startNode);

        while(openList.Count > 0)
        {
            Node currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                //GET THE OPEN NODE WITH THE BEST F COST; IF TWO NODES HAVE THE SAME F COST GET THE ONE WITH LOWEST H COST;
                if (openList[i].fCost < currentNode.fCost || openList[i].fCost == currentNode.fCost)
                {
                    if (openList[i].hCost < currentNode.hCost)
                        currentNode = openList[i];
                }
            }

            //REMOVE CURRENT FROM OPEN;
            openList.Remove(currentNode);
            //ADD CURRENT TO CLOSED;
            closedList.Add(currentNode);

            //REACHED TARGET NODE;
            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                openList.Clear();
                return;
            }

            //CHECK EVERY NEIGHBOUR;
            foreach(Node neighbour in grid.GetNeighbours(currentNode))
            {
                //IF THE NEIGHBOUR IS AN OBSTACLE OR IS ALREADY VISITED CONTINUE;
                if (!neighbour.walkable || closedList.Contains(neighbour))
                    continue;

                int costMovementToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                //IF THIS PATH COST TO THE START NODE IS LOWER THAN THE NEIGHBOUR COST TO START NODE;
                //OR IF THE NEIGHBOUR IS NOT CONTAINED IN THE OPEN LIST;
                //MEANS THAT THIS PATH IS BETTER;
                if(costMovementToNeighbour < neighbour.gCost || !openList.Contains(neighbour))
                {
                    neighbour.gCost = costMovementToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openList.Contains(neighbour))
                        openList.Add(neighbour);
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        grid.path = path;
    } 

    int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        //DISTANCE BETWEEN NODES VERTICALLY AND HORIZONTALLY IS 1 AND DIAGONALLY IS SQUARE ROOT OF 2 (APROX. 1.4) SO WE USE THE INTEGERS 10 AND 14;
        //WE MULTIPLY 14 TIMES THE LOWER VALUE BECAUSE WE MOVE THAT NUMBER DIAGONALLY, AND THE REST (GREATER - LOWER) WE MOVE HORIZONTALLY OR VERTICALLY, SO TIMES 10;
        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);

        return 14 * distX + 10 * (distY - distX);
    }
}
