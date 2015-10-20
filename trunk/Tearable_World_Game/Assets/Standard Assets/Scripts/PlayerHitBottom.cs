using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerHitBottom : MonoBehaviour 
{
	
	private bool hittingBottom = false; //needs to be true or player will fall, and begin to gain massive amount of velocity
	Collider collider;
	public GameObject[] collidables;
    GameStateManager gameStateManagerRef;
	private List<Collider> currentCollisions = new List<Collider>();
	private bool foldOccurred = false;
	private bool tearOccurred = false;
	// references to TearManager and Fold
	TearManager tearManager;
	Fold fold;
	
	public bool getHitting()
    {
		//Debug.Log(hittingBottom);
		return hittingBottom;
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
	
	void OnTriggerEnter(Collider collisionInfo){
		if (collisionInfo.gameObject.CompareTag("Platform") || collisionInfo.gameObject.CompareTag("FoldPlatform"))
        {
			if(!ListContainsElementOfName(currentCollisions, collisionInfo)){
				//Debug.Log("adding element: " + collisionInfo.name);
				currentCollisions.Add(collisionInfo);
			}
            //this.hittingBottom = true;
        }
		//else {hittingBottom = false;}
	}
	
	//checks if the hitbox on the bottom of the player is colliding with an object.
	/*void OnTriggerStay(Collider collisionInfo)
    {
            //Debug.Log("true" + collisionInfo.name);
            // if the hit box is colliding with something set hittingbottom to true and store what we collided with.
            if (collisionInfo.gameObject.CompareTag("Platform") || collisionInfo.gameObject.CompareTag("FoldPlatform"))
            {
                //Debug.Log("COLLISION " + collisionInfo.gameObject.tag.ToString());
                hittingBottom = true;
            }
			else hittingBottom = false;
	}*/
	
	void OnTriggerExit(Collider collisionInfo)
    {
            //Debug.Log("false" + collisionInfo.name);
            if (collisionInfo.gameObject.CompareTag("Platform") || collisionInfo.gameObject.CompareTag("FoldPlatform"))
            {
              	if(ListContainsElementOfName(currentCollisions, collisionInfo)){
					//Debug.Log("Removing element: " + collisionInfo.name);
					currentCollisions.Remove(GetElementByName(currentCollisions, collisionInfo.gameObject.name));
				}
            }
		//this.hittingBottom = false;
	}
	
	// handles checking if the currentCollisions list needs to be cleared when a tear happens
	void handleTear(){
		// logic for finding out if a tear has started and when that torn piece gets placed we need to clear the currentCollision list
		if(tearManager.TornPieceCurrentlyMaskingCollision && !tearOccurred){
			//Debug.Log("Need to clear list from tear");
			//StartCoroutine(delayClearList());
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
		if(fold.isFolded && !foldOccurred){
			//Debug.Log("Need to clear list from fold");
			currentCollisions.Clear();
			foldOccurred = true;
		}
		// if firstTouch is true then a new fold has just been initiated and we need to set foldOccurred back to reset
		else if(fold.firstTouch){
			foldOccurred = false;
		}
	}
	
	void Start(){
		// get a reference to the tear manager
		tearManager = GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>();
		
		// get a reference to fold
		fold = GameObject.FindGameObjectWithTag("FoldObject").GetComponent<Fold>();
	}
	
	void Update(){
		handleFold();
		handleTear();
		if(currentCollisions.Count > 0){
			Debug.Log("List not empty");
			hittingBottom = true;
		}
		else hittingBottom = false;
	}
	
	/*
	void updateCollidableList() {
		GameObject[] c1 = GameObject.FindGameObjectsWithTag("Platform");
		GameObject[] c2 = GameObject.FindGameObjectsWithTag("FoldPlatform");
		collidables = new GameObject[c1.Length + c2.Length];
		c1.CopyTo(collidables,0);
		c2.CopyTo(collidables,c1.Length);
	}
	
	void Start() 
    {
		collider = this.gameObject.collider;
	}
	
	void Update() 
    {
        //UnityEngine.Debug.Log("NUM " + Application.loadedLevel.ToString());

        // JOE, you didn't realize that platforms in level 2
        // also have the "Platform" tag so this code won't work on that level
        // this will eventually need to be changed.
        // In the mean time I just allow the old code above to run
        if (!Application.loadedLevel.Equals((int)GameStateManager.LevelScenes.Level_2))
        {
			// TODO: Make this be called less than once every update.  -- Joe
			updateCollidableList();
            for (int i = 0; i < collidables.Length; i++)
            {
                GameObject currObj = collidables[i];
                if (this.collider.bounds.Intersects(currObj.collider.bounds))
                {
                   // this.hittingBottom = true;
                    return;
                }
            }
            this.hittingBottom = false;
        }
	}*/
	
}
