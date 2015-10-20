using UnityEngine;
using System.Collections;

public class collectable : MonoBehaviour {
	
	public int worth;
	
	void OnTriggerEnter (Collider other){

		if(other.gameObject.tag ==  "Collectable"){
			Debug.Log("Collectables is working");
			if (!other.name.Equals ("LevelGoal"))
			{
				Destroy (other.gameObject);
			}
			else
			{
				// Transition to next level
				GameStateManager.gameStateManagerRef.EnterGameState(Application.loadedLevel+1);
			}
		}
	}
}