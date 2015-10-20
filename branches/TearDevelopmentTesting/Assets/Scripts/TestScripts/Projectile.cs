using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	private float moveSpeed = 0.5f;
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	private void Update () 
	{
		Vector3 direction = gameObject.transform.forward.normalized * moveSpeed;
		this.transform.position = new Vector3(gameObject.transform.position.x + direction.x, 
												gameObject.transform.position.y + direction.x,
												gameObject.transform.position.z + direction.z);
	}
}
