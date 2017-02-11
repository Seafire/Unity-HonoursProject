using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof  (CommonBehaviour))]
[RequireComponent(typeof  (AttackBehaviour))]
[RequireComponent(typeof  (SearchBehaviour))]
[RequireComponent(typeof  (AlertBehaviourMain))]
[RequireComponent(typeof  (ChaseBehaviour))]
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
	public List<CharacterStats> AlliesNear = new List<CharacterStats> ();

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
		attack
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
			commonBehaviour.DecreaseAlertLevel ();
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
		}
	}



	void TargetAvailable ()
	{
		if (target)
		{
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

		Debug.DrawRay (raycastStart, dir + new Vector3 (0, 1, 0));

		if (Physics.Raycast (raycastStart, dir + new Vector3 (0, 1, 0), out hitTowardsLower, 50, excludeLayers)) 
		{
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
