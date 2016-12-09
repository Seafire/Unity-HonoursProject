using UnityEngine;
using System.Collections;

public class WorldCamera : MonoBehaviour 
{
	public LayerMask groundLayer;									/* What the ray will detect as the ground */ 

	/* Limit for camera and mouse */
	public struct BoxLimit
	{
		public float leftLimit;
		public float rightLimit;
		public float topLimit;
		public float bottomLimit;
	}

	public static BoxLimit cameraLimits = new BoxLimit();			/* Used to stop camera going off the map */
	public static BoxLimit mouseScrollLimits = new BoxLimit();		/* Used for the height ajustment */
	public static WorldCamera instance;								/* Refernce to the camera */

	public GameObject mainCamera;									/* Reference to the current instance */
	private GameObject scrollAngle;									/* Used as a local gameobject to allow zooming */

	private float cameraMoveSpeed = 60.0f;							/* Private variable for the camera movement speed */
	private float shiftBonus = 45.0f;								/* Increase to camera movement is shift is pressed */
	private float mouseBoundary = 25.0f;							/* Distance at the edge of the screen to allow the camera to move */

	private float mouseX, mouseY;									/* current mouse position */

	private bool verticalRotationEnabled = true;					/* Option to stop the camera rotating vertical */
	private float verticalRotationMin = 0.0f;	// In degrees		/* Minimum bounds for the camera vertical rotation */
	private float verticalRotationMax = 65.0f;	// In degrees		/* Maximum bounds for the camera vertical rotation */

	public Terrain worldTerrain;									/* Access to the terrain */
	public float worldTerrainPadding = 25.0f;						/* Distance to stop the camera from edge of terrain */

	[HideInInspector]
	public float cameraHeight;										/* Current camera height */
	[HideInInspector]
	public float cameraY;											/* Camera change in y Axis */ 
	public float maxCameraHeight = 80.0f;							/* Max distance the camera can zoom out */
	
	void Awake()
	{
		// Set the instance
		instance = this;
	}

	void Start()
	{
		// Set up camera limits
		cameraLimits.leftLimit = worldTerrain.transform.position.x + worldTerrainPadding;
		cameraLimits.rightLimit = worldTerrain.terrainData.size.x - worldTerrainPadding;
		cameraLimits.topLimit = worldTerrain.transform.position.z + worldTerrainPadding;
		cameraLimits.bottomLimit = worldTerrain.terrainData.size.z - worldTerrainPadding;
		// Set up mouse limits
		mouseScrollLimits.leftLimit = mouseBoundary;
		mouseScrollLimits.rightLimit = mouseBoundary;
		mouseScrollLimits.topLimit = mouseBoundary;
		mouseScrollLimits.bottomLimit = mouseBoundary;
		//Initialize the camera height
		cameraHeight = transform.position.y;
		// Initialize as new empty gameobject
		scrollAngle = new GameObject();
	}

	void LateUpdate()
	{
		// Controls the camera rotation with the mouse
		HandleMouseRotation ();
		// Controls the scrolling of the camera
		ApplyScroll ();
		// If the user has interacted with the scene through input
		if(CheckIfUserCameraInput())
		{
			// Calculate the desired movement
			Vector3 desiredTranlation = GetDesiredTranlation();
			// If the desired move is within the scene limits
			if(!isDesiredPositionOverBoundaries(desiredTranlation))
			{
				// Update the controller position to the new location
				Vector3 desiredPosition = transform.position + desiredTranlation;
				// Update the camera Y position
				UpdateCameraY (desiredPosition);
				// Trandform the camera controller to the new location
				this.transform.Translate(desiredTranlation);
			}
		}
		// Applies the camera scroll
		ApplyCameraY ();
		// Set the current position of the mouse
		mouseX = Input.mousePosition.x;
		mouseY = Input.mousePosition.y;
	}

	void ApplyScroll()
	{
		float deadZone = 0.1f;
		float easeFactor = 20.0f;
		// Set local variable to the input value of scroll wheel
		float scrollWheelValue = Input.GetAxis ("Mouse ScrollWheel") * -easeFactor;

		// Check deadzone
		if (scrollWheelValue > -deadZone && scrollWheelValue < deadZone || scrollWheelValue == 0)
			return;
		// Set value to the current x Angle on the main camera
		float eularAngleX = mainCamera.transform.localEulerAngles.x;

		// Configure the ScreenAngle gameObject
		scrollAngle.transform.position = transform.position;
		scrollAngle.transform.eulerAngles = new Vector3 (eularAngleX, this.transform.eulerAngles.y, this.transform.eulerAngles.z);
		scrollAngle.transform.Translate (Vector3.back * scrollWheelValue);

		// Set local varaible to the newly create gameonject in order to apply it to the camera
		Vector3 desiredScrollPosition = scrollAngle.transform.position;

		// Check if in boundaries
		if (desiredScrollPosition.x < cameraLimits.leftLimit || desiredScrollPosition.x > cameraLimits.rightLimit)
			return;
		if (desiredScrollPosition.z < cameraLimits.topLimit || desiredScrollPosition.z > cameraLimits.bottomLimit)
			return;
		// Check height of camera
		if (desiredScrollPosition.y > maxCameraHeight)
			return;

		if (desiredScrollPosition.y < 20.0f)
			return;

		// Update the camera height and cameraY
		float heightDif = desiredScrollPosition.y - this.transform.position.y;
		cameraHeight += heightDif;
		cameraY = desiredScrollPosition.y;

		// Update the camera Position
		this.transform.position = desiredScrollPosition;

		return;

	}

	// Calculates the camera heigh to the terrain height
	public void UpdateCameraY (Vector3 desiredPosition)
	{
		// Declare local variables
		RaycastHit hit;
		float deadZone = 0.01f;

		// If the Ray collides with anything
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

		// Calculate the movement in the Y direction with a Damp applied
		float newPosY = Mathf.SmoothDamp (transform.position.y, cameraY, ref vel, smoothTime);

		// If the new camera y position is less than the max camera height
		if (newPosY < maxCameraHeight)
			// Update the camera position in the Y axis
			transform.position = new Vector3 (transform.position.x, newPosY, transform.position.z);
		return;
	}

	// Handles the mouse rotation vertically and horizontally
	public void HandleMouseRotation()
	{
		float easeFactor = 10.0f;
		// If the user is holding the right mouse button and the left control button
		if (Input.GetMouseButton (1) && Input.GetKey (KeyCode.LeftControl))
		{
			// Horizontal Rotation - check is there is a change in the X axis of the mouse
			if(Input.mousePosition.x != mouseX)
			{
				// Update the camera horizontal position
				float cameraRotationY = (Input.mousePosition.x - mouseX) * easeFactor * Time.deltaTime;
				this.transform.Rotate(0, cameraRotationY, 0);
			}
			// Vertical Rotation - check is there is a change in the Y axis of the mouse and that vertical rotation is enabled
			if(verticalRotationEnabled && Input.mousePosition.y != mouseY)
			{
				// Get the child of the camera controller - the camera
				GameObject mainCamera = this.gameObject.transform.FindChild("Main Camera").gameObject;
				// Update the camera rotataion calculations
				float cameraRotationX = (mouseY - Input.mousePosition.y) * easeFactor * Time.deltaTime;
				float desiredRotation = mainCamera.transform.eulerAngles.x + cameraRotationX;
				// Check if the rotation if within the range
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
