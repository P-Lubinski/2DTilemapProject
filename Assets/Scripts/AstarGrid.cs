using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarGrid : MonoBehaviour
{
    public LayerMask walkableMask;      // layer which we will be able to walk on
    public Vector2 gridWorldSize;       // area in world coordiantes 
    public float nodeRadius;            // how much space each node covers
    float nodeDiameter;
    AstarNode[,] grid;                  // 2d array of nodes to represent the grid
    int gridSizeX, gridSizeY;

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;                                // size of the node
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter); // how many nodes can fit into the grid on x...
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter); // ... and y axis (rounds up to nearest integer)
    }

    public void CreateGrid()
    {
        grid = new AstarNode[gridSizeX, gridSizeY]; // new 2d array of nodes of size determined above
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++) // this forloop will look at all the nodes and determine which node will represent a walkable tile
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics2D.OverlapCircle(worldPoint, nodeRadius, walkableMask)); // this will return a collision if node collides with a walkable tile
                grid[x, y] = new AstarNode(walkable, worldPoint); // populate walkable nodes
            }
        }
    }

    void OnDrawGizmos()  // mainly for debugging
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1)); // draws a wire-cube showing the area in which nodes will be drawn

        if (grid != null)
        {
            foreach (AstarNode n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red; // this drwas the red nodes where player can walk, and white where player can't
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .25f));
            }
        }
    }
}
