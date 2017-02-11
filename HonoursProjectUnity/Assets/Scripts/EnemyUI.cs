using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{

	public bool show = true;
	public GameObject enemyPrefabUI;
	private GameObject enemyUI;
	private Text textUI;
	private Text morale;
	private Text suppresion;
	private EnemyAI enemyAI;

	// Use this for initialization
	void Start ()
	{
		enemyAI = GetComponent <EnemyAI> ();
		enemyUI = Instantiate (enemyPrefabUI, transform.position, Quaternion.identity) as GameObject;
		enemyUI.transform.SetParent (GameObject.FindGameObjectWithTag ("Canvas").transform);
		//textUI = enemyUI.GetComponentInChildren<Text> ();

		Text[] texts = enemyUI.GetComponentsInChildren<Text> ();

		textUI = texts [0];
		morale = texts [1];
		suppresion = texts [2];
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (show) 
		{
			enemyUI.gameObject.SetActive (true);
			string info = enemyAI.stateAI.ToString ();
			textUI.text = info;
			morale.text = "morale: " + enemyAI.charStats.morale.ToString ();
			suppresion.text = "Suppresion: " + enemyAI.charStats.suppresionLevel.ToString ();

			Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
			enemyUI.transform.position = screenPoint;
		}
		else
		{
			enemyUI.gameObject.SetActive (false);
		}
	}

	public void IsEnabledUI ()
	{
		show = !show;
	}
}
