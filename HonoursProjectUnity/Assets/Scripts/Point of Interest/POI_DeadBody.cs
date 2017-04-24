using UnityEngine;
using System.Collections;

public class POI_DeadBody : POI_Base
{

	public CharacterStats owner;

	void Start ()
	{
		owner = GetComponentInParent<CharacterStats> ();
		owner.enableOnDeath = this;
		this.enabled = false;
	}
}
