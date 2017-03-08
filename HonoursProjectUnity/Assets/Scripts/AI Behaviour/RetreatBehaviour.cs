using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RetreatBehaviour : MonoBehaviour 
{

	public float delayTillNewBehaviour = 3.0f;

	private bool hasAction;
	private bool exitLevel;

	Vector3 targetDes;
	public List<Transform> retreatPositions = new List<Transform> ();
	RetreatActionBase currentRetreatPos;
	public List<Transform> exitLevelPositions = new List<Transform> ();

	LevelManager levelManager;

	EnemyAI enemyAI_Main;
	
	// Use this for initialization
	public void Init () 
	{
		levelManager = LevelManager.GetInstance ();
		enemyAI_Main = GetComponent<EnemyAI> ();

		exitLevelPositions = levelManager.exitPositions;
	}

	public void RetreatAction ()
	{
		// Decide Action
		if (!hasAction)
		{	
			// Search for positions not visited
			retreatPositions.Clear ();

			for (int i = 0; i < levelManager.retreatPosition.Count; i++)
			{
				retreatPositions.Add (levelManager.retreatPosition[i].retreatPosition);
			}

			// if something is found
			if (retreatPositions.Count > 0)
			{
				enemyAI_Main.attackBehaviour.SortPosition (retreatPositions);
				
				currentRetreatPos = levelManager.ReturnRetreatDestination(retreatPositions[0]);
				
				if (currentRetreatPos.inUse == false)
				{
					currentRetreatPos.inUse = true;
					
					// Go to point
					targetDes = currentRetreatPos.retreatPosition.position;
					enemyAI_Main.charStats.MoveToPosition (targetDes);
					
					exitLevel = false;
				}
				else 	// If its in use
				{
					
				}
			}
			else 	// If there is no retreat point
			{
				// Exit the level
				enemyAI_Main.attackBehaviour.SortPosition (exitLevelPositions);
				
				targetDes = exitLevelPositions[0].position;
				enemyAI_Main.charStats.MoveToPosition (targetDes);
				exitLevel = true;
			}
			hasAction = true;
		}
		else
		{
			if (exitLevel)
			{
				ExitLevel ();
			}
			else
			{
				RetreatToPosition ();
			}
		}
	}

	void ExitLevel ()
	{
		if (Vector3.Distance (transform.position, targetDes) < 1)
		{
			transform.GetComponent<EnemyUI> ().enemyUI.SetActive (false);
			gameObject.SetActive (false);
		}
	}

	void RetreatToPosition ()
	{
		if (Vector3.Distance (transform.position, targetDes) < 1)
		{
			currentRetreatPos.visited = true;

			if (currentRetreatPos.reinforcements.Count > 0)
			{
				for (int i = 0; i < currentRetreatPos.reinforcements.Count; i++)
				{
					if (currentRetreatPos.reinforcements[i].stateAI == EnemyAI.StateAI.patrol || currentRetreatPos.reinforcements[i].stateAI == EnemyAI.StateAI.search)
					{
						currentRetreatPos.reinforcements[i].AI_State_HasTarget ();
						currentRetreatPos.reinforcements[i].target = enemyAI_Main.target;
						currentRetreatPos.reinforcements[i].charStats.alertLevel = 10;
					}
				}
			}
			else
			{
				enemyAI_Main.alliesBehaviour.AlertEveryoneInRange (20);
			}

			enemyAI_Main.charStats.morale = 100;
			enemyAI_Main.AI_State_HasTarget ();

			hasAction = false;
			exitLevel = false;
		}
	}

}
