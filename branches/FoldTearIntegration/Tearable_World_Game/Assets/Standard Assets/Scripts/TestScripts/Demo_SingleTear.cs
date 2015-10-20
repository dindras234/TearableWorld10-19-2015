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

public class Demo_SingleTear : MonoBehaviour 
{

	
	#region Public Fields

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
    /// Made this public so I could access this in InputManager - D.A.
    /// This is used to flag once a cutt has started to stop detecting for edge vertice
    /// </summary>
    public bool cuttInProgress = false;
	
	/// <summary>
	/// The end goal object, upon collision, the player wins level
	/// </summary>
	public GameObject EndGoal;
	
	/// <summary>
	/// The bad tear splash screen object
	/// </summary>
	public GameObject BadTearSplash;
	
	/// <summary>
	/// The bad tear splash screen object
	/// </summary>
	public GameObject BadTearSplash1;
	/// <summary>
	/// The bad tear splash screen object
	/// </summary>
	public GameObject BadTearSplash2;


	/// <summary>
	/// The new paper, object being cloned intitially, only
	/// </summary>
	public GameObject newPaper;
	
	/// <summary>
	/// The mesh represents the paper world's mesh being manipulated
	/// </summary>
	public Mesh mesh;
	
	/// <summary>
	/// The decal object used for visually representing a tear
	/// </summary>
	public GameObject DecalObject;
	
	/// <summary>
	/// The clone object represents whether or not this specific instance 
	/// of the world is to be modified, the original version of the object,
	/// we do not modify, only its clones.
	/// </summary>
	public bool CloneObject;
	
	/// <summary>
	/// The tearable object not largest piece of paper.
	/// </summary>
	public bool TearableObjectNotLargestPieceOfPaper;
	
	/// <summary>
	/// The draw tear line ONLY - TESTING PURPOSES!!!!!!!!!!
	/// </summary>
	public bool DrawTearLineONLY;
	
	/// <summary>
	/// THe following is soley for creating DEMO with single tear
	/// </summary>
	public int NUMBER_OF_TEARS_ALOUD = 1;
	
	public GameObject Platform1;
	public GameObject Platform2;
	public GameObject Platform3;
	public GameObject Platform4;
	private List<GameObject> paperPlatforms;
	
	#endregion
	
	#region Variables
	
	private bool currentlyNeedToDrawSplashBadTear = false;
	
	private int splashScreenTimer = 0;
	
	private int TOTAL_SPLASH_TIME= 90;
	
	private int badCuttType = -1;
	
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
	/// The decal dictionary storeing object represting the tearline
	/// </summary>
	private Dictionary<GameObject, int> decalDictionary;
	
	/// <summary>
	/// This is completely determinant upon what plane mesh is being used (THIS IS CURRENTLY HARDCODED)
	/// TODO: READ IN MESH AND ASSIGN, DO NOT HARDCODE THIS VALUE :-(
	/// </summary>
	public float MESH_VERT_OFFSET;
	
	/// <summary>
	/// The screen point is used to store the position being 
	/// compared to an input screen position
	/// </summary>
	private Vector3 screenPoint;
	
	/// <summary>
	/// The clone is used for keeping track of the cloned instance for mesh 
	/// deformation and cutting for new 'island' of mesh vertices
	/// </summary>
	private GameObject Clone;
	
	/// <summary>
	/// The clone_2 is used for keeping track of the cloned instance for mesh 
	/// deformation and cutting for new 'island' of mesh vertices
	/// </summary>
	private GameObject Clone_2;
	
	/// <summary>
	/// The width_ max stores the maximum x value of a vertex within the paper mesh object
	/// </summary>
	private float WIDTH_MAX;
	
	/// <summary>
	/// The width_ minimum stores the minimun x value of a vertex within the paper mesh object
	/// </summary>
	private float WIDTH_MIN;
	
	/// <summary>
	/// The width_ max stores the maximum y value of a vertex within the paper mesh object
	/// </summary>
	private float HEIGHT_MAX;
	
	/// <summary>
	/// The width_ minimum stores the minimun y value of a vertex within the paper mesh object
	/// </summary>
	private float HEIGHT_MIN;
	
	/// <summary>
	/// The new mesh represents the new mesh of the object after a cutt occurs
	/// </summary>
	Vector3[] newMesh;
	
	/// <summary>
	/// The tear line defined to store the vertices the playing is trying to tear along
	/// </summary>
	private Dictionary<Vector3, int> tearLine;
	
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
	/// The paper grid organizes the mesh into a grid like structure for determineing torn pieces of paper
	/// </summary>
	private Dictionary<Vector3, bool> edgeOfObject;
	
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
	private List<Vector2> island1;
	
	/// <summary>
	/// The island_1 list stores the vertices assocciated with the first 'island' of mesh faces
	/// </summary>
	private List<Vector2> island2;
	
	/// <summary>
	/// The adding to piece one flags between both new mesh object for correct new face assignment
	/// </summary>
	private bool addingToPieceOne;
	
	/// <summary>
	/// The adding to piece one flags between both new mesh object for correct new face assignment - Previous island checking
	/// </summary>
	private bool addingToPieceOnePreviously;
	
	
	/// <summary>
	/// The done calculating cutt line flag is used to determine when the player is currently cutting
	/// </summary>
	private bool doneCalculatingCuttLine = false;
	
	/// <summary>
	/// The color of the original tearable object
	/// The color changes when torn object is being moved
	/// </summary>
	private Color originalColor;
	
	/// <summary>
	/// The mouse tear positions stored as the player is performing tear gesture
	/// </summary>
	private List<Vector3> mouseTearPositions;
	
	/// <summary>
	/// The new piece object is used for duplication is convex shapes occur
	/// </summary>
	private GameObject newPiece;
	
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
	/// The player has touched paper this tear flags whether the player has started their tear on the paper object
	/// </summary>
	private bool playerHasTouchedPaperThisTear = false;
	
	/// <summary>
	/// The gap posiitons in the mousePositions list, used for Lerping when input is too fast
	/// </summary>
	private List<int> gapPositions;
	
	/// <summary>
	/// The number mouse positions in mousePositions list
	/// </summary>
	private int numMousPos = 0;
	
	/// <summary>
	/// The player's input previous position, this is used to keeping track
	/// and determining whether the mouse in moving rapidly
	/// </summary>
	private Vector3 playerInputPrevPos;
	
	/// <summary>
	/// The current cutt piece of the paper world
	/// </summary>
	public GameObject CurrentCuttPiece;
	
	//private List<Vector3> edgeVertsForcedTorn;

	
	#endregion
	
	#region Methods - Built In Defined
	/// <summary>
	/// Use this for initialization
	/// </summary>
	private void Start () 
	{
        // assign touch controller for android
        touchController = GameObject.FindGameObjectWithTag("MainObject").GetComponent<TouchController>();
		gameStateManagerRef = GameObject.FindGameObjectWithTag("MainObject").GetComponent<GameStateManager>();
        gameStateManagerRef = GameObject.FindGameObjectWithTag("MainObject").GetComponent<GameStateManager>();
        inputManagerRef = GameObject.FindGameObjectWithTag("MainObject").GetComponent<InputManager>();

		originalColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		
		paperPlatforms = new List<GameObject>();
		paperPlatforms.Add (Platform1);
		paperPlatforms.Add (Platform2);
		paperPlatforms.Add (Platform3);
		paperPlatforms.Add (Platform4);
		//Update platofrms attacked to sutt piece
		if(paperPlatforms.Count() > 0 && NeedToUpdatePlatforms)
		{
			foreach(GameObject go in paperPlatforms)
			{
				//Debug.LogError("TESTING");
				if(Clone.GetComponent<MeshFilter>().mesh.triangles.Contains(go.GetComponent<Demo_PlatformMovement>().Indexer))
				{
					go.transform.Rotate(new Vector3(0, 0, go.GetComponent<Demo_PlatformMovement>().AngleOffset));
				}
				else
				{
					go.transform.Rotate(new Vector3(0, 0, go.GetComponent<Demo_PlatformMovement>().AngleOffset));
				}
			}
		}

		newPiece = newPaper;
		
		if(GetComponent<MeshFilter>().mesh.vertices[0].x != GetComponent<MeshFilter>().mesh.vertices[1].x)
		{
			MESH_VERT_OFFSET = this.transform.TransformPoint(GetComponent<MeshFilter>().mesh.vertices[0]).x - this.transform.TransformPoint(GetComponent<MeshFilter>().mesh.vertices[1]).x;
		}
		else
		{
			MESH_VERT_OFFSET = this.transform.TransformPoint(GetComponent<MeshFilter>().mesh.vertices[0]).y - this.transform.TransformPoint(GetComponent<MeshFilter>().mesh.vertices[1]).y;
		}
		if(MESH_VERT_OFFSET < 0) MESH_VERT_OFFSET *= -1;
		
		//Initially, we set the paper's screen position to be zero
		screenPoint = new Vector3(0.0f, 0.0f, 0.0f);
		
		//Initialize the newMesh to be the same length of the previous
		newMesh = new Vector3[mesh.vertices.Length];
		
		//Create a clone world mesh for deformation, we do not change the original world gameobject mesh
		//if (!CloneObject) CreateNewPaperWorld();
		
		//Make sure the offset is positive
		if(MESH_VERT_OFFSET < 0) 
		{
			MESH_VERT_OFFSET *= -1;
		}
		//Testing, output the offset
		//Debug.LogWarning("Mesh vertice offset = " + MESH_VERT_OFFSET.ToString());
		
		//Initialize the max and min world values of mesh mertex coordinates
		WIDTH_MAX = -100;
	 	WIDTH_MIN = 100;
		HEIGHT_MAX = -100;
		HEIGHT_MIN = 100;
		
		SetBoundsOfPaper();
		
		//init the dictionary storing the vertices along the tear line and their associated index
		//into the the mesh.vertice array
		tearLine = new Dictionary<Vector3, int>();
		tearLineTime = new Dictionary<Vector3, float>();
		tearLinePositionTime = new Dictionary<float, Vector3>();
		decalDictionary = new Dictionary<GameObject, int>();
		
		//edgeVertsForcedTorn = new List<Vector3>();
		
		//Set the tearLineTimer to zero initially as the starting tear line 'time'
		tearLineTimer = 0;
		
		SetPaperGrid();
		
		
		
		int testListCount = 1;
		foreach(List<Vector3> list in paperGrid.Values)
		{
			//Debug.LogError("list #" + testListCount.ToString());
			
			int testPosCount = 1;
			//foreach(Vector3 pos in list)
			{
				//Debug.LogError("Adding new vert #" + testPosCount + " to newPaperGrid = " + pos.ToString());
				++testPosCount;
			}
			++testListCount;
		}
		//Create a clone world mesh for deformation, we do not change the original world gameobject mesh
		if (!CloneObject) CreateNewPaperWorld();
		
		
		//Find the distance between any two adjacent points on mesh
		//(assumming mesh has even distribution of vertices)
		// This is used to know where neighbor vertices are located
		
		//MESH_VERT_OFFSET = paperGrid.Values.ElementAt(0).ElementAt(0).x - paperGrid.Values.ElementAt(0).ElementAt(1).x;
		
		
		
		//Debug.Log("MESH_VERT_OFFSET = " + MESH_VERT_OFFSET.ToString());
	}


	
	/// <summary>
	/// This flag is used to iterate through mesh, checking for convex curvature along edge, 
	/// iff and exists, object is split until convex edges no longer exist
	/// </summary>
	private bool haveCheckedForConvexEdges = false;
	
	/// <summary>
	/// The offset of the player input position to world corrdinates
	/// </summary>
	private Vector3 offset;
	
	/// <summary>
	/// The center of cutt piece - for rotating properly
	/// </summary>
	private Vector3 centerOfCuttPieceRotOffset;
	
	/// <summary>
	/// The currently moving cutt piece flags when the player is moving the cutt piece after performing a tear
	/// </summary>
	private bool currentlyMovingCuttPiece = false;
	
	/// <summary>
	/// The new rotation object used for translation of cutt piece after tear
	/// </summary>
	private GameObject newRotationObject;
	
	private bool NeedToUpdatePlatforms = false;

   
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	private void Update () 
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
                Debug.Log("TOUCH CONTROLLER IS NULL");
        }

        // else by default store it on the pc side using Input.Mouse
        else
        {
            if (currentPlayerInputPos != Input.mousePosition)
            {
                currentPlayerInputPos = Input.mousePosition;
            }
        }

		float scaleFactor = 0.93f;
		if(decalDictionary != null)
		{
			if(decalDictionary.Keys.Count() > 0)
			{
				for(int jtor = 0; jtor < decalDictionary.Keys.Count(); jtor++)
				{
					++decalDictionary[decalDictionary.Keys.ElementAt(jtor)];
					decalDictionary.Keys.ElementAt(jtor).transform.localScale = new Vector3(decalDictionary.Keys.ElementAt(jtor).transform.localScale.x * scaleFactor, decalDictionary.Keys.ElementAt(jtor).transform.localScale.y * scaleFactor, decalDictionary.Keys.ElementAt(jtor).transform.localScale.z);//new Vector3(0.01f, 0.01f, 0.0f);
					if(decalDictionary[decalDictionary.Keys.ElementAt(jtor)] > 30)
					{
						GameObject.Destroy(decalDictionary.Keys.ElementAt(jtor));
						decalDictionary.Remove(decalDictionary.Keys.ElementAt(jtor));
					}
				}
			}
		}

       

		//If we are done calculating the cutt line, then a tear was just successfully calculated
        if (doneCalculatingCuttLine)
        {
            //Testing purposes, draws the tear line only
            if (DrawTearLineONLY)
            {
                //Testing purposes
                DrawTearLineOnly();
            }
            else
            {
                //for(int itor = 0; itor < tearLine.Count(); itor++)
                {
                    //Debug.LogError("tearline vert #" + itor + " = (" + tearLine.Keys.ElementAt(itor).x.ToString() + ", " + tearLine.Keys.ElementAt(itor).y.ToString() + "," + tearLine.Keys.ElementAt(itor).z.ToString() + ")");
                }
                //Traverse through old mesh to determine two new cutt pieces based off tearline
                FindNewCutPieces();

                if (!BadCuttCheck())
                {
                    --NUMBER_OF_TEARS_ALOUD;

                    //Assign flag for moving cutt piece
                    currentlyMovingCuttPiece = true;

                    //Now we set the center of the cuttObject, used for rotation
                    centerOfCuttPieceRotOffset = FindCenterOfCuttPiece();

                    newRotationObject = new GameObject("CuttPieceTranslationObject");
                    newRotationObject.transform.position = centerOfCuttPieceRotOffset;
                    CurrentCuttPiece.transform.parent = newRotationObject.transform;

                    //Assign varaibles for moving piece
                    screenPoint = Camera.main.WorldToScreenPoint(CurrentCuttPiece.transform.position);
                    offset = centerOfCuttPieceRotOffset - Camera.main.ScreenToWorldPoint(
                        new Vector3(currentPlayerInputPos.x, currentPlayerInputPos.y, currentPlayerInputPos.z));

                    //Set flag to update platform rotation
                    NeedToUpdatePlatforms = true;
                }
                else
                {
                    CurrentCuttPiece.GetComponent<MeshFilter>().renderer.material.color = originalColor;
                    ForceStopTear();
                }
            }

            // reset rotation variables
            rotatingPiece = false;
            initRotatingPiece = false;
        }
        else if (currentlyMovingCuttPiece)
        {
            //Here the cuttPiece needs to be translated where ever the player desires
            MoveCuttPiceToDesiredLocation();
        }
        else if (!currentlyMovingCuttPiece && audio.isPlaying)
        {
            // reset rotation variables
            rotatingPiece = false;
            initRotatingPiece = false;
            audio.Pause();
        }

        if (!inputManagerRef.PlayerSelected())
        {

            //TODO; Convert all input to touch input
            if (!CloneObject && NUMBER_OF_TEARS_ALOUD > 0)
            {
                //UnityEngine.Debug.Log("CUT IN PROGRESS " + cuttInProgress.ToString());
                //If the left mouse button is down and the player is touching off the paper (colliding with dead space), and a cutt is no in progress, then,
                //we call mouse down an set flags for tearing accordingly
                if (inputManagerRef.GetcurrPressState().Equals(InputManager.PressState.JUST_PRESSED) && PlayerTouchingDeadSpace() && !cuttInProgress)
                {
                    //UnityEngine.Debug.Log("MOUSE JUST DOWN");
                    cuttInProgress = true;
                    playerHasTouchedPaperThisTear = false;
                    OnMouseDown();
                }

                //If the left mouse is dragging and the player has correctly initiated a tear, and we are not forceint the tear to stop
                else if ((inputManagerRef.GetcurrPressState().Equals(InputManager.PressState.DOWN) || inputManagerRef.GetcurrPressState().Equals(InputManager.PressState.JUST_PRESSED)) && cuttInProgress)
                {
                    // UnityEngine.Debug.Log("MOUSE DRAGGING");
                    OnMouseDrag();
                }

                //If the left mouse button is up, and the player is touching the dead space once again, and a cutt is in progress, then we call onMouseUp
                else if (inputManagerRef.GetcurrPressState().Equals(InputManager.PressState.JUST_RELEASED) && PlayerTouchingDeadSpace() && cuttInProgress)
                {
                    //UnityEngine.Debug.Log("MOUSE UP");
                    OnMouseUp();
                }
            }

            //Update platofrms attacked to sutt piece
            if (paperPlatforms.Count() > 0 && NeedToUpdatePlatforms)
            {
                foreach (GameObject go in paperPlatforms)
                {
                    //Debug.LogError("TESTING");
                    if (Clone.GetComponent<MeshFilter>().mesh.triangles.Contains(go.GetComponent<Demo_PlatformMovement>().Indexer))
                    {
                        go.GetComponent<Demo_PlatformMovement>().myPivotPoint.transform.rotation = Clone.transform.rotation;
                    }
                    else
                    {
                        go.GetComponent<Demo_PlatformMovement>().myPivotPoint.transform.rotation = Clone_2.transform.rotation;
                    }
                }
            }
        }
		
		if(Input.GetKeyDown("'"))
		{
			Application.LoadLevel(Application.loadedLevel);
		}
		
		if(currentlyNeedToDrawSplashBadTear)
		{
			if(badCuttType == 0)
			{
				if(!BadTearSplash.GetComponent<MeshRenderer>().enabled)
				{
					BadTearSplash.GetComponent<MeshRenderer>().enabled = true;
				}
			}
			else if(badCuttType == 1)
			{
				if(!BadTearSplash1.GetComponent<MeshRenderer>().enabled)
				{
					BadTearSplash1.GetComponent<MeshRenderer>().enabled = true;
				}
			}
			else if(badCuttType == 2)
			{
				if(!BadTearSplash2.GetComponent<MeshRenderer>().enabled)
				{
					BadTearSplash2.GetComponent<MeshRenderer>().enabled = true;
				}
			}
			++splashScreenTimer;
			if(splashScreenTimer > TOTAL_SPLASH_TIME)
			{
				currentlyNeedToDrawSplashBadTear = false;
				splashScreenTimer = 0;
				if(badCuttType == 0)
				{
					if(BadTearSplash.GetComponent<MeshRenderer>().enabled)
					{
						BadTearSplash.GetComponent<MeshRenderer>().enabled = false;
					}
				}
				else if(badCuttType == 1)
				{
					if(BadTearSplash1.GetComponent<MeshRenderer>().enabled)
					{
						BadTearSplash1.GetComponent<MeshRenderer>().enabled = false;
						Application.LoadLevel(Application.loadedLevel);
					}
				}
				else if(badCuttType == 2)
				{
					if(BadTearSplash2.GetComponent<MeshRenderer>().enabled)
					{
						BadTearSplash2.GetComponent<MeshRenderer>().enabled = false;
						Application.LoadLevel(Application.loadedLevel);
					}
				}
				badCuttType = 0;
			}
		}
		
	}
	
	#region Mouse Input Methods

	/// <summary>
	/// Raises the mouse down event.
	/// </summary>
	private void OnMouseDown()
	{
		//Debug.Log ("ENTERING ON MOUSE DOWN");
		//Init new list for storage
		mouseTearPositions = new List<Vector3>();

        // save position of where the player pressed on the screen
        inputDownPlayerPos = currentPlayerInputPos;
		
		//Testing to know where we are in logic
		//Debug.Log("Enter MouseDown");
		
		//Save information for later use
		if(!cuttInProgress && NUMBER_OF_TEARS_ALOUD > 0 && !CloneObject)
		{
			//Save old mesh information
			originalMeshVerts = Clone.GetComponent<MeshFilter>().mesh.vertices;//mesh.vertices;
			originalMeshTriangles = Clone.GetComponent<MeshFilter>().mesh.triangles;//mesh.triangles;
		}
	}
	
	
	/// <summary>
	/// Raises the mouse drag event.
	/// </summary>
	void OnMouseDrag()
	{
        
		//Initialize list storing gap indexes
		if(gapPositions == null)
		{
			gapPositions = new List<int>();
            playerInputPrevPos = currentPlayerInputPos;
		}

		//We check to see if the player is touching an edge
		if(cuttInProgress)
		{
            Vector3 curMousePos = Camera.main.ScreenToWorldPoint(new Vector3(currentPlayerInputPos.x, currentPlayerInputPos.y, 
				Camera.main.WorldToScreenPoint(Clone.transform.TransformPoint(Clone.GetComponent<MeshFilter>().mesh.vertices.ElementAt(0))).z));
			Vector3 prevMousePos = Camera.main.ScreenToWorldPoint(new Vector3(playerInputPrevPos.x, playerInputPrevPos.y, 
				Camera.main.WorldToScreenPoint(Clone.transform.TransformPoint(Clone.GetComponent<MeshFilter>().mesh.vertices.ElementAt(0))).z));
			
			//Play audio for tearing
			if(!GameObject.FindGameObjectWithTag("MainCamera").audio.isPlaying && curMousePos.x < WIDTH_MAX && curMousePos.x > WIDTH_MIN &&
				curMousePos.y < HEIGHT_MAX && curMousePos.y > HEIGHT_MIN)
			{
				GameObject.FindGameObjectWithTag("MainCamera").audio.Play();
			}
			
			if(GameObject.FindGameObjectWithTag("MainCamera").audio.isPlaying && (curMousePos.x > WIDTH_MAX || curMousePos.x < WIDTH_MIN ||
				curMousePos.y > HEIGHT_MAX || curMousePos.y < HEIGHT_MIN))
			{
				GameObject.FindGameObjectWithTag("MainCamera").audio.Pause();
			}
			
			curMousePos.z = 0.001f;
			prevMousePos.z = 0.001f;
			
			if(Vector3.Distance(curMousePos, prevMousePos) > MESH_VERT_OFFSET/100 && curMousePos.x < WIDTH_MAX && curMousePos.x > WIDTH_MIN &&
				curMousePos.y < HEIGHT_MAX && curMousePos.y > HEIGHT_MIN)
			{
				//create decal, start timer and add to dictionary
				GameObject newDecal = (GameObject)Instantiate(DecalObject, curMousePos, Quaternion.identity);
				newDecal.transform.Rotate(new Vector3(0, 180, 0));
				newDecal.transform.position = new Vector3(newDecal.transform.position.x, newDecal.transform.position.y, -0.1f);
				decalDictionary.Add(newDecal, 0);
			}
			
			//Debug.Log("DRAG");
			//Get the distance from previous mouseposition to current mouseposition
			float dist = Vector3.Distance(Camera.main.ScreenToWorldPoint(new Vector3(playerInputPrevPos.x, playerInputPrevPos.y, 
				Camera.main.WorldToScreenPoint(Clone.transform.TransformPoint(Clone.GetComponent<MeshFilter>().mesh.vertices.ElementAt(0))).z)),
                Camera.main.ScreenToWorldPoint(new Vector3(currentPlayerInputPos.x, currentPlayerInputPos.y, 
				Camera.main.WorldToScreenPoint(Clone.transform.TransformPoint(Clone.GetComponent<MeshFilter>().mesh.vertices.ElementAt(0))).z)));
			
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
					Camera.main.ScreenToWorldPoint(new Vector3(playerInputPrevPos.x, playerInputPrevPos.y, 
						Camera.main.WorldToScreenPoint(Clone.transform.TransformPoint(Clone.GetComponent<MeshFilter>().mesh.vertices.ElementAt(0))).z)),
                    Camera.main.ScreenToWorldPoint(new Vector3(currentPlayerInputPos.x, currentPlayerInputPos.y, 
						Camera.main.WorldToScreenPoint(Clone.transform.TransformPoint(Clone.GetComponent<MeshFilter>().mesh.vertices.ElementAt(0))).z)), 
					dist);
				
			}
			
			float depth = Camera.main.WorldToScreenPoint(Clone.transform.TransformPoint(Clone.GetComponent<MeshFilter>().mesh.vertices.ElementAt(0))).z;
			
			//When mouse is draging, add input to list
			mouseTearPositions.Add(new Vector3(currentPlayerInputPos.x, currentPlayerInputPos.y, depth));
			
			//Keep track of the mouse's previous position
			playerInputPrevPos = currentPlayerInputPos;
		}
		else
		{
			//This statment should be hit is the UI starts a tear but does not end on an edge properly
			//Debug.LogError("Salty Ballz");
		}
	}
	
	/// <summary>
	/// Raises the mouse up event.
	/// </summary>
	void OnMouseUp()
	{
        //Debug.Log("MOUSE UP");
        // saves the position of where the player released his input
        inputUpPlayerPos = currentPlayerInputPos;

		//Now, we check to see if the player is touching an edge to complete their cutt/tear
		if(cuttInProgress)
		{
			if(GameObject.FindGameObjectWithTag("MainCamera").audio.isPlaying)
			{
				GameObject.FindGameObjectWithTag("MainCamera").audio.Stop();
			}
			
			//This is used to flag loops or back tracking within tear line
			bool badInputDetected = false;
			
			//Transform mouseInput into torn vertice list (tearLine)
			badInputDetected = TurnInputIntoTornVertCurve();
			
			if(badInputDetected)
			{
				badCuttType = 0;
				Debug.LogError("BAD CUTT");
				ForceStopTear();
				return;
			}
			
			//the following deletes excess edge torn vertices
			DeleteExcessEdgeVertices();
			
			//RemoveProblematicSubRegionsOfTear();
			ForceHorizontalCuttAlongEdge();
			
			//Now, we make sure every row that should be torn is torn
			ForceHorizontalCuttFirstRowVert();
			
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
        if (cuttInProgress)
        {
            //UnityEngine.Debug.Log("CUT DONE");
            cuttInProgress = false;
        }
		
		//Reset forceStopTear for next tear check
		if(forceStopTear) forceStopTear = false;
		
		
		//if(tearLine.ContainsKey(paperGrid[0][0]))
		{
			//Debug.LogError("))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))) " + paperGrid.Keys.ElementAt(0).ToString());
		}
	}
	
	#endregion
	
	
	#endregion
	
	#region Defined Methods
	
	private bool BadCuttCheck()
	{
		//Player distance
		Vector2 playerPos = new Vector2(GameObject.FindGameObjectWithTag("Player").transform.position.x, GameObject.FindGameObjectWithTag("Player").transform.position.y);
		
		//end goal distance
		Vector2 endGoalPos = new Vector2(EndGoal.transform.position.x, EndGoal.transform.position.y);
		
		//Store closest distance
		float closestDist = 100000;
		
		//check find closest distance
		for(int itor = 0; itor < CurrentCuttPiece.GetComponent<MeshFilter>().mesh.triangles.Length; ++itor)
		{
			Vector2 curVertPos = new Vector2(CurrentCuttPiece.GetComponent<MeshFilter>().mesh.vertices[CurrentCuttPiece.GetComponent<MeshFilter>().mesh.triangles[itor]].x,
										CurrentCuttPiece.GetComponent<MeshFilter>().mesh.vertices[CurrentCuttPiece.GetComponent<MeshFilter>().mesh.triangles[itor]].y);
			float curDist = Vector2.Distance(playerPos, curVertPos);
			if(curDist < 0) curDist *= -1;
			
			if(curDist < closestDist)
			{
				closestDist = curDist;
				badCuttType = 1;
			}
		}
		for(int itor = 0; itor < CurrentCuttPiece.GetComponent<MeshFilter>().mesh.triangles.Length; ++itor)
		{
			Vector2 curVertPos = new Vector2(CurrentCuttPiece.GetComponent<MeshFilter>().mesh.vertices[CurrentCuttPiece.GetComponent<MeshFilter>().mesh.triangles[itor]].x,
										CurrentCuttPiece.GetComponent<MeshFilter>().mesh.vertices[CurrentCuttPiece.GetComponent<MeshFilter>().mesh.triangles[itor]].y);
			float curDist = Vector2.Distance(endGoalPos, curVertPos);
			if(curDist < 0) curDist *= -1;
			
			if(curDist < closestDist)
			{
				closestDist = curDist;
				badCuttType = 2;
			}
		}
		
		if(closestDist <= MESH_VERT_OFFSET)
		{
			return true;
		}
		
		return false;
	}
	
	/// <summary>
	/// Finds the center of cutt piece.
	/// </summary>
	private Vector3 FindCenterOfCuttPiece()
	{
		//Flag number of vertices 
		int numberOfvertives = 0;
		
		//Create vector to return as object's center position
		Vector3 returnVal = Vector3.zero;
		
		//Create storage to remove duplicate vertice indexex from triangle array
		List<int> VertFaceIndicies = new List<int>();
		
		//Loop through the vertices currently being drawn (determined by triangles currently visible)
		for(int itor = 0; itor < CurrentCuttPiece.GetComponent<MeshFilter>().mesh.triangles.Count(); itor++)
		{
			++numberOfvertives;
			
			//The key here is translating local coordinated into global coordinates by using Transform.TransformPoint()
			returnVal += CurrentCuttPiece.transform.TransformPoint(CurrentCuttPiece.GetComponent<MeshFilter>().mesh.vertices[CurrentCuttPiece.GetComponent<MeshFilter>().mesh.triangles[itor]]);
		}
		//Divide by number of vertices to find center position
		returnVal /= numberOfvertives;
		
		return returnVal;
	}


    /// <summary>
    /// Method that determines if a point is within
    /// the bounds of a game object
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

   

    private double AngleBetweenPoints(Vector2 point2, Vector2 point1)
    {
        double angle = (Math.Atan2(point2.y - point1.y, point2.x - point1.x) * 180 / Math.PI);

        if (angle >= -180 && angle <= -90)
            UnityEngine.Debug.Log("ANGLE BETWEEN -90 & -180");

        else if (angle >= -90 && angle <= 0)
            UnityEngine.Debug.Log("ANGLE BETWEEN -0 & -90");

        else if (angle >= 0 && angle <= 90)
            UnityEngine.Debug.Log("ANGLE BETWEEN 0 & 90");

        else 
            UnityEngine.Debug.Log("ANGLE BETWEEN 90 & 180");
        //if ((Math.Atan2(point2.y - point1.y, point2.x - point1.x) * 180 / Math.PI) < 0)
        //    angle = 180 - (Math.Atan2(point2.y - point1.y, point2.x - point1.x));
        //else
            angle = (Math.Atan2(point2.y - point1.y, point2.x - point1.x));

        //if (angle < 0)
        //    angle = 360 - Math.Abs(angle);
        return angle * -8;
    }

    Vector3 prevPos = new Vector3();
    Vector3 curScreenPoint = new Vector3();
    Vector3 curPosition = new Vector3();
    bool rotatingPiece = false;
    bool initRotatingPiece = false;

    public bool GetRotatingPiece()
    {
        return rotatingPiece;
    }

    public void SetRotatingPiece(bool value)
    {
        rotatingPiece = value;
    }

    public bool GetMovingPiece()
    {
        return currentlyMovingCuttPiece;
    }

    public bool InitRotatingPiece()
    {
        return initRotatingPiece;
    }

    private Vector3 PaperZDepthVec(Vector2 vec)
    {
        return new Vector3(vec.x, vec.y, Camera.mainCamera.WorldToScreenPoint(CurrentCuttPiece.transform.position).z);
    }

    Vector3 prevDist = new Vector3();
    Vector2 origPos = new Vector2();
    float overallRot = 0;
	/// <summary>
	/// Moves the cutt pice to desired location, and rotation
	/// </summary>
	private void MoveCuttPiceToDesiredLocation()
	{
		int test = 10;
        //if (PointInsideObject(CurrentCuttPiece, touchController.GetLastFingerPosition()))
        {
            curScreenPoint = new Vector3(currentPlayerInputPos.x, currentPlayerInputPos.y, screenPoint.z);
            curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);// + offset;
            curPosition.z = CurrentCuttPiece.GetComponent<MeshFilter>().mesh.vertices.ElementAt(0).z - 0.001f;
        }
        
		
		//only move if the mouse is within bounds of the paper
		if((curPosition.x <= WIDTH_MAX && curPosition.x >= WIDTH_MIN &&
			curPosition.y <= HEIGHT_MAX && curPosition.y >= HEIGHT_MIN && 
            !gameStateManagerRef.OnMobileDevice()) ||
            (PointInsideObject(CurrentCuttPiece, touchController.GetLastFingerPosition()) &&
             !rotatingPiece &&
             gameStateManagerRef.OnMobileDevice()))
		{
			//Move the cutt piece
	    	newRotationObject.transform.position = curPosition;
			//centerOfCuttPieceRotOffset = curPosition;
		}

        //UnityEngine.Debug.Log("CURR " + inputManagerRef.GetcurrPressState().ToString() + "\nPREV " + inputManagerRef.GetprevPressState() + "\nTOUCH" + touchController.ReturnTouchType().ToString());

        
		//Need to check for the condition making the player's decision finalized.
        if (
            // using the mouse on the pc, 
            // needs to have the checks for platform, 
            // Input.GetMouse.. will still be called even when touching on device
            (Input.GetMouseButtonDown(0) && !gameStateManagerRef.OnMobileDevice()) ||

            // or if on tablet, looking for double tap
             ((inputManagerRef.DoubleTap() || touchController.ReturnTouchType().Equals(TouchType.MULTITOUCH_2)) &&
              PointInsideObject(CurrentCuttPiece, touchController.GetLastFingerPosition()) &&
              gameStateManagerRef.OnMobileDevice())
            )
        {
            currentlyMovingCuttPiece = false;
            NeedToUpdatePlatforms = false;
            CurrentCuttPiece.GetComponent<MeshRenderer>().material.color = originalColor;
            rotatingPiece = false;
            initRotatingPiece = false;
        }

       
		//The following changes the rotation of the cutt pice
		//TODO: Tranform into touch input!
        // TODONE... D.A.
        else if (Input.GetMouseButton(1) ||
            ((!PointInsideObject(CurrentCuttPiece, touchController.GetLastFingerPosition()) || rotatingPiece) &&
            inputManagerRef.GetcurrPressState().Equals(InputManager.PressState.DOWN) &&
            gameStateManagerRef.OnMobileDevice()))
        {
            //UnityEngine.Debug.Log("ROTATING PAPER\n" + touchController.GetLastFingerPosition().ToString());

            // having a global variable
            // to know when the piece is rotating
            rotatingPiece = true;

            centerOfCuttPieceRotOffset = FindCenterOfCuttPiece();

            // ensuring init rotating piece
            // is only true for one loop iteration
            if (gameStateManagerRef.OnMobileDevice())
            {
                
                double rotation = Math.Atan2(touchController.GetLastFingerPosition().y - Camera.mainCamera.WorldToScreenPoint(centerOfCuttPieceRotOffset).y,
                                             Camera.mainCamera.WorldToScreenPoint(centerOfCuttPieceRotOffset).x - touchController.GetLastFingerPosition().x) * 180 / Math.PI;

                // atan2 returns values from -180 to 180
                if (rotation < 0)
                    rotation = 360 + rotation;


                if (initRotatingPiece)
                    overallRot = (float)rotation;
                
                CurrentCuttPiece.transform.RotateAround(centerOfCuttPieceRotOffset, new Vector3(0, 0, 1), (float)(overallRot - rotation));

                // update the overall rotation
                overallRot = (float)rotation;


                //UnityEngine.Debug.Log("ROT " + rotation);
                //UnityEngine.Debug.Log("PAPER " + Camera.mainCamera.WorldToScreenPoint(centerOfCuttPieceRotOffset).ToString() + "\nFINGER " + touchController.GetLastFingerPosition().ToString());
                //UnityEngine.Debug.Log("LOOK " + ((rotation.z - CurrentCuttPiece.transform.rotation.z) * 180 / Math.PI).ToString() + "\nCURR " + currRot); 
                //UnityEngine.Debug.Log("DIFF " + ((rotation.z - CurrentCuttPiece.transform.rotation.z) * 180 / Math.PI).ToString()); 
            }

            else
                CurrentCuttPiece.transform.RotateAround(centerOfCuttPieceRotOffset, new Vector3(0, 0, 1), test);

          
            if (!audio.isPlaying)
            {
                audio.Play();
            }
        }

        
		
		if(!Input.GetMouseButton(1) && audio.isPlaying)
		{
			if(audio.isPlaying)
			{
				audio.Pause();
			}
		}
		
	}
	
	private int testingFrequency = 0;
	/// <summary>
	/// This function is called once after startup or after a tear/cut to
	/// check for convex slope along mesh edge. If any exist, this
	/// function is used to duplicate the object and create non-convex shapes
	/// </summary>
	private bool CheckForConvexMesh()
	{
		//This flag triggers true when a convex shape is discovered in mesh
		bool convexFlag = false;
		
		//loop through the current mesh
		for(int itor = 0; itor < paperGrid.Keys.Count(); itor++)
		{
			//loop through the current mesh row
			for(int jtor = 0; jtor < paperGrid[paperGrid.Keys.ElementAt(itor)].Count(); jtor++)
			{
				//Debug.LogError("**************Checking distance****************");
				if(jtor > 0)
				{
					Vector3 distanceCheck = paperGrid[paperGrid.Keys.ElementAt(itor)].ElementAt(jtor) - paperGrid[paperGrid.Keys.ElementAt(itor)].ElementAt(jtor - 1);
					
					if(distanceCheck.x < 0) distanceCheck.x *= -1;
					if(distanceCheck.y < 0) distanceCheck.y *= -1;
					
					if(distanceCheck.x <= MESH_VERT_OFFSET && distanceCheck.y <= MESH_VERT_OFFSET)
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
				for(int jtor = 0; jtor < paperGrid[paperGrid.Keys.ElementAt(itor)].Count(); jtor++)
				{
					//Debug.LogError("********perform Convex Test********");
					//check is we aree past first position is row
					if(jtor > 0)
					{
						Vector3 distanceCheck = paperGrid[paperGrid.Keys.ElementAt(itor)].ElementAt(jtor) - paperGrid[paperGrid.Keys.ElementAt(itor)].ElementAt(jtor - 1);
						
						if(distanceCheck.x < 0) distanceCheck.x *= -1;
						if(distanceCheck.y < 0) distanceCheck.y *= -1;
						
						if(!haveSwitchThisRow && distanceCheck.x <= MESH_VERT_OFFSET && distanceCheck.y <= MESH_VERT_OFFSET)
						{
							isl1.Add(paperGrid[paperGrid.Keys.ElementAt(itor)].ElementAt(jtor));
						}
						
						else if(!haveSwitchThisRow)
						{
							
							haveSwitchThisRow = true;
						}
						else if(haveSwitchThisRow)
						{
							isl2.Add(paperGrid[paperGrid.Keys.ElementAt(itor)].ElementAt(jtor));
						}
					}
					else
					{
						//First poistion is row, always island 1
						isl1.Add(paperGrid[paperGrid.Keys.ElementAt(itor)].ElementAt(jtor));
					}
				}
			}
			
			CuttPieceOneFaces = Clone.GetComponent<MeshFilter>().mesh.triangles;
			CuttPieceTwoFaces = Clone.GetComponent<MeshFilter>().mesh.triangles;
			CuttPieceOneVerts = Clone.GetComponent<MeshFilter>().mesh.vertices;
			CuttPieceTwoVerts = Clone.GetComponent<MeshFilter>().mesh.vertices;
			
			int[] isl1Ind = new int[CuttPieceOneFaces.Length];
			int[] isl2Ind = new int[CuttPieceOneFaces.Length];
			
			indexor1 = 0;
			indexor2 = 0;
			
			for(int index = 0; index < CuttPieceOneFaces.Length; index += 3)
			{
				Vector3 testPos = this.transform.TransformPoint(CuttPieceOneVerts[CuttPieceOneFaces[index]]);
				Vector3 testPos1 = this.transform.TransformPoint(CuttPieceOneVerts[CuttPieceOneFaces[index + 1]]);
				Vector3 testPos2 = this.transform.TransformPoint(CuttPieceOneVerts[CuttPieceOneFaces[index + 2]]);
				
				
				//Now we create each new vertice array for each mewMesh
				if( isl1.Contains(testPos) && 
					isl1.Contains(testPos1) && 
					isl1.Contains(testPos2))
				{
					//Debug.LogError("Currently adding the following to island 2*********: " + CuttPieceOneVerts[CuttPieceOneFaces[index]].ToString() + " , " + CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].ToString() + " , " + CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].ToString());
					isl1Ind[indexor1] = CuttPieceOneFaces[index];
					isl1Ind[indexor1 + 1] = CuttPieceOneFaces[index + 1];
					isl1Ind[indexor1 + 2] = CuttPieceOneFaces[index + 2];
					indexor1 += 3;	
						
				}
				else
				{
					isl2Ind[indexor2] = CuttPieceTwoFaces[index];
					isl2Ind[indexor2 + 1] = CuttPieceTwoFaces[index + 1];
					isl2Ind[indexor2 + 2] = CuttPieceTwoFaces[index + 2];
					indexor2 += 3;
				}
			}
			
			newPiece = (GameObject)Instantiate(newPiece, this.transform.position, this.transform.rotation);
			newPiece.GetComponent<Demo_SingleTear>().CloneObject = true;
			newPiece.name = "Poopy #" + testingFrequency.ToString();
			
			newPiece.GetComponent<MeshFilter>().mesh.triangles = isl1Ind;
			newPiece.GetComponent<MeshCollider>().sharedMesh = newPiece.GetComponent<MeshFilter>().mesh;
			this.GetComponent<MeshFilter>().mesh.triangles = isl2Ind;
			this.GetComponent<MeshCollider>().sharedMesh = this.GetComponent<MeshFilter>().mesh;
			
			//haveCheckedForConvexEdges = false;
			
			Debug.LogError("********About to Set paper grid********");
			SetPaperGrid();
			
		}
		else
		{
			Debug.LogError("********CONVEX = FALSE********");
		}
		Debug.LogError("********CONVEX = " + convexFlag.ToString());
		return convexFlag;
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
						Camera.mainCamera.WorldToScreenPoint(newPos).y, Camera.mainCamera.WorldToScreenPoint(newPos).z));//currentPlayerInputPos.
			
		}
	}
	
	/// <summary>
	/// This function takes in the object's mesh to create 
	/// an organized, grid like, structure
	/// </summary>
	private void SetPaperGrid()
	{
		paperGridTearVertCheck = new Dictionary<Vector2, bool>();
		paperGrid = new Dictionary<float, List<Vector3>>();
		
		//Load grid dictionary
		for(int itor = 0; itor < this.GetComponent<MeshFilter>().mesh.vertices.Length; itor++)
		{
			//New world vertice location 
			//Vector3 point = this.transform.TransformPoint(new Vector3(mesh.vertices[itor].x, mesh.vertices[itor].y, mesh.vertices[itor].z));
			Vector3 point = this.transform.TransformPoint(this.GetComponent<MeshFilter>().mesh.vertices[itor]);
			
			//Make sure the vertice index is referenced by triangles (i.e. onyle add visible vertices)
			if(this.GetComponent<MeshFilter>().mesh.triangles.Contains(itor))
			{
			
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
						Vector3 node1 = list[jtor];
						Vector3 node2 = list[itor];
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
		
		for(int itor = 0; itor < tempList122.Count(); itor++)
		{
			for(int jtor = 0; jtor < tempList122.Count(); ++jtor)
			{
				if(tempList122[jtor] > tempList122[itor])
				{
					//Then we swap
					float node1 = tempList122[jtor];
					float node2 = tempList122[itor];
					tempList122[jtor] = node2;
					tempList122[itor] = node1;
				}
			}
		}
		
		
		
		
		
		//Now that we have a list representing the sorted keys,
		Dictionary<float, List<Vector3>> newPaperGrid = new Dictionary<float, List<Vector3>>();
		for(int indexr = 0; indexr < tempList122.Count; indexr++)
		{
			List<Vector3> newList2 = paperGrid[tempList122.ElementAt(indexr)];
			float index2 = tempList122.ElementAt(indexr);
			//Debug.LogError("Adding to newPaperGrid");
			
			newPaperGrid.Add(index2, newList2);
		}
		//Assign the papergrid to the new grid with organized rows
		paperGrid = newPaperGrid;
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
		//Loop through mesh and find/set the bounds
		for(int itor = 0; itor < mesh.vertices.Length; itor++)
		{
			if(this.transform.TransformPoint(mesh.vertices[itor]).x > WIDTH_MAX)
			{
				WIDTH_MAX = this.transform.TransformPoint(mesh.vertices[itor]).x;
			}
			else if(this.transform.TransformPoint(mesh.vertices[itor]).x < WIDTH_MIN)
			{
				WIDTH_MIN = this.transform.TransformPoint(mesh.vertices[itor]).x;
			}
			
			if(this.transform.TransformPoint(mesh.vertices[itor]).y > HEIGHT_MAX)
			{
				HEIGHT_MAX = this.transform.TransformPoint(mesh.vertices[itor]).y;
			}
			else if(this.transform.TransformPoint(mesh.vertices[itor]).y < HEIGHT_MIN)
			{
				HEIGHT_MIN = this.transform.TransformPoint(mesh.vertices[itor]).y;
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
		Clone = (GameObject) Instantiate(newPaper, this.transform.position, this.transform.rotation);
		
		//Set clone informaiton
		Clone_2 = (GameObject) Instantiate(newPaper, this.transform.position, this.transform.rotation);
		Clone_2.GetComponent<Demo_SingleTear>().CloneObject = true;
		Clone_2.GetComponent<MeshRenderer>().enabled = false;
		
		//Turn off the original object's meshRenderer to hide
		this.GetComponent<MeshRenderer>().enabled = false;
		this.GetComponent<MeshCollider>().enabled = false;
		
		//Set flag true to start deformations
		Clone.GetComponent<Demo_SingleTear>().CloneObject = true;
		
		//Initialize the storage keeping tracking of edge information
		edgeOfObject = new Dictionary<Vector3, bool>();
		
		//The following determines which vertices are interior and which are edge
		SetEdgeVertsOfObject();
		
		//Initialize the relationships between edge vertices
		//SetEdgeRelationShips();
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
		edgeVerts = new List<Vector3>();
		edgeTriangles = new List<int>();
		
		//Loop through mesh and store edge vertices into list
		for(int itor = 0; itor < edgeOfObject.Keys.Count(); itor++)
		{
			if(edgeOfObject.Values.ElementAt(itor))
			{
				edgeVerts.Add(edgeOfObject.Keys.ElementAt(itor));
				
				//if((MinWidthBoundsCheck(edgeOfObject.Keys.ElementAt(itor)) || MaxWidthBoundsCheck(edgeOfObject.Keys.ElementAt(itor))) 
				//	&& HeightVerticeBoundsCheck(edgeOfObject.Keys.ElementAt(itor)))
				{
				//	Debug.LogError("EDGE VERT HIT");
				}
			}
		}
		Debug.LogError("NUMBER OF EDGE VERTICES = " + edgeVerts.Count().ToString() + " with meshOffset = " + MESH_VERT_OFFSET.ToString());
		
		
		//Now, edgeVerts contains all edge vertices within the object
		
		List<Vector3> organizedEdgeVert = new List<Vector3>();
		organizedEdgeVert.Add(edgeVerts[0]);
		int indexrr = 0;
		
		List<Vector3> potentialEdgeVertNeighbors;
		
		//Now we organize edgeVerts based off proximity
		for(int itor = 0; itor < edgeVerts.Count(); itor++)
			//while(organizedEdgeVert.Count() < edgeVerts.Count())
		{
			potentialEdgeVertNeighbors = new List<Vector3>();
			
			for(int jtor = 0; jtor < edgeVerts.Count(); jtor++)
			{
				float xDist1 = edgeVerts[jtor].x;
				float yDist1 = edgeVerts[jtor].y;
				float xDist2 = organizedEdgeVert[indexrr].x;
				float yDist2 = organizedEdgeVert[indexrr].y;
				
				//if(xDist1 < 0) xDist1 *= -1;
				//if(yDist1 < 0) yDist1 *= -1;
				//if(xDist2 < 0) xDist2 *= -1;
				//if(yDist2 < 0) yDist2 *= -1;
				
				float ydist = yDist2 - yDist1;
				float xdist = xDist2 - xDist1;
				
				if(ydist < 0) ydist *= -1;
				if(xdist < 0) xdist *= -1;
				
				//Debug.LogError("CurrenDistnace x  =  " + xdist.ToString());
				//Debug.LogError("CurrenDistnace y  =  " + ydist.ToString());
				
				if(!organizedEdgeVert.Contains(edgeVerts[jtor]) &&
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
			
			//Now potentialEdgeVertNeighbors conatains all of the potential 
			//neighbors for the current vertice being examined
			for(int ztor = 0; ztor < potentialEdgeVertNeighbors.Count(); ztor++)
			{
				float xDist1 = potentialEdgeVertNeighbors.ElementAt(ztor).x;
				float yDist1 = potentialEdgeVertNeighbors.ElementAt(ztor).y;
				float xDist2 = organizedEdgeVert[indexrr].x;
				float yDist2 = organizedEdgeVert[indexrr].y;
				
				//if(xDist1 < 0) xDist1 *= -1;
				//if(yDist1 < 0) yDist1 *= -1;
				//if(xDist2 < 0) xDist2 *= -1;
				//if(yDist2 < 0) yDist2 *= -1;
				
				float distanceXDir = xDist1 - xDist2;
				if(distanceXDir < 0 )distanceXDir *= -1;
				float distanceYDir = yDist1 - yDist2;
				if(distanceYDir < 0 )distanceYDir *= -1;
				
				if((distanceXDir <= MESH_VERT_OFFSET && distanceYDir == 0) ||
					(distanceXDir  == 0 && distanceYDir <= MESH_VERT_OFFSET)
					)
				{
					//potentialEdgeVertNeighbors.Add(edgeVerts[jtor]);
					organizedEdgeVert.Add(potentialEdgeVertNeighbors.ElementAt(ztor));
					++indexrr;
					break;
				}
			}
		}
		Debug.LogError("NUMBER OF EDGE VERTICES 2 =  " + organizedEdgeVert.Count().ToString());
		edgeVerts = organizedEdgeVert;
		
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
    		Vector3 curPosition = Camera.main.ScreenToWorldPoint(new Vector3(currentPlayerInputPos.x, currentPlayerInputPos.y, screenPoint.z));
			
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
    		Vector3 curPosition = Camera.main.ScreenToWorldPoint(new Vector3(currentPlayerInputPos.x, currentPlayerInputPos.y, screenPoint.z));
			
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
		bool returnVal = false;
		
		//Create raytrace object
		RaycastHit hit = new RaycastHit();
		Ray ray = Camera.main.ScreenPointToRay(currentPlayerInputPos);
		
		//Check raycast
		if(Physics.Raycast(ray, out hit, 1000.0f))
		{
			//If the player collided with the deadspace object, then they are touching the dead space
			if(hit.transform.tag == "DeadSpace")
    		{
				returnVal = true;
			}
		}
		return returnVal;
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
			//Debug.LogError("adding torn vert along edge #" + ktor.ToString() + " new vertice = " + newTornEdgeVerts.ElementAt(ktor).ToString());
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
				(this.GetComponent<MeshFilter>().mesh.vertices[edgeTriangles[itor]] == vert1 ||
				this.GetComponent<MeshFilter>().mesh.vertices[edgeTriangles[itor + 1]] == vert1 ||
				this.GetComponent<MeshFilter>().mesh.vertices[edgeTriangles[itor + 2]] == vert1 ) 
				&&
				(this.GetComponent<MeshFilter>().mesh.vertices[edgeTriangles[itor]] == vert2 ||
				this.GetComponent<MeshFilter>().mesh.vertices[edgeTriangles[itor + 1]] == vert2 ||
				this.GetComponent<MeshFilter>().mesh.vertices[edgeTriangles[itor + 2]] == vert2)
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
						float newTearTime;
						
						bool timeInverted = false;
						
						//Debug.LogError("Currently trying to force tear at " + tearLine.Keys.ElementAt(0).ToString() + " with time = " + tearLineTime[tearLine.Keys.ElementAt(0)].ToString());
						if(tearLineTime[tearLine.Keys.ElementAt(tearLine.Keys.Count() - 1)] > tearLineTime[tearLine.Keys.ElementAt(0)])
						{
							newTearTime = tearLineTime[tearLine.Keys.ElementAt(0)] - 1;
							timeInverted = false;
						}
						else
						{
							newTearTime = tearLineTime[tearLine.Keys.ElementAt(0)] + 1;
							timeInverted = true;
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
						
						if(tearLineTime[tearLine.Keys.ElementAt(indexor)] > tearLineTime[tearLine.Keys.ElementAt(0)])
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
					//Debug.LogError("*********************Edge Vertice Check 1.0*********************");
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
				else if(tearLine.Keys.ElementAt(0).y > tearLine.Keys.ElementAt(1).y)
				{
					//Debug.LogError("*********************Edge Vertice Check 1.1*********************");
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
					//Debug.LogError("*********************Edge Vertice Check 1.3*********************");
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
					tearLine = newTearLine;
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
		
		//First the mouseTearPositions needs to be translated into world positions
		for(int indexr = 0; indexr < mouseTearPositions.Count - 1; indexr++)
		{
			Vector3 curPosition = this.transform.TransformPoint(Camera.main.ScreenToWorldPoint(
				new Vector3(mouseTearPositions[indexr].x, mouseTearPositions[indexr].y, screenDepth.z)));//
			//Vector3 curPosition = Camera.main.ScreenToWorldPoint(mouseTearPositions[indexr]);
			
			newMouseTearWorldPos.Add (curPosition);
		}
		//Re-assign the mouse positions to the 3D positions
		mouseTearPositions = newMouseTearWorldPos;
		
		//This is used to keeping track of the previously torn vertice
		Vector3 previousTornVert; //Clone.GetComponent<MeshFilter>().mesh.vertices[0];
		
		//This keeps track of the number of 
		int numberOfVerts = 0;
		
		//Create structure to keep track of the previous mouse position
        // JOHH ERROR
        Vector3 previousMousePos = Vector2.zero;
        if (mouseTearPositions.Count > 0)
		     previousMousePos = mouseTearPositions.ElementAt(0);
		
		
		float maxWidth = -1;
		float maxHeight = -1;
		float minWidth = 1000;
		float minHeight = 1000;
		//The following is not being used, but is performing sifferent logic based of what type of tearable object this is
		if(TearableObjectNotLargestPieceOfPaper)
		{
			Debug.LogError("********************************************TearableObjectNotLargestPieceOfPaper == true********************************************");
			for(int index = 0; index < mesh.vertices.Count() - 1; index++)
			{
				if(this.transform.TransformPoint(mesh.vertices.ElementAt(index)).x > maxWidth)
				{
					maxWidth = this.transform.TransformPoint(mesh.vertices.ElementAt(index)).x;
				}
				if(this.transform.TransformPoint(mesh.vertices.ElementAt(index)).x < minWidth)
				{
					minWidth = this.transform.TransformPoint(mesh.vertices.ElementAt(index)).x;
				}
				
				if(this.transform.TransformPoint(mesh.vertices.ElementAt(index)).y > maxHeight)
				{
					maxHeight = this.transform.TransformPoint(mesh.vertices.ElementAt(index)).y;
				}
				if(this.transform.TransformPoint(mesh.vertices.ElementAt(index)).y < minHeight)
				{
					minHeight = this.transform.TransformPoint(mesh.vertices.ElementAt(index)).y;
				}
			}
		}
		
		//Loop through the mouseTearPositions
		for(int jtor = 0; jtor < mouseTearPositions.Count - 1; jtor++)
		{
			//Debug.LogError("turning mousposiiton = " + mouseTearPositions[jtor].ToString() + " into a torn vert, distance checking");
			/*
			if(!TearableObjectNotLargestPieceOfPaper || (TearableObjectNotLargestPieceOfPaper && mouseTearPositions[jtor].x < (maxWidth + MESH_VERT_OFFSET*2)
				&& mouseTearPositions[jtor].x < (maxHeight + MESH_VERT_OFFSET*2)
				&& mouseTearPositions[jtor].y > (minWidth - MESH_VERT_OFFSET*2)
				&& mouseTearPositions[jtor].y > (minHeight - MESH_VERT_OFFSET*2)))
				*/
			{
				//Find change in position of the mouse
				float mouseMoveDistX = mouseTearPositions[jtor].x - previousMousePos.x;
				float mouseMoveDistY = mouseTearPositions[jtor].y - previousMousePos.y;
				
				//Make sure the move distance is always positive for flag checking
				if(mouseMoveDistX < 0) mouseMoveDistX *= -1;
				if(mouseMoveDistY < 0) mouseMoveDistY *= -1;
				
				//If jtor == 0, we are at an edge, so we forsure add, else we only add every time
				// the mouse has moved the distance inbetween any two adjacent vertices on paper mesh
				if(jtor == 0 || mouseMoveDistX > (MESH_VERT_OFFSET * 1/5) || mouseMoveDistY > (MESH_VERT_OFFSET * 1/5))
				{
					//Debug.LogWarning("Mouse Has Moved the offset amount to add new vert");
					//Now, we know here we need to find the vert closest to the mouse position to add to the
					// tearLine
					
					float distToClosestRow = MESH_VERT_OFFSET * 20;
					int rowNum = 0;//-1;
					
					//First, loop through grid to find row with closest y-component
					for(int itor = 0; itor < paperGrid.Keys.Count; itor++)
					{
						float tempDist = paperGrid.Keys.ElementAt(itor) - mouseTearPositions[jtor].y;
						if(tempDist < 0) tempDist *= -1;
						
						if(tempDist < distToClosestRow)
						{
							distToClosestRow = tempDist;
							rowNum = itor;
						}
					}
					
					float distToClosestCol = MESH_VERT_OFFSET * 20;
					int colomnNum = 0;//-1;
					
					//Make sure we have valid input
					//if(rowNum != -1)
					{
						//Now, we loop through grid to find vert in colomn with closest x-component
						//Debug.LogWarning("Row Number = " + rowNum.ToString());
						for(int ktor = 0; ktor < paperGrid[paperGrid.Keys.ElementAt(rowNum)].Count; ktor++)//paperGrid[paperGrid.Keys.ElementAt(rowNum)]
						{
							float tempDist2 =  paperGrid[paperGrid.Keys.ElementAt(rowNum)][ktor].x - mouseTearPositions.ElementAt(jtor).x;
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
						distCheck = Vector3.Distance(mouseTearPositions.ElementAt(jtor), paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum]);
					}
					
					
					//Now rowNum and Colomn Num point to the new tornVert, so we add to tearLine list	
					if((distCheck <= MESH_VERT_OFFSET)// && distCheck != -100)
						&&
						(
						(!tearLine.ContainsKey(paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum]) )//&& distCheck != -100
						|| 
							(
							tearLine.ContainsKey(paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum])
							&& tearLineTime[paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum]] != tearLineTimer
							//&& distCheck != -100
							)
						)
						
					  )
					{
						haveFoundGoodVertice = true;
						//Debug.LogError("*****************distCheck <= MESH_VERT_OFFSET**********************");
						tearLineTimer++;
						numberOfVerts++;
						
							
						//THE FOLLOWING NEEDS OPTIMIZATION, here we need to add the indice of the new torn vertice into tearline,
						// therefore the quickest, but slowest solution is to iterate through all mesh verts....
						int meshIndex = -1;
						for(int gtor = 0; gtor < mesh.vertices.Length; gtor++)
						{
							if(this.transform.TransformPoint(mesh.vertices.ElementAt(gtor)) == paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum])
							{
								meshIndex = gtor;
								break;
							}
						}
						
						//The following try will always be called unless a loop is found within curve, in this case, the satch is called to forceStopTear
						try
						{
							tearLine.Add(paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum], meshIndex);
						}
						catch
						{
							//hear a loop is found within the curve, so we FORCE STOP TEAR BECAUSE LOOP FOUND
							//Debug.LogError("**********************FORCE STOP TEAR**************************");
							badCuttType = 0;
							ForceStopTear();
							return true;
						}
						
						//Flag the current vertice to be a torn vetice
						paperGridTearVertCheck[paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum]] = true;
							
						//This maps a 'time' to the tear vertice
						tearLineTime.Add (paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum], (float)tearLineTimer);
						tearLinePositionTime.Add ((float)tearLineTimer, paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum]);
						previousTornVert = paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum];
						returnVal = false;
					}
					else //if(distCheck != -1000)
					{
						returnVal = true;
					}
					
					//Set previous the the position we just looked at
					previousMousePos = mouseTearPositions.ElementAt(jtor);
				}
			}
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
	private void ForceStopTear()
	{
		currentlyNeedToDrawSplashBadTear = true;
		
		Debug.LogError("Now forcing Stop tear");
		//Rest everything needed to reset the states when a bad tear occurs
		cuttInProgress = false;
		doneCalculatingCuttLine = false;
		mouseTearPositions = new List<Vector3>();
		tearLine = new Dictionary<Vector3, int>();
		gapPositions = null;
		playerInputPrevPos = currentPlayerInputPos;
		tearLineTime = new Dictionary<Vector3, float>();
		tearLinePositionTime = new Dictionary<float, Vector3>();
		addingToPieceOne = true;
		tearLineTimer = 1;
		forceStopTear = false;
		haveTouchedOffPaperToStartTear = false;
		playerHasTouchedPaperThisTear = true;
		
		originalMeshVerts = Clone.GetComponent<MeshFilter>().mesh.vertices;//mesh.vertices;
		originalMeshTriangles = Clone.GetComponent<MeshFilter>().mesh.triangles;
	}
	
	/// <summary>
	/// Cutts the paper object along tearline
	/// Before this is called, we already know which vertices are torn, here, we determine which
	/// vertices belong to which new cutt piece
	/// </summary>
	private void FindNewCutPieces()
	{
		//The following initialize the storages holding the new vertices and faces for the two new citt paper objects
		CuttPieceOneVerts = Clone.GetComponent<MeshFilter>().mesh.vertices;
		CuttPieceTwoVerts = Clone.GetComponent<MeshFilter>().mesh.vertices;
		CuttPieceOneFaces = Clone.GetComponent<MeshFilter>().mesh.triangles;
		CuttPieceTwoFaces = Clone.GetComponent<MeshFilter>().mesh.triangles;
		
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
		
		/*
		//
		//Now we know which verticies are selected and how to distinguish both islands thanks to 
		// the organized paper vert grid found previously
		//
		*/
		
		//Now, we create two lists of verticies to determine which vertices belong to which 'island'
		island1 = new List<Vector2>();
		island2 = new List<Vector2>();
		
		//The following is used to switch in between both new meshes for data transfer after cutt/tear
		addingToPieceOne = true;
		
		//This flags is we are currently at a tear vertice to add to both islands
		bool currentlyInTransition = false;
		
		//This is used for testing, printing the number of torn vertices being created
		int numEdgeTearVerts = 0;
		
		//This points to the vertex in the current row where the tear section begins
		int startingVertIndice = 0;
		
		//We looop through the sorted paperGrid to create two new mesh based of tearLine
		for(int iterator = 0; iterator < paperGrid.Values.Count; iterator++)//each(List<Vector3> tempList in paperGrid.Values)
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
			
			
			if(iterator - 1 >= 0)
			{
				//If the current row's first vertex is not torn and the last row's first vertex is torn
				if ((!tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator)[0]) && 
					tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator - 1)[0]))
					//||
					
					//(tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator)[0]) && 
					//tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator - 1)[0]) 
					//&& !edgeVertsForcedTorn.Contains(paperGrid.Values.ElementAt(iterator - 1)[0])
					//&& edgeVertsForcedTorn.Contains(paperGrid.Values.ElementAt(iterator)[0]))
					)
				{
					//We increase the numEdgeTearVerts to keep track of which island we are adding to when
					//the first row's veretex IS NOT a torn vertex
					numEdgeTearVerts++;
				}
			}
			
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
			
			//Iterate through every single value in the current row being observed
			for(int jtor = 0; jtor < paperGrid.Values.ElementAt(iterator).Count; jtor++)
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
				if(tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator)[jtor]) 
					&& !haveHitTearLine)
				{
					//Set flag to true, for proper assignment of ending poisiton in the current row's tear region
					haveHitTearLine = true;
					
					//Assign the starting position of the current row's tear region being traversed.
					startTearPos = paperGrid.Values.ElementAt(iterator)[jtor];
					
					//This is a pointer to the row's list stored within the dictionary for proper direction of curve calculation
					startingVertIndice = jtor;
					
					//This is flagged to false because we are not yet ready to make the decision of which island to transition to
					readyToDeterminShape = false;
				}
				
				//If we have started a tear region
				if(haveHitTearLine)
				{
					float distanceCheck = 0;
					if((jtor + 1) < paperGrid.Values.ElementAt(iterator).Count)
					{
						distanceCheck = paperGrid.Values.ElementAt(iterator)[jtor + 1].x - paperGrid.Values.ElementAt(iterator)[jtor].x;
						if(distanceCheck < 0) distanceCheck *= -1;
					}
					//If we are not at the last position in the row AND 
					//If the next vertex in row in not torn, then we know
					//	we have finally hit the end of one of the current row's
					//	tear regions
					if(((jtor + 1) < paperGrid.Values.ElementAt(iterator).Count && 
						!tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator)[jtor + 1]))
						||
						distanceCheck > MESH_VERT_OFFSET
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
					endTearPos = paperGrid.Values.ElementAt(iterator)[jtor];
				}
				
				/*
				//
				//Now, at this point, we are ready to determine the incoming and outgoing
				//directions of the curve about one of the current row's tear regions
				//
				*/
				
				//In the following, the x==y and y==x already, these components have already been switched
				TraverseGridAddToNewPiece(paperGrid.Values.ElementAt(iterator)[jtor], new Vector2(paperGrid.Values.ElementAt(iterator)[jtor].y, paperGrid.Values.ElementAt(iterator)[jtor].x), currentlyInTransition, 
					jtor, paperGrid.Values.ElementAt(iterator), startTearPos, endTearPos, readyToDeterminShape, startingVertIndice);
			}
		}
		
		//Make the second clone visible
		Clone_2.GetComponent<MeshRenderer>().enabled = true;
		
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
		
		
		
		///<summary>
		///
		/// ********************TODO THE FOLLOWING NEEDS SHOULD BE NEEDED, WHY IS (-1.25, -0.875) being added 2000 times?!?!?!?!?!?!?*******************************
		///
		///</summary>
		
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
		//Set new modified triangles to define mesh faces
		island2Indicies = newIsland2Ind;
		
		///<summary>
		///
		/// ***************************************************
		///
		///</summary>
		
		
		/*
		//**********************************START OF ISLAND 1 LOGIC*****************************************************
		//Keep track of how many vertices are needed for intitialization purposes
		int numIsland1Verts = island1Indicies.Length/3;
		
		//This represents the new vector3 positions array of the
		//torn piece's vertices defining the mesh
		Vector3[] newIsland1VertPositions = new Vector3[numIsland1Verts];
		
		//This is used as an indexer into the vector defined above
		int newIsland1Indexr = 0;
		
		//This stores the old values as a key and key index values as the key value
		Dictionary<int, int> oldToNewFaceIndexLoopUp = new Dictionary<int, int>();
		
		List<int> newIsland1Indicies = new List<int>();
		
		//Loop through new torn piece faces derived above to find which vertices define 
		//the new torn piece
		for(int index = 0; index < island1Indicies.Length; ++index)
		{
			if(!newIsland1Indicies.Contains(island1Indicies[index]))
			{
				newIsland1Indicies.Add(island1Indicies[index]);
				
				//Add the old index information into storage for future comparision
				oldToNewFaceIndexLoopUp.Add(island1Indicies[index], newIsland1Indexr);
				
				newIsland1VertPositions[newIsland1Indexr] = Clone.GetComponent<MeshFilter>().mesh.vertices[island1Indicies[index]];
				
				//Increase the number of vertices found
				++newIsland1Indexr;
			}
		}
		//Now, newIsland1VertPositions is the new vertice array for Clone
		//We have to now modify triangles so that they are indexing the new vertice
		//array correctly.
		//We do this by switching the old with the new index by using the
		//dictionary table loop up : oldToNewFaceIndexLoopUp
		
		//The following represents the new face indicies into the new vertices array defined above
		int[] newFaceIndiciesIsland1 = new int[island1Indicies.Length];
		
		//Loop through the face array
		for(int itor = 0; itor < island1Indicies.Length; ++itor)
		{
			//Switch old reference into vertice array with the new index reference
			newFaceIndiciesIsland1[itor] = oldToNewFaceIndexLoopUp[island1Indicies[itor]];
		}
		//assign new face information
		island1Indicies = newFaceIndiciesIsland1;
		//*************************************END OF ISLAND 1 LOGIC**************************************************
		
		//**********************************START OF ISLAND 2 LOGIC*****************************************************
		//Keep track of how many vertices are needed for intitialization purposes
		int numIsland2Verts = island2Indicies.Length/3;
		
		//This represents the new vector3 positions array of the
		//torn piece's vertices defining the mesh
		Vector3[] newIsland2VertPositions = new Vector3[numIsland2Verts];
		
		//This is used as an indexer into the vector defined above
		int newIsland2Indexr = 0;
		
		//This stores the old values as a key and key index values as the key value
		Dictionary<int, int> oldToNewFaceIndexLoopUp2 = new Dictionary<int, int>();
		
		List<int> newIsland2Indicies = new List<int>();
		
		//Loop through new torn piece faces derived above to find which vertices define 
		//the new torn piece
		for(int index = 0; index < island2Indicies.Length; ++index)
		{
			if(!newIsland2Indicies.Contains(island2Indicies[index]))
			{
				newIsland2Indicies.Add(island2Indicies[index]);
				
				//Add the old index information into storage for future comparision
				oldToNewFaceIndexLoopUp2.Add(island2Indicies[index], newIsland2Indexr);
				
				newIsland2VertPositions[newIsland2Indexr] = Clone_2.GetComponent<MeshFilter>().mesh.vertices[island2Indicies[index]];
				
				//Increase the number of vertices found
				++newIsland2Indexr;
			}
		}
		//Now, newIsland1VertPositions is the new vertice array for Clone
		//We have to now modify triangles so that they are indexing the new vertice
		//array correctly.
		//We do this by switching the old with the new index by using the
		//dictionary table loop up : oldToNewFaceIndexLoopUp
		
		//The following represents the new face indicies into the new vertices array defined above
		int[] newFaceIndiciesIsland2 = new int[island2Indicies.Length];
		
		//Loop through the face array
		for(int itor = 0; itor < island2Indicies.Length; ++itor)
		{
			//Switch old reference into vertice array with the new index reference
			newFaceIndiciesIsland2[itor] = oldToNewFaceIndexLoopUp2[island2Indicies[itor]];
		}
		//assign new face information
		island2Indicies = newFaceIndiciesIsland2;
		//*************************************END OF ISLAND 2 LOGIC**************************************************
		
		//Reassign new mesh triangles, defining the new faces for the cloned object
		Clone.GetComponent<MeshFilter>().mesh.vertices = newIsland1VertPositions;
		Clone_2.GetComponent<MeshFilter>().mesh.vertices = newIsland2VertPositions;
		*/
		
		
		//Reassign new mesh triangles, defining the new faces for the cloned object
		Clone.GetComponent<MeshFilter>().mesh.triangles = island1Indicies;
		Clone_2.GetComponent<MeshFilter>().mesh.triangles = island2Indicies;
		
		//Update the clone's mesh collider
		Clone.GetComponent<MeshCollider>().sharedMesh = Clone.GetComponent<MeshFilter>().mesh;
		Clone_2.GetComponent<MeshCollider>().sharedMesh = Clone_2.GetComponent<MeshFilter>().mesh;

		//Rename the object based on the number of faces in each of the new meshs
		if(island2.Count > island1.Count)
		{
			Clone_2.name = "paper_CuttPieceOfPaper";
			Clone.name = "paper_LargerPiece";
			originalColor = Clone_2.GetComponent<MeshRenderer>().material.color;
			Clone_2.GetComponent<MeshRenderer>().material.color = new Color(0.7f, 0.9f, 1.0f, 1.0f);
			
			CurrentCuttPiece = Clone_2;
			//for(int itor = 0; itor < island2Indicies.Count();++itor)
			{
				//Debug.LogError("CuttPiece #" + itor.ToString() + " has a vertice index = " + island2Indicies[itor].ToString());
				//Debug.LogError("CuttPiece #" + itor.ToString() + " has a vertice = " + CurrentCuttPiece.GetComponent<MeshFilter>().mesh.vertices[island1Indicies[itor]].x.ToString() + ", " + CurrentCuttPiece.GetComponent<MeshFilter>().mesh.vertices[island1Indicies[itor]].y.ToString());
			}
		}
		else
		{
			Clone.name = "paper_CuttPieceOfPaper";
			Clone_2.name = "paper_LargerPiece";
			originalColor = Clone_2.GetComponent<MeshRenderer>().material.color;
			Clone.GetComponent<MeshRenderer>().material.color = new Color(0.7f, 0.9f, 1.0f, 1.0f);
			
			CurrentCuttPiece = Clone;
			
			
			//for(int itor = 0; itor < island1Indicies.Count();++itor)
			{
				//Debug.LogError("CuttPiece #" + itor.ToString() + " has a vertice index = " + island1Indicies[itor].ToString());
				//Debug.LogError("CuttPiece #" + itor.ToString() + " has a vertice = " + CurrentCuttPiece.GetComponent<MeshFilter>().mesh.vertices[island1Indicies[itor]].x.ToString() + ", " + CurrentCuttPiece.GetComponent<MeshFilter>().mesh.vertices[island1Indicies[itor]].y.ToString());
			}
			
			//for(int itor = 0; itor < CurrentCuttPiece.GetComponent<MeshFilter>().mesh.triangles.Count();++itor)
			{
				//Debug.LogError("CuttPiece #" + itor.ToString() + " has a vertice = " + CurrentCuttPiece.GetComponent<MeshFilter>().mesh.vertices[CurrentCuttPiece.GetComponent<MeshFilter>().mesh.triangles[itor]].x.ToString() + ", " + CurrentCuttPiece.GetComponent<MeshFilter>().mesh.vertices[CurrentCuttPiece.GetComponent<MeshFilter>().mesh.triangles[itor]].y.ToString());
			}
			
		}
		
		//Now perform the hardcoded logic for correct platform rotation/translation/parenting of objects
		foreach(GameObject go in paperPlatforms)
		{
			//Debug.LogError("TESTING");
			if(Clone.GetComponent<MeshFilter>().mesh.triangles.Contains(go.GetComponent<Demo_PlatformMovement>().Indexer))
			{
				go.GetComponent<Demo_PlatformMovement>().PaperWorld = Clone;
			}
			else
			{
				go.GetComponent<Demo_PlatformMovement>().PaperWorld = Clone_2;
			}
		}
		
		//Turn true to flag that we are now done calculating the cut line
		doneCalculatingCuttLine = false;
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
		//Determine if we are currently rtransitioning
		//if(paperGridTearVertCheck[pos] && !edgeVertsForcedTorn.Contains(endPosTearPos))
		if(tearLine.ContainsKey(origPos))// && !edgeVertsForcedTorn.Contains(endPosTearPos))
		{
			currentlyInTransition = true;
		}
		//else if(tearLine.ContainsKey(origPos) && edgeVertsForcedTorn.Contains(endPosTearPos))
		{
		//	Debug.LogError(")))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))Testin662346656856");
		}
		
		//Check which island we are currently adding to
		if(addingToPieceOne && !currentlyInTransition)
		{
			island1.Add (pos);
		}
		else if(!addingToPieceOne && !currentlyInTransition)
		{
			island2.Add (pos);
		}
		//Here we add the vertice to both lists for new both new meshes
		else if(currentlyInTransition)
		{
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
	
	//private bool timeSwitched = false;
	
	/// <summary>
	/// This function is used to flag whether or not the tearLine is making an S-like
	/// shape, and not a U-like shape
	/// </summary>
	/// <returns>
	/// The line moving in S like shape.
	/// </returns>
	private bool TearLineMovingInSLikeShape(Vector3 startTearPos, Vector3 endTearPos, int listIndice, int startingVertIndice, List<Vector3> tempList)
	{
		//The value to be returned
		bool returnVal = false;
		
		//if(!tearLineTime.Keys.Contains(endTearPos) || !tearLineTime.Keys.Contains(startTearPos))
		{
		//	Debug.LogError("************************************TEAR TIMEING ERROR< NEED TO DETERMINE WHERE BUG IS LOCATED*****************************************");
		//	return false;
		}
		
		float distCheck = endTearPos.x - startTearPos.x;
		if(distCheck < 0) distCheck *= -1;
		
		//if(edgeVertsForcedTorn.Contains(startTearPos) && distCheck == MESH_VERT_OFFSET)
		{
		//	Debug.LogError("Shittybeans");
		//	startTearPos = endTearPos;
		}
		
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
				//Debug.LogError("********TIME SWITCHED********");
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
		
		//Here we are checking is the current decision involves an adge vertice
		//if(endTearPos.y == HEIGHT_MAX || endTearPos.y == HEIGHT_MIN) 
		if((HeightVerticeBoundsCheck(endTearPos)))// || HeightVerticeBoundsCheck(startTearPos)))
		{
			//Debug.LogWarning("HeightVerticeBoundsCheck) S-Shape");
			returnVal = true;
			
			//if(HeightVerticeBoundsCheckSLikeShape(endTearPos))
			//{
			//	Debug.LogWarning("HeightVerticeBoundsCheck)****u-Shape");
			//	returnVal = false;
			//}
		}
		
		//else if(startTearPos.x == WIDTH_MIN && endTearPos.x == WIDTH_MIN && EndChangeInY > endTearPos.y)
		else if(MinWidthBoundsCheck(startTearPos) && MinWidthBoundsCheck(endTearPos) && EndChangeInY > endTearPos.y)
		{
			//Debug.LogWarning("MinWidthBoundsCheck(endTearPos) && EndChangeInY > endTearPos.y -> U-Shape");
			returnVal = false;

		}
		//else if(startTearPos.x == WIDTH_MIN && endTearPos.x == WIDTH_MIN && EndChangeInY < endTearPos.y)
		else if(MinWidthBoundsCheck(startTearPos) && MinWidthBoundsCheck(endTearPos) && EndChangeInY < endTearPos.y)
		{
			//Debug.LogWarning("Normal-EDGE (start == end) S-Shape");
			returnVal = true;
		}
		
		//else if(startTearPos.x == WIDTH_MIN && endTearPos.x != WIDTH_MIN && EndChangeInY > endTearPos.y)
		else if(MinWidthBoundsCheck(startTearPos) && !MinWidthBoundsCheck(endTearPos) && EndChangeInY > endTearPos.y)
		{
			//Debug.LogWarning("Normal-EDGE (End != Start) U-Shape");
			returnVal = false;
		}
		//else if(startTearPos.x == WIDTH_MIN && endTearPos.x != WIDTH_MIN && EndChangeInY < endTearPos.y)
		else if(MinWidthBoundsCheck(startTearPos) && !MinWidthBoundsCheck(endTearPos) && EndChangeInY < endTearPos.y)
		{
			//Debug.LogWarning("Normal-EDGE (End != Start)  S-Shape");
			returnVal = true;
		}
		
		else if((StartChangeInY > startTearPos.y && EndChangeInY > endTearPos.y) ||
			(StartChangeInY < startTearPos.y && EndChangeInY < endTearPos.y))
		{
			//Debug.LogWarning("Normal U-Shape");
			returnVal = false;
		}
		else if((StartChangeInY > startTearPos.y && EndChangeInY < endTearPos.y) ||
			(StartChangeInY < startTearPos.y && EndChangeInY > endTearPos.y))
		{
			//Debug.LogWarning("Normal S-Shape");
			returnVal = true;
		}
		else
		{
			if(StartChangeInY == startTearPos.y && EndChangeInY > endTearPos.y)
			{
				//Debug.LogWarning("**************77**************");
				returnVal = true;
			}	
			else if(StartChangeInY == startTearPos.y && EndChangeInY < endTearPos.y)
			{
				//Debug.LogWarning("**************88**************");
				returnVal = false;
			}
			else if(EndChangeInY == endTearPos.y && !MaxWidthBoundsCheck(endTearPos))// && endTearPos.y != WIDTH_MAX)
			{
				//Debug.LogWarning("**************99**************");
				if(edgeOfObject.ContainsKey(new Vector3(endTearPos.x, endTearPos.y - MESH_VERT_OFFSET, endTearPos.z))
					&& edgeOfObject[new Vector3(endTearPos.x, endTearPos.y - MESH_VERT_OFFSET, endTearPos.z)])
				{
					//Debug.LogWarning("**************911**************");
					returnVal = false;
				}
				else
				{
					//Debug.LogWarning("**************411**************");
					returnVal = true;
				}
			}
			else
			{
				//Debug.LogWarning("*****************************NO DECISION REACHED AT SHAPE*********************************");
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
					/*if(prevVert1.y == tornVertPos.y)
					{
						if(tearLinePositionTime.ContainsKey((float)(timeOfCurrent - 2)))
						{
							//Debug.LogError("testingEndCgangeCheck111");
							prevVert1 = tearLinePositionTime[(float)(timeOfCurrent - 2)];
							if(prevVert1.y == tornVertPos.y)
							{
								if(tearLinePositionTime.ContainsKey((float)(timeOfCurrent - 3)))
								{
									//Debug.LogError("testingEndCgangeCheck1111");
									prevVert1 = tearLinePositionTime[(float)(timeOfCurrent - 3)];
								}
							}
						}
					}*/
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
		Vector3 testPos = this.transform.TransformPoint(CuttPieceOneVerts[CuttPieceOneFaces[index]]);
		Vector3 testPos1 = this.transform.TransformPoint(CuttPieceOneVerts[CuttPieceOneFaces[index + 1]]);
		Vector3 testPos2 = this.transform.TransformPoint(CuttPieceOneVerts[CuttPieceOneFaces[index + 2]]);
		
		//Now we create each new vertice array for each mewMesh
		if( island2.Contains(new Vector2(testPos.y, testPos.x)) && 
			island2.Contains(new Vector2(testPos1.y, testPos1.x)) && 
			island2.Contains(new Vector2(testPos2.y, testPos2.x)))
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
	}
	
	public int NumOfEdgesAssociatedWithInteriorFace;
	/// <summary>
	/// Sets the edge verts of object.
	/// </summary>
	private void SetEdgeVertsOfObject()
	{
		//The following represents the amount of faces a vertice must be 
		//associated with in order to be cosidered an interior vertice
		int numberOfEdgesAssociatedWithInteriorFace = NumOfEdgesAssociatedWithInteriorFace;
		
		//This is used to optimize finding which vertices are edges
		//For each vertice, the number of faces in mesh associated is stored
		//to determine interior versus edge vertices
		Dictionary<Vector3, int> faceCounter = new Dictionary<Vector3, int>();
		

		//Loop through the object mesh
		for(int index2 = 0; index2 < mesh.vertices.Count(); index2++)
		{
			//Foreach vertice in mesh, create placement within stored 
			//keeping track of number of faces per vertice in mesh
			faceCounter.Add(this.transform.TransformPoint(mesh.vertices[index2]), 0);
		}
			
		//Loop through each face
		for(int index3 = 0; index3 < mesh.triangles.Count(); index3++)
		{
			//Foreach vertice in face, add to the storeage keeping track of how many faces
			//are associated with each vertice
			faceCounter[this.transform.TransformPoint(mesh.vertices[mesh.triangles[index3]])] += 1;
		}
		

		//Loop through each vertice again and determine whether or not edge or interior
		for(int index4 = 0; index4 < faceCounter.Keys.Count(); index4++)
		{
			bool edgeVertice = false;
			
			//If the current vertice being examined does not have six faces associated with it
			//then, we know that the current vertice is an edge vertice
			if(faceCounter[faceCounter.Keys.ElementAt(index4)] <= numberOfEdgesAssociatedWithInteriorFace)
			{
				edgeVertice = true;
			}
			
			//Here we add the current vertice to the dictionary, and whether or not it is an edge versus interior
			edgeOfObject.Add(faceCounter.Keys.ElementAt(index4), edgeVertice);//this.transform.TransformPoint(mesh.vertices.ElementAt(index4))
		}
	}
	
	/// <summary>
	/// The following return true if the vertice being testing is an edge vertice
	/// </summary>
	private bool VerticeEdgeCheck(Vector3 testVert)
	{
		//Check is the vetice is an edge and return true if so (assuming edgeOfObject has been created properly)
		if(edgeOfObject[testVert])
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
		//We check if the testVert contains a vertice previously to the current
		//if(paperGrid[testVert.y].Contains(new Vector3(testVert.x - MESH_VERT_OFFSET, testVert.y, testVert.z)))
		if(paperGrid[testVert.y].ElementAt(0) != testVert)
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
	/// min width for the current colomn being examined in the current row being examined
	/// </summary>
	private bool MinWidthBoundsCheckSLikeShape(Vector3 testVert)
	{
		//We check if the testVert contains a vertice previously to the current
		if(paperGrid[testVert.y].Contains(new Vector3(testVert.x - MESH_VERT_OFFSET, testVert.y, testVert.z)))
		{
			if(edgeOfObject.ContainsKey(new Vector3(testVert.x, testVert.y - MESH_VERT_OFFSET, testVert.z))
				&& edgeOfObject[new Vector3(testVert.x, testVert.y - MESH_VERT_OFFSET, testVert.z)])
			{
				Debug.LogError("*************************************testing new min check on edge*************************************************");
				return true;
			}
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
