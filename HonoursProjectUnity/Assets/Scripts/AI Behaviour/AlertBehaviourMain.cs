using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AlertBehaviourMain : MonoBehaviour 
{
	// Alert Variables
	public int indexBehaviour;
	public List<Waypoints> onAlertBehaviour = new List<Waypoints> ();
	
	public bool lookAtPoint;
	private bool initAlert;
	public string[] alertLogic;

	// Look Rotation
	Quaternion targetRot;

	// General Behaviour Variables
	public float delayNewBehaviour = 3.0f;
	private float timeNewBehaviour;

	private float alertTimer;
	private float alertTimerIncrement = 1.0f;
	
	EnemyAI enemyAI_Main;

	// Use this for initialization
	public void Init () 
	{
		enemyAI_Main = GetComponent<EnemyAI> ();
	}
	// All actions of the enemy is alerted by the player
	public void Alert ()
	{
		// Make sure that the enemy is not currently moving - focusing on the the point of interest
		if (!lookAtPoint)
		{
			Vector3 dirToLookAt = enemyAI_Main.pointOfInterest - transform.position;
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
				enemyAI_Main.ChangeAIBehaviour (alertLogic[0], 0);
			}
			
			delayNewBehaviour = 3;
		}
	}
	
	public void AlertExtra ()
	{
		if (onAlertBehaviour.Count > 0)
		{
			Waypoints curBehaviour = onAlertBehaviour[indexBehaviour];
			
			if (!enemyAI_Main.goToPos)
			{
				enemyAI_Main.charStats.MoveToPosition (curBehaviour.targetDestination.position);
				enemyAI_Main.goToPos = true;
			}
			else
			{
				float disToTarget = Vector3.Distance (transform.position, curBehaviour.targetDestination.position);
				
				if (disToTarget < enemyAI_Main.plControl.stopDistance)
				{
					enemyAI_Main.commonBehaviour.CheckPosition (curBehaviour, 1);
				}
			}
		}
	}

}
