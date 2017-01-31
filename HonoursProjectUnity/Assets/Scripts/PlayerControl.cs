﻿using UnityEngine;
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

	}
	
	// Update is called once per frame
	void Update () 
	{
		run = charStats.run;

		if (moveToPosition) 
		{
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
		}

		HandleSpeed ();
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
}
