using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Pathfinding : MonoBehaviour 
{
	
	PathRequestManager requestManager;		/* Access to the request manager */
	Grid grid;								/* Access to the Grid */
	
	void Awake() 
	{
		// Initialize the request manager
		requestManager = GetComponent<PathRequestManager>();
		// Initialize the grid
		grid = GetComponent<Grid>();
	}

	public void StartFindPath(Vector3 _startPos, Vector3 _targetPos)
	{
		StartCoroutine(FindPath(_startPos,_targetPos));
	}
	
	IEnumerator FindPath(Vector3 _startPos, Vector3 _targetPos)
	{
		
		Vector3[] waypoints = new Vector3[0];
		bool pathSuccess = false;

		// Convert the positions in terms of Nodes
		Node startNode = grid.NodeFromWorldPoint(_startPos);
		Node targetNode = grid.NodeFromWorldPoint(_targetPos);
		
		// Check if the nodes are passable
		if (startNode.walkable && targetNode.walkable)
		{
			Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
			HashSet<Node> closedSet = new HashSet<Node>();
			openSet.Add(startNode);

			// While the open set is not empty
			while (openSet.Count > 0)
			{
				// The current Node is equal to the first element
				Node currentNode = openSet.RemoveFirst();
				closedSet.Add(currentNode);
				// If the target node is reached
				if (currentNode == targetNode)
				{
					// Stop searching for a path
					pathSuccess = true;
					break;
				}
				
				foreach (Node neighbour in grid.GetNeighbours(currentNode))
				{
					// If the node cannot be traversed
					if (!neighbour.walkable || closedSet.Contains(neighbour))
					{
						continue;
					}
					
					int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
					if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) 
					{
						neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = GetDistance(neighbour, targetNode);
						neighbour.parent = currentNode;
						
						if (!openSet.Contains(neighbour))
							openSet.Add(neighbour);
					}
				}
			}
		}
		yield return null;
		if (pathSuccess) 
		{
			waypoints = RetracePath(startNode,targetNode);
		}
		requestManager.FinishedProcessingPath(waypoints,pathSuccess);
		
	}
	
	Vector3[] RetracePath(Node _startNode, Node _endNode) 
	{
		List<Node> path = new List<Node>();
		Node currentNode = _endNode;
		
		while (currentNode != _startNode)
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		Vector3[] waypoints = SimplifyPath(path);
		Array.Reverse(waypoints);
		return waypoints;
		
	}
	
	Vector3[] SimplifyPath(List<Node> _path)
	{
		List<Vector3> waypoints = new List<Vector3>();
		Vector2 directionOld = Vector2.zero;
		
		for (int i = 1; i < _path.Count; i ++)
		{
			Vector2 directionNew = new Vector2(_path[i-1].gridX - _path[i].gridX, _path[i-1].gridY - _path[i].gridY);
			if (directionNew != directionOld) 
			{
				waypoints.Add(_path[i].worldPosition);
			}
			directionOld = directionNew;
		}
		return waypoints.ToArray();
	}
	
	int GetDistance(Node _nodeA, Node _nodeB)
	{
		int dstX = Mathf.Abs(_nodeA.gridX - _nodeB.gridX);
		int dstY = Mathf.Abs(_nodeA.gridY - _nodeB.gridY);
		
		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
	}
	
	
}