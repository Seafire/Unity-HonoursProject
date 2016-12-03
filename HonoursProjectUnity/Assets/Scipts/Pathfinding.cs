using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour 
{
	public Transform seeker, target;
	Grid grid;

	void Awake()
	{
		grid = GetComponent<Grid> ();
	}

	void Update()
	{
		FindPath (seeker.position, target.position);
	}

	void FindPath(Vector3 startPos, Vector3 targetPos)
	{
		Node startNode = grid.NodeFromWorldPoint (startPos);
		Node targetNode = grid.NodeFromWorldPoint (targetPos);

		List<Node> openSet = new List<Node> ();
		HashSet<Node> closedSet = new HashSet<Node> ();

		openSet.Add (startNode);

		while (openSet.Count > 0) {
			Node currentNode = openSet[0];
			for (int i = 1; i < openSet.Count; i++)
			{
				if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
				{
					currentNode = openSet[i];
				}
			}

			openSet.Remove(currentNode);
			closedSet.Add (currentNode);

			if (currentNode == targetNode)
			{
				RetracePath(startNode, targetNode);
				return;
			}

			foreach (Node neighbours in grid.GetNeighbours(currentNode))
			{
				if (!neighbours.walkable || closedSet.Contains(neighbours))
				{
					continue;
				}

				int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbours);
				if (newMovementCostToNeighbour < neighbours.gCost || !openSet.Contains(neighbours))
				{
					neighbours.gCost = newMovementCostToNeighbour;
					neighbours.hCost = GetDistance(neighbours, targetNode);
					neighbours.parent = currentNode;

					if (!openSet.Contains(neighbours))
					{
						openSet.Add(neighbours);
					}
				}
			}
		}
	}

	void RetracePath (Node startNode, Node endNode)
	{
		List<Node> path = new List<Node> ();
		Node currentNode = endNode;

		while (currentNode != startNode) 
		{
			path.Add (currentNode);
			currentNode = currentNode.parent;
		}
		path.Reverse ();

		grid.path = path;
	}

	int GetDistance(Node nodeA, Node nodeB)
	{
		int disX = Mathf.Abs (nodeA.gridX - nodeB.gridX);
		int disY = Mathf.Abs (nodeA.gridY - nodeB.gridY);

		if (disX > disY)
			return 14 * disY + 10 * (disX - disY);

		
		return 14 * disX + 10 * (disY - disX);
	}
}
