using UnityEngine;
using System.Collections;

public class CharacterStats : MonoBehaviour 
{

	public float health = 100;
	public int morale = 100;
	public int suppresionLevel = 80;
	public int unitRank = 0;

	public float viewAngleLimit = 50.0f;
	public int alertLevel;
	public int team;
	public bool selected;
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

	public Transform alertDebugCube;

	// Use this for initialization
	void Start () 
	{
		health = 100.0f;

		plControl = GetComponent<PlayerControl>();

		if (GetComponent<EnemyAI> ()) 
		{
			enemyAI = GetComponent <EnemyAI> ();
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		//selectCube.SetActive (selected);

		if (run) 
		{
			crouch = false;
		}

		if (alertDebugCube)
		{
			float scale = alertLevel * 0.05f;
			alertDebugCube.localScale = new Vector3(scale, scale, scale);
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

	void KillCharacter()
	{
		dead = true;
	}
}
