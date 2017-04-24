using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySightSphere : MonoBehaviour
{

	EnemyAI enemyAI;
	CharacterStats charStats;
	
	List<CharacterStats> trackingTargets = new List<CharacterStats> ();

	// Use this for initialization
	void Start ()
	{
		enemyAI = GetComponentInParent<EnemyAI> ();
		charStats = GetComponentInParent<CharacterStats> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		for (int i = 0; i < trackingTargets.Count; i++)
		{
			if (trackingTargets[i] != enemyAI.target)
			{
				Vector3 dir = trackingTargets[i].transform.position - transform.position;
				float angleTowardsTarget = Vector3.Angle(transform.parent.forward, dir.normalized);

				if (angleTowardsTarget < charStats.viewAngleLimit)
				{
					enemyAI.target = trackingTargets[i];
				}
			}
			else
			{
				continue;
			}
		}
	}

	public void OnTriggerEnter (Collider col)
	{
		if (col.GetComponent<CharacterStats> ())
		{
			CharacterStats colStats = col.GetComponent<CharacterStats> ();

			if (colStats.team != charStats.team)
			{
				if (!trackingTargets.Contains (colStats))
				{
					trackingTargets.Add (colStats);
				}
			}
			else
			{
				EnemyAI otherAI = colStats.transform.GetComponent<EnemyAI> ();

				if (otherAI != enemyAI)
				{
					if (!enemyAI.AlliesNear.Contains(otherAI))
					{
						enemyAI.AlliesNear.Add (otherAI);
					}
				}
			}
		}

		if (col.GetComponent<POI_Base> ())
		{
			POI_Base poi = col.GetComponent<POI_Base> ();

			if (!enemyAI.PointsOfInterest.Contains (poi))
			{
				enemyAI.PointsOfInterest.Add (poi);
			}
		}
	}

	public void OnTriggerExit (Collider col)
	{
		if (col.GetComponent<CharacterStats> ())
		{
			CharacterStats colStats = col.GetComponent<CharacterStats> ();

			if (trackingTargets.Contains (colStats))
			{
				Debug.Log ("Remove me");
				trackingTargets.Remove (colStats);
			}

			if (colStats.transform.GetComponent<EnemyAI> ())
			{
				EnemyAI otherAI = colStats.transform.GetComponent<EnemyAI> ();

				if (otherAI != enemyAI)
				{
					if (enemyAI.AlliesNear.Contains (otherAI))
					{
						enemyAI.AlliesNear.Remove (otherAI);
					}
				}
			}
		}

		if (col.GetComponent<POI_Base> ())
		{
			POI_Base poi = col.GetComponent<POI_Base> ();
			
			if (enemyAI.PointsOfInterest.Contains (poi))
			{
				enemyAI.PointsOfInterest.Remove (poi);
			}
		}
	}
}
