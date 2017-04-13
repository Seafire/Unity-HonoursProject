using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CommonBehaviour : MonoBehaviour 
{
	// Check each waypoint
	[HideInInspector]
	public bool initCheck;
	[HideInInspector]
	public bool lookAtTarget;
	[HideInInspector]
	public bool overrideAnimation;

	// Waypoints
	public int indexWaypoint;
	// Wait time for Waypoint
	public bool circularList;
	bool decendingList;
	float waitTime;

	public int getIndexList() { return indexWaypoint; }

	// Look Rotation
	Quaternion targetRot;
	
	EnemyAI enemyAI_Main;
	
	// Use this for initialization
	public void Init () 
	{
		enemyAI_Main = GetComponent<EnemyAI> ();
	}

	public void CheckPosition (Waypoints waypoint, int listCase)
	{
		
		if (!initCheck)
		{
			lookAtTarget = waypoint.lookTo;
			overrideAnimation = waypoint.overrideAnim;
			initCheck = true;
		}
		
		
		if (!waypoint.stopList)
		{
			switch (listCase)
			{
			case 0:
				WaitTimerForEachWaypoint (waypoint, enemyAI_Main.waypoints);
				break;
			case 1:
				WaitTimerForEachExtraBehaviour (waypoint, enemyAI_Main.alertBehaviours.onAlertBehaviour);
				break;
			}
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
					enemyAI_Main.charStats.CallFunctionWithString (waypoint.animRoutines[i], 0);
				}
			}
			else
			{
				Debug.Log ("No animation assigned");
			}
			overrideAnimation = false;
		}
	}

	void WaitTimerForEachWaypoint (Waypoints waypoint, List<Waypoints> listWaypoint)
	{
		if (listWaypoint.Count > 1) 
		{
			waitTime += Time.deltaTime;
			
			if (waitTime > waypoint.waitTime)
			{
				if (circularList)
				{
					if (listWaypoint.Count - 1 > indexWaypoint)
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
						if (listWaypoint.Count - 1 == indexWaypoint)
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
				enemyAI_Main.goToPos = false;
				waitTime = 0;
			}
		}
	}
	
	void WaitTimerForEachExtraBehaviour (Waypoints waypoint, List<Waypoints> listWaypoint)
	{
		if (listWaypoint.Count > 1) 
		{
			waitTime += Time.deltaTime;
			
			if (waitTime > waypoint.waitTime)
			{
				if (circularList)
				{
					if (listWaypoint.Count - 1 > enemyAI_Main.alertBehaviours.indexBehaviour)
					{
						enemyAI_Main.alertBehaviours.indexBehaviour ++;
					}
					else
					{
						enemyAI_Main.alertBehaviours.indexBehaviour = 0;
					}
				}
				else
				{
					if (!decendingList)
					{
						if (listWaypoint.Count - 1 == enemyAI_Main.alertBehaviours.indexBehaviour)
						{
							decendingList = true;
							enemyAI_Main.alertBehaviours.indexBehaviour --;
						}
						else
						{
							enemyAI_Main.alertBehaviours.indexBehaviour ++;
						}
					}
					else 
					{
						if (enemyAI_Main.alertBehaviours.indexBehaviour > 0)
						{
							enemyAI_Main.alertBehaviours.indexBehaviour --;
						}
						else
						{
							decendingList = false;
							enemyAI_Main.alertBehaviours.indexBehaviour ++;
						}
					}
				}
				initCheck = false;
				enemyAI_Main.goToPos = false;
				waitTime = 0;
			}
		}
	}

	public void Patrol()
	{
		if (enemyAI_Main.waypoints.Count > 0) 
		{
			Waypoints curWaypoint = enemyAI_Main.waypoints[indexWaypoint];
			
			if (!enemyAI_Main.goToPos)
			{
				enemyAI_Main.charStats.MoveToPosition(curWaypoint.targetDestination.position);
				enemyAI_Main.goToPos = true;
			}
			else
			{
				float distanceToTarget = Vector3.Distance(transform.position, curWaypoint.targetDestination.position);
				
				if (distanceToTarget < enemyAI_Main.plControl.stopDistance)
				{
					CheckPosition (curWaypoint, 0);
				}
			}
		}
	}

	public void DecreaseAlertLevel ()
	{
		
		if (enemyAI_Main.charStats.alertLevel > 0)
		{
			enemyAI_Main.alertTimer += Time.deltaTime * enemyAI_Main.alertMultiplier;
			
			if (enemyAI_Main.alertTimer > enemyAI_Main.alertTimerIncrement * 2.0f)
			{
				enemyAI_Main.charStats.alertLevel--;
				enemyAI_Main.alertTimer = 0.0f;
			}
		}
		
		if (enemyAI_Main.charStats.alertLevel == 0)
		{
			if (enemyAI_Main.stateAI != EnemyAI.StateAI.patrol)
			{
				enemyAI_Main.AI_State_Normal ();
			}
		}
	}
}
