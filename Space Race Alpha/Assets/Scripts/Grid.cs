using UnityEngine;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

    public LayerMask unwalkableMask;
    private Vector2 gridWorldSize;
    public float nodeRadius = 1;
    public Node[,] grid;
    public List<Node> walkableNodes;

    protected GameManager gameManager;

    float nodeDiameter;
    public int gridSizeX, gridSizeY;

    void Awake()
    {
        //Get info from gameManager
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        gridWorldSize = gameManager.gridWorldSize;
        nodeRadius = gameManager.nodeRadius;
        unwalkableMask = gameManager.unwalkableMask;

        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        CreateGrid();
    }

    public void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;
        walkableNodes = new List<Node>();

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics2D.OverlapCircle(worldPoint, nodeRadius,unwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint);
                if (walkable)
                {
                    walkableNodes.Add(grid[x, y]);
                }
            }
        }
    }

    public Node NodeFromWorldPosition(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentX = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }
	void OnDrawGizmos()
    {
        //Gizmos.DrawCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y));

        if (grid != null)
        {
            foreach ( Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));

            }
        }
    }
}
