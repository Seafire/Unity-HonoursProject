using UnityEngine;
using System.Collections;

public class UnconsiousBehaviour : MonoBehaviour 
{

	EnemyAI enemyAI_Main;
	private float timeNewBehaviour;
	
	// Use this for initialization
	public void Init () 
	{
		enemyAI_Main = GetComponent<EnemyAI> ();
		timeNewBehaviour = 0;


	}

	// Update is called once per frame
	void Update () 
	{
	
	}

	public void Unconious ()
	{
	/*	timeNewBehaviour += Time.deltaTime;

		if (timeNewBehaviour > 5)
		{
			enemyAI_Main.charStats.stamina = 100;
			
			//enemyAI_Main.plControl.moveToPosition = false;
			
			enemyAI_Main.charStats.alert = true;
			enemyAI_Main.charStats.alertLevel = 8;
			enemyAI_Main.charStats.crouch = true;
			enemyAI_Main.charStats.plControl.agent.enabled = true;
			enemyAI_Main.charStats.plControl.moveToPosition = false;
			enemyAI_Main.AI_State_OnAlert_RunList ();

			Debug.Log ("Wake Up");
			
			timeNewBehaviour = 0;
		}
	*/}
}
