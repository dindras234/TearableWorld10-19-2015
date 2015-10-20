using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Quadrant
{
	TOP_LEFT,
	TOP_RIGHT,
	BOTTOM_RIGHT,
	BOTTOM_LEFT,
	NONE
}
public class Fold : MonoBehaviour
{	
	#region variables
	public Transform foldPrefab;
	public Transform coverupPrefab;

	#region references
	
	/// <summary>
	/// Reference to the backside of the paper. The fold mesh.
	/// </summary>
	public GameObject backsideReference;
	/// <summary>
	/// Reference to the main camera in the scene.
	/// </summary>
	GameObject cameraReference;
	/// <summary>
	/// Reference to the player in the scene
	/// </summary>
	GameObject playerReference;
	/// <summary>
	/// reference to the platforms on the backside of the paper
	/// </summary>
	GameObject[] backsideCollisionReference;
	/// <summary>
	/// reference to the change mesh script which deletes and restores triangles of objects covered by the fold
	/// </summary>
	public ChangeMeshScript changeMeshScript;
	/// <summary>
	/// reference to the background's transform. Used to get the bounds of the background.
	/// </summary>
	Transform backgroundTransform;
	/// <summary>
	/// reference to the object the fold will unfold to, based on where the fold started
	/// </summary>
	GameObject unfoldReference;
	/// <summary>
	/// Reference to the coverup mesh, the mesh that alphas out fold
	/// </summary>
	public GameObject coverupReference;
	GameObject backsidePivotReference;
	GameObject coverupPivotReference;
	GameObject tornBacksidePieceReference;
	GameObject tornPieceReference;
	GameObject shadowReference;
	GameObject rayTraceBlockRef;
	GameObject paperBorderInsideRef;
	GameObject paperBorderOutsideRef;
	GameObject tornBackground;
	Mesh tearPaperMesh;
	int[] backsideTriangles;
	public List<Vector3> missingTriangles;
	Mesh tornMesh;
	TearManager tearReference;
    GameStateManager gameStateManagerRef;
    SoundManager soundManagerRef;
	UnfoldCollision unfoldCollisionReference;
	Vector3 foldPosition, unfoldPosition;
	/// <summary>
	/// The screen manager reference.
	/// </summary>
	private ScreenManager screenManagerRef;
	Quadrant startingQuadrant;
	Quadrant currentQuadrant;
	Edge[] foldEdge;
	private WorldCollision worldCollisionRef;
	#endregion
	
	#region touch
	/// <summary>
	/// Reference to the touch controller, to allow us to get the user's input.
	/// </summary>
	TouchController touchController;
	/// <summary>
	/// Gives us the type of touch, tap, drag, swipe, multitouch, none, etc.
	/// </summary>
	TouchType touchType;
	/// <summary>
	/// The type of the previous touch.
	/// </summary>
	//TouchType prevTouchType;
	/// <summary>
	/// The list of points on the screen the user's fingers are.
	/// </summary>
	List<Vector2> fingerList;
	
	#endregion
	
	#region general positions and transforms
	Vector3 origin;
	
	/// <summary>
	/// The average finger position modified by origin
	/// </summary>
	Vector3 fingerPos;
	/// <summary>
	/// The background's minimum bounds. Used to calculate if the fold is on the paper.
	/// </summary>
	public Vector2 backgroundObjMin;
	/// <summary>
	/// The background's maximum bounds. Used to calculate if the fold is on the paper.
	/// </summary>
	public Vector2 backgroundObjMax;
	/// <summary>
	/// bounds of the background mesh.
	/// </summary>
	Bounds backgroundBounds;
	/// <summary>
	/// The average finger position. Used to calculate where the fold should go.
	/// </summary>
	public Vector3 avgFingerPos;
	/// <summary>
	/// The position of where the user first touched to start the fold
	/// </summary>
	Vector3 firstTouchPosition;
	/// <summary>
	/// The velocity of the smooth damp
	/// </summary>
	private Vector3 velocity = Vector3.zero;
	/// <summary>
	/// The position of the previous touch
	/// </summary>
	Vector3 lastFingerPosition;
	Vector3 posModLastValid;
	/// <summary>
	/// The start of the smooth damp time
	/// </summary>
	float startDampTime;
	Vector3 posModifier;
	Vector3 unfoldPosModifier;
	static float distCheck = 3f;
	bool blah;
	#endregion
	
	#region Fold positions and transforms
	
	/// <summary>
	/// The temporary z layer while the fold is being moved.
	/// </summary>
	float foldTmpZLayer;
	
	/// <summary>
	/// The original position and rotation of the fold.
	/// </summary>
	private Vector3 foldOriginalPosition;
	private Quaternion foldOriginalRotation;
	
	/// <summary>
	/// The position the fold will unfold to if it is an invalid fold
	/// </summary>
	public Vector3 foldUnfoldPosition;
	/// <summary>
	/// position the smooth damp is currently at. Used for the transforms of the smooth damp to unfold properly.
	/// </summary>
	public Vector3 foldSmoothPosition;
	
	/// <summary>
	/// The last valid finger position for if we need to unfold back to a previous fold
	/// </summary>
	public Vector3 foldLastValidPos;
	
	#endregion
	
	#region Coverup positions and transforms
	/// <summary>
	/// The temporary z layer while the coverup is being moved.
	/// </summary>
	float coverupTmpZLayer;
	/// <summary>
	/// The position we reset coverup to before we do transforms so that the transforms will work properly
	/// </summary>
	private Vector3 coverupOriginalPosition;
	/// <summary>
	/// The starting position of coverup when the program first starts.
	/// </summary>
	private Vector3 coverupStartingPosition;
	/// <summary>
	/// The original and starting rotation of coverup
	/// </summary>
	private Quaternion coverupOriginalRotation;
	/// <summary>
	/// The position the coverup will unfold to if it is an invalid fold
	/// </summary>
	Vector3 coverupUnfoldPosition;
	/// <summary>
	/// position the smooth damp is currently at. Used for the transforms of the smooth damp to unfold properly.
	/// </summary>
	Vector3 coverupSmoothPosition;
	/// <summary>
	/// The last valid finger position for coverup if we need to unfold back to a previous fold
	/// </summary>
	Vector3 coverupLastValidPos;
	
	#endregion
	
	#region booleans
	
	/// <summary>
	/// true if the first touch has happened to start a fold, false if the there is no touch or if the touch has ended.
	/// </summary>
	public bool firstTouch = false;

    /// <summary>
    /// Added to ensure no two mechs are
    /// performed at same time - D.A.
    /// </summary>
    public bool currentlyFolding = false;

	/// <summary>
	/// True if the right mouse button is down, false if released
	/// </summary>
	public bool currMouseState;
	/// <summary>
	/// True if in the last tick currMouseState is true, false if currMouseState was false.
	/// </summary>
	public bool prevMousestate;
	/// <summary>
	/// True if the paper is currently folded, false if not.
	/// </summary>
	public bool isFolded;
	/// <summary>
	/// True if the fold is off the background, false if not.
	/// </summary>
	public bool isOffPaper;
	/// <summary>
	/// True if the paper is over the player, false if not.
	/// </summary>
	public bool overPlayer;
	/// <summary>
	/// true if the paper needs to unfold, false if not
	/// </summary>
	public bool needsToUnfold;
	public bool isUnfoldingOnPaper;
	public bool backsideIsInstantiated;
	public GameObject origBackground;
	private int[] origBackTri;
	private int[] deletedTri;
	public bool justUnfolded;
	bool firstFoldAfterTear = false;
	Color backSideInitialColor;
	Color tearInitialColor;
	Vector2 currentScreenSize = new Vector2();
	Vector2 verticalLimitPos, horizontalLimitPos;
	bool guiEnable;
	bool right;
	bool top;
	public bool foldInput;
	public bool prevFoldInput;
	#endregion
	
	#endregion
	
	// Use this for initialization
	void Start ()
	{
		#region initialization
        // NOTICE : DOM
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
        gameStateManagerRef = mainObject.GetComponent<GameStateManager>();
        gameStateManagerRef.EnsureCoreScriptsAdded();
        gameStateManagerRef.EnsureScriptAdded("TouchController");
        soundManagerRef = mainObject.GetComponent<SoundManager>();
		screenManagerRef = mainObject.GetComponent<ScreenManager>();

		
		
		
		#region initialize references
		
		backsidePivotReference = GameObject.Find("backsidepivot");
		coverupPivotReference = GameObject.Find ("coveruppivot");
		tornBacksidePieceReference = GameObject.Find ("tornBacksidePiece");
		//sets the reference of the touchcontroller to the touchcontroller script.
        touchController = mainObject.GetComponent<TouchController>();
		//sets the reference of the backsidesideReference to the backside or "fold"
		backsideReference = backsidePivotReference.transform.FindChild("backside").gameObject;
		
		backSideInitialColor = backsideReference.GetComponent<MeshRenderer>().material.color;
		
		//sets the reference of the backsideCollisionReference to the platforms on the back of the paper.
		backsideCollisionReference = GameObject.FindGameObjectsWithTag("FoldPlatform");
		//sets the camera reference to the main camera
		cameraReference = GameObject.Find("Main Camera");
		//sets the player reference to the player
		playerReference = GameObject.Find("Player_Prefab");
		//sets the backgroundTransform to the transform of the background paper.
		origBackground = GameObject.FindGameObjectWithTag("background");
		backgroundTransform = origBackground.transform;
		//sets backgroundBounds to the bounds of the background paper
		backgroundBounds = backgroundTransform.GetComponent<MeshFilter>().mesh.bounds;
		//sets changeMeshScript to the ChangeMeshScript which removes and restores triangles.
		changeMeshScript = this.GetComponent<ChangeMeshScript>();
		tearReference = GameObject.Find("Tear_Manager").GetComponent<TearManager>();
		coverupReference = coverupPivotReference.transform.FindChild("coverup").gameObject;
		tearPaperMesh = GameObject.Find("backside").GetComponent<MeshFilter>().mesh;
		unfoldCollisionReference = GameObject.Find("Player_Prefab").GetComponent<UnfoldCollision>();
		shadowReference = GameObject.Find("shadow");
		rayTraceBlockRef = GameObject.Find("rayTraceBlocker");
		paperBorderInsideRef = GameObject.Find("paper_border_inside");
		paperBorderOutsideRef = GameObject.Find("paper_border_outside");
		worldCollisionRef = mainObject.GetComponent<WorldCollision>();
		backsideTriangles = tearPaperMesh.triangles;
		#endregion
		
		//sets original position and rotation to its starting position and rotation
		foldOriginalRotation = backsidePivotReference.transform.rotation;
		foldOriginalPosition = backsidePivotReference.transform.position;
		
		//sets starting position coverup's starting position
		coverupStartingPosition = coverupPivotReference.transform.position;
		//sets coverup's original position to the vector required for the tranforms to work properly
		coverupOriginalPosition = new Vector3(0,0,-3);
		//sets coverup's original rotation to its starting rotation
		coverupOriginalRotation = coverupPivotReference.transform.rotation;
		
		coverupPrefab = coverupPivotReference.transform;
		foldPrefab = backsidePivotReference.transform;
		
		
		//initializes variables to defaults.
		fingerList = new List<Vector2>();
		backgroundObjMax = new Vector2();
		backgroundObjMin = new Vector2();
		posModifier = new Vector3();
		posModLastValid = new Vector3();
		unfoldPosModifier = new Vector3();
		foldTmpZLayer = GVariables.zFoldLayer - 1;
		coverupTmpZLayer = GVariables.zCoverLayer -1;
		prevMousestate = false;
		currMouseState = false;
		firstTouch = false;
		isFolded = false;
		overPlayer = false;
		needsToUnfold = false;
		isOffPaper = true;
		backsideIsInstantiated = false;
		currentlyFolding = false;
		missingTriangles = new List<Vector3>();
		//changeMeshScript.GrabUpdatedPlatforms("FoldPlatform");
		deletedTri = new int[0];
		startingQuadrant = Quadrant.NONE;
		currentQuadrant = Quadrant.NONE;
		foldEdge = Edge.BuildManifoldEdges(backsideReference.GetComponent<MeshFilter>().mesh);
		guiEnable = false;
		blah = false;
		
		foldInput = false;
		prevFoldInput = false;
		#endregion
	}
	
	public void ResetFold()
	{
//		UnityEngine.Debug.Log("STUFF");
		changeMeshScript.RevertWorld();
//		GameObject[] tempArray = GameObject.FindGameObjectsWithTag("background");
//		foreach(GameObject g in tempArray)
//		{
//			if(origBackground.Equals(g))
//			{
////				origBackground.GetComponent<MeshRenderer>().enabled = true;
////				origBackground.GetComponent<MeshCollider>().enabled = true;
////				g.SetActive(true);
//			}
//			else
//			{
//				//g.transform.position = gameObject.transform.position;
//			//	g.SetActive(false);
//			}
//		}
		if(tearReference.MainWorldCutPaper != null)
		{
			tearReference.MainWorldCutPaper.transform.position = origBackground.transform.position;
			tearReference.MainWorldCutPaper.transform.rotation = origBackground.transform.rotation;
		}
		prevMousestate = false;
		currMouseState = false;
		firstTouch = false;
		isFolded = false;
		overPlayer = false;
		needsToUnfold = false;
		isOffPaper = true;
		backsideIsInstantiated = false;
		missingTriangles = new List<Vector3>();
		firstFoldAfterTear = false;
		//changeMeshScript.GrabUpdatedPlatforms("FoldPlatform");
		deletedTri = new int[0];
		backsideIsInstantiated = false;
		currentlyFolding = false;
		GVariables.FoldPieceCoveringTear = false;
		GVariables.TearPieceCoveringFold = false;
		backsidePivotReference.transform.position = foldOriginalPosition;
		backsidePivotReference.transform.rotation = foldOriginalRotation;
		coverupPivotReference.transform.position = coverupStartingPosition;
		coverupPivotReference.transform.rotation = coverupOriginalRotation;
		
		//tearReference.DeathReset();
		backsideReference.GetComponent<MeshCollider>().sharedMesh = origBackground.GetComponent<MeshFilter>().mesh;
		backsideReference.GetComponent<MeshFilter>().mesh =  origBackground.GetComponent<MeshFilter>().mesh;
		backsideTriangles = backsideReference.GetComponent<MeshFilter>().mesh.triangles;
		
		shadowReference.GetComponent<MeshCollider>().sharedMesh = backsideReference.GetComponent<MeshFilter>().mesh;
		rayTraceBlockRef.GetComponent<MeshCollider>().sharedMesh =  backsideReference.GetComponent<MeshFilter>().mesh;
		shadowReference.GetComponent<MeshFilter>().mesh =  backsideReference.GetComponent<MeshFilter>().mesh;
		rayTraceBlockRef.GetComponent<MeshFilter>().mesh =  backsideReference.GetComponent<MeshFilter>().mesh;
		paperBorderInsideRef.GetComponent<MeshCollider>().sharedMesh =  backsideReference.GetComponent<MeshFilter>().mesh;
		paperBorderInsideRef.GetComponent<MeshFilter>().mesh =  backsideReference.GetComponent<MeshFilter>().mesh;
		paperBorderOutsideRef.GetComponent<MeshCollider>().sharedMesh =  backsideReference.GetComponent<MeshFilter>().mesh;
		paperBorderOutsideRef.GetComponent<MeshFilter>().mesh =  backsideReference.GetComponent<MeshFilter>().mesh;
		
		startingQuadrant = Quadrant.NONE;
		currentQuadrant = Quadrant.NONE;
		foldEdge = Edge.BuildManifoldEdges(backsideReference.GetComponent<MeshFilter>().mesh);
		unfoldCollisionReference.restart();
		blah = false;
		foldInput = false;
		prevFoldInput = false;
		
		
	}
	#region update
	// Update is called once per frame
	void Update ()
	{
        // Added by Dom
        // Ensures my own boolean for currently folding
        // is set false accordinly
        if (currentlyFolding &&
            ((!Input.GetMouseButton(1) && !gameStateManagerRef.OnMobileDevice()) ||
             ((gameStateManagerRef.GetInputManager().GetcurrPressState().Equals(InputManager.PressState.UP) ||
             gameStateManagerRef.GetInputManager().GetcurrPressState().Equals(InputManager.PressState.JUST_RELEASED)) &&
             gameStateManagerRef.OnMobileDevice())))
        {
            currentlyFolding = false;
        }
		
		if(Input.GetKey(KeyCode.P) || (gameStateManagerRef.OnMobileDevice())){
			guiEnable = true;
		}
		else{
			guiEnable = false;
		}
//		if (GVariables.TearPieceCoveringFold)
//		{
//			backsideReference.GetComponent<MeshRenderer>().material.color = Color.gray;
//		}
//		else
//		{
//			backsideReference.GetComponent<MeshRenderer>().material.color = backSideInitialColor;
//		}
		
		
		//foldInput = false;
		if (tearReference.HaveTornOnce && !backsideIsInstantiated)
		{
			
			GameObject[] tempBackgroundObjs;
			tornPieceReference = GameObject.Find("paper_CuttPieceOfPaper");
			tempBackgroundObjs = GameObject.FindGameObjectsWithTag("background");
			foreach(GameObject temp in tempBackgroundObjs){
				if(temp.name == "paper_LargerPiece"){
//					missingTriangles = changeMeshScript.DeletePlatformsFromMissingTriangles(backsideTriangles, 
//						temp.GetComponent<MeshFilter>().mesh.triangles, backsideReference.transform, "FoldPlatform");
					tornBackground = temp;
					tearPaperMesh = temp.GetComponent<MeshFilter>().mesh;
				//	changeMeshScript.GrabPlatformsInWorld("FoldPlatform");
				//	changeMeshScript.GrabPlatformsInWorld("Platform");
					
				}
				else if( temp.name == "paper_CuttPieceOfPaper")
				{
						deletedTri = temp.GetComponent<MeshFilter>().mesh.triangles;
				}
			}
			//changeMeshScript.DeletePlatformsFromMissingTriangles(deletedTri,
			//													backsideReference.transform ,"FoldPlatform");
			//changeMeshScript.UpdateAfterTear("FoldPlatform");
			//changeMeshScript.UpdateAfterTear("Platform");
			backsideIsInstantiated = true;
			backsideReference.GetComponent<MeshFilter>().mesh = tearPaperMesh;
			backsideReference.GetComponent<MeshCollider>().sharedMesh = tearPaperMesh;
			shadowReference.GetComponent<MeshCollider>().sharedMesh = tearPaperMesh;
			rayTraceBlockRef.GetComponent<MeshCollider>().sharedMesh = tearPaperMesh;
			shadowReference.GetComponent<MeshFilter>().mesh = tearPaperMesh;
			rayTraceBlockRef.GetComponent<MeshFilter>().mesh = tearPaperMesh;
			
			paperBorderInsideRef.GetComponent<MeshCollider>().sharedMesh = tearPaperMesh;
			paperBorderInsideRef.GetComponent<MeshFilter>().mesh = tearPaperMesh;
			
			paperBorderOutsideRef.GetComponent<MeshCollider>().sharedMesh = tearPaperMesh;
			paperBorderOutsideRef.GetComponent<MeshFilter>().mesh = tearPaperMesh;
			
			backsideTriangles = tearPaperMesh.triangles;
			tornBacksidePieceReference.GetComponent<MeshFilter>().mesh = 
				tornPieceReference.GetComponent<MeshFilter>().mesh;
//			tornBacksidePieceReference.GetComponent<MeshCollider>().sharedMesh = 
//				tornPieceReference.GetComponent<MeshFilter>().mesh;
			tearInitialColor = new Color(0.8f, 0.8f, 0.8f, 1.0f);
			firstFoldAfterTear = true;
			foldEdge = Edge.BuildManifoldEdges(tearPaperMesh);
		//	changeMeshScript.PrintColliderEnabled();
		//	changeMeshScript.ReapplyChanges();
		//	changeMeshScript.PrintColliderEnabled();
		}
		//Try to set logic accordingly when on a tearing level
		try
		{
			if(((Input.GetMouseButton(1) || 
                (gameStateManagerRef.GetInputManager().GetFingerGesture().Equals(InputManager.FingerGesture.FOLD) && touchController.ReturnTouchType() != TouchType.NONE)) && 
                !tearReference.GetComponent<TearManager>().PlayerMovingPlatformState) &&
				( prevFoldInput || (!playerReference.GetComponent<TWCharacterController>().qDown
				&& !playerReference.GetComponent<TWCharacterController>().eDown
				&& !playerReference.GetComponent<TWCharacterController>().getFalling() )))
			{
				foldInput = true;
			}
			
			else
			{
				foldInput = false;
					
				
			}
			
		}
		catch
		{
			//This logic will be hit on NON - Tearing levels
			if(Input.GetMouseButton(1))
			{
				foldInput = true;
			}
		}
		
		//gets the current touch type from the touch controller
		touchType = touchController.ReturnTouchType();
		//gets the current finger positions from the touch controller
		fingerList = touchController.GetFingerPositions();
		
		//initializes three new vectors, one for each of the two fingers needed for folding, 
		//and one for the average finger position.
		Vector3 fingerPosition1 = new Vector3();
		Vector3 fingerPosition2 = new Vector3();
		avgFingerPos = new Vector3();
		
		//if there are 2 fingers on the screen, 
		//set the finger positions to the corresponding world positions, 
		//then finds the average of the two positions. 
		/*if(fingerList.Count == 2)
		{
			fingerPosition1 = Camera.main.ScreenToWorldPoint(fingerList[0]);
			fingerPosition2 = Camera.main.ScreenToWorldPoint(fingerList[1]);
			avgFingerPos.x = (fingerPosition1.x+fingerPosition2.x)/2;
			avgFingerPos.y = (fingerPosition1.y+fingerPosition2.y)/2;
			avgFingerPos.z = -1;
			lastFingerPosition = avgFingerPos;
		}*/
		//or if the right mouse button is down, 
		//sets the average finger position to the world coordinates of the mouse position,
		//also sets currMouse state to true, so we know if the mouse is pressed down.
		 if(foldInput){
            avgFingerPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		    avgFingerPos.z = -1;
			currMouseState = true;
			lastFingerPosition = avgFingerPos;
		}
		//if the right mouse button is not pressed, set currMouseState to false, 
		//so we know we arn't using the mouse.
		if(!foldInput){
			currMouseState = false;
		}
		//if we are not touching the screen, and not clicking the right mouse button...
		if(touchType == TouchType.NONE && !foldInput)
		{
			
			//if the last state was a 2 finger touch, or the right mouse button was pressed, and had just been folding,
			if((prevFoldInput || prevMousestate) && firstTouch)
			{
				//set firstTouch back to false so we know we are done folding, and can now unfold or refold
				firstTouch = false;
				currentQuadrant = Quadrant.NONE;
				//if the fold is over the player, unfold.
				if(unfoldCollisionReference.getOverPlayer())
				{
					blah = true;
					needsToUnfold = true;
					foldSmoothPosition = new Vector3(lastFingerPosition.x -origin.x + posModifier.x, lastFingerPosition.y - origin.y + posModifier.y, foldTmpZLayer);
					startDampTime = Time.time;
					coverupSmoothPosition = lastFingerPosition - origin + posModifier;
					coverupSmoothPosition.z = coverupTmpZLayer;
				}
				//if the fold is not over the player...
				else{
					setDownFold();
				
				}
				
			}
		}
		//if there are two fingers on the screen or right mouse button is pressed...
		else if(foldInput && !needsToUnfold)
		{
			//if this is the first touch to start a fold..
			if(!firstTouch)
			{
				HandleScreenChanges();
				if(!unfoldCollisionReference.getOverPlayer() && !GVariables.TearPieceCoveringFold)
				{
					//if the paper is not already folded and the user's fingers are on the fold border...
					if(onFoldBorder(avgFingerPos) && !isFolded)
					{
						//set the position to its original position, so the transforms work properly
						backsidePivotReference.transform.position = foldOriginalPosition;
						foreach(Transform child in transform)
						{
							origin = avgFingerPos;
							origin.z = 0;
						}
						//sets first touch to true so we know we are now folding.
						firstTouch = true;
						
						//brings back and removed triangles in platforms.
						changeMeshScript.RevertChanges();
						//set the unfold position using where the fold started, so we know where to unfold to if the fold is invalid.
						foldUnfoldPosition = findUnfoldPosition(avgFingerPos, unfoldReference) - origin;
						//set the coverup unfold position using where the fold started, so we know where to unfold to if the fold is invalid.
						coverupUnfoldPosition = findUnfoldPosition(avgFingerPos, unfoldReference);
						coverupUnfoldPosition = new Vector3(coverupUnfoldPosition.x - origin.x, coverupUnfoldPosition.y - origin.y, coverupTmpZLayer);
						posModifier = Vector3.zero;
						firstTouchPosition = avgFingerPos;
						justUnfolded = false;
						startingQuadrant = ReturnSidePressed(avgFingerPos);
						currentQuadrant = startingQuadrant;
						
					}
					//else if the paper is folded, and the user's fingers are on the edge of the fold...
					else if(onUnfoldBorder(avgFingerPos) && isFolded)
					{
						isUnfoldingOnPaper = true;
						//sets first touch to true so we know we are folding.
						firstTouch = true;
						//brings back and removed triangles in platforms.
						changeMeshScript.RevertChanges();
						//sets the unfold position to the last valid position (the last fold position) so it will unfold to the proper position if the fold is invalid
						foldUnfoldPosition = foldLastValidPos;
						//sets the unfold position to the last valid position (the last fold position) so it will unfold to the proper position if the fold is invalid
						coverupUnfoldPosition = coverupLastValidPos;
						posModifier = findPositionOffset();
						justUnfolded = false;
					}
					//gets the position of the user's fingers when they first start the fold.
					
				}
			}

			//if the first touch of the fold already happened and we are folding...
			if(firstTouch && (!gameStateManagerRef.OnMobileDevice()
				|| gameStateManagerRef.GetInputManager().GetFingerGesture().Equals(InputManager.FingerGesture.FOLD)))
			{
				//sets the finger position as the average finger position offset by the origin,
				//so we can get a position relative to the origin of the fold.
				currentQuadrant = ReturnSidePressed(avgFingerPos);
				fingerPos = avgFingerPos - origin+ posModifier - unfoldPosModifier;
				
				//if the finger positions are 0, return becuase it can mess up calculations
				if (fingerPos.x == 0 && fingerPos.y == 0)
				{
					return;
				}
				
				//if the user's fingers are on the paper background, do the transforms normally, 
				//and set the isOffPaper bool to false so we know it is on the paper
				//float distance = Mathf.Sqrt(Mathf.Pow((firstTouchPosition.x - avgFingerPos.x),2f)+Mathf.Pow((firstTouchPosition.y-avgFingerPos.y),2f));
				
				if(!offPaper(avgFingerPos) || currentQuadrant != startingQuadrant){
                    currentlyFolding = true;
					DoFoldTransforms();
					DoCoverupTransforms();
					isOffPaper = false;
                    //if (!soundManagerRef.IsAudioPlaying("paperFold1", "SFX"))
                    //{
                    //    soundManagerRef.PlayAudio("paperFold1", "SFX");
                    //}
                    //else soundManagerRef.StopSound("paperFold1", "SFX");
				}
				//else if te user's fingers are off the paper, set the position and rotation back to defaults, 
				//to give the illusion of completely unfolding. Also sets the isOffPaper bool to true, 
				//so we know they are off the paper.
				else{
                    currentlyFolding = false;
					isUnfoldingOnPaper = false;
					backsidePivotReference.transform.position = foldOriginalPosition;
					backsidePivotReference.transform.rotation = foldOriginalRotation;
					coverupPivotReference.transform.position = coverupStartingPosition;
					coverupPivotReference.transform.rotation = coverupOriginalRotation;
					isOffPaper = true;
					isFolded = false;
					firstTouch = false;
					//prevTouchType = TouchType.NONE;
					prevMousestate = false;
					currentQuadrant = Quadrant.NONE;
					prevFoldInput = false;
					
				}
			}
		}
//		Debug.Log("FoldSmooth: " + foldSmoothPosition);
//		Debug.Log("FoldUnfold: " + foldUnfoldPosition);
		//If we need to unfold the paper...
		if(needsToUnfold)
		{
			//use smooth damp to get a smooth unfold for fold.
			foldSmoothPosition = Vector3.SmoothDamp(foldSmoothPosition, foldUnfoldPosition, ref velocity, .3f);
			
			
			//Do the transforms on the smooth position, which is the DoFoldTransforms function without the rotations
			backsidePivotReference.transform.position = (new Vector3(foldSmoothPosition.x + origin.x, foldSmoothPosition.y+ origin.y, 0));
			
			//use smooth damp to get a smooth unfold for coverup.
			coverupSmoothPosition = Vector3.SmoothDamp(coverupSmoothPosition, coverupUnfoldPosition, ref velocity, .3f);
			
			//Do the transforms on the smooth position for coverup
			coverupPivotReference.transform.position = coverupOriginalPosition;
			coverupPivotReference.transform.position = new Vector3(origin.x + (coverupSmoothPosition.x)/2, origin.y + (coverupSmoothPosition.y)/2, coverupTmpZLayer);
			
//			if (isUnfoldingOnPaper)
//			{
//				backsidePivotReference.transform.rotation = foldOriginalRotation;
//				float angleInDegrees = Mathf.Rad2Deg * (2 * Mathf.Atan2(foldSmoothPosition.y, foldSmoothPosition.x));
//				backsidePivotReference.transform.Rotate(Vector3.forward, angleInDegrees);
//				
//				coverupPivotReference.transform.rotation = coverupOriginalRotation;
//				angleInDegrees = Mathf.Rad2Deg * Mathf.Atan2(coverupSmoothPosition.y, coverupSmoothPosition.x);
//				coverupPivotReference.transform.Rotate(Vector3.forward, angleInDegrees);
//			}
			
			backsidePivotReference.transform.Translate(new Vector3(0, 0, 0));
			backsidePivotReference.transform.Translate (new Vector3(origin.x, -origin.y, foldTmpZLayer));
			
			
			// do the transformations on the smooth rotation for backside and coverup
			backsidePivotReference.transform.rotation = foldOriginalRotation;
			float angleInDegrees2 = Mathf.Rad2Deg * (2 * Mathf.Atan2(foldSmoothPosition.y, foldSmoothPosition.x));
			backsidePivotReference.transform.Rotate(Vector3.forward, angleInDegrees2);
			
			coverupPivotReference.transform.rotation = coverupOriginalRotation;
			angleInDegrees2 = Mathf.Rad2Deg * Mathf.Atan2(coverupSmoothPosition.y, coverupSmoothPosition.x);
			coverupPivotReference.transform.Rotate(Vector3.forward, angleInDegrees2);
			
			//if the smooth position is off the paper, set needsToUnfold to false so we know we no longer need to unfold, 
			//and set the position and rotation back to dafault positions. since it is off the paper, set isFolded to false.
			//if(foldSmoothPosition.x <= -6 -origin.x ||foldSmoothPosition.x >= 6 -origin.x || foldSmoothPosition.y <=-4 -origin.y|| foldSmoothPosition.y >=4-origin.y){
			//float dist = Mathf.Sqrt(Mathf.Pow((foldUnfoldPosition.x - foldSmoothPosition.x),2f)+Mathf.Pow((foldUnfoldPosition.y-foldSmoothPosition.y),2f));
			
			if(offPaper(foldSmoothPosition + origin) && currentQuadrant == startingQuadrant){
				needsToUnfold = false;
				backsidePivotReference.transform.position = foldOriginalPosition;
				backsidePivotReference.transform.rotation = foldOriginalRotation;
				
				coverupPivotReference.transform.position = coverupStartingPosition;
				coverupPivotReference.transform.rotation = coverupOriginalRotation;
				isFolded = false;
				isUnfoldingOnPaper = false;
				justUnfolded = true;
			}
			
			
			//if the smooth damp doesn't bring it off the paper (bringing back to a folded position), have the smooth damp end when the time 
			//runs out. Check if the fold is on the paper or not so we know if it is folded.
			
			else if(Time.time - startDampTime > 0.8F){
				isUnfoldingOnPaper = false;
				needsToUnfold = false;
				justUnfolded = true;
				Vector3 tempVec = foldSmoothPosition + origin;
				//Debug.Log("When is this called?");
				if(!offPaper(tempVec)){
					isOffPaper = false;
					lastFingerPosition = tempVec;
					setDownFold();
					
				}
				else{
					//Debug.Log("This one too?");
					// failsafe for if/when the fold never completely makes it back off the paper
					backsidePivotReference.transform.position = foldOriginalPosition;
					backsidePivotReference.transform.rotation = foldOriginalRotation;
					coverupPivotReference.transform.position = coverupStartingPosition;
					coverupPivotReference.transform.rotation = coverupOriginalRotation;
					isOffPaper = true;
					isFolded = false;
					
				}
			}
		}
		//sets the previous touch and mouse states to the current ones.
		//
		// PLEASE DO NOT TOUCH TORN PIECE MATERIAL HERE -> J.C.
		//
		//prevTouchType = touchType;
		prevMousestate = currMouseState;
		prevFoldInput = foldInput;
		if(unfoldCollisionReference.getOverPlayer()){
			backsideReference.GetComponent<MeshRenderer>().material.color = Color.gray;
		}
		else if(tearReference.HaveTornOnce){
			if(GVariables.TearPieceCoveringFold){
				//tornPieceReference.GetComponent<MeshRenderer>().material.color = tearInitialColor;
				backsideReference.GetComponent<MeshRenderer>().material.color = Color.gray;
			}
			else if(GVariables.FoldPieceCoveringTear){
				backsideReference.GetComponent<MeshRenderer>().material.color = backSideInitialColor;
				//tornPieceReference.GetComponent<MeshRenderer>().material.color = Color.gray;
			}
			else{
				//tornPieceReference.GetComponent<MeshRenderer>().material.color = tearInitialColor;
				backsideReference.GetComponent<MeshRenderer>().material.color = backSideInitialColor;
			}
			
		}
		else{
			backsideReference.GetComponent<MeshRenderer>().material.color = backSideInitialColor;
		}
		
		
	}
	#endregion
	
	#region DoFoldTransforms
	/// <summary>
	/// Does the fold transforms, to get the proper position and rotation the fold needs to be at to look right
	/// </summary>

	void DoFoldTransforms()
	{
		backsidePivotReference.transform.rotation = foldOriginalRotation;
		float angleInDegrees = Mathf.Rad2Deg * (2 * Mathf.Atan2(fingerPos.y, fingerPos.x));
		backsidePivotReference.transform.position = (new Vector3(fingerPos.x + origin.x, fingerPos.y+ origin.y, 0));
		backsidePivotReference.transform.Rotate(Vector3.forward, angleInDegrees);
		backsidePivotReference.transform.Translate(new Vector3(0, 0, 0));
		backsidePivotReference.transform.Translate (new Vector3(origin.x, -origin.y, foldTmpZLayer));
			
	}
	#endregion
	
	#region DoCoverupTransforms
	/// <summary>
	/// Does the coverup transforms, to get coverup in the proper position and rotation to look right
	/// </summary>
	void DoCoverupTransforms(){
		coverupPivotReference.transform.position = coverupOriginalPosition;
		coverupPivotReference.transform.rotation = coverupOriginalRotation;
		float angleInDegrees = Mathf.Rad2Deg * Mathf.Atan2(fingerPos.y, fingerPos.x);
		coverupPivotReference.transform.position = new Vector3(origin.x + fingerPos.x/2, origin.y + fingerPos.y/2, coverupTmpZLayer);
		coverupPivotReference.transform.Rotate(Vector3.forward, angleInDegrees);
	}
	#endregion
	
	void setDownFold()
	{
		if(firstFoldAfterTear)
		{
			changeMeshScript.DeletePlatformsFromMissingTriangles(deletedTri,
																backsideReference.transform ,"FoldPlatform");
			firstFoldAfterTear = false;
		}
		//changeWithSmallerMeshSquare(coverupReference.transform, "Fold");
		//delete the triangles of covered platforms
		changeMeshScript.ChangeMeshWithSquareIgnoreMissingTriangles(backsideReference.transform, "Platform", deletedTri);
		//delete the triangles of plaforms covered by coverup
		changeMeshScript.ChangeWithBasicMeshSquare(coverupReference.transform, "Platform");
		//changeMeshScript.ChangeWithBasicMeshSquare(coverupReference.transform, "FoldPlatform");
//		if (tearReference.HaveTornOnce)
//		{
//			changeMeshScript.ChangeMeshWithTriangles(tornBacksidePieceReference.transform, "FoldPlatform");
//		}
		Mesh temp = backsideReference.GetComponent<MeshFilter>().sharedMesh;
		temp.triangles = backsideTriangles;
		backsideReference.GetComponent<MeshFilter>().sharedMesh = null;
		backsideReference.GetComponent<MeshFilter>().sharedMesh = temp;
		backsideReference.GetComponent<MeshFilter>().sharedMesh.RecalculateBounds();
		backsideReference.GetComponent<MeshFilter>().sharedMesh.RecalculateNormals();
		//set coverup's z depth to right above the fold
		coverupPivotReference.transform.position = new Vector3(coverupPivotReference.transform.position.x, coverupPivotReference.transform.position.y, GVariables.zCoverLayer);
		//set its z depth to right above the paper
		if(tearReference.HaveTornOnce){
			//fold over tear	
			if(tearReference.CheckTornPieceCoveringFold() && tearReference.MainWorldCutPaper.transform.position.z > backsidePivotReference.transform.position.z){
				backsidePivotReference.transform.position = new Vector3(backsidePivotReference.transform.position.x, backsidePivotReference.transform.position.y, GVariables.OnTopDepth);
				GVariables.FoldPieceCoveringTear = true;
				GVariables.TearPieceCoveringFold = false;
				//backsideReference.GetComponent<MeshRenderer>().material.color = backSideInitialColor;
				//tornPieceReference.GetComponent<MeshRenderer>().material.color = Color.gray;
			}
			//tear over fold
			else if(tearReference.CheckTornPieceCoveringFold() && tearReference.MainWorldCutPaper.transform.position.z < backsidePivotReference.transform.position.z){
				backsidePivotReference.transform.position = new Vector3(backsidePivotReference.transform.position.x, backsidePivotReference.transform.position.y, GVariables.OnBottomDepth);
				GVariables.TearPieceCoveringFold = true;
				GVariables.FoldPieceCoveringTear = false;
			}
			else{
				backsidePivotReference.transform.position = new Vector3(backsidePivotReference.transform.position.x, backsidePivotReference.transform.position.y, GVariables.OnBottomDepth);
				GVariables.FoldPieceCoveringTear = false;
				GVariables.TearPieceCoveringFold = false;
				//backsideReference.GetComponent<MeshRenderer>().material.color = backSideInitialColor;
				//tornPieceReference.GetComponent<MeshRenderer>().material.color = tearInitialColor;
			}
		}
		else{
			backsidePivotReference.transform.position = new Vector3(backsidePivotReference.transform.position.x, backsidePivotReference.transform.position.y, GVariables.OnBottomDepth);
		}
		//move the platforms on the backside of the paper to their new posistions to their correspoding positions where fold moved to. 
		foreach(GameObject iterate in backsideCollisionReference)
		{
			iterate.transform.position = new Vector3(iterate.transform.position.x, iterate.transform.position.y, 0f);
		}
		//if the fold if on the paper, set isFolded to true so we know, else false
		
		
		
		/*if(!isOffPaper){
			isFolded = true;
			if(!justUnfolded){
				coverupLastValidPos = lastFingerPosition - origin + posModifier - unfoldPosModifier;
				foldLastValidPos = lastFingerPosition - origin + posModifier - unfoldPosModifier;
			}
			else{
				justUnfolded = false;
			}
			
		}*/
		if(completelyOffPaper()){
			isFolded = false;
			isOffPaper = true;
		}
		else{
			isFolded = true;
			
			if(!justUnfolded){
				coverupLastValidPos = lastFingerPosition - origin + posModifier - unfoldPosModifier;
				foldLastValidPos = lastFingerPosition - origin + posModifier - unfoldPosModifier;
			}
			else{
				justUnfolded = false;
			}
		}
		
	}
	#region onFoldBorder
	/// <summary>
	/// Checks if a position is on the fold border (border around the backgrund paper) or on the desk.
	/// </summary>
	/// <returns>
	/// true if position is on the fold border or desk, false if not.
	/// </returns>
	/// <param name='raytrace'>
	/// If set to <c>true</c> raytrace.
	/// </param>
	public bool onFoldBorder(Vector3 raytrace)
	{
		/*List<GameObject> objThatGotHit = new List<GameObject>();
		RaycastHit hit;
		Vector3 dir = cameraReference.transform.TransformDirection(Vector3.forward);
		//does a raytrace from the passed in position forward from the camera to see if it hits the foldborder or desk
		if(Physics.Raycast(raytrace, dir, out hit,Mathf.Infinity ))
		{
			//it only cares aobut a raycast if the object is a type "foldborder" or type "DeadSpace"
			if(hit.transform.gameObject.tag=="foldborder" || hit.transform.gameObject.tag == "DeadSpace")
			{
				unfoldReference = hit.transform.gameObject;
				return true;

			}
		}
		return false;*/
		
		if(paperBorderOutsideRef.collider.bounds.Contains(new Vector3(raytrace.x, raytrace.y, paperBorderOutsideRef.collider.bounds.center.z)) && 
			!paperBorderInsideRef.collider.bounds.Contains(new Vector3(raytrace.x, raytrace.y, paperBorderInsideRef.collider.bounds.center.z))){
				unfoldReference = paperBorderOutsideRef;
				return true;
		}
		else{
			return false;
		}
	}
	#endregion
	
	/// <summary>
	/// Checks if a position is on the unfold border (border along the backside)
	/// </summary>
	/// <returns>
	/// true if position is on the unfold border, false if not.
	/// </returns>
	/// <param name='raytrace'>
	/// If set to <c>true</c> raytrace.
	/// </param>
	#region onUnfoldBorder
	public bool onUnfoldBorder(Vector3 raytrace)
	{
		/*List<GameObject> objThatGotHit = new List<GameObject>();
		RaycastHit hit;
		Vector3 dir = cameraReference.transform.TransformDirection(Vector3.forward);
		//does a raytrace from the passed in position forward from the camera to see if it hits the unfoldborder
		if(Physics.Raycast(raytrace, dir, out hit,Mathf.Infinity ))
		{
			//it only cares aobut a raycast if the object is a type "unfoldborder" 
			if(hit.transform.gameObject.tag=="unfoldborder")
			{
				unfoldReference = hit.transform.gameObject;
				return true;
			}
		}
		return false;*/
		if(shadowReference.collider.bounds.Contains(new Vector3(raytrace.x, raytrace.y, shadowReference.collider.bounds.center.z)) && 
			!rayTraceBlockRef.collider.bounds.Contains(new Vector3(raytrace.x, raytrace.y, rayTraceBlockRef.collider.bounds.center.z))){
				unfoldReference =  rayTraceBlockRef;
				return true;
		}
		else{
			return false;
		}
		
	}
	#endregion
	
	/// <summary>
	/// Checks if a position is off the background paper or not.
	/// </summary>
	/// <returns>
	/// true if position is off the background paper, false otherwise
	/// </returns>
	/// <param name='fingerPos'>
	/// If set to <c>true</c> finger position.
	/// </param>
	#region offPaper
	public bool offPaper(Vector3 fingerPos)
	{
		if (!tearReference.HaveTornOnce){
			//gets the bounds of the background paper based on the rotation.
			if(backgroundTransform.rotation.y == Quaternion.Euler(0f, 180f,0f).y)
			{
				backgroundObjMax.x = backgroundTransform.TransformPoint((backgroundTransform.localPosition + (backgroundTransform.localScale.x * backgroundBounds.min))).x;
	   			backgroundObjMin.x = backgroundTransform.TransformPoint((backgroundTransform.localPosition + (backgroundTransform.localScale.x * backgroundBounds.max))).x;
			}
			else if(backgroundTransform.rotation.y == Quaternion.Euler(0f, 0f,0f).y)
			{
				backgroundObjMax.x = backgroundTransform.TransformPoint((backgroundTransform.localPosition + (backgroundTransform.localScale.x * backgroundBounds.max))).x;
	   			backgroundObjMin.x = backgroundTransform.TransformPoint((backgroundTransform.localPosition + (backgroundTransform.localScale.x * backgroundBounds.min))).x;
			}
			backgroundObjMax.y = backgroundTransform.TransformPoint((backgroundTransform.localPosition + (backgroundTransform.localScale.y * backgroundBounds.max))).y;
			backgroundObjMin.y = backgroundTransform.TransformPoint((backgroundTransform.localPosition + (backgroundTransform.localScale.y * backgroundBounds.min))).y;
			
			//uses the bounds of the background paper to see if the passed in position lies outside of it.
			if((fingerPos.x > backgroundObjMax.x || fingerPos.x < backgroundObjMin.x)||(fingerPos.y > backgroundObjMax.y || fingerPos.y < backgroundObjMin.y)){
				return true;
			}
			else{
				return false;
			}
		}
		else{
			
			if(tornBackground.collider.bounds.Contains(new Vector3(fingerPos.x, fingerPos.y, tornBackground.collider.bounds.center.z)) || paperBorderOutsideRef.collider.bounds.Contains(new Vector3(fingerPos.x, fingerPos.y, paperBorderOutsideRef.collider.bounds.center.z))){
				
				return false;
			}
			else{
				return true;
			}
			/*RaycastHit hit;
			Vector3 dir = cameraReference.transform.TransformDirection(Vector3.forward);
			//does a raytrace from the passed in position forward from the camera to see if it hits the foldborder or desk
			if(Physics.Raycast(fingerPos, dir, out hit,Mathf.Infinity ))
			{
				//it only cares aobut a raycast if the object is a type "foldborder" or type "DeadSpace"
				if(hit.transform.gameObject.tag == "background" || hit.transform.gameObject.name == "paper_border_inside" || hit.transform.gameObject.name == "paper_border_outside")
				{	
					return false;
				}
			}
			return true;*/
		}
	}
	#endregion
	
	/// <summary>
	/// Finds the position the paper should be unfolding to
	/// </summary>
	/// <returns>
	/// The unfold position.
	/// </returns>
	/// <param name='fingerPos'>
	/// Finger position.
	/// </param>
	/// <param name='unfoldRef'>
	/// The reference to the object from which we calulate where to unfold to. Depends on where the fold started
	/// </param>
	public Vector3 findUnfoldPosition(Vector3 fingerPos, GameObject unfoldRef){
		//gets the bounds of the unfold reference
		Bounds unfoldRefBounds = unfoldRef.transform.GetComponent<Renderer>().renderer.bounds;
		Bounds unfoldInsideRefBounds = paperBorderInsideRef.transform.renderer.bounds;
		
		//if the fold started on the paper border
		if(unfoldRef.name == "paper_border_outside"){
			Vector3 center = backgroundTransform.collider.bounds.center;
			Vector3 directionVec = center-fingerPos;
			directionVec.Normalize();
			Vector3 unfoldPos = new Vector3(fingerPos.x - directionVec.x, fingerPos.y - directionVec.y, foldTmpZLayer);
			unfoldPos = unfoldPos*1.1f;
			
			

			unfoldPosModifier = unfoldPos - fingerPos;
			origin = unfoldPos;
			
			return unfoldPos;
		}
		//if the fold started on the desk off the paper
		else if(unfoldRef.name == "Desk"){
			unfoldPosModifier = Vector3.zero;
			return new Vector3(fingerPos.x, fingerPos.y, foldTmpZLayer);
		}
		//if none of these, have unfold to the original position
		else{
			unfoldPosModifier = Vector3.zero;
			return foldOriginalPosition;
		}
	}
	
	Vector3 findPositionOffset(){
		posModLastValid = foldLastValidPos + origin;
		Vector3 directionVec = posModLastValid - avgFingerPos;
		float dist = Mathf.Sqrt(Mathf.Pow((posModLastValid.x - avgFingerPos.x),2f)+Mathf.Pow((posModLastValid.y-avgFingerPos.y),2f));
		directionVec.Normalize();
		directionVec.z = 0f;
		unfoldPosModifier = Vector3.zero;
		return (directionVec*dist);
	}
	
	private bool FoldOverTornPiece()
	{
		tearReference.MainWorldCutPaper.GetComponent<MeshCollider>().enabled = false;
		for(int itor = 0; itor < tearReference.MainWorldCutPaper.GetComponent<MeshFilter>().mesh.triangles.Length; itor++)
		{
			RaycastHit hit;
			Vector3 rayPos = tearReference.MainWorldCutPaper.transform.TransformPoint(tearReference.MainWorldCutPaper.GetComponent<MeshFilter>().mesh.vertices[tearReference.MainWorldCutPaper.GetComponent<MeshFilter>().mesh.triangles[itor]]);
			rayPos.z = -10;
			bool hitObject = Physics.Raycast(rayPos, -Camera.main.transform.forward, out hit, 20);
			
			if(hitObject )
			{
				if((hit.collider.gameObject.tag == "unfoldborder" || hit.collider.gameObject.tag == "Fold"  || hit.collider.gameObject.tag == "FoldCover"))
				{
					tearReference.MainWorldCutPaper.GetComponent<MeshCollider>().enabled = true;
					return true;
				}
			}
		}
		
		tearReference.MainWorldCutPaper.GetComponent<MeshCollider>().enabled = true;
		return false;
		
	}
	
	private bool completelyOffPaper()
	{
		
		Vector3[] meshVert = backsideReference.GetComponent<MeshFilter>().mesh.vertices;
		int[] meshTri = calculateFoldEdges();
		int backTriLength = meshTri.Length;
		Vector3 rayPos;
		for(int itor = 0; itor < backTriLength; itor++)
		{
			if(itor < meshTri.Length){
			    rayPos = backsideReference.transform.TransformPoint(meshVert[meshTri[itor]]);
				rayPos.z = backsideReference.transform.position.z;
				if(!offPaper (rayPos)){
					return false;
				}
			}
		}
		return true;
		
	}
	
	 private void SetTouchPositionLimits()
    {
        currentScreenSize = screenManagerRef.GetScreenSize();
        verticalLimitPos = Camera.main.ScreenToWorldPoint(new Vector2(screenManagerRef.GetScreenSize().x, screenManagerRef.GetScreenSize().y / 2));
        horizontalLimitPos = Camera.main.ScreenToWorldPoint(new Vector2(screenManagerRef.GetScreenSize().x / 2, screenManagerRef.GetScreenSize().y));
               
    }

    /// <summary>
    /// Ensures the screen size and resolution are what they
    /// need to be as well as resets touch positions when
    /// the screen is re-oriented
    /// </summary>
    private void HandleScreenChanges()
    {
        if (currentScreenSize != screenManagerRef.GetScreenSize())
        {
            currentScreenSize = screenManagerRef.GetScreenSize();
            SetTouchPositionLimits();
        }

        if (screenManagerRef.DeviceOrientationSwitched)
        {
            SetTouchPositionLimits();
        }
    }
	
	public Quadrant ReturnSidePressed(Vector3 fingerPos)
    {
			
            switch (screenManagerRef.CurrentDeviceOrientation())
            { 
                // landscape left
                default:
                    if (fingerPos.x >= horizontalLimitPos.x)
                        right = true;

                    else
                        right = false;
					if (fingerPos.y <= verticalLimitPos.y)
						top = false;
					else
						top = true;
					break;
				

                case DeviceOrientation.LandscapeRight:
                    if (fingerPos.x >= horizontalLimitPos.x)
                        right = false;

                    else
                        right = true;
					if (fingerPos.y <= verticalLimitPos.y)
						top = true;
					else
						top = false;
					break;

                case DeviceOrientation.PortraitUpsideDown:
                    if (fingerPos.y >= horizontalLimitPos.y)
                        right = false;

                    else
                        right = true;
					if (fingerPos.x <= verticalLimitPos.x)
						top = false;
					else
						top = true;
					break;
                case DeviceOrientation.Portrait:
                    if (fingerPos.y >= horizontalLimitPos.y)
                        right = true;

                    else
                        right = false;
					if (fingerPos.x <= verticalLimitPos.x)
						top = true;
					else
						top = false;
					break;
                
            }
			if(top && right)
				return Quadrant.TOP_RIGHT;
			else if(top && !right)
				return Quadrant.TOP_LEFT;
			else if(!top && right)
				return Quadrant.BOTTOM_RIGHT;
			else if(!top && !right)
				return Quadrant.BOTTOM_LEFT;
			else
				return Quadrant.NONE;
            
    }
	
	/*void OnGUI () {
		if(guiEnable){
			GUI.Label(new Rect (10,10,150,100), "start: " + startingQuadrant.ToString());
			GUI.Label(new Rect (150,10,150,100), "current: " + currentQuadrant.ToString());
			GUI.Label(new Rect (300,10,150,100), "right: " + right.ToString());
			GUI.Label(new Rect (420,10,150,100), "top: " + top.ToString());
			GUI.Label(new Rect (500,10,150,100), "touchtype: " + touchType.ToString());
			GUI.Label(new Rect (650,10,150,100), "FingerGesture: " + gameStateManagerRef.GetInputManager().GetFingerGesture().ToString());
			GUI.Label(new Rect (800,10,150,100), "foldinput: " + foldInput.ToString());
			GUI.Label(new Rect (950,10,150,100), "prevfoldinput: " + prevFoldInput.ToString());
			
			GUI.Label(new Rect (10,100,150,100), "firsttouch: " + firstTouch.ToString());
			GUI.Label(new Rect (10,200,150,100), "currentlyFolding: " + currentlyFolding.ToString());
			GUI.Label(new Rect (10,300,150,100), "isfolded: " + isFolded.ToString());
			GUI.Label(new Rect (10,400,150,100), "overplayer: " + overPlayer.ToString());
			GUI.Label(new Rect (10,500,150,100), "offPaper: " + isOffPaper.ToString());
			GUI.Label(new Rect (10,590,150,100), "needsToUnfold: " + needsToUnfold.ToString());
			GUI.Label(new Rect (200,590,150,100), "UnfoldingOnPaper: " + isUnfoldingOnPaper.ToString());
			GUI.Label(new Rect (400,590,150,100), "justUnfolded: " + justUnfolded.ToString());
			GUI.Label(new Rect (600,590,150,100), "blah: " + blah.ToString());
		}
	}*/
	
	
	int[] calculateFoldEdges(){
		int[] indexArray = new int[foldEdge.Length *2];
		int i = 0;
		foreach(Edge edge in foldEdge){
			indexArray[i] = edge.vertexIndex[0];
			indexArray[i+1] = edge.vertexIndex[1];
			i+=2;
		}
		return indexArray;
	}
}

public class Edge
{
    // The indiex to each vertex
    public int[] vertexIndex = new int[2];
    // The index into the face.
    // (faceindex[0] == faceindex[1] means the edge connects to only one triangle)
    public int[] faceIndex = new int[2];

/// Builds an array of edges that connect to only one triangle.
    /// In other words, the outline of the mesh 
    public static Edge[] BuildManifoldEdges(Mesh mesh)
    {
        // Build a edge list for all unique edges in the mesh
        Edge[] edges = BuildEdges(mesh.vertexCount, mesh.triangles);
 
        // We only want edges that connect to a single triangle
        ArrayList culledEdges = new ArrayList();
        foreach (Edge edge in edges)
        {
            if (edge.faceIndex[0] == edge.faceIndex[1])
            {
                culledEdges.Add(edge);
            }
        }
 
        return culledEdges.ToArray(typeof(Edge)) as Edge[];
    }
 
    /// Builds an array of unique edges
    /// This requires that your mesh has all vertices welded. However on import, Unity has to split
    /// vertices at uv seams and normal seams. Thus for a mesh with seams in your mesh you
    /// will get two edges adjoining one triangle.
    /// Often this is not a problem but you can fix it by welding vertices 
    /// and passing in the triangle array of the welded vertices.
    public static Edge[] BuildEdges(int vertexCount, int[] triangleArray)
    {
        int maxEdgeCount = triangleArray.Length;
        int[] firstEdge = new int[vertexCount + maxEdgeCount];
        int nextEdge = vertexCount;
        int triangleCount = triangleArray.Length / 3;
 
        for (int a = 0; a < vertexCount; a++)
            firstEdge[a] = -1;
 
        // First pass over all triangles. This finds all the edges satisfying the
        // condition that the first vertex index is less than the second vertex index
        // when the direction from the first vertex to the second vertex represents
        // a counterclockwise winding around the triangle to which the edge belongs.
        // For each edge found, the edge index is stored in a linked list of edges
        // belonging to the lower-numbered vertex index i. This allows us to quickly
        // find an edge in the second pass whose higher-numbered vertex index is i.
        Edge[] edgeArray = new Edge[maxEdgeCount];
 
        int edgeCount = 0;
        for (int a = 0; a < triangleCount; a++)
        {
            int i1 = triangleArray[a * 3 + 2];
            for (int b = 0; b < 3; b++)
            {
                int i2 = triangleArray[a * 3 + b];
                if (i1 < i2)
                {
                    Edge newEdge = new Edge();
                    newEdge.vertexIndex[0] = i1;
                    newEdge.vertexIndex[1] = i2;
                    newEdge.faceIndex[0] = a;
                    newEdge.faceIndex[1] = a;
                    edgeArray[edgeCount] = newEdge;
 
                    int edgeIndex = firstEdge[i1];
                    if (edgeIndex == -1)
                    {
                        firstEdge[i1] = edgeCount;
                    }
                    else
                    {
                        while (true)
                        {
                            int index = firstEdge[nextEdge + edgeIndex];
                            if (index == -1)
                            {
                                firstEdge[nextEdge + edgeIndex] = edgeCount;
                                break;
                            }
 
                            edgeIndex = index;
                        }
                    }
 
                    firstEdge[nextEdge + edgeCount] = -1;
                    edgeCount++;
                }
 
                i1 = i2;
            }
        }
 
        // Second pass over all triangles. This finds all the edges satisfying the
        // condition that the first vertex index is greater than the second vertex index
        // when the direction from the first vertex to the second vertex represents
        // a counterclockwise winding around the triangle to which the edge belongs.
        // For each of these edges, the same edge should have already been found in
        // the first pass for a different triangle. Of course we might have edges with only one triangle
        // in that case we just add the edge here
        // So we search the list of edges
        // for the higher-numbered vertex index for the matching edge and fill in the
        // second triangle index. The maximum number of comparisons in this search for
        // any vertex is the number of edges having that vertex as an endpoint.
 
        for (int a = 0; a < triangleCount; a++)
        {
            int i1 = triangleArray[a * 3 + 2];
            for (int b = 0; b < 3; b++)
            {
                int i2 = triangleArray[a * 3 + b];
                if (i1 > i2)
                {
                    bool foundEdge = false;
                    for (int edgeIndex = firstEdge[i2]; edgeIndex != -1; edgeIndex = firstEdge[nextEdge + edgeIndex])
                    {
                        Edge edge = edgeArray[edgeIndex];
                        if ((edge.vertexIndex[1] == i1) && (edge.faceIndex[0] == edge.faceIndex[1]))
                        {
                            edgeArray[edgeIndex].faceIndex[1] = a;
                            foundEdge = true;
                            break;
                        }
                    }
 
                    if (!foundEdge)
                    {
                        Edge newEdge = new Edge();
                        newEdge.vertexIndex[0] = i1;
                        newEdge.vertexIndex[1] = i2;
                        newEdge.faceIndex[0] = a;
                        newEdge.faceIndex[1] = a;
                        edgeArray[edgeCount] = newEdge;
                        edgeCount++;
                    }
                }
 
                i1 = i2;
            }
        }
 
        Edge[] compactedEdges = new Edge[edgeCount];
        for (int e = 0; e < edgeCount; e++)
            compactedEdges[e] = edgeArray[e];
 
        return compactedEdges;
    }
}

 
