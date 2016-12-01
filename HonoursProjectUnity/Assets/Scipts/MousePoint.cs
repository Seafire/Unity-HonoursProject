using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MousePoint : MonoBehaviour 
{
	RaycastHit hit;

	public static ArrayList currentlySelectedUnits = new ArrayList(); // Of GameObject

	public static ArrayList unitsOnScreen = new ArrayList (); // Of GameObject
	public static ArrayList unitsInDrag = new ArrayList (); // Of GameObject
	private bool finishDragOnFrame;


	public GUIStyle mouseDragSkin;

	public Camera camera_;
	
	private Vector3 mouseDownPoint;
	private Vector3 mouseUpPoint;
	private Vector3 mouseCurPoint;

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
	private Vector2 boxStart;
	private Vector2 boxFinish;

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
					Debug.Log("User is dragging");
				}
			}
			else if (Input.GetMouseButtonUp (0))
			{
				//if(isUserDragging)
				//	Debug.Log("User is dragging");
				
				timeLeft = 0.0f;
				isUserDragging = false;
				
				Debug.Log (isUserDragging);
			}

			// Mouse click
			if (!isUserDragging)
			{

				if (hit.collider.name == "Plane")
				{
					if (Input.GetMouseButtonDown(1))
					{
						GameObject targetObj = Instantiate(target, hit.point, Quaternion.identity) as GameObject;
						targetObj.name = "target";
					}
					else if(Input.GetMouseButtonDown(0) && DidUserLeftClick(mouseDownPoint))
					{
						if(!ShiftKeysDown())
							DeselectGameObjects();
					}
				}
				else
				{
					if(Input.GetMouseButtonDown(0) && DidUserLeftClick(mouseDownPoint))
					{
						if(hit.collider.transform.FindChild("Selected"))
						{
							if(!UnitAlreadySelected(hit.collider.gameObject))
							{
								// If shif key is not down
								if(!ShiftKeysDown())
								{
									DeselectGameObjects();
								}
								// add unit to currently selected unit
								currentlySelectedUnits.Add(hit.collider.gameObject);
								GameObject selectObj = hit.collider.transform.FindChild("Selected").gameObject;
								selectObj.SetActive(true);
							
							}
							else
							{
								if(ShiftKeysDown())
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
							if(!ShiftKeysDown())
								DeselectGameObjects();
					}
				}
			}
			else
			{
				if(Input.GetMouseButtonDown(0) && DidUserLeftClick(mouseDownPoint))
				{
					if(!ShiftKeysDown())
						DeselectGameObjects();
				}
			}
		}
		else
		{

		}

		Debug.DrawRay (ray.origin, ray.direction * 1000 , Color.red);


		if (isUserDragging) 
		{
			// GUI Variables
			boxWidth = camera_.WorldToScreenPoint(mouseDownPoint).x - camera_.WorldToScreenPoint(mouseCurPoint).x;
			boxHeight = camera_.WorldToScreenPoint(mouseDownPoint).y - camera_.WorldToScreenPoint(mouseCurPoint).y;
			boxLeft = Input.mousePosition.x;
			boxTop = (Screen.height - Input.mousePosition.y) - boxHeight;

			if(FloatToBool(boxWidth))
				if(FloatToBool(boxHeight))
					boxStart = new Vector2(Input.mousePosition.x, Input.mousePosition.y + boxHeight);
			else if(!FloatToBool(boxHeight))
				boxStart = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			else if(!FloatToBool(boxWidth)) 
				if(FloatToBool(boxHeight))
					boxStart = new Vector2(Input.mousePosition.x + boxWidth, Input.mousePosition.y + boxHeight);
			else if (!FloatToBool(boxHeight))
				boxStart = new Vector2(Input.mousePosition.x + boxWidth, Input.mousePosition.y);

			boxFinish = new Vector2 (boxStart.x + Unsigned(boxWidth), boxStart.y - Unsigned (boxHeight));

			Debug.Log (boxStart + ", " + boxFinish);
		}
	}



	void LateUpdate()
	{
		Debug.Log (unitsOnScreen.Count);
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


	public static bool FloatToBool(float val)
	{
		if (val < 0)
			return false;
		else
			return true;
	}

	void OnGUI()
	{
		if (isUserDragging)
		{
			GUI.Box (new Rect (boxLeft, boxTop, boxWidth, boxHeight), "", mouseDragSkin);
		}
	}

	public static float Unsigned(float val)
	{
		if (val < 0)
		{
			val *= -1;
		}

		return val;
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

	public static bool ShiftKeysDown()
	{
		if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift))
			return true;
		else
			return false;
	}

}
