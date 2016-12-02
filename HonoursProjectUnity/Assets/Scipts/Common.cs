using UnityEngine;
using System.Collections;

public class Common : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static float Unsigned(float val)
	{
		if (val < 0)
			val *= -1;
		
		return val;
	}

	public static bool FloatToBool(float val)
	{
		if (val < 0)
			return false;
		else
			return true;
	}

	public static bool ShiftKeysDown()
	{
		if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift))
			return true;
		else
			return false;
	}
}
