using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{

	public CharacterStats target;
	public float sightDistance = 20.0f;
	Vector3 lastKnownPosition;

	private float alertTimer;
	private float alertTimerIncrement = 1.0f;

	// General Behaviour Variables
	public float delayNewBehaviour = 3.0f;
	private float timeNewBehaviour;

	// Attacking Variables
	public bool startShooting;
	public float attackRate = 1.5f;
	public float shootRate = 0.3f;
	private float shootR;
	private float attackR;
	public ParticleSystem muzzleFire;

	// Searching Variables
	public bool decideBehaviour;
	public float decideBehaviourThreshold = 5.0f;
	public float alertMultiplier = 1.0f;
	public List<Transform> possibleHidingAreas = new List<Transform> ();
	public List<Vector3> positionsArroundUnit = new List<Vector3> ();
	private bool getPossibleHidingArea;
	private bool populateListPositions;
	private bool searchAtPoint;
	private bool createSearchPos;
	private bool searchHidingSpot;
	private int indexSearchPos;
	Transform targetHidingSpot;

	// Shooting Variables
	private int timesToShoot;
	private int timesShot;

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

	// Components
	PlayerControl plControl;
	//NavMeshAgent agent;
	CharacterStats charStats;
	EnemyManager enemyManager;

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
		attack
	}
	
	// Use this for initialization
	void Start () 
	{
		plControl = GetComponent<PlayerControl> ();
		//agent = GetComponent<NavMeshAgent> ();
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

		sightDistance = GetComponentInChildren<EnemySightSphere> ().GetComponent<SphereCollider> ().radius;
	}
	
	// Update is called once per frame
	void Update ()
	{
		switch (stateAI) 
		{
		case StateAI.patrol:
			alertMultiplier = 1;
			DecreaseAlertLevel ();
			Patrol();
			TargetAvailable ();
			break;
		case StateAI.alert:
			TargetAvailable ();
			Alert ();
			break;
		case StateAI.onAlertBehaviours:
			TargetAvailable ();
			AlertExtra ();
			break;
		case StateAI.chase:
			TargetAvailable ();
			Chase ();
			break;
		case StateAI.search:
			alertMultiplier = 0.3f;
			TargetAvailable ();
			DecreaseAlertLevel ();
			SearchBehaviour ();
			break;
		case StateAI.hasTarget:
			HasTarget ();
			break;
		case StateAI.attack:
			Attack ();
			break;
		}
	}

	void DecreaseAlertLevel ()
	{

		if (charStats.alertLevel > 0)
		{
			alertTimer += Time.deltaTime * alertMultiplier;

			if (alertTimer > alertTimerIncrement * 2.0f)
			{
				charStats.alertLevel--;
				alertTimer = 0.0f;
			}
		}

		if (charStats.alertLevel == 0)
		{
			if (stateAI != StateAI.patrol)
			{
				ChangeAIBehaviour ("AI_State_Normal", 0);
			}
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

	bool SightRaycasts ()
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
		Vector3 raycastStart = transform.position + new Vector3 (0, 1.6f, 0);
		Vector3 dir = targetPosition - raycastStart;

		// Exclude enemy and ragdoll mask
		LayerMask excludeLayers = ~((1 << 11) | (1 << 10));

		Debug.DrawRay (raycastStart, dir + new Vector3 (0, 1, 0));

		if (Physics.Raycast (raycastStart, dir + new Vector3 (0, 1, 0), out hitTowardsLower, raycastDistance, excludeLayers)) 
		{
			if (hitTowardsLower.transform.GetComponent<CharacterStats>())
			{
				if (target)
				{
					if (hitTowardsLower.transform.GetComponent<CharacterStats> () == target)
					{
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
				if (target)
				{
					if (hitTowardsUpper.transform == target.transform)
					{
						val = true;
					}
				}
			}
		}

		if (val)
			lastKnownPosition = target.transform.position;

		return val;
	}

	void HasTarget ()
	{
		charStats.StopMoving ();

		if (SightRaycasts ())
		{
			Debug.Log ("I should be updating alert");
			if (charStats.alertLevel < 10.0f)
			{
				float distanceToTarget = Vector3.Distance (transform.position, target.transform.position);
				float multiplier = 1 + (distanceToTarget * 0.1f);

				/*
				 * FUTURE DEVELOPMENT - INCREAMENT HOW FAST PLAYER IS DETECTED BASED ON DISTANCE
				 */

				alertTimer += Time.deltaTime * multiplier;

				if (alertTimer > alertTimerIncrement)
				{
					charStats.alertLevel++;
					alertTimer = 0;
				}
			}
			else
			{
				ChangeAIBehaviour ("AI_State_Attack", 0);
			}

			LookAtTarget (lastKnownPosition);
		}
		else
		{
			if (charStats.alertLevel > 5)
			{
				ChangeAIBehaviour ("AI_State_Chase", 0);
				pointOfInterest = lastKnownPosition;
			}
			else
			{
				delayNewBehaviour -= Time.deltaTime;
				
				if (delayNewBehaviour <= 0)
				{
					ChangeAIBehaviour ("AI_State_Normal", 0);
					delayNewBehaviour = 3;
				}
			}
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
		if (target == null) 
		{
			if (!goToPos)
			{
				charStats.MoveToPosition (pointOfInterest);
				charStats.run = true;
				goToPos = true;
			}
		}
		else
		{
			charStats.MoveToPosition (target.transform.position);
			charStats.run = true;
		}

		if (!SightRaycasts ())
		{
			if (target)
			{
				lastKnownPosition = target.transform.position;
				target = null;
			}
			else
			{
				float disFromTargetPos = Vector3.Distance (transform.position, lastKnownPosition);

				if (disFromTargetPos < 2)
				{
					delayNewBehaviour -= Time.deltaTime;
					
					if (delayNewBehaviour <= 0)
					{
						ChangeAIBehaviour ("AI_State_Search", 0);
						delayNewBehaviour = 3;
					}
				}
			}
		}

	}

	void SearchBehaviour ()
	{
		if (!decideBehaviour)
		{
			int ranVal = Random.Range (0, 11);

			if (ranVal < decideBehaviourThreshold)
			{
				searchAtPoint = true;
				Debug.Log ("Searching in position around unit");
			}
			else
			{
				searchHidingSpot = true;
				Debug.Log ("Searching in Hiding Spots");
			}

			decideBehaviour = true;
		}
		else
		{
			if (searchHidingSpot)
			{
				if (!populateListPositions)
				{
					possibleHidingAreas.Clear();

					Collider[] allColliders = Physics.OverlapSphere (transform.position, 20);

					for (int i = 0; i < allColliders.Length; i++)
					{
						if (allColliders[i].GetComponent<HidingSpot>())
						{
							possibleHidingAreas.Add (allColliders[i].transform);
						}
					}
					populateListPositions = true;
				}
				else if (possibleHidingAreas.Count > 0)
				{
					if (!targetHidingSpot)
					{
						int ranVal = Random.Range (0, possibleHidingAreas.Count);

						targetHidingSpot = possibleHidingAreas [ranVal];
					}
					else
					{
						charStats.MoveToPosition (targetHidingSpot.position);

						Debug.Log ("Going to hiding spot");
						float disToTarget = Vector3.Distance (transform.position, targetHidingSpot.position);

						if (disToTarget < 2)
						{
							delayNewBehaviour -= Time.deltaTime;
							
							if (delayNewBehaviour <= 0)
							{
								// reset
								populateListPositions = false;
								targetHidingSpot = null;
								decideBehaviour = false;
								delayNewBehaviour = 3;
							}
						}
					}
				}
				else
				{
					Debug.Log ("No hiding spots found");
					searchAtPoint = false;
					populateListPositions = false;
					targetHidingSpot = null;
				}
			}


			if (searchAtPoint)
			{
				if (!createSearchPos)
				{
					positionsArroundUnit.Clear();

					int ranVal = Random.Range (4, 10);

					for (int i = 0; i < ranVal; i++)
					{
						float offsetX = Random.Range (-10, 10);
						float offsetZ = Random.Range (-10, 10);

						Vector3 originPos = transform.position;
						originPos += new Vector3 (offsetX, 0, offsetZ);

						NavMeshHit hit;

						if (NavMesh.SamplePosition (originPos, out hit, 5, NavMesh.AllAreas))
						{
							positionsArroundUnit.Add (hit.position);
						}
					}

					if (positionsArroundUnit.Count > 0)
					{
						indexSearchPos = 0;
						createSearchPos = true;
					}
				}
				else
				{
					Vector3 targetPos = positionsArroundUnit[indexSearchPos];

					Debug.Log ("Going to position");

					charStats.MoveToPosition (targetPos);

					float disToPos = Vector3.Distance (transform.position, targetPos);

					if (disToPos < 2)
					{
						int ranVal = Random.Range (0, 11);
						decideBehaviour = (ranVal <5);

						if (indexSearchPos < positionsArroundUnit.Count - 1)
						{
							delayNewBehaviour -= Time.deltaTime;
							
							if (delayNewBehaviour <= 0)
							{
								indexSearchPos++;
								delayNewBehaviour = 3;
							}
						}
						else
						{
							delayNewBehaviour -= Time.deltaTime;
							
							if (delayNewBehaviour <= 0)
							{
								indexSearchPos = 0;
								delayNewBehaviour = 3;
							}
						}
					}
				}
			}
		}
	}

	/*
	 * 
	 * --ATTACK FUNCTIONS-- 
	 * 
	 */
	void Attack ()
	{
		if (!startShooting)
		{
			if (SightRaycasts ())
			{
				LookAtTarget (lastKnownPosition);
				charStats.aim = true;

				attackR += Time.deltaTime;

				if (attackR > attackRate)
				{
					startShooting = true;
					timesShot = 0;
					timesToShoot = Random.Range (0, 4);
					attackR = 0.0f;
				}
			}
			else
			{
				charStats.aim = false;
				ChangeAIBehaviour ("AI_State_Chase", 0);
			}
		}
		else
		{
			ShootingBehaviour ();
		}
	}

	/*
	 * 
	 * --SHOOTING FUNCTIONS-- 
	 * 
	 */
	void ShootingBehaviour ()
	{
		if (timesShot < shootRate)
		{
			shootR += Time.deltaTime;

			if (shootR > shootRate)
			{
				muzzleFire.Emit(1);
				//audioSource.Play ();
				charStats.shooting = true;
				timesShot++;
				shootR = 0;
			}
		}
		else
		{
			startShooting = false;
		}
	}

	void LookAtTarget (Vector3 posToLook)
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
		target = null;
		charStats.alert = false;
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

	void AI_State_Search ()
	{
		stateAI = StateAI.search;
		target = null;
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

	void AI_State_HasTarget ()
	{
		stateAI = StateAI.hasTarget;
		charStats.alert = true;
		goToPos = false;
		lookAtPoint = false;
		initCheck = false;
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
