using UnityEngine;
using System.Collections;

public class PlayerHitBottom : MonoBehaviour {
	
	private bool hittingBottom = true; //needs to be true or player will fall, and begin to gain massive amount of velocity
	Collider collisionInfo;
	
	public bool getHitting(){
		return hittingBottom;
	}
	
	public Collider getCollider(){
		return collisionInfo;
	}
	
	//checks if the hitbox on the bottom of the player is colliding with an object.
	void OnTriggerStay(Collider collisionInfo){
		//Debug.Log("true" + collisionInfo.name);
		// if the hit box is colliding with something set hittingbottom to true and store what we collided with.
		if(collisionInfo.gameObject.tag == "Platform")
        {
            //Debug.Log("COLLISION " + collisionInfo.gameObject.tag.ToString());
			hittingBottom = true;
			this.collisionInfo = collisionInfo;
		}
	}
	
	void OnTriggerExit(Collider collisionInfo){
		//Debug.Log("false" + collisionInfo.name);
		if(collisionInfo.gameObject.tag == "Platform"){
			this.hittingBottom = false;
		}
	}
	
}
