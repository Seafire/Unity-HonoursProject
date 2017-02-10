using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackBehaviour : MonoBehaviour 
{
	// General Behaviour Variables
	public float delayNewBehaviour = 3.0f;
	private float timeNewBehaviour;
	
	// Shooting Variables
	private int timesToShoot;
	private int timesShot;

	// Attacking Variables
	public bool startShooting;
	public float attackRate = 1.5f;
	public float shootRate = 0.3f;
	private float shootR;
	private float attackR;
	public ParticleSystem muzzleFire;

	// Cover Variables
	private bool findCoverPosition;
	public List <Transform> coverPositions = new List<Transform> ();
	public List <Transform> ignorePositions = new List<Transform> ();
	private ObjectDistanceComparer comparer;
	public CoverBase currentCover;
	public int maxTries = 3;
	public int curTries;

	EnemyAI enemyAI_Main;
	
	// Use this for initialization
	void Start () 
	{
		enemyAI_Main = GetComponent<EnemyAI> ();
	}

	public void Cover ()
	{
		if (!findCoverPosition)
		{
			FindCover ();
		}
		else
		{
			enemyAI_Main.charStats.MoveToPosition (currentCover.positionObject.position);
			enemyAI_Main.charStats.run = true;

			float dis = Vector3.Distance (transform.position, currentCover.positionObject.position);

			if (dis < 0.5f)
			{
				enemyAI_Main.charStats.StopMoving ();
				enemyAI_Main.AI_State_Attack ();
			}
		}
	}

	void SortPosition (List<Transform> _positions)
	{
		comparer = new ObjectDistanceComparer (this.transform);
		_positions.Sort (comparer);
	}

	private class ObjectDistanceComparer : IComparer<Transform>
	{
		private Transform referenceObject;

		public ObjectDistanceComparer (Transform reference)
		{
			referenceObject = reference;
		}

		public int Compare (Transform x, Transform y)
		{
			float disX = Vector3.Distance (x.position, referenceObject.position);

			float disY = Vector3.Distance (y.position, referenceObject.position);

			int retVal = 0;

			if (disX < disY)
			{
				retVal = -1;
			}
			else if (disX > disY)
			{
				retVal = 1;
			}

			return retVal;
		}
	}

	void FindCover ()
	{
		if (curTries <= maxTries)
		{
			if (!findCoverPosition)
			{
				findCoverPosition = true;
				curTries++;

				CoverBase targetCoverPos = null;
				float disToTarget = Vector3.Distance (transform.position, enemyAI_Main.target.transform.position);
				
				coverPositions.Clear ();
				
				Vector3 tarPosition = enemyAI_Main.target.transform.position;
				
				Collider[] colliders = Physics.OverlapSphere (transform.position, 20);
				
				for (int i = 0; i < colliders.Length; i++)
				{
					if (colliders[i].GetComponent<CoverPositions> ())
					{
						if (!ignorePositions.Contains (colliders[i].transform))
						{
							float disToCanidate = Vector3.Distance (transform.position, colliders[i].transform.position);
							
							if (disToCanidate < disToTarget)
							{
								coverPositions.Add (colliders[i].transform);
							}
						}
					}
				}
				
				if (coverPositions.Count > 0)
				{
					SortPosition (coverPositions);
					
					CoverPositions validatePosition = coverPositions[0].GetComponent<CoverPositions> ();
					
					Vector3 dirOfTarget = tarPosition - validatePosition.transform.position;
					Vector3 coverForward = validatePosition.transform.TransformDirection (Vector3.forward);
					
					if (Vector3.Dot (coverForward, dirOfTarget) < 0)
					{
						for (int i = 0; i < validatePosition.BackPositions.Count; i++)
						{
							if (!validatePosition.BackPositions[i].occupied)
							{
								targetCoverPos = validatePosition.BackPositions[i];
							}
						}
					}
					else
					{
						for (int i = 0; i < validatePosition.FrontPositions.Count; i++)
						{
							if (!validatePosition.FrontPositions[i].occupied)
							{
								targetCoverPos = validatePosition.FrontPositions[i];
							}
						}
					}
				}
				
				if (targetCoverPos == null)
				{
					findCoverPosition = false;
					
					if (coverPositions.Count > 0)
					{
						ignorePositions.Add (coverPositions[0]);
					}
				}
				else
				{
					targetCoverPos.occupied = true;
					currentCover = targetCoverPos;
				}
			}
		}
		else
		{
			enemyAI_Main.AI_State_Attack ();
		}
	}
	
	/*
	 * 
	 * --ATTACK FUNCTIONS-- 
	 * 
	 */
	public void Attack ()
	{
		enemyAI_Main.charStats.StopMoving ();

		if (!startShooting)
		{
			if (enemyAI_Main.SightRaycasts ())
			{
				enemyAI_Main.LookAtTarget (enemyAI_Main.lastKnownPosition);
				enemyAI_Main.charStats.aim = true;
				
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
				enemyAI_Main.charStats.aim = false;
				enemyAI_Main.AI_State_Chase ();
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
	public void ShootingBehaviour ()
	{
		if (timesShot < shootRate)
		{
			shootR += Time.deltaTime;
			
			if (shootR > shootRate)
			{
				muzzleFire.Emit(1);
				//audioSource.Play ();
				enemyAI_Main.charStats.shooting = true;
				timesShot++;
				shootR = 0;
			}
		}
		else
		{
			startShooting = false;
		}
	}

	public void HasTarget ()
	{
		enemyAI_Main.charStats.StopMoving ();
		
		if (enemyAI_Main.SightRaycasts ())
		{
			Debug.Log ("I should be updating alert");
			if (enemyAI_Main.charStats.alertLevel < 10.0f)
			{
				float distanceToTarget = Vector3.Distance (transform.position, enemyAI_Main.target.transform.position);
				float multiplier = 1 + (distanceToTarget * 0.1f);
				
				/*
				 * FUTURE DEVELOPMENT - INCREAMENT HOW FAST PLAYER IS DETECTED BASED ON DISTANCE
				 */
				
				enemyAI_Main.alertTimer += Time.deltaTime * multiplier;
				
				if (enemyAI_Main.alertTimer > enemyAI_Main.alertTimerIncrement)
				{
					enemyAI_Main.charStats.alertLevel++;
					enemyAI_Main.alertTimer = 0;
				}
			}
			else
			{
				enemyAI_Main.ChangeAIBehaviour ("AI_State_Attack", 0);
			}
			
			enemyAI_Main.LookAtTarget (enemyAI_Main.lastKnownPosition);
		}
		else
		{
			if (enemyAI_Main.charStats.alertLevel > 5)
			{
				enemyAI_Main.AI_State_Chase ();
				enemyAI_Main.pointOfInterest = enemyAI_Main.lastKnownPosition;
			}
			else
			{
				delayNewBehaviour -= Time.deltaTime;
				
				if (delayNewBehaviour <= 0)
				{
					enemyAI_Main.AI_State_Normal ();
					delayNewBehaviour = 3;
				}
			}
		}
	}

}
