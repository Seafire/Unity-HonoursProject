using UnityEngine;
using System.Collections;

public class PatrolBehaviour : MonoBehaviour 
{

	EnemyAI enemyAI_Main;
	
	// Use this for initialization
	void Start () 
	{
		enemyAI_Main = GetComponent<EnemyAI> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
