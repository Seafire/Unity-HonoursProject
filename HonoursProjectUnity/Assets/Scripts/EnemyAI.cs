﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof  (CommonBehaviour))]
[RequireComponent(typeof  (AttackBehaviour))]
[RequireComponent(typeof  (SearchBehaviour))]
[RequireComponent(typeof  (AlertBehaviourMain))]
[RequireComponent(typeof  (ChaseBehaviour))]
[RequireComponent(typeof  (AllyBehaviour))]
[RequireComponent(typeof  (RetreatBehaviour))]
[RequireComponent(typeof  (PointOfInterestBehaviour))]
public class EnemyAI : MonoBehaviour
{

	public CharacterStats target;
	public float sightDistance = 20.0f;
	public Vector3 lastKnownPosition;
	
	public float alertTimer;
	public float alertTimerIncrement = 1.0f;
	public float alertMultiplier = 1.0f;
	
	public Vector3 pointOfInterest;

	
	public bool onPatrol;
	public bool canChase;
	public bool alterWaitTime;
	public List<EnemyAI> AlliesNear = new List<EnemyAI> ();
	public List<POI_Base> PointsOfInterest = new List<POI_Base> ();
	
	public List<Waypoints> waypoints = new List<Waypoints> ();

	bool updateAllies;

	// Waypoints
	public bool goToPos;

	// Look Rotation
	Quaternion targetRot;

	// Components
	public PlayerControl plControl;
	//NavMeshAgent agent;
	public CharacterStats charStats;
	EnemyManager enemyManager;

	// Behaviours
	[HideInInspector]
	public CommonBehaviour commonBehaviour;
	[HideInInspector]
	public ChaseBehaviour chaseBehaviour;
	[HideInInspector]
	public AttackBehaviour attackBehaviour;
	[HideInInspector]
	public AlertBehaviourMain alertBehaviours;
	//[HideInInspector]
	[HideInInspector]
	public SearchBehaviour searchBehaviour;
	[HideInInspector]
	public AllyBehaviour alliesBehaviour;
	[HideInInspector]
	public RetreatBehaviour retreatBehaviour;
	[HideInInspector]
	public PointOfInterestBehaviour poiBehaviour;

	// States
	public StateAI stateAI;

	public enum StateAI
	{
		patrol,
		chase,
		alert,
		onAlertBehaviours,
		hasTarget,
		search,
		deciding,
		cover,
		attack,
		retreat,
		unconsious,
		searchLocation,
		dead
	}
	
	// Use this for initialization
	void Start () 
	{
		plControl = GetComponent<PlayerControl> ();
		//agent = GetComponent<NavMeshAgent> ();
		charStats = GetComponent<CharacterStats> ();
		charStats.alert = false;
		commonBehaviour = GetComponent<CommonBehaviour> ();
		chaseBehaviour = GetComponent<ChaseBehaviour> ();
		attackBehaviour = GetComponent<AttackBehaviour> ();
		alertBehaviours = GetComponent<AlertBehaviourMain> ();
		searchBehaviour = GetComponent<SearchBehaviour> ();
		alliesBehaviour = GetComponent<AllyBehaviour> ();
		retreatBehaviour = GetComponent<RetreatBehaviour> ();
		poiBehaviour = GetComponent<PointOfInterestBehaviour> ();

		commonBehaviour.Init ();
		chaseBehaviour.Init ();
		attackBehaviour.Init ();
		alertBehaviours.Init ();
		searchBehaviour.Init ();
		alliesBehaviour.Init ();
		retreatBehaviour.Init ();
		poiBehaviour.Init ();

		if (searchBehaviour)
		{
			Debug.Log ("The component is attached");
		}


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

		sightDistance = GetComponentInChildren<EnemySightSphere> ().GetComponent<SphereCollider> ().radius;

		if (waypoints.Count > 0)
		{

		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		switch (stateAI) 
		{
		case StateAI.patrol:
			alertMultiplier = 0.5f;
			commonBehaviour.DecreaseAlertLevel ();
			commonBehaviour.Patrol();
			TargetAvailable ();
			poiBehaviour.POI_Behaviour ();
			break;
		case StateAI.alert:
			TargetAvailable ();
			alertBehaviours.Alert ();
			break;
		case StateAI.onAlertBehaviours:
			TargetAvailable ();
			alertBehaviours.AlertExtra ();
			break;
		case StateAI.chase:
			TargetAvailable ();
			chaseBehaviour.Chase ();
			break;
		case StateAI.search:
			alertMultiplier = 0.3f;
			TargetAvailable ();
			GetComponent<CommonBehaviour> ().DecreaseAlertLevel ();
			searchBehaviour.Search ();
			break;
		case StateAI.hasTarget:
			attackBehaviour.HasTarget ();
			break;
		case StateAI.cover:
			attackBehaviour.Cover ();
			break;
		case StateAI.deciding:
			attackBehaviour.DecideAttack();
			break;
		case StateAI.attack:
			if (!charStats.hasCover)
				attackBehaviour.Attack ();
			else
				attackBehaviour.AttackFromCover ();
			break;
		case StateAI.retreat:
			retreatBehaviour.RetreatAction ();
			charStats.run = true;
			charStats.aim = false;
			break;
		case StateAI.unconsious:
			charStats.run = false;
			charStats.aim = false;
			charStats.hasCover = false;
			goToPos = false;
			alertBehaviours.lookAtPoint = false;
			commonBehaviour.initCheck = false;
			break;
		case StateAI.searchLocation:
			TargetAvailable ();
			searchBehaviour.SearchTargetLocationOrBody ();
			charStats.run = false;
			charStats.aim = false;
			break;
		}

		if (stateAI != StateAI.cover && stateAI != StateAI.attack)
		{
			if (attackBehaviour.currentCover != null)
				attackBehaviour.currentCover.occupied = false;
		}
	}



	void TargetAvailable ()
	{
		if (target)
		{
			Debug.Log ("Hello");
			if (SightRaycasts())
			{
				ChangeAIBehaviour("AI_State_HasTarget", 0);
			}
		}
	}

	public bool SightRaycasts ()
	{
		bool val = false;
		RaycastHit hitTowardsLower;
		RaycastHit hitTowardsUpper;
		float raycastDistance = sightDistance;
		Vector3 targetPosition = lastKnownPosition;

		if (target)
		{
			targetPosition = target.transform.position;
		}

		/*
		 *--ADD IF STATEMENT TO CHANGE RAY START DEPENDANT ON CROUCH OR STAND-- 
		 * 
		 * 
		 */
		Vector3 raycastStart = transform.position + new Vector3 (0, 1.0f, 0);
		Vector3 dir = targetPosition - raycastStart;

		// Exclude enemy and ragdoll mask
		LayerMask excludeLayers = ~((1 << 11) | (1 << 10));

		Debug.DrawRay (raycastStart, dir);

		if (Physics.Raycast (raycastStart, dir, out hitTowardsLower, 50)) 
		{
			Debug.Log ("I've hit a target");

			if (hitTowardsLower.transform.GetComponent<CharacterStats>())
			{
				if (target)
				{
					if (hitTowardsLower.transform.GetComponent<CharacterStats> () == target)
					{
						Debug.Log ("Now attack");
						val = true;
					}
				}
			}
		}
		/*
		if (val == false)
		{
			//dir += new Vector3 (0, 1.6f, 0);

			if (Physics.Raycast (raycastStart, dir + new Vector3 (0, 1.6f, 0), out hitTowardsUpper, raycastDistance, excludeLayers)) 
			{
				if (hitTowardsUpper.transform.GetComponent<CharacterStats>())
				{
					if (target)
					{
						if (hitTowardsUpper.transform.GetComponent<CharacterStats> () == target)
						{
							Debug.Log ("Now attack");
							val = true;
						}
					}
				}
			}
		}
*/
		if (val)
			lastKnownPosition = target.transform.position;

		return val;
	}
	
	public void LookAtTarget (Vector3 posToLook)
	{
		Vector3 dirToLook = posToLook - transform.position;
		dirToLook.y = 0;

		float angle = Vector3.Angle (transform.forward, dirToLook);

		if (angle > 0.1f)
		{
			targetRot = Quaternion.LookRotation (dirToLook);
			transform.localRotation = Quaternion.Slerp (transform.localRotation, targetRot, Time.deltaTime * 3);
		}
	}

	/*
	 * 
	 *  --COMMON FUNCTIONS-- 
	 * --CHECKING WAYPOINTS--
	 * 
	 */



	/*
	 * 
	 * --ALERT FUNCTIONS-- 
	 * 
	 */

	public void GoOnAlert ( Vector3 _point)
	{
		pointOfInterest = _point;
		stateAI = StateAI.alert;
		alertBehaviours.lookAtPoint = false;
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
	 * --SWITCH STATE FUNCTIONS-- 
	 * 
	 */

	public void ChangeAIBehaviour (string behave, float delay)
	{
		Invoke (behave, delay);
	}

	public void AI_State_Normal ()
	{
		stateAI = StateAI.patrol;
		target = null;
		charStats.alert = false;
		goToPos = false;
		alertBehaviours.lookAtPoint = false;
		commonBehaviour.initCheck = false;
		charStats.hasCover = false;
	}

	public void AI_State_Chase ()
	{
		stateAI = StateAI.chase;
		goToPos = false;
		alertBehaviours.lookAtPoint = false;
		commonBehaviour.initCheck = false;
		charStats.hasCover = false;
	}

	public void AI_State_Search ()
	{
		stateAI = StateAI.search;
		target = null;
		goToPos = false;
		alertBehaviours.lookAtPoint = false;
		commonBehaviour.initCheck = false;
		charStats.hasCover = false;
	}

	public void AI_State_OnAlert_RunList ()
	{
		stateAI = StateAI.onAlertBehaviours;
		charStats.run = true;
		goToPos = false;
		alertBehaviours.lookAtPoint = false;
		charStats.hasCover = false;
	}

	public void AI_State_Attack ()
	{
		alliesBehaviour.AlertAllies ();
		stateAI = StateAI.attack;
	}

	public void AI_State_HasTarget ()
	{
		stateAI = StateAI.hasTarget;
		charStats.alert = true;
		goToPos = false;
		alertBehaviours.lookAtPoint = false;
		commonBehaviour.initCheck = false;
		charStats.hasCover = false;
	}

	public void AI_State_Cover ()
	{
		stateAI = StateAI.cover;
		charStats.run = true;
		goToPos = false;
		alertBehaviours.lookAtPoint = false;
		commonBehaviour.initCheck = false;
	}

	public void AI_State_DecideByStats ()
	{
		stateAI = StateAI.deciding;
		charStats.run = false;
		goToPos = false;
		alertBehaviours.lookAtPoint = false;
		commonBehaviour.initCheck = false;
		charStats.hasCover = false;
	}

	public void AI_State_Retreat ()
	{
		stateAI = StateAI.retreat;
		charStats.crouch = false;
		charStats.aim = false;
		charStats.hasCover = false;
		goToPos = false;
		alertBehaviours.lookAtPoint = false;
		commonBehaviour.initCheck = false;
		attackBehaviour.findCoverPosition = false;
	}

	public void AI_State_SearchLocation (Vector3 targetLocation, CharacterStats body)
	{
		stateAI = StateAI.searchLocation;

		if (body)
		{
			searchBehaviour.curBody = body;
		}
		else
		{
			searchBehaviour.targetLocation = targetLocation;
		}

		searchBehaviour.searchPhase = 0;
		charStats.hasCover = false;
		goToPos = false;
	}

	//public float getWaitTime() {return 1;}
	//public float setWaitTime(float newTime){newTime = newTime;}

	/*
	 * 
	 * --EXTRA WAYPOINT CHECK-- 
	 * --ALLOWS DIFFERENT WAYPOINT NAVIGATION BETWEEN STATES--
	 */
	

}


/*
 * 
 *--STRUCT TO CONTAIN THE INFORMATION OF EACH WAYPOINT THAT IS IN THE WORLD-- 
 * 
 */

[System.Serializable]
public class Waypoints
{
	public Transform targetDestination;
	public float waitTime;
	public bool lookTo;
	public Transform targetLookTo;
	public float speedToLook;
	public bool overrideAnim;
	public string[] animRoutines;
	public bool stopList;
	public bool extraWaypoint;
}
