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
	PlayerControl plControl;

	// Use this for initialization
	void Start () 
	{
		plControl = GetComponent<PlayerControl>();
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
