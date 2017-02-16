using UnityEngine;
using System.Collections;

public class POI_Base : MonoBehaviour 
{
	public TypePOI typePOI;

	public enum TypePOI
	{
		deadBody,
		other
	}
}
