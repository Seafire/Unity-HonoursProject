using UnityEngine;
using System.Collections;

public class PatrolState : IEnemyState 
	
{
	private readonly StatePatternEnemy enemy;
	private int nextWayPoint;
	
	public PatrolState (StatePatternEnemy statePatternEnemy)
	{
		enemy = statePatternEnemy;
	}
	
	public void UpdateState()
	{
		Look ();
		Patrol ();
	}
	
	public void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.CompareTag ("Player"))
			ToAttackState ();
	}
	
	public void ToPatrolState()
	{
		Debug.Log ("Can't transition to same state");
	}
	
	public void ToEngageState()
	{
		enemy.currentState = enemy.engageState;
	}
	
	public void ToAttackState()
	{
		enemy.currentState = enemy.attackState;
	}
	
	private void Look()
	{
		RaycastHit hit;
		if (Physics.Raycast (enemy.eyes.transform.position, enemy.eyes.transform.forward, out hit, enemy.sightRange) && hit.collider.CompareTag ("Player")) {
			enemy.chaseTarget = hit.transform;
			ToEngageState();
		}
	}
	
	void Patrol ()
	{
		enemy.meshRendererFlag.material.color = Color.green;
	}
}
