using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
	public List<Transform> exitPositions = new List<Transform> ();
	public List<RetreatActionBase> retreatPosition = new List<RetreatActionBase> ();

	public static LevelManager instance;

	public static LevelManager GetInstance ()
	{
		return instance;
	}

	// Use this for initialization
	void Awake ()
	{
		instance = this;
	}

	public RetreatActionBase ReturnRetreatDestination (Transform _transform)
	{
		RetreatActionBase retVal = null;

		for (int i = 0; i < retreatPosition.Count; i++)
		{
			if (_transform == retreatPosition[i].retreatPosition)
			{
				retVal = retreatPosition[i];

				break;
			}
		}

		return retVal;
	}

}

[System.Serializable]
public class RetreatActionBase
{
	public bool inUse;
	public bool visited;
	public Transform retreatPosition;
	public List<EnemyAI> reinforcements = new List<EnemyAI> ();
}
