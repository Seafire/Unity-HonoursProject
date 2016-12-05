using UnityEngine;
using System.Collections;

public class StatePatternEnemy : MonoBehaviour 
{
	public float searchingTurnSpeed = 120f;
	public float searchingDuration = 4f;
	public float sightRange = 20f;
	public Transform[] wayPoints;
	public Transform eyes;
	public Vector3 offset = new Vector3 (0,.5f,0);
	public MeshRenderer meshRendererFlag;
	
	
	[HideInInspector] public Transform chaseTarget;
	[HideInInspector] public IEnemyState currentState;
	[HideInInspector] public AttackState attackState;
	[HideInInspector] public EngageState engageState;
	[HideInInspector] public PatrolState patrolState;
	
	private void Awake()
	{
		attackState = new AttackState (this);
		engageState = new EngageState (this);
		patrolState = new PatrolState (this);

	}
	
	// Use this for initialization
	void Start () 
	{
		currentState = patrolState;
	}
	
	// Update is called once per frame
	void Update () 
	{
		currentState.UpdateState ();
	}
	
	private void OnTriggerEnter(Collider other)
	{
		currentState.OnTriggerEnter (other);
	}
}