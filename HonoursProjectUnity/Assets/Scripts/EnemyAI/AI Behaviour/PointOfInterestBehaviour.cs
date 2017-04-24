using UnityEngine;
using System.Collections;

public class PointOfInterestBehaviour : MonoBehaviour 
{

	EnemyAI enemyAI_Main;
	
	// Use this for initialization
	public void Init () 
	{
		enemyAI_Main = GetComponent<EnemyAI> ();
	}

	public void POI_Behaviour ()
	{
		if (enemyAI_Main.PointsOfInterest.Count > 0)
		{
			for (int i = 0; i < enemyAI_Main.PointsOfInterest.Count; i++)
			{
				if (enemyAI_Main.PointsOfInterest[i] != null)
				{
					POI_Behaviour (enemyAI_Main.PointsOfInterest[i].typePOI, enemyAI_Main.PointsOfInterest[i]);
				}
			}
		}
	}

	void POI_Behaviour (POI_Base.TypePOI type, POI_Base poi)
	{
		switch (type)
		{
		case POI_Base.TypePOI.deadBody:

			POI_DeadBody body = poi.transform.GetComponent<POI_DeadBody> ();

			if (body.isActiveAndEnabled)
			{
				Vector3 dirToPOI = body.transform.position - transform.position;
				float angleToTarget = Vector3.Angle (transform.forward, dirToPOI.normalized);

				if (angleToTarget < enemyAI_Main.charStats.viewAngleLimit)
				{
					Vector3 origin = transform.position + new Vector3 (0, 1, 0);
					Vector3 rayDirection = body.transform.position - origin;

					RaycastHit hit;

					if (Physics.Raycast (origin, rayDirection, out hit, 50))
					{
						if (hit.transform.Equals (body.transform) || hit.transform.GetComponentInParent<CharacterStats> ())
						{
							if (hit.transform.GetComponentInParent<CharacterStats> ().dead)
							{
								enemyAI_Main.lastKnownPosition = poi.transform.position;
								enemyAI_Main.charStats.alert = true;
								enemyAI_Main.charStats.alertLevel = 10;

								enemyAI_Main.AI_State_Chase ();

								enemyAI_Main.PointsOfInterest.Remove (poi);
								Destroy (poi);
							}
						}
					}
				}
			}
			else
			{
				enemyAI_Main.PointsOfInterest.Remove (poi);
			}
			break;
		case POI_Base.TypePOI.other:
			break;
		case POI_Base.TypePOI.unconsious:

			POI_Unconsious uncon = poi.transform.GetComponent<POI_Unconsious> ();

			if (uncon.isActiveAndEnabled)
			{
				Vector3 directionTowardsPOI = uncon.transform.position - transform.position;
				float angleTowardsTarget = Vector3.Angle (transform.forward, directionTowardsPOI.normalized);

				if (angleTowardsTarget < enemyAI_Main.charStats.viewAngleLimit)
				{
					Vector3 origin = transform.position + new Vector3 (0.0f, 1.0f, 0.0f);
					Vector3 rayDirection = uncon.transform.position - origin;

					RaycastHit hit;

					if (Physics.Raycast (origin, rayDirection, out hit, 50))
					{
						if (hit.transform.Equals (uncon.transform) || hit.transform.GetComponentInParent<CharacterStats> ())
						{
							if (hit.transform.GetComponentInParent<CharacterStats> ().unconsious)
							{
								enemyAI_Main.lastKnownPosition = uncon.transform.position;

								enemyAI_Main.AI_State_SearchLocation (Vector3.zero, uncon.owner);

								enemyAI_Main.PointsOfInterest.Remove (poi);
							}
						}
					}
				}
			}
			else
			{
				enemyAI_Main.PointsOfInterest.Remove (poi);
			}
			break;
		}
	}
}
