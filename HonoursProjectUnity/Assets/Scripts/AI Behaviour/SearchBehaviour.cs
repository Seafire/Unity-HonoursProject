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

	EnemyAI enemyAI_Main;
	
	// Use this for initialization
	void Start () 
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
					
					Collider[] allColliders = Physics.OverlapSphere (transform.position, 20);
					
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
						decideBehaviour = (ranVal <5);
						
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
}
