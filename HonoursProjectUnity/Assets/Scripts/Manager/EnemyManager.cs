using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
	
	public List<CharacterStats> AllEnemies = new List<CharacterStats> ();
	public List<CharacterStats> EnemyChase = new List<CharacterStats> ();
	public List<CharacterStats> EnemyPatrol = new List<CharacterStats> ();

	public bool showBehaviour;
	public bool resetEnemies;
	public bool univeralAlert;
	public bool allThatCanChase;
	public bool patrolOnly;

	public Transform PointOfInterestDebug;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (resetEnemies)
		{
			for (int i = 0; i < AllEnemies.Count; i++)
			{
				AllEnemies[i].ChangeToNormal ();
			}

			resetEnemies = false;
		}

		if (univeralAlert)
		{
			for (int i = 0; i < AllEnemies.Count; i++)
			{
				AllEnemies[i].ChangeToAlert (PointOfInterestDebug.position);
			}

			univeralAlert = false;
		}

		if (allThatCanChase) 
		{
			
			for (int i = 0; i < EnemyChase.Count; i++)
			{
				EnemyChase[i].ChangeToAlert (PointOfInterestDebug.position);
			}

			allThatCanChase = false;
		}

		if (patrolOnly)
		{
			
			for (int i = 0; i < EnemyPatrol.Count; i++)
			{
				EnemyPatrol[i].ChangeToAlert (PointOfInterestDebug.position);
			}
			
			allThatCanChase = false;
		}

		if (showBehaviour) 
		{
			for (int i = 0; i < AllEnemies.Count; i++)
			{
				AllEnemies[i].GetComponent<EnemyUI> ().IsEnabledUI ();
			}

			showBehaviour = false;
		}
	}

	public void UpdateListOfChaseEnemies ()
	{
		if (AllEnemies.Count > 0)
		{
			for (int i = 0; i < AllEnemies.Count; i++)
			{
				if (AllEnemies[i].GetComponent<EnemyAI> ().canChase)
				{
					if (!EnemyChase.Contains(AllEnemies[i]))
					{
						EnemyChase.Add (AllEnemies[i]);
					}
				}
			}
		}
	}
}
