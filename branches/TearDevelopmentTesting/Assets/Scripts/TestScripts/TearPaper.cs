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
	/// The tearable object not largest piece of paper.
	/// </summary>
	public bool TearableObjectNotLargestPieceOfPaper;
	
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
		//newPiece = newPaper;
		originalColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		
		//Set the mesh offset distance based off object's vertice topology
		SetMeshOffsetWorldSpace();
		//SetMeshOffsetScreenSpace();
		
		//Initially, we set the paper's screen position to be zero
		screenPoint = new Vector3(0.0f, 0.0f, 0.0f);
		
		//Initialize the newMesh to be the same length of the previous
		newMesh = new Vector3[mesh.vertices.Length];
		
		//The following is not being utilized any more - a
		//hardcoded method in establishing max heigh and width of paper bounds
		//SetBoundsOfPaper();
		
		//init the dictionary storing the vertices along the tear line and their associated index
		//init the the mesh.vertice array
		tearLine = new Dictionary<Vector3, int>();
		tearLineTime = new Dictionary<Vector3, float>();
		tearLinePositionTime = new Dictionary<float, Vector3>();
		
		//Set the tearLineTimer to zero initially as the starting tear line 'time'
		tearLineTimer = 0;
		
		//The following is used to create organized grid structure
		// to easily access vertices during runtime
		SetPaperGrid();
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	private void Update () 
	{
		//Checking BadTera prevents player from tearing while game displaying preveious BadTear graphics
		if(!GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().BadTear)
		{
			//Ensure we stop tearing with bad input	
			if(cuttInProgress && PlayerInPaperBounds())
			{
				haveHitPaper = true;
			}
			
			if((Input.GetMouseButtonUp(0) && !haveHitPaper) || 
				(Input.GetMouseButtonUp(0) &&  PlayerInPaperBounds() && (tearLine.Keys.Count() > 0 || mouseTearPositions.Count() > 0))
				)
			{
				ForceStopTear();
			}
			
			
			//Ensure flags are set ocrrectly for decal drawing
			if(cuttInProgress && !PlatformPaper)
			{
				GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().PlayerCurrentlyTearing = true;
			}
			else if(!cuttInProgress && !PlatformPaper)
			{
				GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().PlayerCurrentlyTearing = false;
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
				else
				{
					//Traverse through old mesh to determine two new cutt pieces based off tearline
					FindNewCutPieces();	
					
					//Force stop tear if player tears through player or non tearable objects, or folded area
					CheckForBadTear();
				}
			}
			
			//TODO; Convert all input to touch input
			if(!CloneObject)
			{
				//If the left mouse button is down and the player is touching off the paper (colliding with dead space), and a cutt is no in progress, then,
				//we call mouse down an set flags for tearing accordingly
				if(Input.GetMouseButtonDown(0) && PlayerTouchingDeadSpace() && !cuttInProgress)
				{
					cuttInProgress = true;
					mouseTearPositions = new List<Vector3>();
					//OnMouseTearDown();
				}
				
				//If the left mouse is dragging and the player has correctly initiated a tear, and we are not forceint the tear to stop
				if(Input.GetMouseButton(0) && cuttInProgress)
				{
					OnMousetTearDrag();
	
				}
				
				//If the left mouse button is up, and the player is touching the dead space once again, and a cutt is in progress, then we call onMouseUp
				if(Input.GetMouseButtonUp(0)&& PlayerTouchingDeadSpace() && cuttInProgress)
				{
					OnMouseTearUp();
				}
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
				Vector3 testVertPos = Clone.GetComponent<MeshFilter>().mesh.vertices.ElementAt(0);
				for(int itor = 1; itor < Clone.GetComponent<MeshFilter>().mesh.vertices.Count(); ++itor)
				{
					//Debug.LogError("vertice = " + Clone.GetComponent<MeshFilter>().mesh.vertices.ElementAt(itor).ToString());
					if(Clone.GetComponent<MeshFilter>().mesh.vertices.ElementAt(itor) != testVertPos)
					{
						checkNotNeededObject = false;
						break;
					}
				}
				
				if(!checkNotNeededObject)
				{
					checkCloneNum = 2;
					testVertPos = Clone2.GetComponent<MeshFilter>().mesh.vertices.ElementAt(0);
					for(int itor = 1; itor < Clone2.GetComponent<MeshFilter>().mesh.vertices.Count(); ++itor)
					{
						//Debug.LogError("vertice = " + Clone2.GetComponent<MeshFilter>().mesh.vertices.ElementAt(itor).ToString());
						if(Clone2.GetComponent<MeshFilter>().mesh.vertices.ElementAt(itor) != testVertPos)
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
						//Debug.LogError("********TEST2.1******** Clone length = " + Clone.GetComponent<MeshFilter>().mesh.triangles.Length.ToString());
						GameObject.Destroy(Clone);
						GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().TornPlatforms.Remove(Clone);
					}
					else if(checkCloneNum == 2)
					{
						//Debug.LogError("********TEST2.2******** Clone2length = " + Clone2.GetComponent<MeshFilter>().mesh.triangles.Length.ToString());
						GameObject.Destroy(Clone2);
						GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().TornPlatforms.Remove(Clone2);
					}
				}
				else
				{
					if((Clone.GetComponent<TearPaper>().tearLine.Count() == 0 || Clone.GetComponent<TearPaper>().tearLine.Count() == 1
						|| Clone.GetComponent<TearPaper>().tearLine.Count() == 2) && Clone.GetComponent<MeshFilter>().mesh.triangles.Length == 0)
					{
						//Debug.LogError("********TEST_OIOIUUOIOUI___1******** Clone length = " + Clone.GetComponent<MeshFilter>().mesh.triangles.Length.ToString());
						GameObject.Destroy(Clone);
						GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().TornPlatforms.Remove(Clone);
					}
					
					if((Clone2.GetComponent<TearPaper>().tearLine.Count() == 0 || Clone2.GetComponent<TearPaper>().tearLine.Count() == 1
						|| Clone2.GetComponent<TearPaper>().tearLine.Count() == 2) && Clone2.GetComponent<MeshFilter>().mesh.triangles.Length == 0)
					{
						//Debug.LogError("********TEST_OIOIUUOIOUI___2******** Clone2 length = " + Clone2.GetComponent<MeshFilter>().mesh.triangles.Length.ToString());
						GameObject.Destroy(Clone2);
						GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().TornPlatforms.Remove(Clone2);
					}
					
					if((tearLine.Count() == 0 || tearLine.Count() == 1 || tearLine.Count() == 2) && gameObject.GetComponent<MeshFilter>().mesh.triangles.Length == 0)
					{
						//Debug.LogError("********TEST_OIOIUUOIOUI___3******** gameObject" + gameObject.GetComponent<MeshFilter>().mesh.triangles.Length.ToString());
						GameObject.Destroy(this);
						GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().TornPlatforms.Remove(gameObject);
					}
					
					if(!checkNotNeededObject)
					{
						//Debug.LogError("TEST3");
					}
				}
				
				NeedToCheckExcessDuplicates = false;
			}
		}
	}
	
	private bool haveHitPaper = false;
	
	#region Mouse Input Methods
	
	/// <summary>
	/// Raises the mouse down event.
	/// </summary>
	private void OnMouseTearDown()
	{
		//Debug.Log ("ENTERING ON MOUSE DOWN");
		//Init new list for storage
		mouseTearPositions = new List<Vector3>();
		
		//Testing to know where we are in logic
		//Debug.Log("Enter MouseDown");
	}
	
	/// <summary>
	/// Raises the mouse drag event.
	/// </summary>
	void OnMousetTearDrag()
	{
		//Initialize list storing gap indexes
		if(gapPositions == null)
		{
			gapPositions = new List<int>();	
			mousePrevPos = Input.mousePosition;
		}

		//We check to see if the player is touching an edge
		if(cuttInProgress)
		{
			
			//Debug.Log("DRAG");
			//Get the distance from previous mouseposition to current mouseposition
			
				
			
			float dist = Vector3.Distance(Camera.main.ScreenToWorldPoint(new Vector3(mousePrevPos.x, mousePrevPos.y, 
				Camera.main.WorldToScreenPoint(this.transform.TransformPoint(mesh.vertices.ElementAt(0))).z)), 
				Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 
				Camera.main.WorldToScreenPoint(this.transform.TransformPoint(mesh.vertices.ElementAt(0))).z)));
			
			/*
			float dist = Vector2.Distance(new Vector2(mousePrevPos.x, mousePrevPos.y), 
				new Vector2(Camera.main.WorldToScreenPoint(Clone.transform.TransformPoint(Clone.GetComponent<MeshFilter>().mesh.vertices.ElementAt(0))).x,
				Camera.main.WorldToScreenPoint(Clone.transform.TransformPoint(Clone.GetComponent<MeshFilter>().mesh.vertices.ElementAt(0))).y));
			*/
			
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
					Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 
						Camera.main.WorldToScreenPoint(this.transform.TransformPoint(mesh.vertices.ElementAt(0))).z)), 
					dist);
				
				/*
				AddMissingTearMousePositionsScreen(mousePrevPos, Input.mousePosition, dist);
				*/
			}
			
			float depth = Camera.main.WorldToScreenPoint(this.transform.TransformPoint(mesh.vertices.ElementAt(0))).z;
			//float depth = 0;
			
			//When mouse is draging, add input to list
			mouseTearPositions.Add(new Vector3(Input.mousePosition.x, Input.mousePosition.y, depth));
			//mouseTearPositions.Add(Input.mousePosition);
			
			//Keep track of the mouse's previous position
			mousePrevPos = Input.mousePosition;
				
			
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
		Vector3 testPlayerPos = new Vector3(Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y)).x, 
			Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y)).y, transform.position.z);
		
		if(GetComponent<MeshCollider>().bounds.Contains(testPlayerPos))
		{
			return true;
		}
		else 
		{
			return false;
		}
	}
	
	/// <summary>
	/// Raises the mouse up event.
	/// </summary>
	void OnMouseTearUp()
	{	
		if(CloneObject) return;
		
		//Save information for later use
		if(!cuttInProgress && Clone != null)
		{
			//Save old mesh information
			originalMeshVerts = Clone.GetComponent<MeshFilter>().mesh.vertices;//mesh.vertices;
			originalMeshTriangles = Clone.GetComponent<MeshFilter>().mesh.triangles;//mesh.triangles;
		}
		
		//Now, we check to see if the player is touching an edge to complete their cutt/tear
		if(cuttInProgress && mouseTearPositions.Count() > 0)
		{
			//Create a clone world mesh for deformation, we do not change the original world gameobject mesh
			if (!CloneObject) CreateNewPaperWorld();
			
			//Debug.LogError("GOOD CUTT");
			
			//This is used to flag loops or back tracking within tear line
			bool badInputDetected = false;
			
			//Transform mouseInput into torn vertice list (tearLine)
			badInputDetected = TurnInputIntoTornVertCurve();
			
			if(badInputDetected)
			{
				//Debug.LogError("BAD CUTT");
			//	ForceStopTear();
				return;
			}
			
			bool allTornVerticesEdgeVertices = false;
			for(int jtor = 0; jtor < tearLine.Keys.Count(); jtor++)
			{
				if(!edgeVerts.Contains(tearLine.Keys.ElementAt(jtor)))
				{
					allTornVerticesEdgeVertices = false;
					break;
				}
				else allTornVerticesEdgeVertices = true;
			}
			if(allTornVerticesEdgeVertices)
			{
				//Debug.LogError("BAD TEAR LINE -- allTornVerticesEdgeVertices TRUE");
				ForceStopTear();
				return;
			}
			
			//the following deletes excess edge torn vertices
			DeleteExcessEdgeVertices();
			
			if(tearLine.Keys.Count() != 0)
			{
				//RemoveProblematicSubRegionsOfTear();
				ForceHorizontalCuttAlongEdge();
			
				//Now, we make sure every row that should be torn is torn
				ForceHorizontalCuttFirstRowVert();
			}
			
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
		
		
		//if(tearLine.ContainsKey(paperGrid[0][0]))
		{
			//Debug.LogError("))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))) " + paperGrid.Keys.ElementAt(0).ToString());
		}
	}
	
	#endregion
	
	
	#endregion
	
	#region Defined Methods
	
	/// <summary>
	/// This function is used to ForceStopTear when the player 
	/// trys to tear out an untearable object
	/// </summary>
	private void PlayerTornUnTearableObjects()
	{
		ForceStopTear();
		if(GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().TearFinished)
		{
			GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().TearFinished = false;
		}
		
		if(GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().TornPlatforms.Count() > 0)
		{
			GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().TornPlatforms = new List<GameObject>();
		}
		
		GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().MainWorldCutPaper = null;
		GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().MainWorldPaper = null;
	}
	
	/// <summary>
	/// Checks for bad tear.
	/// </summary>
	private void CheckForBadTear()
	{
		if(!PlatformPaper)
		{
			for(int itor = 0; itor < CurrentCuttPiece.GetComponent<MeshFilter>().mesh.triangles.Count(); itor++)
			{
				Vector3 worldVertPos = CurrentCuttPiece.transform.TransformPoint(CurrentCuttPiece.GetComponent<MeshFilter>().mesh.vertices[CurrentCuttPiece.GetComponent<MeshFilter>().mesh.triangles.ElementAt(itor)]);
				worldVertPos.z = GameObject.FindGameObjectWithTag("Player").transform.position.z;
				if(GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider>().bounds.Contains(worldVertPos))
				{
					PlayerTornUnTearableObjects();
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
			}
		}
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
		
		//loop through the current mesh
		for(int itor = 0; itor < paperGrid.Keys.Count(); itor++)
		{
			//loop through the current mesh row
			for(int jtor = 0; jtor < paperGrid[paperGrid.Keys.ElementAt(itor)].Count(); jtor++)
			{
				//Debug.LogError("**************Checking distance****************");
				if(jtor > 0)
				{
					float distanceXCheck = paperGrid[paperGrid.Keys.ElementAt(itor)].ElementAt(jtor).x - paperGrid[paperGrid.Keys.ElementAt(itor)].ElementAt(jtor - 1).x;
					float distanceYCheck = paperGrid[paperGrid.Keys.ElementAt(itor)].ElementAt(jtor).y - paperGrid[paperGrid.Keys.ElementAt(itor)].ElementAt(jtor - 1).y;
					
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
			
			//CuttPieceOneFaces = Clone.GetComponent<MeshFilter>().mesh.triangles;
			//CuttPieceTwoFaces = Clone.GetComponent<MeshFilter>().mesh.triangles;
			//CuttPieceOneVerts = Clone.GetComponent<MeshFilter>().mesh.vertices;
			//CuttPieceTwoVerts = Clone.GetComponent<MeshFilter>().mesh.vertices;
			
			int[] isl1Ind = new int[gameObject.GetComponent<MeshFilter>().mesh.triangles.Count()];
			int[] isl2Ind = new int[gameObject.GetComponent<MeshFilter>().mesh.triangles.Count()];
			
			indexor1 = 0;
			indexor2 = 0;
			
			for(int index = 0; index < gameObject.GetComponent<MeshFilter>().mesh.triangles.Count(); index += 3)
			{
				Vector3 testPos = this.transform.TransformPoint(gameObject.GetComponent<MeshFilter>().mesh.vertices[gameObject.GetComponent<MeshFilter>().mesh.triangles[index]]);
				Vector3 testPos1 = this.transform.TransformPoint(gameObject.GetComponent<MeshFilter>().mesh.vertices[gameObject.GetComponent<MeshFilter>().mesh.triangles[index + 1]]);
				Vector3 testPos2 = this.transform.TransformPoint(gameObject.GetComponent<MeshFilter>().mesh.vertices[gameObject.GetComponent<MeshFilter>().mesh.triangles[index + 1]]);
				
				
				//Now we create each new vertice array for each mewMesh
				if( isl1.Contains(testPos) && 
					isl1.Contains(testPos1) && 
					isl1.Contains(testPos2))
				{
					//Debug.LogError("Currently adding the following to island 2*********: " + CuttPieceOneVerts[CuttPieceOneFaces[index]].ToString() + " , " + CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].ToString() + " , " + CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].ToString());
					isl1Ind[indexor1] = gameObject.GetComponent<MeshFilter>().mesh.triangles[index];
					isl1Ind[indexor1 + 1] = gameObject.GetComponent<MeshFilter>().mesh.triangles[index + 1];
					isl1Ind[indexor1 + 2] = gameObject.GetComponent<MeshFilter>().mesh.triangles[index + 2];
					indexor1 += 3;	
						
				}
				else
				{
					isl2Ind[indexor2] = gameObject.GetComponent<MeshFilter>().mesh.triangles[index];
					isl2Ind[indexor2 + 1] = gameObject.GetComponent<MeshFilter>().mesh.triangles[index + 1];
					isl2Ind[indexor2 + 2] = gameObject.GetComponent<MeshFilter>().mesh.triangles[index + 2];
					indexor2 += 3;
				}
			}
			
			GameObject newPiece = (GameObject)Instantiate(newPaper, this.transform.position, this.transform.rotation);
			newPiece.GetComponent<TearPaper>().CloneObject = true;
			newPiece.name = "Poopy #" + testingFrequency.ToString();
			
			newPiece.GetComponent<MeshFilter>().mesh.triangles = isl1Ind;
			newPiece.GetComponent<MeshCollider>().sharedMesh = newPiece.GetComponent<MeshFilter>().mesh;
			this.GetComponent<MeshFilter>().mesh.triangles = isl2Ind;
			//this.mesh.RecalculateBounds();
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
		paperGridTearVertCheck = new Dictionary<Vector2, bool>();
		paperGrid = new Dictionary<float, List<Vector3>>();
		
		//Load grid dictionary
		for(int itor = 0; itor < this.GetComponent<MeshFilter>().mesh.vertices.Length; itor++)
		{
			//New world vertice location 
			//Vector3 point = this.transform.TransformPoint(new Vector3(mesh.vertices[itor].x, mesh.vertices[itor].y, mesh.vertices[itor].z));
			Vector3 point = this.transform.TransformPoint(this.GetComponent<MeshFilter>().mesh.vertices[itor]);
			//Vector3 point = this.GetComponent<MeshFilter>().mesh.vertices[itor];
			
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
		//Initialize the max and min world values of mesh mertex coordinates
		WIDTH_MAX = -100;
	 	WIDTH_MIN = 100;
		HEIGHT_MAX = -100;
		HEIGHT_MIN = 100;
		
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
		Clone.GetComponent<TearPaper>().CloneObject = true;
		Clone.GetComponent<TearPaper>().NeedToCheckExcessDuplicates = true;
		
		//Set clone informaiton
		Clone2 = (GameObject) Instantiate(newPaper, this.transform.position, this.transform.rotation);
		Clone2.GetComponent<TearPaper>().CloneObject = true;
		Clone2.GetComponent<TearPaper>().NeedToCheckExcessDuplicates = true;
		Clone2.GetComponent<MeshRenderer>().enabled = false;
		
		//Turn off the original object's meshRenderer to hide
		this.GetComponent<MeshRenderer>().enabled = false;
		this.GetComponent<MeshCollider>().enabled = false;
		
		//Initialize the storage keeping tracking of edge information
		edgeOfObject = new Dictionary<Vector3, bool>();
		
		//The following determines which vertices are interior and which are edge
		SetEdgeVertsOfObject();
		
		//Initialize the relationships between edge vertices
		SetEdgeRelationShips();
		
		//This flag makes sure that excess duplicated get properly disgarded
		NeedToCheckExcessDuplicates = true;
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
		//Debug.LogError("NUMBER OF EDGE VERTICES = " + edgeVerts.Count().ToString() + " with meshOffset = " + MESH_VERT_OFFSET.ToString());
		
		
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
		//Debug.LogError("NUMBER OF EDGE VERTICES 2 =  " + organizedEdgeVert.Count().ToString());
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
		bool returnVal = false;
		
		//Create raytrace object
		RaycastHit hit = new RaycastHit();
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
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
							//Debug.Log("KeyNotFoundException");
							ForceStopTear();
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
				else if(tearLine.Keys.ElementAt(0).y > tearLine.Keys.ElementAt(1).y && tearLineTime.ContainsKey(tearLine.Keys.ElementAt(0)))
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
			//Vector3 curPosition = this.transform.TransformPoint(Camera.main.ScreenToWorldPoint(
			//	new Vector3(mouseTearPositions[indexr].x, mouseTearPositions[indexr].y, screenDepth.z)));
			
			Vector3 curPosition = Camera.main.ScreenToWorldPoint(
				new Vector3(mouseTearPositions[indexr].x, mouseTearPositions[indexr].y, screenDepth.z));
			
			//Vector3 curPosition = Camera.main.ScreenToWorldPoint(mouseTearPositions[indexr]);
			
			newMouseTearWorldPos.Add (curPosition);
		}
		//Debug.LogError("*********Converting mouse positions with length = " + mouseTearPositions.Count().ToString());
		//Re-assign the mouse positions to the 3D positions
		mouseTearPositions = newMouseTearWorldPos;
		
		//This is used to keeping track of the previously torn vertice
		Vector3 previousTornVert; //Clone.GetComponent<MeshFilter>().mesh.vertices[0];
		
		//This keeps track of the number of 
		int numberOfVerts = 0;
		
		//Create structure to keep track of the previous mouse position
		Vector3 previousMousePos = mouseTearPositions.ElementAt(0);
		
		
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
							ForceStopTear();
							return true;
						}
						
						//Flag the current vertice to be a torn vetice
						paperGridTearVertCheck[paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum]] = true;
							
						//This maps a 'time' to the tear vertice
						tearLineTime.Add (paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum], (float)tearLineTimer);
						tearLinePositionTime.Add ((float)tearLineTimer, paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum]);
						previousTornVert = paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum];
					}
					//else //if(distCheck != -1000)
					//{
					//	returnVal = true;
					//}
					
					//Set previous the the position we just looked at
					previousMousePos = mouseTearPositions.ElementAt(jtor);
				}
			}
		}
		
		bool check = false;
		if(tearLine.Keys.Count() > 2)
		{
			bool badTear = true;
			Vector3 tempTearPos = tearLine.Keys.ElementAt(0);
			for(int itor = 1; itor < tearLine.Keys.Count(); itor++)
			{
				if(tearLine.Keys.ElementAt(itor) != tempTearPos)
				{
					badTear = false;
					break;
				}
			}
			
			if(badTear)
			{
				check = true;
				Debug.Log("BAD TEAR FOUND - same vertice is a torn vertice multiple times");
			}
		}
		
		if(tearLine.Keys.Count() == 0 || tearLine.Keys.Count() == 1 || tearLine.Keys.Count() == 2 || check)
		{
			//Debug.Log("Tearline Too small");
			ForceStopTear();
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
	private void ForceStopTear()
	{
		if(!PlatformPaper)
		{
			GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().BadTear = true;
		}
		
		//Debug.LogError("Now forceing Stop tear");
		//Rest everything needed to reset the states when a bad tear occurs
		cuttInProgress = false;
		doneCalculatingCuttLine = false;
		mouseTearPositions = new List<Vector3>();
		tearLine = null;
		tearLine = new Dictionary<Vector3, int>();
		gapPositions = null;//Leave null for reseting mousePrevPos
		tearLineTime = null;
		tearLineTime = new Dictionary<Vector3, float>();
		tearLinePositionTime = null;
		tearLinePositionTime = new Dictionary<float, Vector3>();
		addingToPieceOne = true;
		tearLineTimer = 1;
		forceStopTear = false;
		haveTouchedOffPaperToStartTear = false;
		
		haveHitPaper = false;
		
		
		//The following makes sure every object is properly disposed
		CleanUpClonedObjects();
	}
	
	/// <summary>
	/// Cleans up cloned objects and restores mesh 
	/// visibility and collision iff needed
	/// </summary>
	private void CleanUpClonedObjects()
	{
		if(Clone != null)
		{
			GameObject.Destroy(Clone);
		}
		if(Clone2 != null)
		{
			GameObject.Destroy(Clone2);
		}
		if(!GetComponent<MeshRenderer>().enabled)
		{
			GetComponent<MeshRenderer>().enabled = true;
		}
		if(!GetComponent<MeshCollider>().enabled)
		{
			GetComponent<MeshCollider>().enabled = true;
		}
		
	}
	
	/// <summary>
	/// Cutts the paper object along tearline
	/// Before this is called, we already know which vertices are torn, here, we determine which
	/// vertices belong to which new cutt piece
	/// </summary>
	private void FindNewCutPieces()
	{
		if(Clone == null)
		{
			//Debug.Log("Testing bad tear check");
			doneCalculatingCuttLine = false;
			return;
		}
		
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
				//Debug.LogError(" test1 = " + tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator)[0]).ToString());
				//Debug.LogError(" test2 = " + tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator - 1)[0]).ToString());
				
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
					//Debug.Log("increasing number of edge torn vertices");
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
			
			//Debug.LogError("starting new row, and addingToPieceOne = " + addingToPieceOne.ToString() + " and tearline contains the first element of this row = " + tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator)[0]).ToString () + " with first vert = " + paperGrid.Values.ElementAt(iterator)[0].ToString());
			
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
					
					//Testing printing
					//Debug.LogWarning("Current vert = " + paperGrid.Values.ElementAt(iterator)[jtor].ToString() + " has hit tearvert in row = " + iterator.ToString());
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
							
						if(distanceCheck > MESH_VERT_OFFSET)
						{
							//Debug.LogError("**************************************currently, test to prevent wrong ushape triggered**************************************");
						}
						
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
				
				newIsland2VertPositions[newIsland2Indexr] = Clone2.GetComponent<MeshFilter>().mesh.vertices[island2Indicies[index]];
				
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
		Clone2.GetComponent<MeshFilter>().mesh.vertices = newIsland2VertPositions;
		*/
		
		
		//Reassign new mesh triangles, defining the new faces for the cloned object
		Clone.GetComponent<MeshFilter>().mesh.triangles = island1Indicies;
		Clone2.GetComponent<MeshFilter>().mesh.triangles = island2Indicies;
		
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
		
		//We only add to tear manager here iff we are a platofrm piece of paper
		if(PlatformPaper)
		{
			//Add clones to manager
			GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().TornPlatforms.Add (Clone);
			GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().TornPlatforms.Add (Clone2);
		}
		

		//Rename the object based on the number of faces in each of the new meshs
		if(island2.Count > island1.Count)
		{
			Clone2.name = "paper_CuttPieceOfPaper";
			Clone.name = "paper_LargerPiece";
			
			if(!PlatformPaper)
			{
				originalColor = Clone2.GetComponent<MeshRenderer>().material.color;
				Clone2.GetComponent<MeshRenderer>().material.color = Color.green;
			}
			
			
			CurrentCuttPiece = Clone2;
			
			//If world paper
			if(!PlatformPaper)
			{
				GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().MainWorldCutPaper = Clone2;
				GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().MainWorldPaper = Clone;
				GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().TearFinished = true;
			}
			
			//for(int itor = 0; itor < island2Indicies.Count();++itor)
			{
				//Debug.LogError("CuttPiece #" + itor.ToString() + " has a vertice index = " + island2Indicies[itor].ToString());
				//Debug.LogError("CuttPiece #" + itor.ToString() + " has a vertice = " + CurrentCuttPiece.GetComponent<MeshFilter>().mesh.vertices[island1Indicies[itor]].x.ToString() + ", " + CurrentCuttPiece.GetComponent<MeshFilter>().mesh.vertices[island1Indicies[itor]].y.ToString());
			}
		}
		else
		{
			Clone.name = "paper_CuttPieceOfPaper";
			Clone2.name = "paper_LargerPiece";
			
			if(!PlatformPaper)
			{
				originalColor = Clone2.GetComponent<MeshRenderer>().material.color;
				Clone.GetComponent<MeshRenderer>().material.color = Color.green;
			}
			
			CurrentCuttPiece = Clone;
			
			//If world paper
			if(!PlatformPaper)
			{
				GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().MainWorldCutPaper = Clone;
				GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().MainWorldPaper = Clone2;
				GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().TearFinished = true;
			}
			
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
		
		//Turn true to flag that we are now done calculating the cut line
		doneCalculatingCuttLine = false;
		
		
	}
	
	/// <summary>
	/// Sets the mesh offset for this object's vertice topology
	/// </summary>
	private void SetMeshOffsetWorldSpace()
	{
		//The following assumes vertices located at 0 and 1 are located next to eachother
		if(GetComponent<MeshFilter>().mesh.vertices[0].x != GetComponent<MeshFilter>().mesh.vertices[1].x)
		{
			MESH_VERT_OFFSET = this.transform.TransformPoint(GetComponent<MeshFilter>().mesh.vertices[0]).x - this.transform.TransformPoint(GetComponent<MeshFilter>().mesh.vertices[1]).x;
		}
		else
		{
			MESH_VERT_OFFSET = this.transform.TransformPoint(GetComponent<MeshFilter>().mesh.vertices[0]).y - this.transform.TransformPoint(GetComponent<MeshFilter>().mesh.vertices[1]).y;
		}
		
		//Make sure the mesh offset is non-negative
		if(MESH_VERT_OFFSET < 0) MESH_VERT_OFFSET *= -1;
		
		//Testing, output the offset
		//Debug.LogWarning("Mesh vertice offset = " + MESH_VERT_OFFSET.ToString());
	}
	
		/// <summary>
	/// Sets the mesh offset for this object's vertice topology
	/// </summary>
	private void SetMeshOffsetScreenSpace()
	{
		//The following assumes vertices located at 0 and 1 are located next to eachother
		if(GetComponent<MeshFilter>().mesh.vertices[0].x != GetComponent<MeshFilter>().mesh.vertices[1].x)
		{
			MESH_VERT_OFFSET = Camera.main.WorldToScreenPoint(this.transform.TransformPoint(GetComponent<MeshFilter>().mesh.vertices[0])).x - Camera.main.WorldToScreenPoint(this.transform.TransformPoint(GetComponent<MeshFilter>().mesh.vertices[1])).x;
		}
		else
		{
			MESH_VERT_OFFSET = Camera.main.WorldToScreenPoint(this.transform.TransformPoint(GetComponent<MeshFilter>().mesh.vertices[0])).y - Camera.main.WorldToScreenPoint(this.transform.TransformPoint(GetComponent<MeshFilter>().mesh.vertices[1])).y;
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
				Debug.LogWarning("*****************************NO DECISION REACHED AT SHAPE*********************************");
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
