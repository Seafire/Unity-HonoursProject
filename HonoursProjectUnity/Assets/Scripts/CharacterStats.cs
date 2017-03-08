using UnityEngine;
using System.Collections;

public class CharacterStats : MonoBehaviour 
{

	public float health = 100;
	public float stamina = 100;
	public int morale = 100;
	public int suppresionLevel = 80;
	public int unitRank = 0;

	public float viewAngleLimit = 50.0f;
	public int alertLevel;
	public int team;
	public bool selected;
	public bool unconsious;
	public bool dead;
	public bool crouch;
	public bool hasCover;
	public bool run;
	public bool alert = true;
	public bool aim;
	public bool shooting;
	//public GameObject selectCube;
	private PlayerControl plControl;
	private EnemyAI enemyAI;

	private float stTimer = 0;		/* Timer used for stamina */

	[HideInInspector]
	public POI_DeadBody enableOnDeath;
	[HideInInspector]
	public POI_Unconsious enableOnUnconsious;

	public Transform alertDebugCube;

	// Use this for initialization
	void Start () 
	{
		health = 100.0f;
		stamina = 100.0f;

		plControl = GetComponent<PlayerControl>();

		if (GetComponent<EnemyAI> ()) 
		{
			enemyAI = GetComponent <EnemyAI> ();
		}

		plControl.Init ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		//selectCube.SetActive (selected);
		plControl.Tick ();

		if (run) 
		{
			crouch = false;
		}

		if (alertDebugCube)
		{
			float scale = alertLevel * 0.05f;
			alertDebugCube.localScale = new Vector3(scale, scale, scale);
		}

		if (morale < 0)
		{
			morale = 0;
		}

		if (health <= 0)
		{
			health = 0;

			if (!dead)
			{
				dead = true;

				if (enemyAI)
				{
					//enemyAI.AlliesNear.DecreaseAlliesMorale(30);
					enemyAI.alliesBehaviour.DecreaseAlliesMorale (30);
					enemyAI.alliesBehaviour.AddInterestOnAlliesPOI (enableOnDeath.transform.GetComponent<POI_Base> ());
					enemyAI.stateAI = EnemyAI.StateAI.dead;
				}

				KillCharacter ();
			}
		}

		if (!unconsious)
		{
			if (stamina <= 0)
			{
				stamina = 0;

				enemyAI.stateAI = EnemyAI.StateAI.unconsious;
				enemyAI.alliesBehaviour.AddInterestOnAlliesPOI (enableOnDeath.transform.GetComponent<POI_Base> ());
				enableOnUnconsious.enabled = true;
				// plControl.RagdollCharacter ();
				plControl.agent.Stop ();
				plControl.agent.enabled = false;
				unconsious = true;
				alert = true;
				alertLevel = 5;
			}
		}
		else
		{
			stTimer += Time.deltaTime;
			if (stTimer > 1)
			{
				stamina++;
				stTimer = 0;
			}

			if (stamina > 99)
			{
				unconsious = false;
				Vector3 curPos = enableOnUnconsious.transform.position;
				transform.position = curPos;

				plControl.agent.enabled = true;
				plControl.moveToPosition = false;
				// plControl.InitRagdoll ();
			}
		}
	}

	public void MoveToPosition (Vector3 position)
	{
		plControl.moveToPosition = true;
		plControl.destPosition = position;
	}

	public void StopMoving ()
	{
		plControl.moveToPosition = false;
	}

	public void CallFunctionWithString (string functionIndentifier, float delay)
	{
		Invoke (functionIndentifier, delay);
	}

	public void ChangeToNormal ()
	{
		enemyAI.ChangeAIBehaviour ("AI_State_Normal", 0);
		alert = false;
		crouch = false;
		run = false;
	}

	public void ChangeToAlert (Vector3 _interest)
	{
		alert = true;
		plControl.moveToPosition = false;

		enemyAI.GoOnAlert (_interest);
	}

	void ChangeStance ()
	{
		crouch = !crouch;
	}

	void AlertPhase ()
	{
		alert = !alert;
	}

	void ChangeRunState ()
	{
		run = !run;
	}

	public void KillCharacter()
	{
		//plControl.RagdollCharacter ();

		MonoBehaviour[] comp = GetComponents<MonoBehaviour> ();

		for (int i = 0; i < comp.Length; i++)
		{
			comp[i].enabled = false;
		}

		this.enabled = true;
		/*
		enemyAI.commonBehaviour.enabled = false;
		enemyAI.chaseBehaviour.enabled = false;
		enemyAI.searchBehaviour.enabled = false;
		enemyAI.attackBehaviour.enabled = false;
		enemyAI.alertBehaviours.enabled = false;
		enemyAI.alliesBehaviour.enabled = false;
		enemyAI.enabled = false;
		*/
		GetComponent<Collider> ().enabled = false;
		GetComponent<Rigidbody> ().isKinematic = true;
		GetComponent<NavMeshAgent> ().enabled = false;
		enableOnDeath.enabled = true;

	}
}
