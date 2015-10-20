using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGoal: MonoBehaviour{
	private ScreenManager				screenManagerRef;
	private GameStateManager			gameStateManagerRef;
	private TWCharacterController		characterControllerRef;
	private SoundManager				soundManagerRef;
	private GVariables					globalVariables;

	private GameObject					mainCamera;
	private bool						covered = false,
										tearOrFoldOccurred = false,
										proceedToNextLevel = false,
										coresColliding = false,
										orientationCorrect = false;
	
	public bool							goalOnBackSide = false;
	public float 						offset = 0;
	public Texture2D					coveredDoor;
	public Texture2D					Door;
	
	// References for TearManager, Fold, and this goal's core.
	private TearManager					tearManager;
	private Fold						fold;
	
	private LvlGoalCore					LvlGoalCoreRef;
	
	GameObject buttonCamera;
	/// <summary>
	/// Reset the goal back to start of level.
	/// </summary>
	public void Reset()
	{
		covered = false;
		tearOrFoldOccurred = false;
		coresColliding = false;
		orientationCorrect = false;
		offset = 0;
	}
	void Start(){
		// the following is now needed due to the prefab of 'MainObject'
		GameObject mainObject = GameObject.FindGameObjectsWithTag("MainObject")[0];
		gameStateManagerRef = mainObject.GetComponent<GameStateManager>();
		gameStateManagerRef.EnsureCoreScriptsAdded();
		screenManagerRef = mainObject.GetComponent<ScreenManager>();
		soundManagerRef = mainObject.GetComponent<SoundManager>();
		characterControllerRef = GameObject.FindGameObjectWithTag("Player").GetComponent<TWCharacterController>();
		mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		
		// get a reference to the tear manager
		tearManager = GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>();
		
		// get a reference to fold
		fold = GameObject.FindGameObjectWithTag("FoldObject").GetComponent<Fold>();
		
		LvlGoalCoreRef = GameObject.FindGameObjectWithTag("GoalCore").GetComponent<LvlGoalCore>();
		
		// Get the global variables reference.
		GameObject gVar = GameObject.FindGameObjectsWithTag("globalVariables")[0];
		globalVariables = gVar.GetComponent<GVariables>();
		
		// if the goal is on the backside move necessary components by offset to be at correct location.
		if(goalOnBackSide){
			// change the box collider on the goal core
			Vector3 GoalCollider = this.gameObject.GetComponent<BoxCollider>().center;
			GoalCollider = new Vector3(GoalCollider.x, GoalCollider.y, GoalCollider.z + (offset*2f));
			this.gameObject.GetComponent<BoxCollider>().center = GoalCollider;
			
			// change the graphics child object position
			Vector3 GraphicTrans = transform.FindChild("Graphics").transform.position;
			GraphicTrans = new Vector3(GraphicTrans.x, GraphicTrans.y, GraphicTrans.z + offset);
			transform.FindChild("Graphics").transform.position = GraphicTrans;
			
			// change the goal core child object.
			Vector3 GoalCoreTrans = transform.FindChild("GoalCore").transform.position;
			GoalCoreTrans = new Vector3(GoalCoreTrans.x, GoalCoreTrans.y, GoalCoreTrans.z + offset);
			transform.FindChild("GoalCore").transform.position = GoalCoreTrans;
		}
		buttonCamera = GameObject.FindGameObjectWithTag("button");
	}
	
	// Update is called once per frame
	void Update(){
		// make sure our orientation is okay to pass through door
		CheckOrientation();
		
		// if covered change the level goal image to blocked
		if (covered)
		{
			transform.FindChild("Graphics").GetComponent<MeshRenderer>().material.mainTexture = coveredDoor;
		}
		else
		{
			transform.FindChild("Graphics").GetComponent<MeshRenderer>().material.mainTexture = Door;
		}

		// make sure playercore colliding with levelgoal core
		coresColliding = LvlGoalCoreRef.getCoresColliding();
		//Debug.Log("Cores colliding: " + coresColliding);
		
		handleTearAndFold();
		//Debug.Log("proceedToNextLevel: " + proceedToNextLevel);
		//Debug.Log("covered: " + covered);
		// if proceedToNextLevel and not covered finish the level
		if(proceedToNextLevel && !covered)
		{
			finishLevel();
		}
	}
	
	// checks to see if we are within a passable orientation to the door. 
	void CheckOrientation(){
		// gets the angle between the player and the level goal
		float angle = Quaternion.Angle(characterControllerRef.transform.rotation, this.transform.rotation);
		//Debug.Log("Angle: " + angle);
		//Debug.Log(this.transform.rotation);
		if(angle<=45){
			orientationCorrect = true;
		}
		else orientationCorrect = false;
	}
	
	// pauses in game events and calls for next level
	void finishLevel(){
		//Debug.Log("colliding with door found"); 
		screenManagerRef.DisplayScreen(ScreenAreas.LevelComplete);
		// pauses in game events so that the player can't continue to move around
		Time.timeScale = 0;
	}
	
	void OnTriggerEnter (Collider other){
		//Debug.Log("coresColliding: " + coresColliding + " OrientationCorrect: "+ orientationCorrect + " other.tag: " + other.tag + " isGrounded: "+ characterControllerRef.getGrounded()); 
		if(other.tag.Equals("Player")
					&& (globalVariables.keys >= globalVariables.keysNeeded)
					&& characterControllerRef.getGrounded()
					&& coresColliding
					&& orientationCorrect){
			buttonCamera.SetActive(false);
			//Debug.Log("Player just hit the door");
			proceedToNextLevel = true;
		}
	}
	
	void OnTriggerStay (Collider other){
		//Debug.Log("coresColliding: " + coresColliding + " OrientationCorrect: "+ orientationCorrect + " other.tag: " + other.tag + " isGrounded: "+ characterControllerRef.getGrounded());
		if(other.tag.Equals("Player")
					&& (globalVariables.keys >= globalVariables.keysNeeded)
					&& characterControllerRef.getGrounded()
					&& coresColliding
					&& orientationCorrect){
			soundManagerRef.PlayAudio("doorOpen", "SFX");
			buttonCamera.SetActive(false);
			proceedToNextLevel = true;
		}
	}
	
	void OnTriggerExit(Collider other){
		if(other.tag.Equals("Player")){
			proceedToNextLevel = false;
		}
	}
	
	// handles checking if the currentCollisions list needs to be cleared when a tear happens
	void handleTearAndFold(){
		// logic for finding out if a tear has started and when that torn piece gets placed we need to clear the currentCollision list
		if((tearManager.TornPieceCurrentlyMaskingCollision ||
			!fold.currentlyFolding) && !tearOrFoldOccurred){
			tearOrFoldOccurred = true;
			//Debug.Log("Running raycast");
			//Debug.Log(new Vector3(this.transform.position.x-this.transform.localScale.x, this.transform.position.y-this.transform.localScale.y, -10F));
			//Debug.Log(new Vector3(this.transform.position.x+this.transform.localScale.x, this.transform.position.y-this.transform.localScale.y, -10F));
			//Debug.Log(new Vector3(this.transform.position.x-this.transform.localScale.x, this.transform.position.y+this.transform.localScale.y, -10F));
			//Debug.Log(new Vector3(this.transform.position.x+this.transform.localScale.x, this.transform.position.y+this.transform.localScale.y, -10F));
			//Debug.Log(this.collider.bounds);
			if(raycastCheck(new Vector3(this.transform.position.x-this.collider.bounds.extents.x, this.transform.position.y-this.collider.bounds.extents.y, -10F))||
				raycastCheck(new Vector3(this.transform.position.x+this.collider.bounds.extents.x, this.transform.position.y-this.collider.bounds.extents.y, -10F))||
				raycastCheck(new Vector3(this.transform.position.x-this.collider.bounds.extents.x, this.transform.position.y+this.collider.bounds.extents.y, -10F))||
				raycastCheck(new Vector3(this.transform.position.x+this.collider.bounds.extents.x, this.transform.position.y+this.collider.bounds.extents.y, -10F))){
				
				covered = true;
			}
			else {covered = false;}
			//Debug.Log("covered set to: " + covered);
		}
		// if PlayerMovingPlatformState is true then we need to set tearOccurred = false signalling that a new tear is happening. 
		else if(tearManager.PlayerMovingPlatformState || fold.currentlyFolding){
			tearOrFoldOccurred = false;
		}
	}
	
	// does a raycast against all colliders in a line from start checking for a torn piece of paper or a folded piece of paper.
	bool raycastCheck(Vector3 start){
		// raycast to see if the level goal is being eclipsed by anything
		RaycastHit[] hits;
		hits = Physics.RaycastAll(start, Vector3.forward, 20F);
		bool foundTear = false;
		RaycastHit	foldHit = new RaycastHit(),
					tearHit = new RaycastHit();
		bool foundFold = false;
		foreach (RaycastHit hit in hits){
			//Debug.Log("raycast is hitting: " + hit.transform.name);
			if(hit.transform.name.Equals("paper_CuttPieceOfPaper")){
				foundTear = true;
				tearHit = hit;
			}
			else if(hit.transform.tag.Equals("Fold")){
				//Debug.Log("Raycast" + start + " found: " + hit.transform.name);
				foundFold = true;
				foldHit = hit;
			}
		}
		if(!goalOnBackSide && (foundFold || foundTear)){
			return true;
		}
		else if(goalOnBackSide && foundTear && foundFold){
			if(foldHit.transform.position.z > tearHit.transform.position.z){
				return true;
			}
		}
		return false;
	}
}
