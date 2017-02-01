using UnityEngine;
using System.Collections;

public class CharacterStats : MonoBehaviour 
{

	public float health;
	public int team;
	public bool selected;
	public bool dead;
	public bool crouch;
	public bool run;
	public bool alert = true;
	//public GameObject selectCube;
	private PlayerControl plControl;
	private EnemyAI enemyAI;

	// Use this for initialization
	void Start () 
	{
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
	}

	public void MoveToPosition (Vector3 position)
	{
		plControl.moveToPosition = true;
		plControl.destPosition = position;
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
		MoveToPosition (transform.position);
		//plControl.moveToPosition = false;

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
