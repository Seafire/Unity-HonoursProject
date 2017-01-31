using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour 
{
	public bool isGridGizmoDisplayed;		/* Are the Gizmos related to the grid layout being displayed */ 
	public LayerMask unwalkableMask;		/* Mask for all world objects that cannot be passed through */
	public Vector2 gridWorldSize;			/* The size of the grid that will be used for pathfinding */
	public float nodeRadius;				/* The size of each node */ 
	Node[,] grid;							/* An array to store each node that will create the grid */ 
	
	float nodeDiameter;						/* Diameter of the node */
	int gridSizeX, gridSizeY;				/* X and Y of the grid */
	
	void Awake()
	{
		nodeDiameter = nodeRadius * 2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);
		CreateGrid();
	}
	
	public int MaxSize 
	{
		get { return gridSizeX * gridSizeY; }
	}
	
	void CreateGrid() 
	{
		// Set the grid size to the desired grid
		grid = new Node[gridSizeX,gridSizeY];
		// Bottom left corner of the world
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y/2;

		// Draw the grid for each x 
		for (int x = 0; x < gridSizeX; x ++) 
		{
			// For each Y grid
			for (int y = 0; y < gridSizeY; y ++)
			{
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
				// Check if the node is colliding with obstacles
				bool walkable = !(Physics.CheckSphere(worldPoint,nodeRadius,unwalkableMask));
				grid[x,y] = new Node(walkable,worldPoint, x,y);
			}
		}
	}
	
	public List<Node> GetNeighbours(Node _node) 
	{
		List<Node> neighbours = new List<Node>();
		
		for (int x = -1; x <= 1; x++) 
		{
			for (int y = -1; y <= 1; y++)
			{
				// Skip the iteration as this will be the actual node
				if (x == 0 && y == 0)
					continue;
				
				int checkX = _node.gridX + x;
				int checkY = _node.gridY + y;
				// Check if neighbour is in the grid
				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
				{
					neighbours.Add(grid[checkX,checkY]);
				}
			}
		}
		return neighbours;
	}

	// Returns node
	public Node NodeFromWorldPoint(Vector3 _worldPosition)
	{
		// Return the position of the current node between 0 and 1
		float percentX = (_worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
		float percentY = (_worldPosition.z + gridWorldSize.y/2) / gridWorldSize.y;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);
		
		int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY-1) * percentY);
		return grid[x,y];
	}
	
	void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(transform.position,new Vector3(gridWorldSize.x,1,gridWorldSize.y));
		if (grid != null && isGridGizmoDisplayed)
		{
			foreach (Node n in grid) 
			{
				Gizmos.color = (n.walkable)?Color.white:Color.red;
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter-.1f));
			}
		}
	}
}