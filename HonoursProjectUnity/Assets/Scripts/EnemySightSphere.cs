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
		}
	}

	public void OnTriggerExit (Collider col)
	{
		if (col.GetComponent<CharacterStats> ())
		{
			CharacterStats colStats = col.GetComponent<CharacterStats> ();

			if (!trackingTargets.Contains (colStats))
			{
				trackingTargets.Remove (colStats);
			}
		}
	}
}
