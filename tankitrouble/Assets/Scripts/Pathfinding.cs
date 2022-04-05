using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {
    public Grid grid;
    public Transform target;

    Node GetStartNodeFromBBox() {
        Bounds bounds = GetComponentInChildren<Renderer>().bounds;
        for (int x = -1; x <= 1; x += 2) {
            for (int y = -1; y <= 1; y += 2) {
                Vector2 newPos = new Vector2(bounds.center[0] + x * bounds.extents[0], bounds.center[1] + y * bounds.extents[1]);
                Node node = grid.NodeFromWorldPos(newPos);
                if (node.walkable) return node;
            }
        }
        return null;
        // return grid.NodeFromWorldPos(transform.position);
    }

    public List<Node> FindPath() {
        Vector2 startPos = transform.position;
        Vector2 targetPos = target.position;

        Node start = grid.NodeFromWorldPos(startPos);
        if (!start.walkable) {
            Debug.Log("unwalkable start");
            // wrong position assigned. It probably ran into a wall
            start = GetStartNodeFromBBox();
        }
        Node targetNode = grid.NodeFromWorldPos(targetPos);

        // Debug.Log("finding path from " + start.gridX + "," + start.gridY + " to " + targetNode.gridX + "," + targetNode.gridY + ".");

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

        // tank cant navigate corners well
        if (distX == 1 && distY == 1) {
            bool aWalkable = grid.grid[a.gridX, b.gridY].walkable;
            bool bWalkable = grid.grid[b.gridX, a.gridY].walkable;
            if (!aWalkable && !bWalkable) {
                // cant go through here at all
                return 1000;
            }
            if (!aWalkable || !bWalkable) {
                return 30; // set to 30 to prevent diagonal movement
            }
        }

        if (distX > distY) return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);
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
