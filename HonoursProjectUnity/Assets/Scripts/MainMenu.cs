﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	public void Quit ()
	{
		Application.Quit ();
	}

	public void LevelOne ()
	{
		Application.LoadLevel (1);
	}

	public void LevelTwo ()
	{
		Application.LoadLevel (2);
	}
}
