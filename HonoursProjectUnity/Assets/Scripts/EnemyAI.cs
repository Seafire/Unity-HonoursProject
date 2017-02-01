using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{

	// General Behaviour Variables
	public float delayNewBehaviour = 3.0f;
	private float timeNewBehaviour;

	// Alert Variables
	public bool onPatrol;
	public bool canChase;
	public int indexBehaviour;
	public List<Waypoints> onAlertBehaviour = new List<Waypoints> ();

	private bool lookAtPoint;
	private bool initAlert;
	Vector3 pointOfInterest;

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
	public string[] alertLogic;

	// States
	public StateAI stateAI;

	public enum StateAI
	{
		patrol,
		chase,
		alert,
		onAlertBehaviours,
		attack
	}

	// Components
	PlayerControl plControl;
	NavMeshAgent agent;
	CharacterStats charStats;
	EnemyManager enemyManager;
	

	// Use this for initialization
	void Start () 
	{
		plControl = GetComponent<PlayerControl> ();
		agent = GetComponent<NavMeshAgent> ();
		charStats = GetComponent<CharacterStats> ();
		charStats.alert = false;
		enemyManager = GameObject.FindGameObjectWithTag ("GameController").GetComponent<EnemyManager> ();
		enemyManager.AllEnemies.Add (charStats);

		if (onPatrol) 
		{
			canChase = true;
			enemyManager.EnemyPatrol.Add (charStats);
		}

		if (canChase)
		{
			enemyManager.EnemyChase.Add (charStats);
		}
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
			Alert ();
			break;
		case StateAI.onAlertBehaviours:
			AlertExtra ();
			break;
		case StateAI.chase:
			Chase ();
			break;
		case StateAI.attack:
			Attack ();
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
					CheckPosition (curWaypoint, 0);
				}
			}
		}
	}

	// All actions of the enemy is alerted by the player
	void Alert ()
	{
		// Make sure that the enemy is not currently moving - focusing on the the point of interest
		plControl.moveToPosition = false;
		if (!lookAtPoint)
		{
			Vector3 dirToLookAt = pointOfInterest - transform.position;
			dirToLookAt.y = 0.0f;

			float angle = Vector3.Angle (transform.forward, dirToLookAt);

			if (angle > 0.1f)
			{
				targetRot = Quaternion.LookRotation (dirToLookAt);
				transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * 3);
			}
			else
			{
				Debug.Log ("New state please");
				lookAtPoint = true;
			}
		}
		delayNewBehaviour -= Time.deltaTime;
		
		if (delayNewBehaviour <= 0)
		{
			if (alertLogic.Length > 0)
			{
				ChangeAIBehaviour (alertLogic[0], 0);
			}
			
			delayNewBehaviour = 3;
		}
	}

	void AlertExtra ()
	{
		if (onAlertBehaviour.Count > 0)
		{
			Waypoints curBehaviour = onAlertBehaviour[indexBehaviour];

			if (!goToPos)
			{
				charStats.MoveToPosition (curBehaviour.targetDestination.position);
				goToPos = true;
			}
			else
			{
				float disToTarget = Vector3.Distance (transform.position, curBehaviour.targetDestination.position);

				if (disToTarget < plControl.stopDistance)
				{
					CheckPosition (curBehaviour, 1);
				}
			}
		}
	}

	// All actions if the enemy loses site of the player
	void Chase ()
	{
		charStats.MoveToPosition (pointOfInterest);
		charStats.run = true;
		goToPos = true;
	}
	
	void Attack ()
	{
		// All actions if the enemy is attacking the player
	}

	/*
	 * 
	 *  --COMMON FUNCTIONS-- 
	 * --CHECKING WAYPOINTS--
	 * 
	 */

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
				WaitTimerForEachWaypoint (waypoint, waypoints);
				break;
			case 1:
				WaitTimerForEachExtraBehaviour (waypoint, onAlertBehaviour);
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

	/*
	 * 
	 * --ALERT FUNCTIONS-- 
	 * 
	 */

	public void GoOnAlert ( Vector3 _point)
	{
		this.pointOfInterest = _point;
		stateAI = StateAI.alert;
		lookAtPoint = false;
	}

	/*
	 * 
	 * --EXTRA ALERT FUNCTIONS-- 
	 * 
	 */

	/*
	 * 
	 * --CHASE FUNCTIONS-- 
	 * 
	 */

	/*
	 * 
	 * --ATTACK FUNCTIONS-- 
	 * 
	 */
	
	/*
	 * 
	 * --SWITCH STATE FUNCTIONS-- 
	 * 
	 */

	public void ChangeAIBehaviour (string behave, float delay)
	{
		Invoke (behave, delay);
	}

	void AI_State_Normal ()
	{
		stateAI = StateAI.patrol;
		goToPos = false;
		lookAtPoint = false;
		initCheck = false;
	}

	void AI_State_Chase ()
	{
		stateAI = StateAI.chase;
		goToPos = false;
		lookAtPoint = false;
		initCheck = false;
	}

	void AI_State_OnAlert_RunList ()
	{
		stateAI = StateAI.onAlertBehaviours;
		charStats.run = true;
		goToPos = false;
		lookAtPoint = false;
	}

	void AI_State_Attack ()
	{
		stateAI = StateAI.attack;
	}

	/*
	 * 
	 * --EXTRA WAYPOINT CHECK-- 
	 * --ALLOWS DIFFERENT WAYPOINT NAVIGATION BETWEEN STATES--
	 */
	
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
				goToPos = false;
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
					if (listWaypoint.Count - 1 > indexBehaviour)
					{
						indexBehaviour ++;
					}
					else
					{
						indexBehaviour = 0;
					}
				}
				else
				{
					if (!decendingList)
					{
						if (listWaypoint.Count - 1 == indexBehaviour)
						{
							decendingList = true;
							indexBehaviour --;
						}
						else
						{
							indexBehaviour ++;
						}
					}
					else 
					{
						if (indexBehaviour > 0)
						{
							indexBehaviour --;
						}
						else
						{
							decendingList = false;
							indexBehaviour ++;
						}
					}
				}
				initCheck = false;
				goToPos = false;
				waitTime = 0;
			}
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
	public bool stopList;
}
