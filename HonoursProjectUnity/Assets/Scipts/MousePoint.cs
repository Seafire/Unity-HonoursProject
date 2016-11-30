using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MousePoint : MonoBehaviour 
{
	RaycastHit hit;

	public GameObject target;

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
}
