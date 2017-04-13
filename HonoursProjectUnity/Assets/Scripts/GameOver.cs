using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour
{

	void OnTriggerEnter(Collider coll)
	{
		if (coll.tag == "Player") 
		{
			Application.LoadLevel (0);
		}
	}
}
