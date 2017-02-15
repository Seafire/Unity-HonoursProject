using UnityEngine;
using System.Collections;

public class DebugScript : MonoBehaviour 
{
	GameObject enemy;
	EnemyAI enemyAI;

	// Use this for initialization
	void Start () 
	{
		enemy = GameObject.Find ("Unit_Enemy (2)");
		enemyAI = enemy.GetComponent<EnemyAI> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKey (KeyCode.Space))
		{
			Debug.Log ("Run");
			enemyAI.charStats.morale = 10;
			enemyAI.stateAI = EnemyAI.StateAI.retreat;
			enemyAI.plControl.runSpeed = 3.0f;
		}
	}
}
