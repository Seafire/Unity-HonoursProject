/*using UnityEngine;
using System.Collections;
using Pathfinding;

public class AStarPath : MonoBehaviour
{

	public Vector3 targetPosition;
	private Seeker seeker;
	private CharacterController controller;
	public Path path;
	public float speed;

	// This is the max distance from the AI to the next waypoint
	public float nextWaypointDistance = 10.0f;
	//Current Waypoint
	private int currentWaypoint = 0;

	// Use this for initialization
	public void Start () 
	{
		targetPosition = GameObject.Find ("Target").transform.position;
		seeker = GetComponent<Seeker> ();
		controller = GetComponent<CharacterController> ();

		// Set path
		seeker.StartPath (transform.position, targetPosition, OnPathComplete);
	}

	public virtual void OnPathComplete(Path _p)
	{
		ABPath p = _p as ABPath;
		Debug.Log ("I'm I getting here");
		if (!p.error) 
		{
			path = p;
			Debug.Log ("I should be setting a path");
			// Reset the waypoint counter
			currentWaypoint = 0;
		}
	}

	public void FixedUpdate()
	{
		if (path == null)
			return;

		if (currentWaypoint >= path.vectorPath.Count)
			return;

		// Calculate the direction of the unit
		Vector3 dir = (path.vectorPath [currentWaypoint] - transform.position).normalized;
		dir *= speed * Time.fixedDeltaTime;
		controller.SimpleMove (dir);		// Move the unit
		Debug.Log (dir);

		// Check if close enough to current waypoint. If true move onto  the next waypoint
		if (Vector3.Distance (transform.position, path.vectorPath [currentWaypoint]) < nextWaypointDistance) 
		{
			currentWaypoint++;
			return;
		}

	}

	// Update is called once per frame
	void Update () 
	{
	
	}
}*/
