using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{

	public CharacterStats selectedUnit;
	public int playerTeam;
	public GameObject unitControls;					/* Public Gameobject to activate/deactivate buttons when player is selected */
	public bool doubleClick;						/* To trigger if the player will run */ 
	public bool overUIElements;


	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!overUIElements)
			HandleSelection ();

		bool hasUnit = selectedUnit;
	//	unitControls.SetActive (hasUnit);
	}

	void HandleSelection ()
	{

	}

	IEnumerator DoubleClickCheck()
	{
		yield return new WaitForSeconds (1);
		doubleClick = false;
	}
}
