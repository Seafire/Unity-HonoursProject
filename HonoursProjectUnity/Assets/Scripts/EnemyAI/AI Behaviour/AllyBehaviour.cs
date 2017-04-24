using UnityEngine;
using System.Collections;

public class AllyBehaviour : MonoBehaviour 
{

	EnemyAI enemyAI_Main;
	
	// Use this for initialization
	public void Init () 
	{
		enemyAI_Main = GetComponent<EnemyAI> ();
	}
	
	public void AlertAllies ()
	{
		if (enemyAI_Main.AlliesNear.Count > 0)
		{
			for (int i = 0; i < enemyAI_Main.AlliesNear.Count; i++)
			{
				if (enemyAI_Main.AlliesNear[i].stateAI == EnemyAI.StateAI.patrol)
				{
					enemyAI_Main.AlliesNear[i].AI_State_HasTarget ();
					enemyAI_Main.AlliesNear[i].target = enemyAI_Main.target;
					enemyAI_Main.AlliesNear[i].charStats.alertLevel = 10;
				}
			}
		}
	}

	public void AlertEveryoneInRange (float _range)
	{
		LayerMask mask = 1 << gameObject.layer;

		Collider[] col = Physics.OverlapSphere (transform.position, _range, mask);

		Debug.Log (col.Length);

		for (int i = 0; i < col.Length; i++)
		{
			if (col[i].transform.GetComponent<EnemyAI> ())
			{
				EnemyAI otherAI = col[i].transform.GetComponent<EnemyAI> ();

				if (otherAI.stateAI == EnemyAI.StateAI.patrol)
				{
					enemyAI_Main.AlliesNear[i].AI_State_HasTarget ();
					enemyAI_Main.AlliesNear[i].target = enemyAI_Main.target;
					enemyAI_Main.AlliesNear[i].charStats.alertLevel = 10;
				}
			}
		}
	}

	public void DecreaseAlliesMorale (int _amount)
	{
		if (enemyAI_Main.AlliesNear.Count > 0)
		{
			for (int i = 0; i < enemyAI_Main.AlliesNear.Count; i ++)
			{
				enemyAI_Main.AlliesNear[i].charStats.morale -= _amount;
			}
		}
	}

	public void AddInterestOnAlliesPOI (POI_Base poi)
	{
		if (enemyAI_Main.AlliesNear.Count > 0)
		{
			for (int i = 0; i < enemyAI_Main.AlliesNear.Count; i++)
			{
				if (!enemyAI_Main.AlliesNear[i].PointsOfInterest.Contains (poi))
				{
					enemyAI_Main.AlliesNear[i].PointsOfInterest.Add (poi);
				}
			}
		}
	}
}
