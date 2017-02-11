using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour 
{

	private Animator anim;
	private NavMeshAgent agent;
	private CharacterStats charStats;

	public bool moveToPosition;
	public Vector3 destPosition;

	public bool run;
	public bool crouch;
	public bool alert;

	public float walkSpeed = 10.0f;
	public float runSpeed = 20.0f;
	public float crouchSpeed = 7.7f;

	public float stopDistance = 0.1f;
	public float maxStance = 0.9f;
	public float minStance = 0.1f;
	float targetStance;
	float stance;

	// Pathfinding
	Vector3[] path;
	int targetIndex;

	// Use this for initialization
	void Start () 
	{
		//anim = GetComponent<Animator> ();
		//SetUpAnimator ();
		agent = GetComponent<NavMeshAgent> ();
		charStats = GetComponent<CharacterStats> ();
		agent.stoppingDistance = stopDistance - 0.1f;
		agent.speed = walkSpeed;

		agent.updateRotation = true;
		//agent.angularSpeed = 500;
		//agent.autoBraking = false;

		if (GetComponentInChildren<EnemySightSphere> ())
		{
			GetComponentInChildren<EnemySightSphere> ().gameObject.layer = 2;
		}
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		run = charStats.run;

		if (moveToPosition) 
		{
			agent.Resume ();
			agent.updateRotation = true;
			agent.SetDestination (destPosition);

			float distanceToTarget = Vector3.Distance(transform.position, destPosition);

			if (distanceToTarget <= stopDistance)
			{
				moveToPosition = false;
				charStats.run = false;
			}
		}
		else
		{
			// Stop the agent
			agent.Stop ();
			agent.updateRotation = false;

			Debug.Log ("I should only be here once I reach my Destination");
		}

		HandleSpeed ();
		//HandleAiming ();
		HandleAnimation ();
		HandleStates ();
	}

	void SetUpAnimator ()
	{

	}

	void HandleAnimation ()
	{
		/*
		Vector3 relativeDirection = (transform.InverseTransformDirection (agent.desiredVelocity)).normalized;
		float animValue = relativeDirection.z;

		if (!run)
		{
			animValue = Mathf.Clamp (animValue, 0.0f, 0.5f)
		}
		anim.SetFloat ("Forward", animValue, 0.3f, Time.deltaTime)
		*/
	}

	void HandleAiming ()
	{
		// anim.SetBool ("Aim", charStats.aim);

		//if (charStats.shooting)
	//	{
		//	anim.SetTrigger ("Shooting");
		//	charStats.shooting = false;
	//	}
	}

	void HandleSpeed ()
	{
		if (!run)
		{
			if (!crouch)
			{
				// Set agent speed to walkSpeed
				agent.speed = walkSpeed;
			}
			else
			{
				// Set agent speed to crouchSpeed
				agent.speed = crouchSpeed;
			}
		}
		else
		{
			// Set agent speed to runSpeed
			agent.speed = runSpeed;
		}
	}
	
	void HandleStates ()
	{
		if (charStats.run) 
		{
			targetStance = minStance;
		}
		else 
		{
			if (charStats.crouch)
			{
				targetStance = maxStance;
			}
			else
			{
				targetStance = minStance;
			}
		}

		stance = Mathf.Lerp (stance, targetStance, Time.deltaTime * 3.0f);
		//anim.SetFloat("Stance", stance);
	}

	void InitRagdoll ()
	{
		Rigidbody[] rigB = GetComponentsInChildren<Rigidbody> ();
		Collider[] cols = GetComponentsInChildren<Collider> ();

		for (int i = 0; i < rigB.Length; i++)
		{
			rigB[i].isKinematic = true;
		}

		for (int i = 0; i < cols.Length; i++)
		{
			if (i != 0)
			{
				cols[i].gameObject.layer = 10;
			}
			cols[i].isTrigger = true;
		}
	}
}
