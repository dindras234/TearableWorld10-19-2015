using UnityEngine;
using System.Collections;

public class RestartScript : MonoBehaviour 
{
	/// <summary>
	/// The tear manager local reference. -> J.C.
	/// </summary>
	private TearManager TearManagerLocalRef;
	
    GameStateManager gameStateManagerRef;
    WorldCollision worldCollisionRef;
	TWCharacterController playerRef;

    // Use this for initialization
    void Start()
    {
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
        gameStateManagerRef = mainObject.GetComponent<GameStateManager>();
        gameStateManagerRef.EnsureGameScriptsAdded();
        worldCollisionRef = gameStateManagerRef.GetWorldCollision();
		playerRef = GameObject.FindGameObjectWithTag("Player").GetComponent<TWCharacterController>();
		
		//Init local reference - J.C.
		TearManagerLocalRef = GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>();
		
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerRef)
        {
            playerRef = GameObject.FindGameObjectWithTag("Player").GetComponent<TWCharacterController>();
        }

        if (gameStateManagerRef.inGame && playerRef)
        {
                if(Input.GetMouseButtonDown(0) &&
				//used to be pointinsideobject, not pointinsideobjectbutton - doug
                // if the finger is within the bounds of this object
                (worldCollisionRef.PointInsideObjectButton(gameObject,
                gameStateManagerRef.GetTouchController().GetLastFingerPosition()) ||
                worldCollisionRef.PointInsideObjectButton(gameObject, Input.mousePosition)) &&

                !gameStateManagerRef.GetInputManager().tearManagerRef.PlayerCurrentlyTearing &&
                !gameStateManagerRef.GetInputManager().foldRef.currentlyFolding &&

                // not performing a tear or a fold
                !gameStateManagerRef.GetInputManager().GetFingerGesture().Equals(InputManager.FingerGesture.TEAR) &&
                !gameStateManagerRef.GetInputManager().GetFingerGesture().Equals(InputManager.FingerGesture.FOLD))
            {
               // gameStateManagerRef.GetScreenManager().DisplayScreen(ScreenAreas.RestartLevel);
				playerRef.playerIsDead = true;
				
				
            }
        }
    }
}
