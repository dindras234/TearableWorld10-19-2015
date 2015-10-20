using UnityEngine;
using System.Collections;

public class Star : MonoBehaviour {
	private GameObject[] stars;
	// Update is called once per frame
	void Update () {
		stars = GameObject.FindGameObjectsWithTag("Star");
		foreach (GameObject star in stars){
			Vector3 zaxis = star.transform.TransformDirection(-1*Vector3.forward);
			if(Physics.Raycast(star.transform.position, zaxis, Mathf.Infinity)){
				Debug.Log ("it is in front of the object");
			}
			//Debug.Log("star");
		}
	}
}
