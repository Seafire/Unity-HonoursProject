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


	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		cameraLimits.leftLimit = 10.0f;
		cameraLimits.rightLimit = 240.0f;
		cameraLimits.topLimit = 204.0f;
		cameraLimits.bottomLimit = -20.0f;

		mouseScrollLimits.leftLimit = mouseBoundary;
		mouseScrollLimits.rightLimit = mouseBoundary;
		mouseScrollLimits.topLimit = mouseBoundary;
		mouseScrollLimits.bottomLimit = mouseBoundary;
	}

	void Update()
	{
		if(CheckIfUserCameraInput())
		{
			Vector3 cameraDesiredMove = GetDesiredTranlation();
			Debug.Log (cameraDesiredMove);
			if(!isDesiredPositionOverBoundaries(cameraDesiredMove))
			{
				this.transform.Translate(cameraDesiredMove);

			}

			//Debug.Log (cameraDesiredMove);
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

		if (Input.GetKey (KeyCode.W))
			desiredZ = moveSpeed;

		if (Input.GetKey (KeyCode.S))
			desiredZ = -moveSpeed;

		if (Input.GetKey (KeyCode.D))
			desiredX = moveSpeed;

		if (Input.GetKey (KeyCode.A))
			desiredX = -moveSpeed;


		if (Input.mousePosition.x < mouseScrollLimits.leftLimit)
			desiredX = -moveSpeed;

		if (Input.mousePosition.x > Screen.width - mouseScrollLimits.rightLimit)
			desiredX = moveSpeed;

		if (Input.mousePosition.y < mouseScrollLimits.bottomLimit)
			desiredZ = -moveSpeed;
		
		if (Input.mousePosition.y > Screen.height - mouseScrollLimits.topLimit)
			desiredZ = moveSpeed;
		return new Vector3 (desiredX, 0.0f, desiredZ);
	}

	public bool isDesiredPositionOverBoundaries(Vector3 DesiredMove)
	{
		bool overBoundaries = false;

		if ((this.transform.position.x + DesiredMove.x) < cameraLimits.leftLimit)
			overBoundaries = true;
		if ((this.transform.position.x + DesiredMove.x) > cameraLimits.rightLimit)
			overBoundaries = true;
		if ((this.transform.position.z + DesiredMove.z) > cameraLimits.topLimit)
			overBoundaries = true;
		if ((this.transform.position.z + DesiredMove.z) < cameraLimits.bottomLimit)
			overBoundaries = true;

		overBoundaries = false;

		Debug.Log (overBoundaries);

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


/*	[System.Serializable]
	public class PositionSettings
	{
		public bool invertPan = true;
		public float panSmooth = 7.0f;
		public float distanceFromGround = 40.0f;
		public bool allowZoom = true;
		public float zoomSmoth = 5.0f;
		public float zoomStep = 5.0f;
		public float maxZoom = 25.0f;
		public float minZoom = 80.0f;

		[HideInInspector]
		public float newDistance = 40.0f;

	}

	[System.Serializable]
	public class OrbitSetting
	{
		public float xRotation = 50.0f;
		public float yRotation = 0.0f;
		public bool allowYOrbit = true;
		public float yOrbitSmooth = 10f;
	}

	[System.Serializable]
	public class InputSetting
	{
		public string PAN = "MousePan";
		public string ORBITY = "MouseTurn";
		public string ZOOM = "Mouse ScrollWheel";
	}

	public PositionSettings position = new PositionSettings();
	public OrbitSetting orbit = new OrbitSetting();
	public InputSetting input = new InputSetting();

	Vector3 destination = Vector3.zero;
	Vector3 camVel = Vector3.zero;
	Vector3 previousMousePos = Vector3.zero;
	Vector3 currentMousePos = Vector3.zero;

	float panInput, orbitInput, zoomInput;
	int panDirection = 0;

	// Use this for initialization
	void Start ()
	{
		panInput = 0.0f;
		orbitInput = 0.0f;
		zoomInput = 0.0f;
	}

	void GetInput()
	{
		// Resposable for setting input variables
		panInput = Input.GetAxis (input.PAN);
		orbitInput = Input.GetAxis (input.ORBITY);
		zoomInput = Input.GetAxis (input.ZOOM);

		previousMousePos = currentMousePos;
		currentMousePos = Input.mousePosition;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Update input
		GetInput ();
		// Zooming
		if (position.allowZoom) 
		{
			Zoom ();
		}
		// Rotating
		if (orbit.allowYOrbit) 
		{
			Rotate();
		}
		// Panning
		PanWorld ();
	}

	void FixedUpdate()
	{
		HandleCameraDistance ();
	}

	void PanWorld()
	{
		Vector3 targetPos = transform.position;

		if (position.invertPan) 
		{
			panDirection = -1;
		}
		else
		{
			panDirection = 1;
		}

		if (panInput > 0)
		{
			targetPos += transform.right * (currentMousePos.x - previousMousePos.x) * position.panSmooth * panDirection * Time.deltaTime;
			targetPos += Vector3.Cross(transform.right, Vector3.up) * (currentMousePos.y - previousMousePos.y) * position.panSmooth * panDirection * Time.deltaTime;
		}
		// Add Lerp if wanted
		//transform.position = targetPos;
		transform.position = Vector3.Lerp (transform.position , targetPos, position.zoomSmoth * Time.deltaTime);
	}

	void HandleCameraDistance()
	{
		Ray ray = new Ray (transform.position, transform.forward);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 100, groundLayer))
		{
			destination = Vector3.Normalize(transform.position - hit.point) * position.distanceFromGround;
			destination += hit.point;

			transform.position = Vector3.SmoothDamp(transform.position, destination, ref camVel, 0.3f);
		}
	}

	void Zoom()
	{
		position.newDistance += position.zoomStep * -zoomInput;

		position.distanceFromGround = Mathf.Lerp (position.distanceFromGround, position.newDistance, position.zoomSmoth * Time.deltaTime);

		if (position.distanceFromGround < position.maxZoom)
		{
			position.distanceFromGround = position.maxZoom;
			position.newDistance = position.maxZoom;
		}
		if (position.distanceFromGround > position.minZoom)
		{
			position.distanceFromGround = position.minZoom;
			position.newDistance = position.minZoom;
		}
	}

	void Rotate()
	{
		if (orbitInput > 0)
		{
			orbit.yRotation += (currentMousePos.x - previousMousePos.x) * orbit.yOrbitSmooth * Time.deltaTime;
		}

		transform.rotation = Quaternion.Euler (orbit.xRotation, orbit.yRotation, 0);
	}*/
}
