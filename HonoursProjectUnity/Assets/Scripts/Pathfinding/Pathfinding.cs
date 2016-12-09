using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Pathfinding : MonoBehaviour 
{
	
	PathRequestManager requestManager;
	Grid grid;
	
	void Awake() 
	{
		requestManager = GetComponent<PathRequestManager>();
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
		
		Node startNode = grid.NodeFromWorldPoint(_startPos);
		Node targetNode = grid.NodeFromWorldPoint(_targetPos);
		
		
		if (startNode.walkable && targetNode.walkable)
		{
			Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
			HashSet<Node> closedSet = new HashSet<Node>();
			openSet.Add(startNode);
			
			while (openSet.Count > 0)
			{
				Node currentNode = openSet.RemoveFirst();
				closedSet.Add(currentNode);
				
				if (currentNode == targetNode)
				{
					pathSuccess = true;
					break;
				}
				
				foreach (Node neighbour in grid.GetNeighbours(currentNode))
				{
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