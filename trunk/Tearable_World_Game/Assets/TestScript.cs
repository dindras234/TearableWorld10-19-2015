using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {
	private GameObject plat;
	//ChangeMeshScript script;
	bool down = false;
	// Use this for initialization
	void Start () {
		//script = gameObject.GetComponent<ChangeMeshScript>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0))
			down = true;
		if(Input.GetMouseButtonUp(0))
			down = false;
		if(down)
		{
			Vector3 temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			temp.z = 0f;
			transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			transform.position = temp;
//			script.ChangeMesh(transform.gameObject.GetComponentInChildren<MeshFilter>().mesh.vertices, transform, 0.3f, 0.3f);
		}
	}
}
