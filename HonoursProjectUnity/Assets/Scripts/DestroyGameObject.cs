using UnityEngine;
using System.Collections;

public class DestroyGameObject : MonoBehaviour 
{
	/* Standard function to destroy a current instance of game object */
	void DestroyObject()
	{
		// Destroys this current instance of game object
		Destroy (this.gameObject.transform.parent.gameObject);
	}

}
