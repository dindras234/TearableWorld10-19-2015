using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

public class Restart_Scene : MonoBehaviour {

	//On mouse Down restart scene
	void OnMouseDown()
	{
		Application.LoadLevel(Application.loadedLevel);
	}
}
