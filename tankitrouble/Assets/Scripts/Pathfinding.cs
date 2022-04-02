using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {
    public Grid grid;
    public Transform target;

    public List<Node> FindPath() {
        Vector2 startPos = transform.position;
        Vector2 targetPos = target.position;

        Node start = grid.NodeFromWorldPos(startPos);
        Node targetNode = grid.NodeFromWorldPos(targetPos);

        // Debug.Log("finding path from " + start.gridX + "," + start.gridY + " to " + target.gridX + "," + target.gridY + ".");

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(start);

        while (openSet.Count > 0) {
            Node current = openSet[0];
            for (int i = 1; i < openSet.Count; i++) {
                Node node = openSet[i];
                if (node.fCost < current.fCost || (node.fCost == current.fCost && node.hCost < current.hCost)) {
                    current = node;
                }
            }

            openSet.Remove(current);
            closedSet.Add(current);

            if (current == targetNode) {
                return RetracePath(start, targetNode);
            }

            foreach (Node neighbour in grid.GetNeighbours(current)) {
                if (!neighbour.walkable || closedSet.Contains(neighbour)) {
                    continue;
                }

                int newCostToNeighbour = current.gCost + GetDistance(current, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = current;

                    if (!openSet.Contains(neighbour)) {
                        openSet.Add(neighbour);
                    }
                }
            }
        }
        return new List<Node>();
    }

    int GetDistance(Node a, Node b) {
        int distX = Mathf.Abs(a.gridX - b.gridX);
        int distY = Mathf.Abs(a.gridY - b.gridY);

        if (distX > distY) return 14 * distY + 10 * (distX - distY); // 14
        return 14 * distX + 10 * (distY - distX); // set to 30 to prevent diagonal movement
    }

    List<Node> RetracePath(Node startNode, Node endNode) {
        List<Node> path = new List<Node>();
        Node current = endNode;

        while (current != startNode) {
            path.Add(current);
            current = current.parent;
        }
        path.Reverse();
        return path;
    }
}
