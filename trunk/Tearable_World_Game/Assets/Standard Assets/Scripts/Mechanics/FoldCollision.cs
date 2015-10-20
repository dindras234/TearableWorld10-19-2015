using UnityEngine;
using System.Collections;

public class FoldCollision : MonoBehaviour {
	
	public bool hitting = false;
	Collider collisionInfo;
	GameObject playerReference;
	GameObject coverupReference;
	GameObject foldReference;
	bool firstCollision = true;
	public Vector3 newFoldPosition;
	public Vector3 newCoverPosition;

	public bool getHitting(){
		return hitting;
	}
	
	public Collider getCollider(){
		return collisionInfo;
	}
	
	//checks if the hitbox on the bottom of the player is colliding with an object.
	void OnTriggerStay(Collider collisionInfo){
		// if the hit box is colliding with something set hittingbottom to true and store what we collided with.
		if(collisionInfo.gameObject.tag == "Player")
        {
            //Debug.Log("COLLISION " + collisionInfo.gameObject.tag.ToString());
			coverupReference = GameObject.FindGameObjectWithTag("coverpivot");
			foldReference = GameObject.FindGameObjectWithTag("backpivot");
			if(firstCollision){
				newFoldPosition = foldReference.transform.position;
				newCoverPosition = coverupReference.transform.position;
				firstCollision = false;
			}
			playerReference = GameObject.FindGameObjectWithTag("Player");
			hitting = true;
			this.collisionInfo = collisionInfo;
			//reference.rigidbody.isKinematic = true;
			playerReference.rigidbody.velocity = Vector3.zero;
			foldReference.transform.position = newFoldPosition;
			coverupReference.transform.position = newCoverPosition;
		}
	}
	
	void OnTriggerExit(Collider collisionInfo){
		if(collisionInfo.gameObject.tag == "Player"){
			playerReference = GameObject.FindGameObjectWithTag("Player");
			this.hitting = false;
			firstCollision = true;
			//reference.rigidbody.isKinematic = false;
		}
	}
	
}
