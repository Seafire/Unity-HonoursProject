using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{

	public bool show = true;
	public GameObject enemyPrefabUI;
	private GameObject enemyUI;
	private Text textUI;
	private EnemyAI enemyAI;

	// Use this for initialization
	void Start ()
	{
		enemyAI = GetComponent <EnemyAI> ();
		enemyUI = Instantiate (enemyPrefabUI, transform.position, Quaternion.identity) as GameObject;
		enemyUI.transform.SetParent (GameObject.FindGameObjectWithTag ("Canvas").transform);
		textUI = enemyUI.GetComponentInChildren<Text> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (show) 
		{
			enemyUI.gameObject.SetActive (true);
			string info = enemyAI.stateAI.ToString ();
			textUI.text = info;

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
