using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSightController : MonoBehaviour 
{


	EnemyAI enemyAI;
	Unit unit;
	CharacterStats charStats;
	
	public List<EnemyAI> trackingTargets = new List<EnemyAI> ();

	// Use this for initialization
	void Start () 
	{
		unit = GetComponentInParent<Unit> ();
		charStats = GetComponentInParent<CharacterStats> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		for (int i = 0; i < trackingTargets.Count; i++)
		{
			Vector3 dir = trackingTargets[i].transform.position - transform.position;
			float angleTowardsTarget = Vector3.Angle(transform.parent.forward, dir.normalized);
			
			if (angleTowardsTarget < charStats.viewAngleLimit)
			{
				//enemyAI.target = trackingTargets[i];

				Debug.Log ("I have the enemy in sights");
				unit.sleepTar = trackingTargets[i];

				// Set a variable to allow sleep to effect enemy
				// Add variable to Unit script as only applies to unit
				// Or it may need to be a variable set in the enemy AI to put them to sleep
				// Change the stamina to 0
			}
			else
			{
				// Variable is false
			}
		}

		if (trackingTargets == null) 
		{
			unit.sleepTar = null;
		}
	}

	public void OnTriggerEnter (Collider col)
	{
		if (col.GetComponent<EnemyAI> ())
		{
			EnemyAI colStats = col.GetComponent<EnemyAI> ();
	
			if (!trackingTargets.Contains (colStats))
			{
				trackingTargets.Add (colStats);
			}
		}
	}
	
	public void OnTriggerExit (Collider col)
	{
		if (col.GetComponent<EnemyAI> ())
		{
			EnemyAI colStats = col.GetComponent<EnemyAI> ();
			
			if (trackingTargets.Contains (colStats))
			{
				Debug.Log ("Remove me");
				trackingTargets.Remove (colStats);
			}
		}
	}
}
