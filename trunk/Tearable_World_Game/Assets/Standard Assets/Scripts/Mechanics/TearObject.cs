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

public class TearObject : MonoBehaviour 
{
	
	#region Public Fields
	
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
	/// This is completely determinant upon what plane mesh is being used (THIS IS CURRENTLY HARDCODED)
	/// TODO: READ IN MESH AND ASSIGN, DO NOT HARDCODE THIS VALUE :-(
	/// </summary>
	public float MESH_VERT_OFFSET;
	
	/// <summary>
	/// The tearable objects stores all object that can be torn
	/// </summary>
	public List<TornObject> TearableObjects = new List<TornObject>();
	
	/// <summary>
	/// The objects that can be torn, this is intended to be added to within
	/// unity editor, then addded to the tearableObjects list during start
	/// </summary>
	public List<GameObject> ObjectsThatCanBeTorn = new List<GameObject>();
	
	/// <summary>
	/// The current cutt piece being manipulated by player
	/// </summary>
	public GameObject CurrentCuttPiece;
	
	#endregion
	
	#region Variables
	
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
	private Dictionary<Vector2, bool> paperGrid_1;
	
	/// <summary>
	/// The paper grid organizes the mesh into a grid like structure for determineing torn pieces of paper
	/// </summary>
	private Dictionary<float, List<Vector3>> paperGrid;
	
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
	
	#endregion
	
	#region Methods - Built In Defined
	/// <summary>
	/// Use this for initialization
	/// </summary>
	private void Start () 
	{
		//Set moveDirection for rotation = 0
		moveDirection = Vector3.zero;
		
		//Initially, we set the paper's screen position to be zero
		screenPoint = new Vector3(0.0f, 0.0f, 0.0f);
		
		//Initialize the newMesh to be the same length of the previous
		newMesh = new Vector3[mesh.vertices.Length];
		
		//Create a clone world mesh for deformation, we do not change the original world gameobject mesh
		CreateNewPaperWorld();
		
		//Make sure the offset is positive
		if(MESH_VERT_OFFSET < 0) 
		{
			MESH_VERT_OFFSET *= -1;
		}
		//Testing, output the offset
		Debug.LogWarning("Mesh vertice offset = " + MESH_VERT_OFFSET.ToString());
		
		//Initialize the max and min world values of mesh mertex coordinates
		WIDTH_MAX = -100;
	 	WIDTH_MIN = 100;
		HEIGHT_MAX = -100;
		HEIGHT_MIN = 100;
		SetBoundsOfPaper();
		
		//init the dictionary storing the vertices along the tear line and their associated index
		//into the the mesh.vertice array
		tearLine = new Dictionary<Vector3, int>();
		paperGrid = new Dictionary<float, List<Vector3>>();
		tearLineTime = new Dictionary<Vector3, float>();
		tearLinePositionTime = new Dictionary<float, Vector3>();
		paperGrid_1 = new Dictionary<Vector2, bool>();
		
		//Set the tearLineTimer to zero initially as the starting tear line 'time'
		tearLineTimer = 0;
		
		//Load grid dictionary
		for(int itor = 0; itor < mesh.vertices.Length; itor++)
		{
			//Row dependent, therefore we store the height first
			paperGrid_1.Add(new Vector2(mesh.vertices[itor].y, mesh.vertices[itor].x), false);
			
			//If the key doesn't already exist within dictionary
			if(!paperGrid.ContainsKey(mesh.vertices[itor].y))
			{
				List<Vector3> valueDict = new List<Vector3>();
				valueDict.Add(new Vector3(mesh.vertices[itor].x, mesh.vertices[itor].y, mesh.vertices[itor].z));
				paperGrid.Add(mesh.vertices[itor].y, valueDict);//[mesh.vertices[itor].y] = valueDict;
			}
			else
			{
				List<Vector3> newList = paperGrid[mesh.vertices[itor].y];
				newList.Add(new Vector3(mesh.vertices[itor].x, mesh.vertices[itor].y, mesh.vertices[itor].z));
				paperGrid[mesh.vertices[itor].y] = newList;
			}
		}
		
		//bubble sort each list (colomns) within the dictionary
		//TODO: Optomize into heap, quick, merge, or another faster sorting alg
		foreach(List<Vector3> list in paperGrid.Values)
		{
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
		tempList122.Sort();
		//Now that we have a list representing the sorted keys,
		Dictionary<float, List<Vector3>> newPaperGrid = new Dictionary<float, List<Vector3>>();
		for(int indexr = 0; indexr < tempList122.Count; indexr++)
		{
			List<Vector3> newList2 = paperGrid[tempList122.ElementAt(indexr)];
			float index2 = tempList122.ElementAt(indexr);
			newPaperGrid.Add(index2, newList2);
		}
		//Assign the papergrid to the new grid with organized rows
		paperGrid = newPaperGrid;
		
		//Find the distance between any two adjacent points on mesh
		//(assumming mesh has even distribution of vertices)
		// This is used to know where neighbor vertices are located
		MESH_VERT_OFFSET = paperGrid[0][0].x - paperGrid[0][1].x;
		if(MESH_VERT_OFFSET < 0) MESH_VERT_OFFSET *= -1;
		
		Debug.Log("MESH_VERT_OFFSET = " + MESH_VERT_OFFSET.ToString());
	}
	
	public bool DrawTearLineONLY;
	
	/// <summary>
	/// The offset of the player input position to world corrdinates
	/// </summary>
	private Vector3 offset;
	
	/// <summary>
	/// The currently moving cutt piece flags when the player is moving the cutt piece after performing a tear
	/// </summary>
	private bool currentlyMovingCuttPiece = false;
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	private void Update () 
	{
		//Debug.Log("current size off tearbleObjects list = " + TearableObjects.Count.ToString());
		//foreach TearableObjects object, update it
		for(int jtor = 0; jtor < TearableObjects.Count; jtor++)
		{
			//If largest background piece is true, then we update differently
			if(TearableObjects.ElementAt(jtor).LargestbackgroundPiece)
			{
				//Debug.Log("Entering largest + jtor = " + jtor.ToString());
				//Update the priginalPiece of paper
				//here, tearing starts based of the player initiating the tear off screen
				//TODO, switch from mouse input to touch input
				Update_PieceOfPaper(TearableObjects.ElementAt(jtor));
			}
			else
			{
				//Debug.Log("Entering tearableUpdateObject + jtor = " + jtor.ToString());
				UpdateTearableObject(TearableObjects.ElementAt(jtor));
			}
		}
		
	}
	
	/// <summary>
	/// The force stop tear flag is used to stop the tear once player has left paper during a tear
	/// </summary>
	private bool forceStopTear = false;
	
	/// <summary>
	/// The player has touched paper this tear flags whether the player has started their tear on the paper object
	/// </summary>
	private bool playerHasTouchedPaperThisTear = false;
	
	#region Mouse Input Methods
	
	/// <summary>
	/// Raises the mouse down event.
	/// </summary>
	private void OnMouseDown()
	{
		//Debug.Log ("ENTERING ON MOUSE DOWN");
		//Init new list for storage
		mouseTearPositions = new List<Vector3>();
		
		//Testing to know where we are in logic
		Debug.Log("Enter MouseDown");
		
		//Save information for later use
		if(!cuttInProgress)
		{
			//Save old mesh information
			originalMeshVerts = Clone.GetComponent<MeshFilter>().mesh.vertices;//mesh.vertices;
			originalMeshTriangles = Clone.GetComponent<MeshFilter>().mesh.triangles;//mesh.triangles;
		}
	}
	
	private List<int> gapPosiitons;
	private int numMousPos = 0;
	
	/// <summary>
	/// The mouse's previous position, this is used to keeping track
	/// and determining whether the mouse in moving rapidly
	/// </summary>
	private Vector3 mousePrevPos;

	
	/// <summary>
	/// Raises the mouse drag event.
	/// </summary>
	void OnMouseDrag()
	{
		//Initialize list storing gap indexes
		if(gapPosiitons == null)
		{
			gapPosiitons = new List<int>();	
			mousePrevPos = Input.mousePosition;
		}
		
		//Debug.LogWarning("**PlayerTouchingOffPaper = " + PlayerTouchingOffPaper().ToString());
		//Debug.LogWarning("**PlayerTouchingPaper =  " + PlayerTouchingPaper().ToString());
		//Debug.LogWarning("**cuttInProgress =  " + cuttInProgress.ToString());
		//We check to see if the player is touching an edge
		if(cuttInProgress)//(PlayerTouchingOffPaper() && !cuttInProgress) || (PlayerTouchingPaper() && cuttInProgress))//PlayerTouchingEdge(true) || cuttInProgress
		{
			//Debug.Log("DRAG");
			//Get the distance from previous mouseposition to current mouseposition
			float dist = Vector3.Distance(Camera.main.ScreenToWorldPoint(new Vector3(mousePrevPos.x, mousePrevPos.y, 
				Camera.main.WorldToScreenPoint(Clone.GetComponent<MeshFilter>().mesh.vertices.ElementAt(0)).z)), 
				Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 
				Camera.main.WorldToScreenPoint(Clone.GetComponent<MeshFilter>().mesh.vertices.ElementAt(0)).z)));
			
			//Just in case the distance is negative
			if(dist < 0)
			{
				dist *= -1;
			}
			
			//If the position is greater than the meshvertoffset, then we know they are moving extremely fast,
			//therefore, we have to fill in the missing positions
			if(dist > (MESH_VERT_OFFSET/2))
			{
				//We check to make sure the mouse is inbounds when trying to add missing pieces of input
				//if(MouseInBounds(Input.mousePosition))
				{
					Debug.LogError("PLEASE NOTE: A GAP IS FOUND! Provide slower input for more presice cutt");
					AddMissingTearMousePositions(
						Camera.main.ScreenToWorldPoint(new Vector3(mousePrevPos.x, mousePrevPos.y, 
							Camera.main.WorldToScreenPoint(Clone.GetComponent<MeshFilter>().mesh.vertices.ElementAt(0)).z)), 
						Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 
							Camera.main.WorldToScreenPoint(Clone.GetComponent<MeshFilter>().mesh.vertices.ElementAt(0)).z)), 
						dist);
				}
			}
			
			//When mouse is draging, add input to list
			mouseTearPositions.Add(Input.mousePosition);
			
			//Keep track of the mouse's previous position
			mousePrevPos = Input.mousePosition;
		}
		else
		{
			//This statment should be hit is the UI starts a tear but does not end on an edge properly
			Debug.LogError("POOOP");
		}
	}
	
	/// <summary>
	/// Raises the mouse up event.
	/// </summary>
	void OnMouseUp()
	{	
		Debug.Log ("ENTERING ON MOUSE UP");
		//Now, we check to see if the player is touching an edge to complete their cutt/tear
		if(PlayerTouchingDeadSpace() && cuttInProgress)//PlayerTouchingEdge(false) PlayerTouchingOffPaper()//PlayerTouchingDeadSpace()  PlayerTouchingOffPaper()
		{
			//Get rid of any loops within tear curve
			//CheckForAndRemoveLoops();
			
			//Transform mouseInput into torn vertice list (tearLine)
			if(!CloneObject) TurnInputIntoTornVertCurve();
			
			else 
			{
				//Here we assume tearable objects contains the objects that need to be split (torn/cutt)
				foreach(TornObject to in TearableObjects)
				{
					//here, we want to turn the input mouse line into tear vertices differently depending upon what the current largest
					//piece of the paper background
					if(to.LargestbackgroundPiece)
					{
						//This is called on the largest piece because this pieace initiallizez and finalized the tear pine
						TurnInputIntoTornVertCurveLargestPiece(to);
					}
					else
					{
						//here we are looping through each individual 
						TurnInputIntoTornVertCurveOnTearObject(to);
					}
				}
			}
			
			//Save old mesh information
			//Clone.GetComponent<MeshFilter>().mesh.vertices = originalMeshVerts;
			//Clone.GetComponent<MeshFilter>().mesh.triangles = originalMeshTriangles;	
			
			//Flag we are done finding cutt/tear line
			doneCalculatingCuttLine = true;
		}
		
		//The cutt has now stopped
		if(cuttInProgress) cuttInProgress = false;
		
		if(forceStopTear) forceStopTear = false;
	}
	
	#endregion
	
	
	#endregion
	
	#region Defined Methods
	
	private GameObject newEmptyRotationGameObject;
	
	/// <summary>
	/// Updates the tearable object.
	/// </summary>
	private void UpdateTearableObject(TornObject to)
	{
		if(doneCalculatingCuttLine)
		{
			Debug.LogError("done calculating tear TornObject");
			//This flag is for testing purposes only,
			//this flag is intented to be enabled from within unity to visuall debug and test tear line
			if(DrawTearLineONLY)
			{
				DrawTearLineOnly();
			}
			else
			{
				//This is used to parse through the mesh to determine which vertices and faces
				//belong to which cut piece
				FindNewCutPieces();	
				
				//Assign flag for moving cutt piece
				currentlyMovingCuttPiece = true;
				
				//Assign varaibles for moving piece
				screenPoint = Camera.main.WorldToScreenPoint(CurrentCuttPiece.transform.position);
    			offset = CurrentCuttPiece.transform.position - Camera.main.ScreenToWorldPoint(
        			new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
				
				//Now we set the center of the cuttObject, used for rotation
				centerOfCuttPieceRotOffset =  FindCenterOfCuttPiece();
			}
		}
		else if (currentlyMovingCuttPiece)
		{
			//Here the cuttPiece needs to be translated where ever the player desires
			MoveCuttPiceToDesiredLocation();
		}
	
		//The OnMouse functions are only called when mouse collides with an objects collider,
		//The following
		if(Input.GetMouseButtonDown(0) && PlayerTouchingDeadSpace() && !cuttInProgress)//PlayerTouchingDeadSpace() PlayerTouchingOffPaper()
		{
			//Debug.LogWarning("Updateing, Left Mouse DRAG");
			cuttInProgress = true;
			playerHasTouchedPaperThisTear = false;
			forceStopTear = false;
			OnMouseDown();
		}
		if(Input.GetMouseButton(0) && !forceStopTear && cuttInProgress)// && !PlayerTouchingDeadSpace())
		{
			//Debug.LogWarning("Updateing, Left Mouse DOWN");
			OnMouseDrag();
			
			if(PlayerTouchingPaper())
			{
				playerHasTouchedPaperThisTear = true;	
			}
			if(playerHasTouchedPaperThisTear &&  PlayerTouchingDeadSpace())//PlayerTouchingDeadSpace()  PlayerTouchingOffPaper()
			{
				playerHasTouchedPaperThisTear = false;
				forceStopTear = true;
			}
		}
		if((Input.GetMouseButtonUp(0)&& PlayerTouchingDeadSpace() && cuttInProgress) || forceStopTear)//PlayerTouchingDeadSpace()  PlayerTouchingOffPaper()
		{
			//The following prevents mouseUp from being called if the player just cutts the dead space
			if(forceStopTear || playerHasTouchedPaperThisTear)
			{
				OnMouseUp();
			}
		}
		else if(Input.GetMouseButtonUp(0)&& !PlayerTouchingDeadSpace()&& cuttInProgress)
		{
			Debug.LogError("REFRESHING MOUSE TEAR POSITIONS");
			mouseTearPositions = new List<Vector3>();
			mousePrevPos = Input.mousePosition;
			//Flag we are done finding cutt/tear line
			doneCalculatingCuttLine = false;
			cuttInProgress = false;
			
			gapPosiitons = null;
		}
	}
	
	/// <summary>
	/// Updates the piece of paper original piece of paper
	/// </summary>
	private void Update_PieceOfPaper(TornObject to)
	{
		if(doneCalculatingCuttLine)
		{
			Debug.LogError("done calculating tear original");
			//This flag is for testing purposes only,
			//this flag is intented to be enabled from within unity to visuall debug and test tear line
			if(DrawTearLineONLY)
			{
				DrawTearLineOnly();
			}
			else
			{
				//This is used to parse through the mesh to determine which vertices and faces
				//belong to which cut piece
				FindNewCutPieces();	
				
				//Assign flag for moving cutt piece
				currentlyMovingCuttPiece = true;
				
				//Assign varaibles for moving piece
				screenPoint = Camera.main.WorldToScreenPoint(CurrentCuttPiece.transform.position);
    			offset = CurrentCuttPiece.transform.position - Camera.main.ScreenToWorldPoint(
        			new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
				
				//Now we set the center of the cuttObject, used for rotation
				centerOfCuttPieceRotOffset =  FindCenterOfCuttPiece();
			}
		}
		else if (currentlyMovingCuttPiece)
		{
			//Here the cuttPiece needs to be translated where ever the player desires
			MoveCuttPiceToDesiredLocation();
		}
	
		//The OnMouse functions are only called when mouse collides with an objects collider,
		//The following
		if(Input.GetMouseButtonDown(0) && PlayerTouchingDeadSpace() && !cuttInProgress)//PlayerTouchingDeadSpace() PlayerTouchingOffPaper()
		{
			//Debug.LogWarning("Updateing, Left Mouse DRAG");
			cuttInProgress = true;
			playerHasTouchedPaperThisTear = false;
			forceStopTear = false;
			OnMouseDown();
		}
		if(Input.GetMouseButton(0) && !forceStopTear && cuttInProgress)// && !PlayerTouchingDeadSpace())
		{
			//Debug.LogWarning("Updateing, Left Mouse DOWN");
			OnMouseDrag();
			
			if(PlayerTouchingPaper())
			{
				playerHasTouchedPaperThisTear = true;	
			}
			if(playerHasTouchedPaperThisTear &&  PlayerTouchingDeadSpace())//PlayerTouchingDeadSpace()  PlayerTouchingOffPaper()
			{
				playerHasTouchedPaperThisTear = false;
				forceStopTear = true;
			}
		}
		if((Input.GetMouseButtonUp(0)&& PlayerTouchingDeadSpace() && cuttInProgress) || forceStopTear)//PlayerTouchingDeadSpace()  PlayerTouchingOffPaper()
		{
			//The following prevents mouseUp from being called if the player just cutts the dead space
			if(forceStopTear || playerHasTouchedPaperThisTear)
			{
				OnMouseUp();
			}
		}
		else if(Input.GetMouseButtonUp(0)&& !PlayerTouchingDeadSpace()&& cuttInProgress)
		{
			Debug.LogError("REFRESHING MOUSE TEAR POSITIONS");
			mouseTearPositions = new List<Vector3>();
			mousePrevPos = Input.mousePosition;
			//Flag we are done finding cutt/tear line
			doneCalculatingCuttLine = false;
			cuttInProgress = false;
			
			gapPosiitons = null;
		}
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
	
	private Vector3 moveDirection;
	private Vector3 centerOfCuttPieceRotOffset;
	
	/// <summary>
	/// Moves the cutt pice to desired location after the player tears the paper
	/// </summary>
	private void MoveCuttPiceToDesiredLocation()
	{
		//Move the cutt piece
    	Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
    	Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);// + offset;
    	//CurrentCuttPiece.transform.position = curPosition;
		
		//Need to check for the condition making the player's decision finalized.
		if(Input.GetMouseButtonDown(0))
		{
			currentlyMovingCuttPiece = false;
		}
		
		//The following changes the rotation of the cutt pice
		//TODO: Tranform into touch input!
		if(Input.GetMouseButton(1))
		{
			//Perform rotation around the center position derived from averaging visible vertice locations
			CurrentCuttPiece.transform.RotateAround(centerOfCuttPieceRotOffset, new Vector3(0,0,1), test);
		}
		
	}
	
	float test = 10;
	
	/// <summary>
	/// Checks for and remove loops wihtin the tear curve
	/// </summary>
	private void CheckForAndRemoveLoops()
	{
		//Loop through the existing mouse positions list
		for(int itor = 0; itor < mouseTearPositions.Count; itor++)
		{
			for(int jtor = itor; jtor < mouseTearPositions.Count; jtor++)
			{
				//If we are not comparing the same modes wihtin list
				if(jtor != itor)
				{
					//Here, we check is the positions within the list are identical
					//TODO, ADD AOE radius, removing any mouseposiitons wihtin radius...
					if(mouseTearPositions.ElementAt(itor) == mouseTearPositions.ElementAt(jtor))
					{
						Debug.LogWarning("***LOOOOOP IN TEAR CURVE FOUND!!!***, now removing");
						//If this is true, then we remove every element inbetween itor and jtor within mouseTearPositions
						
						List<Vector3> noLoopCurve = new List<Vector3>();
						
						for(int index = 0; index <= itor; index++)
						{
							noLoopCurve.Add(mouseTearPositions.ElementAt(index));
						}
						for(int index2 = jtor + 1; index2 < mouseTearPositions.Count; index2++)
						{
							noLoopCurve.Add(mouseTearPositions.ElementAt(index2));
						}
					}
				}
			}
		}
	}
	
	/// <summary>
	/// Determined whether the mouses is in bounds of the world paper
	/// </summary>
	/// <returns>
	/// The in bounds.
	/// </returns>
	/// <param name='curPos'>
	/// If set to <c>true</c> current position.
	/// </param>
	private bool MouseInBounds(Vector3 curPos)
	{
		bool returnVal = false;
		
		//If the current mouse position is within beounds, then we return true
		if(curPos.x >= WIDTH_MIN && curPos.x <= WIDTH_MAX &&
			curPos.y >= HEIGHT_MIN && curPos.y <= HEIGHT_MAX)
		{
			returnVal = true;
		}
		
		return returnVal;	
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
						Camera.mainCamera.WorldToScreenPoint(newPos).y, Input.mousePosition.z));
			
		}
	}
	
	/// <summary>
	/// Draws the tear line only.
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
		for(int itor = 0; itor < mesh.vertices.Length; itor++)
		{
			if(mesh.vertices[itor].x > WIDTH_MAX)
			{
				WIDTH_MAX = mesh.vertices[itor].x;
			}
			else if(mesh.vertices[itor].x < WIDTH_MIN)
			{
				WIDTH_MIN = mesh.vertices[itor].x;
			}
			
			if(mesh.vertices[itor].y > HEIGHT_MAX)
			{
				HEIGHT_MAX = mesh.vertices[itor].y;
			}
			else if(mesh.vertices[itor].y < HEIGHT_MIN)
			{
				HEIGHT_MIN = mesh.vertices[itor].y;
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
		Clone = (GameObject) Instantiate(newPaper, newPaper.transform.position, newPaper.transform.rotation);
		
		Clone_2 = (GameObject) Instantiate(newPaper, newPaper.transform.position, newPaper.transform.rotation);
		Clone_2.GetComponent<TearPaper>().CloneObject = true;
		Clone_2.GetComponent<MeshRenderer>().enabled = false;
		
		//Turn off the original object's meshRenderer to hide
		newPaper.GetComponent<MeshRenderer>().enabled = false;
		
		//Set flag true to start deformations
		Clone.GetComponent<TearPaper>().CloneObject = true;
		
		//Loop through list storing any gameobject that can be torn
		foreach(GameObject go in ObjectsThatCanBeTorn)
		{
			//Create and determine mesh offset
			float newMeshOffset = 0;
			if(go.GetComponent<MeshFilter>().mesh.vertices.ElementAt(0).x != go.GetComponent<MeshFilter>().mesh.vertices.ElementAt(1).x)
			{
				newMeshOffset  = go.GetComponent<MeshFilter>().mesh.vertices.ElementAt(0).x - go.GetComponent<MeshFilter>().mesh.vertices.ElementAt(1).x;
				if(newMeshOffset < 0) newMeshOffset *= -1;
			}
			else
			{
				newMeshOffset  = go.GetComponent<MeshFilter>().mesh.vertices.ElementAt(0).y - go.GetComponent<MeshFilter>().mesh.vertices.ElementAt(1).y;
				if(newMeshOffset < 0) newMeshOffset *= -1;
			}
			
			//Now create new tearableObject
			TornObject newTearableObject = new TornObject(go, go.GetComponent<MeshFilter>().mesh, newMeshOffset, false);
			
			//Now add the object to tearableObjetsList for correctly updating tear
			TearableObjects.Add (newTearableObject);
		}
	}
	
	/// <summary>
	/// This function is used to see if the player is the touching an edge to begin a tear upon the paper object.
	/// We can check this by using the Max and Min Height and Width established within the Start method
	/// </summary>
	private bool PlayerTouchingEdge(bool checkingStartPos)
	{
		bool returnVal = false;
		
		//Loop through mesh vertices
		for (var itor = 0; itor < newMesh.Length; itor++)
		{
			//Find the screen poition of the vertex
			screenPoint = Camera.main.WorldToScreenPoint(Clone.GetComponent<MeshFilter>().mesh.vertices[itor]);
			
			//Get the current position of the mouse in the scene (world) with the same z-depth as the vertex being examined
    		Vector3 curPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
			
			//Now find the distance between the world mouse position and the vertex world position for distance comparison
			float dist = Vector3.Distance(curPosition, Clone.GetComponent<MeshFilter>().mesh.vertices[itor]);
			
			//Muliply the distance to widen the difference between the mouse and all mesh vertices
			dist *= 10000;
			
			//Check is the distance is less than 1500 (arbitrarily found through trial and error)
			if(dist < 1000 && (Clone.GetComponent<MeshFilter>().mesh.vertices[itor].x == WIDTH_MAX ||
				Clone.GetComponent<MeshFilter>().mesh.vertices[itor].x == WIDTH_MIN ||
				Clone.GetComponent<MeshFilter>().mesh.vertices[itor].y == HEIGHT_MAX ||
				Clone.GetComponent<MeshFilter>().mesh.vertices[itor].y == HEIGHT_MIN))
			{
				returnVal = true;
				cuttInProgress = true;
				//Break here because we only care about one vertex to cutt at a time
				if(checkingStartPos && !haveStoredStartPos)
				{
					startPos = Clone.GetComponent<MeshFilter>().mesh.vertices[itor];
					haveStoredStartPos = true;
				}
				else
				{
					endPos = Clone.GetComponent<MeshFilter>().mesh.vertices[itor];
				}
				
				break;
			}
		}
		return returnVal;
	}
	
	/// <summary>
	/// This function is used to see if the player is the touching the paper object.
	/// We can check this by using the Max and Min Height and Width established within the Start method
	/// </summary>
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
				cuttInProgress = true;
			}
			
			
		}
		//Debug.Log("Touching Paper = " + returnVal.ToString());
		return returnVal;
	}
	
	/// <summary>
	/// This function is used to see if the player is NOT the touching the paper object.
	/// We can check this by using the Max and Min Height and Width established within the Start method
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
	}
	
	/// <summary>
	/// Players the touching dead space is used to determine whether or not the player is touching off the level (paper Object)
	/// </summary>
	/// <returns>
	/// The touching dead space is true if the playing is touching off screen
	/// </returns>
	private bool PlayerTouchingDeadSpace()
	{
		bool returnVal = false;
		
		RaycastHit hit = new RaycastHit();
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		if(Physics.Raycast(ray, out hit, 1000.0f))
		{
			if(hit.transform.tag == "DeadSpace")
    		{
				returnVal = true;
				Debug.LogError("JUST HIT DEAD SPACE");
			}
		}
		
		//Debug.Log("Touching Off Paper = " + returnVal.ToString());
		return returnVal;
	}
	
	/// <summary>
	/// Turns the input into torn vert curve largest piece.
	/// </summary>
	private void TurnInputIntoTornVertCurveLargestPiece(TornObject tornPiece)
	{
		Vector3 screenDepth = Camera.main.WorldToScreenPoint(mesh.vertices[0]);//Clone.GetComponent<MeshFilter>()
		List<Vector3> newMouseTearWorldPos = new List<Vector3>();
		//First the mouseTearPositions needs to be translated into world positions
		for(int indexr = 0; indexr < mouseTearPositions.Count - 1; indexr++)
		{
			Vector3 curPosition = Camera.main.ScreenToWorldPoint(
				new Vector3(mouseTearPositions[indexr].x, mouseTearPositions[indexr].y, screenDepth.z));
			newMouseTearWorldPos.Add (curPosition);
		}
		mouseTearPositions = newMouseTearWorldPos;
		
		Vector3 previousTornVert = Clone.GetComponent<MeshFilter>().mesh.vertices[0];//Clone.GetComponent<MeshFilter>().
		int numberOfVerts = 0;
		
		Vector3 previousMousePos = mouseTearPositions.ElementAt(0);
		
		//foreach(Vector3 mousePos in mouseTearPositions)
		for(int jtor = 0; jtor < mouseTearPositions.Count - 1; jtor++)
		{
			//Find change in position of the mouse
			float mouseMoveDistX = mouseTearPositions[jtor].x - previousMousePos.x;
			float mouseMoveDistY = mouseTearPositions[jtor].y - previousMousePos.y;
			
			if(mouseMoveDistX < 0) mouseMoveDistX *= -1;
			if(mouseMoveDistY < 0) mouseMoveDistY *= -1;
			
			//If jtor == 0, we are at an edge, so we forsure add, else we only add every time
			// the mouse has moved the distance inbetween any two adjacent vertices on paper mesh
			if(jtor == 0 || mouseMoveDistX > (MESH_VERT_OFFSET * 1/3) || mouseMoveDistY > (MESH_VERT_OFFSET * 1/3))
			{
				//Debug.LogWarning("Mouse Has Moved the offset amount to add new vert");
				//Now, we know here we need to find the vert closest to the mouse position to add to the
				// tearLine
				
				float distToClosestRow = MESH_VERT_OFFSET * 20;
				int rowNum = 0;
				
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
				int colomnNum = 0;
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
				
				//Now rowNum and Colomn Num point to the new tornVert, so we add to tearLine list
				
				if(!tearLine.ContainsKey(paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum]))
				{
					tearLineTimer++;
					numberOfVerts++;
					
					//THE FOLLOWING NEEDS OPTIMIZATION, here we need to add the indice of the new torn vertice into tearline,
					// therefore the quickest, but slowest solution is to iterate through all mesh verts....
					int meshIndex = -1;
					for(int gtor = 0; gtor < mesh.vertices.Length; gtor++)
					{
						if(mesh.vertices.ElementAt(gtor) == paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum])
						{
							meshIndex = gtor;
							break;
						}
					}
					
					tearLine.Add(paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum], meshIndex);
					paperGrid_1[paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum]] = true;
						
					//This maps a 'time' to the tear vertice
					tearLineTime.Add (paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum], (float)tearLineTimer);
					tearLinePositionTime.Add ((float)tearLineTimer, paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum]);
					previousTornVert = paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum];
				}
				
				//Set previous the the position we just looked at
				previousMousePos = mouseTearPositions.ElementAt(jtor);
			}
		}
	}
	
	/// <summary>
	/// Turns the input into torn vert curve on tear object, not dependent upon 
	/// the player touching the deadSpace first
	/// </summary>
	private void TurnInputIntoTornVertCurveOnTearObject(TornObject tornPiece)
	{
		Vector3 screenDepth = Camera.main.WorldToScreenPoint(mesh.vertices[0]);//Clone.GetComponent<MeshFilter>()
		List<Vector3> newMouseTearWorldPos = new List<Vector3>();
		//First the mouseTearPositions needs to be translated into world positions
		for(int indexr = 0; indexr < mouseTearPositions.Count - 1; indexr++)
		{
			Vector3 curPosition = Camera.main.ScreenToWorldPoint(
				new Vector3(mouseTearPositions[indexr].x, mouseTearPositions[indexr].y, screenDepth.z));
			newMouseTearWorldPos.Add (curPosition);
		}
		mouseTearPositions = newMouseTearWorldPos;
		
		Vector3 previousTornVert = Clone.GetComponent<MeshFilter>().mesh.vertices[0];//Clone.GetComponent<MeshFilter>().
		int numberOfVerts = 0;
		
		Vector3 previousMousePos = mouseTearPositions.ElementAt(0);
		
		//foreach(Vector3 mousePos in mouseTearPositions)
		for(int jtor = 0; jtor < mouseTearPositions.Count - 1; jtor++)
		{
			//Find change in position of the mouse
			float mouseMoveDistX = mouseTearPositions[jtor].x - previousMousePos.x;
			float mouseMoveDistY = mouseTearPositions[jtor].y - previousMousePos.y;
			
			if(mouseMoveDistX < 0) mouseMoveDistX *= -1;
			if(mouseMoveDistY < 0) mouseMoveDistY *= -1;
			
			//for(int jtor = 0; jtor < 
			
			//If jtor == 0, we are at an edge, so we forsure add, else we only add every time
			// the mouse has moved the distance inbetween any two adjacent vertices on paper mesh
			if(jtor == 0 || mouseMoveDistX > (MESH_VERT_OFFSET * 1/3) || mouseMoveDistY > (MESH_VERT_OFFSET * 1/3))
			{
				//Debug.LogWarning("Mouse Has Moved the offset amount to add new vert");
				//Now, we know here we need to find the vert closest to the mouse position to add to the
				// tearLine
				
				float distToClosestRow = MESH_VERT_OFFSET * 20;
				int rowNum = 0;
				
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
				int colomnNum = 0;
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
				
				//Now rowNum and Colomn Num point to the new tornVert, so we add to tearLine list
				
				if(!tearLine.ContainsKey(paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum]))
				{
					tearLineTimer++;
					numberOfVerts++;
					
					//THE FOLLOWING NEEDS OPTIMIZATION, here we need to add the indice of the new torn vertice into tearline,
					// therefore the quickest, but slowest solution is to iterate through all mesh verts....
					int meshIndex = -1;
					for(int gtor = 0; gtor < mesh.vertices.Length; gtor++)
					{
						if(mesh.vertices.ElementAt(gtor) == paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum])
						{
							meshIndex = gtor;
							break;
						}
					}
					
					tearLine.Add(paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum], meshIndex);
					paperGrid_1[paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum]] = true;
						
					//This maps a 'time' to the tear vertice
					tearLineTime.Add (paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum], (float)tearLineTimer);
					tearLinePositionTime.Add ((float)tearLineTimer, paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum]);
					previousTornVert = paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum];
				}
				
				//Set previous the the position we just looked at
				previousMousePos = mouseTearPositions.ElementAt(jtor);
			}
		}
	}
	
	/// <summary>
	/// Turns the input into torn vert curve.
	/// </summary>
	private void TurnInputIntoTornVertCurve()
	{
		Vector3 screenDepth = Camera.main.WorldToScreenPoint(mesh.vertices[0]);//Clone.GetComponent<MeshFilter>()
		List<Vector3> newMouseTearWorldPos = new List<Vector3>();
		//First the mouseTearPositions needs to be translated into world positions
		for(int indexr = 0; indexr < mouseTearPositions.Count - 1; indexr++)
		{
			Vector3 curPosition = Camera.main.ScreenToWorldPoint(
				new Vector3(mouseTearPositions[indexr].x, mouseTearPositions[indexr].y, screenDepth.z));
			newMouseTearWorldPos.Add (curPosition);
		}
		mouseTearPositions = newMouseTearWorldPos;
		
		Vector3 previousTornVert = Clone.GetComponent<MeshFilter>().mesh.vertices[0];//Clone.GetComponent<MeshFilter>().
		int numberOfVerts = 0;
		
		Vector3 previousMousePos = mouseTearPositions.ElementAt(0);
		
		//foreach(Vector3 mousePos in mouseTearPositions)
		for(int jtor = 0; jtor < mouseTearPositions.Count - 1; jtor++)
		{
			//Find change in position of the mouse
			float mouseMoveDistX = mouseTearPositions[jtor].x - previousMousePos.x;
			float mouseMoveDistY = mouseTearPositions[jtor].y - previousMousePos.y;
			
			if(mouseMoveDistX < 0) mouseMoveDistX *= -1;
			if(mouseMoveDistY < 0) mouseMoveDistY *= -1;
			
			//If jtor == 0, we are at an edge, so we forsure add, else we only add every time
			// the mouse has moved the distance inbetween any two adjacent vertices on paper mesh
			if(jtor == 0 || mouseMoveDistX > (MESH_VERT_OFFSET * 1/3) || mouseMoveDistY > (MESH_VERT_OFFSET * 1/3))
			{
				//Debug.LogWarning("Mouse Has Moved the offset amount to add new vert");
				//Now, we know here we need to find the vert closest to the mouse position to add to the
				// tearLine
				
				float distToClosestRow = MESH_VERT_OFFSET * 20;
				int rowNum = 0;
				
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
				int colomnNum = 0;
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
				
				//Now rowNum and Colomn Num point to the new tornVert, so we add to tearLine list
				
				if(!tearLine.ContainsKey(paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum]))
				{
					tearLineTimer++;
					numberOfVerts++;
					
					//THE FOLLOWING NEEDS OPTIMIZATION, here we need to add the indice of the new torn vertice into tearline,
					// therefore the quickest, but slowest solution is to iterate through all mesh verts....
					int meshIndex = -1;
					for(int gtor = 0; gtor < mesh.vertices.Length; gtor++)
					{
						if(mesh.vertices.ElementAt(gtor) == paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum])
						{
							meshIndex = gtor;
							break;
						}
					}
					
					tearLine.Add(paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum], meshIndex);
					paperGrid_1[paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum]] = true;
						
					//This maps a 'time' to the tear vertice
					tearLineTime.Add (paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum], (float)tearLineTimer);
					tearLinePositionTime.Add ((float)tearLineTimer, paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum]);
					previousTornVert = paperGrid[paperGrid.Keys.ElementAt(rowNum)][colomnNum];
				}
				
				//Set previous the the position we just looked at
				previousMousePos = mouseTearPositions.ElementAt(jtor);
			}
		}
		
		
		//Debug.LogWarning("Number of verts torn = " + numberOfVerts.ToString());
	}
	
	/// <summary>
	/// Cutts the paper object along cutt verts ('tearLine').
	/// </summary>
	private void FindNewCutPieces()
	{
		//The following initialize the storages holding the new vertices and faces for the two new citt paper objects
		CuttPieceOneVerts = Clone.GetComponent<MeshFilter>().mesh.vertices;
		CuttPieceTwoVerts = Clone_2.GetComponent<MeshFilter>().mesh.vertices;
		CuttPieceOneFaces = Clone.GetComponent<MeshFilter>().mesh.triangles;
		CuttPieceTwoFaces = Clone_2.GetComponent<MeshFilter>().mesh.triangles;
		
		//First, we update the paperGrid_1 DICT for determining whether a vertex has been hit be the user input
		for(int itor = 0; itor < CuttPieceOneVerts.Length; itor++)
		{
			if(tearLine.ContainsKey(CuttPieceOneVerts[itor]))
			{
				paperGrid_1[new Vector2(CuttPieceOneVerts[itor].y, CuttPieceOneVerts[itor].x)] = true;
			}
			else
			{
				paperGrid_1[new Vector2(CuttPieceOneVerts[itor].y, CuttPieceOneVerts[itor].x)] = false;
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
				if (!tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator)[0]) && 
					tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator - 1)[0]))
				{
					//We increase the numEdgeTearVerts to keep track of which island we are adding to when
					//the first row's veretex IS NOT a torn vertex
					numEdgeTearVerts++;
				}
			}
			
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
				if(tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator)[jtor]) && !haveHitTearLine)
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
					//If we are not at the last position in the row AND 
					//If the next vertex in row in not torn, then we know
					//	we have finally hit the end of one of the current row's
					//	tear regions
					if((jtor + 1) < paperGrid.Values.ElementAt(iterator).Count && 
						!tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator)[jtor + 1]))
					{
						//This is set to false for triggering true for the possibility of the current
						//row containing another torn vertice region
						haveHitTearLine = false;
						
						//Flag we are now ready to make a decision based on incoming and outgoing direction
						//of curve about this tear region
						readyToDeterminShape = true;
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
			CreatNewCuttPaperMeshPieces(itor);
		}
		/*
		
		Vector3[] newIsland1 = new Vector3[island1.Count()];
		Vector3[] newIsland2 = new Vector3[island2.Count()];
		
		int indexIsland1 = 0;
		int indexIsland2 = 0;
		
		for(int itor = 0; itor < Clone.GetComponent<MeshFilter>().mesh.vertices.Count(); ++itor)
		{
			if(island1.Contains(new Vector2(Clone.GetComponent<MeshFilter>().mesh.vertices[itor].x, Clone.GetComponent<MeshFilter>().mesh.vertices[itor].y)))
			{
				Debug.LogError("test 1");
				newIsland1[indexIsland1] = Clone.GetComponent<MeshFilter>().mesh.vertices[itor];
				++indexIsland1;

			}
			if(island2.Contains(new Vector2(Clone.GetComponent<MeshFilter>().mesh.vertices[itor].x, Clone.GetComponent<MeshFilter>().mesh.vertices[itor].y)))
			{
				Debug.LogError("test 2");
				newIsland2[indexIsland2] = Clone.GetComponent<MeshFilter>().mesh.vertices[itor];//new Vector3(island1.ElementAt(itor1).y, island2.ElementAt(itor1).x, 0);
				++indexIsland2;
				
				
			}
		}
		
		int[] newIsland1indicies = island1Indicies;
		int inderIsland1 = 0;
		int[] newIsland2indicies = island2Indicies;
		int inderIsland2 = 0;
		
		for(int finder = 0; finder < island1Indicies.Length; finder += 1)
		{
			for(int finder2 = 0; finder2 < newIsland1.Length; finder2++)
			{
				if(Clone.GetComponent<MeshFilter>().mesh.vertices[island1Indicies[finder]] == newIsland1[finder2])
				{
					newIsland1indicies[inderIsland1] = finder;
					++inderIsland1;
				}
			}
		}
		island1Indicies = newIsland1indicies;
		
		

		
		Mesh newMesh1 = Clone.GetComponent<MeshFilter>().mesh;
		Mesh newMesh2 = Clone_2.GetComponent<MeshFilter>().mesh;
		
		newMesh1.Clear();
		newMesh2.Clear();
		
		newMesh1.vertices = newIsland1;
		newMesh2.vertices = newIsland2;
		
		newMesh1.triangles = island1Indicies;
		newMesh2.triangles = island2Indicies;
		
		newMesh1.RecalculateBounds();
		newMesh1.RecalculateNormals();
		newMesh2.RecalculateBounds();
		newMesh2.RecalculateNormals();
		
		Clone.GetComponent<MeshFilter>().mesh = newMesh1;
		Clone_2.GetComponent<MeshFilter>().mesh = newMesh2;
		*/
		//Reassign new mesh triangles, defining the new faces for the cloned object
		
		
		
		List<int> newIsland1indTempList = new List<int>();
		int numOfvertsIsland1 = 0;
		bool haveStarted = false;
		for(int itor = island1Indicies.Count() - 1; itor >  -1; --itor)
		{
			if(island1Indicies[itor] != 0 || haveStarted)
			{
				haveStarted = true;
				newIsland1indTempList.Add(island1Indicies[itor]);
				++numOfvertsIsland1;
			}
		}
		int[] newIsland1Ind = new int[numOfvertsIsland1];
		int indexer = 0;
		for(int itor = newIsland1indTempList.Count() - 1; itor >  -1; --itor)
		{
			newIsland1Ind[indexer] = newIsland1indTempList[itor];
			++indexer;
		}
		island1Indicies = newIsland1Ind;
		
		List<int> newIsland2indTempList = new List<int>();
		int numOfvertsIsland2 = 0;
		bool haveStarted2 = false;
		for(int itor = island2Indicies.Count() - 1; itor >  -1; --itor)
		{
			if(island2Indicies[itor] != 0 || haveStarted2)
			{
				haveStarted2 = true;
				newIsland2indTempList.Add(island2Indicies[itor]);
				++numOfvertsIsland2;
			}
		}
		int[] newIsland2Ind = new int[numOfvertsIsland2];
		int indexer2 = 0;
		for(int itor = newIsland2indTempList.Count() - 1; itor >  -1; --itor)
		{
			newIsland2Ind[indexer2] = newIsland2indTempList[itor];
			++indexer2;
		}
		island2Indicies = newIsland2Ind;
		
		Clone.GetComponent<MeshFilter>().mesh.triangles = island1Indicies;
		Clone_2.GetComponent<MeshFilter>().mesh.triangles = island2Indicies;
		
		/*
		Clone.GetComponent<MeshFilter>().mesh.RecalculateBounds();
		Clone.GetComponent<MeshFilter>().mesh.RecalculateNormals();
		Clone_2.GetComponent<MeshFilter>().mesh.RecalculateBounds();
		Clone_2.GetComponent<MeshFilter>().mesh.RecalculateNormals();
		*/
		/*
		Clone.GetComponent<MeshFilter>().mesh.vertices = newIsland2;
		Clone.GetComponent<MeshFilter>().mesh.RecalculateBounds();
		Clone.GetComponent<MeshFilter>().mesh.RecalculateNormals();
		Clone_2.GetComponent<MeshFilter>().mesh.vertices = newIsland1;
		Clone_2.GetComponent<MeshFilter>().mesh.RecalculateBounds();
		Clone_2.GetComponent<MeshFilter>().mesh.RecalculateNormals();
		
		
		Clone.GetComponent<MeshFilter>().mesh.triangles = island1Indicies;
		Clone_2.GetComponent<MeshFilter>().mesh.triangles = island2Indicies;
		*/
		
		//Update the clone's mesh collider
		Clone.GetComponent<MeshCollider>().sharedMesh = Clone.GetComponent<MeshFilter>().mesh;
		Clone_2.GetComponent<MeshCollider>().sharedMesh = Clone_2.GetComponent<MeshFilter>().mesh;

		//Rename the object based on the number of faces in each of the new meshs
		if(island2.Count > island1.Count)
		{
			//Assign pointer towards object currently being moved and rotated by player
			CurrentCuttPiece = Clone_2;
			//Now move the piece being moved up a layer
			Clone_2.transform.position = new Vector3(Clone_2.transform.position.x, Clone_2.transform.position.y, Clone_2.transform.position.z + 0.001f);
			
			//Now we add the new clones to list of tearbleObjects
			TornObject newTornPiece = new TornObject(Clone_2, Clone_2.GetComponent<MeshFilter>().mesh, MESH_VERT_OFFSET, false);
			//GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearObject>().
			TearableObjects.Add (newTornPiece);
			
			TornObject newTornPiece2 = new TornObject(Clone, Clone.GetComponent<MeshFilter>().mesh, MESH_VERT_OFFSET, true);
			TearableObjects.Add (newTornPiece2);
			
			Clone_2.name = "paper_CuttPieceOfPaper";
			Clone.name = "paper_LargerPiece";
			Clone_2.GetComponent<MeshRenderer>().material.color = Color.green;
		}
		else
		{
			//Assign pointer towards object currently being moved and rotated by player
			CurrentCuttPiece = Clone;
			//Now move the piece being moved up a layer
			Clone.transform.position = new Vector3(Clone.transform.position.x, Clone.transform.position.y, Clone.transform.position.z + 0.001f);
			
			//Now we add the new clones to list of tearbleObjects
			TornObject newTornPiece = new TornObject(Clone, Clone.GetComponent<MeshFilter>().mesh, MESH_VERT_OFFSET, false);
			//GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearObject>().
			TearableObjects.Add (newTornPiece);
			TornObject newTornPiece2 = new TornObject(Clone_2, Clone_2.GetComponent<MeshFilter>().mesh, MESH_VERT_OFFSET, true);
			TearableObjects.Add (newTornPiece2);
			
			Clone.name = "paper_CuttPieceOfPaper";
			Clone_2.name = "paper_LargerPiece";
			Clone.GetComponent<MeshRenderer>().material.color = Color.green;
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
		if(paperGrid_1[pos])//tearLine.ContainsKey(endPosTearPos))//paperGrid_1[pos]
		{
			currentlyInTransition = true;
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

			//Reset flags for next function iteration through this function
			//if(listIndice != tempList.Count - 1)
			//{
				//Make sure the tearLine is not makeing a 'U-like' turn
				if((TearLineMovingInSLikeShape(startTearPos, endPosTearPos, listIndice, startingVertIndice, tempList) && readyToDeterminShape)) 
				{
					//Rotate between the two new islands
					if(addingToPieceOne) addingToPieceOne = false;
					else addingToPieceOne = true;
				}
			//}
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
		
		//Find the time associated wiht the end time tear position
		int endTime = (int)tearLineTime[endTearPos];
		
		//Find the time associated wiht the start time tear position
		int startTime = (int)tearLineTime[startTearPos];
		
		//We determine if the time is switched in the given row to determine which way to look when
		//find ing which way the durve is moving
		bool timeSwitched = false;
		int sTime = (int)tearLineTime[startTearPos];
		int eTime = (int)tearLineTime[endTearPos];
		if(sTime > eTime) timeSwitched = true;
		
		//Find y-component of curve coming into starting position
		float StartChangeInY = startTearPos.y;
		
		//if(startTearPos.x != WIDTH_MIN//startTearPos.x != WIDTH_MAX && 
		//	)
			//&&
			//startTearPos.y != HEIGHT_MAX && startTearPos.y != HEIGHT_MIN)
		{
			StartChangeInY = ChangeInHeightTearVerticeDir(startTearPos, true, timeSwitched);
		}
		
		///Find y-component of curve leaving ending position
		float EndChangeInY = endTearPos.y;
		//if(endTearPos.x != WIDTH_MIN//endTearPos.x != WIDTH_MAX && 
		//	)
			//&&
			//endTearPos.y != HEIGHT_MAX && endTearPos.y != HEIGHT_MIN)
		{
			EndChangeInY = ChangeInHeightTearVerticeDir(endTearPos, false, timeSwitched);
		}
		
		//Here we are checking is the current decision involves an adge vertice
		if(endTearPos.y == HEIGHT_MAX || endTearPos.y == HEIGHT_MIN ||
			startTearPos.y == HEIGHT_MAX || startTearPos.y == HEIGHT_MIN
			||
			endTearPos.x == WIDTH_MAX || endTearPos.x == WIDTH_MIN ||
			startTearPos.x == WIDTH_MAX || startTearPos.x == WIDTH_MIN)
		{
			//Testing
			Debug.LogWarning("DESCISION AT SHAPE IS TRUE BECAUSE WE ARE TOUCHING AN EDGE DURING THIS DECISION!!!!");
			
			returnVal = true;
		}
			
		//Edge case clearification
		else if(((startTearPos.x == WIDTH_MIN// endTearPos.x == WIDTH_MIN //endTearPos.x == WIDTH_MAX ||
			//||
			//endTearPos.y == HEIGHT_MAX || endTearPos.y == HEIGHT_MIN
			) 
			&&
			EndChangeInY > endTearPos.y))
		{
			//Here, we continue adding to the same island
			returnVal = false;
		}
		else if(((startTearPos.x == WIDTH_MIN //startTearPos.x == WIDTH_MAX || 
			//||
			//startTearPos.y == HEIGHT_MAX || startTearPos.y == HEIGHT_MIN
			) &&
			EndChangeInY < endTearPos.y))
		{
			returnVal = true;
		}
		else if((StartChangeInY > startTearPos.y && EndChangeInY > endTearPos.y) ||
			(StartChangeInY < startTearPos.y && EndChangeInY < endTearPos.y))
		{
			returnVal = false;
		}
		else if((StartChangeInY > startTearPos.y && EndChangeInY < endTearPos.y) ||
			(StartChangeInY < startTearPos.y && EndChangeInY > endTearPos.y))
		{
			returnVal = true;
		}
		
		return returnVal;
	}
	
	/// <summary>
	/// Determined the change in height from the next/previous teart vertices in tearline dependent
	/// upon whether or not startVertFlag is true
	/// </summary>
	/// <returns>
	/// Returns the change in height
	/// </returns>
	/// <param name='tornVertPos'>
	/// Torn vert position.
	/// </param>
	/// <param name='startVertFlag'>
	/// Start vert flag determines whether or not we are currently checking the starting or ending vert position
	/// </param>
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
				///*
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
				//*/
				
				//Vector3 futureVert = tearLinePositionTime[(float)(timeOfCurrent + 1)];
				//returnVal = futureVert.y;
			}
			else
			{
				///*
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
				//*/
				//Vector3 prevVert = tearLinePositionTime[(float)(timeOfCurrent - 1)];
				//returnVal = prevVert.y;
			}
		}
		else
		{
			//If time is switched, we know that the startingPosition has a greater time than the endingPosition,
			//therefore, we access the directions by using the timescale backwards
			if(timeSwitched)
			{
				///*
				Vector3 prevVert1;
				if(tearLinePositionTime.ContainsKey((float)(timeOfCurrent - 1)))
				{
					prevVert1 = tearLinePositionTime[(float)(timeOfCurrent - 1)];
				}
				else
				{
					prevVert1 = tornVertPos;
				}
				returnVal = prevVert1.y;
				//*/
				//Vector3 prevVert1 = tearLinePositionTime[(float)(timeOfCurrent - 1)];
				//returnVal = prevVert1.y;
			}
			else
			{
				///*
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
				//*/
				
				//Vector3 futureVert1 = tearLinePositionTime[(float)(timeOfCurrent + 1)];
				//returnVal = futureVert1.y;
			}
		}
		
		//Return the value
		return returnVal;
	}
	
	/// <summary>
	/// Creats the new cutt paper mesh pieces.
	/// </summary>
	/// <param name='index'>
	/// Index.
	/// </param>
	private void CreatNewCuttPaperMeshPieces(int index)
	{
		if( island2.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index]].y, CuttPieceOneVerts[CuttPieceOneFaces[index]].x)) && 
			island2.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].x)) && 
			island2.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].x)))
		{
			//Debug.Log("itor = Test2_ ENTER = " + itor.ToString());
			island1Indicies[indexor1] = CuttPieceOneFaces[index];
			island1Indicies[indexor1 + 1] = CuttPieceOneFaces[index + 1];
			island1Indicies[indexor1 + 2] = CuttPieceOneFaces[index + 2];
			indexor1 += 3;
				
		}
		else
		{
			//Debug.Log("jtor = Test1_ ENTER = " + jtor.ToString());
			island2Indicies[indexor2] = CuttPieceTwoFaces[index];
			island2Indicies[indexor2 + 1] = CuttPieceTwoFaces[index + 1];
			island2Indicies[indexor2 + 2] = CuttPieceTwoFaces[index + 2];
			indexor2 += 3;
		}
	}
	
	#endregion
}
