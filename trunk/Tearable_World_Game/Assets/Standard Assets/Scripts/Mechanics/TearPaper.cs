/// <summary>
/// 
/// FILE: 
/// 	Tear Paper
/// 
/// DESCRIPTION: 
/// 	This file is used as a testing script for tear world paper mesh
/// 
/// AUTHOR: 
/// 	John Crocker - jrcrocke@ucsc.edu
/// 
/// DATE: 
/// 	1/19/2013 - ...
/// 
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class TearPaper : MonoBehaviour {
	
	#region Public Fields
	//currently changin island1 and island2 from list to hashset to see if speed becomes better

    Vector3 curScreenPoint = new Vector3();
    Vector3 curPosition = new Vector3();
    bool rotatingPiece = false;
    bool initRotatingPiece = false;

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
    /// Player's input regardless of platform
    /// </summary>
    public Vector3 currentPlayerInputPos = new Vector3();

    /// <summary>
    /// The position of the player's input when first pressing
    /// on the screen, regardless of platform
    /// </summary>
    public Vector3 inputDownPlayerPos = new Vector3();

    /// <summary>
    /// The position of the player's input when first releasing
    /// on the screen after the drag, regardless of platform
    /// </summary>
    public Vector3 inputUpPlayerPos = new Vector3();
	
	/// <summary>
	/// The platform tear collider object is sued to trigger recording 
	/// user input for mouse tear on a tearable platform
	/// </summary>
	public GameObject PlatformTearCollider;
	
	/// <summary>
	/// The platform paper determines whether or not 
	/// this instance on TearPaper is the paperWorld
	/// or a tearable platform
	/// </summary>
	public bool PlatformPaper;
	
	/// <summary>
	/// The new paper, object being cloned intitially, only
	/// </summary>
	public GameObject newPaper;
	
	/// <summary>
	/// The mesh represents the paper world's mesh being manipulated
	/// </summary>
	public Mesh mesh;
	
	/// <summary>
	/// The clone object represents whether or not this specific instance 
	/// of the world is to be modified, the original version of the object,
	/// we do not modify, only its clones.
	/// </summary>
	public bool CloneObject;
	
	/// <summary>
	/// The draw tear line ONLY - TESTING PURPOSES!!!!!!!!!!
	/// </summary>
	public bool DrawTearLineONLY;
		
	/// <summary>
	/// This is completely determinant upon what plane mesh is being used (THIS IS CURRENTLY HARDCODED)
	/// TODO: READ IN MESH AND ASSIGN, DO NOT HARDCODE THIS VALUE :-(
	/// </summary>
	public float MESH_VERT_OFFSET;
	
	/// <summary>
	/// The screen's meshvertOffset distance
	/// </summary>
	private float SCREEN_MESH_VERT_OFFSET;
	
	/// <summary>
	/// The maximum number of triangles which can be associated as a edge vertice
	/// for this particular object's mesh/vertice topology
	/// </summary>
	public int MaxTrianglesAssociatedWithEdgeVertice;
	
	/// <summary>
	/// The clone is used for keeping track of the cloned instance for mesh 
	/// deformation and cutting for new 'island' of mesh vertices
	/// </summary>
	public GameObject Clone;
	
	/// <summary>
	/// The clone_2 is used for keeping track of the cloned instance for mesh 
	/// deformation and cutting for new 'island' of mesh vertices
	/// </summary>
	public GameObject Clone2;
	
	/// <summary>
	/// The tear line defined to store the vertices the playing is trying to tear along
	/// </summary>
	public Dictionary<Vector3, int> tearLine;
	
	/// <summary>
	/// Local reference to the scnene's Tearmanager gameObject
	/// </summary>
	private TearManager TearManager;
	
	/// <summary>
	/// The tear manager main cut piece mesh filter.
	/// </summary>
	private MeshFilter tearManagerMainCutPieceMeshFilter;
	
	/// <summary>
	/// The player object reference.
	/// </summary>
	private GameObject PlayerObjectRef;
	
	/// <summary>
	/// The end goal object reference.
	/// </summary>
	private GameObject EndGoalObjectRef;
	
	/// <summary>
	/// The mesh filter belonging to this object
	/// </summary>
	private MeshFilter thisMeshFilter;
	
	/// <summary>
	/// The bad tear material no magic carpet.
	/// </summary>
	public Material BadTearMaterialNoMagicCarpet;
	
	/// <summary>
	/// The bad tear material no door tear.
	/// </summary>
	public Material BadTearMaterialNoDoorTear;
	
	/// <summary>
	/// The bad tear material no precise tear.
	/// </summary>
	public Material BadTearMaterialNoPreciseTear;
	
	/// <summary>
	/// The bad tear material no tearing fold.
	/// </summary>
	public Material BadTearMaterialNoTearingFold;
	
	/// <summary>
	/// The bad tear material alpha.
	/// </summary>
	public Material BadTearMaterialAlpha;
	
	/// <summary>
	/// The bad tear visual.
	/// </summary>
	public GameObject BadTearVisual;
	
	#endregion
	
	#region Variables
	
	/// <summary>
	/// The color of the original object
	/// this is sued to change color of torn piece while moving
	/// </summary>
	private Color originalColor;
		
	/// <summary>
	/// The testing frequency is sued to count the number of objects that
	/// are created from recursively breaking up dynamic shapes that break
	/// the tear logic
	/// </summary>
	private int testingFrequency = 0;
	
	/// <summary>
	/// The screen point is used to store the position being 
	/// compared to an input screen position
	/// </summary>
	private Vector3 screenPoint;
	
	/// <summary>
	/// The maximum x-component value fould in 
	/// vertice of original world paper
	/// </summary>
	public float MaxWorldWidth = 0;
	
	/// <summary>
	/// The minimum x-component value fould in 
	/// vertice of original world paper
	/// </summary>
	public float MinWorldWidth = 0;
	
	/// <summary>
	/// The maximum y-component value fould in 
	/// vertice of original world paper
	/// </summary>
	public float MaxWorldHeight = 0;
	
	/// <summary>
	/// The minimum y-component value fould in 
	/// vertice of original world paper
	/// </summary>
	public float MinWorldHeight = 0;
	
	/// <summary>
	/// The new mesh represents the new mesh of the object after a cutt occurs
	/// </summary>
	Vector3[] newMesh;
	
	/// <summary>
	/// The paper grid_1 simply stores the mesh coordinates into a more accessible storage
	/// The bool flag represents whether the vert is a tear vertice
	/// </summary>
	private Dictionary<Vector2, bool> paperGridTearVertCheck;
	
	/// <summary>
	/// The paper grid organizes the mesh into a grid like structure for determineing torn pieces of paper
	/// </summary>
	private Dictionary<float, List<Vector3>> paperGrid;
	
	/// <summary>
	/// The vertice position indecies into mesh object for O(n) access.
	/// </summary>
	private Dictionary<Vector3, int> verticePosIndexIntoMesh;
	
	/// <summary>
	/// The paper grid organizes the mesh into a grid like structure for determineing torn pieces of paper
	/// </summary>
	private Dictionary<Vector3, bool> edgeOfObject;

	/// <summary>
	/// The tear vert edge FX pos list stores positions to draw tearing edge effects
	/// after player has completed a successful tear
	/// </summary>
	private List<Vector3> tearVertEdgeFXpos = new List<Vector3>();
	
	/// <summary>
	/// The tear edge FX list.
	/// </summary>
	private List<GameObject> tearEdgeFXlist = new List<GameObject>();
	
	/// <summary>
	/// The tear vert edge FX pos final list stores positions to draw tearing edge effects
	/// after player has completed a successful tear
	/// 
	/// This list is used as final once tearVertEdgeFXpos has been cleaned
	/// </summary>
	private List<Vector3> tearVertEdgeFXposFinal = new List<Vector3>();

	/// <summary>
	/// The tear edge FX container is used to store all 
	/// torn edge FX gameobjects
	/// </summary>
	private GameObject tearEdgeFXcontainer;
	
	/// <summary>
	/// The tear line time is used to store the time of the tear line input. This is used to distinguish between
	///  U-like and S-like shaped tear curves
	/// </summary>
	private Dictionary<Vector3, float> tearLineTime;
	
	/// <summary>
	/// The tear line time is used to store the time of the tear line input. This is used to distinguish between
	///  U-like and S-like shaped tear curves
	/// </summary>
	private Dictionary<float, Vector3> tearLinePositionTime;
	
	// <summary>
	/// Tear Line Timer is used as a timer to map each tear vertice with an associated tear time
	/// </summary>
	private int tearLineTimer;
	/// <summary>
	/// The island_1 list stores the vertices assocciated with the first 'island' of mesh faces
	/// </summary>
	//private List<Vector2> island1;
	private HashSet<Vector2> island1;
	/// <summary>
	/// The island_2 list stores the vertices assocciated with the second 'island' of mesh faces
	/// </summary>
	//private List<Vector2> island2;
	private HashSet<Vector2> island2;
	/// <summary>
	/// The adding to piece one flags between both new mesh object for correct new face assignment
	/// </summary>
	private bool addingToPieceOne;
	
	/// <summary>
	/// The adding to piece one flags between both new mesh object for correct new face assignment - Previous island checking
	/// </summary>
	private bool addingToPieceOnePreviously;
	
	/// <summary>
	/// This is used to flag once a cutt has started to stop detecting for edge vertice
	/// </summary>
	private bool cuttInProgress = false;
	
	/// <summary>
	/// The done calculating cutt line flag is used to determine when the player is currently cutting
	/// </summary>
	private bool doneCalculatingCuttLine = false;
	
	/// <summary>
	/// The mouse tear positions stored as the player is performing tear gesture
	/// </summary>
	private List<Vector3> mouseTearPositions;
	
	/// <summary>
	/// The need to check excess duplicates after a tear occurs.
	/// </summary>
	public bool NeedToCheckExcessDuplicates = false;
	
	/// <summary>
	/// The new piece object is used for duplication is convex shapes occur
	/// </summary>
	//private GameObject newPiece;
	
	/// <summary>
	/// The original mesh triangles are used for restating the current state of the mesh if the user
	/// does not end their cut on an edge
	/// </summary>
	int[] originalMeshTriangles;
	
	/// <summary>
	/// The original mesh verts are used for restating the current state of the mesh if the user
	/// does not end their cut on an edge
	/// </summary>
	Vector3[] originalMeshVerts;
	
	/// <summary>
	/// The start position of a tear
	/// </summary>
	private Vector3 startPos;
	
	/// <summary>
	/// The end position of a tear
	/// </summary>
	private Vector3 endPos;
	
	/// <summary>
	/// The have stored start position flags the moment we have stored the starting position to rpevent 
	/// the next iteration from re-writing the start position
	/// </summary>
	private bool haveStoredStartPos = false;
	
	/// <summary>
	/// The cutt piece one faces represents the faces of one of the cutt paper mesh 'islands'
	/// </summary>
	private int[] CuttPieceOneFaces;
	
	/// <summary>
	/// The cutt piece two faces represents the faces of one of the cutt paper mesh 'islands'
	/// </summary>
	private int[] CuttPieceTwoFaces;
	
	/// <summary>
	/// The cutt piece one verts represents the vertices of one of the cutt paper mesh 'islands'
	/// </summary>
	private Vector3[] CuttPieceOneVerts;
	
	/// <summary>
	/// The cutt piece one verts represents the vertices of one of the cutt paper mesh 'islands'
	/// </summary>
	private Vector3[] CuttPieceTwoVerts;
	
	/// <summary>
	/// The indexor1 is used to index the new island's triangles
	/// </summary>
	private int indexor1 = 0;
	
	/// <summary>
	/// The indexor2 is used to index the new island's triangles
	/// </summary>
	private int indexor2 = 0;
	
	/// <summary>
	/// The island1 indicies is used to index the new island's triangles
	/// </summary>
	private int[] island1Indicies; 
	
	/// <summary>
	/// The island2 indicies is used to index the new island's triangles
	/// </summary>
	private int[] island2Indicies;
	
	/// <summary>
	/// The force stop tear timer is used to stop a tear in progress after a certin desired amount of time
	/// off paper
	/// </summary>
	private int ForceStopTearTimer = 0;
	
	/// <summary>
	/// The have touched off paper to start tear flags the event when the player has triggered the start of a tear
	/// </summary>
	private bool haveTouchedOffPaperToStartTear = false;
	
	/// <summary>
	/// The force stop tear flag is used to stop the tear once player has left paper during a tear
	/// </summary>
	private bool forceStopTear = false;
	
	/// <summary>
	/// The gap posiitons in the mousePositions list, used for Lerping when input is too fast
	/// </summary>
	private List<int> gapPositions;
	
	/// <summary>
	/// The number mouse positions in mousePositions list
	/// </summary>
	private int numMousPos = 0;
	
	/// <summary>
	/// The mouse's previous position, this is used to keeping track
	/// and determining whether the mouse in moving rapidly
	/// </summary>
	private Vector3 mousePrevPos;
	
	/// <summary>
	/// The current cutt piece of the paper world
	/// </summary>
	private GameObject CurrentCuttPiece;
	
	/// <summary>
	/// The current cutt piece of the paper world
	/// </summary>
	private GameObject CurrentLargeCuttPiece;
	
	/// <summary>
	/// This flag is used to iterate through mesh, checking for convex curvature along edge, 
	/// iff and exists, object is split until convex edges no longer exist
	/// </summary>
	private bool haveCheckedForConvexEdges = false;
	
	#endregion
	
	#region Methods - Built In Defined
	/// <summary>
	/// Use this for initialization
	/// </summary>
	private void Start () 
	{
		//init the local reference to this component
		thisMeshFilter = GetComponent<MeshFilter>();
		
		//Initialize pointers to Scene GameObjects
		TearManager = GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>();
		PlayerObjectRef = GameObject.FindGameObjectWithTag("Player");
		EndGoalObjectRef = GameObject.FindGameObjectWithTag("EndGoal");
		
		
		
		//newPiece = newPaper;
		originalColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		
		//Set the mesh offset distance based off object's vertice topology
		SetMeshOffsetWorldSpace();
		//SetMeshOffsetScreenSpace();
		//Initially, we set the paper's screen position to be zero
		screenPoint = new Vector3(0.0f, 0.0f, 0.0f);
		
		//Initialize the newMesh to be the same length of the previous
		newMesh = new Vector3[mesh.vertices.Length];
		
		//The following is being used for tearing edge FX
		//could be better, doesn't need to go through every vert
		SetBoundsOfPaper();
		
		//init the dictionary storing the vertices along the tear line and their associated index
		//init the the mesh.vertice array
		tearLine = new Dictionary<Vector3, int>();
		tearLineTime = new Dictionary<Vector3, float>();
		tearLinePositionTime = new Dictionary<float, Vector3>();
		
		if(!PlatformPaper && !CloneObject)
		{
			//Init container into for torn edge FX
			tearEdgeFXcontainer = new GameObject("Tear_EdgeFXcontainer");
			tearEdgeFXcontainer.transform.position = new Vector3(0,0,0);
		}
		
		//Set the tearLineTimer to zero initially as the starting tear line 'time'
		tearLineTimer = 0;
		
		//The following is used to create organized grid structure
		// to easily access vertices during runtime
		SetPaperGrid();
		
		
		Clone = (GameObject) Instantiate(newPaper, this.transform.position, this.transform.rotation);
		Clone2 = (GameObject) Instantiate(newPaper, this.transform.position, this.transform.rotation);
		
		//TearManager.OriginalPlatformTopology.Add(Clone, GetComponent<MeshFilter>().mesh.triangles);
		//TearManager.OriginalPlatformTopology.Add(Clone2, GetComponent<MeshFilter>().mesh.triangles);
		
		Clone.GetComponent<TearPaper>().enabled = false;
		Clone2.GetComponent<TearPaper>().enabled = false;
		Clone.SetActive(false);
		Clone2.SetActive(false);
		
		if(TearEdgeFXPool == null)
		{
			int totalNumDecalsInPool = 500;
			TearEdgeFXPool = new GameObject[totalNumDecalsInPool];
			
			for(int itor = 0; itor < totalNumDecalsInPool; itor++)
			{
				TearEdgeFXPool[itor] = (GameObject)Instantiate(TearManager.EdgeDecalObject);
			}
		}
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	private void Update () 
	{
		//UnityEngine.Debug.LogError("updating");
		/*
		//Check to turn off player shield when player is dead
		if(PlayerObjectRef.GetComponent<TWCharacterController>().playerIsDead 
			&& TearManager.PlayerShield.GetComponent<MeshRenderer>().enabled) 
		{
			UnityEngine.Debug.Log("TESTING");
			TearManager.PlayerShield.GetComponent<MeshRenderer>().enabled = false;
		}*/
		
		//THE FOLLOWING MAKES SURE TEAR IS NOT BEING UPDATED UNLESS TOUCH
		// INPUT FOR TEAR IS FLAGGED TRUE
		if(gameStateManagerRef == null)
		{
			//UnityEngine.Debug.LogError("test 1");
			gameStateManagerRef = GameObject.FindGameObjectWithTag("MainObject").GetComponent<GameStateManager>();
		}
		if(gameStateManagerRef.OnMobileDevice() && !gameStateManagerRef.GetInputManager().CheckIfPlayerTearing())
		{
			//UnityEngine.Debug.LogError("test 2");
			return;
		}
		
		
		//Check to return if the player is trying to restart
		
		
		//Ensure flags are set ocrrectly for decal drawing
		if(cuttInProgress && !PlatformPaper)
		{
			TearManager.PlayerCurrentlyTearing = true;
		}
		else if(!cuttInProgress && !PlatformPaper)
		{
			TearManager.PlayerCurrentlyTearing = false;
		}
		
		//Checking BadTear prevents player from tearing while game displaying preveious BadTear graphics
		if(!TearManager.BadTear)
		{
			forceOnlyOneBadTearCall = true;
			
			//Ensure we stop tearing with bad input	
			if(cuttInProgress && PlayerInPaperBounds())
			{
				
				//UnityEngine.Debug.Log("testing location, 1");
				haveHitPaper = true;
			}
			
			if ((TearManager.GetInputManager().GetcurrPressState().Equals(InputManager.PressState.JUST_RELEASED) && !haveHitPaper) ||
                (TearManager.GetInputManager().GetcurrPressState().Equals(InputManager.PressState.JUST_RELEASED) && 
                PlayerInPaperBounds() && 
                (tearLine.Keys.Count() > 0 || mouseTearPositions.Count() > 0)
				))
			{
				
				if((TearManager.GetInputManager().GetcurrPressState().Equals(InputManager.PressState.JUST_RELEASED) && !haveHitPaper))
				{
					//UnityEngine.Debug.Log("Testing");
					
					totalTimeTearingTimer = 0;
					TearManager.TearFinished = false;
					ForceFinishTearLine = false;
					TearManager.hittingFoldedArea = false;
					TearManager.haveHitPaper = false;
					
					if(TearManager.centerPositions == null) TearManager.centerPositions = new Dictionary<GameObject, Vector3>();
					else TearManager.centerPositions.Clear();
					
					cuttInProgress = false;
					doneCalculatingCuttLine = false;
					
					if(mouseTearPositions == null) mouseTearPositions = new List<Vector3>();
					else mouseTearPositions.Clear();
					
					if(tearLine == null) tearLine = new Dictionary<Vector3, int>();
					else tearLine.Clear();
					
					if(gapPositions == null) gapPositions = new List<int>();
					else gapPositions.Clear();//Leave null for reseting mousePrevPos
					
					if(tearLineTime == null) tearLineTime = new Dictionary<Vector3, float>();
					else tearLineTime.Clear();
					
					if(tearLinePositionTime == null) tearLinePositionTime = new Dictionary<float, Vector3>();
					else tearLinePositionTime.Clear ();
					
					addingToPieceOne = true;
					tearLineTimer = 1;
					forceStopTear = false; 
					haveTouchedOffPaperToStartTear = false;
					TearManager.tornThroughBadObject = false;
					haveHitPaper = false;
					ForceStopTearTimer = 0;
					NeedToCheckExcessDuplicates = false;

				}
				else
				{
					
					//UnityEngine.Debug.Log("testing location, 1");
					//UnityEngine.Debug.LogError("ForceStopTear #1");
					
					//if(!TearManager.BadTear)
					ForceStopTear("precise");
					hitPlayerDuringTear = 0;
				}
				return;
			}
			
			//The following is NOT IN USE, this for MULTI TEAR
			//DynamicObjectSplitCheck();
			
			//If we are done calculating the cutt line, then a tear was just successfully calculated
			if(doneCalculatingCuttLine)
			{
				//Testing purposes, draws the tear line only
				if(DrawTearLineONLY)
				{
					//Testing purposes
					DrawTearLineOnly();
				}
				else if(!TearManager.tornThroughBadObject)
				{
					
					//UnityEngine.Debug.Log("testing location, 1");
					
					//Debug.LogError("POOOPE");
					//Traverse through old mesh to determine two new cutt pieces based off tearline
					FindNewCutPieces();	
						
					
					//Force stop tear if player tears through player or non tearable objects, or folded area
					CheckForBadTear();
					
					//Prevent the player from loop cutting - This is done before FindNewCutPieces() creates clones for cut pieces
					if(CheckForLoopTear() && !PlatformPaper)
					{
						//UnityEngine.Debug.Log("PlayerHasTornIncorrectly 1");		
						//if(!TearManager.BadTear)
							ForceStopTear("precise");
							hitPlayerDuringTear = 0;
					}
					
					/*
					//create the torn edge effects
					if(!PlatformPaper &&   
						!CloneObject && 
						TearManager.MainWorldCutPaper != null)
					{
						DrawTearFX();
					}
					*/
					
					//UnityEngine.Debug.Log("testFreq " + testFreq.ToString());			
				}
			}
			
			//TODO; Convert all input to touch input
			if(!CloneObject)
			{
//				UnityEngine.Debug.Log("testing location, 2");
				//If the left mouse button is down and the player is touching off the paper (colliding with dead space), and a cutt is no in progress, then,
				//we call mouse down an set flags for tearing accordingly
				
				if ((TearManager.GetInputManager().GetcurrPressState().Equals(InputManager.PressState.JUST_PRESSED)
					&& PlayerTouchingDeadSpace() && !cuttInProgress)
					&& TearManager.badTearVisualTimer == 0)
				{
					cuttInProgress = true;
					
					
					if(mouseTearPositions ==  null) mouseTearPositions = new List<Vector3>();
					else mouseTearPositions.Clear();
					//OnMouseTearDown();
				}
				
				//If the left mouse is dragging and the player has correctly initiated a tear, and we are not forceint the tear to stop
				
				if (
					(
					((TearManager.GetInputManager().GetcurrPressState().Equals(InputManager.PressState.DOWN) ||
                     TearManager.GetInputManager().GetcurrPressState().Equals(InputManager.PressState.JUST_PRESSED))
							&& cuttInProgress)
					&& !ForceFinishTearLine
					)
					
					&& TearManager.badTearVisualTimer == 0)
				{
					if(!PlatformPaper)
					{
						//totalTimeTearingTimer += Time.deltaTime * 1000;
						
						//if(totalTimeTearingTimer <= totalTearingTimeAloud)
						//{
							OnMousetTearDrag();
						//}
						/*
						else
						{
							OnMouseTearUp();
							
							//if(!TearManager.BadTear)
								ForceStopTear("precise");
								hitPlayerDuringTear = 0;
							
							Camera.main.audio.Pause();
						}
						*/
						
						//GC.Collect();
							
					}
					/*
					else if(PlatformPaper)
					{
						//Perform Distance check
						Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(new Vector3(TearManager.GetInputPos().x, 
																						   TearManager.GetInputPos().y, 
																						   Camera.main.WorldToScreenPoint(gameObject.transform.position).z));
						worldMousePos.z = PlatformTearCollider.transform.position.z;
						
						if(PlatformTearCollider.GetComponent<BoxCollider>().bounds.Contains(worldMousePos))
						{
							//Debug.LogError("Testing new platform mouseDown referecning");
							OnMousetTearDrag();
							
							//GC.Collect();
							
						}
					}
					*/
				}
			//	UnityEngine.Debug.Log("testing location, just released =  " +  TearManager.GetInputManager().GetcurrPressState().Equals(InputManager.PressState.JUST_RELEASED));
				
				bool checkToSmallTear = false;
				if(
					!haveHitLimitDuringTear 
					&& 
					cuttInProgress
					&&
					(Input.GetMouseButtonUp(0) || TearManager.GetInputManager().GetcurrPressState().Equals(InputManager.PressState.JUST_RELEASED))
					)
				{
					checkToSmallTear = true;
				} 
				else 
				{
					checkToSmallTear = false;	
				}
				
				//If the left mouse button is up, and the player is touching the dead space once again, and a cutt is in progress, then we call onMouseUp
				//We also make sure that the tearManager has not detected the player. tearing through a Non Tearable object (ex, player)
                if (haveHitLimitDuringTear 
					&&
					((TearManager.GetInputManager().GetcurrPressState().Equals(InputManager.PressState.JUST_RELEASED)
					&& PlayerTouchingDeadSpace() && cuttInProgress 
					&& !TearManager.tornThroughBadObject)
					|| ForceFinishTearLine))
				{
					if(TearManager.hittingFoldedArea 
						//&& !TearManager.BadTear
						) 
					{
						//UnityEngine.Debug.Log("PlayerHasTornIncorrectly 1");
						ForceStopTear("tearingFold");
						hitPlayerDuringTear = 3;
					}
					else
					{
						//UnityEngine.Debug.Log("PlayerHasTornIncorrectly 2");
						OnMouseTearUp();
						ForceFinishTearLine = false;
					}
				}
				//We see if the player's tear was invalid
                else if (
					
					(
					checkToSmallTear
					)
					
					||
					
					(
					TearManager.GetInputManager().GetcurrPressState().Equals(InputManager.PressState.JUST_RELEASED)
					&& PlayerTouchingDeadSpace() 
					&& cuttInProgress 
					&& TearManager.tornThroughBadObject)
					)
				{
					
					if(checkToSmallTear)
					{
						//UnityEngine.Debug.Log("PlayerHasTornIncorrectly 3");
						ForceStopTear("precise");
						hitPlayerDuringTear = 0;
					}
					else
					{
						//ForceStopTear();
						//UnityEngine.Debug.Log("PlayerHasTornIncorrectly 4");
						//if(!TearManager.BadTear)
						ForceStopTear("magicCarpet");
						hitPlayerDuringTear = 2;
					}
				}
				/*
				else 
					 	//&& cuttInProgress && PlayerInPaperBounds())
				{
					UnityEngine.Debug.Log("just released = " + TearManager.GetInputManager().GetcurrPressState().Equals(InputManager.PressState.JUST_RELEASED));
					if(inputManagerRef ==  null)
					{
						inputManagerRef = gameStateManagerRef.GetInputManager();
					}
					
					if(TearManager.GetInputManager().GetcurrPressState().Equals(InputManager.PressState.JUST_RELEASED) && !PlayerTouchingDeadSpace())
					{
						UnityEngine.Debug.Log("precise testing");
						ForceStopTear("precise");
					}
				}*/
				
				
				
				//UnityEngine.Debug.Log("testing Just released = " + TearManager.GetInputManager().GetcurrPressState().Equals(InputManager.PressState.JUST_RELEASED).ToString());
			}
			
			if(CloneObject && NeedToCheckExcessDuplicates)
			{
				//TODO, determine why the following line is needed, by this point, both clones should have values
				if(Clone == null || Clone2 == null)
				{
					//Debug.LogError("Creating new paper world clones");
					NeedToCheckExcessDuplicates = false;
					return;
					//CreateNewPaperWorld();
				}
				
				
				
				int checkCloneNum = 1;
				bool checkNotNeededObject = true;
				Vector3[] cloneMeshVerts = Clone.GetComponent<MeshFilter>().mesh.vertices;
				Vector3 testVertPos = cloneMeshVerts[0];//Clone.GetComponent<MeshFilter>().mesh.vertices.ElementAt(0);
				for(int itor = 1; itor < cloneMeshVerts.Length/*Clone.GetComponent<MeshFilter>().mesh.vertices.Count()*/; ++itor)
				{
					//Debug.LogError("vertice = " + Clone.GetComponent<MeshFilter>().mesh.vertices.ElementAt(itor).ToString());
					if(cloneMeshVerts[itor]/*Clone.GetComponent<MeshFilter>().mesh.vertices.ElementAt(itor)*/ != testVertPos)
					{
						checkNotNeededObject = false;
						break;
					}
				}
				
				if(!checkNotNeededObject)
				{
					checkCloneNum = 2;
					Vector3[] clone2MeshVerts = Clone2.GetComponent<MeshFilter>().mesh.vertices;
					testVertPos = clone2MeshVerts[0];//Clone2.GetComponent<MeshFilter>().mesh.vertices.ElementAt(0);
					for(int itor = 1; itor < clone2MeshVerts.Length/*Clone2.GetComponent<MeshFilter>().mesh.vertices.Count()*/; ++itor)
					{
						//Debug.LogError("vertice = " + Clone2.GetComponent<MeshFilter>().mesh.vertices.ElementAt(itor).ToString());
						if(clone2MeshVerts[itor]/*Clone2.GetComponent<MeshFilter>().mesh.vertices.ElementAt(itor)*/ != testVertPos)
						{
							checkNotNeededObject = false;
							break;
						}
					}
				}
				
				if(checkNotNeededObject)
				{
					if(checkCloneNum == 1)
					{
						Debug.LogError("********TEST2.1******** Clone length = " + Clone.GetComponent<MeshFilter>().mesh.triangles.Length.ToString());
						GameObject.Destroy(Clone);
						TearManager.TornPlatforms.Remove(Clone);
					}
					else if(checkCloneNum == 2)
					{
						Debug.LogError("********TEST2.2******** Clone2length = " + Clone2.GetComponent<MeshFilter>().mesh.triangles.Length.ToString());
						GameObject.Destroy(Clone2);
						TearManager.TornPlatforms.Remove(Clone2);
					}
				}
				else
				{
					if((Clone.GetComponent<TearPaper>().tearLine.Count() == 0 || Clone.GetComponent<TearPaper>().tearLine.Count() == 1
						|| Clone.GetComponent<TearPaper>().tearLine.Count() == 2) && Clone.GetComponent<MeshFilter>().mesh.triangles.Length == 0)
					{
						Debug.LogError("********TEST_OIOIUUOIOUI___1******** Clone length = " + Clone.GetComponent<MeshFilter>().mesh.triangles.Length.ToString());
						GameObject.Destroy(Clone);
						TearManager.TornPlatforms.Remove(Clone);
					}
					
					if((Clone2.GetComponent<TearPaper>().tearLine.Count() == 0 || Clone2.GetComponent<TearPaper>().tearLine.Count() == 1
						|| Clone2.GetComponent<TearPaper>().tearLine.Count() == 2) && Clone2.GetComponent<MeshFilter>().mesh.triangles.Length == 0)
					{
						Debug.LogError("********TEST_OIOIUUOIOUI___2******** Clone2 length = " + Clone2.GetComponent<MeshFilter>().mesh.triangles.Length.ToString());
						GameObject.Destroy(Clone2);
						TearManager.TornPlatforms.Remove(Clone2);
					}
					
					if((tearLine.Count() == 0 || tearLine.Count() == 1 || tearLine.Count() == 2) && thisMeshFilter.mesh.triangles.Length == 0)
					{
						Debug.LogError("********TEST_OIOIUUOIOUI___3******** gameObject" + thisMeshFilter.mesh.triangles.Length.ToString());
						GameObject.Destroy(this);
						TearManager.TornPlatforms.Remove(gameObject);
					}
					
					if(!checkNotNeededObject)
					{
						//Debug.LogError("TEST3");
					}
				}
				
				NeedToCheckExcessDuplicates = false;
			}
		}
		
		else if(forceOnlyOneBadTearCall)
		{
			forceOnlyOneBadTearCall = false;
			
				//Debug.LogError("ForceStopTear #2");
			if(hitPlayerDuringTear == 3)
			{
				ForceStopTear("tearingFold");
			}
			else if(hitPlayerDuringTear == 2)
			{
				ForceStopTear("magicCarpet");
			}
			else if(hitPlayerDuringTear == 1)
			{
				ForceStopTear("door");
			}
			else if(hitPlayerDuringTear == 0)
			{
				ForceStopTear("precise");
			}
			else if(hitPlayerDuringTear == -1)
			{
				ForceStopTear("ignore");
			}
			
			
			hitPlayerDuringTear = 0;
		}
	}
	
	/// <summary>
	/// The hit player during tear.
	/// </summary>
	private int hitPlayerDuringTear = 0;
	
	/// <summary>
	/// Gets the bad tear value.
	/// </summary>
	public int GetBadTearVal()
	{
		return hitPlayerDuringTear;
	}
	
	/// <summary>
	/// Sets the bad tear value.
	/// </summary>
	public void SetBadTearVal(int val)
	{
		if(val == 0
			|| val == 1
			|| val == 2
			|| val == 3)
		{
			hitPlayerDuringTear = val;
		}
	}
	
	/// <summary>
	/// The force only one bad tear call.
	/// </summary>
	private bool forceOnlyOneBadTearCall = false;
	
	/// <summary>
	/// The have hit paper.
	/// </summary>
	private bool haveHitPaper = false;
	
	#region Mouse Input Methods - (OLD REGION, TODO-> RENAME PROPERLY AND ORGANIZE (J.C.)
	
	/// <summary>
	/// Checks for missing user input, ensure no gaps when player
	/// swipes finger fast so tear logic can map to vertices correctly
	/// </summary>
	private void CheckForMissingUserInput()
	{
		/*
		int insertPosition = 1;
		
		//Here, we loop through the mous positions to check for gap
		for(int itor = 0; itor + 1 < mouseTearPositions.Count(); itor += insertPosition)
		{
			//Find locations of current and next mouseTearPosition for testing distance
			Vector2 testPos1 = new Vector2(mouseTearPositions.ElementAt(itor).x, mouseTearPositions.ElementAt(itor).y);
			Vector2 testPos2 = new Vector2(mouseTearPositions.ElementAt(itor + 1).x, mouseTearPositions.ElementAt(itor + 1).y);
			
			//now get distance between positions
			float distCheck = Vector2.Distance(testPos1, testPos2);
			
			//make sure the distCheck is positive
			if(distCheck < 0) distCheck *= -1;
			
			//Now we check if gap present
			if(distCheck > (MESH_VERT_OFFSET))
			{
				UnityEngine.Debug.Log("Currently CheckForMissingUserInput ---- GAP");
				
				//now we fill in missing position
				insertPosition = FillInMissingTearInput(itor + 1, mouseTearPositions.ElementAt(itor), mouseTearPositions.ElementAt(itor + 1), distCheck);
				--insertPosition;
				
			}
			else
			{
				insertPosition = 1;
			}
		}
		*/
		
		
		
		bool gapsFoundInMousePositions = true;
		
		//The following ensures we continue to fill in gaps until none found
		while(gapsFoundInMousePositions)
		{
			//UnityEngine.Debug.Log("Currently checking for UI gaps");
			
			//Here, we loop through the mous positions to check for gap
			for(int itor = 0; itor + 1 < mouseTearPositions.Count(); itor++)
			{
				//Find locations of current and next mouseTearPosition for testing distance
				Vector3 testPos0 = Camera.main.ScreenToWorldPoint(mouseTearPositions.ElementAt(itor));
				Vector3 testPos09 = Camera.main.ScreenToWorldPoint(mouseTearPositions.ElementAt(itor + 1));
				
				Vector2 testPos1 = mouseTearPositions.ElementAt(itor);//new Vector2(testPos0.x, testPos0.y);
				Vector2 testPos2 = mouseTearPositions.ElementAt(itor + 1);//new Vector2(testPos09.x, testPos09.y);
				
				//now get distance between positions
				float distCheck = Vector2.Distance(testPos1, testPos2);
				
				//make sure the distCheck is positive
				if(distCheck < 0) distCheck *= -1;
				
				//Now we check if gap present
				if(distCheck > (MESH_VERT_OFFSET))
				{
					//Here we make sure the while loop is hit again for potentially another gap
					//further into the mouseTearPositions
					gapsFoundInMousePositions = true;
					
					//now we fill in missing position
					FillInMissingTearInput(itor + 1, mouseTearPositions.ElementAt(itor), mouseTearPositions.ElementAt(itor + 1), distCheck);
					
					//Break so we can start again
					break;
				}
				else
				{
					//When this is triggered, no gaps found, we are done with filling in 
					//missing positions with fst user input
					gapsFoundInMousePositions = false;
				}
			}
		}
		
		
	}
	
	/// <summary>
	/// Fills the in missing tear input and adds the
	/// mouseTearPosition at the desired insert position
	/// </summary>
	private void FillInMissingTearInput(int insertPos, Vector3 startingPos, Vector3 endingPos, float distance)
	{
		float numbObjects = (float)(distance/(MESH_VERT_OFFSET));
		float hardCodedLerp = 0.2f;
		
		//LERP from starting to ending adding missing positions within mouseTearPositions
		for(float lerpStatus = hardCodedLerp; lerpStatus < 1; lerpStatus += hardCodedLerp)
		{
			Vector3 newPos = Vector3.Lerp(startingPos, endingPos, lerpStatus);
			mouseTearPositions.Insert(insertPos, newPos);
			++insertPos;
		}
	}
	
	/// <summary>
	/// Raises the mouse down event.
	/// </summary>
	private void OnMouseTearDown()
	{
		//Debug.Log ("ENTERING ON MOUSE DOWN");
		//Init new list for storage
		
		if(mouseTearPositions == null) mouseTearPositions = new List<Vector3>();
		else mouseTearPositions.Clear();
		
		//Testing to know where we are in logic
		//Debug.Log("Enter MouseDown");
	}
	
	/// <summary>
	/// The have hit limit during tear.
	/// </summary>
	private bool haveHitLimitDuringTear = false;
	
	/// <summary>
	/// Raises the mouse drag event.
	/// </summary>
	void OnMousetTearDrag()
	{
		//Initialize list storing gap indexes
		if(gapPositions == null)
		{
			gapPositions = new List<int>();
            mousePrevPos = TearManager.GetInputPos();
		}
		
		Vector3 newPos = Vector3.zero;
		
		//We check to see if the player is touching an edge
		if(cuttInProgress)
		{
            try
            {
                float depth = Camera.main.WorldToScreenPoint(this.transform.TransformPoint(mesh.vertices.ElementAt(0))).z;
				
				newPos = new Vector3(TearManager.GetInputPos().x,
                                                    TearManager.GetInputPos().y,
                                                    depth);
				
                mouseTearPositions.Add(newPos);
            }

            catch
            { 
            
            }
			
			Vector3 testPos = Camera.main.ScreenToWorldPoint(newPos);
			testPos.z = TearManager.TearLimitBounds.collider.transform.position.z;
			if(TearManager.TearLimitBounds.collider.bounds.Contains(testPos))
			{
				haveHitLimitDuringTear = true;
			}
			
			/*
			
			//Debug.Log("DRAG");
			//Get the distance from previous mouseposition to current mouseposition
			
			float dist = Vector3.Distance(Camera.main.ScreenToWorldPoint(new Vector3(mousePrevPos.x, mousePrevPos.y, 
				Camera.main.WorldToScreenPoint(this.transform.TransformPoint(mesh.vertices.ElementAt(0))).z)), 
				Camera.main.ScreenToWorldPoint(new Vector3(TearManager.GetInputPos().x, 
                                                           TearManager.GetInputPos().y,
				Camera.main.WorldToScreenPoint(this.transform.TransformPoint(mesh.vertices.ElementAt(0))).z)));
			
			
			//float dist = Vector2.Distance(new Vector2(mousePrevPos.x, mousePrevPos.y), 
			//	new Vector2(Camera.main.WorldToScreenPoint(Clone.transform.TransformPoint(Clone.GetComponent<MeshFilter>().mesh.vertices.ElementAt(0))).x,
			//	Camera.main.WorldToScreenPoint(Clone.transform.TransformPoint(Clone.GetComponent<MeshFilter>().mesh.vertices.ElementAt(0))).y));
			
			
			//Just in case the distance is negative
			if(dist < 0)
			{
				dist *= -1;
			}
			
			//If the position is greater than the meshvertoffset, then we know they are moving extremely fast,
			//therefore, we have to fill in the missing positions
			if(dist > (MESH_VERT_OFFSET/2))
			{
				//This is called when the player is moving too fast, LERP is used to fill in gaps
				//Debug.LogError("AddMissingTearMousePositions");
				
				AddMissingTearMousePositions(
					Camera.main.ScreenToWorldPoint(new Vector3(mousePrevPos.x, mousePrevPos.y, 
						Camera.main.WorldToScreenPoint(this.transform.TransformPoint(mesh.vertices.ElementAt(0))).z)), 
					Camera.main.ScreenToWorldPoint(new Vector3(TearManager.GetInputPos().x, 
                                                               TearManager.GetInputPos().y, 
					
						Camera.main.WorldToScreenPoint(this.transform.TransformPoint(mesh.vertices.ElementAt(0))).z)), 
					dist);
				
				
				//AddMissingTearMousePositionsScreen(mousePrevPos, Input.mousePosition, dist);
				
			}
			
			float depth = Camera.main.WorldToScreenPoint(this.transform.TransformPoint(mesh.vertices.ElementAt(0))).z;
			//float depth = 0;
			
			//When mouse is draging, add input to list
			mouseTearPositions.Add(new Vector3(TearManager.GetInputPos().x, 
                                               TearManager.GetInputPos().y, depth));
			
			
			//mouseTearPositions.Add(Input.mousePosition);
			
			//Keep track of the mouse's previous position
            mousePrevPos = TearManager.GetInputPos();
			 */
			
		}
		else
		{
			//This statment should be hit is the UI starts a tear but does not end on an edge properly
			//Debug.LogError("Salty Ballz");
		}
	}
	
	/// <summary>
	/// Players the in paper bounds.
	/// </summary>
	/// <returns>
	/// Returns true is the player is within the paperWorld Object bounds
	/// </returns>
	private bool PlayerInPaperBounds()
	{
        if (Camera.main)
        {
            Vector3 testPlayerPos = new Vector3(Camera.main.ScreenToWorldPoint(new Vector2(TearManager.GetInputPos().x,
                                                                                           TearManager.GetInputPos().y)).x,
                Camera.main.ScreenToWorldPoint(new Vector2(TearManager.GetInputPos().x,
                                                           TearManager.GetInputPos().y)).y, transform.position.z);

            if (GetComponent<MeshCollider>().bounds.Contains(testPlayerPos))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
	}
	
	/// <summary>
	/// The total tearing time aloud.
	/// </summary>
	private float totalTearingTimeAloud = 3500;
	
	/// <summary>
	/// The total time tearing timer.
	/// </summary>
	private float totalTimeTearingTimer = 0;
	

	
	/// <summary>
	/// Raises the mouse up event.
	/// </summary>
	private void OnMouseTearUp()
	{	
		if(CloneObject) return;

		
		//Now, we check to see if the player is touching an edge to complete their cutt/tear
		if(cuttInProgress && mouseTearPositions.Count() > 0)// && Clone != null && Clone2 != null)
		{
			//UnityEngine.Debug.Log("OnMouseTearUp");
			//Create a clone world mesh for deformation, we do not change the original world gameobject mesh
			if (!CloneObject) CreateNewPaperWorld();
			
			//Debug.LogError("GOOD CUTT");
			
			//This is used to flag loops or back tracking within tear line
			bool badInputDetected = false;
			
			//Transform mouseInput into torn vertice list (tearLine)
			badInputDetected = TurnInputIntoTornVertCurve();
			
			
			//At this point we clean the storage for mousePositions
			mouseTearPositions.Clear();
			
			if(badInputDetected)
			{
				
				Debug.LogError("ForceStopTear #3");
				
				//Debug.LogError("BAD CUTT");
				//if(!TearManager.BadTear)
					ForceStopTear("precise");
				hitPlayerDuringTear = 0;
				return;
			}
			
			
			 
			 
			 
			bool allTornVerticesEdgeVertices = false;
			List<Vector3> tearLineKeys = tearLine.Keys.ToList();
			for(int jtor = 0; jtor < tearLineKeys.Count(); jtor++)
			{
				if(!edgeVerts.Contains(tearLineKeys[jtor]))
				{
					allTornVerticesEdgeVertices = false;
					break;
				}
				else allTornVerticesEdgeVertices = true;
			}
			if(allTornVerticesEdgeVertices)
			{
				
				//Debug.LogError("ForceStopTear #4");
				
				//Debug.LogError("BAD TEAR LINE -- allTornVerticesEdgeVertices TRUE");
				
				//if(!TearManager.BadTear)
					ForceStopTear("precise");
				hitPlayerDuringTear = 0;
				return;
			}
			
			
			
			
			
			//the following deletes excess edge torn vertices
			//DeleteExcessEdgeVertices();
			
			//The following is used to remove excess edge vertices
			RemoveExcessEdgeVertices();
			
			/*
			if(tearLine.Keys.Count() != 0 && PlatformPaper)
			{
				//Debug.LogError("PLATFORM PAPER --- ForceHorizontalCuttAlongEdge && ForceHorizontalCuttFirstRowVert HIT");
				//RemoveProblematicSubRegionsOfTear();
				ForceHorizontalCuttAlongEdge();
			
				//Now, we make sure every row that should be torn is torn
				ForceHorizontalCuttFirstRowVert();
			}
			*/
			
			
			//Now, we connect the starting and ending poisitons of the tearLine
			//along the edge of the object
			//ForceEdgeVertsToBeTorn();
			
			//Save old mesh information
			//Clone.GetComponent<MeshFilter>().mesh.vertices = originalMeshVerts;
			//Clone.GetComponent<MeshFilter>().mesh.triangles = originalMeshTriangles;	
			
			//Flag we are done finding cutt/tear line and there was not a loop
			doneCalculatingCuttLine = true;
			
		}
		else
		{
			//This is called when we force the player from tearing due to bad input
			//ForceStopTear();
		}
		
		//The cutt has now stopped
		if(cuttInProgress) cuttInProgress = false;
		
		//Reset forceStopTear for next tear check
		if(forceStopTear) forceStopTear = false;
	}
	
	#endregion
	
	
	#endregion
	
	#region Defined Methods
	
	
	/// <summary>
	/// Removes the excess edge vertices.
	/// </summary>
	private void RemoveExcessEdgeVertices()
	{
	//	List<Vector> tearLineKeys = tearLine.Keys.ToList
		while(tearLine.Keys.Count() > 3 && 
			(tearLine.Keys.ElementAt(1).x >= MaxWorldWidth ||
			tearLine.Keys.ElementAt(1).y >= MaxWorldHeight ||
			tearLine.Keys.ElementAt(1).x <= MinWorldWidth ||
			tearLine.Keys.ElementAt(1).y <= MinWorldHeight)
			)
		{
			tearLine.Remove(tearLine.Keys.ElementAt(0));
		}
		
		while(tearLine.Keys.Count() > 3 && 
			(tearLine.Keys.ElementAt(tearLine.Keys.Count() - 2).x >= MaxWorldWidth ||
			tearLine.Keys.ElementAt(tearLine.Keys.Count() - 2).y >= MaxWorldHeight ||
			tearLine.Keys.ElementAt(tearLine.Keys.Count() - 2).x <= MinWorldWidth ||
			tearLine.Keys.ElementAt(tearLine.Keys.Count() - 2).y <= MinWorldHeight)
			)
		{
			tearLine.Remove(tearLine.Keys.ElementAt(tearLine.Keys.Count() - 1));
		}
	}
	
	/// <summary>
	/// Controls drawing the tear FX
	/// </summary>
	private void DrawTearFX()
	{
		//The following will get hit on a bad tear
		if(TearManager.MainWorldCutPaper == null) return;
		
		DetermineEdgeFXplacement();
		//DetermineEdgeFXplacement2();
		
		//LerpFromEdgeFxToNeighbor();
		//LerpFromEdgeFxToNeighbor2();
		//OrganizeEdgeFX();
		
		//The following forces the depth of the edge FX and also
		//destroys and effest along non torn edge (original paper world edge)
		PositionDepthFX();
		
		//The following is used to duplicate FX and parent to cutt piece
		CreateCuttFXDuplicate();
	}
	
	/// <summary>
	/// Lerps from edge FX to neighbor.
	/// </summary>
	private void LerpFromEdgeFxToNeighbor()
	{
		
		List<Vector3> newAllEdgeFX = new List<Vector3>();
		
		
		
		for(int i = 0; i < allEdgeFX.Count(); i++)
		{
			if(allEdgeFX[i].x >= MaxWorldWidth ||
				allEdgeFX[i].y >= MaxWorldHeight ||
				allEdgeFX[i].x <= MinWorldWidth ||
				allEdgeFX[i].y <= MinWorldHeight)
			{
				newAllEdgeFX.Add(allEdgeFX[i]);
				//UnityEngine.Debug.Log("Edge FX found");
				break;
			}
		}
		
		for (int i = 0; i < newAllEdgeFX.Count(); i++)
	    {
			float closestDistCheck = 1000;
			Vector3 closestPos = new Vector3(-1000, -1000, -1000);
			
	        for (int j = 0; j < allEdgeFX.Count(); j++)
		    {
		        if(!newAllEdgeFX.Contains(allEdgeFX[j]))
				{
					float distCheck = Vector3.Distance(newAllEdgeFX[i], allEdgeFX[j]);
					if(distCheck < 0) distCheck *= -1;
					
					
					
					if(distCheck < closestDistCheck)
					{
						
						closestDistCheck = distCheck;
						closestPos = allEdgeFX[j];
					}
				}
		    }
			
			if(closestDistCheck <= (MESH_VERT_OFFSET * 1.5f) 
				&& !newAllEdgeFX.Contains(closestPos)
				&& closestPos != new Vector3(-1000, -1000, -1000))
			{
				//UnityEngine.Debug.Log("GOOD distCheck = " + closestDistCheck.ToString());
				newAllEdgeFX.Add(closestPos);
			}
			
			
	    }
		allEdgeFX.Clear();
		allEdgeFX = newAllEdgeFX;
		//newAllEdgeFX.Clear();
		
		
		List<Vector3> noDuplicates = new List<Vector3>();
		
		for(int itor = 0; itor < allEdgeFX.Count(); itor++)
		{
			if(!noDuplicates.Contains(allEdgeFX[itor]))
			{
				noDuplicates.Add(allEdgeFX[itor]);
			}
		}
		allEdgeFX.Clear();
		allEdgeFX = noDuplicates;
		//noDuplicates.Clear();
		
		
		
		//allEdgeFX.
		
		for(int itor = 0; itor + 1 < allEdgeFX.Count(); itor++)
		{
			LerpEdgeFX2(allEdgeFX.ElementAt(itor), allEdgeFX.ElementAt(itor + 1));
			
		}
		
		//newAllEdgeFX.Clear();
	}
	
	/// <summary>
	/// Lerps from edge fx to neighbor2.
	/// </summary>
	private void LerpFromEdgeFxToNeighbor2()
	{
		
		List<Vector3> newAllEdgeFX = new List<Vector3>();
		
		
		
		for(int i = 0; i < allEdgeFX.Count(); i++)
		{
			if(allEdgeFX[i].x >= MaxWorldWidth ||
				allEdgeFX[i].y >= MaxWorldHeight ||
				allEdgeFX[i].x <= MinWorldWidth ||
				allEdgeFX[i].y <= MinWorldHeight)
			{
				newAllEdgeFX.Add(allEdgeFX[i]);
				//UnityEngine.Debug.Log("Edge FX found");
				break;
			}
		}
		
		for (int i = 0; i < newAllEdgeFX.Count(); i++)
	    {
			float closestDistCheck = 1000;
			Vector3 closestPos = new Vector3(-1000, -1000, -1000);
			
	        for (int j = 0; j < allEdgeFX.Count(); j++)
		    {
		        if(!newAllEdgeFX.Contains(allEdgeFX[j]))
				{
					float distCheck = Vector3.Distance(newAllEdgeFX[i], allEdgeFX[j]);
					if(distCheck < 0) distCheck *= -1;
					
					
					
					if(distCheck < closestDistCheck)
					{
						
						closestDistCheck = distCheck;
						closestPos = allEdgeFX[j];
					}
				}
		    }
			
			if(closestDistCheck <= (MESH_VERT_OFFSET * 1.5f) 
				&& !newAllEdgeFX.Contains(closestPos)
				&& closestPos != new Vector3(-1000, -1000, -1000))
			{
				//UnityEngine.Debug.Log("GOOD distCheck = " + closestDistCheck.ToString());
				newAllEdgeFX.Add(closestPos);
			}
			
			
	    }
		allEdgeFX.Clear();
		allEdgeFX = newAllEdgeFX;
		//newAllEdgeFX.Clear();
		
		
		List<Vector3> noDuplicates = new List<Vector3>();
		
		for(int itor = 0; itor < allEdgeFX.Count(); itor++)
		{
			if(!noDuplicates.Contains(allEdgeFX[itor]))
			{
				noDuplicates.Add(allEdgeFX[itor]);
			}
		}
		allEdgeFX.Clear();
		allEdgeFX = noDuplicates;
		//noDuplicates.Clear();
		
		
		
		//allEdgeFX.
		
		for(int itor = 0; itor + 1 < allEdgeFX.Count(); itor++)
		{
			LerpEdgeFX2(allEdgeFX.ElementAt(itor), allEdgeFX.ElementAt(itor + 1));
			
		}
		
		//newAllEdgeFX.Clear();
	}
	
	/// <summary>
	/// Organizes the edge FX list for next step LERPing
	/// </summary>
	private void OrganizeEdgeFX()
	{
		//The following will get hit on a bad tear
		if(TearManager.MainWorldCutPaper == null) return;
		
		for(int itor = 0; 
			itor < tearManagerMainCutPieceMeshFilter.mesh.triangles.Count(); 
			itor += 3)
		{
			int numbAssocateVerts = 0;
			
			List<Vector3> lerpPositions = new List<Vector3>();
			List<int> lerpIndecies = new List<int>();
			
			for(int jtor = 0; jtor < itorsIntoMeshEdgeFX.Count(); jtor++)
			{
				
				if(itorsIntoMeshEdgeFX.ElementAt(jtor) == 
					tearManagerMainCutPieceMeshFilter.mesh.triangles[itor])
				{
					Vector3 newPos = TearManager.MainWorldCutPaper.transform.TransformPoint(
							tearManagerMainCutPieceMeshFilter.mesh.vertices
							[tearManagerMainCutPieceMeshFilter.mesh.triangles[itor]]);
					
					++numbAssocateVerts;
					lerpPositions.Add (newPos);
					lerpIndecies.Add(itorsIntoMeshEdgeFX.ElementAt(jtor));
				}
				if(itorsIntoMeshEdgeFX.ElementAt(jtor) == 
					tearManagerMainCutPieceMeshFilter.mesh.triangles[itor + 1])
				{
					Vector3 newPos = TearManager.MainWorldCutPaper.transform.TransformPoint(
							tearManagerMainCutPieceMeshFilter.mesh.vertices
							[tearManagerMainCutPieceMeshFilter.mesh.triangles[itor + 1]]);
					
					
					++numbAssocateVerts;
					lerpPositions.Add (newPos);
					lerpIndecies.Add(itorsIntoMeshEdgeFX.ElementAt(jtor));
				}
				if(itorsIntoMeshEdgeFX.ElementAt(jtor) == 
					tearManagerMainCutPieceMeshFilter.mesh.triangles[itor + 2])
				{
					Vector3 newPos = TearManager.MainWorldCutPaper.transform.TransformPoint(
							tearManagerMainCutPieceMeshFilter.mesh.vertices
							[tearManagerMainCutPieceMeshFilter.mesh.triangles[itor + 2]]);
					
					
					++numbAssocateVerts;
					lerpPositions.Add (newPos);
					lerpIndecies.Add(itorsIntoMeshEdgeFX.ElementAt(jtor));
				}
				
				if(numbAssocateVerts >= 3) break;
			}
			
			//Check to break;
			if(numbAssocateVerts == 2)
			{
				if(CheckFrequencyOfEdge(lerpIndecies.ElementAt(0), lerpIndecies.ElementAt(1)) <= 1)
				{
					if(!LerpFromToCheck.ContainsKey(lerpPositions.ElementAt(0)))
					{
						LerpFromToCheck.Add(lerpPositions.ElementAt(0), 1);
					}
					else
					{
						++LerpFromToCheck[lerpPositions.ElementAt(0)];
					}
					
					if(!LerpFromToCheck.ContainsKey(lerpPositions.ElementAt(1)))
					{
						LerpFromToCheck.Add(lerpPositions.ElementAt(1), 1);
					}
					else
					{
						++LerpFromToCheck[lerpPositions.ElementAt(1)];
					}
					
					LerpEdgeFX(lerpPositions.ElementAt(0), lerpPositions.ElementAt(1));
				}
			}
			else if(numbAssocateVerts >= 3)
			{
				if(!LerpFromToCheck.ContainsKey(lerpPositions.ElementAt(0)))
				{
					LerpFromToCheck.Add(lerpPositions.ElementAt(0), 1);
				}
				else
				{
					++LerpFromToCheck[lerpPositions.ElementAt(0)];
				}
				
				if(!LerpFromToCheck.ContainsKey(lerpPositions.ElementAt(1)))
				{
					LerpFromToCheck.Add(lerpPositions.ElementAt(1), 1);
				}
				else
				{
					++LerpFromToCheck[lerpPositions.ElementAt(1)];
				}
				
				if(!LerpFromToCheck.ContainsKey(lerpPositions.ElementAt(2)))
				{
					LerpFromToCheck.Add(lerpPositions.ElementAt(2), 1);
				}
				else
				{
					++LerpFromToCheck[lerpPositions.ElementAt(2)];
				}
				
				if(CheckFrequencyOfEdge(lerpIndecies.ElementAt(0), lerpIndecies.ElementAt(1)) <= 1)
				{
					LerpEdgeFX(lerpPositions.ElementAt(0), lerpPositions.ElementAt(1));
				}
				if(CheckFrequencyOfEdge(lerpIndecies.ElementAt(1), lerpIndecies.ElementAt(2)) <= 1)
				{
					LerpEdgeFX(lerpPositions.ElementAt(1), lerpPositions.ElementAt(2));
				}
				if(CheckFrequencyOfEdge(lerpIndecies.ElementAt(2), lerpIndecies.ElementAt(0)) <= 1)
				{
					LerpEdgeFX(lerpPositions.ElementAt(2), lerpPositions.ElementAt(0));
				}
			}
			
			//lerpPositions.Clear();
			//lerpIndecies.Clear();
			
		}
		//clear list memory
		//lerpPositions.Clear();
		//lerpIndecies.Clear();
		LerpFromToCheck.Clear();
		LerpToFromCheck.Clear();
		itorFXcheck.Clear();
		itorsIntoMeshEdgeFX.Clear();
	}
	/// <summary>
	/// The itors into mesh storage in MeshFilter for edge FX
	/// </summary>
	private Dictionary<Vector3, int> LerpFromToCheck = new Dictionary<Vector3, int>();
	
	/// <summary>
	/// The itors into mesh storage in MeshFilter for edge FX
	/// </summary>
	private Dictionary<int, Vector3> LerpToFromCheck = new Dictionary<int, Vector3>();
	
	/// <summary>
	/// The itors into mesh storage in MeshFilter for edge FX
	/// </summary>
	private Dictionary<Vector3, bool> itorFXcheck = new Dictionary<Vector3, bool>();
	
	/// <summary>
	/// The itors into mesh storage in MeshFilter for edge FX
	/// </summary>
	private List<int> itorsIntoMeshEdgeFX = new List<int>();
	
	/// <summary>
	/// Determines the edge FX placement.
	/// </summary>
	private void DetermineEdgeFXplacement()
	{
		for(int itor = 0; 
			itor < TearManager.MainStartingWorldPaper.GetComponent<MeshFilter>().mesh.vertices.Count() ; 
			itor++)
		{
			if(TearManager.MainWorldCutPaper != null &&
				TearManager.MainWorldPaper != null)
			{
				
				if(tearManagerMainCutPieceMeshFilter != GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().MainWorldCutPaper.GetComponent<MeshFilter>())
				{
					tearManagerMainCutPieceMeshFilter = GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().MainWorldCutPaper.GetComponent<MeshFilter>();
				}
				
				if(tearManagerMainCutPieceMeshFilter.mesh.triangles.Contains(itor)
					&&
					TearManager.MainWorldPaper.GetComponent<MeshFilter>().mesh.triangles.Contains(itor))
				{
					
					Vector3 newPos = TearManager.MainWorldCutPaper.transform.TransformPoint(
						tearManagerMainCutPieceMeshFilter.mesh.vertices[itor]);
					
					//newPos.z -= 0.1f;
					CreateNewTornEdgeFX2(newPos, false);
					
					//Add itor for lerping to complete edge FX
					itorsIntoMeshEdgeFX.Add(itor);
					
					LerpToFromCheck.Add(itor, newPos);
				}
			}
			else
			{
				//MainWorldCutPaper and MainWorldPaper NOT SET YET...
				break;
			}
		}
	}
	
	/// <summary>
	/// Determines the edge FX placement.
	/// </summary>
	private void DetermineEdgeFXplacement2()
	{
		for(int itor = 0; itor < tearLine.Keys.Count(); itor++)
		{
			if(TearManager.MainWorldCutPaper != null &&
				TearManager.MainWorldPaper != null)
			{
			
				Vector3 newPos = tearLine.Keys.ElementAt(itor);
				
				//newPos.z -= 0.1f;
				CreateNewTornEdgeFX2(newPos, false);
				
				//Add itor for lerping to complete edge FX
				itorsIntoMeshEdgeFX.Add(itor);
				
				LerpToFromCheck.Add(itor, newPos);
				
			}
			else
			{
				//MainWorldCutPaper and MainWorldPaper NOT SET YET...
				break;
			}
		}
	}
	
	/// <summary>
	/// Finsh the edge effect by Lerping
	/// </summary>
	private void LerpEdgeFX(Vector3 pos1, Vector3 pos2)
	{
		LerpEdgeFX2(pos1, pos2);
		return;
		
		float numbObjects = 0.35f;
		
		//iterate through the existing tear edge FX storage
		//to find lerp positions
		for(int itor = 0; itor + 1 < tearEdgeFXlist.Count(); itor++)
		{
			for(float lerpStatus = numbObjects; lerpStatus < 1; lerpStatus += numbObjects)
			{
				Vector3 newPos = Vector3.Lerp(pos1, pos2, lerpStatus);
				if(!allEdgeFX.Contains(newPos))
				{
					CreateNewTornEdgeFX2(newPos, true);
				}
			}
		}
	}
	
	/// <summary>
	/// Finsh the edge effect by Lerping
	/// </summary>
	private void LerpEdgeFX2(Vector3 pos1, Vector3 pos2)
	{
		float numbObjects = 0.35f;
		
		//iterate through the existing tear edge FX storage
		//to find lerp positions
		for(float lerpStatus = numbObjects; lerpStatus < 1; lerpStatus += numbObjects)
		{
			Vector3 newPos = Vector3.Lerp(pos1, pos2, lerpStatus);
			if(!allEdgeFX.Contains(newPos))
			{
				CreateNewTornEdgeFX3(newPos);
			}
		}
		
	}
	
	/// <summary>
	/// Creates the cutt FX duplicates
	/// </summary>
	private void CreateCuttFXDuplicate()
	{
		//The folowing will get hit on a bad loop tear
		if(TearManager.MainWorldCutPaper == null ) return;
		
		GameObject newEdgeContainer = (GameObject)Instantiate(tearEdgeFXcontainer, 
																tearEdgeFXcontainer.transform.position, 
																tearEdgeFXcontainer.transform.rotation); 
		
		
						
		for(int ztor = 0; ztor < newEdgeContainer.transform.GetChildCount(); ztor++)
		{
			if(newEdgeContainer.transform.GetChild(ztor).renderer.material.color != Color.grey)
			{
				newEdgeContainer.transform.GetChild(ztor).renderer.material.color = Color.grey;
			}

			if(newEdgeContainer.transform.GetChild(ztor).transform.position.x <= MinWorldWidth ||
				newEdgeContainer.transform.GetChild(ztor).transform.position.x >= MaxWorldWidth ||
				newEdgeContainer.transform.GetChild(ztor).transform.position.y <= MinWorldHeight ||
				newEdgeContainer.transform.GetChild(ztor).transform.position.y >= MaxWorldHeight)
			{
				GameObject.Destroy(newEdgeContainer.transform.GetChild(ztor).gameObject);
			}
		}
		
		newEdgeContainer.transform.parent = TearManager.MainWorldCutPaper.transform;
		TearManager.TornEdgeFXContainter = newEdgeContainer;		
	}
	
	/// <summary>
	/// Positions the depth F.
	/// </summary>
	private void PositionDepthFX()
	{
		GameObject[] FXobjects = GameObject.FindGameObjectsWithTag("tearEdgeFX");
		for(int itor = 0; itor< FXobjects.Length; itor++)
		{
			FXobjects[itor].transform.position = new Vector3(FXobjects[itor].transform.position.x, FXobjects[itor].transform.position.y, 1.01f);
			
			if(FXobjects[itor].transform.position.x == MaxWorldWidth ||
				FXobjects[itor].transform.position.x == MinWorldWidth ||
				FXobjects[itor].transform.position.y == MaxWorldHeight ||
				FXobjects[itor].transform.position.y == MinWorldHeight)
			{
				if(tearFXholder.Contains(FXobjects[itor]))
				{
					tearFXholder.Remove(FXobjects[itor]);
				}
				
				GameObject.Destroy(FXobjects[itor]);
			}
		}
	}
	
	/// <summary>
	/// Finishs the edge FX
	/// </summary>
	private void FinishEdgeFX()
	{
		float numbObjects = 0.2f;
		
		//iterate through the existing tear edge FX storage
		//to find lerp positions
		for(int itor = 0; itor + 1 < tearEdgeFXlist.Count(); itor++)
		{
			for(float lerpStatus = numbObjects; lerpStatus < 1; lerpStatus += numbObjects)
			{
				Vector3 newPos = Vector3.Lerp(tearEdgeFXlist.ElementAt(itor).transform.position, 
												 tearEdgeFXlist.ElementAt(itor + 1).transform.position,
												 lerpStatus);
				if(!allEdgeFX.Contains(newPos))
				{
					CreateNewTornEdgeFX2(newPos, true);
				}
			}
		}
	}
	
	/// <summary>
	/// Finds the missing torn edge FX
	/// </summary>
	private void FindMissingTornEdgeFX()
	{
		List<int> indeciesToInsertNewFX = new List<int>();
		
		int testingNumberOfPotentialMissing = 0;
		
		//iterate through tearEdgeFXlist gameobjects to shcek for potential missing edge FX
		for(int itor = 0; itor < tearEdgeFXlist.Count(); itor++)
		{
			//Make sure we have not hit end of list
			if(itor + 1 < tearEdgeFXlist.Count())
			{
				//Find distance between adjacent edge FX
				float dist = Vector2.Distance(new Vector2(tearEdgeFXlist.ElementAt(itor).transform.position.x, 
															tearEdgeFXlist.ElementAt(itor).transform.position.y), 
												new Vector2(tearEdgeFXlist.ElementAt(itor + 1).transform.position.x, 
															tearEdgeFXlist.ElementAt(itor + 1).transform.position.y));
				if(dist < 0) dist *= -1;
				
				//Check for adjacent tearEdgeFXlist gameObjects that are diagonal from one another
				if(dist > MESH_VERT_OFFSET && dist < ((1.5f) * MESH_VERT_OFFSET))
				{
					++testingNumberOfPotentialMissing;
					indeciesToInsertNewFX.Add (itor);
				}
			}
		}
		
		//Debug.LogError("Testing number of Potential missing edge FX objects = " + testingNumberOfPotentialMissing.ToString());
		
		//Loop through the diagnal agjacent edge FX to determine if any are missing
		for(int itor = 0; itor < indeciesToInsertNewFX.Count(); itor++)
		{
			int indexr = indeciesToInsertNewFX.ElementAt(itor) + numbMissingEdgeFX;
				
			//Get direction for positioning raycast for determining if missing edge FX
			Vector3 directionOffset = tearEdgeFXlist.ElementAt(indexr + 1).transform.position - 
																			tearEdgeFXlist.ElementAt(indexr).transform.position;
			
			//Here, we ray cast from the two diagonal directionOffset components to determine if
			//missing edge teear FX
			Vector3 rayPos1 = tearEdgeFXlist.ElementAt(indexr).transform.position;
			rayPos1.x += directionOffset.x;
			
			Vector3 rayPos2 = tearEdgeFXlist.ElementAt(indexr).transform.position;
			rayPos2.y += directionOffset.y;
			
			int test1 = RayCastOffsetPosition(rayPos1, "paper_LargerPiece", MESH_VERT_OFFSET/3);
			int test2 = RayCastOffsetPosition(rayPos2, "paper_LargerPiece", MESH_VERT_OFFSET/3);
			
			if(test1 != 12 && 
				test1 != 0 && rayPos1.y < MaxWorldHeight && rayPos1.y > MinWorldHeight
									  && rayPos1.x < MaxWorldWidth && rayPos1.x > MinWorldHeight)
			{
				Debug.LogError("Testing numb");
				CreateNewTornEdgeFX(rayPos1, true, true, indexr);
				++numbMissingEdgeFX;
			}
			if(test2 != 12 && 
				test2 != 0 && rayPos2.y < MaxWorldHeight && rayPos2.y > MinWorldHeight
									  && rayPos2.x < MaxWorldWidth && rayPos2.x > MinWorldHeight)	
			{
				Debug.LogError("Testing numb");
				CreateNewTornEdgeFX(rayPos2, true, true, indexr);
				++numbMissingEdgeFX;
			}
			
		}
		
		//Now we make sure any duplicates are properly disposed
		for(int itor = 0; itor + 1 < tearEdgeFXlist.Count(); itor++)
		{
			if(tearEdgeFXlist.ElementAt(itor).transform.position == 
				tearEdgeFXlist.ElementAt(itor + 1).transform.position)
			{
				
				Debug.LogError("testing 123");
				
				//Destroy unwanted edge FX object
				GameObject.Destroy (tearEdgeFXlist.ElementAt(itor));
				
				//Remove pointer
				tearEdgeFXlist.Remove(tearEdgeFXlist.ElementAt(itor));
				
				
				if(tearFXholder.Contains(tearEdgeFXlist.ElementAt(itor)))
				{
					tearFXholder.Remove(tearEdgeFXlist.ElementAt(itor));
				}
			}
		}
		
		
		//Cleanup memory
		indeciesToInsertNewFX.Clear();
		//indeciesToInsertNewFX.Clear();
		
	}
	
	
	/// <summary>
	/// Finds the missing torn edge FX
	/// </summary>
	private void FindMissingTornEdgeFX2()
	{
		List<int> indeciesToInsertNewFX = new List<int>();
		
		int testingNumberOfPotentialMissing = 0;
		
		//iterate through tearEdgeFXlist gameobjects to shcek for potential missing edge FX
		for(int itor = 0; itor < tearEdgeFXlist.Count(); itor++)
		{
			//Make sure we have not hit end of list
			if(itor + 1 < tearEdgeFXlist.Count())
			{
				//Find distance between adjacent edge FX
				float dist = Vector2.Distance(new Vector2(tearEdgeFXlist.ElementAt(itor).transform.position.x, 
															tearEdgeFXlist.ElementAt(itor).transform.position.y), 
												new Vector2(tearEdgeFXlist.ElementAt(itor + 1).transform.position.x, 
															tearEdgeFXlist.ElementAt(itor + 1).transform.position.y));
				if(dist < 0) dist *= -1;
				
				//Check for adjacent tearEdgeFXlist gameObjects that are diagonal from one another
				if(dist > MESH_VERT_OFFSET && dist < ((1.5f) * MESH_VERT_OFFSET))
				{
					++testingNumberOfPotentialMissing;
					indeciesToInsertNewFX.Add (itor);
				}
			}
		}
		
		//Debug.LogError("Testing number of Potential missing edge FX objects = " + testingNumberOfPotentialMissing.ToString());
		
		//Loop through the diagnal agjacent edge FX to determine if any are missing
		for(int itor = 0; itor < indeciesToInsertNewFX.Count(); itor++)
		{
			int indexr = indeciesToInsertNewFX.ElementAt(itor) + numbMissingEdgeFX;
				
			//Get direction for positioning raycast for determining if missing edge FX
			Vector3 directionOffset = tearEdgeFXlist.ElementAt(indexr + 1).transform.position - 
																			tearEdgeFXlist.ElementAt(indexr).transform.position;
			
			//Here, we ray cast from the two diagonal directionOffset components to determine if
			//missing edge teear FX
			Vector3 testPos1 = tearEdgeFXlist.ElementAt(indexr).transform.position;
			testPos1.x += directionOffset.x;
			testPos1.z = TearManager.MainWorldPaper.transform.position.z;
			
			Vector3 testPos2 = tearEdgeFXlist.ElementAt(indexr).transform.position;
			testPos2.y += directionOffset.y;
			testPos2.z = TearManager.MainWorldPaper.transform.position.z;
			
			
			
			if(FoundAssociatedVertice(testPos1) && testPos1.y < MaxWorldHeight && testPos1.y > MinWorldHeight
									  && testPos1.x < MaxWorldWidth && testPos1.x > MinWorldHeight)
			{
				Debug.LogError("Testing numb 1");
				CreateNewTornEdgeFX(testPos1, true, true, indexr);
				++numbMissingEdgeFX;
			}
			if(FoundAssociatedVertice(testPos2) && testPos2.y < MaxWorldHeight && testPos2.y > MinWorldHeight
									  && testPos2.x < MaxWorldWidth && testPos2.x > MinWorldHeight)	
			{
				Debug.LogError("Testing numb 2");
				CreateNewTornEdgeFX(testPos2, true, true, indexr);
				++numbMissingEdgeFX;
			}
			
		}
		
		//Now we make sure any duplicates are properly disposed
		for(int itor = 0; itor + 1 < tearEdgeFXlist.Count(); itor++)
		{
			if(tearEdgeFXlist.ElementAt(itor).transform.position == 
				tearEdgeFXlist.ElementAt(itor + 1).transform.position)
			{
				
				Debug.LogError("testing 123");
				
				//Destroy unwanted edge FX object
				GameObject.Destroy (tearEdgeFXlist.ElementAt(itor));
				
				//Remove pointer
				tearEdgeFXlist.Remove(tearEdgeFXlist.ElementAt(itor));
				
				
				if(tearFXholder.Contains(tearEdgeFXlist.ElementAt(itor)))
				{
					tearFXholder.Remove(tearEdgeFXlist.ElementAt(itor));
				}
			}
		}
		
		//Cleanup memory
		indeciesToInsertNewFX.Clear();
		
	}
	
	/// <summary>
	/// The number of missing edge FX
	/// </summary>
	private int numbMissingEdgeFX = 0;
	
	/// <summary>
	/// Ensures the excess edge are properly FX disposed.
	/// </summary>
	private void EnsureExcessEdgeFXDisposed()
	{
		//Find all objects that are tearEdgeFX
		GameObject[] tearFX = GameObject.FindGameObjectsWithTag("tearEdgeFX");
		
		//Keep track of the number of original edge vertices that are now 
		//torn edge vertices used to create the tearing edge FX
		int numOriginalEdges = 0;
		
		//Loop through every tear edge FX object in current scene
		for(int itor = 0; itor < tearFX.Length; itor++)
		{
			if(tearFX[itor].transform.position.x >= MaxWorldWidth || 
				tearFX[itor].transform.position.y >= MaxWorldHeight ||
				tearFX[itor].transform.position.x <= MinWorldWidth ||
				tearFX[itor].transform.position.y <= MinWorldHeight)
			{
				++numOriginalEdges;
			}
		}
	}
	
	/// <summary>
	/// Ensures the tear edge FX placement along torn edge
	/// </summary>
	private void EnsureTearEdgeFXplacement()
	{
		//Find all objects that are tearEdgeFX
		GameObject[] tearFX = GameObject.FindGameObjectsWithTag("tearEdgeFX");
		
		
		for(int itor = 0; itor < tearFX.Length; itor++)
		{
			
			//IF none of the above raycasts hit the largerPiece, we delete them
			if(!FoundAssociatedVertice(tearFX[itor].transform.position))
			{
				Debug.LogError("testing 123453");
				
				//Destroy unwanted edge FX object
				GameObject.Destroy (tearFX[itor]);
				
				//Remove pointer
				tearEdgeFXlist.Remove(tearFX[itor]);
				
				
				if(tearFXholder.Contains(tearFX[itor]))
				{
					tearFXholder.Remove(tearFX[itor]);
				}
			}
		}
		
		
		/*
		for(int itor = 0; itor < tearFX.Length; itor++)
		{
			//Find number of times we hit paper_LargerPiece
			int freq = RayCastOffsetPosition(tearFX[itor].transform.position, "paper_LargerPiece", MESH_VERT_OFFSET/3);
			
			//IF none of the above raycasts hit the largerPiece, we delete them
			if((freq == 0))
			{
				Debug.LogError("testing 123");
				
				//Destroy unwanted edge FX object
				GameObject.Destroy (tearFX[itor]);
				
				//Remove pointer
				tearEdgeFXlist.Remove(tearFX[itor]);
				
				
				if(tearFXholder.Contains(tearFX[itor]))
				{
					tearFXholder.Remove(tearFX[itor]);
				}
			}
		}
		*/
	}
	
	/// <summary>
	/// Founds the associated vertice for each edge FX position
	/// This ensures proper placement
	/// </summary>
	private bool FoundAssociatedVertice(Vector3 tearEdgeFxPos)
	{
		bool returnVal = false;
		
		
		for(int itor = 0; 
			itor < TearManager.MainWorldPaper.GetComponent<MeshFilter>().mesh.vertices.Count(); 
			++itor)
		{
			if(TearManager.MainWorldPaper.GetComponent<MeshFilter>().mesh.triangles.Contains(itor))
			{
				Vector3 testPos1 = TearManager.MainWorldPaper.transform.TransformPoint(
					TearManager.MainWorldPaper.GetComponent<MeshFilter>().mesh.vertices[itor]);
				
				testPos1.z = 0;
				tearEdgeFxPos.z = 0;
				
				if(testPos1 == tearEdgeFxPos)
				{
					returnVal = true;
					break;
				}
			}
		}
		
		
		return returnVal;
	}
	
	/// <summary>
	/// Creates the paper world torn edge FX
	/// </summary>
	private void CreatePaperWorldTornEdgeFX()
	{
		for(int jtor = 0; jtor < tearLine.Keys.Count(); jtor++)
		{
			Vector3 newPos = tearLine.Keys.ElementAt(jtor);
			newPos.z -= 0.1f;
			CreateNewTornEdgeFX(newPos, true, false, 420);
		}
		/*
		for(int jtor = 0; jtor < tearEdgeFXlist.Count(); jtor++)
		{
			int test1 = RayCastOffsetPosition(tearEdgeFXlist.ElementAt(jtor).transform.position, "paper_LargerPiece", MESH_VERT_OFFSET/3);
			
			if(test1 == 12 || test1 == 0)
			{
				//Debug.LogError("testing 45645");
				
				//Destroy unwanted edge FX object
				GameObject.Destroy (tearEdgeFXlist.ElementAt(jtor));
				
				//Remove pointer
				tearEdgeFXlist.Remove(tearEdgeFXlist.ElementAt(jtor));
				
				
				if(tearFXholder.Contains(tearEdgeFXlist.ElementAt(jtor)))
				{
					tearFXholder.Remove(tearEdgeFXlist.ElementAt(jtor));
				}
			}
		}
		*/
		
	}
	
	/// <summary>
	/// Raycast offset position, return how many times hitting ObjectName
	/// </summary>
	private int RayCastOffsetPosition(Vector3 pos, string ObjectName, float RayOffset)
	{
		
		RaycastHit hit;
		Vector3 rayPos = pos;
		rayPos.x += RayOffset;
		rayPos.y += RayOffset;
		rayPos.z += 0.1f;
		
		int returnVal = 0;

		if(Physics.Raycast(rayPos, Camera.main.transform.forward, out hit, 20) 
			&& hit.collider.gameObject.name == ObjectName
			)
		{
			++returnVal;
		}
		
		rayPos = pos;
		rayPos.x -= RayOffset;
		rayPos.y += RayOffset;
		rayPos.z += 0.1f;
		if(Physics.Raycast(rayPos, Camera.main.transform.forward, out hit, 20) 
			&& hit.collider.gameObject.name == ObjectName
			)
		{
			++returnVal;
		}
		
		rayPos = pos;
		rayPos.x -= RayOffset;
		rayPos.y -= RayOffset;
		rayPos.z += 0.1f;
		if(Physics.Raycast(rayPos, Camera.main.transform.forward, out hit, 20) 
			&& hit.collider.gameObject.name == ObjectName
			)
		{
			++returnVal;
		}
		
		rayPos = pos;
		rayPos.x += RayOffset;
		rayPos.y -= RayOffset;
		rayPos.z += 0.1f;
		if(Physics.Raycast(rayPos, Camera.main.transform.forward, out hit, 20) 
			&& hit.collider.gameObject.name == ObjectName
			)
		{
			++returnVal;
		}
		
		//return returnVal;
		
		
		///
		/// BLOCKED OUT - ^ RAYCAST PER TEAR EDGE FX OBJECT FOR ENSURANCE ON PLACEMENT
		///
		
		//RaycastHit hit;
		//Vector3 
			rayPos = pos;
		rayPos.x += RayOffset/2;
		rayPos.y += RayOffset;
		rayPos.z += 0.1f;
		
		//int returnVal = 0;

		if(Physics.Raycast(rayPos, Camera.main.transform.forward, out hit, 20) 
			&& hit.collider.gameObject.name == ObjectName
			)
		{
			++returnVal;
		}
		
		rayPos = pos;
		rayPos.x -= RayOffset/2;
		rayPos.y += RayOffset;
		rayPos.z += 0.1f;
		if(Physics.Raycast(rayPos, Camera.main.transform.forward, out hit, 20) 
			&& hit.collider.gameObject.name == ObjectName
			)
		{
			++returnVal;
		}
		
		rayPos = pos;
		rayPos.x -= RayOffset/2;
		rayPos.y -= RayOffset;
		rayPos.z += 0.1f;
		if(Physics.Raycast(rayPos, Camera.main.transform.forward, out hit, 20) 
			&& hit.collider.gameObject.name == ObjectName
			)
		{
			++returnVal;
		}
		
		rayPos = pos;
		rayPos.x += RayOffset/2;
		rayPos.y -= RayOffset;
		rayPos.z += 0.1f;
		if(Physics.Raycast(rayPos, Camera.main.transform.forward, out hit, 20) 
			&& hit.collider.gameObject.name == ObjectName
			)
		{
			++returnVal;
		}
		
		rayPos = pos;
		rayPos.x += RayOffset;
		rayPos.y -= RayOffset/2;
		rayPos.z += 0.1f;
		if(Physics.Raycast(rayPos, Camera.main.transform.forward, out hit, 20) 
			&& hit.collider.gameObject.name == ObjectName
			)
		{
			++returnVal;
		}
		
		rayPos = pos;
		rayPos.x += RayOffset;
		rayPos.y += RayOffset/2;
		rayPos.z += 0.1f;
		if(Physics.Raycast(rayPos, Camera.main.transform.forward, out hit, 20) 
			&& hit.collider.gameObject.name == ObjectName
			)
		{
			++returnVal;
		}
		
		rayPos = pos;
		rayPos.x -= RayOffset;
		rayPos.y -= RayOffset/2;
		rayPos.z += 0.1f;
		if(Physics.Raycast(rayPos, Camera.main.transform.forward, out hit, 20) 
			&& hit.collider.gameObject.name == ObjectName
			)
		{
			++returnVal;
		}
		
		rayPos = pos;
		rayPos.x -= RayOffset;
		rayPos.y += RayOffset/2;
		rayPos.z += 0.1f;
		if(Physics.Raycast(rayPos, Camera.main.transform.forward, out hit, 20) 
			&& hit.collider.gameObject.name == ObjectName
			)
		{
			++returnVal;
		}	
		
		return returnVal;
	}
	
	/// <summary>
	/// Checks for bad tear.
	/// </summary>
	private void CheckForBadTear()
	{
		//The following will be called when the tearline is too small 
		//(along an edge)
		if(CurrentCuttPiece == null)
		{
			
			ForceStopTear("ignore");
			hitPlayerDuringTear = -1;
			return;
		}
		
		
		if(!PlatformPaper)
		{
			//if(BoundsCheckOnTornPiece(PlayerObjectRef.transform.position) ||
			//	BoundsCheckOnTornPiece(EndGoalObjectRef.transform.position))
			{
				int currCutPieceMeshTriCount= CurrentCuttPiece.GetComponent<MeshFilter>().mesh.triangles.Count();
				int[] currCutPieceMeshTri = CurrentCuttPiece.GetComponent<MeshFilter>().mesh.triangles;
				Vector3[] currCutPieceMeshVert = CurrentCuttPiece.GetComponent<MeshFilter>().mesh.vertices;
				for(int itor = 0; itor < currCutPieceMeshTriCount; itor++)
				{
					Vector3 worldVertPos = CurrentCuttPiece.transform.TransformPoint(
										currCutPieceMeshVert[currCutPieceMeshTri[itor]]
																					);
					worldVertPos.z = PlayerObjectRef.transform.position.z;
					
					Vector2 testPos1 = new Vector2(worldVertPos.x, worldVertPos.y);
					
					Vector2 testPos2 = new Vector2(PlayerObjectRef.transform.position.x, 
						PlayerObjectRef.transform.position.y);
					
					float testDist = Vector2.Distance(testPos1, testPos2);
					if(testDist < 0) testDist *= -1;
					
					if(//PlayerObjectRef.GetComponent<CapsuleCollider>().bounds.Contains(worldVertPos) || 
						testDist < (MESH_VERT_OFFSET*(3/2)))
					{
						//UnityEngine.Debug.Log("PlayerHasTornIncorrectly 3");
						if(!TearManager.BadTear)
						{
							ForceStopTear("magicCarpet");
				hitPlayerDuringTear = 1;
							hitPlayerDuringTear = 2;
						}
						return;
					}
					
					testPos2 = new Vector2(EndGoalObjectRef.transform.position.x, 
						EndGoalObjectRef.transform.position.y);
					
					testDist = Vector2.Distance(testPos1, testPos2);
					if(testDist < 0) testDist *= -1;
					
					//worldVertPos.z = GameObject.FindGameObjectWithTag("EndGoal").transform.position.z;
					if(//EndGoalObjectRef.GetComponent<BoxCollider>().bounds.Contains(worldVertPos) || 
						testDist < (MESH_VERT_OFFSET*(3/2)))
					{
						//UnityEngine.Debug.Log("PlayerHasTornIncorrectly 4");
						if(!TearManager.BadTear)
						{
							ForceStopTear("door");
							hitPlayerDuringTear = 1;
						}
						return;
					}
					
					//TODO PREVENT TEARING OUT DOOR OR OTHER NON TEARABLE OBJECT!!!!
					/*
					worldVertPos.z = GameObject.FindGameObjectWithTag("Door").transform.position.z;
					if(GameObject.FindGameObjectWithTag("Door").GetComponent<BoxCollider>().bounds.Contains(worldVertPos))
					{
						PlayerTornUnTearableObjects();
						return;
					}
					*/
					
					hitPlayerDuringTear = 0;
				}
			}
		}
	}
	
	/// <summary>
	/// This function is used to return true if the test position is
	/// within the bounds of the torn Piece
	/// </summary>
	/// <returns>
	private bool BoundsCheckOnTornPiece(Vector3 testPos)
	{
		testPos.z = TearManager.MainWorldCutPaper.collider.transform.position.z;
		
		if(TearManager.MainWorldCutPaper.collider.bounds.Contains(testPos))
			return true;
		else return false;
	}
	
	/// <summary>
	/// The force finish tear line flags when the tear needs to be forcfully completed
	/// </summary>
	public bool ForceFinishTearLine = false;
	
	/// <summary>
	/// Checks for loop tear. here we iterate through the mesh
	/// structure and compare each neighbor vertices.
	/// IFF two vertices are neighbors and they have tearTimes
	/// which are not related, then this function returns true to trigger
	/// a Bad Tear
	/// </summary>
	private bool CheckForLoopTear()
	{
		//iterate through mech structure
		for(int iterator = 0; iterator < tearLine.Count(); iterator++)
		{
			for(int jtor = iterator + 5; jtor < tearLine.Count(); jtor++)
			{
				float testDist = Vector3.Distance(tearLine.Keys.ElementAt(iterator), tearLine.Keys.ElementAt(jtor));
				if(testDist < 0) testDist *= -1;
				
				if(testDist < (MESH_VERT_OFFSET * 2))
				{
					return true;
				}
			}
			
		}
		
		//Return false if loop not found above
		return false;
	}
	
	/// <summary>
	/// Check for object which will break tearing logic, then is convexity is true,
	/// the object is recursively split until tearing will work 
	/// - NOT FINISHED, MULTI TEAR PUT ON HOLD (4/10/2013, JC)
	/// </summary>
	private void DynamicObjectSplitCheck()
	{
		//Only call the following once after startup, or after a cut/tear
		if(!haveCheckedForConvexEdges && !CloneObject)
		{
			int itor = 0;
			bool testflag = true;
			//The following checks for convex edge slopes and creates new objects,
			//allowing the tear algorithm to function properly
			
			while(testflag)
			{
				++itor;
				Debug.LogError("**************In While LOOP****************");
				testflag = CheckForConvexMesh();
				
				//This is called when this logic is not working
				if(itor > 10) break;
			}
			Debug.LogError("itor testing = " + itor.ToString());
			
			
			//We only want to call this function once
			haveCheckedForConvexEdges = true;
		}
	}

	/// <summary>
	/// This function is called once after startup or after a tear/cut to
	/// check for convex slope along mesh edge. If any exist, this
	/// function is used to duplicate the object and create non-convex shapes
	/// </summary>
	private bool CheckForConvexMesh()
	{
		//This flag triggers true when a convex shape is discovered in mesh
		bool convexFlag = false;
		List<float> paperGridKeys = paperGrid.Keys.ToList();
		
		//loop through the current mesh
		for(int itor = 0; itor < paperGrid.Keys.Count(); itor++)
		{
			//loop through the current mesh row
			for(int jtor = 0; jtor < paperGrid[paperGridKeys[itor]/*paperGrid.Keys.ElementAt(itor)*/].Count(); jtor++)
			{
				//Debug.LogError("**************Checking distance****************");
				if(jtor > 0)
				{
					float distanceXCheck = paperGrid[paperGridKeys[itor]].ElementAt(jtor).x - paperGrid[paperGridKeys[itor]].ElementAt(jtor - 1).x;
					float distanceYCheck = paperGrid[paperGridKeys[itor]].ElementAt(jtor).y - paperGrid[paperGridKeys[itor]].ElementAt(jtor - 1).y;
					
					Debug.LogError("**************Checking distance**************** x = " + distanceXCheck.ToString() + ", y = " + distanceYCheck.ToString());
					
					if(distanceXCheck < 0) distanceXCheck *= -1;
					if(distanceYCheck < 0) distanceYCheck *= -1;
					
					if(distanceYCheck <= MESH_VERT_OFFSET && distanceYCheck <= MESH_VERT_OFFSET)
					{
						convexFlag = false;
					}
					else
					{
						convexFlag = true;
						break;
					}
				}
				else
				{
					convexFlag = false;
				}
			}
			if(convexFlag) break;
		}
		
		//Check if a convex shape is disconvered
		if(convexFlag)
		{
			++testingFrequency;
			//GameObject duplicateObj = (GameObject)Instantiate(newPaper, this.transform.position, this.transform.rotation);
			List<Vector3> isl1 = new List<Vector3>();
			List<Vector3> isl2 = new List<Vector3>();
			
			Debug.LogError("******************************************************************CONVEX SHAPE FOUND***********************************************************");
			
			//loop through the current mesh
			for(int itor = 0; itor < paperGrid.Keys.Count(); itor++)
			{
				bool haveSwitchThisRow = false;
				
				//loop through the current mesh row
				for(int jtor = 0; jtor < paperGrid[paperGridKeys[itor]].Count(); jtor++)
				{
					//Debug.LogError("********perform Convex Test********");
					//check is we aree past first position is row
					if(jtor > 0)
					{
						Vector3 distanceCheck = paperGrid[paperGridKeys[itor]].ElementAt(jtor) - paperGrid[paperGridKeys[itor]].ElementAt(jtor - 1);
						
						if(distanceCheck.x < 0) distanceCheck.x *= -1;
						if(distanceCheck.y < 0) distanceCheck.y *= -1;
						
						if(!haveSwitchThisRow && distanceCheck.x <= MESH_VERT_OFFSET && distanceCheck.y <= MESH_VERT_OFFSET)
						{
							isl1.Add(paperGrid[paperGridKeys[itor]].ElementAt(jtor));
						}
						
						else if(!haveSwitchThisRow)
						{
							
							haveSwitchThisRow = true;
						}
						else if(haveSwitchThisRow)
						{
							isl2.Add(paperGrid[paperGridKeys[itor]].ElementAt(jtor));
						}
					}
					else
					{
						//First poistion is row, always island 1
						isl1.Add(paperGrid[paperGridKeys[itor]].ElementAt(jtor));
					}
				}
			}
			
			//CuttPieceOneFaces = Clone.GetComponent<MeshFilter>().mesh.triangles;
			//CuttPieceTwoFaces = Clone.GetComponent<MeshFilter>().mesh.triangles;
			//CuttPieceOneVerts = Clone.GetComponent<MeshFilter>().mesh.vertices;
			//CuttPieceTwoVerts = Clone.GetComponent<MeshFilter>().mesh.vertices;
			
			int[] isl1Ind = new int[thisMeshFilter.mesh.triangles.Count()];
			int[] isl2Ind = new int[thisMeshFilter.mesh.triangles.Count()];
			
			indexor1 = 0;
			indexor2 = 0;
			
			for(int index = 0; index < thisMeshFilter.mesh.triangles.Count(); index += 3)
			{
				Vector3 testPos = this.transform.TransformPoint(thisMeshFilter.mesh.vertices[thisMeshFilter.mesh.triangles[index]]);
				Vector3 testPos1 = this.transform.TransformPoint(thisMeshFilter.mesh.vertices[thisMeshFilter.mesh.triangles[index + 1]]);
				Vector3 testPos2 = this.transform.TransformPoint(thisMeshFilter.mesh.vertices[thisMeshFilter.mesh.triangles[index + 1]]);
				
				
				//Now we create each new vertice array for each mewMesh
				if( isl1.Contains(testPos) && 
					isl1.Contains(testPos1) && 
					isl1.Contains(testPos2))
				{
					//Debug.LogError("Currently adding the following to island 2*********: " + CuttPieceOneVerts[CuttPieceOneFaces[index]].ToString() + " , " + CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].ToString() + " , " + CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].ToString());
					isl1Ind[indexor1] = thisMeshFilter.mesh.triangles[index];
					isl1Ind[indexor1 + 1] = thisMeshFilter.mesh.triangles[index + 1];
					isl1Ind[indexor1 + 2] = thisMeshFilter.mesh.triangles[index + 2];
					indexor1 += 3;	
						
				}
				else
				{
					isl2Ind[indexor2] = thisMeshFilter.mesh.triangles[index];
					isl2Ind[indexor2 + 1] = thisMeshFilter.mesh.triangles[index + 1];
					isl2Ind[indexor2 + 2] = thisMeshFilter.mesh.triangles[index + 2];
					indexor2 += 3;
				}
			}
			
			GameObject newPiece = (GameObject)Instantiate(newPaper, this.transform.position, this.transform.rotation);
			newPiece.GetComponent<TearPaper>().CloneObject = true;
			newPiece.name = "Poopy #" + testingFrequency.ToString();
			
			newPiece.GetComponent<MeshFilter>().mesh.triangles = isl1Ind;
			newPiece.GetComponent<MeshCollider>().sharedMesh = newPiece.GetComponent<MeshFilter>().mesh;
			thisMeshFilter.mesh.triangles = isl2Ind;
			//this.mesh.RecalculateBounds();
			this.GetComponent<MeshCollider>().sharedMesh = thisMeshFilter.mesh;
			
			//haveCheckedForConvexEdges = false;
			
			Debug.LogError("********About to Set paper grid********");
			SetPaperGrid();
			
			isl1.Clear();
			isl2.Clear();
			
		}
		else
		{
			Debug.LogError("********CONVEX = FALSE********");
		}
		Debug.LogError("********CONVEX = " + convexFlag.ToString());
		return convexFlag;
		
		
	}
	
	/// <summary>
	/// The torn edge visual fx reference.
	/// </summary>
	private List<GameObject> tornEdgeVisualFxRef;
	
	private int currentItor = 0;
	private GameObject[] TearEdgeFXPool;
	
	/// <summary>
	/// Creates the new decal GameObject
	/// </summary>
	private void CreateNewTornEdgeFX(Vector3 pos, bool addToLerpStorage, bool insertingLate, int lateInsert)
	{
		
		
		//Create new particle decal object
		GameObject newdecal = TearEdgeFXPool[currentItor];//(GameObject)Instantiate(TearManager.EdgeDecalObject);
		++currentItor;
		
		
		newdecal.name = "TornEdge_DecalObject";
		newdecal.tag = "tearEdgeFX";
		newdecal.transform.position = pos;
		//newdecal.transform.position = new Vector3(newdecal.transform.position.x, newdecal.transform.position.y, 0.9f);
		
		//RANDOM ROTATION
		//newdecal.transform.Rotate(new Vector3(0,0, UnityEngine.Random.Range(0, 360)));
		
		newdecal.transform.localScale = new Vector3(0.1f, 0.1f, 1.0f);
		
		newdecal.renderer.material.color = Color.white;
		
		//Parent to container
		newdecal.transform.parent = tearEdgeFXcontainer.transform;
		
		if(addToLerpStorage && !insertingLate)
		{
			tearEdgeFXlist.Add(newdecal);
		}
		else if(addToLerpStorage && insertingLate)
		{
			tearEdgeFXlist.Insert(lateInsert + 1, newdecal);
		}
		
		tearFXholder.Add(newdecal);
		
		//if(newdecal.transform.position.y >= MaxWorldHeight || newdecal.transform.position.y || MinWorldHeight
		//	&& newdecal.transform.position.x ||MaxWorldWidth && newdecal.transform.position.x || MinWorldHeight
		
		if(tornEdgeVisualFxRef == null) tornEdgeVisualFxRef = new List<GameObject>();
		tornEdgeVisualFxRef.Add(newdecal);
	}
	
	/// <summary>
	/// All edge FX objcet within scene
	/// </summary>
	private List<Vector3> allEdgeFX = new List<Vector3>();
	
	/// <summary>
	/// Creates the new decal GameObject
	/// </summary>
	private void CreateNewTornEdgeFX2(Vector3 pos, bool lerping)
	{
		//Create new particle decal object
		GameObject newdecal = (GameObject)Instantiate(TearManager.EdgeDecalObject, pos, Quaternion.identity);
		
		newdecal.name = "TornEdge_DecalObject";
		newdecal.tag = "tearEdgeFX";
		newdecal.transform.position = pos;
		newdecal.transform.position = new Vector3(newdecal.transform.position.x, newdecal.transform.position.y, 1.01f);
		
		//RANDOM ROTATION
		newdecal.transform.Rotate(new Vector3(0,0, UnityEngine.Random.Range(0, 360)));
		
		newdecal.transform.localScale = new Vector3(0.05f, 0.05f, 1.0f);
		
		newdecal.renderer.material.color = Color.white;
		
		//Parent to container
		newdecal.transform.parent = tearEdgeFXcontainer.transform;
		
		if(!lerping)
		{
			tearEdgeFXlist.Add(newdecal);
			
			tearFXholder.Add(newdecal);
		}
		allEdgeFX.Add(newdecal.transform.position);

		
		//if(newdecal.transform.position.y >= MaxWorldHeight || newdecal.transform.position.y || MinWorldHeight
		//	&& newdecal.transform.position.x ||MaxWorldWidth && newdecal.transform.position.x || MinWorldHeight
	}
	
	/// <summary>
	/// Creates the new decal GameObject
	/// </summary>
	private void CreateNewTornEdgeFX3(Vector3 pos)
	{
		//Create new particle decal object
		GameObject newdecal = (GameObject)Instantiate(TearManager.EdgeDecalObject, TearManager.EdgeDecalObject.transform.position, Quaternion.identity);
		
		newdecal.name = "TornEdge_DecalObject";
		newdecal.tag = "tearEdgeFX";
		newdecal.transform.position = pos;
		newdecal.transform.position = new Vector3(newdecal.transform.position.x, newdecal.transform.position.y, 1.01f);
		
		//RANDOM ROTATION
		newdecal.transform.Rotate(new Vector3(0,0, UnityEngine.Random.Range(0, 360)));
		
		newdecal.transform.localScale = new Vector3(0.08f, 0.08f, 1.0f);
		
		newdecal.renderer.material.color = Color.white;
		
		//Parent to container
		newdecal.transform.parent = tearEdgeFXcontainer.transform;
	}
	
	
	int testFreq = 0;
	/// <summary>
	/// Checks the frequency of edge occuring within MainWorldCutPiece
	/// </summary>
	/// <returns>
	private int CheckFrequencyOfEdge(int index1, int index2)
	{
		++testFreq;
		int returnFreq = 0;
		
		for(int itor = 0; 
			itor < tearManagerMainCutPieceMeshFilter.mesh.triangles.Length;
			itor += 3)
		{
			if((tearManagerMainCutPieceMeshFilter.mesh.triangles[itor] == index1 ||
				tearManagerMainCutPieceMeshFilter.mesh.triangles[itor + 1] == index1 ||
				tearManagerMainCutPieceMeshFilter.mesh.triangles[itor + 2] == index1)
				&&
				
				(tearManagerMainCutPieceMeshFilter.mesh.triangles[itor] == index2 ||
				tearManagerMainCutPieceMeshFilter.mesh.triangles[itor + 1] == index2 ||
				tearManagerMainCutPieceMeshFilter.mesh.triangles[itor + 2] == index2)
				)
			{
				++returnFreq;
			}
			
			if(returnFreq >= 2) return returnFreq;
		}
		
		return returnFreq;
	}
	
	/// <summary>
	/// The tear FX gameObject holder.
	/// </summary>
	List<GameObject> tearFXholder = new List<GameObject>();
	
	/// <summary>
	/// Creates the new torn edge FX duplicate.
	/// </summary>
	private void CreateNewTornEdgeFXDuplicate(Vector3 pos)
	{
		//Create new particle decal object
		GameObject newdecal = (GameObject)Instantiate(TearManager.EdgeDecalObject);
		newdecal.name = "TornEdge_DecalObject";
		newdecal.tag = "tearEdgeFX";
		newdecal.transform.position = pos;
		//newdecal.transform.position = new Vector3(newdecal.transform.position.x, newdecal.transform.position.y, 0.9f);
		newdecal.transform.Rotate(new Vector3(0,0, UnityEngine.Random.Range(0, 360)));
		newdecal.transform.localScale = new Vector3(0.1f, 0.1f, 1.0f);
		
		newdecal.renderer.material.color = Color.white;
		
		//Parent to container
		newdecal.transform.parent = TearManager.MainWorldCutPaper.transform;
	}
	
	/// <summary>
	/// Adds the missing tear mouse position to the mousePositions list to
	/// fill in the caps of the mousePosition input curve
	/// </summary>
	private void AddMissingTearMousePositions(Vector3 mousePPos, Vector3 mouseCurPos, float distance)
	{
		//This represents how many missing points are needed to fill in the current gap found
		float numberOfNewPositions = distance/(MESH_VERT_OFFSET/3);
		
		//LERP from the prev, to the current mouse positions, number of iterations dependent upon
		// the distance between the two points
		for(float itor = 1/numberOfNewPositions; itor <= numberOfNewPositions; itor += 1/numberOfNewPositions)
		{
			//Create new interpolated position
			Vector3 newPos = Vector3.Lerp(mousePPos, mouseCurPos, itor);
			
			//Add the new interpolated position to the mouse position list
			mouseTearPositions.Add (new Vector3(Camera.mainCamera.WorldToScreenPoint(newPos).x, 
						Camera.mainCamera.WorldToScreenPoint(newPos).y, Camera.mainCamera.WorldToScreenPoint(newPos).z));//Input.mousePosition.
			
		}
	}
	
	/// <summary>
	/// Adds the missing tear mouse position to the mousePositions list to
	/// fill in the caps of the mousePosition input curve in screen space
	/// </summary>
	private void AddMissingTearMousePositionsScreen(Vector3 mousePPos, Vector3 mouseCurPos, float distance)
	{
		//This represents how many missing points are needed to fill in the current gap found
		float numberOfNewPositions = distance/(MESH_VERT_OFFSET/3);
		
		//LERP from the prev, to the current mouse positions, number of iterations dependent upon
		// the distance between the two points
		for(float itor = 1/numberOfNewPositions; itor <= numberOfNewPositions; itor += 1/numberOfNewPositions)
		{
			//Create new interpolated position
			Vector3 newPos = Vector3.Lerp(mousePPos, mouseCurPos, itor);
			
			//Add the new interpolated position to the mouse position list
			mouseTearPositions.Add (newPos);
			
		}
	}
	
	/// <summary>
	/// This function takes in the object's mesh to create 
	/// an organized, grid like, structure
	/// </summary>
	private void SetPaperGrid()
	{
		//Initialize the storage keeping tracking of edge information
		if(edgeOfObject == null) edgeOfObject = new Dictionary<Vector3, bool>();
		else edgeOfObject.Clear();
		
		//The following determines which vertices are interior and which are edge
		SetEdgeVertsOfObject();
		
		//Initialize the relationships between edge vertices
		SetEdgeRelationShips();
		
		//Debug.LogError("********ENTERING paper grid Set********");
		
		if(paperGridTearVertCheck == null) paperGridTearVertCheck = new Dictionary<Vector2, bool>();
		else paperGridTearVertCheck.Clear();
		
		if(paperGrid == null) paperGrid = new Dictionary<float, List<Vector3>>();
		else paperGrid.Clear();
		Vector3 point;
		//making mesh vert, length, and triangle here so we're not calling so often to find them
		Vector3[] thisMeshFilterVerts = thisMeshFilter.mesh.vertices;
		int thisMeshFilterVertsLength = thisMeshFilterVerts.Length;
		int[] thisMeshFilterTris = thisMeshFilter.mesh.triangles;
		HashSet<int> thisMeshFilterTriHS = CreateHashSet(thisMeshFilterTris);
		//Load grid dictionary
		for(int itor = 0; itor < thisMeshFilterVertsLength; itor++)
		{
			//New world vertice location 
			//Vector3 point = this.transform.TransformPoint(new Vector3(mesh.vertices[itor].x, mesh.vertices[itor].y, mesh.vertices[itor].z));
			point = gameObject.transform.TransformPoint(thisMeshFilterVerts[itor]);
			//Vector3 point = this.GetComponent<MeshFilter>().mesh.vertices[itor];
			
			//Make sure the vertice index is referenced by triangles (i.e. onyle add visible vertices)
			if(thisMeshFilterTriHS.Contains(itor))//thisMeshFilterTris.Contains(itor))
			{
				//Ensure dictionary is initialized
				if(verticePosIndexIntoMesh == null)
				{
					verticePosIndexIntoMesh = new Dictionary<Vector3, int>();
				}
				//Here we store the index into mesh for each vertice
				verticePosIndexIntoMesh.Add(point , itor);
				
				//Row dependent, therefore we store the height first
				paperGridTearVertCheck.Add(new Vector2(point.y, point.x), false);
				
				//If the key doesn't already exist within dictionary
				if(!paperGrid.ContainsKey(point.y))
				{
					//Debug.LogError("Adding new row to papergrid");
					List<Vector3> valueDict = new List<Vector3>();
					valueDict.Add(point);
					paperGrid.Add(point.y, valueDict);//[mesh.vertices[itor].y] = valueDict;
				}
				else
				{
					//Debug.LogError("Adding new colomn value to papergrid");
					//List<Vector3> newList = paperGrid[this.transform.TransformPoint(mesh.vertices[itor]).y];
					//newList.Add(point);
					paperGrid[point.y].Add(point);
					//paperGrid[point.y] = newList;
				}
			}
		}
		//originally inside for loop under if
		Vector3 node1;
		Vector3 node2;
		//bubble sort each list (colomns) within the dictionary
		//TODO: Optomize into heap, quick, merge, or another faster sorting alg
		foreach(List<Vector3> list in paperGrid.Values)
		{
			//bubble sort each row of grid
			for(int itor = 0; itor < list.Count; itor++)
			{
				for(int jtor = 0; jtor < list.Count; ++jtor)
				{
					if(	list[jtor].x > list[itor].x )
					{
						//Then we swap
						node1 = list[jtor];
						node2 = list[itor];
						list[jtor] = node2;
						list[itor] = node1;
					}
				}
			}
		}
		
		//Now we sort each row in dictionary
		List<float> tempList122 = paperGrid.Keys.ToList();
		
		
		//TODO: Check if the following sort method is more efficient than other we can create (such as heap)
		
		
		//tempList122.Sort();
		//originally declared inside for loop under if statement, was called node1 and node2
		float node1f;
		float node2f;
		for(int itor = 0; itor < tempList122.Count(); itor++)
		{
			for(int jtor = 0; jtor < tempList122.Count(); ++jtor)
			{
				if(tempList122[jtor] > tempList122[itor])
				{
					//Then we swap
					node1f = tempList122[jtor];
					node2f = tempList122[itor];
					tempList122[jtor] = node2f;
					tempList122[itor] = node1f;
				}
			}
		}
		
		
		
		//was originally declared inside for loop
		List<Vector3> newList2;
		float index2;
		//Now that we have a list representing the sorted keys,
		Dictionary<float, List<Vector3>> newPaperGrid = new Dictionary<float, List<Vector3>>();
		for(int indexr = 0; indexr < tempList122.Count; indexr++)
		{
			newList2 = paperGrid[tempList122/*.ElementAt(*/[indexr]];
			index2 = tempList122.ElementAt(indexr);
			//Debug.LogError("Adding to newPaperGrid");
			
			newPaperGrid.Add(index2, newList2);
		}
		paperGrid.Clear();
		//Assign the papergrid to the new grid with organized rows
		paperGrid = newPaperGrid;

	}
	//this function will create a hashset with given values in it
	HashSet<int> CreateHashSet(int[] values)
	{
		HashSet<int> hashVal = new HashSet<int>();
		for(int i = 0; i < values.Length; i++)
		{
			hashVal.Add(values[i]);
		}
		return hashVal;
	}
	/// <summary>
	/// Draws the tear line only.
	/// ******THIS IS FOR TESTING PURPOSED ONLY, this will only draw the cutt/tear line******
	/// </summary>
	private void DrawTearLineOnly()
	{
		
		int[] newFaces = new int[Clone.GetComponent<MeshFilter>().mesh.triangles.Length];
		int intorator = 0;
		//int triangleCount = 1;
		for(int itor = 0; itor < Clone.GetComponent<MeshFilter>().mesh.triangles.Length; itor += 3)
		{
			if(tearLine.ContainsValue(Clone.GetComponent<MeshFilter>().mesh.triangles[itor]) ||
				tearLine.ContainsValue(Clone.GetComponent<MeshFilter>().mesh.triangles[itor + 1]) ||
				tearLine.ContainsValue(Clone.GetComponent<MeshFilter>().mesh.triangles[itor + 2]))
			{
				if(itor%3 == 0)
				{
					newFaces[intorator] = Clone.GetComponent<MeshFilter>().mesh.triangles[itor];
					newFaces[intorator  + 1] = Clone.GetComponent<MeshFilter>().mesh.triangles[itor + 1];
					newFaces[intorator  + 2] = Clone.GetComponent<MeshFilter>().mesh.triangles[itor + 2];
					intorator +=3;
				}
				else if(itor%3 == 1)
				{
					newFaces[intorator] = Clone.GetComponent<MeshFilter>().mesh.triangles[itor - 1];
					newFaces[intorator  + 1] = Clone.GetComponent<MeshFilter>().mesh.triangles[itor];
					newFaces[intorator  + 2] = Clone.GetComponent<MeshFilter>().mesh.triangles[itor + 1];
					intorator +=3;
				}
				else if(itor%3 == 2)
				{
					newFaces[intorator] = Clone.GetComponent<MeshFilter>().mesh.triangles[itor - 2];
					newFaces[intorator  + 1] = Clone.GetComponent<MeshFilter>().mesh.triangles[itor - 1];
					newFaces[intorator  + 2] = Clone.GetComponent<MeshFilter>().mesh.triangles[itor];
					intorator +=3;
				}

			}
			
			//triangleCount;
			//if(triangleCount == 4) triangleCount = 1;
		}
		Clone.GetComponent<MeshFilter>().mesh.triangles = newFaces;
		Clone.GetComponent<MeshCollider>().sharedMesh = Clone.GetComponent<MeshFilter>().mesh;
		
		
		
		doneCalculatingCuttLine = false;
	}
	
	/// <summary>
	/// Sets the bounds of paper world during initialization for edge to edge cutting
	/// </summary>
	private void SetBoundsOfPaper()
	{
		//Initialize the max and min world values of mesh mertex coordinates
		MaxWorldWidth = -100;
	 	MinWorldWidth = 100;
		MaxWorldHeight = -100;
		MinWorldHeight = 100;
		
		//Loop through mesh and find/set the bounds
		int meshVertLength = mesh.vertices.Length;
		Vector3[] meshVerts = mesh.vertices;
		for(int itor = 0; itor < meshVertLength; itor++)
		{
			//should make one vector3 with gameobject.transform.transformpoint(meshverts[itor]) since we do it so often
			if(gameObject.transform.TransformPoint(meshVerts[itor]).x > MaxWorldWidth)
			{
				MaxWorldWidth = gameObject.transform.TransformPoint(meshVerts[itor]).x;
			}
			else if(gameObject.transform.TransformPoint(meshVerts[itor]).x < MinWorldWidth)
			{
				MinWorldWidth = gameObject.transform.TransformPoint(meshVerts[itor]).x;
			}
			
			if(gameObject.transform.TransformPoint(meshVerts[itor]).y > MaxWorldHeight)
			{
				MaxWorldHeight = gameObject.transform.TransformPoint(meshVerts[itor]).y;
			}
			else if(gameObject.transform.TransformPoint(meshVerts[itor]).y < MinWorldHeight)
			{
				MinWorldHeight = gameObject.transform.TransformPoint(meshVerts[itor]).y;
			}
		}
	}
	
	/// <summary>
	/// Creates the new paper world clone and hides the original mesh renderer
	/// This function also sets flags handling which object is being manipulated
	/// </summary>
	private void CreateNewPaperWorld()
	{
		
		//Assign the clone to be the new instance
		Clone.SetActive(true);
		Clone2.SetActive(true);
		
		//Clone = (GameObject) Instantiate(newPaper, this.transform.position, this.transform.rotation);
		Clone.GetComponent<TearPaper>().CloneObject = true;
		Clone.GetComponent<TearPaper>().NeedToCheckExcessDuplicates = true;
		//Clone.GetComponent<MeshRenderer>().enabled = false;
		
		//Set clone informaiton
		//Clone2 = (GameObject) Instantiate(newPaper, this.transform.position, this.transform.rotation);
		Clone2.GetComponent<TearPaper>().CloneObject = true;
		Clone2.GetComponent<TearPaper>().NeedToCheckExcessDuplicates = true;
		Clone2.GetComponent<MeshRenderer>().enabled = false;
		
		//Turn off the original object's meshRenderer to hide
		this.GetComponent<MeshRenderer>().enabled = false;
		this.GetComponent<MeshCollider>().enabled = false;
		

		
		//This flag makes sure that excess duplicated get properly disgarded
		NeedToCheckExcessDuplicates = true;
		
		//We only add to tear manager here iff we are a platform piece of paper
		if(PlatformPaper)
		{
			//Add clones to manager
			TearManager.TornPlatforms.Add (Clone);
			TearManager.TornPlatforms.Add (Clone2);
		}
		
	}
	//Create structure to store edge vertices soley
	private List<Vector3> edgeVerts;
	
	//Structure holding information creating edge triangles
	private List<int> edgeTriangles;
	
	/// <summary>
	/// Sets the edge relation ships.
	/// </summary>
	private void SetEdgeRelationShips()
	{
		if(edgeVerts == null) edgeVerts = new List<Vector3>();
		else edgeVerts.Clear();
		
		if(edgeTriangles == null) edgeTriangles = new List<int>();
		else edgeTriangles.Clear();
		
		List<bool> edgeOfObjectValues = edgeOfObject.Values.ToList();
		List<Vector3> edgeOfObjectKeys = edgeOfObject.Keys.ToList();
		//Loop through mesh and store edge vertices into list
		for(int itor = 0; itor < edgeOfObjectKeys.Count(); itor++)
		{
			if(edgeOfObjectValues[itor])//edgeOfObject.Values.ElementAt(itor))
			{
				edgeVerts.Add(edgeOfObjectKeys[itor]);//edgeOfObject.Keys.ElementAt(itor));
				
				//if((MinWidthBoundsCheck(edgeOfObject.Keys.ElementAt(itor)) || MaxWidthBoundsCheck(edgeOfObject.Keys.ElementAt(itor))) 
				//	&& HeightVerticeBoundsCheck(edgeOfObject.Keys.ElementAt(itor)))
				{
				//	Debug.LogError("EDGE VERT HIT");
				}
			}
		}
		//Debug.LogError("NUMBER OF EDGE VERTICES = " + edgeVerts.Count().ToString() + " with meshOffset = " + MESH_VERT_OFFSET.ToString());
		
		
		//Now, edgeVerts contains all edge vertices within the object
		HashSet<Vector3> organizedEdgeVertHash = new HashSet<Vector3>();
		List<Vector3> organizedEdgeVert = new List<Vector3>();
		organizedEdgeVert.Add(edgeVerts[0]);
		organizedEdgeVertHash.Add(edgeVerts[0]);
		int indexrr = 0;
		
		List<Vector3> potentialEdgeVertNeighbors;
		
		//these floats used to be in the for loop
		float xDist1;
		float yDist1;
		float xDist2;
		float yDist2;
		
		float ydist;
		float xdist;
		
		int edgeVertCount = edgeVerts.Count();
		//Now we organize edgeVerts based off proximity
		for(int itor = 0; itor < edgeVertCount; itor++)
			//while(organizedEdgeVert.Count() < edgeVerts.Count())
		{
			potentialEdgeVertNeighbors = new List<Vector3>();
			
			for(int jtor = 0; jtor < edgeVertCount; jtor++)
			{
				xDist1 = edgeVerts[jtor].x;
				yDist1 = edgeVerts[jtor].y;
				xDist2 = organizedEdgeVert[indexrr].x;
				yDist2 = organizedEdgeVert[indexrr].y;
				
				//if(xDist1 < 0) xDist1 *= -1;
				//if(yDist1 < 0) yDist1 *= -1;
				//if(xDist2 < 0) xDist2 *= -1;
				//if(yDist2 < 0) yDist2 *= -1;
				
				ydist = yDist2 - yDist1;
				xdist = xDist2 - xDist1;
				
				if(ydist < 0) ydist *= -1;
				if(xdist < 0) xdist *= -1;
				
				//Debug.LogError("CurrenDistnace x  =  " + xdist.ToString());
				//Debug.LogError("CurrenDistnace y  =  " + ydist.ToString());
				
				if(!organizedEdgeVertHash.Contains(edgeVerts[jtor]) &&
					xdist <= MESH_VERT_OFFSET &&
					ydist <= MESH_VERT_OFFSET
					)
				{
					potentialEdgeVertNeighbors.Add(edgeVerts[jtor]);
					//organizedEdgeVert.Add(edgeVerts[jtor]);
					//++indexrr;
					//break;
				}
			}
			//used to be declared in for loop
			float distanceXDir;
			float distanceYDir;
			//Now potentialEdgeVertNeighbors conatains all of the potential 
			//neighbors for the current vertice being examined
			
			int potentialEdgeVertNeighborCount = potentialEdgeVertNeighbors.Count();
			for(int ztor = 0; ztor < potentialEdgeVertNeighborCount; ztor++)
			{
				xDist1 = potentialEdgeVertNeighbors/*.ElementAt(*/[ztor].x;
				yDist1 = potentialEdgeVertNeighbors/*.ElementAt(*/[ztor].y;
				xDist2 = organizedEdgeVert[indexrr].x;
				yDist2 = organizedEdgeVert[indexrr].y;
				
				//if(xDist1 < 0) xDist1 *= -1;
				//if(yDist1 < 0) yDist1 *= -1;
				//if(xDist2 < 0) xDist2 *= -1;
				//if(yDist2 < 0) yDist2 *= -1;
				
				distanceXDir = xDist1 - xDist2;
				if(distanceXDir < 0 )distanceXDir *= -1;
				distanceYDir = yDist1 - yDist2;
				if(distanceYDir < 0 )distanceYDir *= -1;
				
				if((distanceXDir <= MESH_VERT_OFFSET && distanceYDir == 0) ||
					(distanceXDir  == 0 && distanceYDir <= MESH_VERT_OFFSET)
					)
				{
					//potentialEdgeVertNeighbors.Add(edgeVerts[jtor]);
					organizedEdgeVert.Add(potentialEdgeVertNeighbors/*.ElementAt(*/[ztor]);
					organizedEdgeVertHash.Add(potentialEdgeVertNeighbors[ztor]);
					++indexrr;
					break;
				}
			}
		}
		edgeVerts.Clear();
		//Debug.LogError("NUMBER OF EDGE VERTICES 2 =  " + organizedEdgeVert.Count().ToString());
		edgeVerts = organizedEdgeVert;
		
		//organizedEdgeVert.Clear();
		
		//for(int itor = 0; itor < edgeVerts.Count(); itor++)
		{
		//	Debug.LogError("Loading edgeverts with position = " + edgeVerts.ElementAt(itor).x.ToString() + ", " + edgeVerts.ElementAt(itor).y.ToString() + ", " + edgeVerts.ElementAt(itor).z.ToString());
		}
		
		/*
		//Loop through the existing triangles to create edgeTriangles
		for(int itor = 0; itor < this.GetComponent<MeshFilter>().mesh.triangles.Count(); itor += 3)
		{
			if(
				    (
					edgeVerts.Contains(this.GetComponent<MeshFilter>().mesh.vertices[this.GetComponent<MeshFilter>().mesh.triangles[itor]])
					&&
					edgeVerts.Contains(this.GetComponent<MeshFilter>().mesh.vertices[this.GetComponent<MeshFilter>().mesh.triangles[itor + 1]])
					)
					
					||
					(
					edgeVerts.Contains(this.GetComponent<MeshFilter>().mesh.vertices[this.GetComponent<MeshFilter>().mesh.triangles[itor + 1]])
					&&
					edgeVerts.Contains(this.GetComponent<MeshFilter>().mesh.vertices[this.GetComponent<MeshFilter>().mesh.triangles[itor + 2]])
					)
					||
					(
					edgeVerts.Contains(this.GetComponent<MeshFilter>().mesh.vertices[this.GetComponent<MeshFilter>().mesh.triangles[itor]])
					&&
					edgeVerts.Contains(this.GetComponent<MeshFilter>().mesh.vertices[this.GetComponent<MeshFilter>().mesh.triangles[itor + 2]])
					)
				)	
			{
				edgeTriangles.Add(this.GetComponent<MeshFilter>().mesh.triangles[itor]);
				edgeTriangles.Add(this.GetComponent<MeshFilter>().mesh.triangles[itor + 1]);
				edgeTriangles.Add(this.GetComponent<MeshFilter>().mesh.triangles[itor + 2]);
			}
		}
		
		//Now, we must organize edgeVerts into the shape the edge verts actually
		//create. This enables us the properly iterate through the edges
		
		//Create structure to store organized edge vertices soley
		List<Vector3> organizedEdgeVerts = new List<Vector3>();
		organizedEdgeVerts.Add(edgeVerts[0]);
		int indexr = 0;
		//Loop through the edge vertices
		for(int gtor = 0; gtor < edgeVerts.Count(); gtor++)
		{
			//if(!organizedEdgeVerts.Contains(edgeVerts[gtor + 1]))
			{
				//Loop through the edge vertices starting from gtor's neighbor
				for(int ztor = 1; ztor < edgeVerts.Count(); ztor++)
				{
					if(VerticesShareTriangleOnEdge(organizedEdgeVerts.ElementAt(indexr), edgeVerts[ztor]) && !organizedEdgeVerts.Contains(edgeVerts[ztor]))
					{
						organizedEdgeVerts.Add(edgeVerts[ztor]);
						++indexr;
						break;
					}
				}
			}
		}
		edgeVerts = organizedEdgeVerts;
		*/
	}
	
	/// <summary>
	/// This function is used to see if the player is the touching the paper object.
	/// We can check this by using the Max and Min Height and Width established within the Start method
	/// *************CURRENTLY NOT IN USE********************
	/// </summary>
	/*
	private bool PlayerTouchingPaper()
	{
		
		bool returnVal = false;
		
		//Loop through mesh vertices
		for (var itor = 0; itor < newMesh.Length; itor++)
		{
			//Find the screen poition of the vertex
			screenPoint = Camera.main.WorldToScreenPoint(Clone.GetComponent<MeshFilter>().mesh.vertices[itor]);
			
			//Get the current position of the mouse in the scene (world) with the same z-depth as the vertex being examined
    		Vector3 curPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
			
			if(curPosition.x <= WIDTH_MAX && curPosition.x >= WIDTH_MIN &&
				curPosition.y <= HEIGHT_MAX && curPosition.y >= HEIGHT_MIN)
			{
				returnVal = true;
				//cuttInProgress = true;
			}
			
			
		}
		return returnVal;
		
	}
	
	/// <summary>
	/// This function is used to see if the player is NOT the touching the paper object.
	/// We can check this by using the Max and Min Height and Width established within the Start method
	/// *************CURRENTLY NOT IN USE********************
	/// </summary>
	private bool PlayerTouchingOffPaper()
	{
		
		bool returnVal = false;
		
		//Loop through mesh vertices
		for (var itor = 0; itor < newMesh.Length; itor++)
		{
			//Find the screen poition of the vertex
			screenPoint = Camera.main.WorldToScreenPoint(Clone.GetComponent<MeshFilter>().mesh.vertices[itor]);
			
			//Get the current position of the mouse in the scene (world) with the same z-depth as the vertex being examined
    		Vector3 curPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
			
			if(curPosition.x > WIDTH_MAX || curPosition.x < WIDTH_MIN ||
				curPosition.y > HEIGHT_MAX || curPosition.y < HEIGHT_MIN)
			{
				returnVal = true;
			}
			
			
		}
		//Debug.Log("Touching Off Paper = " + returnVal.ToString());
		return returnVal;
		
	}*/
	
	/// <summary>
	/// Players the touching dead space is used to determine whether or not the player is touching off the level (paper Object)
	/// </summary>
	/// <returns>
	/// The touching dead space is true if the playing is touching off screen
	/// </returns>
	private bool PlayerTouchingDeadSpace()
	{
		//bool returnVal = false;
		
		/*
		//Create raytrace object
		RaycastHit hit = new RaycastHit();
        Ray ray = Camera.main.ScreenPointToRay(TearManager.GetInputPos());
		
		//Check raycast
		if(Physics.Raycast(ray, out hit, 1000.0f))
		{
			//If the player collided with the deadspace object, then they are touching the dead space
			if(hit.transform.tag == "DeadSpace")
    		{
				returnVal = true;
			}
		}
		*/
		
		try
		{
			Vector3 worldpos = Camera.main.ScreenToWorldPoint(new Vector3(TearManager.GetInputPos().x, TearManager.GetInputPos().y, 
																			Camera.main.WorldToScreenPoint(gameObject.collider.transform.position).z));
			if(worldpos.x >= MaxWorldWidth ||
				worldpos.y >= MaxWorldHeight ||
				worldpos.x <= MinWorldWidth ||
				worldpos.y <= MinWorldHeight)
			{
				return true;
			}
			
			return false;
		}
		catch
		{
			return false;
		}
	}
	
	/// <summary>
	/// Removes the problematic sub regions of tear, getting rid of any points in a tear where
	/// the decision could become ambiguos
	/// This function is mainly used for tears along the edge which make a sharp 'v' shape, which trhoughs of my logic to determine
	/// which vertice belongs to which new cutt piece, therefore, I get rid of any of these problematic regions
	/// </summary>
	private void RemoveProblematicSubRegionsOfTear()
	{
		//Here we check if the tear was started on the right edge
		
		//if(tearLine.Keys.ElementAt(0).x == WIDTH_MIN)
		if(MinWidthBoundsCheck(tearLine.Keys.ElementAt(0)))
		{
			//Checking for ambiguos decision. Upward V directly connected to edge at begining
			// of tear line
			if(tearLine.Keys.ElementAt(0).y == 
				tearLine.Keys.ElementAt(2).y 
				&&
				tearLine.Keys.ElementAt(1).x == 
				tearLine.Keys.ElementAt(2).x 
				&&
				tearLine.Keys.ElementAt(1).y > 
				tearLine.Keys.ElementAt(2).y 
				)
			{
				Debug.LogError("*********************Upward V detected*********************");
			}
			//Checking for ambiguos decision. Downward V directly connected to edge at begining
			// of tear line
			else if(tearLine.Keys.ElementAt(0).y == 
				tearLine.Keys.ElementAt(2).y 
				&&
				tearLine.Keys.ElementAt(1).x == 
				tearLine.Keys.ElementAt(2).x 
				&&
				tearLine.Keys.ElementAt(1).y < 
				tearLine.Keys.ElementAt(2).y 
				)
			{
				Debug.LogError("*********************Downward V detected*********************");
			}
				
		}
		//Here we check if the tear ended on the right edge
		
		//if(tearLine.Keys.ElementAt(tearLine.Count - 1).x == WIDTH_MIN)
		if(MinWidthBoundsCheck(tearLine.Keys.ElementAt(tearLine.Count - 1)))
		{
			//Checking for ambiguos decision. Upward V directly connected to edge at begining
			// of tear line
			if(tearLine.Keys.ElementAt(tearLine.Count - 1).y == 
				tearLine.Keys.ElementAt(tearLine.Count - 3).y 
				&&
				tearLine.Keys.ElementAt(tearLine.Count - 2).x == 
				tearLine.Keys.ElementAt(tearLine.Count - 3).x 
				&&
				tearLine.Keys.ElementAt(tearLine.Count - 2).y > 
				tearLine.Keys.ElementAt(tearLine.Count - 3).y 
				)
			{
				Debug.LogError("*********************Upward V detected*********************");
			}
			//Checking for ambiguos decision. Downward V directly connected to edge at begining
			// of tear line
			else if(tearLine.Keys.ElementAt(tearLine.Count - 1).y == 
				tearLine.Keys.ElementAt(tearLine.Count - 3).y 
				&&
				tearLine.Keys.ElementAt(tearLine.Count - 2).x == 
				tearLine.Keys.ElementAt(tearLine.Count - 3).x 
				&&
				tearLine.Keys.ElementAt(tearLine.Count - 2).y < 
				tearLine.Keys.ElementAt(tearLine.Count - 3).y 
				)
			{
				Debug.LogError("*********************Downward V detected*********************");
			}
		}
	}
	
	/// <summary>
	/// Forces the edge verts to be torn from starting to ending user input tearline
	/// </summary>
	private void ForceEdgeVertsToBeTorn()
	{
		//create flag to swap between torn and non torn edge vertices
		bool addingToTorn = true;
		
		//create structure to store the actual vertices to be added to tearLine
		List<Vector3> newTornEdgeVerts = new List<Vector3>();
		
		Debug.LogError("********************@@@@@@@@@@@@@@@@*************testing number of edge vertices = " + edgeVerts.Count().ToString());
		
		//Now we loop through each of these edges, switching whehter
		//or not torn when an hitting a user defined torn edge
		for(int jtor = 0; jtor < edgeVerts.Count(); jtor++)
		{
			bool test = true;
			
			if(jtor - 1 > 0 && tearLine.ContainsKey(edgeVerts.ElementAt(jtor - 1)))
			{
				test = false;
			}
			//Check is the current edge vertice is part of the user's tearLine, if so, 
			//we swap which edges are being torn vs non torn
			if(tearLine.ContainsKey(edgeVerts.ElementAt(jtor)) && test)
			{
				Debug.LogError("Just Hit tear line along edge, rotating which island we are force tearline along edge");
				//Rotate flag
				if(addingToTorn) addingToTorn = false;
				else addingToTorn = true;
			}
			
			//If we are adding to the torn piece, then add to newTornEdgeVerts
			//so that we can properly add to tearLine next
			if(addingToTorn && !tearLine.ContainsKey(edgeVerts.ElementAt(jtor)))
			{
				newTornEdgeVerts.Add(edgeVerts.ElementAt(jtor));
			}
		}
		//Now, newTornEdgeVerts needs to be added to tearLine
		
		//create tearLine time for new tear positions
		float newEdgeTearLineTime;// = tearLine.Values.ElementAt(tearLine.Keys.Count() - 1);
		bool reverseFlag = false;
		if(tearLineTime[tearLine.Keys.ElementAt(tearLine.Keys.Count() - 1)] > tearLineTime[tearLine.Keys.ElementAt(0)])
		{
			newEdgeTearLineTime = tearLineTime[tearLine.Keys.ElementAt(tearLine.Keys.Count() - 1)] + 1;
			reverseFlag = false;
		}
		else
		{
			newEdgeTearLineTime = tearLineTime[tearLine.Keys.ElementAt(tearLine.Keys.Count() - 1)] - 1;
			reverseFlag = true;
		}
		
		//Loop through newTornEdgeVerts, adding to tearLine, and proper timing storage
		for(int ktor = 0; ktor < newTornEdgeVerts.Count(); ktor++)
		{
			Debug.LogError("adding torn vert along edge #" + ktor.ToString() + " new vertice = " + newTornEdgeVerts.ElementAt(ktor).ToString());
			//Add new vertice to tearLine
			//if(tearLine.ContainsKey(newTornEdgeVerts.ElementAt(ktor))) tearLine.Remove(newTornEdgeVerts.ElementAt(ktor));
				
			tearLine.Add(newTornEdgeVerts.ElementAt(ktor), (int)newEdgeTearLineTime);
			//edgeVertsForcedTorn.Add(newTornEdgeVerts.ElementAt(ktor));
			
			if(tearLinePositionTime.ContainsKey(newEdgeTearLineTime)) tearLinePositionTime.Remove(newEdgeTearLineTime);
			if(tearLineTime.ContainsKey(newTornEdgeVerts.ElementAt(ktor))) tearLineTime.Remove(newTornEdgeVerts.ElementAt(ktor));
			
			//Now add the new tear vertice information and it's associated torn time
			tearLinePositionTime.Add (newEdgeTearLineTime, newTornEdgeVerts.ElementAt(ktor));
			tearLineTime.Add (newTornEdgeVerts.ElementAt(ktor), newEdgeTearLineTime);
			
			//Increment the tear Timer
			if(reverseFlag)
			{
				--newEdgeTearLineTime;
			}
			else
			{
				++newEdgeTearLineTime;
			}
		}
		newTornEdgeVerts.Clear();
		//DrawTearLineONLY = true;
	}
	
	/// <summary>
	/// Verticeses the share triangle.
	/// </summary>
	/// <returns>
	/// Returns true iff the two vertices share a traingle on mesh
	/// </returns>
	private bool VerticesShareTriangleOnEdge(Vector3 vert1, Vector3 vert2)
	{
		//Loop through this object's mesh
		for(int itor = 0; itor < edgeTriangles.Count(); itor += 3)
		{
			//If the two vertices passed share a triangle, we return true
			/*
			if(
				(this.GetComponent<MeshFilter>().mesh.vertices[this.GetComponent<MeshFilter>().mesh.triangles[itor]] == vert1 ||
				this.GetComponent<MeshFilter>().mesh.vertices[this.GetComponent<MeshFilter>().mesh.triangles[itor]] == vert2) 
				&&
				(this.GetComponent<MeshFilter>().mesh.vertices[this.GetComponent<MeshFilter>().mesh.triangles[itor + 1]] == vert1 ||
				this.GetComponent<MeshFilter>().mesh.vertices[this.GetComponent<MeshFilter>().mesh.triangles[itor + 1]] == vert2) 
				&&
				(this.GetComponent<MeshFilter>().mesh.vertices[this.GetComponent<MeshFilter>().mesh.triangles[itor + 2]] == vert1 ||
				this.GetComponent<MeshFilter>().mesh.vertices[this.GetComponent<MeshFilter>().mesh.triangles[itor + 3]] == vert2)
				)
				*/
				
			if(
				(thisMeshFilter.mesh.vertices[edgeTriangles[itor]] == vert1 ||
				thisMeshFilter.mesh.vertices[edgeTriangles[itor + 1]] == vert1 ||
				thisMeshFilter.mesh.vertices[edgeTriangles[itor + 2]] == vert1 ) 
				&&
				(thisMeshFilter.mesh.vertices[edgeTriangles[itor]] == vert2 ||
				thisMeshFilter.mesh.vertices[edgeTriangles[itor + 1]] == vert2 ||
				thisMeshFilter.mesh.vertices[edgeTriangles[itor + 2]] == vert2)
				)	
			{
				return true;
			}
		}
		
		//return false if the above was not triggered
		return false;
	}
	
	/// <summary>
	/// When a cutt occurs along the right edge of object, we need to determine which new cutt
	/// piece we are adding to properly when starting. This function is used to force vertices to
	/// become torn in order for the decision to be correct upon entering each row
	/// </summary>
	private void ForceHorizontalCuttFirstRowVert()
	{
		//Check if we even need to perform additional tearline vertice logic
		if(NeedToForceRowVertsTorn())
		{
			//This is used to flag when the first tearLine vertice need modification
			bool haveAddedToTear = false;
			
			//Keep track of how many vertices are added at beggining
			int numOfVertsAdded = 0;
			
			//Here, we check if we need to add more vertices to connect tearline with a 
			//starting vertice in the begining of the tearline mesh row
			if(!MinWidthBoundsCheck(tearLine.Keys.ElementAt(0)) 
				&& !MaxWidthBoundsCheck(tearLine.Keys.ElementAt(0))
				//&& !HeightVerticeBoundsCheck(tearLine.Keys.ElementAt(0))
				)
			{
				//Debug.LogError("*************NeedToForceRowVertsTorn***************** poop 1");
				
				//store index into mesh grid of first tear vertice in row
				int oldStartVertIndex = -1;
				
				//loop through currrent row and find which vertices need to be added
				for(int itor = 0; itor < paperGrid[tearLine.Keys.ElementAt(0).y].Count(); itor++)
				{
					//Check is the current vertice in row is the first torn vertice
					if(paperGrid[tearLine.Keys.ElementAt(0).y].ElementAt(itor) == tearLine.Keys.ElementAt(0))
					{
						//Store the old starting vertice index
						oldStartVertIndex = itor - 1;
						
						//Flag that we have added to beggining of tearline
						haveAddedToTear = true;
						
						//Creat new time value for new tear vertices
						float newTearTime = -10;
						
						bool timeInverted = false;
						
						//Debug.Log("CloneObject = " + CloneObject.ToString());
						//Debug.LogError("Currently trying to force tear at " + tearLine.Keys.ElementAt(tearLine.Keys.Count() - 1).ToString() + " with time = " + tearLineTime[tearLine.Keys.ElementAt(tearLine.Keys.Count() - 1)].ToString());
						try
						{
							if(!CloneObject &&
								tearLine != null &&
								tearLine.Keys.Count() - 1 > 0 &&
								tearLineTime.ContainsKey(tearLine.Keys.ElementAt(tearLine.Keys.Count() - 1)) &&
								tearLineTime[tearLine.Keys.ElementAt(tearLine.Keys.Count() - 1)] > tearLineTime[tearLine.Keys.ElementAt(0)])
							{
								newTearTime = tearLineTime[tearLine.Keys.ElementAt(0)] - 1;
								timeInverted = false;
							}
							else
							{
								newTearTime = tearLineTime[tearLine.Keys.ElementAt(0)] + 1;
								timeInverted = true;
							}
						}
						catch (KeyNotFoundException ex)
						{
							//Debug.Log("************************************KeyNotFoundException***************************************");
							//if(!TearManager.BadTear)
								ForceStopTear("precise");
				hitPlayerDuringTear = 0;
							return;
						}
						
						Vector3 originalVal = new Vector3(10000,10000,10000);
						Vector3 previouslyAddedVert = originalVal;
						
						//Now we keep adding tear vertices to the current row, until we reach to first vertice in row
						while(oldStartVertIndex >= 0)
						{
							//Debug.LogError("*************NeedToForceRowVertsTorn***************** poop 1 + " + oldStartVertIndex.ToString());
							
							//Create vector to represent the new torn vertice position
							Vector3 newTornVert = paperGrid[tearLine.Keys.ElementAt(0).y].ElementAt(oldStartVertIndex);
							
							if(previouslyAddedVert != originalVal)
							{
								float xdist = previouslyAddedVert.x - newTornVert.x;
								if(xdist < 0) xdist *= -1;
								
								if(xdist > MESH_VERT_OFFSET)
								{
									//Debug.LogError("******OH SHIT*0************************************************************************************************************************************");
									break;
								}
							}
							
							//makeSure we are only forceing along an edge which is a local heght min or max
							if(!HeightVerticeBoundsCheckSLikeShape(newTornVert))
							{
								//Debug.LogError("******OH SHIT****&&&&&&&&&&&&&&&&&&&2222222222&&&&&&&&*");
								break;
							}
							
							//Make sure the vertex is not already in tearline
							if(!tearLine.ContainsKey(newTornVert))
							{
								//Add new vertice to tearLine
								tearLine.Add(newTornVert, (int)newTearTime);
								
								//Keep track of hoe many vertices are added to the beggining
								++numOfVertsAdded;
								
								if(tearLineTime.ContainsKey(newTornVert))
								{
									tearLineTime.Remove(newTornVert);
								}
								
								if(tearLinePositionTime.ContainsKey(newTearTime))
								{
									tearLinePositionTime.Remove(newTearTime);
								}
								
								//Debug.LogError("*************NeedToForceRowVertsTorn***************** poop 1. new tearbvert = " + newTornVert.ToString() + ", with time = " + newTearTime.ToString());
								
								//Now add the new tear vertice information and it's associated torn time
								tearLinePositionTime.Add (newTearTime, newTornVert);
								tearLineTime.Add(newTornVert, newTearTime);
								
								//Decrement timeAnd Index for next iteration
								if(timeInverted)
								{
									++newTearTime;
								}
								else
								{
									--newTearTime;
								}
								//--oldStartVertIndex;
							}
							--oldStartVertIndex;
							previouslyAddedVert = newTornVert;
						}
						
						//Now we are done
						break;
					}
				}
			}
			
			int indexIntoEndOfTear = 1;
				
			if(haveAddedToTear)
			{
				indexIntoEndOfTear += numOfVertsAdded;
			}
			
			int indexor = tearLine.Keys.Count() - indexIntoEndOfTear;
			
			//Here, we check if we need to add more vertices to connect tearline with a 
			//ending vertice in the begining of the tearline mesh row
			if(!MinWidthBoundsCheck(tearLine.Keys.ElementAt(indexor)) 
				&& !MaxWidthBoundsCheck(tearLine.Keys.ElementAt(indexor))
				//&& !HeightVerticeBoundsCheck(tearLine.Keys.ElementAt(indexor))
				)
			{
				//Debug.LogError("*************NeedToForceRowVertsTorn***************** poop 2");
				
				//store index into mesh grid of first tear vertice in row
				int oldStartVertIndex = -1;
				
				//loop through currrent row and find which vertices need to be added
				for(int itor = 0; itor < paperGrid[tearLine.Keys.ElementAt(indexor).y].Count(); itor++)
				{
					//Check is the current vertice in row is the first torn vertice
					if(paperGrid[tearLine.Keys.ElementAt(indexor).y].ElementAt(itor) == tearLine.Keys.ElementAt(indexor))
					{
						//Store the old starting vertice index
						oldStartVertIndex = itor - 1;
						
						//Creat new time value for new tear vertices
						float newTearTime; 
						bool timeInverted = false;
						
						//Debug.LogError("Currently trying to force tear at " + tearLine.Keys.ElementAt(indexor).ToString() + " with time = " + tearLineTime[tearLine.Keys.ElementAt(indexor)].ToString());
						//Debug.Log("**CloneObject = " + CloneObject.ToString());
						if(!CloneObject &&
							tearLine != null &&
							tearLine.Keys.Count() > indexor && 
							tearLineTime.ContainsKey(tearLine.Keys.ElementAt(indexor)) && 
							tearLineTime.ContainsKey(tearLine.Keys.ElementAt(0)) && 
							tearLineTime[tearLine.Keys.ElementAt(indexor)] > tearLineTime[tearLine.Keys.ElementAt(0)])
						{
							newTearTime = tearLineTime[tearLine.Keys.ElementAt(indexor)] + 1;
							timeInverted = false;
						}
						else
						{
							newTearTime = tearLineTime[tearLine.Keys.ElementAt(indexor)] - 1;
							timeInverted = true;
						}
						
						Vector3 originalVal = new Vector3(10000,10000,10000);
						Vector3 previouslyAddedVert = originalVal;
						
						
						
						//Now we keep adding tear vertices to the current row, until we reach to first vertice in row
						while(oldStartVertIndex >= 0)
						{
							//Debug.LogError("* " + (tearLine.Keys.ElementAt(indexor).y).ToString() + " ************NeedToForceRowVertsTorn***************** poop 2 + " + oldStartVertIndex.ToString());
							
							//Create vector to represent the new torn vertice position
							Vector3 newTornVert = paperGrid[tearLine.Keys.ElementAt(indexor).y].ElementAt(oldStartVertIndex);//new Vector3(tearLine.Keys.ElementAt(tearLine.Keys.Count() - indexIntoEndOfTear).x - MESH_VERT_OFFSET, tearLine.Keys.ElementAt(tearLine.Keys.Count() - indexIntoEndOfTear).y, tearLine.Keys.ElementAt(tearLine.Keys.Count() - indexIntoEndOfTear).z);//
							 
							if(previouslyAddedVert != originalVal)
							{
								float xdist = previouslyAddedVert.x - newTornVert.x;
								if(xdist < 0) xdist *= -1;
								
								if(xdist > MESH_VERT_OFFSET)
								{
									//Debug.LogError("******OH SHIT****************************************************************************************************************** " + newTornVert.ToString());
									break;
								}
							}
							
							//makeSure we are only forceing along an edge which is a local heght min or max
							if(!HeightVerticeBoundsCheckSLikeShape(newTornVert))
							{
								//Debug.LogError("******OH SHIT****&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&*");
								break;
							}
							//Make sure the vertex is not already in tearline
						//	if(!tearLine.ContainsKey(newTornVert) && edgeOfObject[newTornVert])
							{
								if(tearLine.ContainsKey(newTornVert))tearLine.Remove(newTornVert);
								//Add new vertice to tearLine
								tearLine.Add(newTornVert, (int)newTearTime);
								
								//Debug.LogError("******OH SHIT****&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&*");
								
								if(tearLineTime.ContainsKey(newTornVert))
								{
									tearLineTime.Remove(newTornVert);
								}
								
								if(tearLinePositionTime.ContainsKey(newTearTime))
								{
									tearLinePositionTime.Remove(newTearTime);
								}
								//Debug.LogError("*************NeedToForceRowVertsTorn***************** poop 2. new tearbvert = " + newTornVert.ToString() + ", with time = " + newTearTime.ToString());
								
								//Now add the new tear vertice information and it's associated torn time
								tearLinePositionTime.Add (newTearTime, newTornVert);
								tearLineTime.Add(newTornVert, newTearTime);
								
								//Decrement timeAnd Index for next iteration
								if(timeInverted)
								{
									--newTearTime;
								}
								else
								{
									++newTearTime;
								}
								//--oldStartVertIndex;
							}
							--oldStartVertIndex;
							
							previouslyAddedVert = newTornVert;
						}
						
						//Now we are done
						break;
					}
				}
			}
		}
	}
	
	/// <summary>
	/// This function returns true if the tearline needs addition vertices
	/// along the tear row for proper object splitting decision logic
	/// </summary>
	private bool NeedToForceRowVertsTorn()
	{
		//Create value to return
		bool returnVal = false;
		
		//We check if the first and last tearVertices meet all reqiurements for decision at starting edge
		//to be made properly
		if((
				MinWidthBoundsCheck(tearLine.Keys.ElementAt(0)) 
				|| MaxWidthBoundsCheck(tearLine.Keys.ElementAt(0)) 
				//|| HeightVerticeBoundsCheck(tearLine.Keys.ElementAt(0)
				//)
			)
			&&
			(
				MinWidthBoundsCheck(tearLine.Keys.ElementAt(tearLine.Keys.Count() - 1)) 
				|| MaxWidthBoundsCheck(tearLine.Keys.ElementAt(tearLine.Keys.Count() - 1)) 
				//|| HeightVerticeBoundsCheck(tearLine.Keys.ElementAt(tearLine.Keys.Count() - 1))
				)
			)
		{
			returnVal = false;
		}
		else
		{
			//Else, here we know that either the starting or ending vertive in tearLine
			//does not fit the needed requirements for tearing to work properly
			//I.E. We need additional vertices to be torn in the ending&&||starting rows
			//for the initial decision in tearing to work properly
			returnVal = true;
		}
		
		if(returnVal)
		{
			//Debug.LogError("NeedToForceRowVertsTorn  == true");
		}
		else
		{
			//Debug.LogError("NeedToForceRowVertsTorn  == false");
		}
		//return the appropriate flag
		return returnVal;
	}
	
	/// <summary>
	/// Forces the horizontal cutt along edge, this is used for removing areas in the tear line which
	/// break the decision logic determining which vertice belongs to which new cutt piece
	/// </summary>
	private void ForceHorizontalCuttAlongEdge()
	{
		//Debug.LogError("*************************TESTING EDGE CASES***************************");
		float newTearLineTime = -1;
		
		//Here we check if the tear was started on the right edge
		if(MinWidthBoundsCheck(tearLine.Keys.ElementAt(0)))// tearLine.Keys.ElementAt(0).x == WIDTH_MIN)
		{
			if(tearLine.Keys.ElementAt(0).y != tearLine.Keys.ElementAt(1).y)
			{
				if(tearLine.Keys.ElementAt(0).y < tearLine.Keys.ElementAt(1).y)
				{
					Debug.LogError("*********************Edge Vertice Check 1.0*********************");
					//Record time of old tear vertice
					newTearLineTime = tearLineTime[tearLine.Keys.ElementAt(0)];
					
					//Remove the old tear vertice from storage for further logic testing
					tearLinePositionTime.Remove (tearLineTime[tearLine.Keys.ElementAt(0)]);	
					tearLineTime.Remove (tearLine.Keys.ElementAt(0));
					
					//Change current edge tear vertice in tearLine
					Vector3 newposition = new Vector3(tearLine.Keys.ElementAt(0).x, tearLine.Keys.ElementAt(0).y + MESH_VERT_OFFSET, tearLine.Keys.ElementAt(0).z);
					tearLine.Remove(tearLine.Keys.ElementAt(0));
					if(tearLine.ContainsKey(newposition))tearLine.Remove(newposition);
					tearLine.Add(newposition, (int)newTearLineTime);
					
					if(tearLinePositionTime.ContainsKey(newTearLineTime)) tearLinePositionTime.Remove(newTearLineTime);
					if(tearLineTime.ContainsKey(tearLine.Keys.ElementAt(0))) tearLineTime.Remove(tearLine.Keys.ElementAt(0));
					
					//Now add the new tear vertice information and it's associated torn time
					tearLinePositionTime.Add (newTearLineTime, tearLine.Keys.ElementAt(0));
					tearLineTime.Add(tearLine.Keys.ElementAt(0), newTearLineTime);
				}
				else if(tearLine.Keys.ElementAt(0).y > tearLine.Keys.ElementAt(1).y && tearLineTime.ContainsKey(tearLine.Keys.ElementAt(0)))
				{
					Debug.LogError("*********************Edge Vertice Check 1.1*********************");
					//Record time of old tear vertice
					newTearLineTime = tearLineTime[tearLine.Keys.ElementAt(0)];
					
					//Remove the old tear vertice from storage for further logic testing
					tearLinePositionTime.Remove (tearLineTime[tearLine.Keys.ElementAt(0)]);	
					tearLineTime.Remove (tearLine.Keys.ElementAt(0));
					
					//Change current edge tear vertice in tearLine
					Vector3 newposition = new Vector3(tearLine.Keys.ElementAt(0).x, tearLine.Keys.ElementAt(0).y - MESH_VERT_OFFSET, tearLine.Keys.ElementAt(0).z);
					tearLine.Remove(tearLine.Keys.ElementAt(0));
					if(tearLine.ContainsKey(newposition))tearLine.Remove(newposition);
					tearLine.Add(newposition, (int)newTearLineTime);
					
					if(tearLinePositionTime.ContainsKey(newTearLineTime)) tearLinePositionTime.Remove(newTearLineTime);
					if(tearLineTime.ContainsKey(tearLine.Keys.ElementAt(0))) tearLineTime.Remove(tearLine.Keys.ElementAt(0));
					
					//Now add the new tear vertice information and it's associated torn time
					tearLinePositionTime.Add (newTearLineTime, tearLine.Keys.ElementAt(0));
					tearLineTime.Add(tearLine.Keys.ElementAt(0), newTearLineTime);
				}
			}
				
		}
		//Here we check if the tear ended on the right edge
		
		//if(tearLine.Keys.ElementAt(tearLine.Count - 1).x == WIDTH_MIN)
		if(MinWidthBoundsCheck(tearLine.Keys.ElementAt(tearLine.Count - 1)))
		{
			if(tearLine.Keys.ElementAt(tearLine.Count - 1).y != tearLine.Keys.ElementAt(tearLine.Count - 2).y)
			{
				if(tearLine.Keys.ElementAt(tearLine.Count - 1).y < tearLine.Keys.ElementAt(tearLine.Count - 2).y)
				{
					//Debug.LogError("*********************Edge Vertice Check 1.2*********************");
					//Record time of old tear vertice
					newTearLineTime = tearLineTime[tearLine.Keys.ElementAt(tearLine.Count - 1)];
					
					//Remove the old tear vertice from storage for further logic testing
					tearLinePositionTime.Remove (tearLineTime[tearLine.Keys.ElementAt(tearLine.Count - 1)]);	
					tearLineTime.Remove (tearLine.Keys.ElementAt(tearLine.Count - 1));
					
					//Change current edge tear vertice in tearLine
					Vector3 newposition = new Vector3(tearLine.Keys.ElementAt(tearLine.Count - 1).x, tearLine.Keys.ElementAt(tearLine.Count - 1).y + MESH_VERT_OFFSET, tearLine.Keys.ElementAt(tearLine.Count - 1).z);
					tearLine.Remove(tearLine.Keys.ElementAt(tearLine.Count - 1));
					if(tearLine.ContainsKey(newposition))tearLine.Remove(newposition);
					tearLine.Add(newposition, (int)newTearLineTime);
					
					if(tearLinePositionTime.ContainsKey(newTearLineTime)) tearLinePositionTime.Remove(newTearLineTime);
					if(tearLineTime.ContainsKey(tearLine.Keys.ElementAt(tearLine.Count - 1))) tearLineTime.Remove(tearLine.Keys.ElementAt(tearLine.Count - 1));
					
					//Now add the new tear vertice information and it's associated torn time
					tearLinePositionTime.Add (newTearLineTime, tearLine.Keys.ElementAt(tearLine.Count - 1));
					tearLineTime.Add(tearLine.Keys.ElementAt(tearLine.Count - 1), newTearLineTime);
				}
				else if(tearLine.Keys.ElementAt(tearLine.Count - 1).y > tearLine.Keys.ElementAt(tearLine.Count - 2).y)
				{
					Debug.LogError("*********************Edge Vertice Check 1.3*********************");
					//Record time of old tear vertice
					newTearLineTime = tearLineTime[tearLine.Keys.ElementAt(tearLine.Count - 1)];
					
					//Remove the old tear vertice from storage for further logic testing
					tearLinePositionTime.Remove (tearLineTime[tearLine.Keys.ElementAt(tearLine.Count - 1)]);	
					tearLineTime.Remove (tearLine.Keys.ElementAt(tearLine.Count - 1));
					
					//Change current edge tear vertice in tearLine
					Vector3 newposition = new Vector3(tearLine.Keys.ElementAt(tearLine.Count - 1).x, tearLine.Keys.ElementAt(tearLine.Count - 1).y - MESH_VERT_OFFSET, tearLine.Keys.ElementAt(tearLine.Count - 1).z);
					tearLine.Remove(tearLine.Keys.ElementAt(tearLine.Count - 1));
					if(tearLine.ContainsKey(newposition))tearLine.Remove(newposition);
					tearLine.Add(newposition, (int)newTearLineTime);
					
					if(tearLinePositionTime.ContainsKey(newTearLineTime)) tearLinePositionTime.Remove(newTearLineTime);
					if(tearLineTime.ContainsKey(tearLine.Keys.ElementAt(tearLine.Count - 1))) tearLineTime.Remove(tearLine.Keys.ElementAt(tearLine.Count - 1));
					
					//Now add the new tear vertice information and it's associated torn time
					tearLinePositionTime.Add (newTearLineTime, tearLine.Keys.ElementAt(tearLine.Count - 1));
					tearLineTime.Add(tearLine.Keys.ElementAt(tearLine.Count - 1), newTearLineTime);
				}
			}
		}
	}
	
	/// <summary>
	/// Deletes the excess edge vertices from tear list to prevent wierd anomolies
	/// </summary>
	private void DeleteExcessEdgeVertices()
	{
		bool haveFoundStartingEdgeVert = false;
		bool haveFoundEndingEdgeVert = false;
		Dictionary<Vector3, int> newTearLine = new Dictionary<Vector3, int>();
		
		for(int itor = 0; itor < tearLine.Count; itor++)
		{
			//If we have not solidified the starting vertice yet
			if(!haveFoundStartingEdgeVert)
			{
				//If we are not at the end of the current row
				if(itor + 1 < tearLine.Keys.Count)
				{
					//If the current vertice is and edge and the next is not
					/*if((tearLine.Keys.ElementAt(itor).x == WIDTH_MIN ||
						tearLine.Keys.ElementAt(itor).x == WIDTH_MAX ||
						tearLine.Keys.ElementAt(itor).y == HEIGHT_MIN ||
						tearLine.Keys.ElementAt(itor).y == HEIGHT_MAX) &&
						(tearLine.Keys.ElementAt(itor + 1).x != WIDTH_MIN &&
						tearLine.Keys.ElementAt(itor + 1).x != WIDTH_MAX &&
						tearLine.Keys.ElementAt(itor + 1).y != HEIGHT_MIN &&
						tearLine.Keys.ElementAt(itor + 1).y != HEIGHT_MAX))
					*/
					if(VerticeEdgeCheck(tearLine.Keys.ElementAt(itor)) && !VerticeEdgeCheck(tearLine.Keys.ElementAt(itor + 1)))
					{
						
						//Debug.LogError("New Tear Vertice = (" + tearLine.Keys.ElementAt(itor).x.ToString() + ", " + tearLine.Keys.ElementAt(itor).y.ToString() + ", " + tearLine.Keys.ElementAt(itor).z.ToString() + ") ");
						newTearLine.Add(tearLine.Keys.ElementAt(itor), tearLine.Values.ElementAt(itor));
						haveFoundStartingEdgeVert = true;
						haveFoundEndingEdgeVert = false;

					}
					
					else
					{
						//Debug.LogError("Removing");
						tearLinePositionTime.Remove (tearLineTime[tearLine.Keys.ElementAt(itor)]);	
						tearLineTime.Remove (tearLine.Keys.ElementAt(itor));	
						//tearLine.Remove(tearLine.Keys.ElementAt(itor));
					}
					
				}
			}
			else //if(!haveFoundEndingEdgeVert)//by this point, we have hit the starting tear vertice, and now we just add unitl we hit 
				//the ending tear vertice
			{
				/*
				if(tearLine.Keys.ElementAt(itor).x != WIDTH_MIN &&
					tearLine.Keys.ElementAt(itor).x != WIDTH_MAX &&
					tearLine.Keys.ElementAt(itor).y != HEIGHT_MIN &&
					tearLine.Keys.ElementAt(itor).y != HEIGHT_MAX)
					*/
				if(!VerticeEdgeCheck(tearLine.Keys.ElementAt(itor)))
					
				{
					//Debug.LogError("New Tear Vertice = (" + tearLine.Keys.ElementAt(itor).x.ToString() + ", " + tearLine.Keys.ElementAt(itor).y.ToString() + ", " + tearLine.Keys.ElementAt(itor).z.ToString() + ") ");
					newTearLine.Add(tearLine.Keys.ElementAt(itor), tearLine.Values.ElementAt(itor));
				}
				else /*if(tearLine.Keys.ElementAt(itor).x == WIDTH_MIN ||
					tearLine.Keys.ElementAt(itor).x == WIDTH_MAX ||
					tearLine.Keys.ElementAt(itor).y == HEIGHT_MIN ||
					tearLine.Keys.ElementAt(itor).y == HEIGHT_MAX)*/
				if(VerticeEdgeCheck(tearLine.Keys.ElementAt(itor)))
				{
					//Debug.LogError("**Last** New Tear Vertice = (" + tearLine.Keys.ElementAt(itor).x.ToString() + ", " + tearLine.Keys.ElementAt(itor).y.ToString() + ", " + tearLine.Keys.ElementAt(itor).z.ToString() + ") ");
					newTearLine.Add(tearLine.Keys.ElementAt(itor), tearLine.Values.ElementAt(itor));
					haveFoundEndingEdgeVert = true;
					
					tearLine.Clear();
					tearLine = newTearLine;
					//newTearLine.Clear();
					return;
				}
				else if(haveFoundEndingEdgeVert &&
					VerticeEdgeCheck(tearLine.Keys.ElementAt(itor))
					/*
					(tearLine.Keys.ElementAt(itor).x == WIDTH_MIN ||
					tearLine.Keys.ElementAt(itor).x == WIDTH_MAX ||
					tearLine.Keys.ElementAt(itor).y == HEIGHT_MIN ||
					tearLine.Keys.ElementAt(itor).y == HEIGHT_MAX)
					*/
					)
				{
					Debug.LogError("Removing 2");
					tearLinePositionTime.Remove (tearLineTime[tearLine.Keys.ElementAt(itor)]);	
					tearLineTime.Remove (tearLine.Keys.ElementAt(itor));
					//tearLine.Remove(tearLine.Keys.ElementAt(itor));
				}
			}
		}
	}
	
	/// <summary>
	/// Turns the input into torn vert curve
	/// This function is called to turn the mousePosition into torn vertices
	/// </summary>
	private bool TurnInputIntoTornVertCurve()
	{
		//Define a flag to return upon bad user input
		bool returnVal = false;
		
		//Flags the event when a tear successfully hits an existing vertice
		//this is used for error checking
		bool haveFoundGoodVertice = false;
		
		//Find the on screen depth of the object being torn
		Vector3 screenDepth = Camera.main.WorldToScreenPoint(this.transform.TransformPoint(mesh.vertices[0]));
		
		//Turn mouse positions into 3D world positions
		List<Vector3> newMouseTearWorldPos = new List<Vector3>();
		
		//int mouseTearPositionsCount = mouseTearPositions.Count();
		//First the mouseTearPositions needs to be translated into world positions
		for(int indexr = 0; indexr < mouseTearPositions.Count(); indexr++)
		{
			Vector3 curPosition = Camera.main.ScreenToWorldPoint(
				new Vector3(mouseTearPositions[indexr].x, mouseTearPositions[indexr].y, screenDepth.z));
			
			newMouseTearWorldPos.Add (curPosition);
		}
		
		mouseTearPositions.Clear();
		//Debug.LogError("*********Converting mouse positions with length = " + mouseTearPositions.Count().ToString());
		//Re-assign the mouse positions to the 3D positions
		mouseTearPositions = newMouseTearWorldPos;
		
		//UnityEngine.Debug.LogError("Testing1 mouseTearPositions.Count() = " + mouseTearPositions.Count());
		//newMouseTearWorldPos.Clear ();
		//UnityEngine.Debug.LogError("Testing2 mouseTearPositions.Count() = " + mouseTearPositions.Count());
		
		//Make sure UI filled
		CheckForMissingUserInput();
		
		//This is used to keeping track of the previously torn vertice
		Vector3 previousTornVert; //Clone.GetComponent<MeshFilter>().mesh.vertices[0];
		
		//This keeps track of the number of 
		int numberOfVerts = 0;
		
		//Create structure to keep track of the previous mouse position
		Vector3 previousMousePos = mouseTearPositions[0];//mouseTearPositions.ElementAt(0);

		int testFreq = 0;
		int testDistGood = 0;
		
		//UnityEngine.Debug.Log("mouseTearPosition Count = " + mouseTearPositions.Count.ToString());
		
		//Loop through the mouseTearPositions
		//float distToClosestRow;
		int rowNum;
		int paperGridKeysCount = paperGrid.Keys.Count;
		List<float> paperGridKeys = paperGrid.Keys.ToList();
		for(int jtor = 0; jtor < mouseTearPositions.Count - 1; jtor++)
		{
			//Vector3 screenWorldDepth = new Vector3(mouseTearPositions[jtor].x, mouseTearPositions[jtor].y, 
			//										Camera.main.WorldToScreenPoint(gameObject.collider.transform.position).z);
			//Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(screenWorldDepth);
			
			
			//Find change in position of the mouse
			//float mouseMoveDistX = mouseTearPositions[jtor].x - previousMousePos.x;
			//float mouseMoveDistY = mouseTearPositions[jtor].y - previousMousePos.y;
			
			//Make sure the move distance is always positive for flag checking
			//if(mouseMoveDistX < 0) mouseMoveDistX *= -1;
			//if(mouseMoveDistY < 0) mouseMoveDistY *= -1;
			
			//float ratio = 0.2f;
			//UnityEngine.Debug.Log("distance mouse in x = " + mouseMoveDistX.ToString() + " Mesh Offset = " + MESH_VERT_OFFSET.ToString());
			//UnityEngine.Debug.Log("distance mouse in y = " + mouseMoveDistY.ToString() + " Mesh Offset = " + MESH_VERT_OFFSET.ToString());
			
			//If jtor == 0, we are at an edge, so we forsure add, else we only add every time
			// the mouse has moved the distance inbetween any two adjacent vertices on paper mesh
			//if(jtor == 0 || mouseMoveDistX >= (MESH_VERT_OFFSET * (ratio)) || mouseMoveDistY >= (MESH_VERT_OFFSET * (ratio)))
			{
				//Debug.LogWarning("Mouse Has Moved the offset amount to add new vert");
				//Now, we know here we need to find the vert closest to the mouse position to add to the
				// tearLine
				
				float distToClosestRow = MESH_VERT_OFFSET * 200;
				rowNum = -1;
				//paperGridKeysCount = paperGridKeys.Count;//paperGrid.Keys.Count;
				//First, loop through grid to find row with closest y-component
				for(int itor = 0; itor < paperGridKeysCount; itor++)
				{
					float tempDist = paperGridKeys[itor] - mouseTearPositions[jtor].y;
						//used to be this
						//paperGrid.Keys.ElementAt(itor) - mouseTearPositions[jtor].y;
					if(tempDist < 0) tempDist *= -1;
					
					if(tempDist < distToClosestRow)
					{
						distToClosestRow = tempDist;
						rowNum = itor;
					}
				}
				
				float distToClosestCol = MESH_VERT_OFFSET * 200;
				int colomnNum = -1;
				
				//Make sure we have valid input
				//if(rowNum != -1)
				{
					//Now, we loop through grid to find vert in colomn with closest x-component
					//Debug.LogWarning("Row Number = " + rowNum.ToString());
					//not sure what this is less than, but if issues just switch with ktor < part
					int count = paperGrid[paperGridKeys[rowNum]].Count;//paperGrid[paperGrid.Keys.ElementAt(rowNum)].Count;
					for(int ktor = 0; ktor < count; ktor++)//paperGrid[paperGrid.Keys.ElementAt(rowNum)]
					{
						float tempDist2 =  paperGrid[paperGridKeys[rowNum]][ktor].x - mouseTearPositions[jtor].x;
							//used to be this
							//paperGrid[paperGrid.Keys.ElementAt(rowNum)][ktor].x - mouseTearPositions.ElementAt(jtor).x;
						if(tempDist2 < 0) tempDist2 *= -1;
						
						if(tempDist2 < distToClosestCol)
						{
							distToClosestCol = tempDist2;
							colomnNum = ktor;
						}
					}
				}
				
				
				float distCheck;// = -100;
				//Only perform a real distance check if we have fould a valid indice to add
				//if(colomnNum != -1 && rowNum != -1)
				{
					distCheck = Vector3.Distance(mouseTearPositions[jtor], paperGrid[paperGridKeys[rowNum]][colomnNum]);
						//paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum]);
					++testFreq;
				}
				
				
				//Now rowNum and Colomn Num point to the new tornVert, so we add to tearLine list	
				if((distCheck <= MESH_VERT_OFFSET)// && distCheck != -100)
					&&
					(
					(!tearLine.ContainsKey(paperGrid[paperGridKeys[rowNum]][colomnNum]) )
					//used to be this
					//paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum]) )//&& distCheck != -100
					|| 
						(
						tearLine.ContainsKey(paperGrid[paperGridKeys[rowNum]][colomnNum])
						&& tearLineTime[paperGrid[paperGridKeys[rowNum]][colomnNum]] != tearLineTimer
						//&& distCheck != -100
						)
					)
					
				  )
				{
					++testDistGood;
					haveFoundGoodVertice = true;
					//Debug.LogError("*****************distCheck <= MESH_VERT_OFFSET**********************");
					tearLineTimer++;
					numberOfVerts++;
					
					/*	
					//THE FOLLOWING NEEDS OPTIMIZATION, here we need to add the indice of the new torn vertice into tearline,
					// therefore the quickest, but slowest solution is to iterate through all mesh verts....
					int meshIndex = -1;
					for(int gtor = 0; gtor < mesh.vertices.Length; gtor++)
					{
						if(this.transform.TransformPoint(mesh.vertices.ElementAt(gtor)) == this.transform.TransformPoint(paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum]))
						{
							meshIndex = gtor;
							break;
						}
					}
					*/
					
					//The following try will always be called unless a loop is found within curve, in this case, the satch is called to forceStopTear
					try
					{
						int time = verticePosIndexIntoMesh[paperGrid[paperGridKeys[rowNum]][colomnNum]];
							//paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum]];
						tearLine.Add(paperGrid[paperGridKeys[rowNum]][colomnNum], time);
							//paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum], time);
						
						//tearLine.Add(paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum], meshIndex);
					}
					catch
					{
				
				//Debug.LogError("ForceStopTear #7");
				
						//hear a loop is found within the curve, so we FORCE STOP TEAR BECAUSE LOOP FOUND
						//Debug.Log("**********************FORCE STOP TEAR**************************");
						//if(!TearManager.BadTear)
							ForceStopTear("precise");
				hitPlayerDuringTear = 0;
						return true;
					}
					
					//Flag the current vertice to be a torn vetice
					paperGridTearVertCheck[paperGrid[paperGridKeys[rowNum]][colomnNum]] = true;
						//paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum]] = true;
						
					//This maps a 'time' to the tear vertice
					tearLineTime.Add (paperGrid[paperGridKeys[rowNum]][colomnNum]
						//paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum]
						, (float)tearLineTimer);
					tearLinePositionTime.Add ((float)tearLineTimer, paperGrid[paperGridKeys[rowNum]][colomnNum]);
						//paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum]);
					previousTornVert = paperGrid[paperGridKeys[rowNum]][colomnNum];
						//paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum];
					
					//previousMousePos = previousTornVert;
					
				}
				//else //if(distCheck != -1000)
				//{
				//	returnVal = true;
				//}
				//UnityEngine.Debug.Log("Previous about to be set");
				//Set previous the the position we just looked at
				
				
				previousMousePos = mouseTearPositions[jtor];
			}
			
		}
		
		
		//UnityEngine.Debug.Log("Testing how many times distance checked = " + testFreq.ToString());
		//UnityEngine.Debug.Log("Testing how many times distance GOOD = " + testDistGood.ToString());
		//UnityEngine.Debug.Log("Tearline length = " + tearLine.Keys.Count().ToString());
		List<Vector3> tearLineKeys = tearLine.Keys.ToList();
		bool check = false;
		if(tearLine.Keys.Count() > 2)
		{
			bool badTear = true;
			Vector3 tempTearPos = tearLineKeys[0];//tearLine.Keys.ElementAt(0);
			for(int itor = 1; itor < tearLineKeys.Count(); itor++)
			{
				if(tearLineKeys[itor]/*Keys.ElementAt(itor)*/ != tempTearPos)
				{
					badTear = false;
					break;
				}
			}
			
			if(badTear)
			{
				check = true;
				if(!TearManager.BadTear)
					Debug.Log("BAD TEAR FOUND - same vertice is a torn vertice multiple times");
			}
		}
		
		if(tearLineKeys.Count() == 0 || tearLineKeys.Count() == 1 || tearLineKeys.Count() == 2 || check)
		{
			Debug.Log("Tearline Too small");
			//if(!TearManager.BadTear)
				ForceStopTear("precise");
				hitPlayerDuringTear = 0;
			return true;
		}
		
		//Set flag correctly if a successful tear vertice was found
		if(haveFoundGoodVertice)
		{
			returnVal = false;
		}
		
		return returnVal;
	}
	
	/// <summary>
	/// Forces the tear to stop when bad input detected, such as a loop
	/// </summary>
	public void ForceStopTear(string badTearReason)
	{
		//Debug.Log(badTearReason);
		//GetComponent<TearCoverUp>().ReturnToPreviousPlatformState();
		
		
		haveHitLimitDuringTear = false;
		
		
		if(badTearReason != "reset" && badTearReason != "ignore" )
		{
			//BadTearVisual.renderer.enabled = false;
			TearManager.DrawBadTearVisual = true;
		}
		else
		{
			TearManager.DrawBadTearVisual = false;
		}
		
		//if(TearManager.DrawBadTearVisual)
		{
			//Change material for correct visual of bad tear
			if(badTearReason == "magicCarpet" && BadTearVisual.renderer.material != BadTearMaterialNoMagicCarpet)
			{
				BadTearVisual.renderer.material = BadTearMaterialNoMagicCarpet;
			}
			else if(badTearReason == "door" && BadTearVisual.renderer.material != BadTearMaterialNoDoorTear)
			{
				BadTearVisual.renderer.material = BadTearMaterialNoDoorTear;
			}
			else if(badTearReason == "precise" && BadTearVisual.renderer.material != BadTearMaterialNoPreciseTear)
			{
				BadTearVisual.renderer.material = BadTearMaterialNoPreciseTear;
			}
			else if(badTearReason == "tearingFold" && BadTearVisual.renderer.material != BadTearMaterialNoTearingFold)
			{
				BadTearVisual.renderer.material = BadTearMaterialNoTearingFold;
			}
			else if(badTearReason == "noTear" && BadTearVisual.renderer.material != BadTearMaterialAlpha)
			{
				BadTearVisual.renderer.material = BadTearMaterialAlpha;
			}
		}
		
		if(TearEdgeFXPool != null)
		{
			//remove any tear edge FX
			for(int ktor = 0; ktor < TearEdgeFXPool.Length; ktor++)
			{
				TearEdgeFXPool[ktor].transform.position = new Vector3(-1000, -1000, -1000);
			}
			//tornEdgeVisualFxRef.Clear();
			
			currentItor = 0;
			
		}
		if(allEdgeFX != null) allEdgeFX.Clear();
		if(itorsIntoMeshEdgeFX != null) itorsIntoMeshEdgeFX.Clear();
		if(LerpToFromCheck != null) LerpToFromCheck.Clear();
		if(tearFXholder != null) tearFXholder.Clear();
			GameObject.Destroy (tearEdgeFXcontainer);
			tearEdgeFXcontainer = new GameObject("Tear_EdgeFXcontainer");
		
		
		//if(TearManager.PlayerShield.renderer.enabled) TearManager.PlayerShield.renderer.enabled = false;
		
		TearManager.ResetParentingPlatforms();
		
		//UnityEngine.Debug.LogError("Entering Force Stop tear because -> " + badTearReason.ToString());
		
		indexor1 = 0;
		indexor2 = 0;
		
		totalTimeTearingTimer = 0;
		
		if(TearManager.TearFinished)
		{
			TearManager.TearFinished = false;
			
		}
		ForceFinishTearLine = false;
		
		TearManager.hittingFoldedArea = false;
		TearManager.haveHitPaper = false;
		
		TearManager.ResetPlatforms();
		
		if(TearManager.TornPlatforms.Count() > 0)
		{
			TearManager.TornPlatforms.Clear();
		}
		
		
		
		//If world paper
		if(!PlatformPaper)
		{
			
			
			TearManager.BadTear = true;
			
			//Remove all tear effects along edge if already calculated
			GameObject[] tearFx = GameObject.FindGameObjectsWithTag("tearEdgeFX");
			for(int itor = 0; itor < tearFx.Length; itor++)
			{
				GameObject.Destroy(tearFx[itor]);
			}
			if(tearVertEdgeFXpos == null) tearVertEdgeFXpos = new List<Vector3>();
			else tearVertEdgeFXpos.Clear();
			
			if(tearVertEdgeFXposFinal == null) tearVertEdgeFXposFinal = new List<Vector3>();
			else tearVertEdgeFXposFinal.Clear();
			
			
		}
		
		if(TearManager.centerPositions.Count > 0)
		{
			TearManager.centerPositions.Clear();
		}
		
		//Debug.LogError("Now forceing Stop tear");
		//Rest everything needed to reset the states when a bad tear occurs
		cuttInProgress = false;
		doneCalculatingCuttLine = false;
		
		if(mouseTearPositions == null) mouseTearPositions = new List<Vector3>();
		else mouseTearPositions.Clear ();
		
		//tearLine = null;
		tearLine.Clear ();// = new Dictionary<Vector3, int>();
		
		if(gapPositions == null) gapPositions = new List<int>();
		else gapPositions.Clear ();
		
		//tearLineTime = null;
		
		if(tearLineTime == null) tearLineTime = new Dictionary<Vector3, float>();
		else tearLineTime.Clear ();
		
		if(tearLinePositionTime == null) tearLinePositionTime = new Dictionary<float, Vector3>();
		else tearLinePositionTime.Clear ();
		
		addingToPieceOne = true;
		tearLineTimer = 1;
		forceStopTear = false; 
		haveTouchedOffPaperToStartTear = false;
		TearManager.tornThroughBadObject = false;
		haveHitPaper = false;
		
		ForceStopTearTimer = 0;
		
		
		NeedToCheckExcessDuplicates = false;
		
		island1Indicies = null;
		island2Indicies = null;
		
		
		//The following makes sure every object is properly disposed
		CleanUpClonedObjects();
	}
	
	/// <summary>
	/// Cleans up cloned objects and restores mesh 
	/// visibility and collision iff needed
	/// </summary>
	private void CleanUpClonedObjects()
	{
		
		if(TearManager.MainWorldCutPaper != null)
		{
			TearManager.MainWorldCutPaper.transform.position = TearManager.MainStartingWorldPaper.transform.position;
			TearManager.MainWorldCutPaper.transform.rotation = TearManager.MainStartingWorldPaper.transform.rotation;
		}
		
		if(TearManager.MainWorldPaper != null)
		{
			TearManager.MainWorldPaper.transform.position = TearManager.MainStartingWorldPaper.transform.position;
			TearManager.MainWorldPaper.transform.rotation = TearManager.MainStartingWorldPaper.transform.rotation;
		}
		
		
		//reset clone information
		Clone.GetComponent<MeshFilter>().mesh = TearManager.MainStartingWorldPaper.GetComponent<MeshFilter>().mesh;
		Clone2.GetComponent<MeshFilter>().mesh = TearManager.MainStartingWorldPaper.GetComponent<MeshFilter>().mesh;
		Clone.GetComponent<MeshCollider>().mesh = TearManager.MainStartingWorldPaper.GetComponent<MeshFilter>().mesh;
		Clone2.GetComponent<MeshCollider>().mesh = TearManager.MainStartingWorldPaper.GetComponent<MeshFilter>().mesh;
		Clone.GetComponent<MeshRenderer>().material.color = Color.white;
		Clone2.GetComponent<MeshRenderer>().material.color = Color.white;
		
		
		
		
		
		TearManager.MainWorldPaper = null;
		TearManager.MainWorldCutPaper = null;
		
		Clone.SetActive(false);
		Clone2.SetActive(false);
		
		if(!GetComponent<MeshRenderer>().enabled && !CloneObject)
		{
			GetComponent<MeshRenderer>().enabled = true;
		}
		if(!GetComponent<MeshCollider>().enabled && !CloneObject)
		{
			GetComponent<MeshCollider>().enabled = true;
		}
		
		TearManager.ResetTriangles(gameObject);
	}
	
	/// <summary>
	/// Cutts the paper object along tearline
	/// Before this is called, we already know which vertices are torn, here, we determine which
	/// vertices belong to which new cutt piece
	/// </summary>
	private void FindNewCutPieces()
	{
		bool badTearCheckr = false;
		//Ensure that the tearline in not made up of only edge vertice

		//TEST FOR TEAR CODE WITHOUT ELEMENTAT
		//used to be the the for loop 
		//foreach(Vector3 tearLineKey in tearLine.Keys)
			
		List<Vector3> tearLineKeys = tearLine.Keys.ToList();
		for(int itor = 0; itor < tearLineKeys.Count(); itor++)
		{
			if(tearLineKeys[itor].x < MaxWorldWidth
				&& tearLineKeys[itor].x > MinWorldWidth
				&& tearLineKeys[itor].y < MaxWorldHeight
				&& tearLineKeys[itor].y > MinWorldHeight)
		/*{
			if(tearLineKey.x < MaxWorldWidth
				&& tearLineKey.x > MinWorldWidth
				&& tearLineKey.y < MaxWorldHeight
				&& tearLineKey.y > MinWorldHeight
				)*/
			{
				badTearCheckr = false;
				break;
			}
			else
			{
				badTearCheckr = true;
			}
		}
		if(badTearCheckr)
		{
			ForceStopTear("precise");
				hitPlayerDuringTear = 0;
			return;
		}
		
		
		if(Clone == null)
		{
			//Debug.Log("Testing bad tear check");
			doneCalculatingCuttLine = false;
			return;
		}
		
		//The following initialize the storages holding the new vertices and faces for the two new citt paper objects
		CuttPieceOneVerts = GetComponent<MeshFilter>().mesh.vertices;
		CuttPieceTwoVerts = GetComponent<MeshFilter>().mesh.vertices;
		CuttPieceOneFaces = GetComponent<MeshFilter>().mesh.triangles;
		CuttPieceTwoFaces = GetComponent<MeshFilter>().mesh.triangles;
		
		/*
		//First, we update the paperGridTearVertCheck DICT for determining whether a vertex has been hit be the user input
		for(int itor = 0; itor < CuttPieceOneVerts.Length; itor++)
		{
			Vector3 testPos = this.transform.TransformPoint(CuttPieceOneVerts[itor]);
			if(tearLine.ContainsKey(testPos))
			{
				paperGridTearVertCheck[new Vector2(testPos.y, testPos.x)] = true;
			}
			else
			{
				paperGridTearVertCheck[new Vector2(testPos.y, testPos.x)] = false;
			}
		}
		*/
		
		/*
		//
		//Now we know which verticies are selected and how to distinguish both islands thanks to 
		// the organized paper vert grid found previously
		//
		*/
		
		//Now, we create two lists of verticies to determine which vertices belong to which 'island'
		if(island1 == null) island1 = new HashSet<Vector2>();//List<Vector2>();
		else island1.Clear();
		
		if(island2 == null) island2 = new HashSet<Vector2>();//List<Vector2>();
		else island2.Clear();
		
		//The following is used to switch in between both new meshes for data transfer after cutt/tear
		addingToPieceOne = true;
		
		//This flags is we are currently at a tear vertice to add to both islands
		bool currentlyInTransition = false;
		
		//This is used for testing, printing the number of torn vertices being created
		int numEdgeTearVerts = 0;
		
		//This points to the vertex in the current row where the tear section begins
		int startingVertIndice = 0;

		//added by doug, so you can check to ensure that the last point and new point are in different islands
		//Vector3 lastPaperGridValueVector = new Vector3(30000, 30000, 30000);
		//bool lastPaperGridValueVectorAssigned = false;
		List<List<Vector3>> paperGridValues = paperGrid.Values.ToList();
		//We looop through the sorted paperGrid to create two new mesh based of tearLine
		//foreach(List<Vector3> paperGridValueList in paperGrid.Values)
		for(int iterator = 0; iterator < paperGridValues.Count; iterator++)//each(List<Vector3> tempList in paperGrid.Values)
		{

			//This is used for correctly assigning the beginning and ending tear sections of a tear region within
			//the currect row being raversed.
			bool haveHitTearLine = false;
			
			//This represents the tear vertex's position at the beginning of the current row's tear region
			//This is the first one hit
			Vector3 startTearPos = Vector3.zero;
			
			//This represents the tear vertex's position at the end of the current row's tear region
			//This is the last one hit ebfore another non-torn vertex is reached in the current row
			Vector3 endTearPos = Vector3.zero;
			
			//This is triggered true once we reach the end of one of the current row's tear regions,
			// this is when the critical decision is made, deciding which island to add to next for the next 
			// non- torn vertex within the current row
			bool readyToDeterminShape = false;
			
							/*
			//
			//First we see if the begginning of the current row is a NON torn vertex and the beggginning
			//of the last row IS a torn vertex
			//
			*/
			
			//We make sure we are already past the first row because the following logic
			//is dependent upon the last row looked at
			//if(lastPaperGridValueVectorAssigned)
			//commented if out by douglas
			if(iterator - 1 >= 0)
			{
				//If the current row's first vertex is not torn and the last row's first vertex is torn
				if ((!tearLine.ContainsKey(paperGridValues[iterator][0]/*paperGrid.Values.ElementAt(iterator)[0]*/) && 
					tearLine.ContainsKey(paperGridValues[iterator-1][0]/*paperGrid.Values.ElementAt(iterator - 1)[0]*/))
					)
				{
					//Debug.Log("increasing number of edge torn vertices");
					//We increase the numEdgeTearVerts to keep track of which island we are adding to when
					//the first row's veretex IS NOT a torn vertex
					numEdgeTearVerts++;
				}
			}
			//lastPaperGridValueVector = paperGridValueList[0];
			//lastPaperGridValueVectorAssigned = true;
			//Keep tarck of which piec we have been adding to previously
			addingToPieceOnePreviously = addingToPieceOne;
			
			//Now we change which island we are adding to, rotating between each one
			if(numEdgeTearVerts % 2 == 0)
			{
				addingToPieceOne = true;
			}
			else if(numEdgeTearVerts % 2 == 1)
			{
				addingToPieceOne = false;
			}
			
			
			//if(!CloneObject && !PlatformPaper)
			//{
			//	UnityEngine.Debug.Log("About to start deciding for row # " + iterator.ToString());
			//}
			
			//Iterate through every single value in the current row being observed
			for(int jtor = 0; jtor < paperGridValues[iterator].Count/*paperGrid.Values.ElementAt(iterator).Count*/; jtor++)
			{
				/*
				//
				//Now, we need to remember the starting and ending vertices 
				// in each of the current row's torn regions, this allows us
				// to accuratly find the incoming and outgoing directions of
				// the tear curve to then determine if we are rotating which
				// island we are adding to in the current row
				//
				*/
				
				//if the current vertex being looked at is a tear vertex and we have not hit one of the tear 
				// region within the current row yet.
				if(tearLine.ContainsKey(paperGridValues[iterator][jtor])//paperGrid.Values.ElementAt(iterator)[jtor]) 
					&& !haveHitTearLine)
				{
					//Set flag to true, for proper assignment of ending poisiton in the current row's tear region
					haveHitTearLine = true;
					
					//Assign the starting position of the current row's tear region being traversed.
					startTearPos = paperGridValues[iterator][jtor];//paperGrid.Values.ElementAt(iterator)[jtor];
					
					//This is a pointer to the row's list stored within the dictionary for proper direction of curve calculation
					startingVertIndice = jtor;
					
					//This is flagged to false because we are not yet ready to make the decision of which island to transition to
					readyToDeterminShape = false;
				}
				
				//If we have started a tear region
				if(haveHitTearLine)
				{
					//float distanceCheck = 0;
					//if((jtor + 1) < paperGrid.Values.ElementAt(iterator).Count)
					//{
					//	distanceCheck = paperGrid.Values.ElementAt(iterator)[jtor + 1].x - paperGrid.Values.ElementAt(iterator)[jtor].x;
					//	if(distanceCheck < 0) distanceCheck *= -1;
					//}
					
					//If we are not at the last position in the row AND 
					//If the next vertex in row in not torn, then we know
					//	we have finally hit the end of one of the current row's
					//	tear regions
					if(((jtor + 1) < paperGridValues[iterator].Count/*paperGrid.Values.ElementAt(iterator).Count*/ && 
						!tearLine.ContainsKey(paperGridValues[iterator][jtor + 1]/*paperGrid.Values.ElementAt(iterator)[jtor + 1]*/))
						//||
						//distanceCheck > MESH_VERT_OFFSET
						)
					{
						//This is set to false for triggering true for the possibility of the current
						//row containing another torn vertice region
						haveHitTearLine = false;
						
						//Flag we are now ready to make a decision based on incoming and outgoing direction
						//of curve about this tear region
						readyToDeterminShape = true;
						
						//Reset time switched for each row
						//timeSwitched = false;
					}
					
					
					//Assign the end position in the current row's tear region to be the current vertex
					//because we know that the next vertex in the row is not torn
					endTearPos = paperGridValues[iterator][jtor];//paperGrid.Values.ElementAt(iterator)[jtor];
				}
				
				
				//In the following, the x==y and y==x already, these components have already been switched
				TraverseGridAddToNewPiece(paperGridValues[iterator][jtor]/*paperGrid.Values.ElementAt(iterator)[jtor]*/, 
					new Vector2(paperGridValues[iterator][jtor].y/*paperGrid.Values.ElementAt(iterator)[jtor].y*/, 
					paperGridValues[iterator][jtor].x/*paperGrid.Values.ElementAt(iterator)[jtor].x*/), 
					currentlyInTransition, 
					jtor, paperGridValues[iterator]/*paperGrid.Values.ElementAt(iterator)*/, 
					startTearPos, endTearPos, readyToDeterminShape, startingVertIndice);
			}
		}
		
		
		
		
		//Make the second clone visible
		Clone2.GetComponent<MeshRenderer>().enabled = true;
		
		//Initialize the length of each new triangle storage for the new objects faces
		island1Indicies = new int[CuttPieceOneFaces.Length];
		island2Indicies = new int[CuttPieceTwoFaces.Length];
		
		//This is used to properly index the above new triangle face data for the
		// two new objects after tear
		indexor1 = 0;
		indexor2 = 0;
		
		/*
		//
		//Now we have the associates vertices stored within island1 and island 2
		//We can now determine the new faces for each mesh by FIRST finding the indices for each vertice
		//
		*/
		
		//Loop through every single face in the originial object to determine
		// which new faces belong on which island based of the vertex seperation 
		// logic performed above
		for(int itor = 0; itor < CuttPieceOneFaces.Length; itor += 3)
		{
			//The following is used to add a specific face to one island or the other
			CreateNewCuttPaperMeshPieces(itor);
		}
		
		
		//timetest += Time.deltaTime;
		//UnityEngine.Debug.Log("Time before island1Indicies = " + timetest.ToString());
		
		
		//Stores iland 1 face index information for construction of faces on mesh.
		//We are using this to remove the extra (-1.25, -0.875) (index = 0) because as of now
		//these are somehow being added to island1Indicies
		List<int> newIsland1indTempList = new List<int>();
		//Store the number of faces references to properly create new triangles array
		int numOfvertsIsland1 = 0;
		//This flags the moments we have hit actual faces within island to prevent
		//actual faces with index = 0 from being created
		bool haveStarted = false;
		//Loop through the island1Indicies array which stores the inexs into the 
		//vertices to create faces of mesh
		for(int itor = island1Indicies.Count() - 1; itor >  -1; --itor)
		{
			//prevent the mysterious index = 0 ==> vertex (-1.25, -0.875) from 
			//being added to the end multiple times ***TODO*** Fix this bug
			if(island1Indicies[itor] != 0 || haveStarted)
			{
				haveStarted = true;
				newIsland1indTempList.Add(island1Indicies[itor]);
				++numOfvertsIsland1;
			}
		}
		
		//Create new storeage for traingles date
		int[] newIsland1Ind = new int[numOfvertsIsland1];
		//Create index for storing new information with chopped off
		//index = 0 ==> vertex (-1.25, -0.875) from the end
		int indexer = 0;
		//Loop through the modified (trimmed of excess mysterious indexs) list derived above
		//to create new triangles data for new torn piece
		for(int itor = newIsland1indTempList.Count() - 1; itor >  -1; --itor)
		{
			newIsland1Ind[indexer] = newIsland1indTempList[itor];
			++indexer;
		}
		
		newIsland1indTempList.Clear();
		//Set new modified triangles to define mesh faces
		island1Indicies = newIsland1Ind;
		
		//Stores iland 1 face index information for construction of faces on mesh.
		//We are using this to remove the extra (-1.25, -0.875) (index = 0) because as of now
		//these are somehow being added to island2Indicies
		List<int> newIsland2indTempList = new List<int>();
		//Store the number of faces references to properly create new triangles array
		int numOfvertsIsland2 = 0;
		//This flags the moments we have hit actual faces within island to prevent
		//actual faces with index = 0 from being created
		bool haveStarted2 = false;
		//Loop through the island12ndicies array which stores the inexs into the 
		//vertices to create faces of mesh
		for(int itor = island2Indicies.Count() - 1; itor >  -1; --itor)
		{
			//prevent the mysterious index = 0 ==> vertex (-1.25, -0.875) from 
			//being added to the end multiple times ***TODO*** Fix this bug
			if(island2Indicies[itor] != 0 || haveStarted2)
			{
				haveStarted2 = true;
				newIsland2indTempList.Add(island2Indicies[itor]);
				++numOfvertsIsland2;
			}
		}
		
		//Create new storeage for traingles date
		int[] newIsland2Ind = new int[numOfvertsIsland2];
		//Create index for storing new information with chopped off
		//index = 0 ==> vertex (-1.25, -0.875) from the end
		int indexer2 = 0;
		//Loop through the modified (trimmed of excess mysterious indexs) list derived above
		//to create new triangles data for new torn piece
		for(int itor = newIsland2indTempList.Count() - 1; itor >  -1; --itor)
		{
			newIsland2Ind[indexer2] = newIsland2indTempList[itor];
			++indexer2;
		}
		newIsland2indTempList.Clear();
		//Set new modified triangles to define mesh faces
		island2Indicies = newIsland2Ind;
		
		
		
		//timetest += Time.deltaTime;
		//UnityEngine.Debug.Log("Time after island1Indicies = " + timetest.ToString());
		
		
		
		if(island1Indicies.Count() % 3 == 0 && island2Indicies.Count() % 3 == 0)
		{
			//Reassign new mesh triangles, defining the new faces for the cloned object
			Clone.GetComponent<MeshFilter>().mesh.triangles = island1Indicies;
			Clone2.GetComponent<MeshFilter>().mesh.triangles = island2Indicies;
			
			Clone.GetComponent<MeshFilter>().mesh.RecalculateBounds();
			Clone.GetComponent<MeshFilter>().mesh.RecalculateNormals();
			Clone2.GetComponent<MeshFilter>().mesh.RecalculateBounds();
			Clone2.GetComponent<MeshFilter>().mesh.RecalculateNormals();
			gameObject.GetComponent<MeshFilter>().mesh.RecalculateBounds();
			gameObject.GetComponent<MeshFilter>().mesh.RecalculateNormals();
			
			
			//Update the clone's mesh collider
			Clone.GetComponent<MeshCollider>().sharedMesh = Clone.GetComponent<MeshFilter>().mesh;
			Clone2.GetComponent<MeshCollider>().sharedMesh = Clone2.GetComponent<MeshFilter>().mesh;
			
			//Assign relations correctly between duplicate objects
			Clone.GetComponent<TearPaper>().Clone = Clone;
			Clone.GetComponent<TearPaper>().Clone2 = Clone2;
			Clone.GetComponent<TearPaper>().tearLine = tearLine;
			Clone2.GetComponent<TearPaper>().tearLine = tearLine;
			Clone2.GetComponent<TearPaper>().Clone = Clone;
			Clone2.GetComponent<TearPaper>().Clone2 = Clone2;
		}
		else
		{
				
			//	Debug.LogError("ForceStopTear #9");
				
			//if(!TearManager.BadTear)
				ForceStopTear("precise");
				hitPlayerDuringTear = 0;
			Debug.LogError("TODO --> Fix this bullshit bug -> J.C.");
		}
		

		//Rename the object based on the number of faces in each of the new meshs
		if(island2.Count > island1.Count)
		{
			if(!PlatformPaper)
			{
				Clone2.name = "paper_CuttPieceOfPaper";
				Clone.name = "paper_LargerPiece";
				
				originalColor = Clone2.GetComponent<MeshRenderer>().material.color;
				Clone2.GetComponent<MeshRenderer>().material.color = Color.grey;
			}
			
			
			CurrentCuttPiece = Clone2;
			CurrentLargeCuttPiece = Clone;
			
			//If world paper
			if(!PlatformPaper)
			{
				TearManager.MainWorldCutPaper = Clone2;
				TearManager.MainWorldCutPaperTriangles = Clone2.GetComponent<MeshFilter>().mesh.triangles;
				TearManager.MainWorldCutPaperVertices = Clone2.GetComponent<MeshFilter>().mesh.vertices;
				
				TearManager.MainWorldPaper = Clone;
				TearManager.TearFinished = true;
			}

		}
		else
		{
			if(!PlatformPaper)
			{
				Clone.name = "paper_CuttPieceOfPaper";
				Clone2.name = "paper_LargerPiece";
				
				originalColor = Clone2.GetComponent<MeshRenderer>().material.color;
				Clone.GetComponent<MeshRenderer>().material.color = Color.grey;
			}
			
			CurrentCuttPiece = Clone;
			CurrentLargeCuttPiece = Clone2;
			
			//If world paper
			if(!PlatformPaper)
			{
				TearManager.MainWorldCutPaper = Clone;
				TearManager.MainWorldCutPaperTriangles = Clone.GetComponent<MeshFilter>().mesh.triangles;
				TearManager.MainWorldCutPaperVertices = Clone.GetComponent<MeshFilter>().mesh.vertices;
				
				
				TearManager.MainWorldPaper = Clone2;
				TearManager.TearFinished = true;
				
				
			}
			
		}
		
		island1.Clear();
		island2.Clear();
		
		//Turn true to flag that we are now done calculating the cut line
		doneCalculatingCuttLine = false;
		
		
	}
	
	
	/// <summary>
	/// Sets the mesh offset for this object's vertice topology
	/// </summary>
	private void SetMeshOffsetWorldSpace()
	{
		//The following assumes vertices located at 0 and 1 are located next to eachother
		Vector3[] thisMeshFilterVert = thisMeshFilter.mesh.vertices;
		if(thisMeshFilterVert[0].x != thisMeshFilterVert[1].x)
		{
			SCREEN_MESH_VERT_OFFSET = Camera.main.WorldToScreenPoint(this.transform.TransformPoint(thisMeshFilterVert[0])).x - 
										Camera.main.WorldToScreenPoint(this.transform.TransformPoint(thisMeshFilterVert[1])).x;
			MESH_VERT_OFFSET = this.transform.TransformPoint(thisMeshFilterVert[0]).x - 
										this.transform.TransformPoint(thisMeshFilterVert[1]).x;
		}
		else
		{
			SCREEN_MESH_VERT_OFFSET = Camera.main.WorldToScreenPoint(this.transform.TransformPoint(thisMeshFilterVert[0])).y - 
										Camera.main.WorldToScreenPoint(this.transform.TransformPoint(thisMeshFilterVert[1])).y;
			MESH_VERT_OFFSET = this.transform.TransformPoint(thisMeshFilterVert[0]).y - 
										this.transform.TransformPoint(thisMeshFilterVert[1]).y;
		}
		
		//Make sure the mesh offset is non-negative
		if(MESH_VERT_OFFSET < 0) MESH_VERT_OFFSET *= -1;
		if(SCREEN_MESH_VERT_OFFSET < 0) SCREEN_MESH_VERT_OFFSET *= -1;
		
		//Testing, output the offset
		//Debug.LogWarning("Mesh vertice offset = " + MESH_VERT_OFFSET.ToString());
		
		//if this tearPaper script is not a platform, then we know it's the main paper world
		//therefore, we set flag to check closest distance order one
		//if(!PlatformPaper)
		{
			//GetComponent<ClosestVertice>().MeshOffset = MESH_VERT_OFFSET;
		}
	}
	
		/// <summary>
	/// Sets the mesh offset for this object's vertice topology
	/// </summary>
	private void SetMeshOffsetScreenSpace()
	{
		//The following assumes vertices located at 0 and 1 are located next to eachother
		if(thisMeshFilter.mesh.vertices[0].x != thisMeshFilter.mesh.vertices[1].x)
		{
			MESH_VERT_OFFSET = Camera.main.WorldToScreenPoint(this.transform.TransformPoint(thisMeshFilter.mesh.vertices[0])).x - 
				Camera.main.WorldToScreenPoint(this.transform.TransformPoint(thisMeshFilter.mesh.vertices[1])).x;
		}
		else
		{
			MESH_VERT_OFFSET = Camera.main.WorldToScreenPoint(this.transform.TransformPoint(thisMeshFilter.mesh.vertices[0])).y - 
				Camera.main.WorldToScreenPoint(this.transform.TransformPoint(thisMeshFilter.mesh.vertices[1])).y;
		}
		
		//Make sure the mesh offset is non-negative
		if(MESH_VERT_OFFSET < 0) MESH_VERT_OFFSET *= -1;
		
		//Testing, output the offset
		Debug.LogWarning("Mesh vertice offset = " + MESH_VERT_OFFSET.ToString());
	}
	
	/// <summary>
	/// Traverses the grid add to new piece...
	/// </summary>
	/// <param name='pos'>
	/// Position.
	/// </param>
	/// <param name='currentlyInTransition'>
	/// Currently in transition.
	/// </param>
	/// <param name='listIndice'>
	/// List indice.
	/// </param>
	/// <param name='tempList'>
	/// Temp list.
	/// </param>
	/// <param name='rowTearStartPos'>
	/// Row tear start position.
	/// </param>
	/// <param name='endPosTearStartPos'>
	/// End position tear start position.
	/// </param>
	/// <param name='readyToDeterminShape'>
	/// Ready to determin shape.
	/// </param>
	/// <param name='startingVertIndice'>
	/// Starting vert indice.
	/// </param>
	
	private void TraverseGridAddToNewPiece(Vector3 origPos, Vector2 pos, bool currentlyInTransition, int listIndice, 
		List<Vector3> tempList, Vector3 startTearPos, Vector3 endPosTearPos, bool readyToDeterminShape,
		int startingVertIndice)
	{
		bool debugging = false;
		 
		
		//Determine if we are currently rtransitioning
		if(tearLine.ContainsKey(origPos))
		{
			if(debugging) Debug.LogError("currentlyInTransition island 1 and 2");
			currentlyInTransition = true;
		}
		
		//Check which island we are currently adding to
		if(addingToPieceOne && !currentlyInTransition)
		{
			if(debugging)Debug.LogError("island1");
			island1.Add (pos);
		}
		else if(!addingToPieceOne && !currentlyInTransition)
		{
			if(debugging)Debug.LogError("island2");
			island2.Add (pos);
		}
		//Here we add the vertice to both lists for new both new meshes
		else if(currentlyInTransition)
		{
			//Keep track of positions for tearing FX
			tearVertEdgeFXpos.Add (origPos);
			
			//Add to both new islands
			island1.Add (pos);
			island2.Add (pos);
			
			if(readyToDeterminShape)
			{
				bool sShape = (TearLineMovingInSLikeShape(startTearPos, endPosTearPos, listIndice, startingVertIndice, tempList));
				
				//Make sure the tearLine is not makeing a 'U-like' turn
				if(sShape) 
				{
					//Rotate between the two new islands
					if(addingToPieceOne) addingToPieceOne = false;
					else addingToPieceOne = true;
				}
			}
			
			//Now we know we are no longer in a transition
			currentlyInTransition = false;
		}
	}
	
	/// <summary>
	/// This function is used to flag whether or not the tearLine is making an S-like
	/// shape, and not a U-like shape
	/// </summary>
	/// <returns>
	/// The line moving in S like shape.
	/// </returns>
	private bool TearLineMovingInSLikeShape(Vector3 startTearPos, Vector3 endTearPos, int listIndice, int startingVertIndice, List<Vector3> tempList)
	{
		bool debugging = false;
		
		//The value to be returned
		bool returnVal = false;
		
		float distCheck = endTearPos.x - startTearPos.x;
		if(distCheck < 0) distCheck *= -1;
		
		//Find the time associated wiht the end time tear position
		int endTime = (int)tearLineTime[endTearPos];
		
		//Find the time associated wiht the start time tear position
		int startTime = (int)tearLineTime[startTearPos];
		
		//We determine if the time is switched in the given row to determine which way to look when
		//find ing which way the durve is moving
		bool timeSwitched = false;
		int sTime = (int)tearLineTime[startTearPos];
		int eTime = (int)tearLineTime[endTearPos];
		
		
		bool StartEqualsEndOnEdgeFlag = false;
		
		//Debug.LogError("starting tear pos = (" + startTearPos.x.ToString() + ", " + startTearPos.y.ToString() + ", " + startTearPos.z.ToString() + ") with starting time = " + sTime.ToString());
		//Debug.LogError("ending tear pos = (" + endTearPos.x.ToString() + ", " + endTearPos.y.ToString() + ", " + endTearPos.z.ToString() + ") with starting time = " + eTime.ToString());
		
		//The folling switchs expectation of time dependent upon how the player cutt which edge when and where
		if(sTime > eTime) 
		{
			//if((startTearPos.x == WIDTH_MIN && endTearPos != startTearPos) || startTearPos.x != WIDTH_MIN)
			if((MinWidthBoundsCheck(startTearPos) && endTearPos != startTearPos) || !MinWidthBoundsCheck(startTearPos))
			{
				if(debugging) Debug.LogError("********TIME SWITCHED********");
				timeSwitched = true;
			}
		}
		
		//Find y-component of curve coming into starting position
		float StartChangeInY = startTearPos.y;
		
		if(!StartEqualsEndOnEdgeFlag)
		{
			StartChangeInY = ChangeInHeightTearVerticeDir(startTearPos, true, timeSwitched);
		}
		
		///Find y-component of curve leaving ending position
		float EndChangeInY = ChangeInHeightTearVerticeDir(endTearPos, false, timeSwitched);//endTearPos.y;
		
		/*
		if(((StartChangeInY > startTearPos.y && EndChangeInY < endTearPos.y) ||
			(StartChangeInY < startTearPos.y && EndChangeInY > endTearPos.y))
			&& !MinWidthBoundsCheck(startTearPos) 
			&& !MinWidthBoundsCheck(endTearPos))
		{
			if(debugging) Debug.LogWarning("Normal S-Shape");
			returnVal = true;
		}
		else if(((StartChangeInY > startTearPos.y && EndChangeInY > endTearPos.y) ||
			(StartChangeInY < startTearPos.y && EndChangeInY < endTearPos.y))
			&& !MinWidthBoundsCheck(startTearPos) 
			&& !MinWidthBoundsCheck(endTearPos))
		{
			if(debugging) Debug.LogWarning("Normal U-Shape");
			returnVal = false;
		}*/
		
		//Here we are checking is the current decision involves an adge vertice
		//if(endTearPos.y == HEIGHT_MAX || endTearPos.y == HEIGHT_MIN) 
		
		//else 
		if((HeightVerticeBoundsCheck(endTearPos)))// || HeightVerticeBoundsCheck(startTearPos)))
		{
			if(debugging) Debug.LogWarning("HeightVerticeBoundsCheck) S-Shape");
			returnVal = true;
		}
		
		//else if(startTearPos.x == WIDTH_MIN && endTearPos.x == WIDTH_MIN && EndChangeInY > endTearPos.y)
		else if(MinWidthBoundsCheck(startTearPos) && MinWidthBoundsCheck(endTearPos) && EndChangeInY > endTearPos.y)
		{
			if(debugging) Debug.LogError("**************MinWidthBoundsCheck(endTearPos) && EndChangeInY > endTearPos.y -> U-Shape");
			returnVal = true;

		}
		//else if(startTearPos.x == WIDTH_MIN && endTearPos.x == WIDTH_MIN && EndChangeInY < endTearPos.y)
		else if(MinWidthBoundsCheck(startTearPos) && MinWidthBoundsCheck(endTearPos) && EndChangeInY < endTearPos.y)
		{
			if(debugging) Debug.LogWarning("Normal-EDGE (start == end) S-Shape");
			returnVal = true;
		}
		
		//else if(startTearPos.x == WIDTH_MIN && endTearPos.x != WIDTH_MIN && EndChangeInY > endTearPos.y)
		else if(MinWidthBoundsCheck(startTearPos) && !MinWidthBoundsCheck(endTearPos) && EndChangeInY > endTearPos.y)
		{
			if(debugging) Debug.LogWarning("Normal-EDGE (End != Start) U-Shape");
			returnVal = false;
		}
		//else if(startTearPos.x == WIDTH_MIN && endTearPos.x != WIDTH_MIN && EndChangeInY < endTearPos.y)
		else if(MinWidthBoundsCheck(startTearPos) && !MinWidthBoundsCheck(endTearPos) && EndChangeInY < endTearPos.y)
		{
			if(debugging) Debug.LogWarning("Normal-EDGE (End != Start)  S-Shape");
			returnVal = true;
		}
		
		else if((StartChangeInY > startTearPos.y && EndChangeInY > endTearPos.y) ||
			(StartChangeInY < startTearPos.y && EndChangeInY < endTearPos.y))
		{
			if(debugging) Debug.LogWarning("Normal U-Shape");
			returnVal = false;
		}
		else if((StartChangeInY > startTearPos.y && EndChangeInY < endTearPos.y) ||
			(StartChangeInY < startTearPos.y && EndChangeInY > endTearPos.y))
		{
			if(debugging) Debug.LogWarning("Normal S-Shape");
			returnVal = true;
		}
		
		else
		{
			if(StartChangeInY == startTearPos.y && EndChangeInY > endTearPos.y)
			{
				if(debugging) Debug.LogWarning("**************77**************");
				returnVal = true;
			}	
			else if(StartChangeInY == startTearPos.y && EndChangeInY < endTearPos.y)
			{
				if(debugging) Debug.LogWarning("**************88**************");
				returnVal = true;
			}
			else if(EndChangeInY == endTearPos.y && !MaxWidthBoundsCheck(endTearPos))// && endTearPos.y != WIDTH_MAX)
			{
				//Debug.LogWarning("**************99**************");
				if(edgeOfObject.ContainsKey(new Vector3(endTearPos.x, endTearPos.y - MESH_VERT_OFFSET, endTearPos.z))
					&& edgeOfObject[new Vector3(endTearPos.x, endTearPos.y - MESH_VERT_OFFSET, endTearPos.z)])
				{
					if(debugging) Debug.LogWarning("**************911**************");
					returnVal = true;
				}
				else
				{
					if(debugging) Debug.LogWarning("**************411**************");
					returnVal = false;
				}
			}
			else
			{
				if(debugging) Debug.LogWarning("*****************************NO DECISION REACHED AT SHAPE*********************************");
			}
		}
		
		return returnVal;
	}
	
	
	/// <summary>
	/// Determined the change in height from the next/previous teart vertices in tearline dependent
	/// upon whether or not startVertFlag is true
	/// Determines the slope, which in turn relutles in determining s vs. u like shape in tear line
	/// </summary>
	private float ChangeInHeightTearVerticeDir(Vector3 tornVertPos, bool startFlag, bool timeSwitched)
	{
		//The value to be returned
		float returnVal = 0.0f;
		
		int timeOfCurrent = (int)tearLineTime[tornVertPos];
		
		//If we are looking at the starting position, we know which way to look based off 
		if(startFlag)
		{
			//If time is switched, we know that the startingPosition has a greater time than the endingPosition,
			//therefore, we access the directions by using the timescale backwards
			if(timeSwitched)
			{
				
				Vector3 futureVert;
				if(tearLinePositionTime.ContainsKey((float)(timeOfCurrent + 1)))
				{
					futureVert = tearLinePositionTime[(float)(timeOfCurrent + 1)];
				}
				else
				{
					futureVert = tornVertPos;
				}
				returnVal = futureVert.y;
			}
			else
			{
				Vector3 prevVert;
				if(tearLinePositionTime.ContainsKey((float)(timeOfCurrent - 1)))
				{
					prevVert = tearLinePositionTime[(float)(timeOfCurrent - 1)];
				}
				else
				{
					prevVert = tornVertPos;
				}
				returnVal = prevVert.y;
			}
		}
		else
		{
			//If time is switched, we know that the startingPosition has a greater time than the endingPosition,
			//therefore, we access the directions by using the timescale backwards
			if(timeSwitched)
			{
				//Debug.LogError("testingEndCgangeCheck1");
				Vector3 prevVert1;
				if(tearLinePositionTime.ContainsKey((float)(timeOfCurrent - 1)))
				{
					//Debug.LogError("testingEndCgangeCheck11");
					prevVert1 = tearLinePositionTime[(float)(timeOfCurrent - 1)];
				}
				else
				{
					//Debug.LogError("testingEndCgangeCheck12");
					prevVert1 = tornVertPos;
				}
				returnVal = prevVert1.y;
			}
			else
			{
				//Debug.LogError("testingEndCgangeCheck2");
				Vector3 futureVert1;
				if(tearLinePositionTime.ContainsKey((float)(timeOfCurrent + 1)))
				{
					futureVert1 = tearLinePositionTime[(float)(timeOfCurrent + 1)];
				}
				else
				{
					futureVert1 = tornVertPos;
				}
				returnVal = futureVert1.y;
			}
		}
		
		//Return the new value
		return returnVal;
	}
	
	/// <summary>
	/// Creats the new cutt paper mesh pieces.
	/// </summary>
	/// <param name='index'>
	/// Index.
	/// </param>
	private void CreateNewCuttPaperMeshPieces(int index)
	{
		Vector3 testPos  = gameObject.transform.TransformPoint(CuttPieceOneVerts[CuttPieceOneFaces[index]]);
		Vector3 testPos1 = gameObject.transform.TransformPoint(CuttPieceOneVerts[CuttPieceOneFaces[index + 1]]);
		Vector3 testPos2 = gameObject.transform.TransformPoint(CuttPieceOneVerts[CuttPieceOneFaces[index + 2]]);
		
		//Now we create each new vertice array for each mewMesh
		if( island2.Contains(new Vector2(testPos.y, testPos.x)) && 
			island2.Contains(new Vector2(testPos1.y, testPos1.x)) && 
			island2.Contains(new Vector2(testPos2.y, testPos2.x))
			//&&
			//!island1.Contains(new Vector2(testPos.y, testPos.x)) && 
			//!island1.Contains(new Vector2(testPos1.y, testPos1.x)) && 
			//!island1.Contains(new Vector2(testPos2.y, testPos2.x))
			)
		{
			//Debug.LogError("Currently adding the following to island 2*********: " + CuttPieceOneVerts[CuttPieceOneFaces[index]].ToString() + " , " + CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].ToString() + " , " + CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].ToString());
			island1Indicies[indexor1] = CuttPieceOneFaces[index];
			island1Indicies[indexor1 + 1] = CuttPieceOneFaces[index + 1];
			island1Indicies[indexor1 + 2] = CuttPieceOneFaces[index + 2];
			indexor1 += 3;	
				
		}
		else
		{
			//Debug.LogError("Currently adding the following to island 1: " + CuttPieceOneVerts[CuttPieceOneFaces[index]].ToString() + " , " + CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].ToString() + " , " + CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].ToString());
			island2Indicies[indexor2] = CuttPieceTwoFaces[index];
			island2Indicies[indexor2 + 1] = CuttPieceTwoFaces[index + 1];
			island2Indicies[indexor2 + 2] = CuttPieceTwoFaces[index + 2];
			indexor2 += 3;
		}
		
		
		if(!PlatformPaper && !CloneObject)
		{
			if(
				   island2.Contains(new Vector2(testPos.y, testPos.x))
				&& island1.Contains(new Vector2(testPos.y, testPos.x))
				&& !tearVertEdgeFXposFinal.Contains(testPos)
				)
			{
				tearVertEdgeFXposFinal.Add(testPos);
			}
			if(
				   island2.Contains(new Vector2(testPos1.y, testPos1.x))
				&& island1.Contains(new Vector2(testPos1.y, testPos1.x))
				&& !tearVertEdgeFXposFinal.Contains(testPos1)
				)
			{
				tearVertEdgeFXposFinal.Add(testPos1);
			}
			if(
				   island2.Contains(new Vector2(testPos2.y, testPos2.x))
				&& island1.Contains(new Vector2(testPos2.y, testPos2.x))
				&& !tearVertEdgeFXposFinal.Contains(testPos2)
				)
			{
				tearVertEdgeFXposFinal.Add(testPos2);
			}
		}
		
	}

	/// <summary>
	/// Sets the edge verts of object.
	/// </summary>
	private void SetEdgeVertsOfObject()
	{
		//The following represents the amount of faces a vertice must be 
		//associated with in order to be cosidered an interior vertice
		int numberOfEdgesAssociatedWithInteriorFace = MaxTrianglesAssociatedWithEdgeVertice;
		
		//This is used to optimize finding which vertices are edges
		//For each vertice, the number of faces in mesh associated is stored
		//to determine interior versus edge vertices
		Dictionary<Vector3, int> faceCounter = new Dictionary<Vector3, int>();
		
		int meshVertCount = mesh.vertices.Count();
		int meshTriCount = mesh.triangles.Count();
		int[] meshTris = mesh.triangles;
		Vector3[] meshVerts = mesh.vertices;
		
		//Loop through the object mesh
		for(int index2 = 0; index2 < meshVertCount; index2++)
		{
			//Foreach vertice in mesh, create placement within stored 
			//keeping track of number of faces per vertice in mesh
			faceCounter.Add(this.transform.TransformPoint(meshVerts[index2]), 0);
		}
			
		//Loop through each face
		for(int index3 = 0; index3 < meshTriCount; index3++)
		{
			//Foreach vertice in face, add to the storeage keeping track of how many faces
			//are associated with each vertice
			faceCounter[this.transform.TransformPoint(meshVerts[meshTris[index3]])] += 1;
		}
		
		List<Vector3> faceCounterKeys = faceCounter.Keys.ToList();
		int faceCounterKeysCount = faceCounterKeys.Count;
		//Loop through each vertice again and determine whether or not edge or interior
		for(int index4 = 0; index4 < faceCounterKeysCount; index4++)
		{
			bool edgeVertice = false;
			
			//If the current vertice being examined does not have six faces associated with it
			//then, we know that the current vertice is an edge vertice
			if(faceCounter[faceCounterKeys[index4]/*faceCounter.Keys.ElementAt(index4)*/] <= numberOfEdgesAssociatedWithInteriorFace 
				&& !(faceCounter[faceCounterKeys[index4]/*faceCounter.Keys.ElementAt(index4)*/] <= 0))
			{
				edgeVertice = true;
			}
			
			//Here we add the current vertice to the dictionary, and whether or not it is an edge versus interior
			edgeOfObject.Add(faceCounterKeys[index4]/*faceCounter.Keys.ElementAt(index4)*/, edgeVertice);//this.transform.TransformPoint(mesh.vertices.ElementAt(index4))
		}
		
		faceCounter.Clear();
	}
	
	/// <summary>
	/// The following return true if the vertice being testing is an edge vertice
	/// </summary>
	private bool VerticeEdgeCheck(Vector3 testVert)
	{
		//Check is the vetice is an edge and return true if so (assuming edgeOfObject has been created properly)
		if((testVert.x >= MaxWorldWidth || 
			testVert.y >= MaxWorldHeight || 
			testVert.x <= MinWorldWidth || 
			testVert.y <= MinWorldHeight)
			||
			edgeOfObject[testVert])
		{
			return true;	
		}
		else
		{
			return false;
		}
	}
	
	/// <summary>
	/// This function is used to test whether or not the current vertice is a 
	/// max or min height for the current colomn being examined in the current row
	/// </summary>
	private bool HeightVerticeBoundsCheck(Vector3 testVert)
	{
		//We check if the testVert contains a vertice above AND below in the paperGrid
		if(paperGrid.Keys.Contains(testVert.y + MESH_VERT_OFFSET) && 
			!paperGrid.Keys.Contains(testVert.y - MESH_VERT_OFFSET))
		{
			return true;
		}
		else if(!paperGrid.Keys.Contains(testVert.y + MESH_VERT_OFFSET) && 
			paperGrid.Keys.Contains(testVert.y - MESH_VERT_OFFSET))
		{
			return true;
		}
		else if(paperGrid.Keys.Contains(testVert.y + MESH_VERT_OFFSET) && 
			paperGrid.Keys.Contains(testVert.y - MESH_VERT_OFFSET))
		{
			return false;
		}
		
		/*
		 if(paperGrid.Keys.Contains(testVert.y + MESH_VERT_OFFSET) && 
			!paperGrid.Keys.Contains(testVert.y - MESH_VERT_OFFSET))
		{
			return true;
		}
		else if(!paperGrid.Keys.Contains(testVert.y + MESH_VERT_OFFSET) && 
			paperGrid.Keys.Contains(testVert.y - MESH_VERT_OFFSET))
		{
			return true;
		}
		else if(paperGrid.Keys.Contains(testVert.y + MESH_VERT_OFFSET) && 
			paperGrid.Keys.Contains(testVert.y - MESH_VERT_OFFSET))
		{
			//here we know that a colomn above and below testVertice exist, however, this
			//does not imply that there exists a vertice directly below with the same x-value
			
			if(paperGrid[testVert.y + MESH_VERT_OFFSET].Contains(new Vector3(testVert.x, testVert.y + MESH_VERT_OFFSET, testVert.z))
				&& paperGrid[testVert.y - MESH_VERT_OFFSET].Contains(new Vector3(testVert.x, testVert.y - MESH_VERT_OFFSET, testVert.z)))
			{
				return false;
			}
			
			
		}
		 */
		//Here, since either a vertice in the current colomn does
		//not exist above And/OR below, then we know that testVert is
		//an edge vertice on the tearable object
		return true;
		
	}
	
	
	/// <summary>
	/// This function is used to test whether or not the current vertice is a 
	/// max or min height for the current colomn being examined in the current row
	/// </summary>
	private bool HeightVerticeBoundsCheckSLikeShape(Vector3 testVert)
	{
		//We check if the testVert contains a vertice above AND below in the paperGrid
		
		if(paperGrid.Keys.Contains(testVert.y + MESH_VERT_OFFSET) && 
			!paperGrid.Keys.Contains(testVert.y - MESH_VERT_OFFSET))
		{
			return true;
		}
		else if(!paperGrid.Keys.Contains(testVert.y + MESH_VERT_OFFSET) && 
			paperGrid.Keys.Contains(testVert.y - MESH_VERT_OFFSET))
		{
			return true;
		}
		else if(paperGrid.Keys.Contains(testVert.y + MESH_VERT_OFFSET) && 
			paperGrid.Keys.Contains(testVert.y - MESH_VERT_OFFSET))
		{
			//here we know that a colomn above and below testVertice exist, however, this
			//does not imply that there exists a vertice directly below with the same x-value
			
			if(paperGrid[testVert.y + MESH_VERT_OFFSET].Contains(new Vector3(testVert.x, testVert.y + MESH_VERT_OFFSET, testVert.z))
				&& paperGrid[testVert.y - MESH_VERT_OFFSET].Contains(new Vector3(testVert.x, testVert.y - MESH_VERT_OFFSET, testVert.z)))
			{
				return false;
			}
			
			
		}
		 
		//Here, since either a vertice in the current colomn does
		//not exist above And/OR below, then we know that testVert is
		//an edge vertice on the tearable object
		return true;
		
	}
	
	
	/// <summary>
	/// This function is used to test whether or not the current vertice is a 
	/// min width for the current colomn being examined in the current row being examined
	/// </summary>
	private bool MinWidthBoundsCheck(Vector3 testVert)
	{		
		if(testVert.x >= MaxWorldWidth || testVert.x <= MinWorldWidth)
		{
			return true;
		}
		else
		{
			return false;		
		}
		
		
		//We check if the testVert contains a vertice previously to the current
		//if(paperGrid[testVert.y].Contains(new Vector3(testVert.x - MESH_VERT_OFFSET, testVert.y, testVert.z)))
		if(paperGrid.ContainsKey(testVert.y) && paperGrid[testVert.y].ElementAt(0) != testVert)
		{
			//Here, we know that the vertice is not a minimun bounds if 
			//there exists a previous vertice in current row
			return false;
		}
		else
		{
			//Here, we know since there doesn't exist a vertice previously, 
			//we return true because the current vertice is a minimum bound
			return true;
		}
	}

	
	/// <summary>
	/// This function is used to test whether or not the current vertice is a 
	/// max width for the current colomn being examined in the current row being examined
	/// </summary>
	private bool MaxWidthBoundsCheck(Vector3 testVert)
	{
		//We check if the testVert contains a vertice previously to the current
		//if(paperGrid[testVert.y].Contains(new Vector3(testVert.x + MESH_VERT_OFFSET, testVert.y, testVert.z))
		//	&& !edgeOfObject[new Vector3(testVert.x + MESH_VERT_OFFSET, testVert.y, testVert.z)])
		if(paperGrid[testVert.y].ElementAt(paperGrid[testVert.y].Count() - 1) != testVert)
		{
			//Here, we know that the vertice is not a max bounds if 
			//there exists a previous vertice in current row
			return false;
		}
		else
		{
			//Here, we know since there doesn't exist a vertice previously, 
			//we return true because the current vertice is a max bound
			return true;
		}
	}
	
	#endregion
}
