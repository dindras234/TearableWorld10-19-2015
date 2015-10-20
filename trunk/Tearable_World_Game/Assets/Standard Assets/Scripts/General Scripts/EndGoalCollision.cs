using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Script should be attached to the end goal object
/// that will determine collision with the player and raise
/// the end game flag.  This script assumes that the player is
/// already in game at start.
/// </summary>
public class EndGoalCollision : MonoBehaviour 
{

    GameObject playerObject;
    WorldCollision worldCollisionRef;
    
	// Use this for initialization
	void Start () 
    {
        // the following is now needed
        // due to the prefab of 'MainObject'
        GameObject mainObject = GameObject.FindGameObjectsWithTag("MainObject")[0];

        if (GameObject.FindGameObjectsWithTag("MainObject").Length > 1)
        {
            GameObject[] mainObjectList = GameObject.FindGameObjectsWithTag("MainObject");
            for (int i = 0; i < mainObjectList.Length; ++i)
            {
                if (mainObjectList[i].GetComponent<GameStateManager>().objectSaved)
                    mainObject = mainObjectList[i];
            }
        }

        // Ensures all necessary scripts are added for the MainObject
        GameStateManager gameStateManagerRef = mainObject.GetComponent<GameStateManager>();
        gameStateManagerRef.EnsureScriptAdded("WorldCollision");

        worldCollisionRef = mainObject.GetComponent<WorldCollision>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () 
    {
       
	}
}
