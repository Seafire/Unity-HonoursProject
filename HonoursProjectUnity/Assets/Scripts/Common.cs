using UnityEngine;
using System.Collections;

public class Common : MonoBehaviour 
{
	/* Common function to return a positive value */
	public static float Unsigned(float _val)
	{
		if (_val < 0)
			_val *= -1;
		
		return _val;
	}
	/* Common function to return true or false with pass of float value*/ 
	public static bool FloatToBool(float _val)
	{
		if (_val < 0)
			return false;
		else
			return true;
	}
	/* Common function to get input for left of right shift press */ 
	public static bool ShiftKeysDown()
	{
		if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift))
			return true;
		else
			return false;
	}
}
