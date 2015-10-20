using UnityEngine;
using System.Collections;

public class Restart_Scene : MonoBehaviour {

	//On mouse Down restart scene
	void OnMouseDown()
	{
		Application.LoadLevel(Application.loadedLevel);
	}
}
