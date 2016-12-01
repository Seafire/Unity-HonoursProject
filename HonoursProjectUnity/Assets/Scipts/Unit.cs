using UnityEngine;
using System.Collections;

/*
 * This script should be attached to all units in the game
*/

public class Unit : MonoBehaviour
{
	// For mouse
	public Vector2 screenPos;
	public bool isOnScreen;
	public bool isSelected = false;

	void Update()
	{
		// If the unit is not selected
		if (!isSelected) 
		{
			// Track the screen position
			screenPos = Camera.main.WorldToScreenPoint(this.transform.position);

			// If within the screen space
			if (MousePoint.UnitWithinScreenSpace(screenPos))
			{
				// And not already added to UnitsOnScreen, add it
				if (!isOnScreen)
				{
					MousePoint.unitsOnScreen.Add (this.gameObject);
					isOnScreen = true;
				}
			}
			// Unit is not on Screen space
			else
			{
				// Remove if previously on screen
				if (isOnScreen)
				{
					MousePoint.RemoveFromOnScreenUnits(this.gameObject);
				}
			}
		}
	}
}
