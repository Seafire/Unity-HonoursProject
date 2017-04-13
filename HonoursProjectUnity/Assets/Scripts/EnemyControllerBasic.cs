using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyControllerBasic : MonoBehaviour
{
	public List<EnemyAI> allEnemies = new List<EnemyAI> ();
	public List<EnemyAI> defaultEnemies = new List<EnemyAI> ();
	public List<EnemyAI> smartEnemies = new List<EnemyAI> ();

	public List<Transform> extraWaypoint = new List<Transform> ();
	public List<Transform> extraLookTo = new List<Transform> ();
	public List<Transform> defaultWaypoint = new List<Transform> ();
	public List<Transform> defaultLookTo = new List<Transform> ();
	//public GameObject[] allEnemies;
	private int enemies = 0;
	private int maxWaitTime;
	private float orderDelay;
	private int cheapfix = 1;

	// Access to the enemy AI
	EnemyAI enemyAI_Main;

	// Use this for initialization
	void Start () 
	{
		// Get all enemies accessable to the AI
		foreach (EnemyAI enemy in gameObject.GetComponentsInChildren<EnemyAI> ()) 
		{
			allEnemies.Add (enemy);
			if (allEnemies.Count < 9)
			{
				defaultEnemies.Add (enemy);
			}
			else
			{
				smartEnemies.Add (enemy);
			}
		}

		// Can change these to be unique for each enemy
		maxWaitTime = 12;
		orderDelay = 20;

		for (int i = 0; i < defaultEnemies.Count; i++)
		{
			if (defaultEnemies[i].waypoints.Count > 0)
			{
				defaultWaypoint.Add (defaultEnemies[i].waypoints[1].targetDestination);
				defaultLookTo.Add (defaultEnemies[i].waypoints[1].targetLookTo);
			}
		}


		enemies = gameObject.GetComponentsInChildren<EnemyAI> ().Length;

		for (int i = 0; i < allEnemies.Count; i++)
		{
			// Start all enemies as inactive
			// allEnemies[i].gameObject.SetActive (false);


			// For more advanced AI:
			// Need access to the waypoints of that unit 
			// Each waypoint needs collider to current lookat point
			// Lookat point must exist
			// Can add extra waypoints and switvh out the position routes
			// Change times waiting at wach point
			// If collding - don't double up enemies

			// How to chose what enemies to spawn - may not need if waypoints change...

			// Basic method pick randomly

			// Needs to be more advance

			/*	Task List
			 *  Move waypoints to main AI script - DONE
			 * 	Make sure components can be accessed - If not convert to Class - DONE
			 *  Set-up more enemies and test to an extent
			 *  Add end Level - Basic
			 *  Predication Level complete
			 *  Set up all waypoints
			 *  Create secondary waypoints for enemies
			 *  Create a system to change between waypoints after each route is complete - can be random as all routes start and end at the same point - DONE
			 *  Create more advanced method for updating waypoints
			 *  Only add alterate midpoint routes so it can switch each cycle
			 *  Ajust waypoint wait times each loop - DONE - Updates orders of each enemies and ajusts the wait times - DONE
			 *  Think about setting up order delays and wait time for individual enemies
			 * 	Extra - alter look at points - DONE
			 *  Add better method for adding extra waypoints - System for altering 2 waypoint enemies compared to 3
			 *  Only after everything else is done
			 *  Think about Fuzzy Logic for responses to player detection
			 */

		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		// For all default enemies
		for (int i = 0; i < defaultEnemies.Count; i++)
		{
			// If this instance of enemy has 3 waypoints
			if (defaultEnemies[i].waypoints.Count > 1)
			{
				/*
				 * Alter the waypoints here 
				 * Need to delay the update to waypoints
				 */
				Debug.Log (defaultEnemies[i].commonBehaviour.getIndexList());
				// Set the extra waypoint
				if (defaultEnemies[i].commonBehaviour.getIndexList() == 2 || defaultEnemies[i].commonBehaviour.getIndexList() == 0)
				{
					if (cheapfix == 1 )
					{
						Debug.Log ("Flip me");
						defaultEnemies[i].waypoints[1].targetDestination = extraWaypoint[i];

						// If the enemies looks at points - then add new point
						if (defaultEnemies[i].waypoints[1].lookTo == true)
						{
							// Set the new point
							defaultEnemies[i].waypoints[1].targetLookTo = extraLookTo[i];
						}
						cheapfix *= -1;
					}
					else if (cheapfix == -1)
					{
						
						defaultEnemies[i].waypoints[1].targetDestination = defaultWaypoint[i];
						// If the enemies looks at points - then add new point
						if (defaultEnemies[i].waypoints[1].lookTo == true)
						{
							// Set the new point
							defaultEnemies[i].waypoints[1].targetLookTo = defaultLookTo[i];
						}
						cheapfix *= -1;
					}
				}

				if (defaultEnemies[i].alterWaitTime)
				{
					orderDelay -= Time.deltaTime;

					if (orderDelay <= 0)
					{
						int delay1 = Random.Range(2, maxWaitTime - 4);
						maxWaitTime -= delay1;
						int delay2 = Random.Range(2, maxWaitTime - 3);
						maxWaitTime -= delay2;
						int delay3 = Random.Range(2, 6);

						defaultEnemies[i].waypoints[0].waitTime = delay1;
						defaultEnemies[i].waypoints[1].waitTime = delay2;
						defaultEnemies[i].waypoints[2].waitTime = maxWaitTime;

						orderDelay = Random.Range(15, 30);
						maxWaitTime = 12;

					}
				}
			}
		}

	}

	public struct Enemies
	{
		EnemyAI enemy_AI;
	}
}
