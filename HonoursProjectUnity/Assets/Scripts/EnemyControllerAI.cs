using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyControllerAI : MonoBehaviour
{
	// Access to header of patrols containing waypoints and lookAtPoints
	public List<Routes> Patrol = new List<Routes> ();
	// Use this for initialization
	void Start () 
	{
		if (Patrol != null)
			Debug.Log ("I'm not empty");

		for (int i = 0; i < Patrol.Count; i++) 
		{
			Patrol[i].waypoints = Patrol[i].route.FindChild ("Waypoints");
			Patrol[i].lookAtPoints = Patrol[i].route.FindChild ("LookAtPoints");

			if (Patrol[i].waypoints == null)
				continue;
			Patrol[i].waypoint = Patrol[i].waypoints.GetComponentsInChildren<Transform> ();
		}

	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}

[System.Serializable]
public class Routes 
{
	public Transform route;
	//[HideInInspector]
	public Transform waypoints;
	//[HideInInspector]
	public Transform[] waypoint;
	[HideInInspector]
	public Transform lookAtPoints;
	[HideInInspector]
	public Transform[] lookAtPoint;
}