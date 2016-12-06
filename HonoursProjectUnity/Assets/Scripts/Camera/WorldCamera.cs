using UnityEngine;
using System.Collections;

public class WorldCamera : MonoBehaviour 
{
	public LayerMask groundLayer;

	public struct BoxLimit
	{
		public float leftLimit;
		public float rightLimit;
		public float topLimit;
		public float bottomLimit;
	}

	public static BoxLimit cameraLimits = new BoxLimit();
	public static BoxLimit mouseScrollLimits = new BoxLimit();
	public static WorldCamera instance;

	private float cameraMoveSpeed = 60.0f;
	private float shiftBonus = 45.0f;
	private float mouseBoundary = 25.0f;

	private float mouseX, mouseY;

	private bool verticalRotationEnabled = true;
	private float verticalRotationMin = 0.0f;	// In degrees
	private float verticalRotationMax = 65.0f;	// In degrees

	public Terrain worldTerrain;
	public float worldTerrainPadding = 25.0f;

	[HideInInspector]
	public float cameraHeight;
	[HideInInspector]
	public float cameraY;
	public float maxCameraHeight = 80.0f;



	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		cameraLimits.leftLimit = worldTerrain.transform.position.x + worldTerrainPadding;
		cameraLimits.rightLimit = worldTerrain.terrainData.size.x - worldTerrainPadding;
		cameraLimits.topLimit = worldTerrain.transform.position.z + worldTerrainPadding;
		cameraLimits.bottomLimit = worldTerrain.terrainData.size.z - worldTerrainPadding;

		mouseScrollLimits.leftLimit = mouseBoundary;
		mouseScrollLimits.rightLimit = mouseBoundary;
		mouseScrollLimits.topLimit = mouseBoundary;
		mouseScrollLimits.bottomLimit = mouseBoundary;

		cameraHeight = transform.position.y;
	}

	void LateUpdate()
	{
		HandleMouseRotation ();

		if(CheckIfUserCameraInput())
		{
			Vector3 desiredTranlation = GetDesiredTranlation();
			if(!isDesiredPositionOverBoundaries(desiredTranlation))
			{
				Vector3 desiredPosition = transform.position + desiredTranlation;

				UpdateCameraY (desiredPosition);

				this.transform.Translate(desiredTranlation);
			}
		}

		ApplyCameraY ();

		mouseX = Input.mousePosition.x;
		mouseY = Input.mousePosition.y;
	}

	// Calculates the camera heigh to the terrain height
	public void UpdateCameraY (Vector3 desiredPosition)
	{
		RaycastHit hit;
		float deadZone = 0.01f;

		if (Physics.Raycast(desiredPosition, Vector3.down, out hit, Mathf.Infinity))
		{
			float newHeight = cameraHeight + hit.point.y;
			float heightDif = newHeight - cameraY;


			if (heightDif > -deadZone && heightDif < deadZone)
				return;
			if (newHeight > maxCameraHeight)
				return;

			cameraY = newHeight;
		}
		return;
	}

	// Apply the camera Y to smooth damp
	public void ApplyCameraY ()
	{
		if (cameraY == transform.position.y || cameraY == 0) 
		{
			return;
		}

		// Smooth damp
		float smoothTime = 0.2f;
		float vel = 0.0f;

		float newPosY = Mathf.SmoothDamp (transform.position.y, cameraY, ref vel, smoothTime);

		if (newPosY < maxCameraHeight)
		{
			Debug.Log ("Update the camera height now");
			transform.position = new Vector3 (transform.position.x, newPosY, transform.position.z);
		}

		return;
	}

	// Handles the mouse rotation vertically and horizontally
	public void HandleMouseRotation()
	{
		float easeFactor = 10.0f;
		if (Input.GetMouseButton (1) && Input.GetKey (KeyCode.LeftControl))
		{
			// Horizontal Rotation
			if(Input.mousePosition.x != mouseX)
			{
				float cameraRotationY = (Input.mousePosition.x - mouseX) * easeFactor * Time.deltaTime;
				this.transform.Rotate(0, cameraRotationY, 0);
			}

			// Vertical Rotation
			if(verticalRotationEnabled && Input.mousePosition.y != mouseY)
			{
				GameObject mainCamera = this.gameObject.transform.FindChild("Main Camera").gameObject;

				float cameraRotationX = (mouseY - Input.mousePosition.y) * easeFactor * Time.deltaTime;
				float desiredRotation = mainCamera.transform.eulerAngles.x + cameraRotationX;

				if (desiredRotation >= verticalRotationMin && desiredRotation <= verticalRotationMax)
				{
					// Affected the main camera to avoid the camera arking when rotating
					mainCamera.transform.Rotate(cameraRotationX, 0, 0);
				}
			}
		}

	}

	public bool CheckIfUserCameraInput()
	{
		bool keyboardMove;
		bool mouseMove;
		bool canMove;

		if (WorldCamera.AreCameraKeyboardButonsPressed ())
			keyboardMove = true;
		else
			keyboardMove = false;

		if (WorldCamera.IsMousePositionWithinBoundaires ())
			mouseMove = true;
		else
			mouseMove = false;

		if (keyboardMove || mouseMove)
			canMove = true;
		else
			canMove = false;

		return canMove;

	}

	public Vector3 GetDesiredTranlation()
	{
		float moveSpeed = 0.0f;
		float desiredX = 0.0f;
		float desiredZ = 0.0f;

		if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift))
			moveSpeed = (cameraMoveSpeed + shiftBonus) * Time.deltaTime;
		else
			moveSpeed = cameraMoveSpeed * Time.deltaTime;

		if (Input.GetKey (KeyCode.W) || Input.mousePosition.y > (Screen.height - mouseScrollLimits.topLimit))
			desiredZ = moveSpeed;

		if (Input.GetKey (KeyCode.S) || Input.mousePosition.y < mouseScrollLimits.bottomLimit)
			desiredZ = -moveSpeed;

		if (Input.GetKey (KeyCode.D) || Input.mousePosition.x > (Screen.width - mouseScrollLimits.rightLimit))
			desiredX = moveSpeed;

		if (Input.GetKey (KeyCode.A) || Input.mousePosition.x < mouseScrollLimits.leftLimit)
			desiredX = -moveSpeed;

		return new Vector3 (desiredX, 0.0f, desiredZ);
	}

	public bool isDesiredPositionOverBoundaries(Vector3 DesiredMove)
	{
		Vector3 desiredWorldPos = this.transform.TransformPoint (DesiredMove);

		bool overBoundaries = false;

		if ((desiredWorldPos.x) < cameraLimits.leftLimit)
			overBoundaries = true;
		if ((desiredWorldPos.x) > cameraLimits.rightLimit)
			overBoundaries = true;
		if ((desiredWorldPos.z) > cameraLimits.topLimit)
			overBoundaries = true;
		if ((desiredWorldPos.z) < cameraLimits.bottomLimit)
			overBoundaries = true;

		overBoundaries = false;

		return overBoundaries;
	}

	public static bool AreCameraKeyboardButonsPressed()
	{
		if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.D))
			return true;
		else
			return false;
	}

	public static bool IsMousePositionWithinBoundaires()
	{
		if (
			(Input.mousePosition.x < mouseScrollLimits.leftLimit && Input.mousePosition.x > -5) ||
			(Input.mousePosition.x > Screen.width - mouseScrollLimits.rightLimit && Input.mousePosition.x < (Screen.width + 5)) ||
			(Input.mousePosition.y < mouseScrollLimits.bottomLimit && Input.mousePosition.y > -5) ||
			(Input.mousePosition.y > Screen.height - mouseScrollLimits.topLimit && Input.mousePosition.y < (Screen.height + 5)))
			return true;
		else
			return false;
	}
}
