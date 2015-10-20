/// <summary>
/// 
/// FILE: 
/// 	Tear Mechanic Manager
/// 
/// DESCRIPTION: 
/// 	This file is used for managing mesh filters and collision during a tear
/// 
/// AUTHOR: 
/// 	John Crocker - jrcrocke@ucsc.edu
/// 
/// DATE: 
/// 	4/12/2013 - ...
/// 
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Diagnostics;
//using UnityEditor;

public class TearManager : MonoBehaviour 
{
	/// <summary>
	/// The torn edge containter.
	/// This is set on a tear comletion
	/// </summary>
	public GameObject TornEdgeFXContainter;


    private SoundManager soundManagerRef;
    /// <summary>
    /// Input manager to get types of player 
    /// input regardless of platform
    /// </summary>
    private InputManager inputManagerRef;

    /// <summary>
    /// Gamestate manager reference for detecting unity remote
    /// </summary>
    private GameStateManager gameStateManagerRef;

    /// <summary>
    /// The touch controller for tablet input
    /// </summary>
    private TouchController touchController;

    /// <summary>
    /// self managed rotation of cut piece on mobile device
    /// </summary>
    private float overallRot = 0;

    /// <summary>
    /// Initial flag for rotating the piece,
    /// helps set the intial look direction
    /// </summary>
    bool initRotatingPiece = false;

    /// <summary>
    /// If the current cut piece is being rotated at any moment
    /// </summary>
    private bool rotatingPiece = false;

    /// <summary>
    /// Player's input regardless of platform,
    /// Use instead of Input. (dot)
    /// </summary>
    private Vector3 currentPlayerInputPos = new Vector3();


	/// <summary>
	/// The rotation audio for when the player is rotationg a torn piece.
	/// </summary>
	public AudioClip RotationAudio;
	
	/// <summary>
	/// The rotation audio for when the player is rotationg a torn piece.
	/// </summary>
	public AudioClip HurtAudio;
	
	/// <summary>
	/// The tear audio.
	/// </summary>
	public AudioClip TearAudio;
	
	/// <summary>
	/// The torn piece currently masking collision flags when a torn
	/// piece covers other collidable objects and masks accordingly
	/// </summary>
	public bool TornPieceCurrentlyMaskingCollision;
	
	/// <summary>
	/// The platforms array stores all tearalbe platforms
	/// </summary>
	private List<GameObject> Platforms;
	
	/// <summary>
	/// The torn platforms represents the 
	/// </summary>
	public List<GameObject> TornPlatforms;
	
	/// <summary>
	/// The main starting world paper used to check UI bounds for
	/// creating tearing 'effect'
	/// </summary>
	public GameObject MainStartingWorldPaper;
	
	/// <summary>
	/// The player shield used to visualize when player trying to cover character
	/// </summary>
	//public GameObject PlayerShield;
	
	/// <summary>
	/// The original platform positions is used to store the position of the 
	/// platforms when the scene starts. This is stored so we can wquickly reset the placement
	/// when level restarts
	/// </summary>
	private Dictionary<GameObject, Vector3> originalPlatformPositions;
	
	/// <summary>
	/// The fold border outside.
	/// </summary>
	public GameObject FoldBorderOutside;
	
	/// <summary>
	/// The fold border inside.
	/// </summary>
	public GameObject FoldBorderInside;
	
	/// <summary>
	/// The original platform topology.
	/// </summary>
	public Dictionary <GameObject, int[]> OriginalPlatformTopology;
	
	/// <summary>
	/// The decal object for tearing visual effect
	/// </summary>
	public GameObject DecalObject;
	
	/// <summary>
	/// The edge decal object for tearing visual effect
	/// </summary>
	public GameObject EdgeDecalObject;
	
	/// <summary>
	/// The tear finished flags the moment a tear is done, triggering
	/// parenting so that translations and roations of torn 
	/// platforms correlated to the world peice they ly on top of
	/// </summary>
	public bool TearFinished = false;
	
	/// <summary>
	/// The main world cut paper.
	/// </summary>
	public GameObject MainWorldCutPaper;
	
	/// <summary>
	/// The main world cut paper.
	/// </summary>
	public int[] MainWorldCutPaperTriangles;
	
	/// <summary>
	/// The main world cut paper.
	/// </summary>
	public Vector3[] MainWorldCutPaperVertices;
	
	/// <summary>
	/// The main world cut paper.
	/// </summary>
	public Vector3 MainWorldCutPaperCenterPos;
	
	/// <summary>
	/// The main world paper (non cut portion after user tear)
	/// </summary>
	public GameObject MainWorldPaper;
	
	/// <summary>
	/// The decal life.
	/// </summary>
	public float DecalLife = 20f;
	
	/// <summary>
	/// The decal shrink speed.
	/// </summary>
	public float DecalShrinkSpeed = 0.005f;
	
	/// <summary>
	/// The bad tear flag thown during bad input
	/// </summary>
	public bool BadTear = false;
	
	/// <summary>
	/// The rotation speed of a torn piece
	/// </summary>
	public int RotationSpeed = 2;
	
	/// <summary>
	/// The center positions of every object n eeding rotation
	/// </summary>
	public Dictionary<GameObject, Vector3> centerPositions;
	
	/// <summary>
	/// The dist check from each tearable object's center to the torn world object,
	/// used for correctly assigning parent child relationships
	/// </summary>
	private Dictionary<GameObject, int> distCheck;
	
	// <summary>
	/// The have checked decal collision stores a bool to each decal object,
	/// this flags when the decal object has been properly checked already
	/// </summary>
	public Dictionary<GameObject, bool> haveCheckedDecalCollision;
	
	/// <summary>
	/// The objects belonging to cut piece (platforms being parented
	/// for one to one translations and rotations
	/// </summary>
	public List<GameObject> objectsBelongingToCutPiece;
	
	/// <summary>
	/// This flags when parent child relations need to be set
	/// </summary>
	private bool needToSetParentChildRelations = false;
	
	/// <summary>
	/// The player completed tear falgs the moment they can start rotating and 
	/// translating the torn piece
	/// </summary>
	private bool playerCompletedTear = false;
	
	/// <summary>
	/// The player moving platform flag represents the state when the
	/// player is moving and rotating the torn piece
	/// </summary>
	public bool PlayerMovingPlatformState = false;
	
	/// <summary>
	/// The main cut piece rotation object used for rotating about user input correctly
	/// </summary>
	private GameObject MainCutPieceRotationObject;
	
	/// <summary>
	/// The current mouse world position ONLY when player moving torn piece
	/// and when the mouse has moved slightly
	/// </summary>
	private Vector2 currentMouseWorldPos = Vector2.zero;
	
	/// <summary>
	/// The previous mouse world position.
	/// </summary>
	private Vector2 prevMouseWorldPos = Vector2.zero;
	
	/// <summary>
	/// The minimum mouse distance until prev and current get set again
	/// </summary>
	private float minMouseDistance = 0.01f;
	
	/// <summary>
	/// The tear limit bounds.
	/// </summary>
	public GameObject TearLimitBounds;
	
	/// <summary>
	/// The main cut piece rotation object previous position.
	/// </summary>
	private Vector3 MainCutPieceRotationObjectPrevPos = Vector3.zero;
	
	/// <summary>
	/// The main cut piece rotation object previous rot.
	/// </summary>
	private Quaternion MainCutPieceRotationObjectPrevRot = Quaternion.identity;
	
	/// <summary>
	/// The decal objects stores all existing decal objects
	/// </summary>
	private Dictionary<GameObject, int> decalObjects;
	
	/// <summary>
	/// The decal objects stores all existing decal objects - OLD, used 
	/// to update bad decals for a bad tear
	/// </summary>
	private Dictionary<GameObject, int> oldDecalObjects;
	
	/// <summary>
	/// Flag needing to reset collision.
	/// </summary>
	private bool needToResetCollision = false;
	
	/// <summary>
	/// The tear piece colliding with player flag to prevent user movement
	/// </summary>
	private bool TearPieceCollidingWithPlayer = false;
	
	/// <summary>
	/// The previous player input position, used to stop the torn
	/// piece from covering player
	/// </summary>
	private Vector3 prevPlayerInputPos;
	
	/// <summary>
	/// The have torn once.
	/// </summary>
	public bool HaveTornOnce = false;
	
	/// <summary>
	/// The fold object.
	/// </summary>
	public GameObject FoldObject;
	
	/// <summary>
	/// The fold script.
	/// </summary>/
	private GameObject FoldScript;
		
	/// <summary>
	/// The torn through bad object flag is true as soon as (during) the player
	/// trying to tear through untearable object for decal color change
	/// </summary>
	public bool tornThroughBadObject = false;
	
	/// <summary>
	/// The need to reset bad decals.
	/// </summary>
	private bool needToResetBadDecals = false;
	
	/// <summary>
	/// The player object reference.
	/// </summary>
	private GameObject PlayerObjectRef;
	
	/// <summary>
	/// The unfold border object reference.
	/// </summary>
	private GameObject unfoldBorderObjectRef;
	
	public TWCharacterController twCharController;

    /// <summary>
    /// Reference for Input Manager
    /// </summary>
    /// <returns></returns>
    public InputManager GetInputManager()
    {
        return inputManagerRef;
    }


    public Vector3 GetInputPos()
    {
        return currentPlayerInputPos;
    }


    /// <summary>
    /// Getter for rotating cut piece
    /// </summary>
    /// <returns></returns>
    public bool GetRotatingPiece()
    {
        return rotatingPiece;
    }

    /// <summary>
    /// Set the rotating cut piece
    /// </summary>
    /// <param name="value"></param>
    public void SetRotatingPiece(bool value)
    {
        rotatingPiece = value;
    }
	
	/// <summary>
	/// Gets the GS manager.
	/// </summary>
	public GameStateManager GetGSManager()
	{
		return gameStateManagerRef;
	}
	
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	private void Start () 
	{ 
        // Init manger refs
        touchController = GameObject.FindGameObjectWithTag("MainObject").GetComponent<TouchController>();
        gameStateManagerRef = GameObject.FindGameObjectWithTag("MainObject").GetComponent<GameStateManager>();
        gameStateManagerRef = GameObject.FindGameObjectWithTag("MainObject").GetComponent<GameStateManager>();
        inputManagerRef = GameObject.FindGameObjectWithTag("MainObject").GetComponent<InputManager>();
        soundManagerRef = GameObject.FindGameObjectWithTag("MainObject").GetComponent<SoundManager>();
		
		PlayerObjectRef = GameObject.FindGameObjectWithTag("Player");
		twCharController = PlayerObjectRef.GetComponent<TWCharacterController>();
		unfoldBorderObjectRef = GameObject.FindGameObjectWithTag("unfoldborder");

		//Initialize the tornPlatform list which will be modified
		//during runtime when player triggeres tearing mechanic
		TornPlatforms = new List<GameObject>();
		
		//Initialize the centerPosition dictionary
		centerPositions = new Dictionary<GameObject, Vector3>();
		
		//Initialize the list storing the tearable objects which need
		//to be parented to the world cut piece
		objectsBelongingToCutPiece = new List<GameObject>();
		
		//Initialize decalObject dictionary storeing every decal object for proper updating and cleanup
		decalObjects = new Dictionary<GameObject, int>();
		
		//Create new dictionary for platforms original positions
		originalPlatformPositions = new Dictionary<GameObject, Vector3>();
		
		
		//Init
		haveCheckedDecalCollision = new Dictionary<GameObject, bool>();
		
		
		GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
		Platforms = new List<GameObject>();
		foreach(GameObject go in platforms)
		{
			Platforms.Add(go);
		}
		platforms = null;
		
		//Store original positions of paltforms
		foreach(GameObject go in Platforms)
		{
			originalPlatformPositions.Add(go, go.transform.position);
		}
		
		
		OriginalPlatformTopology = new Dictionary <GameObject, int[]>();
		foreach(GameObject go in Platforms)
		{
			OriginalPlatformTopology.Add(go, go.GetComponent<MeshFilter>().mesh.triangles);
		}
		OriginalPlatformTopology.Add(MainStartingWorldPaper, MainStartingWorldPaper.GetComponent<MeshFilter>().mesh.triangles);
		
		
		//init local ref
		TearLimitBounds = GameObject.FindGameObjectWithTag("TearLimit");
		
		CreateDecalPool();
	}
	
	/// <summary>
	/// The currently tearing flag to draw decal objects for visual tearing effect
	/// </summary>
	public bool PlayerCurrentlyTearing = false;
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	private void Update () 
	{
		//Update the player position based off which device we are currently running
		UpdatePlayerPos();
		
		//Reset the tear when player tears through folded region of paper
       	// if (inputManagerRef.GetcurrPressState().Equals(InputManager.PressState.JUST_RELEASED) && httingFoldedArea)
		//{
		//	Reset();
		//}
		
		//Check, and only perform once
		if(TearFinished && !HaveTornOnce)
		{
			//UnityEngine.Debug.LogError("****************testing**********GROUPING ");
			GroupTornObjectsFindCenter();
		}
		
		//Wait for the flag to trigger parent child relation establishment
		if(needToSetParentChildRelations)
		{
			//UnityEngine.Debug.LogError("****************testing**********ParentObjectsToWorldCutPiece ");
			ParentObjectsToWorldCutPiece();
			DisableTearScripts();
		}
		
		//Reset Fold Flag blockers for player being able to fold
		if(PlayerMovingPlatformState && GVariables.TearPieceCoveringFold)
		{
			//GVariables.TearPieceCoveringFold = false;
			previousPositionCoveringFoldFlag = false;
		}
		
		if(FoldScript == null) FoldScript = GameObject.FindGameObjectWithTag("FoldObject");
		if(GVariables.FoldPieceCoveringTear) newDepth = unfoldBorderObjectRef.transform.position.z + 0.5f;
		if(FoldScript.GetComponent<Fold>().isFolded 
			|| 
			FoldScript.GetComponent<Fold>().currentlyFolding)
		{
			newDepth = -6;
		}
		
		//Translate torn objects with user input
		if(playerCompletedTear && MainWorldCutPaper != null)
		{
			//UnityEngine.Debug.LogError("TranslateRotateTornObjects");
			TranslateRotateTornObjects();
		}
		
		if(PlayerCurrentlyTearing)
		{
			//UnityEngine.Debug.LogError("PlayerCurrentlyTearing");
			//Draw decal mouse input effects
			CreateTearingEffect();
		}
		
		//Update the tear decal FX while player is tearing
		UpdateTearDecals();
		
		//RE ASSIGN Z DEPTH so player can properly interact with platforms
		EnsurePlatformDepthFixed();
		
		
		if(MainWorldCutPaper != null)
		{
			bool playerCollidingCheck = PlayerCollidingMaterialChange();//PlayerCollidingWithTornPieceCheck();
			//Ensure the torn pice changed material color when fold is covering
			if(PlayerMovingPlatformState 
				&& playerCollidingCheck
				)
			{
				if(  MainWorldCutPaper.GetComponent<MeshRenderer>().material.color != new Color(0.2f, 0.2f, 0.2f, 1.0f))
				{
					//UnityEngine.Debug.LogError("test1");
					MainWorldCutPaper.GetComponent<MeshRenderer>().material.color = new Color(0.2f, 0.2f, 0.2f, 1.0f);
				}
			}
			else if(
				(
					GVariables.FoldPieceCoveringTear 
				|| (!GVariables.FoldPieceCoveringTear && PlayerMovingPlatformState) 
				|| (!PlayerMovingPlatformState && playerCollidingCheck)
				) 
				&& MainWorldCutPaper.GetComponent<MeshRenderer>().material.color != Color.grey)
			{
				//UnityEngine.Debug.LogError("test2");
				MainWorldCutPaper.GetComponent<MeshRenderer>().material.color = Color.grey;
				
			}
			else if(!GVariables.FoldPieceCoveringTear 
				&& !PlayerMovingPlatformState 
				&& MainWorldCutPaper.GetComponent<MeshRenderer>().material.color != new Color(0.8f, 0.8f, 0.8f, 1.0f)
				&& !playerCollidingCheck)
			{
				//UnityEngine.Debug.LogError("test3");
				MainWorldCutPaper.GetComponent<MeshRenderer>().material.color = new Color(0.8f, 0.8f, 0.8f, 1.0f);
			}
		}
		
		ForceTearDrawDepth();
		if(HaveTornOnce)
		{
			UpdateRenderQueue();
		}
		
		if(TearFinished && GVariables.TearPieceCoveringFold && !PlayerMovingPlatformState && 
			(PlayerGraphicsAndCollision.transform.position.z > -7 || PlayerGraphicsAndCollision.transform.position.z < -9))
		{
			PlayerGraphicsAndCollision.transform.position = new Vector3(PlayerGraphicsAndCollision.transform.position.x,
																		PlayerGraphicsAndCollision.transform.position.y, 
																		-8);
		}
		else if(PlayerMovingPlatformState && (PlayerGraphicsAndCollision.transform.position.z > 0.1f || PlayerGraphicsAndCollision.transform.position.z < -0.1f))
		{
			PlayerGraphicsAndCollision.transform.position = new Vector3(PlayerGraphicsAndCollision.transform.position.x,
																		PlayerGraphicsAndCollision.transform.position.y, 
																		0);
		}
	}
	
	/// <summary>
	/// Resets the triangles.
	/// </summary>
	public void ResetTriangles(GameObject go)
	{
		return;
		
		//UnityEngine.Debug.Log("ResetTriangles on " + go.name.ToString());
		if(OriginalPlatformTopology.ContainsKey(go))
		{
			go.GetComponent<MeshFilter>().mesh.triangles = OriginalPlatformTopology[go];
			go.GetComponent<MeshCollider>().mesh.triangles = OriginalPlatformTopology[go];
		}
		
	}
	
	/// <summary>
	/// Updates the player position.
	/// </summary>
	private void UpdatePlayerPos()
	{
		// store player input from swipe positions
        if (gameStateManagerRef.OnMobileDevice())
        {
            if (touchController != null)
            {
                if (touchController.GetFingerPositions().Count != 0)
                {
                    Vector3 touchPos = new Vector3(touchController.GetFingerPositions().ElementAt(0).x,
                                                    touchController.GetFingerPositions().ElementAt(0).y);

                    if (currentPlayerInputPos != touchPos)
                    {
                        currentPlayerInputPos = touchPos;
                    }
                }
            }

            else
                UnityEngine.Debug.Log("TOUCH CONTROLLER IS NULL");
        }

        // else by default store it on the pc side using Input.Mouse
        else
        {
            if (currentPlayerInputPos != Input.mousePosition)
            {
                currentPlayerInputPos = Input.mousePosition;
				//UnityEngine.Debug.Log ("MousePOsition");
            }
        }
	}
	
	/// <summary>
	/// Updates the render queue.
	/// </summary>
	private void UpdateRenderQueue()
	{
		if(MainWorldCutPaper != null)
		{
			/*
			 if(GVariables.TearPieceCoveringFold && 
				MainWorldCutPaper.gameObject.GetComponent<SetRenderQueue>().GetFirstQueue() != newSetRenderDepth)
			{
				MainWorldCutPaper.gameObject.GetComponent<SetRenderQueue>().OriginalDepth = MainWorldCutPaper.gameObject.GetComponent<SetRenderQueue>().GetFirstQueue();
				MainWorldCutPaper.gameObject.GetComponent<SetRenderQueue>().SetFirstQueue(newSetRenderDepth);
				
				if(TornEdgeFXContainter != null)
				{
					for(int itor = 0; itor < TornEdgeFXContainter.transform.GetChildCount(); itor++)
					{
						TornEdgeFXContainter.transform.GetChild(itor).gameObject.GetComponent<SetRenderQueue>().OriginalDepth = MainWorldCutPaper.gameObject.GetComponent<SetRenderQueue>().GetFirstQueue();
						TornEdgeFXContainter.transform.GetChild(itor).gameObject.GetComponent<SetRenderQueue>().SetFirstQueue(newSetRenderDepth);
					}
				}
			}
			*/
			
			
			if(GVariables.TearPieceCoveringFold && MainWorldCutPaper.GetComponent<SetRenderQueue>())
			{
				UnityEngine.Debug.Log("DESTROY RENDER");
				// MainWorldCutPaper.GetComponent<SetRenderQueue>().e
				Destroy(MainWorldCutPaper.GetComponent<SetRenderQueue>());
			}
			
			else if(!GVariables.TearPieceCoveringFold && !MainWorldCutPaper.GetComponent<SetRenderQueue>())
			{
				UnityEngine.Debug.Log("ADD RENDER");
				MainWorldCutPaper.AddComponent<SetRenderQueue>();
			}
			
			
			else if(!GVariables.TearPieceCoveringFold && 
				MainWorldCutPaper.gameObject.GetComponent<SetRenderQueue>().GetFirstQueue() != 
				MainWorldCutPaper.gameObject.GetComponent<SetRenderQueue>().OriginalDepth)
			{
				MainWorldCutPaper.gameObject.GetComponent<SetRenderQueue>().SetFirstQueue(MainWorldCutPaper.gameObject.GetComponent<SetRenderQueue>().OriginalDepth);
				
				if(TornEdgeFXContainter != null)
				{
					for(int itor = 0; itor < TornEdgeFXContainter.transform.GetChildCount(); itor++)
					{
						TornEdgeFXContainter.transform.GetChild(itor).gameObject.GetComponent<SetRenderQueue>().SetFirstQueue(TornEdgeFXContainter.transform.GetChild(itor).gameObject.GetComponent<SetRenderQueue>().OriginalDepth);
					}
				}
			}
		}
	}
	
	/// <summary>
	/// Ensures the platform depths are fixed.
	/// </summary>
	private void EnsurePlatformDepthFixed()
	{
		//RE ASSIGN Z DEPTH so player can properly interact with platforms
		if(objectsBelongingToCutPiece != null &&
			objectsBelongingToCutPiece.Count() != 0
			&& !PlayerMovingPlatformState)
		{
			// Ensures the destroyed objects are not being referenced
			EnsureDestroyedObjectsNotbeingReferenced(Platforms);
			EnsureDestroyedObjectsNotbeingReferenced(objectsBelongingToCutPiece);
			
			foreach(GameObject platform in Platforms)//objectsBelongingToCutPiece
			{
				platform.transform.position = new Vector3(platform.transform.position.x, platform.transform.position.y, 0.0f);
			}
			foreach(GameObject platform in objectsBelongingToCutPiece)//objectsBelongingToCutPiece
			{
				platform.transform.position = new Vector3(platform.transform.position.x, platform.transform.position.y, 0.0f);
			}
		}
	}
	/// <summary>
	/// The bad tear visual timer.
	/// </summary>
	public float badTearVisualTimer = 0;
	
	/// <summary>
	/// The bad tear visual time.
	/// </summary>
	private float badTearVisualTime = 2;
	
	/// <summary>
	/// The bad tear visual local reference
	/// </summary>
	public GameObject BadTearVisual;
	
	/// <summary>
	/// Resets the bad tear visual info.
	/// </summary>
	private void ResetBadTearVisualInfo()
	{
		BadTear = false;
		badTearTimer = 0;
		tornThroughBadObject = false;
		hittingFoldedArea = false;
		haveCheckedDecalCollision.Clear();
		BadTearVisual.GetComponent<MeshRenderer>().enabled = false;
		//reset timer for next bad tear
		badTearVisualTimer = 0;
	}
	

	
	/// <summary>
	/// Updates the tear decals.
	/// </summary>
	private void UpdateTearDecals()
	{
		if(TearFinished && BadTearVisual.GetComponent<MeshRenderer>().enabled)
		{
			ResetBadTearVisualInfo();
		}
		
		//Update decal Objects
		if(decalObjects.Keys.Count() > 0) 
		{
			UpdateDecalObjects();
		}
		
		if(DrawBadTearVisual 
			&& !twCharController.playerIsDead)//!PlayerObjectRef.GetComponent<TWCharacterController>().playerIsDead)
		{
			//UnityEngine.Debug.LogError("******testing");
			
			//Keep track of badTearVisual time
			badTearVisualTimer += Time.deltaTime;
			
			if(badTearVisualTimer > badTearVisualTime)
			{
				//If decalObjects is empty, then make sure to reset Badtear iff needed
				//Debug.LogError("****************testing -->  BadTear && decalObjects.Keys.Count() == 0, tornplatform count = " + TornPlatforms.Count().ToString());
				ResetBadTearVisualInfo();
				DrawBadTearVisual = false;
				
			}
			else if(!BadTearVisual.GetComponent<MeshRenderer>().enabled)
			{
				BadTearVisual.GetComponent<MeshRenderer>().enabled = true;
			}
		}
		

		/*
		else if((!DrawBadTearVisual && BadTearVisual.GetComponent<MeshRenderer>().enabled) || 
					(PlayerObjectRef.GetComponent<TWCharacterController>().playerIsDead)
			)
		{
			ResetBadTearVisualInfo();
		}
		*/
		
		
		/*
		if(oldDecalObjects != null)
		{
			//Update decal Objects
			if(oldDecalObjects.Keys.Count() > 0) 
			{
				//This is when the decal objects need to be re-sezes, materials changed, and renderers re-enabled
				if(needToResetBadDecals)
				{
					for(int itor = 0; itor < oldDecalObjects.Keys.Count(); itor++)
					{
						if(oldDecalObjects.Keys.ElementAt(itor).transform.localScale != new Vector3(0.2f, 0.2f, 1.0f))
						{
							oldDecalObjects.Keys.ElementAt(itor).transform.localScale = new Vector3(0.2f, 0.2f, 1.0f);
						}
						if(oldDecalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().material.color != Color.red)
						{
							//Debug.LogError("test red #3");
							
							oldDecalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().material.color = Color.red;
						}
						
						if(!oldDecalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().enabled)
						{
							oldDecalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().enabled = true;
						}
					}
					//Make sure this is only hit the first time
					needToResetBadDecals = false;
				}
				
				UpdateDecalObjectsOldBadTear();
			}
		}
		*/
	}
	
	/// <summary>
	/// The reset bad tear visual blocker.
	/// </summary>
	public bool ResetBadTearVisualBlocker = false;
	
	/// <summary>
	/// The draw bad tear visual.
	/// </summary>
	public bool DrawBadTearVisual = false;
	
	/// <summary>
	/// Ensures the destroyed objects are not being referenced.
	/// </summary>
	private void EnsureDestroyedObjectsNotbeingReferenced(List<GameObject> list)
	{
		for(int itor = 0; itor < list.Count(); itor++)
		{
			if(list.ElementAt(itor) == null)
			{
				list.Remove(list.ElementAt(itor));
			}
			
		}
	}
	
	public float DepthOffsetTearOnFold = 6;

	
	/// <summary>
	/// Forces the tear draw depth depending upon the flags in GlobalVariable (GVariables.cs) gameObject
	/// </summary>
	private void ForceTearDrawDepth()
	{
		//UnityEngine.Debug.LogError("call testing --> in ForceTearDrawDepth");
		
		

		if(HaveTornOnce && MainWorldCutPaper && 
			((Input.GetMouseButton(0)
			|| (FoldObject.GetComponent<Fold>() != null 
				&& FoldObject.GetComponent<Fold>().currentlyFolding))
			|| (FoldObject.GetComponent<Fold>() != null 
				&& FoldObject.GetComponent<Fold>().isFolded))
		  )
		{
			if(PlayerMovingPlatformState
			//&& 
			//(MainWorldCutPaper.transform.position.z <= -8.0f
			  // && MainWorldCutPaper.transform.position.z >= -6.0f)
				)
			{
				//if((MainWorldCutPaper.transform.position.z <= -8.0f
			    //	&& MainWorldCutPaper.transform.position.z >= -6.0f))
				{
					//UnityEngine.Debug.LogError("testing  ForceTearDrawDepth 3");
					MainWorldCutPaper.transform.position = new Vector3(MainWorldCutPaper.transform.position.x,
																				MainWorldCutPaper.transform.position.y,
																				-7f);
				}
				
			}
			else if(!PlayerMovingPlatformState)
			{
				if(GVariables.TearPieceCoveringFold 
				 //&& !GVariables.FoldPieceCoveringTear 
				// &&
				//(MainWorldCutPaper.transform.position.z <= -8.0f
				   //&& MainWorldCutPaper.transform.position.z >= -6.0f)
					)
				{
					//if((MainWorldCutPaper.transform.position.z <= -8.0f
				   	//	&& MainWorldCutPaper.transform.position.z >= -6.0f))
					{
						//UnityEngine.Debug.LogError("testing  ForceTearDrawDepth 2");
						MainWorldCutPaper.transform.position = new Vector3(MainWorldCutPaper.transform.position.x,
																					MainWorldCutPaper.transform.position.y,
																					-7f);
					}
					
				}
				
				if(!GVariables.TearPieceCoveringFold)// || GVariables.FoldPieceCoveringTear)
				{
					if((FoldObject.GetComponent<Fold>() != null 
							&& FoldObject.GetComponent<Fold>().currentlyFolding)
						|| (FoldObject.GetComponent<Fold>() != null 
							&& FoldObject.GetComponent<Fold>().isFolded))
					{
						//UnityEngine.Debug.LogError("testing  ForceTearDrawDepth 2.5");
						MainWorldCutPaper.transform.position = new Vector3(MainWorldCutPaper.transform.position.x,
																					MainWorldCutPaper.transform.position.y,
																					-7f);
					}
					else
					{
						//UnityEngine.Debug.LogError("testing  ForceTearDrawDepth 4");
						MainWorldCutPaper.transform.position = new Vector3(MainWorldCutPaper.transform.position.x,
																		MainWorldCutPaper.transform.position.y,
																		0f);
					}
					
					
				}
				
			}
			else 
			{
				//UnityEngine.Debug.LogError("testing  ForceTearDrawDepth NO DECISION");
			}
		
		}
		
		if(GVariables.FoldPieceCoveringTear) 
		{
			//UnityEngine.Debug.LogError("trying 5");
			//if((MainWorldCutPaper.transform.position.z >= (unfoldBorderObjectRef.transform.position.z + 0.6f)
		   //		&& MainWorldCutPaper.transform.position.z <= (unfoldBorderObjectRef.transform.position.z + 0.3f)))
			{
				//UnityEngine.Debug.LogError("testing  ForceTearDrawDepth 5");
				MainWorldCutPaper.transform.position = new Vector3(MainWorldCutPaper.transform.position.x,
																		MainWorldCutPaper.transform.position.y,
																		(unfoldBorderObjectRef.transform.position.z+0.5f));// + 0.5f));
			}
		}
		
		if(PlayerMovingPlatformState && 
			((FoldObject.GetComponent<Fold>() != null && FoldObject.GetComponent<Fold>().currentlyFolding) 
			||
			(FoldObject.GetComponent<Fold>() != null && FoldObject.GetComponent<Fold>().isFolded))
			)
		{
			//UnityEngine.Debug.LogError("trying 5");
			//if((MainWorldCutPaper.transform.position.z >= (unfoldBorderObjectRef.transform.position.z + 0.6f)
		   //		&& MainWorldCutPaper.transform.position.z <= (unfoldBorderObjectRef.transform.position.z + 0.3f)))
			{
				//UnityEngine.Debug.LogError("testing  ForceTearDrawDepth 6");
				MainWorldCutPaper.transform.position = new Vector3(MainWorldCutPaper.transform.position.x,
																	MainWorldCutPaper.transform.position.y,
																	-7f);
			}
		}
		
		
		/*
		if(HaveTornOnce)
		{
			//UnityEngine.Debug.LogError("testing  ForceTearDrawDepth ");
			if(!PlayerMovingPlatformState)
			{
				if(GVariables.TearPieceCoveringFold 
					&& MainCutPieceRotationObject.transform.position.z != (unfoldBorderObjectRef.transform.position.z - DepthOffsetTearOnFold))
				{
					UnityEngine.Debug.LogError("testing  ForceTearDrawDepth 1");
					MainCutPieceRotationObject.transform.position = new Vector3(MainCutPieceRotationObject.transform.position.x,
																		MainCutPieceRotationObject.transform.position.y,
																		unfoldBorderObjectRef.transform.position.z - DepthOffsetTearOnFold);
					//MainWorldCutPaper.transform.position = new Vector3(MainWorldCutPaper.transform.position.x,
					//													MainWorldCutPaper.transform.position.y,
					//													GVariables.OnTopDepth);
					/*
					if(MainWorldCutPaper.transform.GetChild(0).transform.position != 
							new Vector3(MainWorldCutPaper.transform.GetChild(0).transform.position.x,
								MainWorldCutPaper.transform.GetChild(0).transform.position.y, 0))
					{
						MainWorldCutPaper.transform.GetChild(0).transform.position = 
							new Vector3(MainWorldCutPaper.transform.GetChild(0).transform.position.x,
								MainWorldCutPaper.transform.GetChild(0).transform.position.y, 0);
					}
					
					for(int itor = 0; itor < MainWorldCutPaper.transform.GetChild(0).transform.GetChildCount(); itor++)
					{
						if(MainWorldCutPaper.transform.GetChild(0).transform.GetChild(itor).transform.position != 
							new Vector3(MainWorldCutPaper.transform.GetChild(0).transform.GetChild(itor).transform.position.x,
								MainWorldCutPaper.transform.GetChild(0).transform.GetChild(itor).transform.position.y,
								MainWorldCutPaper.transform.GetChild(0).transform.GetChild(itor).transform.position.z + 0.01f))
						{
							MainWorldCutPaper.transform.GetChild(0).transform.GetChild(itor).transform.position = 
								new Vector3(MainWorldCutPaper.transform.GetChild(0).transform.GetChild(itor).transform.position.x,
								MainWorldCutPaper.transform.GetChild(0).transform.GetChild(itor).transform.position.y,
								MainWorldCutPaper.transform.GetChild(0).transform.GetChild(itor).transform.position.z + 0.01f);
						}
					}
					///////
					
				}
				else if(MainCutPieceRotationObject.transform.position.z != GVariables.TearOnBottom 
						&& MainCutPieceRotationObject.transform.position.z != GVariables.TearOnBottom
						&& !GVariables.TearPieceCoveringFold)
				{
					UnityEngine.Debug.LogError("testing  ForceTearDrawDepth 2");
					/*
					if(!GVariables.TearPieceCoveringFold)
					{
						MainCutPieceRotationObject.transform.position = new Vector3(MainCutPieceRotationObject.transform.position.x,
																		MainCutPieceRotationObject.transform.position.y,
																		GVariables.TearOnTopOfDoor);
					}
					else
					{8
					
					
					///////
						MainCutPieceRotationObject.transform.position = new Vector3(MainCutPieceRotationObject.transform.position.x,
																		MainCutPieceRotationObject.transform.position.y,
																		GVariables.TearOnBottom);
					//}
					
					//MainWorldCutPaper.transform.position = new Vector3(MainWorldCutPaper.transform.position.x,
					//													MainWorldCutPaper.transform.position.y,
					//													GVariables.OnBottomDepth);
					/*
					if(MainWorldCutPaper.transform.GetChild(0).transform.position != 
							new Vector3(MainWorldCutPaper.transform.GetChild(0).transform.position.x,
								MainWorldCutPaper.transform.GetChild(0).transform.position.y, 0))
					{
						MainWorldCutPaper.transform.GetChild(0).transform.position = 
							new Vector3(MainWorldCutPaper.transform.GetChild(0).transform.position.x,
								MainWorldCutPaper.transform.GetChild(0).transform.position.y, 0);
					}
					
					for(int itor = 0; itor < MainWorldCutPaper.transform.GetChild(0).transform.GetChildCount(); itor++)
					{
						if(MainWorldCutPaper.transform.GetChild(0).transform.GetChild(itor).transform.position != 
							new Vector3(MainWorldCutPaper.transform.GetChild(0).transform.GetChild(itor).transform.position.x,
								MainWorldCutPaper.transform.GetChild(0).transform.GetChild(itor).transform.position.y,
								GVariables.TearOnBottom + 0.01f))
						{
							MainWorldCutPaper.transform.GetChild(0).transform.GetChild(itor).transform.position = 
								new Vector3(MainWorldCutPaper.transform.GetChild(0).transform.GetChild(itor).transform.position.x,
								MainWorldCutPaper.transform.GetChild(0).transform.GetChild(itor).transform.position.y,
								GVariables.TearOnBottom + 0.01f);
						}
					}
					///////
				}
				
			}
			else if(PlayerMovingPlatformState
				 && MainCutPieceRotationObject.transform.position.z != -6.0f
						&& !GVariables.TearPieceCoveringFold)
			{
				UnityEngine.Debug.LogError("testing  ForceTearDrawDepth 3");
				MainCutPieceRotationObject.transform.position = new Vector3(MainCutPieceRotationObject.transform.position.x,
																			MainCutPieceRotationObject.transform.position.y,
																			 -6.0f);
				
				//MainWorldCutPaper.transform.position = new Vector3(MainWorldCutPaper.transform.position.x,
				//												   MainWorldCutPaper.transform.position.y,
				//												   -2.0f);
				
				/*if(MainWorldCutPaper.transform.GetChild(0).transform.position != 
						new Vector3(MainWorldCutPaper.transform.GetChild(0).transform.position.x,
							MainWorldCutPaper.transform.GetChild(0).transform.position.y, 0))
				{
					MainWorldCutPaper.transform.GetChild(0).transform.position = 
						new Vector3(MainWorldCutPaper.transform.GetChild(0).transform.position.x,
							MainWorldCutPaper.transform.GetChild(0).transform.position.y, 0);
				}
				
				for(int itor = 0; itor < MainWorldCutPaper.transform.GetChild(0).transform.GetChildCount(); itor++)
					{
						if(MainWorldCutPaper.transform.GetChild(0).transform.GetChild(itor).transform.position != 
							new Vector3(MainWorldCutPaper.transform.GetChild(0).transform.GetChild(itor).transform.position.x,
								MainWorldCutPaper.transform.GetChild(0).transform.GetChild(itor).transform.position.y,
								-1.99f))
						{
							MainWorldCutPaper.transform.GetChild(0).transform.GetChild(itor).transform.position = 
								new Vector3(MainWorldCutPaper.transform.GetChild(0).transform.GetChild(itor).transform.position.x,
								MainWorldCutPaper.transform.GetChild(0).transform.GetChild(itor).transform.position.y,
								-1.99f);
						}
					}
					
					////////
			}
			else if(GVariables.TearPieceCoveringFold 
					&& MainCutPieceRotationObject.transform.position.z != (unfoldBorderObjectRef.transform.position.z - DepthOffsetTearOnFold))
			{
				UnityEngine.Debug.LogError("testing  ForceTearDrawDepth 4");
				MainCutPieceRotationObject.transform.position = new Vector3(MainCutPieceRotationObject.transform.position.x,
																	MainCutPieceRotationObject.transform.position.y,
																	unfoldBorderObjectRef.transform.position.z - DepthOffsetTearOnFold);
				
			}
		
		}
		*/
	}
	
	
	/// <summary>
	/// Disables the tear script from each tearable object
	/// </summary>
	private void DisableTearScripts()
	{
		/*
		foreach(GameObject go in Platforms)
		{
			go.GetComponent<TearPaper>().enabled = false;	
		}
		foreach(GameObject go in TornPlatforms)
		{
			go.GetComponent<TearPaper>().enabled = false;	
		}
		*/
		
		MainWorldCutPaper.GetComponent<TearPaper>().enabled = false;
		MainWorldPaper.GetComponent<TearPaper>().enabled = false;
	}
	
	
    private bool fingerLifted = false;

    /// <summary>
    /// Method that determines if a point is within
    /// the bounds of a game object. TODO CHANGE D.A.
    /// </summary>
    /// <param name="meshObject"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    private bool PointInsideObject(GameObject gameObject, Vector2 point)
    {
        return (gameObject.collider.bounds.Contains(new Vector3(Camera.mainCamera.ScreenToWorldPoint(point).x,
                                                                     Camera.mainCamera.ScreenToWorldPoint(point).y,
                                                                     gameObject.collider.bounds.center.z)));
    }


    public bool GetMovingPiece()
    {
        return PlayerMovingPlatformState;
    }

    Stopwatch bufferWatch = new Stopwatch();
    int INPUT_BUFFER_LIMIT = 250;
	

	//private bool haveFinishedMovingOffPlayer = false;
	
	float newDepth;
	/// <summary>
	/// Translates and rotates the torn objects.
	/// </summary>
	private void TranslateRotateTornObjects()
	{
        //Debug.Log("STATE " + inputManagerRef.GetcurrPressState().ToString());
        //Debug.Log("ROT " + rotatingPiece);
        //Debug.Log("LIFT " + fingerLifted);
		
		Vector3 newPosForTear = Vector3.zero;
		
		if(PlayerMovingPlatformState)// && PlayerInputInBounds())
		{
           
            // converts screen to world position - D.A.
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(MainWorldCutPaper.transform.position);
            Vector3 curScreenPoint = new Vector3(currentPlayerInputPos.x, currentPlayerInputPos.y, screenPoint.z);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);// + offset;
            curPosition.z = GVariables.OnBottomDepth;
			
            newPosForTear.z = newDepth;
			
            prevPlayerInputPos = MainCutPieceRotationObject.transform.position;

            // Making sure player is touching inside the cut piece of paper if on the device
            // Otherwise, if on the keyboard, then update update since you're (John) already checking
            // if the player is inside the bounds above - D.A.
		    if((!gameStateManagerRef.OnMobileDevice()) ||
                (gameStateManagerRef.OnMobileDevice() &&
                 PointInsideObject(MainWorldCutPaper, touchController.GetLastFingerPosition()) &&
                 (!rotatingPiece || fingerLifted)))
		        {
                    newPosForTear = Camera.main.ScreenToWorldPoint(new
                                                                Vector3(currentPlayerInputPos.x, currentPlayerInputPos.y,
                                                                Camera.main.WorldToScreenPoint(new Vector3(0,0,-1)).z));

                   
                    MainCutPieceRotationObject.transform.position = newPosForTear;
                    rotatingPiece = false;
                    fingerLifted = false;
				
					//UnityEngine.Debug.LogError("Dominic test win Translating Objects Hit");
		        }
		}
		
		//Transition between moving and not moving the torn piece of 
		//paper by switching playerMovingPlatform on and off

       
		bool playerCollidingWithTornPieceCheckTester = PlayerCollidingWithTornPieceCheck();
		
		/*
		if(playerCollidingWithTornPieceCheckTester 
			&& !PlayerShield.GetComponent<MeshRenderer>().enabled 
			&& !PlayerObjectRef.GetComponent<TWCharacterController>().playerIsDead)
		{
			PlayerShield.GetComponent<MeshRenderer>().enabled = true;
		}
		else if(!playerCollidingWithTornPieceCheckTester && PlayerShield.GetComponent<MeshRenderer>().enabled)
		{
			PlayerShield.GetComponent<MeshRenderer>().enabled = false;
		}
		
		if(PlayerShield.GetComponent<MeshRenderer>().enabled)
		{
			PlayerShield.transform.RotateAround(new Vector3(0, 1, 1), 0.1f);
		}
		*/
		
		
		bool bKeyHitFlag = Input.GetMouseButtonDown(0);
		bool mouseClickOverTornPiece = false;
		if(bKeyHitFlag)
		{
//			RaycastHit hit;
//			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//			if(Physics.Raycast(ray, out hit, 100))
//			{
//					UnityEngine.Debug.Log("DUMB" + hit.collider.gameObject.name);
//
//				if((hit.collider.gameObject.name == "paper_CuttPieceOfPaper"))
//				{
//						mouseClickOverTornPiece = true;
//				}
//			}
			if(HaveTornOnce && twCharController.getGrounded())
			{
				if ( gameStateManagerRef.GetWorldCollision().PointInsideObject(MainWorldCutPaper, 
				/*Camera.mainCamera.WorldToScreenPoint(*/Input.mousePosition))//)
					mouseClickOverTornPiece = true;
			}
		}
		//	if(gameStateManagerRef.//Input.GetKeyUp("b");
		bool test1 = false;
		
		if(!playerCollidingWithTornPieceCheckTester
			&& !GVariables.FoldPieceCoveringTear
			&&
				(
					(bKeyHitFlag &&  mouseClickOverTornPiece &&
		            !gameStateManagerRef.OnMobileDevice())
					
					||
		
		            // or if on tablet, 
		            // While the piece is active looking for double tap or multi touch to place current moving piece
		             (
						(
						  (inputManagerRef.DoubleTap()) &&
			              PointInsideObject(MainWorldCutPaper, currentPlayerInputPos) &&
			              gameStateManagerRef.OnMobileDevice() &&
			              (bufferWatch.ElapsedMilliseconds == 0 ||
			              bufferWatch.ElapsedMilliseconds > INPUT_BUFFER_LIMIT) 
						) 
			            // or new one, if the piece is idle for 1000 ms (1 second)
			            || (gameStateManagerRef.OnMobileDevice() && PlayerMovingPlatformState && inputManagerRef.idleKeyWatch.ElapsedMilliseconds > 1000)
						
					 )
				
				
				)
				
			 )
			{
				
				test1 = true;
			}
			else
			{
				test1 = false;
			}
		
		bool test2 = false;
		
		if(PlayerMovingPlatformState&&
			playerCollidingWithTornPieceCheckTester
			&& !GVariables.FoldPieceCoveringTear
			&&
				(
					(bKeyHitFlag &&//mouseClickOverTornPiece&&
		            !gameStateManagerRef.OnMobileDevice())
					
					||
		
		            // or if on tablet, 
		            // While the piece is active looking for double tap or multi touch to place current moving piece
		             (
						(
						  (inputManagerRef.DoubleTap()) &&
			              PointInsideObject(MainWorldCutPaper, currentPlayerInputPos) &&
			              gameStateManagerRef.OnMobileDevice() &&
			              (bufferWatch.ElapsedMilliseconds == 0 ||
			              bufferWatch.ElapsedMilliseconds > INPUT_BUFFER_LIMIT) 
						) 
			            // or new one, if the piece is idle for 1000 ms (1 second)
			            || (gameStateManagerRef.OnMobileDevice() && PlayerMovingPlatformState && inputManagerRef.idleKeyWatch.ElapsedMilliseconds > 1000)
						
					 )
				
				
				)
				
			 )
			{
				
				test2 = true;
			}
			else
			{
				test2 = false;
			}
			
		
		////UnityEngine.Debug.LogError("currently testing = " + test);
		//UnityEngine.Debug.LogError("currently on MOBILE = " + gameStateManagerRef.OnMobileDevice());
		//UnityEngine.Debug.LogError("hitting b = " + bKeyHitFlag);	
			
		if(test2)
		{
			PlayerMovingPlatformState = false;
			
			if(Camera.main.audio.clip != HurtAudio)
			{
				Camera.main.audio.clip = HurtAudio;
			}
			
			//Play rotation audio
			if(!Camera.main.audio.isPlaying && !BadTear)Camera.main.audio.Play();
			
			if(previousPositionCoveringFoldFlag)
			{
				//UnityEngine.Debug.LogError("Testing  DepthOffsetTearOnFold");
				MainCutPieceRotationObjectPrevPos.z = DepthOffsetTearOnFold;
			}
			else
			{
				//UnityEngine.Debug.LogError("Testing  0.9f");
				MainCutPieceRotationObjectPrevPos.z = 0.9f;
			}
			
			//UnityEngine.Debug.LogError("Reseting position");
			MainCutPieceRotationObject.transform.position = MainCutPieceRotationObjectPrevPos;
			MainCutPieceRotationObject.transform.rotation = MainCutPieceRotationObjectPrevRot;
			
			
			
			//if(!needToResetCollision)
			//	needToResetCollision = true;
		}
			
        
		/*** D.A. bool check mod for touchn controls***/
        // if player is on the pc, without unity remote
        if(test1)
        {
            //UnityEngine.Debug.Log(" hitting b");

            if (bufferWatch.ElapsedMilliseconds == 0)
                bufferWatch.Start();

            else if (bufferWatch.ElapsedMilliseconds > INPUT_BUFFER_LIMIT)
            {
                bufferWatch.Reset();
                bufferWatch.Start();
            }
			
			//set to true to check for collision
			if (!playerCollidingWithTornPieceCheckTester)
			{
				needToResetCollision = true;
			}
			
			
			//else
			//{
			//	haveFinishedMovingOffPlayer = false;
			//}

			

            //Rotate between being true and not true
            if (PlayerMovingPlatformState)
            {	
				//Set flags for preventing fold when tear interacts with already folded region
				if(CheckTornPieceCoveringFold())
				{
					
					previousPositionCoveringFoldFlag = true;
					GVariables.TearPieceCoveringFold = true;
					
				}
				else
				{
					
					previousPositionCoveringFoldFlag = false;
					GVariables.TearPieceCoveringFold = false;
				}
				
                //MainCutPieceRotationObject.transform.position = new Vector3(MainCutPieceRotationObject.transform.position.x,
				//							                    MainCutPieceRotationObject.transform.position.y, 
				//												GVariables.OnBottomDepth);
				
				
				
				
				
				
               // UnityEngine.Debug.Log("FALSE");
                if (playerCollidingWithTornPieceCheckTester)
                {
                //    UnityEngine.Debug.Log("TRUE");
					EnsureDestroyedObjectsNotbeingReferenced(objectsBelongingToCutPiece);
					foreach(GameObject go in objectsBelongingToCutPiece)
					{
						go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, -2.0f);
					}
                }
				else
				{
					
					//Disable All torn piece platform collision
					EnableTornChildrenColliders(false);
					//return all collision masking to previous state
					gameObject.GetComponent<TearCoverUp>().CheckToMaskPlatforms(MainWorldCutPaper);
					//Re-enable All torn piece platform collision
					EnableTornChildrenColliders(true);
					
					EnsureDestroyedObjectsNotbeingReferenced(objectsBelongingToCutPiece);
					//Remove parent relations to assign correct depth for player interaction
					foreach(GameObject go in objectsBelongingToCutPiece)
					{
						go.transform.parent = null;
					}
					
					//foreach(GameObject go in objectsBelongingToCutPiece)
					//{
					//	go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, 0.0f);
					//}
				}
				
				//MainWorldCutPaper.GetComponent<MeshRenderer>().material.color = new Color(0.8f, 0.8f, 0.8f, 1.0f);

                PlayerMovingPlatformState = false;
					
				/*
				if(MainWorldCutPaper.transform.GetChildCount() > 0 &&
					MainWorldCutPaper.transform.GetChild(0) != null)
				{
					for(int ztor = 0; ztor < MainWorldCutPaper.transform.GetChild(0).transform.GetChildCount(); ztor++)
					{
						if( MainWorldCutPaper.transform.GetChild(0).transform.GetChild(ztor).renderer != null)
						{
							if( MainWorldCutPaper.transform.GetChild(0).transform.GetChild(ztor).renderer.material.color != new Color(0.8f, 0.8f, 0.8f, 1.0f))
							{
								MainWorldCutPaper.transform.GetChild(0).transform.GetChild(ztor).renderer.material.color = new Color(0.8f, 0.8f, 0.8f, 1.0f);
							}
						}
					}
				}
				*/
            }

            else if(!playerCollidingWithTornPieceCheckTester)
            {
				//Set flags for preventing fold when tear interacts with already folded region
				if(CheckTornPieceCoveringFold())
				{
					
					previousPositionCoveringFoldFlag = true;
					GVariables.TearPieceCoveringFold = true;
				}
				else
				{
					
					previousPositionCoveringFoldFlag = false;
					GVariables.TearPieceCoveringFold = false;
				}
				
				foreach(GameObject go in objectsBelongingToCutPiece)
				{
					go.transform.parent = MainWorldCutPaper.transform;
				}
				foreach(GameObject go in objectsBelongingToCutPiece)
				{
					go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, -3.5f);
				}
			
				//MainWorldCutPaper.GetComponent<MeshRenderer>().material.color = Color.grey;
				
                //UnityEngine.Debug.Log("TRUE");
                PlayerMovingPlatformState = true;
				
				//save previous position
				MainCutPieceRotationObjectPrevPos = MainCutPieceRotationObject.transform.position;
				MainCutPieceRotationObjectPrevRot = MainCutPieceRotationObject.transform.rotation;
				
				
				/*
				if(MainWorldCutPaper.transform.GetChildCount() > 0 &&
					MainWorldCutPaper.transform.GetChild(0) != null)
				{
					for(int ztor = 0; ztor < MainWorldCutPaper.transform.GetChild(0).transform.GetChildCount(); ztor++)
					{
						if( MainWorldCutPaper.transform.GetChild(0).transform.GetChild(ztor).renderer != null)
						{
							if( MainWorldCutPaper.transform.GetChild(0).transform.GetChild(ztor).renderer.material.color != Color.grey)
							{
								 MainWorldCutPaper.transform.GetChild(0).transform.GetChild(ztor).renderer.material.color = Color.grey;
							}
						}
					}
				}
				
				
				
				
				for(int ztor = 0; ztor < MainWorldCutPaper.transform.GetChildCount(); ztor++)
				{
					if(MainWorldCutPaper.transform.GetChild(ztor).renderer != null)
					{
						if(MainWorldCutPaper.transform.GetChild(ztor).renderer.material.color != Color.grey)
						{	
							MainWorldCutPaper.transform.GetChild(ztor).renderer.material.color = Color.grey;
						}
					}
				}
				*/
				
				//UnityEngine.Debug.Log("about to call ReturnToPreviousPlatformState");
				//return all collision masking to previous state
				gameObject.GetComponent<TearCoverUp>().ReturnToPreviousPlatformState();
				
				
				BlockingTransition = false;
            }
			else if(playerCollidingWithTornPieceCheckTester)
			{
				BlockingTransition = true;
				//UnityEngine.Debug.Log("testing BlockingTransition");
				
			}

           // if (soundManagerRef.IsAudioPlaying("TearEffect", "SFX")) soundManagerRef.StopSound("TearEffect", "SFX");


            rotatingPiece = false;
            fingerLifted = true;
        }

        else
        {
            if (inputManagerRef.GetcurrPressState().Equals(InputManager.PressState.UP))
            {
                fingerLifted = true;
            }

            else
            {
                fingerLifted = false;
            }
        }

		///if(!PlayerMovingPlatformState 
		//	&& !haveFinishedMovingOffPlayer 
		//	&& !playerCollidingWithTornPieceCheckTester)
		//{
		//	haveFinishedMovingOffPlayer = true;
		//}
		
		//Check if the player sets a torn piece on top of player!!!!
		
		 /*
		if(!PlayerMovingPlatformState 
			//&& !haveFinishedMovingOffPlayer 
			&& playerCollidingWithTornPieceCheckTester 
			&& !BlockingTransition)
		{
			//UnityEngine.Debug.LogError("trying to MoveTornPieceOffPlayer");
			//Move the torn piece off player
			
			//MoveTornPieceOffPlayer();
			
			if(Camera.main.audio.clip != HurtAudio)
			{
				Camera.main.audio.clip = HurtAudio;
			}
			
			//Play rotation audio
			if(!Camera.main.audio.isPlaying && !BadTear)Camera.main.audio.Play();
			
			
			MainCutPieceRotationObjectPrevPos.z = 0.9f;
			MainCutPieceRotationObject.transform.position = MainCutPieceRotationObjectPrevPos;
			MainCutPieceRotationObject.transform.rotation = MainCutPieceRotationObjectPrevRot;
			
			
			
			if(!needToResetCollision)
				needToResetCollision = true;
		}
		else */
		
		
		/*
		if(!PlayerMovingPlatformState 
			&& needToResetCollision 
			&& !playerCollidingWithTornPieceCheckTester)
		{
			//MainCutPieceRotationObject.transform.position = new Vector3(MainCutPieceRotationObject.transform.position.x,
			//												MainCutPieceRotationObject.transform.position.y, 
			//												GVariables.OnBottomDepth);
			
			needToResetCollision = false;
			
			//MainCutPieceRotationObject.rigidbody.velocity = new Vector3(0, 0, 0);
			
			foreach(GameObject platform in objectsBelongingToCutPiece)
			{
				platform.transform.parent = null;
			}
			
			foreach(GameObject platform in objectsBelongingToCutPiece)
			{
				platform.transform.position = new Vector3(platform.transform.position.x, platform.transform.position.y, 0.0f);
			}
			
			//Disable All torn piece platform collision
			EnableTornChildrenColliders(false);
			//return all collision masking to previous state
			gameObject.GetComponent<TearCoverUp>().CheckToMaskPlatforms(MainWorldCutPaper);
			//Re-enable All torn piece platform collision
			EnableTornChildrenColliders(true);
			
			
		}*/
		
		if(PlayerMovingPlatformState && PlayerInputInBounds() )//&& !playerCollidingWithTornPieceCheckTester)
		{
            // ROTATING THE PIECE - D.A.
            if ((Input.GetKey(KeyCode.Q) && !gameStateManagerRef.OnMobileDevice()) 
				
				
				||
                ((!PointInsideObject(MainWorldCutPaper, touchController.GetLastFingerPosition()) || rotatingPiece) &&
                !touchController.ReturnTouchType().Equals(TouchType.MULTITOUCH_2) && 
                !inputManagerRef.DoubleTap() &&
                inputManagerRef.GetcurrPressState().Equals(InputManager.PressState.DOWN) &&
                gameStateManagerRef.OnMobileDevice())
				)
			{
                // ensuring init rotating piece
                // is only true for one loop iteration
                if (gameStateManagerRef.OnMobileDevice())
                {

                    Vector3 cutPieceCenter = FindCenterOfMeshObject(MainWorldCutPaper);

                    double rotation = Math.Atan2(touchController.GetLastFingerPosition().y - Camera.mainCamera.WorldToScreenPoint(cutPieceCenter).y,
                                                 Camera.mainCamera.WorldToScreenPoint(cutPieceCenter).x - touchController.GetLastFingerPosition().x) * 180 / Math.PI;

                    // atan2 returns values from -180 to 180
                    if (rotation < 0)
                        rotation = 360 + rotation;

                    if (initRotatingPiece)
                        overallRot = (float)rotation;

                    MainWorldCutPaper.transform.RotateAround(cutPieceCenter, new Vector3(0, 0, 1), (float)(overallRot - rotation));

                    // update the overall rotation
                    overallRot = (float)rotation;


                    // global rotate flag for entire clsss
                    rotatingPiece = true;

                    //UnityEngine.Debug.Log("ROT " + rotation);
                    //UnityEngine.Debug.Log("PAPER " + Camera.mainCamera.WorldToScreenPoint(centerOfCuttPieceRotOffset).ToString() + "\nFINGER " + touchController.GetLastFingerPosition().ToString());
                    //UnityEngine.Debug.Log("LOOK " + ((rotation.z - CurrentCuttPiece.transform.rotation.z) * 180 / Math.PI).ToString() + "\nCURR " + currRot); 
                    //UnityEngine.Debug.Log("DIFF " + ((rotation.z - CurrentCuttPiece.transform.rotation.z) * 180 / Math.PI).ToString()); 
                }

                else
                {
                    MainCutPieceRotationObject.transform.RotateAround(newPosForTear, new Vector3(0, 0, 1), RotationSpeed * (Time.deltaTime*100));
                }
				
				if(Camera.main.audio.clip != RotationAudio)
				{
					Camera.main.audio.clip = RotationAudio;
				}
				
				//Play rotation audio
				if(!Camera.main.audio.isPlaying && !BadTear)Camera.main.audio.Play();
			}
			else if(Input.GetKey(KeyCode.E) && !gameStateManagerRef.OnMobileDevice()
				||
                ((!PointInsideObject(MainWorldCutPaper, touchController.GetLastFingerPosition()) || rotatingPiece) &&
                !touchController.ReturnTouchType().Equals(TouchType.MULTITOUCH_2) && 
                !inputManagerRef.DoubleTap() &&
                inputManagerRef.GetcurrPressState().Equals(InputManager.PressState.DOWN) &&
                gameStateManagerRef.OnMobileDevice())
				)
			{
                // ensuring init rotating piece
                // is only true for one loop iteration
                if (gameStateManagerRef.OnMobileDevice())
                {

                    Vector3 cutPieceCenter = FindCenterOfMeshObject(MainWorldCutPaper);

                    double rotation = Math.Atan2(touchController.GetLastFingerPosition().y - Camera.mainCamera.WorldToScreenPoint(cutPieceCenter).y,
                                                 Camera.mainCamera.WorldToScreenPoint(cutPieceCenter).x - touchController.GetLastFingerPosition().x) * 180 / Math.PI;

                    // atan2 returns values from -180 to 180
                    if (rotation < 0)
                        rotation = 360 + rotation;

                    if (initRotatingPiece)
                        overallRot = (float)rotation;

                    MainWorldCutPaper.transform.RotateAround(cutPieceCenter, new Vector3(0, 0, 1), (float)(overallRot - rotation));

                    // update the overall rotation
                    overallRot = (float)rotation;


                    // global rotate flag for entire clsss
                    rotatingPiece = true;

                    //UnityEngine.Debug.Log("ROT " + rotation);
                    //UnityEngine.Debug.Log("PAPER " + Camera.mainCamera.WorldToScreenPoint(centerOfCuttPieceRotOffset).ToString() + "\nFINGER " + touchController.GetLastFingerPosition().ToString());
                    //UnityEngine.Debug.Log("LOOK " + ((rotation.z - CurrentCuttPiece.transform.rotation.z) * 180 / Math.PI).ToString() + "\nCURR " + currRot); 
                    //UnityEngine.Debug.Log("DIFF " + ((rotation.z - CurrentCuttPiece.transform.rotation.z) * 180 / Math.PI).ToString()); 
                }

                else
                {
                    MainCutPieceRotationObject.transform.RotateAround(newPosForTear, new Vector3(0, 0, 1), RotationSpeed * (Time.deltaTime*-100));
                }
				
				if(Camera.main.audio.clip != RotationAudio)
				{
					Camera.main.audio.clip = RotationAudio;
				}
				
				//Play rotation audio
				if(!Camera.main.audio.isPlaying && !BadTear)Camera.main.audio.Play();
			}
			else
			{
				//Stop rotation audio
				if(Camera.main.audio.isPlaying)Camera.main.audio.Pause();
			}
			
		}

        else 
        {
			///THE FOLLOWING TRY CATCH IS TO PREVENT INPUT REFERENCE ERROR - J.C.
			try
			{
				if(!PlayerInputInBounds() && Camera.main.audio.isPlaying)
				{
            		Camera.main.audio.Pause();
				}
			}
			catch
			{
				//UnityEngine.Debug.Log("MIS UNDERSTOOD ERROR");
			}
        }
		
		//The following boolean checks for update player's movement direction.
		//This is used to auto translate torn piece if player set piece on
		//top of player or another nonCoverable object
		if(PlayerMovingPlatformState)
		{
			//Check for uninitialized values
			if(currentMouseWorldPos == Vector2.zero)
			{
                currentMouseWorldPos = new Vector2(currentPlayerInputPos.x, currentPlayerInputPos.y);
			}
			if(prevMouseWorldPos == Vector2.zero)
			{
                prevMouseWorldPos = new Vector2(currentPlayerInputPos.x, currentPlayerInputPos.y);
			}
			
			//Check if the player has moved atlease the minDist to reassign movement values
            if (Vector2.Distance(currentMouseWorldPos, new Vector2(currentPlayerInputPos.x, currentPlayerInputPos.y)) >= minMouseDistance)
			{
				prevMouseWorldPos = currentMouseWorldPos;
                currentMouseWorldPos = new Vector2(currentPlayerInputPos.x, currentPlayerInputPos.y);
			}
		}
	}
	
	/// <summary>
	/// The blocking transition flags when the player is on top of the torn piece
	/// preventing the torn piece from returning to previous position
	/// </summary>
	private bool BlockingTransition = false;
	
	/// <summary>
	/// The previous position covering fold flag to ensure the correct depth is assigned
	/// when player places torn piece on top of player when the previous position was on
	/// top of the fold
	/// </summary>
	private bool previousPositionCoveringFoldFlag = false;
	
	/// <summary>
	/// Enables the torn children colliders.
	/// </summary>
	private void EnableTornChildrenColliders(bool enable)
	{
		for(int itor = 0; itor < MainWorldCutPaper.transform.GetChildCount(); itor++)
		{
			if(MainWorldCutPaper.transform.GetChild(itor).collider is MeshCollider)
			{
				MainWorldCutPaper.transform.GetChild(itor).collider.enabled = enable;
				
				for(int jtor = 0; jtor < MainWorldCutPaper.transform.GetChild(itor).transform.GetChildCount(); jtor++)
				{
					MainWorldCutPaper.transform.GetChild(itor).transform.GetChild(jtor).collider.enabled = enable;
				}
			}
		}
	}
	
	/// <summary>
	/// Moves the torn piece off player.
	/// </summary>
	private void MoveTornPieceOffPlayer()
	{
		
		Vector2 movDir = prevMouseWorldPos - currentMouseWorldPos;
		
		movDir.Normalize();
		
		float speed = 0.1f;
		
		movDir *= speed;
		
		MainCutPieceRotationObject.transform.position += new Vector3(movDir.x, movDir.y, 0);//GVariables.OnBottomDepth);

		//MainCutPieceRotationObject.rigidbody.velocity = new Vector3(movDir.x, movDir.y, 0);
	}
	
	/// <summary>
	/// Parents the objects to world cut piece.
	/// </summary>
	private void ParentObjectsToWorldCutPiece()
	{
		foreach(GameObject go in objectsBelongingToCutPiece)
		{
			go.transform.parent = MainWorldCutPaper.transform;
		}
		
		
		/*
		//*********TESTING*********
		foreach(GameObject go in objectsBelongingToCutPiece)
		{
			go.transform.RotateAround(centerPositions[go], new Vector3(0, 0, 1), 1);
		}
		*/
		
		//Ensure this logic is only hit once
		needToSetParentChildRelations = false;
		
		//Flag playerCompletedTear so that they can move and rotate tonr piece
		playerCompletedTear = true;
		
		//Flag the movement the player is in the movement state of the torn piece
		PlayerMovingPlatformState = true;
				
		//save previous position
		MainCutPieceRotationObjectPrevPos = MainCutPieceRotationObject.transform.position;
		MainCutPieceRotationObjectPrevRot = MainCutPieceRotationObject.transform.rotation;
		
		
		MainStartingWorldPaper.GetComponent<TearPaper>().enabled = false;
	}
	
	/// <summary>
	/// Checks any of the torn piece vertices covering the fold
	/// </summary>
	/// <returns>
	/// This returns true if the torn piece is covering any folded region
	/// </returns>
	public bool CheckTornPieceCoveringFold()
	{
		MainWorldCutPaper.GetComponent<MeshCollider>().enabled = false;
		
		int hitCount = 0;
		for(int itor = 0; itor < MainWorldCutPaperTriangles.Count(); itor++)
		{
			RaycastHit hit;
			Vector3 rayPos = MainWorldCutPaper.transform.TransformPoint(MainWorldCutPaperVertices[MainWorldCutPaperTriangles[itor]]);
			rayPos.z = -10;
			bool hitObject = Physics.Raycast(rayPos, Camera.main.transform.forward, out hit, 20);
			
			if(hitObject )
			{
				if((hit.collider.gameObject.tag == "unfoldborder" 
					|| hit.collider.gameObject.tag == "Fold" 
					|| hit.collider.gameObject.tag == "FoldPlatform" 
					|| hit.collider.gameObject.tag == "RayTraceBlocker"
					|| hit.collider.gameObject.tag == "FoldCover"))
				{
					++hitCount;
				}
			}
			
			if(hitCount >= 3)
			{
				MainWorldCutPaper.GetComponent<MeshCollider>().enabled = true;
				return true;
			}
		}
		
		MainWorldCutPaper.GetComponent<MeshCollider>().enabled = true;
		return false;
	}
	
	/// <summary>
	/// Groups the torn objects after tear complete, only called once
	/// </summary>
	private void GroupTornObjectsFindCenter()
	{
		//Find centerposition of main world cutt piece
		Vector3 newCenterPos = Vector3.zero;
		int numberOfVertices = 0;
		//Find center of every platform - those torn and not torn - only care about vertices
		//currently visible in tirangles storage
		for(int ktor =0; ktor < MainWorldCutPaperTriangles.Count(); ktor++)
		{
			newCenterPos += MainWorldCutPaper.transform.TransformPoint(MainWorldCutPaperVertices[MainWorldCutPaperTriangles[ktor]]);
			numberOfVertices++;
		}
		//Divide to find average
		newCenterPos /= numberOfVertices;
		MainWorldCutPaperCenterPos = newCenterPos;
		
		MainCutPieceRotationObject = new GameObject();
		
		//MainCutPieceRotationObject.AddComponent<Rigidbody>();
		//MainCutPieceRotationObject.rigidbody.useGravity = false;
		
		MainCutPieceRotationObject.name = "Tear_CenterOfCutPieceRotation";
		MainCutPieceRotationObject.transform.position = MainWorldCutPaperCenterPos;
		MainWorldCutPaper.transform.parent = MainCutPieceRotationObject.transform;
		
		//Debug.Log("center " + MainWorldCutPaperCenterPos.ToString());
		
		//RemoveOldPlatforms();
		
		centerPositions.Clear();
		
		//Create dictionary centerPositions
		foreach(GameObject go in Platforms)
		{
			centerPositions.Add(go, new Vector3(0, 0, 0));
		}
		foreach(GameObject go in TornPlatforms)
		{
			if(!centerPositions.ContainsKey(go))
			{
				centerPositions.Add(go, new Vector3(0, 0, 0));
			}
		}
		
		//Debug.LogError("********testing tear manager triggered******** length of centerPositions = " + centerPositions.Keys.Count().ToString());
		
		FindCenterOfObject();
		
		//Assign parent child relations
		DetermineIfPlatformOnCutPiece();
		
		//Ensure this bool logic is only triggered once each time set to true outside
		// from an instance of TearPaper
		//TearFinished = false;
		HaveTornOnce = true;
		
		//Testing center of object
		needToSetParentChildRelations = true;
	}
	
	/// <summary>
	/// Resets the parenting platforms.
	/// </summary>
	public void ResetParentingPlatforms()
	{
		foreach(GameObject key in objectsBelongingToCutPiece)
		{
			key.transform.parent = null;
		}
		
		objectsBelongingToCutPiece.Clear();	
	}
	
	/// <summary>
	/// Determines if platform on cut world piece.
	/// </summary>
	private void DetermineIfPlatformOnCutPiece()
	{
		if(FoldBorderOutside == null)
		{
			FoldBorderOutside = GameObject.FindGameObjectWithTag("foldborder");
		}
		
		if(FoldBorderInside == null)
		{
			FoldBorderInside = GameObject.FindGameObjectWithTag("insideBorder");
		}
		
		/*if(FoldBorderOutside != null)
			FoldBorderOutside.collider.enabled = false;
		if(FoldBorderInside != null)
			FoldBorderInside.collider.enabled = false;*/
		List<GameObject> centerPosKeys = centerPositions.Keys.ToList();
		//Raycast from each center position to determine whether it belongs 
		//on cutt piece or not
		for(int itor = 0; itor < centerPosKeys.Count(); itor++)
		{
			
			RaycastHit hit = new RaycastHit();
			Vector3 direction = Camera.main.transform.forward;
			Vector3 pos = new Vector3(centerPositions[centerPosKeys[itor]].x, 
										centerPositions[centerPosKeys[itor]].y, 
										centerPosKeys[itor].transform.position.z + 0.2f);
			if (Physics.Raycast (pos, direction, out hit, 10)) 
			{
				//Debug.Log(centerPositions.Keys.ElementAt(itor).gameObject.name.ToString() + " is hitting object with name = " + hit.collider.gameObject.name.ToString());
				
				//Check if the object is hitting cutt piece
            	if(hit.collider.gameObject.name == "paper_CuttPieceOfPaper" && hit.collider.gameObject != centerPosKeys[itor])
				{
       				objectsBelongingToCutPiece.Add(centerPosKeys[itor]);
  				}
       		}
       		
			
			//Vector3 tempPos = new Vector3(centerPositions[centerPositions.Keys.ElementAt(itor)].x, centerPositions[centerPositions.Keys.ElementAt(itor)].y, MainWorldCutPaper.transform.position.z);
			//if(MainWorldCutPaper.GetComponent<MeshCollider>().bounds.Contains(tempPos))
			//{
			//	objectsBelongingToCutPiece.Add(centerPositions.Keys.ElementAt(itor));
			//}
			
		}
		/*
		if(FoldBorderOutside != null)
			FoldBorderOutside.collider.enabled = true;
		if(FoldBorderInside != null)
			FoldBorderInside.collider.enabled = true;*/
	}


    /// <summary>
    /// Finds the center of object with a mesh.
    /// </summary>
    private Vector3 FindCenterOfMeshObject(GameObject gObject)
    {
        //Flag number of vertices 
        int numberOfvertives = 0;

        //Create vector to return as object's center position
        Vector3 returnVal = Vector3.zero;

        //Create storage to remove duplicate vertice indexex from triangle array
        List<int> VertFaceIndicies = new List<int>();

        //Loop through the vertices currently being drawn (determined by triangles currently visible)
        for (int itor = 0; itor < gObject.GetComponent<MeshFilter>().mesh.triangles.Count(); itor++)
        {
            ++numberOfvertives;

            //The key here is translating local coordinated into global coordinates by using Transform.TransformPoint()
            returnVal += gObject.transform.TransformPoint(gObject.GetComponent<MeshFilter>().mesh.vertices[gObject.GetComponent<MeshFilter>().mesh.triangles[itor]]);
        }
        //Divide by number of vertices to find center position
        returnVal /= numberOfvertives;

        return returnVal;
    }



    /// <summary>
	/// Finds the center of all interactable game objects
	/// </summary>
	private void FindCenterOfObject()
	{
		List<GameObject> centerPosKeysList = centerPositions.Keys.ToList();
		//Now loop through each object, find the center and store into centerPositions
		for(int itor = 0; itor < centerPosKeysList.Count(); itor++)
		{
			/*
			//Create position to represent the center of the current object
			Vector3 currObjectCenter = new Vector3(0, 0, 0);
			
			int frequency = 0;
			
			//Find center of every platform - those torn and not torn - only care about vertices
			//currently visible in tirangles storage
			for(int ktor =0; ktor < centerPositions.Keys.ElementAt(itor).GetComponent<MeshFilter>().mesh.triangles.Count(); ktor++)
			{
				currObjectCenter += centerPositions.Keys.ElementAt(itor).transform.TransformPoint(centerPositions.Keys.ElementAt(itor).GetComponent<MeshFilter>().mesh.vertices[centerPositions.Keys.ElementAt(itor).GetComponent<MeshFilter>().mesh.triangles[ktor]]);
				frequency++;
			}
			//Divide to find average
			currObjectCenter /= frequency;
			*/
			Vector3 currObjectCenter = centerPosKeysList[itor].gameObject.transform.position;
			
			//Store center Position
			centerPositions[centerPosKeysList[itor]] = currObjectCenter;
			
			//Debug.Log(centerPositions.Keys.ElementAt(itor).name + " has a center = " + currObjectCenter.ToString());
		}
	}
	
	/// <summary>
	/// Removes the old platforms from tearable game level scene
	/// </summary>
	private void RemoveOldPlatforms()
	{
		//Create structure to destroy excess objects not being used
		Dictionary<GameObject, bool> oldTornPlatforms = new Dictionary<GameObject, bool>();
		
		List<GameObject> deletedObjectsPlatforms = new List<GameObject>();
		List<GameObject> deletedObjectsTornPlatforms = new List<GameObject>();
		//remove excess objects
		foreach(GameObject go in Platforms)
		{
			if(go != null)
			{
				if(!go.GetComponent<MeshRenderer>().enabled)
				{
					oldTornPlatforms.Add(go, true);
				}
			}
			else
			{
				deletedObjectsPlatforms.Add(go);
			}
		}
		
		foreach(GameObject go in TornPlatforms)
		{
			if(go != null)
			{
				if(!go.GetComponent<MeshRenderer>().enabled)
				{
					oldTornPlatforms.Add(go, true);
				}
			}
			else
			{
				deletedObjectsTornPlatforms.Add(go);
			}
		}
		
		List<GameObject> newPlatforms = new List<GameObject>();
		for(int itor = 0; itor < Platforms.Count(); itor++)
		{
			bool check = true;
			foreach(GameObject go in deletedObjectsPlatforms)
			{
				if(Platforms.ElementAt(itor) == go)
				{
					check = false;
				}
			}
			
			if(check)
			{
				newPlatforms.Add(Platforms.ElementAt(itor));
			}
		}
		Platforms = newPlatforms;
		deletedObjectsPlatforms.Clear();
		
		List<GameObject> newTornPlatforms = new List<GameObject>();
		for(int itor = 0; itor < TornPlatforms.Count(); itor++)
		{
			bool check = true;
			foreach(GameObject go in deletedObjectsTornPlatforms)
			{
				if(TornPlatforms.ElementAt(itor) == go)
				{
					check = false;
				}
			}
			
			if(check)
			{
				newTornPlatforms.Add(TornPlatforms.ElementAt(itor));
			}
		}
		TornPlatforms = newTornPlatforms;
		deletedObjectsTornPlatforms.Clear();

		
		//Now, oldTornPlatforms contains all old platforms needing to be erased
		for(int itor = 0; itor < oldTornPlatforms.Keys.Count(); itor++)
		{
			if(oldTornPlatforms[oldTornPlatforms.Keys.ElementAt(itor)])
			{
				//Debug.LogError("Removing Test, object = " + oldTornPlatforms.Keys.ElementAt(itor).name.ToString());
				GameObject.Destroy(oldTornPlatforms.Keys.ElementAt(itor));
				Platforms.Remove(oldTornPlatforms.Keys.ElementAt(itor));
			}
		}
		oldTornPlatforms = null;
		
	}
	
	/// <summary>
	/// Players the on torn piece check.
	/// </summary>
	private bool PlayerOnTornPieceCheck()
	{
		return PlayerCollidingWithTornPieceCheck();
		
		
		Vector3 playerPos1 = PlayerObjectRef.transform.position;
		playerPos1.x += PlayerObjectRef.GetComponent<CapsuleCollider>().bounds.extents.x;
		playerPos1.y += PlayerObjectRef.GetComponent<CapsuleCollider>().bounds.extents.y;
		playerPos1.z = MainWorldCutPaper.collider.transform.position.z;
		
		Vector3 playerPos2 = PlayerObjectRef.transform.position;
		playerPos2.x += PlayerObjectRef.GetComponent<CapsuleCollider>().bounds.extents.x;
		playerPos2.y -= PlayerObjectRef.GetComponent<CapsuleCollider>().bounds.extents.y;
		playerPos2.z = MainWorldCutPaper.collider.transform.position.z;
		
		Vector3 playerPos3 = PlayerObjectRef.transform.position;
		playerPos3.x -= PlayerObjectRef.GetComponent<CapsuleCollider>().bounds.extents.x;
		playerPos3.y += PlayerObjectRef.GetComponent<CapsuleCollider>().bounds.extents.y;
		playerPos3.z = MainWorldCutPaper.collider.transform.position.z;
		
		Vector3 playerPos4 = PlayerObjectRef.transform.position;
		playerPos4.x -= PlayerObjectRef.GetComponent<CapsuleCollider>().bounds.extents.x;
		playerPos4.y -= PlayerObjectRef.GetComponent<CapsuleCollider>().bounds.extents.y;
		playerPos4.z = MainWorldCutPaper.collider.transform.position.z;
		int vertCount = 0;
		
		for(int itor = 0; itor < MainWorldCutPaperTriangles.Count(); itor ++)
		{
			Vector3 vertPos = MainWorldCutPaper.transform.TransformPoint(MainWorldCutPaperVertices[MainWorldCutPaperTriangles[itor]]);
			playerPos1.z = vertPos.z;
			float dist1 = Vector3.Distance(playerPos1, vertPos);
			if(dist1 < 0) dist1 *= -1;
			
			
			playerPos2.z = vertPos.z;
			float dist2 = Vector3.Distance(playerPos2, vertPos);
			if(dist2 < 0) dist2 *= -1;
			
			playerPos3.z = vertPos.z;
			float dist3 = Vector3.Distance(playerPos3, vertPos);
			if(dist3 < 0) dist3 *= -1;
			
			playerPos4.z = vertPos.z;
			float dist4 = Vector3.Distance(playerPos4, vertPos);
			if(dist4 < 0) dist4 *= -1;
			
			
			//The following will result in the torn piece moving JUST off the player
			//(i.e. doesnt move too far away from player)
			if(dist1 <= (MainWorldCutPaper.GetComponent<TearPaper>().MESH_VERT_OFFSET)
				|| dist2 <= (MainWorldCutPaper.GetComponent<TearPaper>().MESH_VERT_OFFSET)
				|| dist3 <= (MainWorldCutPaper.GetComponent<TearPaper>().MESH_VERT_OFFSET)
				|| dist4 <= (MainWorldCutPaper.GetComponent<TearPaper>().MESH_VERT_OFFSET))
			{
				++vertCount;
			}
			
			if(vertCount >= 1)
			{
				return true;
			}
		}
		
		return false;
		
		
	}
	
	/// <summary>
	/// Players the colliding material change.
	/// </summary>
	/// <returns>
	/// The colliding material change.
	/// </returns>
	public bool PlayerCollidingMaterialChange()
	{
		
		MainWorldCutPaper.GetComponent<MeshCollider>().enabled = false;
		
		Vector3 playerPos = PlayerObjectRef.transform.position;
		//playerPos.z = MainWorldCutPaper.transform.position.z;
		int vertCount = 0;
		
		for(int itor = 0; itor < MainWorldCutPaperTriangles.Count(); itor ++)
		{
			Vector3 vertPos = MainWorldCutPaper.transform.TransformPoint(MainWorldCutPaperVertices[MainWorldCutPaperTriangles[itor]]);
			playerPos.z = vertPos.z;
			float dist = Vector3.Distance(playerPos, vertPos);
			
			//The following will result in the torn piece moving JUST off the player
			//(i.e. doesnt move too far away from player)
			if(dist <= PlayerObjectRef.collider.bounds.size.x)
			{
				++vertCount;
			}
			
			if(vertCount >= 3)
			{
				break;
			}
		}
		
		MainWorldCutPaper.GetComponent<MeshCollider>().enabled = true;
		if(vertCount >= 3)
		{
			return true;
		}
		else
		{
			return false;
		}
		
	}
	
	/// <summary>
	/// The player graphics and collision.
	/// </summary>
	public GameObject PlayerGraphicsAndCollision;
	
	
	/// <summary>
	/// Players the colliding with torn piece check.
	/// </summary>
	/// <returns>
	/// The colliding with torn piece check.
	/// </returns>
	public bool PlayerCollidingWithTornPieceCheck()
	{
		//TODO OPTIMIZE AND RE-DEFINE
		//return false;
		if(MainWorldCutPaper == null) return false;
		
		//UnityEngine.Debug.Log("PlayerCollidingWithTornPieceCheck returning = " + PlayerGraphicsAndCollision.GetComponent<UnfoldCollision>().GetTornOverPlayer().ToString());
		//return PlayerGraphicsAndCollision.GetComponent<UnfoldCollision>().GetTornOverPlayer();
		
		MainWorldCutPaper.GetComponent<MeshCollider>().enabled = false;
		Vector3 playerPos = PlayerObjectRef.transform.position;
		
		for(int itor = 0; itor < MainWorldCutPaperTriangles.Count(); itor++)
		{
			Vector3 vertPos = MainWorldCutPaper.transform.TransformPoint(MainWorldCutPaperVertices[MainWorldCutPaperTriangles[itor]]);
			vertPos.z = playerPos.z;
			
			if(PlayerObjectRef.GetComponent<CapsuleCollider>().bounds.Contains(vertPos))
			{
				//UnityEngine.Debug.LogError("Bounds contain player -- true");
				MainWorldCutPaper.GetComponent<MeshCollider>().enabled = true;
				return true;
			}
		}
		MainWorldCutPaper.GetComponent<MeshCollider>().enabled = true;
		//UnityEngine.Debug.LogError("Bounds contain player -- false");
		return false;
		
		
		
		/*
		PlayerCollidingMaterialChange();
		*/
		
		
		
		
		/*
		//if(MainWorldCutPaper.collider.bounds.Contains(playerPos))
		//{
			//UnityEngine.Debug.LogError("Bounds contain player");
			for(int itor = 0; itor < MainWorldCutPaper.GetComponent<MeshFilter>().mesh.vertices.Count(); itor ++)
			{
				if(MainWorldCutPaper.GetComponent<MeshFilter>().mesh.triangles.Contains(itor))
				{
					RaycastHit hit;
					Vector3 rayPos = MainWorldCutPaper.transform.TransformPoint(MainWorldCutPaper.GetComponent<MeshFilter>().mesh.vertices[itor]);
					rayPos.z = -10;
					bool hitObject = Physics.Raycast(rayPos, Camera.main.transform.forward, out hit, 20);
					
					if(hitObject && hit.collider.gameObject.tag == "Player")
					{
						UnityEngine.Debug.LogError("PlayerCollidingWithTornPieceCheck   true");
						MainWorldCutPaper.GetComponent<MeshCollider>().enabled = true;
						return true;
					}
					if(hitObject && hit.collider.gameObject.name == "paper_CuttPiece")
					{
						UnityEngine.Debug.LogError("paper_CuttPiece   true");
						//MainWorldCutPaper.GetComponent<MeshCollider>().enabled = true;
						//return true;
					}
				}
			}
		//}
		*/
		
		//MainWorldCutPaper.GetComponent<MeshCollider>().enabled = true;
		//return false;
		
		/*
		for(int itor = 0; itor < MainWorldCutPaper.GetComponent<MeshFilter>().mesh.vertices.Count(); itor ++)
		{
			if(MainWorldCutPaper.GetComponent<MeshFilter>().mesh.triangles.Contains(itor))
			{	
				Vector3 vertWorldPos = MainCutPieceRotationObject.transform.TransformPoint(
					MainWorldCutPaper.transform.TransformPoint(MainWorldCutPaper.GetComponent<MeshFilter>().mesh.vertices[itor]));
				
				vertWorldPos.z = PlayerObjectRef.transform.position.z;
				
				float distCheck = Vector3.Distance(vertWorldPos, PlayerObjectRef.transform.position);
				if(distCheck < 0) distCheck *= -1;
				
				float max = PlayerObjectRef.collider.bounds.extents.x;
				if(max < PlayerObjectRef.collider.bounds.extents.y)
				{
					max = PlayerObjectRef.collider.bounds.extents.y;
				}
				
				if(distCheck <= max)
				{
					return true;
				}
			}
		}
		return false;
		*/
	}
	
	/// <summary>
	/// The have hit paper flags when the player has started touching the paper
	/// to initialize tear, this is used to force a tear to stop when needed
	/// </summary>
	public bool haveHitPaper = false;
	
	/// <summary>
	/// Creates the visual tearing effect when player tears
	/// </summary>
	private void CreateTearingEffect()
	{
        if (inputManagerRef.GetcurrPressState().Equals(InputManager.PressState.DOWN) && !TearFinished)
		{
			//Play tear audio
			//if(!Camera.main.audio.isPlaying && PlayerCurrentlyTearing)Camera.main.audio.Play();
			
			
			
            Vector2 world2DPos = Camera.main.ScreenToWorldPoint(new Vector2(currentPlayerInputPos.x, currentPlayerInputPos.y));
			Vector3 worldPos = new Vector3(world2DPos.x, world2DPos.y, MainStartingWorldPaper.transform.position.z);
			
			RaycastHit hit;
			Vector3 rayPos = worldPos;
			rayPos.z -= 10;
			bool hitObject = Physics.Raycast(rayPos, Camera.main.transform.forward, out hit, 20);
			
			if(MainStartingWorldPaper.GetComponent<MeshCollider>().bounds.Contains(worldPos) 
				&& 
				(!hitObject || 
					(hitObject && hit.collider.gameObject.tag != "FoldCover"))
				)
			{
				if(!haveHitPaper)
				{
					haveHitPaper = true;
					TimerToForceFinishTear = 0;
				}
					
				if(Camera.main.audio.clip != TearAudio)
				{
					Camera.main.audio.clip = TearAudio;
				}
				
				//Play tear audio
				if(!Camera.main.audio.isPlaying && PlayerCurrentlyTearing && !BadTear)Camera.main.audio.Play();
                //if (PlayerCurrentlyTearing && !BadTear)
                //{
                //    soundManagerRef.PlayAudio("TearEffect", "SFX");
                //}
                //else soundManagerRef.StopSound("TearEffect", "SFX");
				CreateNewDecal(worldPos);
			}
			else
			{
				if(haveHitPaper)
				{
					TimerToForceFinishTear += (Time.deltaTime * 100);
					
					if(TimerToForceFinishTear > TimeToTriggerForceFinishTear)
					{
						//UnityEngine.Debug.Log("haveHitPaper testing stoping tear fx");
						MainStartingWorldPaper.GetComponent<TearPaper>().ForceFinishTearLine = true;
					}
				}
				
				
				
				//Stop tear audio
				if(Camera.main.audio.isPlaying)Camera.main.audio.Pause();
			}
		}
		else
		{
			//Stop tear audio
            if (Camera.main)
                if (Camera.main.audio)
			        if(Camera.main.audio.isPlaying)Camera.main.audio.Pause();
		}
	}
	
	private float TimeToTriggerForceFinishTear = 10;
	private float TimerToForceFinishTear = 0;
	
	/// <summary>
	/// The tear decal pool
	/// </summary>
	private GameObject[] TearDecals;
	
	/// <summary>
	/// The tear decal indexor.
	/// </summary>
	private int tearDecalIndexor = 0;
	
	/// <summary>
	/// Creates the decal pool.
	/// </summary>
	private void CreateDecalPool()
	{
		TearDecals = new GameObject[50];
		
		for(int itor = 0; itor < 50; itor++)
		{
			GameObject newdecal = (GameObject)Instantiate(DecalObject);
			newdecal.transform.position = new Vector3(-1000, -1000, -1);
			newdecal.transform.RotateAround(new Vector3(0, 1, 0), 180);
			newdecal.transform.localScale = new Vector3(0.2f, 0.2f, 1.0f);
			//decalObjects.Add(newdecal, 0);
			TearDecals[itor] = newdecal;
		}
	}
	
	
	/// <summary>
	/// Creates the new decal GameObject
	/// </summary>
	private void CreateNewDecal(Vector3 pos)
	{
		//Create new particle decal object
		GameObject newdecal = TearDecals[tearDecalIndexor];
	
		++tearDecalIndexor;
		if(tearDecalIndexor >= TearDecals.Length)
		{
			tearDecalIndexor = 0;
		}
		
		newdecal.transform.position = new Vector3(pos.x, pos.y, -1);
		newdecal.transform.RotateAround(new Vector3(0, 1, 0), 180);
		newdecal.transform.localScale = new Vector3(0.2f, 0.2f, 1.0f);
		decalObjects.Add(newdecal, 0);
		
		if(!haveCheckedDecalCollision.ContainsKey(newdecal))
			haveCheckedDecalCollision.Add (newdecal, false);
	}
	
	private int badTearTimer = 0;
	private int displayBadTearLine = 90;
	public bool hittingFoldedArea = false;
	
	/// <summary>
	/// Updates the OLD BAD TEAR decal objects.
	/// </summary>
	private void UpdateDecalObjectsOldBadTear()
	{
		int millisecondsTime = (int)(Time.deltaTime * 100);
		badTearTimer += millisecondsTime;
		
		for(int itor = 0; itor < oldDecalObjects.Keys.Count(); itor++)
		{
			
			//PULSATE BAD TEARLINE
			if(badTearTimer >= 0 && badTearTimer < 30)
			{
				oldDecalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().enabled = true;
			}
			if(badTearTimer >= 30 && badTearTimer < 40)
			{
				oldDecalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().enabled = false;
			}
			if(badTearTimer >= 40 && badTearTimer < 70)
			{
				oldDecalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().enabled = true;
			}
			if(badTearTimer >= 70 || badTearTimer > displayBadTearLine)
			{
				oldDecalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().enabled = false;
				
				if(badTearTimer > displayBadTearLine)
				{
					//GameObject.Destroy(oldDecalObjects.Keys.ElementAt(itor));
					oldDecalObjects.Keys.ElementAt(itor).transform.position = new Vector3(-1000,-1000, 6);
					
					oldDecalObjects.Remove(oldDecalObjects.Keys.ElementAt(itor));
				}
			}
			
		}
	}
	
	/// <summary>
	/// The fold border reference
	/// </summary>
	public GameObject FoldBorder;
	
	/// <summary>
	/// Updates the decal objects.
	/// </summary>
	private void UpdateDecalObjects()
	{
		/*
		if(BadTear)
		{
			//++badTearTimer;
			int millisecondsTime = (int)(Time.deltaTime * 100);
			badTearTimer += millisecondsTime;
		}
		*/
		
		for(int itor = 0; itor < decalObjects.Keys.Count(); itor++)
		{
			//increment the time each decal is alive
			++decalObjects[decalObjects.Keys.ElementAt(itor)];
			
			
			//Check for decal Object colliding with folded region to prevent
			//tearing about folded areas of world paper
			if(!hittingFoldedArea && !haveCheckedDecalCollision[decalObjects.Keys.ElementAt(itor)])
			{
				//FoldBorder.GetComponent<MeshCollider>().enabled = false;
				
				RaycastHit hit;
				Vector3 rayPos = decalObjects.Keys.ElementAt(itor).transform.position;
				rayPos.z -= 10;
				bool hitObject = Physics.Raycast(rayPos, Camera.main.transform.forward, out hit, 20);
				
				//UnityEngine.Debug.LogError("currently hitting " + hit.collider.gameObject.tag.ToString());
				
				if(hitObject && (
					hit.collider.gameObject.tag == "Fold" 
					||  
								hit.collider.gameObject.tag == "unfoldborder"
					||  
								hit.collider.gameObject.tag == "FoldPlatform")
					)
				{
                    //UnityEngine.Debug.LogError("Raytest");
					hittingFoldedArea = true;
				
				}
				haveCheckedDecalCollision[decalObjects.Keys.ElementAt(itor)] = true;
			}
			
			
			
			//
			/*
			if(decalObjects[decalObjects.Keys.ElementAt(itor)] > DecalLife)
			{
				//GameObject.Destroy(decalObjects.Keys.ElementAt(itor));
				//decalObjects.Remove(decalObjects.Keys.ElementAt(itor));
				decalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().enabled = false;
			}
			else
			{
				//Here we update the size of each new Decal Object
				decalObjects.Keys.ElementAt(itor).transform.localScale = 
					new Vector3(decalObjects.Keys.ElementAt(itor).transform.localScale.x - DecalShrinkSpeed,
						decalObjects.Keys.ElementAt(itor).transform.localScale.y - DecalShrinkSpeed,
						decalObjects.Keys.ElementAt(itor).transform.localScale.z - DecalShrinkSpeed);
				
			}
			*/
			
			/*
			if(BadTear)
			{
				decalObjects.Keys.ElementAt(itor).transform.localScale = new Vector3(0.2f, 0.2f, 1.0f);
				//Debug.LogError("test red #1");
				
				decalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().material.color = Color.red;
				
				if(!decalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().enabled)
				{
					decalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().enabled = true;
					decalObjects[decalObjects.Keys.ElementAt(itor)] = 0;
					decalObjects.Keys.ElementAt(itor).transform.rotation = new Quaternion(0,180,0,0);
				}
				
				//PULSATE BAD TEARLINE
				if(badTearTimer >= 0 && badTearTimer < 30)
				{
					decalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().enabled = true;
				}
				if(badTearTimer >= 30 && badTearTimer < 40)
				{
					decalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().enabled = false;
				}
				if(badTearTimer >= 40 && badTearTimer < 70)
				{
					decalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().enabled = true;
				}
				if(badTearTimer >= 70 || badTearTimer > displayBadTearLine)
				{
					decalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().enabled = false;
					
					if(badTearTimer > displayBadTearLine)
					{
						GameObject.Destroy(decalObjects.Keys.ElementAt(itor));
						decalObjects.Remove(decalObjects.Keys.ElementAt(itor));
					}
				}
				
				/*
				if(badTearTimer > displayBadTearLine)
				{
					GameObject.Destroy(decalObjects.Keys.ElementAt(itor));
					decalObjects.Remove(decalObjects.Keys.ElementAt(itor));
				}
				
			}
		*/
			//else
			{
				if(decalObjects[decalObjects.Keys.ElementAt(itor)] > DecalLife)
				{
					//GameObject.Destroy(decalObjects.Keys.ElementAt(itor));
					decalObjects.Keys.ElementAt(itor).transform.position = new Vector3(-1000, -1000, 6);
					
					decalObjects.Remove(decalObjects.Keys.ElementAt(itor));
					//decalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().enabled = false;
				}
				else if(decalObjects.Keys.ElementAt(itor) != null)
				{
					decalObjects.Keys.ElementAt(itor).transform.rotation = new Quaternion(0,180,0,0);
					
					//Here we update the size of each new Decal Object
					decalObjects.Keys.ElementAt(itor).transform.localScale = 
						new Vector3(decalObjects.Keys.ElementAt(itor).transform.localScale.x - DecalShrinkSpeed,
							decalObjects.Keys.ElementAt(itor).transform.localScale.y - DecalShrinkSpeed,
							decalObjects.Keys.ElementAt(itor).transform.localScale.z);
					
					//Check for decal object coliding with untearable object
					if((DecalCollidedWithUntearableObject(decalObjects.Keys.ElementAt(itor)) &&
						decalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().material.color != Color.red)
						|| 
						tornThroughBadObject)
					{
						//Debug.LogError("test red #2");
						decalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().material.color = Color.red;
						tornThroughBadObject = true;
						BadTear = true;
						
						if(Camera.main.audio.isPlaying)
							Camera.main.audio.Pause();
					}
					
				}
			}
			/*
			if(TearFinished && !BadTear 
				&& decalObjects.ContainsKey(decalObjects.Keys.ElementAt(itor)) &&
				decalObjects.Keys.ElementAt(itor) != null)
			{
				GameObject.Destroy(decalObjects.Keys.ElementAt(itor));
				decalObjects.Remove(decalObjects.Keys.ElementAt(itor));
			}
			*/
		}
		
		//Here we force all tearable Objects to ForceStoptear
		if(playerCompletedTear && tornThroughBadObject)
		{
			//Debug.LogError("****************testing reseting decal infor in tear manager");
			
		
			
			ManagerForceStopTear("tornThroughBadObject");
			
		}
		
	}
	
	/// <summary>
	/// force stop tear from manaager
	/// </summary>
	private void ManagerForceStopTear(string reason)
	{
		//UnityEngine.Debug.LogError("ManagerForceStopTear");
		
		//foreach(GameObject go in Platforms)
		//{
		//	go.GetComponent<TearPaper>().ForceStopTear();
		//}
		
		TornPlatforms.Clear();
	  	//if(!BadTear)
			MainStartingWorldPaper.GetComponent<TearPaper>().ForceStopTear(reason);
		decalObjects.Clear();
		BadTear = false;
		badTearTimer = 0;
		tornThroughBadObject = false;
		HaveTornOnce = false;
		
		//Init
		haveCheckedDecalCollision = new Dictionary<GameObject, bool>();
		hittingFoldedArea = false;
		haveHitPaper = false;
		
		
		TearFinished  = false;
		//needToSetParentChildRelations = true;
		if(MainCutPieceRotationObject != null) //Douglas - added because it would cause game to crash if you didn't tear and player died
		{
			for(int itor = 0; itor < MainCutPieceRotationObject.transform.GetChildCount(); itor ++)
			{
				MainCutPieceRotationObject.transform.GetChild(itor).parent = null;
			}
			GameObject.Destroy(MainCutPieceRotationObject);
		}
		
		playerCompletedTear = false;
		
		//Flag the movement the player is in the movement state of the torn piece
		PlayerMovingPlatformState = false;
		
		//Start();
	}
	
	/// <summary>
	/// Player Death - therefore game reset.
	/// </summary>
	public void DeathReset()
	{
		MainStartingWorldPaper.GetComponent<TearPaper>().enabled = true;
		ManagerForceStopTear("reset");
		
		DrawBadTearVisual = false;
		
		ResetPlatforms();
	}
	
	/// <summary>
	/// Resets the platforms.
	/// </summary>
	public void ResetPlatforms()
	{
		//Store original positions of paltforms
		foreach(GameObject go in Platforms)
		{
			go.transform.rotation = Quaternion.identity;
			go.transform.position = originalPlatformPositions[go];
			
		}
	}
	
	/// <summary>
	/// Returns true if the decal GameObject is colliding with Non Tearable Object
	/// </summary>
	private bool DecalCollidedWithUntearableObject(GameObject decal)
	{
		RaycastHit hit;
		Vector3 rayPos = decal.transform.position;
		rayPos.z -= 10;
		bool hitObject = Physics.Raycast(rayPos, Camera.main.transform.forward, out hit, 20);
		
		if(hitObject && hit.collider.gameObject.tag == "Fold")
		{
			MainStartingWorldPaper.GetComponent<TearPaper>().SetBadTearVal(3);
			return true;
		}
		if(hitObject && hit.collider.gameObject.tag == "Player")
		{
			MainStartingWorldPaper.GetComponent<TearPaper>().SetBadTearVal(2);
			return true;
		}
		
		if(hitObject && hit.collider.gameObject.tag == "EndGoal")
		{
			MainStartingWorldPaper.GetComponent<TearPaper>().SetBadTearVal(1);
			return true;
		}
		
		/*
		Vector3 foldDecalPos = decal.transform.position;
		foldDecalPos.z = FoldObject.transform.position.z;
		if(FoldObject.GetComponent<BoxCollider>().bounds.Contains(foldDecalPos))
		{
			return true;
		}
		
		
		//Test for tear through player
		foldDecalPos.z = PlayerObjectRef.transform.position.z;
		if(PlayerObjectRef.GetComponent<CapsuleCollider>().bounds.Contains(foldDecalPos))
		{
			return true;
		}
		*/
		
		//TODO ADD LOGIC FOR TEARING THROUGH UNTEARABLE OBJECTS, LIKE 3D MUG (Object on paper, paper weight)
		
		return false;
	}


    public GameObject GetMainPaper()
    {
        return MainWorldPaper;
    }
	
	/// <summary>
	/// Players the input in bounds.
	/// </summary>
	/// <returns>
	/// The input in bounds.
	/// </returns>
	private bool PlayerInputInBounds()
	{
		///THE FOLLOWING TRY CATCH IS TO PREVENT INPUT REFERENCE ERROR - J.C.
		try
		{
	        Vector2 world2DPos = Camera.main.ScreenToWorldPoint(new Vector2(currentPlayerInputPos.x, currentPlayerInputPos.y));
			Vector3 worldPos = new Vector3(world2DPos.x, world2DPos.y, 0);
			if(worldPos.x <= MainWorldPaper.GetComponent<TearPaper>().MaxWorldWidth &&
				worldPos.x >= MainWorldPaper.GetComponent<TearPaper>().MinWorldWidth &&
				worldPos.y <= MainWorldPaper.GetComponent<TearPaper>().MaxWorldHeight &&
				worldPos.y >= MainWorldPaper.GetComponent<TearPaper>().MinWorldHeight)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		catch
		{
			return false;
		}
	}
}
