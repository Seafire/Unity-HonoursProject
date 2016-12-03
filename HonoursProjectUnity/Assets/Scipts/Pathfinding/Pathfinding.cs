using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

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
		if (Input.GetButtonDown ("Jump")) 
		{
			FindPath (seeker.position, target.position);
		}
	}

	void FindPath(Vector3 startPos, Vector3 targetPos)
	{
		Stopwatch sw = new Stopwatch ();
		sw.Start ();
		Node startNode = grid.NodeFromWorldPoint (startPos);
		Node targetNode = grid.NodeFromWorldPoint (targetPos);

		Heap<Node> openSet = new Heap<Node> (grid.MaxSize);
		HashSet<Node> closedSet = new HashSet<Node> ();

		openSet.Add (startNode);

		while (openSet.Count > 0) 
		{
			Node currentNode = openSet.RemoveFirst();

			closedSet.Add (currentNode);

			if (currentNode == targetNode)
			{
				sw.Stop ();
				print ("Path found: " + sw.ElapsedMilliseconds);
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
					else
					{
						openSet.UpdateItem(neighbours);
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
