using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoverPositions : MonoBehaviour 
{
	public List<CoverBase> FrontPositions = new List<CoverBase> ();
	public List<CoverBase> BackPositions = new List<CoverBase> ();

	void Start ()
	{
		for (int i = 0; i < BackPositions.Count; i++)
		{
			BackPositions[i].backPos = true;
		}
	}

}

[System.Serializable]
public class CoverBase
{
	public bool occupied;
	public Transform positionObject;
	public bool backPos;
}
