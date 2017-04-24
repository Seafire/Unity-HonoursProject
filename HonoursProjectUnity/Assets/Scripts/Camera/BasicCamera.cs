using UnityEngine;
using System.Collections;

public class BasicCamera : MonoBehaviour
{
	
	private float cameraMoveSpeed = 60.0f;							/* Private variable for the camera movement speed */


	public float panSpeed = 40f;
	public float panBorderThickness = 10f;
	public Vector2 panLimit;

	public float scrollSpeed = 20f;
	public float minY = 20f;
	public float maxY = 120f;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Vector3 desiredTranlation = GetDesiredTranlation();

	//	this.transform.Translate(desiredTranlation);

		Vector3 pos = transform.position;

		if (Input.GetKey (KeyCode.W) || Input.mousePosition.y > (Screen.height - panBorderThickness))
			pos.z += panSpeed * Time.deltaTime;
		
		if (Input.GetKey (KeyCode.S) || Input.mousePosition.y < panBorderThickness)
			pos.z -= panSpeed * Time.deltaTime;
		
		if (Input.GetKey (KeyCode.D) || Input.mousePosition.x > (Screen.width - panBorderThickness))
			pos.x += panSpeed * Time.deltaTime;
		
		if (Input.GetKey (KeyCode.A) || Input.mousePosition.x < panBorderThickness)
			pos.x -= panSpeed * Time.deltaTime;

		float scroll = Input.GetAxis ("Mouse ScrollWheel");
		pos.y -= scroll * 300f * Time.deltaTime;

		//pos.x = Mathf.Clamp (pos.x, -panLimit.x, panLimit.x);
		pos.y = Mathf.Clamp (pos.y, minY, maxY);
		//pos.z = Mathf.Clamp (pos.z, -panLimit.y, panLimit.y);

		transform.position = pos;


	}

	/*public Vector3 GetDesiredTranlation()
	{
		float moveSpeed = 0.0f;
		float desiredX = 0.0f;
		float desiredZ = 0.0f;

		Vector3 pos = transform.position;
		
		if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift))
			moveSpeed = (cameraMoveSpeed) * Time.deltaTime;
		else
			moveSpeed = cameraMoveSpeed * Time.deltaTime;
		

		
		return new Vector3 (desiredX, 0.0f, desiredZ);
	}*/

}
