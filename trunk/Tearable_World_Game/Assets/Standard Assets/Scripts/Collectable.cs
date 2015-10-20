using UnityEngine;
using System.Collections;

// This is a script attached to the player object for collecting things in each of the levels.
public class Collectable : MonoBehaviour{
	// Reference pointers to the main object and the global variables.
	private GameStateManager			gameStateManagerRef;
	private GVariables					globalVariables;
	
	// Set reference pointers to the correct objects.
	public void Start(){
		// Get the main object reference and ensure core scripts are added.
		GameObject mainObject = GameObject.FindGameObjectsWithTag("MainObject")[0];
		
		if(GameObject.FindGameObjectsWithTag("MainObject").Length > 1){
			GameObject[] mainObjectList = GameObject.FindGameObjectsWithTag("MainObject");
			for(int i = 0; i < mainObjectList.Length; ++i){
				if(mainObjectList[i].GetComponent<GameStateManager>().objectSaved){
					mainObject = mainObjectList[i];
				}
			}
		}
		gameStateManagerRef = mainObject.GetComponent<GameStateManager>();
		gameStateManagerRef.EnsureCoreScriptsAdded();
		
		// Get the global variables reference.
		GameObject gVar = GameObject.FindGameObjectsWithTag("globalVariables")[0];
		globalVariables = gVar.GetComponent<GVariables>();
	}
	
	// When the object this script is attached to (the player) collides with a collectable, collect it.
	void OnTriggerEnter(Collider other){
		// If this was a coin, collect it.
		if(other.tag.Equals("Coin")){
			if(other.renderer.enabled){ ++globalVariables.coins; }
			//Debug.Log("Coins collected: " + globalVariables.coins);
			other.collider.enabled = false;
			other.renderer.enabled = false;
		}
		// If this was a key, collect it.
		else if(other.tag.Equals("Key")){
			if(other.renderer.enabled){ ++globalVariables.keys; }
			//Debug.Log("Keys collected: " + globalVariables.keys);
			
			// Give an extra coin for a key the player did not need to win.
			if(globalVariables.keys > globalVariables.keysNeeded){
				if(other.renderer.enabled){ ++globalVariables.coins; }
			}
			other.collider.enabled = false;
			other.renderer.enabled = false;
		}
	}
}