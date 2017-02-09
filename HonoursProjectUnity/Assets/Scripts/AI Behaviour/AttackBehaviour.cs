using UnityEngine;
using System.Collections;

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

	EnemyAI enemyAI_Main;
	
	// Use this for initialization
	void Start () 
	{
		enemyAI_Main = GetComponent<EnemyAI> ();
	}

	
	// Update is called once per frame
	void Update () {
	
	}

	/*
	 * 
	 * --ATTACK FUNCTIONS-- 
	 * 
	 */
	public void Attack ()
	{
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
