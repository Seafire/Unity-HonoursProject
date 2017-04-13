using UnityEngine;
using System.Collections;

public class BasicCamera : MonoBehaviour
{
	
	private float cameraMoveSpeed = 60.0f;							/* Private variable for the camera movement speed */


	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 desiredTranlation = GetDesiredTranlation();

		this.transform.Translate(desiredTranlation);
	}

	public Vector3 GetDesiredTranlation()
	{
		float moveSpeed = 0.0f;
		float desiredX = 0.0f;
		float desiredZ = 0.0f;
		
		if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift))
			moveSpeed = (cameraMoveSpeed) * Time.deltaTime;
		else
			moveSpeed = cameraMoveSpeed * Time.deltaTime;
		
		if (Input.GetKey (KeyCode.W) /*|| Input.mousePosition.y > (Screen.height - mouseScrollLimits.topLimit)*/)
			desiredZ = moveSpeed;
		
		if (Input.GetKey (KeyCode.S) /*|| Input.mousePosition.y < mouseScrollLimits.bottomLimit*/)
			desiredZ = -moveSpeed;
		
		if (Input.GetKey (KeyCode.D) /*|| Input.mousePosition.x > (Screen.width - mouseScrollLimits.rightLimit)*/)
			desiredX = moveSpeed;
		
		if (Input.GetKey (KeyCode.A) /*|| Input.mousePosition.x < mouseScrollLimits.leftLimit*/)
			desiredX = -moveSpeed;
		
		return new Vector3 (desiredX, 0.0f, desiredZ);
	}

}
