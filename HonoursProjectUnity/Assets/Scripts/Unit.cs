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

	public Transform target;
	float speed = 10;
	Vector3[] path;
	int targetIndex;

	void Start()
	{
		PathRequestManager.RequestPath (transform.position, target.position, OnPathFound);
	}

	public void OnPathFound(Vector3[] newPath, bool success)
	{
		if (success) 
		{
			path = newPath;
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
}
