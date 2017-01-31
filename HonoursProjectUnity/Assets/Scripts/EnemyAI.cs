using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
	// Check each waypoint
	bool initCheck;
	bool lookAtTarget;
	bool overrideAnimation;

	// Wait time for Waypoint
	public bool circularList;
	bool decendingList;
	float waitTime;

	// Look Rotation
	Quaternion targetRot;

	// Waypoints
	bool goToPos;
	public int indexWaypoint;
	public List<Waypoints> waypoints = new List<Waypoints> ();

	// States
	public StateAI stateAI;

	public enum StateAI
	{
		patrol,
		chase,
		alert,
		attack
	}

	// Components
	PlayerControl plControl;
	NavMeshAgent agent;
	CharacterStats charStats;
	

	// Use this for initialization
	void Start () 
	{
		plControl = GetComponent<PlayerControl> ();
		agent = GetComponent<NavMeshAgent> ();
		charStats = GetComponent<CharacterStats> ();
		charStats.alert = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		switch (stateAI) 
		{
		case StateAI.patrol:
			Patrol();
			break;
		case StateAI.alert:
			break;
		case StateAI.chase:
			break;
		case StateAI.attack:
			break;
		}
	}

	void Patrol()
	{
		if (waypoints.Count > 0) 
		{
			Waypoints curWaypoint = waypoints[indexWaypoint];

			if (!goToPos)
			{
				charStats.MoveToPosition(curWaypoint.targetDestination.position);
				goToPos = true;
			}
			else
			{
				float distanceToTarget = Vector3.Distance(transform.position, curWaypoint.targetDestination.position);

				if (distanceToTarget < plControl.stopDistance)
				{
					CheckPosition (curWaypoint);
				}
			}
		}
	}

	public void CheckPosition (Waypoints waypoint)
	{

		if (!initCheck)
		{
			lookAtTarget = waypoint.lookTo;
			overrideAnimation = waypoint.overrideAnim;
			initCheck = true;
		}
		waitTime += Time.deltaTime;

		if (waitTime > waypoint.waitTime)
		{
			if (circularList)
			{
				if (waypoints.Count - 1 > indexWaypoint)
				{
					indexWaypoint ++;
				}
				else
				{
					indexWaypoint = 0;
				}
			}
			else
			{
				if (!decendingList)
				{
					if (waypoints.Count - 1 == indexWaypoint)
					{
						decendingList = true;
						indexWaypoint --;
					}
					else
					{
						indexWaypoint ++;
					}
				}
				else 
				{
					if (indexWaypoint > 0)
					{
						indexWaypoint --;
					}
					else
					{
						decendingList = false;
						indexWaypoint ++;
					}
				}
			}

			initCheck = false;
			goToPos = false;
			waitTime = 0;
		}

		if (lookAtTarget)
		{
			Vector3 direction = waypoint.targetLookTo.position - transform.position;
			direction.y = 0;

			float angle = Vector3.Angle(transform.forward, direction);

			if (angle > 0.1f)
			{
				targetRot = Quaternion.LookRotation(direction);
				transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * waypoint.speedToLook);
			}
			else
			{
				lookAtTarget = false;
			}
		}

		// Animation Stuff for future development

		if (overrideAnimation)
		{
			if (waypoint.animRoutines.Length > 0)
			{
				for (int i = 0; i < waypoint.animRoutines.Length; i++)
				{
					charStats.CallFunctionWithString (waypoint.animRoutines[i], 0);
				}
			}
			else
			{
				Debug.Log ("No animation assigned");
			}
			overrideAnimation = false;
		}
	}
}


[System.Serializable]
public struct Waypoints
{
	public Transform targetDestination;
	public float waitTime;
	public bool lookTo;
	public Transform targetLookTo;
	public float speedToLook;
	public bool overrideAnim;
	public string[] animRoutines;
}
