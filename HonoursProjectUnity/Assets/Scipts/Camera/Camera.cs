using UnityEngine;
using System.Collections;

public class Camera : MonoBehaviour 
{
	public LayerMask groundLayer;

	[System.Serializable]
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
		public float yPotation = 0.0f;
		public bool allowYOrbit = true;
		public float yOrbitSmooth = 0.5f;
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
	public InputSetting Input = new InputSetting();

	Vector3 destination = Vector3.zero;
	Vector3 camVel = Vector3.zero;
	Vector3 previousMousePos = Vector3.zero;
	Vector3 currentMousePos = Vector3.zero;

	float panInput, orbitInput, zoomInput;
	int panDirection = 0;

	// Use this for initialization
	void Start ()
	{
	
	}

	void GetInput()
	{
		// Resposable for setting input variables
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Update input
		// Zooming
		// Rotating
		// Panning
	}

	void FixedUpdate()
	{
		// Handle camera distance
	}

	void PanWorld()
	{

	}

	void HandleCameraDistance()
	{

	}

	void Zoom()
	{

	}

	void Rotate()
	{

	}
}
