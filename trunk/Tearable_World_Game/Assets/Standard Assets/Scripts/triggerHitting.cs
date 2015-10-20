using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class triggerHitting : MonoBehaviour 
{
	
	private bool hitting = false; 
	Collider collider;
	public string placementOf;
    GameStateManager gameStateManagerRef;
	private List<Collider> currentCollisions = new List<Collider>();
	
	// reference for TearManager and Fold
	TearManager tearManager;
	Fold fold;
	
	public bool getHitting()
    {
		//Debug.Log(hitting);
		return hitting;
	}
	
	public Collider getCollider()
    {
		return collider;
	}
	
		// returns true or false if a Collider with the same name as hit exists
	bool ListContainsElementOfName(List<Collider> currList, Collider hit){
		foreach (Collider current in currList){
			if(current.gameObject.name.Equals(hit.gameObject.name)){
					return true;
				}
		}
		return false;
	}
	
	// returns true or false if a Collision with the same name as string name exists
	bool ListContainsElementOfGameObject(List<Collider> currList, GameObject obj){
		foreach (Collider current in currList){
			if(current != null)
			{
				if(current.gameObject != null && obj != null)
				{
					if(current.gameObject == obj){
						return true;
					}
				}
			}
		}
		return false;
	}
	
		// search for an Collider element in a list by matching their names, returns the element
	Collider GetElementByName(List<Collider> currList, string name){
		foreach (Collider current in currList){
			if(current.gameObject.name.Equals(name)){
				//Debug.Log("Removing " + current.gameObject.name);
				return current;
			}
		}
		return null;
	}
	
	Collider GetElementByGameObject(List<Collider> currList, GameObject obj){
		foreach (Collider current in currList){
			if(current.gameObject == obj){
				return current;
			}
		}
		return null;
	}
	
	string printList(List<Collider> currList){
		string output = "";
		foreach (Collider current in currList){
			output += " " + current.name + " ";
		}
		return output;
	}
	
	// on trigger enter if currentCollisions(A list of current collisions occuring) does not contain the element then add it.
	void OnTriggerEnter(Collider collisionInfo){
		if (collisionInfo.gameObject.CompareTag("Platform") || collisionInfo.gameObject.CompareTag("FoldPlatform"))
        {
			if(!ListContainsElementOfGameObject(currentCollisions, collisionInfo.gameObject)){
				//Debug.Log("adding element: " + collisionInfo.name);
				currentCollisions.Add(collisionInfo);
			}
        }
	}
	
	// on trigger stay if currentCollisions(A list of current collisions occuring) does not contain the element then add it.
	void OnTriggerStay(Collider collisionInfo){
		if (collisionInfo.gameObject.CompareTag("Platform") || collisionInfo.gameObject.CompareTag("FoldPlatform"))
        {
			if(!ListContainsElementOfGameObject(currentCollisions, collisionInfo.gameObject)){
				//Debug.Log("adding element: " + collisionInfo.name);
				currentCollisions.Add(collisionInfo);
			}
        }
	}
	
	// on trigger exit remove the correct object from currentCollisions.
	void OnTriggerExit(Collider collisionInfo)
    {
            //Debug.Log("false" + collisionInfo.name);
            if (collisionInfo.gameObject.CompareTag("Platform") || collisionInfo.gameObject.CompareTag("FoldPlatform"))
            {
              	if(ListContainsElementOfGameObject(currentCollisions, collisionInfo.gameObject)){
					//Debug.Log("Removing element: " + collisionInfo.name);
					currentCollisions.Remove(GetElementByGameObject(currentCollisions, collisionInfo.gameObject));
				}
            }
	} 
	
	// clears list after delay
	IEnumerator delayClearList(){
		yield return new WaitForSeconds(1);
		currentCollisions.Clear();
		//Debug.Log("Clear has been called");
	}
	
	// handles checking if the currentCollisions list needs to be cleared when a tear happens
	void handleTear(){
		// logic for finding out if a tear has started and when that torn piece gets placed we need to clear the currentCollision list
		if(tearManager.TornPieceCurrentlyMaskingCollision && !tearOccurred){
			//Debug.Log("Need to clear list from tear");
			currentCollisions.Clear();
			tearOccurred = true;
		}
		// if PlayerMovingPlatformState is true then we need to set tearOccurred = false signalling that a new tear is happening. 
		else if(tearManager.PlayerMovingPlatformState){
			tearOccurred = false;
		}
	}
	
	// handles checking if the currentCollisions list needs to be cleared when a fold happens
	void handleFold(){
		// checks if a fold has not occured and if isFolded is true then we need to clear the list because a fold just occurred
		if(!fold.currentlyFolding && !foldOccurred){
			//Debug.Log("Need to clear list from fold");
			currentCollisions.Clear();
			foldOccurred = true;
		}
		// if firstTouch is true then a new fold has just been initiated and we need to set foldOccurred back to reset
		else if(fold.currentlyFolding && foldOccurred){
			foldOccurred = false;
		}
	}
	
	private bool tearOccurred = false;
	private bool foldOccurred = false;
	
	void Start(){
		// get a reference to the tear manager
		tearManager = GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>();
		
		// get a reference to fold
		fold = GameObject.FindGameObjectWithTag("FoldObject").GetComponent<Fold>();
	}
	public void Reset()
	{
		currentCollisions.Clear();
	}
	void Update(){
		
		handleTear();
		handleFold();
		
		//Debug.Log("Trigger hitting: " + printList(currentCollisions));
		if(currentCollisions.Count > 0){
			//Debug.Log("List not empty");
			hitting = true;
		}
		else hitting = false;
	}
	
}
