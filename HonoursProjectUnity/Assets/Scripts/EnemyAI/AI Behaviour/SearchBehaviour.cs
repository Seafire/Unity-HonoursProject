using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SearchBehaviour : MonoBehaviour 
{
	// Searching Variables
	public bool decideBehaviour;
	public float decideBehaviourThreshold = 5.0f;
	public List<Transform> possibleHidingAreas = new List<Transform> ();
	public List<Vector3> positionsArroundUnit = new List<Vector3> ();
	private bool getPossibleHidingArea;
	private bool populateListPositions;
	private bool searchAtPoint;
	private bool createSearchPos;
	private bool searchHidingSpot;
	private int indexSearchPos;
	Transform targetHidingSpot;

	// General Behaviour Variables
	public float delayNewBehaviour = 3.0f;
	private float timeNewBehaviour;

	[HideInInspector]
	public CharacterStats curBody;
	[HideInInspector]
	public Vector3 targetLocation;
	public int searchPhase;

	EnemyAI enemyAI_Main;
	
	// Use this for initialization
	public void Init () 
	{
		enemyAI_Main = GetComponent<EnemyAI> ();
	}

	public void Search ()
	{
		if (!decideBehaviour)
		{
			int ranVal = Random.Range (0, 11);
			
			if (ranVal < decideBehaviourThreshold)
			{
				searchAtPoint = true;
				Debug.Log ("Searching in position around unit");
			}
			else
			{
				searchHidingSpot = true;
				Debug.Log ("Searching in Hiding Spots");
			}
			
			decideBehaviour = true;
		}
		else
		{
			if (searchHidingSpot)
			{
				if (!populateListPositions)
				{
					possibleHidingAreas.Clear();
					
					Collider[] allColliders = Physics.OverlapSphere (transform.position, 100);
					
					for (int i = 0; i < allColliders.Length; i++)
					{
						if (allColliders[i].GetComponent<HidingSpot>())
						{
							possibleHidingAreas.Add (allColliders[i].transform);
						}
					}
					populateListPositions = true;
				}
				else if (possibleHidingAreas.Count > 0)
				{
					if (!targetHidingSpot)
					{
						int ranVal = Random.Range (0, possibleHidingAreas.Count);
						
						targetHidingSpot = possibleHidingAreas [ranVal];
					}
					else
					{
						enemyAI_Main.charStats.MoveToPosition (targetHidingSpot.position);
						
						Debug.Log ("Going to hiding spot");
						float disToTarget = Vector3.Distance (transform.position, targetHidingSpot.position);
						
						if (disToTarget < 2)
						{
							delayNewBehaviour -= Time.deltaTime;
							
							if (delayNewBehaviour <= 0)
							{
								// reset
								populateListPositions = false;
								targetHidingSpot = null;
								decideBehaviour = false;
								delayNewBehaviour = 3;
							}
						}
					}
				}
				else
				{
					Debug.Log ("No hiding spots found");
					searchAtPoint = false;
					populateListPositions = false;
					targetHidingSpot = null;
				}
			}
			
			
			if (searchAtPoint)
			{
				if (!createSearchPos)
				{
					positionsArroundUnit.Clear();
					
					int ranVal = Random.Range (4, 10);
					
					for (int i = 0; i < ranVal; i++)
					{
						float offsetX = Random.Range (-10, 10);
						float offsetZ = Random.Range (-10, 10);
						
						Vector3 originPos = transform.position;
						originPos += new Vector3 (offsetX, 0, offsetZ);
						
						NavMeshHit hit;
						
						if (NavMesh.SamplePosition (originPos, out hit, 5, NavMesh.AllAreas))
						{
							positionsArroundUnit.Add (hit.position);
						}
					}
					
					if (positionsArroundUnit.Count > 0)
					{
						indexSearchPos = 0;
						createSearchPos = true;
					}
				}
				else
				{
					Vector3 targetPos = positionsArroundUnit[indexSearchPos];
					
					Debug.Log ("Going to position");
					
					enemyAI_Main.charStats.MoveToPosition (targetPos);
					
					float disToPos = Vector3.Distance (transform.position, targetPos);
					
					if (disToPos < 2)
					{
						int ranVal = Random.Range (0, 11);
						decideBehaviour = (ranVal < 5);
						
						if (indexSearchPos < positionsArroundUnit.Count - 1)
						{
							delayNewBehaviour -= Time.deltaTime;
							
							if (delayNewBehaviour <= 0)
							{
								indexSearchPos++;
								delayNewBehaviour = 3;
							}
						}
						else
						{
							delayNewBehaviour -= Time.deltaTime;
							
							if (delayNewBehaviour <= 0)
							{
								indexSearchPos = 0;
								delayNewBehaviour = 3;
							}
						}
					}
				}
			}
		}
	}

	public void SearchTargetLocationOrBody ()
	{
		if (curBody)
		{
			targetLocation = curBody.enableOnUnconsious.transform.position;
		}

		switch (searchPhase)
		{
		case 0:

			enemyAI_Main.LookAtTarget (targetLocation);
			enemyAI_Main.plControl.moveToPosition = false;

			timeNewBehaviour += Time.deltaTime;

			if (timeNewBehaviour > 4)
			{
				searchPhase++;
			}
			break;
		case 1:
			enemyAI_Main.charStats.MoveToPosition (targetLocation);
			float distanceToTarget = Vector3.Distance (transform.position, targetLocation);
			if (distanceToTarget < 2)
			{

				enemyAI_Main.charStats.alert = true;
				enemyAI_Main.charStats.alertLevel = 2;
				enemyAI_Main.charStats.crouch = true;

				timeNewBehaviour += Time.deltaTime;

				if (timeNewBehaviour > 4)
				{
					curBody.stamina = 100;
					curBody = null;

					enemyAI_Main.plControl.moveToPosition = false;

					searchPhase++;

					timeNewBehaviour = 0;
				}
			}
			break;
		case 2:
			timeNewBehaviour += Time.deltaTime;

			if (timeNewBehaviour > 8)
			{
				enemyAI_Main.AI_State_Normal ();

				timeNewBehaviour = 0;
			}
			break;
		}
	}
}
