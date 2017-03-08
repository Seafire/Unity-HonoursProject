using UnityEngine;
using System.Collections;

public class ChaseBehaviour : MonoBehaviour 
{
	// General Behaviour Variables
	public float delayNewBehaviour = 3.0f;
	private float timeNewBehaviour;

	EnemyAI enemyAI_Main;
	
	// Use this for initialization
	public void Init () 
	{
		enemyAI_Main = GetComponent<EnemyAI> ();
	}

	// All actions if the enemy loses site of the player
	public void Chase ()
	{
		if (enemyAI_Main.target == null) 
		{
			if (!enemyAI_Main.goToPos)
			{
				enemyAI_Main.charStats.MoveToPosition (enemyAI_Main.lastKnownPosition);
				enemyAI_Main.charStats.run = true;
				enemyAI_Main.goToPos = true;
			}
		}
		else
		{
			enemyAI_Main.charStats.MoveToPosition (enemyAI_Main.target.transform.position);
			enemyAI_Main.charStats.run = true;
		}
		
		if (!enemyAI_Main.SightRaycasts ())
		{
			if (enemyAI_Main.target)
			{
				enemyAI_Main.lastKnownPosition = enemyAI_Main.target.transform.position;
				enemyAI_Main.target = null;
			}
			else
			{
				float disFromTargetPos = Vector3.Distance (transform.position, enemyAI_Main.lastKnownPosition);
				
				if (disFromTargetPos < 0.1)
				{
					delayNewBehaviour -= Time.deltaTime;
					
					if (delayNewBehaviour <= 0)
					{
						enemyAI_Main.AI_State_Search ();
						delayNewBehaviour = 3;
					}
				}
			}
		}
	}
}
