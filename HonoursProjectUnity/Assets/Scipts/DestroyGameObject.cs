using UnityEngine;
using System.Collections;

public class DestroyGameObject : MonoBehaviour 
{

	void DestroyObject()
	{
		Destroy (this.gameObject.transform.parent.gameObject);
	}

}
