using UnityEngine;
using System.Collections;

public class POI_Unconsious : POI_Base 
{
	public CharacterStats owner;

	// Use this for initialization
	void Start () 
	{
		owner = GetComponentInParent<CharacterStats> ();
		owner.enableOnUnconsious = this;
		this.enabled = false;
	}
}
