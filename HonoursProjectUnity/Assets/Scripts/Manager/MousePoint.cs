using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MousePoint : MonoBehaviour 
{
	RaycastHit hit;

	public static Vector3 RightMouseClick;
	public static ArrayList currentlySelectedUnits = new ArrayList(); // Of GameObject
	public static ArrayList unitsOnScreen = new ArrayList (); // Of GameObject
	public static ArrayList unitsInDrag = new ArrayList (); // Of GameObject
	private bool finishDragOnFrame;
	private bool startedDrag;

	public GUIStyle mouseDragSkin;

	public Camera camera_;
	
	private Vector3 mouseDownPoint;
	private Vector3 mouseCurPoint;
	public Vector3 rightClickPoint;

	public static bool isUserDragging;
	private static float timeLimit = 1.0f;
	private static float timeLeft;
	private static Vector2 mouseDragStart;
	private static float clickDragZone = 1.3f;

	// GUI
	private float boxWidth;
	private float boxHeight;
	private float boxLeft;
	private float boxTop;
	private static Vector2 boxStart;
	private static Vector2 boxFinish;

	public GameObject target;

	void Awake()
	{
		mouseDownPoint = Vector3.zero;
	}

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		var layerMask = ~((1 << 12) | (1 << 10) | (1 << 9));

		if (Physics.Raycast(ray, out hit, 1000))
		{

			mouseCurPoint = hit.point;
			// Mouse Drag
			if (Input.GetMouseButtonDown (0)) 
			{
				mouseDownPoint = hit.point;
			}
			if (Input.GetMouseButtonDown (0)) 
			{
				timeLeft = timeLimit;
				mouseDragStart = Input.mousePosition;
				startedDrag = true;
			}
			else if (Input.GetMouseButton (0))
			{
				// If the user is not dragging
				if (!isUserDragging)
				{
					timeLeft -= Time.deltaTime;
					if(timeLeft <= 0 || UserDragging(mouseDragStart, Input.mousePosition))
					{
						// If true the user is dragging
						isUserDragging = true;
					}
				}
				if(isUserDragging)
				{

				}
			}
			else if (Input.GetMouseButtonUp (0))
			{
				if(isUserDragging)
					finishDragOnFrame = true;

				isUserDragging = false;
			}

			// Mouse click
			if (!isUserDragging)
			{

				if (hit.collider.name == "Plane")
				{
					if (Input.GetMouseButtonDown(1) && !Input.GetKey(KeyCode.LeftControl))
					{
						GameObject targetObj = Instantiate(target, hit.point, Quaternion.identity) as GameObject;
						targetObj.name = "target";
						RightMouseClick = hit.point;
					}
					else if(Input.GetMouseButtonUp(0) && DidUserLeftClick(mouseDownPoint))
					{
						if(!Common.ShiftKeysDown())
							DeselectGameObjects();
					}
				}
				else
				{
					if(Input.GetMouseButtonUp(0) && DidUserLeftClick(mouseDownPoint))
					{
						if(hit.collider.gameObject.GetComponent<Unit>())
						{
							if(!UnitAlreadySelected(hit.collider.gameObject))
							{
								// If shif key is not down
								if(!Common.ShiftKeysDown())
								{
									DeselectGameObjects();
								}
								// add unit to currently selected unit
								currentlySelectedUnits.Add(hit.collider.gameObject);
								GameObject selectObj = hit.collider.transform.FindChild("Selected").gameObject;
								selectObj.SetActive(true);

								// Change the unit Selected value to true
								hit.collider.gameObject.GetComponent<Unit>().isSelected  =true;
							
							}
							else
							{
								if(Common.ShiftKeysDown())
								{
									RemoveUnitFromArray(hit.collider.gameObject);
									GameObject selectObj = hit.collider.transform.FindChild("Selected").gameObject;
									selectObj.SetActive(false);
								}
								else
								{
									DeselectGameObjects();
									GameObject selectObj = hit.collider.transform.FindChild("Selected").gameObject;
									selectObj.SetActive(true);
								}
							}
						}
						else
							if(!Common.ShiftKeysDown())
								DeselectGameObjects();
					}
				}
			}
			else
			{
				if(Input.GetMouseButtonUp(0) && DidUserLeftClick(mouseDownPoint))
				{
					if(!Common.ShiftKeysDown())
						DeselectGameObjects();
				}
			}
		}
		else
		{
			if (!Common.ShiftKeysDown() && startedDrag && isUserDragging)
			{
				DeselectGameObjects();
				startedDrag = false;
			}
		}

		Debug.DrawRay (ray.origin, ray.direction * 1000 , Color.red);


		if (isUserDragging) 
		{
			// GUI Variables
			boxWidth = Camera.main.WorldToScreenPoint(mouseDownPoint).x - Camera.main.WorldToScreenPoint(mouseCurPoint).x;
			boxHeight = Camera.main.WorldToScreenPoint(mouseDownPoint).y - Camera.main.WorldToScreenPoint(mouseCurPoint).y;
			boxLeft = Input.mousePosition.x;
			boxTop = (Screen.height - Input.mousePosition.y) - boxHeight;

			if(Common.FloatToBool(boxWidth))
				if(Common.FloatToBool(boxHeight))
					boxStart = new Vector2(Input.mousePosition.x, Input.mousePosition.y + boxHeight);
			else if(!Common.FloatToBool(boxHeight))
				boxStart = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			else if(!Common.FloatToBool(boxWidth)) 
				if(Common.FloatToBool(boxHeight))
					boxStart = new Vector2(Input.mousePosition.x + boxWidth, Input.mousePosition.y + boxHeight);
			else if (!Common.FloatToBool(boxHeight))
				boxStart = new Vector2(Input.mousePosition.x + boxWidth, Input.mousePosition.y);

			boxFinish = new Vector2 (boxStart.x + Common.Unsigned(boxWidth), boxStart.y - Common.Unsigned (boxHeight));

			Debug.Log (boxStart + ", " + boxFinish);
		}
	}



	void LateUpdate()
	{
		unitsInDrag.Clear ();
		// If user is dragging or finished on this frame and there are units to select on the screen
		if ((isUserDragging || finishDragOnFrame) && unitsOnScreen.Count > 0)
		{
			// Loop through those units on screen
			for (int i = 0; i < unitsOnScreen.Count; i++)
			{
				GameObject unitObj = unitsOnScreen[i] as GameObject;
				Unit unitScript = unitObj.GetComponent<Unit>();
				GameObject selectedObj = unitObj.transform.FindChild("Selected").gameObject;

				// If not already in dragged units
				if (!UnitAlreadyInDraggedUnits(unitObj))
				{
					if(UnitInsideDrag(unitScript.screenPos))
					{
						selectedObj.SetActive(true);
						unitsInDrag.Add (unitObj);
						Debug.Log ("Should be selecting");
					}
					// Unit is not in drag
					else
					{
						// Remove the selected graphic id unit is not already selected
						if (!UnitAlreadySelected(unitObj))
						{
							selectedObj.SetActive(false);
						}
					}
				}
			}
		}

		if (finishDragOnFrame)
		{
			finishDragOnFrame = false;
			PutDraggedUnitsInCurrentlySelectedUnits();
		}
	}








	// Check if units are within screen space
	public static bool UnitWithinScreenSpace(Vector2 unitScreenPos)
	{
		if ((unitScreenPos.x < Screen.width && unitScreenPos.y < Screen.height) && (unitScreenPos.x > 0.0f && unitScreenPos.y > 0.0f))
			return true;
		else
			return false;
	}

	// Remove a unit from screen space
	public static void RemoveFromOnScreenUnits(GameObject unit)
	{
		for (int i = 0; i < unitsOnScreen.Count; i++) 
		{
			GameObject unitObj = unitsOnScreen[i] as GameObject;
			if(unit == unitObj)
			{
				unitsOnScreen.RemoveAt(i);
				unitObj.GetComponent<Unit>().isOnScreen = false;
				return;
			}
		}

		return;
	}


	void OnGUI()
	{
		if (isUserDragging)
			GUI.Box (new Rect (boxLeft, boxTop, boxWidth, boxHeight), "", mouseDragSkin);
	}

	public bool UserDragging(Vector2 startPoint, Vector2 newPoint)
	{
		if(newPoint.x > startPoint.x + clickDragZone || newPoint.x < startPoint.x - clickDragZone ||
		   newPoint.y > startPoint.y + clickDragZone || newPoint.y < startPoint.y - clickDragZone)
		{
			return true;
		}
		else
			return false;
	}

	public bool DidUserLeftClick(Vector3 hitPoint)
	{
		if (
			(mouseDownPoint.x < hitPoint.x + clickDragZone && mouseDownPoint.x > hitPoint.x - clickDragZone) &&
			(mouseDownPoint.y < hitPoint.y + clickDragZone && mouseDownPoint.y > hitPoint.y - clickDragZone) &&
			(mouseDownPoint.z < hitPoint.z + clickDragZone && mouseDownPoint.z > hitPoint.z - clickDragZone)
			)
			return true;
		else
			return false;

	}

	public static void DeselectGameObjects()
	{
		if (currentlySelectedUnits.Count > 0)
		{
			for (int i = 0; i < currentlySelectedUnits.Count; i++)
			{
				GameObject ArrayListUnits = currentlySelectedUnits[i] as GameObject;
				ArrayListUnits.transform.FindChild("Selected").gameObject.SetActive(false);
				ArrayListUnits.GetComponent<Unit>().isSelected = false;
			}
			currentlySelectedUnits.Clear();
		}
	}

	// Check if a unit is already in the currently selected unit array
	public static bool UnitAlreadySelected(GameObject unit)
	{
		if (currentlySelectedUnits.Count > 0)
		{
			for (int i = 0; i < currentlySelectedUnits.Count; i++)
			{
				GameObject ArrayListUnits = currentlySelectedUnits[i] as GameObject;
				if (ArrayListUnits == unit)
				{
					return true;
				}

				return false;
			}
		}
		return false;
	}

	public void RemoveUnitFromArray(GameObject unit)
	{
		if (currentlySelectedUnits.Count > 0)
		{
			for (int i = 0; i < currentlySelectedUnits.Count; i++)
			{
				GameObject ArrayListUnits = currentlySelectedUnits[i] as GameObject;
				if (ArrayListUnits == unit)
				{
					currentlySelectedUnits.RemoveAt(i);
					ArrayListUnits.transform.FindChild("Selected").gameObject.SetActive(false);
				}
				return;
			}
		}
		else
			return;
	}
	
	// IsUnitInside Drag
	public static bool UnitInsideDrag(Vector2 unitScreenPos)
	{
		if (
			(unitScreenPos.x > boxStart.x && unitScreenPos.y < boxStart.y) && 
		    (unitScreenPos.x < boxFinish.x && unitScreenPos.y > boxFinish.y)
		    )
		{
			return true;
		} 
		else
			return false;
	}

	// Check if unit is already in unit in drag list
	public static bool UnitAlreadyInDraggedUnits(GameObject unit)
	{
		if (unitsInDrag.Count > 0)
		{
			for (int i = 0; i < unitsInDrag.Count; i++) 
			{
				GameObject ArrayListUnits = unitsInDrag [i] as GameObject;
				if (ArrayListUnits == unit) 
				{
					return true;
				}
				
				return false;
			}
		}
		return false;
	}

	// Take all units from UnitsInDrag, into currently selected units
	public static void PutDraggedUnitsInCurrentlySelectedUnits()
	{
		if (unitsInDrag.Count > 0) 
		{
			for (int i = 0; i < unitsInDrag.Count; i++)
			{
				GameObject unitObj = unitsInDrag[i] as GameObject;

				// If the unit is not already in CurrentlySelectedUnits, add it!
				if (!UnitAlreadySelected(unitObj))
				{
					currentlySelectedUnits.Add (unitObj);
					unitObj.GetComponent<Unit>().isSelected = true;
				}
			}
			unitsInDrag.Clear();
		}
	}
}
