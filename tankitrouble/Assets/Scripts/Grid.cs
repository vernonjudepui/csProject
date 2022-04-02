using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {
    public bool showGizmos = false;
    public LayerMask unwalkableMask;
    public int gridSizeX;
    public int gridSizeY;
    public float nodeWidth;

    Vector2 worldSize;
    public Node[,] grid;

    void Start() {
        worldSize = new Vector2(gridSizeX * nodeWidth, gridSizeY * nodeWidth);

        // create grid
        grid = new Node[gridSizeX, gridSizeY];
        Vector2 worldBottomLeft = (Vector2)transform.position - Vector2.right * worldSize.x / 2 - Vector2.up * worldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++) {
            for (int y = 0; y < gridSizeY; y++) {
                Vector2 nodeCenter = worldBottomLeft + Vector2.right * (x * nodeWidth + nodeWidth / 2) + Vector2.up * (y * nodeWidth + nodeWidth / 2);
                bool walkable = !(Physics2D.OverlapCircle(nodeCenter, nodeWidth / 2, unwalkableMask));
                grid[x, y] = new Node(walkable, nodeCenter, x, y);
            }
        }
    }

    public Node NodeFromWorldPos(Vector2 worldPos) {
        float percentX = (worldPos.x + worldSize.x / 2) / worldSize.x;
        float percentY = (worldPos.y + worldSize.y / 2) / worldSize.y;
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    public List<Node> GetNeighbours(Node node) {
        List<Node> neighbours = new List<Node>();

        for (int dx = -1; dx <= 1; dx++) {
            for (int dy = -1; dy <= 1; dy++) {
                if (dx == 0 && dy == 0) continue;

                int checkX = node.gridX + dx;
                int checkY = node.gridY + dy;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public List<Node> path;

    void OnDrawGizmos() {
        if (!showGizmos) return;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridSizeX * nodeWidth, gridSizeY * nodeWidth, 0));

        if (grid != null) {
            foreach (Node n in grid) {
                Gizmos.color = n.walkable ? Color.white : Color.red;
                if (path != null && path.Contains(n)) {
                    Gizmos.color = Color.black;
                }
                Gizmos.DrawCube(n.position, Vector2.one * (nodeWidth - .1f));
            }
        }
    }
}
