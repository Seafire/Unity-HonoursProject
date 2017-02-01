using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PointOfInterest : MonoBehaviour 
{
	public bool createPointOfInterest;

	public List <CharacterStats> affectedCharacters = new List<CharacterStats> ();

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (createPointOfInterest)
		{
			for (int i = 0; i < affectedCharacters.Count; i++)
			{
				Vector3 curPos = transform.position;
				affectedCharacters[i].ChangeToAlert (curPos);
			}

			createPointOfInterest = false;
		}
	}

	public void OnTriggerEnter (Collider col)
	{
		if (col.GetComponent <CharacterStats> ())
		{
			if (!affectedCharacters.Contains (col.GetComponent<CharacterStats> ()))
			{
				Debug.Log ("Kill me");
				affectedCharacters.Add (col.GetComponent<CharacterStats> ());
			}
		}
	}

	public void OnTriggerExit (Collider col)
	{
		if (col.GetComponent <CharacterStats> ())
		{
			if (affectedCharacters.Contains (col.GetComponent<CharacterStats> ()))
			{
				affectedCharacters.Remove (col.GetComponent<CharacterStats> ());
			}
		}
	}
}
