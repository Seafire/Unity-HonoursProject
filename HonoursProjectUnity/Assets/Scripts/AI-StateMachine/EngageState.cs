using UnityEngine;
using System.Collections;

public class EngageState : IEnemyState
{

	private readonly StatePatternEnemy enemy;
	private float searchTimer;
	
	public EngageState (StatePatternEnemy statePatternEnemy)
	{
		enemy = statePatternEnemy;
	}
	
	public void UpdateState()
	{
		Look ();
		Search ();
	}
	
	public void OnTriggerEnter (Collider other)
	{
		
	}
	
	public void ToPatrolState()
	{
		enemy.currentState = enemy.patrolState;
		searchTimer = 0f;
	}
	
	public void ToEngageState()
	{
		Debug.Log ("Can't transition to same state");
	}
	
	public void ToAttackState()
	{
		enemy.currentState = enemy.attackState;
		searchTimer = 0f;
	}
	
	private void Look()
	{
		RaycastHit hit;
		if (Physics.Raycast (enemy.eyes.transform.position, enemy.eyes.transform.forward, out hit, enemy.sightRange) && hit.collider.CompareTag ("Player")) {
			enemy.chaseTarget = hit.transform;
			ToAttackState();
		}
	}
	
	private void Search()
	{
		enemy.meshRendererFlag.material.color = Color.yellow;
		enemy.transform.Rotate (0, enemy.searchingTurnSpeed * Time.deltaTime, 0);
		searchTimer += Time.deltaTime;
		
		if (searchTimer >= enemy.searchingDuration)
			ToPatrolState ();
	}
}
