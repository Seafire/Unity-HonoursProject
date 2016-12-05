using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour
{
	public GameObject target;
	public GameObject cloneFrom;
	public int amount;
	public float radius;
	
	void Start()
	{
		Surround (target, cloneFrom, amount, radius);
	}
	
	// this function replicate the object surrounding the target
	public void Surround(GameObject target, GameObject prefab, int amount, float radius)
	{
		float angle = 360f / amount;
		
		for (int i = 0; i < amount; i++)
		{
			GameObject go = Instantiate (prefab) as GameObject;
			
			go.transform.Rotate (Vector3.up, angle*i);
			go.transform.position = target.transform.position - (go.transform.forward * radius);
			// OR  go.GetComponent<NavMeshAgent>().Warp(target.transform.position-(go.transform.forward*radius));
		}
	}
}
