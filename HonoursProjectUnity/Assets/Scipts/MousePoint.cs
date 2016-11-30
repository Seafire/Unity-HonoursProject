using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MousePoint : MonoBehaviour 
{
	RaycastHit hit;

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		GameObject target = GameObject.Find ("Sphere");
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		if (Physics.Raycast(ray, out hit, 1000))
		{
			Debug.Log (hit.collider.name);
			if (hit.collider.name == "Plane")
			{
				target.transform.position = hit.point;
			}
		}

		Debug.DrawRay (ray.origin, ray.direction * 1000 , Color.red);
	}
}
