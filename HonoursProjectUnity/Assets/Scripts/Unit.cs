using UnityEngine;
using System.Collections;

/*
 * This script should be attached to all units in the game
*/

public class Unit : MonoBehaviour
{
	// For mouse
	public Vector2 screenPos;
	public bool isOnScreen;
	public bool isSelected = false;
	
	public bool isWalkable = true;

	public Transform target;
	float speed = 10;
	Vector3[] path;
	int targetIndex;

	public void OnPathFound(Vector3[] newPath, bool success)
	{
		if (success) 
		{
			path = newPath;
			targetIndex = 0;
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
	}

	IEnumerator FollowPath()
	{
		Vector3 currentWaypoint = path [0];

		while (true) 
		{
			if (transform.position == currentWaypoint)
			{
				targetIndex++;
				if (targetIndex >= path.Length)
				{
					yield break;
				}
				currentWaypoint = path[targetIndex];
			}
			transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
			yield return null;
		}
	}

	public void OnDrawGizmos()
	{
		// If there is currently a path
		if (path != null)
		{
			for (int i = targetIndex; i < path.Length; i++ )
			{
				Gizmos.color = Color.black;
				Gizmos.DrawCube(path[i], Vector3.one);
				if(i == targetIndex)
				{
					Gizmos.DrawLine(transform.position, path[i]);
				}
				else
				{
					Gizmos.DrawLine(path[i - 1], path[i]);
				}
			}
		}
	}

	void Update()
	{
		// If the unit is not selected
		if (!isSelected) 
		{
			// Track the screen position
			screenPos = Camera.main.WorldToScreenPoint(this.transform.position);
			
			// If within the screen space
			if (MousePoint.UnitWithinScreenSpace(screenPos))
			{
				// And not already added to UnitsOnScreen, add it
				if (!isOnScreen)
				{
					MousePoint.unitsOnScreen.Add (this.gameObject);
					isOnScreen = true;
				}
			}
			// Unit is not on Screen space
			else
			{
				// Remove if previously on screen
				if (isOnScreen)
				{
					MousePoint.RemoveFromOnScreenUnits(this.gameObject);
				}
			}
		}
	}

	void LateUpdate()
	{
		if (isSelected) 
		{
			// If the mouse right click is pressed and the left control is currently not pressed 
			if (Input.GetMouseButtonDown(1) && !Input.GetKey(KeyCode.LeftControl))
			{
				// Start pathfinding from unit current position to right mouse click position
				PathRequestManager.RequestPath (transform.position, MousePoint.RightMouseClick, OnPathFound);
			}
		}
	}
}
