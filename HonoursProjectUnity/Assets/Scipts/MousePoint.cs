using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MousePoint : MonoBehaviour 
{
	RaycastHit hit;

	public static GameObject currentlySelectedUnit;
	private Vector3 mouseDownPoint;

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

			// Store point of mouse click
			if (Input.GetMouseButtonDown(0))
			{
				mouseDownPoint = hit.point;
			}
			Debug.Log (hit.collider.name);
			if (hit.collider.name == "Plane")
			{
				if (Input.GetMouseButtonDown(1))
				{
					GameObject targetObj = Instantiate(target, hit.point, Quaternion.identity) as GameObject;
					targetObj.name = "target";
				}
			}
		}
		Debug.DrawRay (ray.origin, ray.direction * 1000 , Color.red);
	}

	public bool DidUserLeftClick(Vector3 hitPoint)
	{
		float clickZone = 1.3f;

		if (
			(mouseDownPoint.x < hitPoint.x + clickZone && mouseDownPoint.x > hitPoint.x - clickZone) &&
			(mouseDownPoint.y < hitPoint.y + clickZone && mouseDownPoint.y > hitPoint.y - clickZone) &&
			(mouseDownPoint.z < hitPoint.z + clickZone && mouseDownPoint.z > hitPoint.z - clickZone)
			)
			return true;
		else
			return false;

	}

	public static void DeselectGameObject()
	{
		if (currentlySelectedUnit != null)
		{
			currentlySelectedUnit.transform.FindChild("Selected").gameObject.SetActive(false);
			currentlySelectedUnit = null;
		}
	}

}
