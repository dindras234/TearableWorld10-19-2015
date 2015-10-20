using UnityEngine;
using System.Collections;

public class LvlGoalCore : MonoBehaviour {
	
	private bool coresColliding = false;
	
	public bool getCoresColliding(){
		return coresColliding;
	}
	
	void OnTriggerEnter(Collider other){
		if(other.gameObject.name.Equals("PlayerLvlGoalCore")){
			//Debug.Log("hitting");
			coresColliding = true;
		}
	}
	
	void OnTriggerStay(Collider other){
		if(other.gameObject.name.Equals("PlayerLvlGoalCore")){
			coresColliding = true;
		}
	}
	
	void OnTriggerExit(Collider other){
		if(other.gameObject.name.Equals("PlayerLvlGoalCore")){
			//Debug.Log("not hitting");
			coresColliding = false;
		}
	}
	
	/// <summary>
	/// Start this instance -> J.C.
	/// </summary>
	public void Start()
	{
		//
		//THE FOLLOWING IS A BRUTE FORCE METHOD IN MAKING SURE THE 
		//  DOOR IS NOT DRAWN ON TOP OF OBJECTS (mainly torn piece and fold)!!!!!! ---> J.C.
		//
		gameObject.transform.parent.transform.position = new Vector3(gameObject.transform.parent.transform.position.x, 
																		gameObject.transform.parent.transform.position.y, 
																		0.5f);
	}
}
