using UnityEngine;
using System.Collections;

public class collectable : MonoBehaviour {
	
	public int worth;
	
	void OnTriggerEnter (Collider other){

		if(other.gameObject.tag ==  "Collectable"){
			Debug.Log("Collectables is working");
			Destroy (other.gameObject);
		}
		else {
			Debug.Log ("He's dead, Jim");	
		}
	}
}