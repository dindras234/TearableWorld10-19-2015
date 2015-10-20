using UnityEngine;
using System.Collections;

public class FoldButtonScript : MonoBehaviour {
	InputManager inpManRef;
	WorldCollision worldCollisionRef;
	GameStateManager gameStateManagerRef;
	// Use this for initialization
	void Start () {
		gameStateManagerRef = GameObject.FindGameObjectWithTag("MainObject").GetComponent<GameStateManager>();
		inpManRef = gameStateManagerRef.GetInputManager();
		worldCollisionRef = gameStateManagerRef.GetWorldCollision();
	}
	
	// Update is called once per frame
	void Update () {
		if(inpManRef == null) inpManRef = gameStateManagerRef.GetInputManager();
		if(worldCollisionRef == null) worldCollisionRef = gameStateManagerRef.GetWorldCollision();
		if(inpManRef.foldMode) gameObject.renderer.material.color = new Color(0.0f, 0.0f, 0.0f);
		else gameObject.renderer.material.color = new Color(1.0f, 1.0f, 1.0f);
        // if the finger is within the bounds of this object
#if UNITY_IPHONE
		if(worldCollisionRef.PointInsideObject(gameObject,
        gameStateManagerRef.GetTouchController().GetLastFingerPosition())
#endif
#if UNITY_ANDROID
		if(worldCollisionRef.PointInsideObject(gameObject,
        gameStateManagerRef.GetTouchController().GetLastFingerPosition())
#endif
#if UNITY_STANDALONE
		if(Input.GetMouseButtonDown(0) &&  (worldCollisionRef.PointInsideObject(gameObject, Input.mousePosition)) 
#endif
		&& !inpManRef.tearManagerRef.PlayerCurrentlyTearing &&
		!inpManRef.foldRef.currentlyFolding) /*&&	                                                                 
        // not performing a tear or a fold
        !gameStateManagerRef.GetInputManager().GetFingerGesture().Equals(InputManager.FingerGesture.TEAR) &&
        !gameStateManagerRef.GetInputManager().GetFingerGesture().Equals(InputManager.FingerGesture.FOLD)*/
    	{
			inpManRef.moveMode = false;
			inpManRef.foldMode = true;
			inpManRef.tearMode = false;
			gameObject.renderer.material.color = new Color(0.0f, 0.0f, 0.0f);
    	}
	}
}
