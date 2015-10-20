using UnityEngine;
using System.Collections;

// Class used to hold all global Variables

public class GVariables : MonoBehaviour{
	// The number of coins and keys collected in this level.
	public int			coins = 0, keys = 0,
	// The number of keys required to win in this level.
						keysNeeded = 0;
	
	// The z fold layer and z cover layer.
	public static float	zFoldLayer = -0.4F, zCoverLayer = -0.5F;
	
	/*
		The tear piece covering fold flags the event when a player sets the torn piece down on top of the folded
			paper area, preventing a fold from being modified. This is set true within the tearManager.cs once the
			player sets down the peice of paper after finalizing translations and rotations of torn piece
		-> J.C.
	*/
	public static bool	TearPieceCoveringFold = false,
	
	/*
		The fold piece covering tear is set true within fold mecahnic logic scripts. This blocks and translation/rotation of
			torn piece. This is set true when a fold covers any region of the torn piece
			
		TODO -> TONTON && AUDREY -> please add logic in your fold mechaics scripts to set this true and false accordingly
			when the player decides to fold over the torn piece, as well as when they unfold from having the torn peice covered,
		THANK YOU -> John
	*/
						FoldPieceCoveringTear = false;
	
	
	//TODO -> SOLIDIFY THE ACTUAL DEPTH VALUES WE WANT TO USE FOR THE FOLLOWING TWO STATICS -> J.C.

	// This depth is used as the fold/torn piece z-depth payer when it is to be drawn ON TOP of other mechanic
	public static float	OnTopDepth = 0.1F,
	// This depth is used as the fold/torn piece z-depth payer when it is to be drawn ON BOTTOM of other mechanic (i.e. covered)
						OnBottomDepth = 0.2F,
	
	// The tear on bottom, on top, and on the door.
						TearOnBottom = 0.3F, TearOnTop = 0.3F,
						TearOnTopOfDoor = -1.0F;
	
	// Start this instance.
	private void Start(){
		TearPieceCoveringFold = false;
		FoldPieceCoveringTear = false;
		
		keysNeeded = GameObject.FindGameObjectsWithTag("Key").Length;
	}
	
	
	// Update this instance.
	private void Update(){
	//	Debug.Log("torn piece covering Fold = " + TearPieceCoveringFold.ToString());
	//	Debug.Log("Fold Covering Tear = " + FoldPieceCoveringTear.ToString());
	}
	
}
