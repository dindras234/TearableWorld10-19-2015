/// <summary>
/// 
/// FILE: 
/// 	Test_ script_ cutt
/// 
/// DESCRIPTION: 
/// 	This file is used as a testing script for cutting world paper.
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

public class Test_Script_Cutt : MonoBehaviour {
	
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
	public const float MESH_VERT_OFFSET = 0.125f;

	
	#endregion
	
	#region Private Fields
	/// <summary>
	/// The screen point is used to store the position being 
	/// compared to an input screen position
	/// </summary>
	private Vector3 screenPoint;
	
	/// <summary>
	/// The index is used to store a specific index into the vertex array, flaging
	/// the vertex hit by user input after distance check
	/// </summary>
	private int Index;
	
	/// <summary>
	/// The new mesh represents the new mesh of the object after a cutt occurs
	/// </summary>
	Vector3[] newMesh;
	
	/// <summary>
	/// The clone is used for keeping track of the cloned instance for mesh 
	/// deformation and cutting
	/// </summary>
	private GameObject Clone;
	
	/// <summary>
	/// The clone_2 is used for keeping track of the cloned instance for mesh 
	/// deformation and cutting
	/// </summary>
	private GameObject Clone_2;
	
	/// <summary>
	/// This flag is used to determine whether or not a face needs to 
	/// be deleted (this is riggered when the user cuts the paper
	/// </summary>
	private bool NeedToDeleteFace;
	
	/// <summary>
	/// The mesh verts store the mesh vertices into a dictionary for easy lookup
	/// TODO
	/// </summary>
	private Dictionary<Vector3, Vector3> meshVerts;
	
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
	/// This is used to flag once a cutt has started to stop detecting for edge vertice
	/// </summary>
	private bool cuttInProgress = false;
	
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
	/// The currently calculating cutt line flag is used to determine when the player is currently cutting
	/// </summary>
	private bool currentlyCalculatingCuttLine = false;
	
	/// <summary>
	/// The done calculating cutt line flag is used to determine when the player is currently cutting
	/// </summary>
	private bool doneCalculatingCuttLine = false;
	
	/// <summary>
	/// The tear line defined to store the vertices the playing is trying to tear along
	/// </summary>
	private Dictionary<Vector3, int> tearLine;
	
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
	/// The paper grid_1 stores the mesh coordinates into a more accessible storage
	/// </summary>
	private Dictionary<Vector2, bool> paperGrid_1;
	
	/// <summary>
	/// The paper grid organizes the mesh into a grid like structure for determineing torn pieces of paper
	/// </summary>
	private Dictionary<float, List<Vector3>> paperGrid;
	
	/// <summary>
	/// The paper grid organizes the mesh into a grid like structure for determineing torn pieces of paper
	/// </summary>
	private Dictionary<float, List<Vector3>> paperGrid_Vertical;
	
	/// <summary>
	/// The tear line time is used to store the time of the tear line input. This is sued to distinguish between
	///  U-like and S-like shaped tear curves
	/// </summary>
	private Dictionary<Vector3, float> tearLineTime;
	
	/// <summary>
	/// The tear line time is used to store the time of the tear line input. This is sued to distinguish between
	///  U-like and S-like shaped tear curves
	/// </summary>
	private Dictionary<float, Vector3> tearLinePositionTime;
	
	/// <summary>
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
	/// The island_1 list stores the vertices assocciated with the first 'island' of mesh faces, vertically
	/// </summary>
	private List<Vector2> island1_Vertical;
	
	/// <summary>
	/// The island_1 list stores the vertices assocciated with the first 'island' of mesh faces, vertically
	/// </summary>
	private List<Vector2> island2_Vertical;
	
	/// <summary>
	/// The island_1 list stores the vertices assocciated with the first 'island' of mesh faces, vertically
	/// </summary>
	private List<Vector2> island1_2;
	
	/// <summary>
	/// The island_1 list stores the vertices assocciated with the first 'island' of mesh faces, vertically
	/// </summary>
	private List<Vector2> island2_2;
	
	/// <summary>
	/// The island_1 list stores the vertices assocciated with the first 'island' of mesh faces, vertically
	/// </summary>
	private List<Vector2> island1_2_2;
	
	/// <summary>
	/// The island_1 list stores the vertices assocciated with the first 'island' of mesh faces, vertically
	/// </summary>
	private List<Vector2> island2_2_2;
	
	/// <summary>
	/// The adding to piece one flags between both new mesh object for correct new face assignment
	/// </summary>
	private bool addingToPieceOne;
	
	
	#endregion
	
	#region Methods
	/// <summary>
	/// Start this instance, used for initialization
	/// </summary>
	private void Start () 
	{
		//Initially, we set the paper's screen position to be zero
		screenPoint = new Vector3(0.0f, 0.0f, 0.0f);
		
		//Initialize the newMesh to be the same length of the previous
		newMesh = new Vector3[mesh.vertices.Length];
		
		//Create a clone world mesh for deformation, we do not change the original world gameobject mesh
		if (!CloneObject) CreateNewPaperWorld();
		
		//Initialize the max and min world values of mesh mertex coordinates
		WIDTH_MAX = -100;
	 	WIDTH_MIN = 100;
		HEIGHT_MAX = -100;
		HEIGHT_MIN = 100;
		
		//Now we check for the paperBounds for edge to edge tearing
		SetBoundsOfPaper();
		
		//init the dictionary storing the vertices along the tear line and their associated index
		//into the the mesh.vertice array
		tearLine = new Dictionary<Vector3, int>();
		paperGrid = new Dictionary<float, List<Vector3>>();
		tearLineTime = new Dictionary<Vector3, float>();
		tearLinePositionTime = new Dictionary<float, Vector3>();
		paperGrid_Vertical = new Dictionary<float, List<Vector3>>();
		paperGrid_1 = new Dictionary<Vector2, bool>();
		
		//Set the tearLineTimer to zero initially as the starting tear line 'time'
		tearLineTimer = 0;
		
		int testFreq = 0;
		//Load grid dictionary
		for(int itor = 0; itor < mesh.vertices.Length; itor++)
		{
			//Debug.Log("Vert = " + mesh.vertices[itor].ToString());
			//Row dependent, therefore we store the height first
			paperGrid_1.Add(new Vector2(mesh.vertices[itor].y, mesh.vertices[itor].x), false);
			++testFreq;
			
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
						
			
			if(!paperGrid_Vertical.ContainsKey(mesh.vertices[itor].x))
			{
				List<Vector3> valueDict = new List<Vector3>();
				valueDict.Add(new Vector3(mesh.vertices[itor].x, mesh.vertices[itor].y, mesh.vertices[itor].z));
				paperGrid_Vertical.Add(mesh.vertices[itor].x, valueDict);
			}
			else
			{
				List<Vector3> newList = paperGrid_Vertical[mesh.vertices[itor].x];
				newList.Add(new Vector3(mesh.vertices[itor].x, mesh.vertices[itor].y, mesh.vertices[itor].z));
				paperGrid_Vertical[mesh.vertices[itor].x] = newList;
			}
		}
		
		//Debug.Log("Number of verts in original mesh = " + testFreq.ToString());
		
		//bubble sort each list within the dictionary
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
		
		List<float> tempList122 = paperGrid.Keys.ToList();
		tempList122.Sort();

		
		Dictionary<float, List<Vector3>> newPaperGrid = new Dictionary<float, List<Vector3>>();
		
		for(int indexr = 0; indexr < tempList122.Count; indexr++)
		{
			List<Vector3> newList2 = paperGrid[tempList122.ElementAt(indexr)];
			float index2 = tempList122.ElementAt(indexr);
			newPaperGrid.Add(index2, newList2);
		}
		paperGrid = newPaperGrid;
				
		
		//bubble sort each list within the dictionary
		foreach(List<Vector3> list in paperGrid_Vertical.Values)
		{
			for(int itor = 0; itor < list.Count; itor++)
			{
				for(int jtor = 0; jtor < list.Count; ++jtor)
				{
					if(	list[jtor].y < list[itor].y )
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
		
		List<float> tempList1223 = paperGrid_Vertical.Keys.ToList();
		tempList122.Sort();
		
		Dictionary<float, List<Vector3>> newPaperGrid2 = new Dictionary<float, List<Vector3>>();
		
		for(int indexr = 0; indexr < tempList1223.Count; indexr++)
		{
			List<Vector3> newList = paperGrid_Vertical[tempList1223.ElementAt(indexr)];
			float index = tempList1223.ElementAt(indexr);
			newPaperGrid2.Add(index, newList);
		}
		paperGrid_Vertical = newPaperGrid2;
		
		//Testing*****
		/*
		foreach(List<Vector3> vecList in paperGrid.Values)
		{
			string printMe = "";
			foreach(Vector3 vec in vecList)
			{
				printMe += ("__" + vec.y.ToString());
			}
			Debug.LogWarning(printMe);
		}*/
		
		//At this point, we have completed creating the grid
		
		
		//testing!
		//CuttPieceOneVerts = new Vector3[mesh.vertices.Length];
		//CuttPieceOneVerts = mesh.vertices;
		
	}
	public bool NewCutLineMethod;
	public bool DrawCutt;
	/// <summary>
	/// Update this instance, called once per frame
	/// </summary>
	private void Update () 
	{
		//Debug.Log ("MaxWidth = " + WIDTH_MAX + " . MinWidth = " + WIDTH_MIN + " . MaxHeight = " + HEIGHT_MAX + " . MinHeight = " + HEIGHT_MIN);
		
		//for(int itor = 0; itor < CuttPieceOneVerts.Length; itor++)
		//{
		//	Debug.Log("Mesh.vert #" + itor + " position = " + CuttPieceOneVerts[itor].ToString());
		//}
		
		//We check to see if the player is done calculating their tear/cutt line along the paper/plane object
		if(doneCalculatingCuttLine)
		{
			Debug.Log("Currently DoneCalculatingCuttLine");
			if(!DrawCutt)
			{
				if(NewCutLineMethod)
				{
					//DetermineNewIslandsFromTearVertices();
				}
				else
				{
					Debug.Log("Currently entering CuttPaperObjectAlongTearVerts()");
					CuttPaperObjectAlongTearVerts();
				}
			}
			else
			{
			
			/*
			//TESTING printing out the torn verts through cebug window
			foreach(List<Vector3> vecs in paperGrid.Values)
			{
				string row = "";
				
				foreach(Vector3 vec in vecs)
				{
					if(tearLine.ContainsKey(vec))
					{
						row += ".";
					}
					else
					{
						row += "_";
					}
				}
				
				
				Debug.Log(row.ToString());
			}*/

			
			
			//Testing - > THE FOLLOWING SHOWS THE CUTT LINE!
			//UpdateTearInputLine();
			int[] newFaces = new int[Clone.GetComponent<MeshFilter>().mesh.triangles.Length];
			int intorator = 0;
			//int triangleCount = 1;
			for(int itor = 0; itor < Clone.GetComponent<MeshFilter>().mesh.triangles.Length; itor += 3)
			{
				if(tearLine.ContainsValue(Clone.GetComponent<MeshFilter>().mesh.triangles[itor]) &&
					tearLine.ContainsValue(Clone.GetComponent<MeshFilter>().mesh.triangles[itor + 1]) &&
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
		}
	}
	
	/// <summary>
	/// Raises the mouse drag event.
	/// </summary>
	void OnMouseDrag()
	{
		//CuttPaperWorldMesh();
		//CuttWorld();
		TearPaper();
	}
	
	/// <summary>
	/// Raises the mouse up event.
	/// </summary>
	void OnMouseUp()
	{	
		//Debug.Log ("MouseUpTriggered, PlayerTouchingEdge() = " + PlayerTouchingEdge(false).ToString() + " . cuttInProgress = " + cuttInProgress.ToString() + " . CloneObject = " + CloneObject.ToString());
		//Now, we check to see if the player is touching an edge to complete their cutt/tear
		if(PlayerTouchingEdge(false) && cuttInProgress)//PlayerTouchingEdge()
		{
			
			UpdateTearInputLine();
			
			//Debug.Log ("MouseUpTriggered _DONE");
			//Save old mesh information
			Clone.GetComponent<MeshFilter>().mesh.vertices = originalMeshVerts;
			Clone.GetComponent<MeshFilter>().mesh.triangles = originalMeshTriangles;	
			
			//Flag we are done finding cutt/tear line
			doneCalculatingCuttLine = true;
		}
		
		//The cutt has now stopped
		if(cuttInProgress) cuttInProgress = false;
	}
	
	/// <summary>
	/// Raises the mouse down event.
	/// </summary>
	void OnMouseDown()
	{
		mouseTearPositions = new List<Vector3>();
		Debug.Log("Enter MouseDown");
		if(!cuttInProgress)
		{
			//Save old mesh information
			originalMeshVerts = Clone.GetComponent<MeshFilter>().mesh.vertices;//mesh.vertices;
			originalMeshTriangles = Clone.GetComponent<MeshFilter>().mesh.triangles;//mesh.triangles;
		}
	}
	
	private int indexor1 = 0;
	private int indexor2 = 0;
	private int[] island1Indicies; 
	private int[] island2Indicies;
	
	
	public bool TestingMouseTime;
	
	
	/// <summary>
	/// Determines the new islands from tear vertices.
	/// </summary>
	private void DetermineNewIslandsFromTearVertices()
	{
		//Debug.Log("Just entered cutt paper along tear line (Verts)");
		CuttPieceOneVerts = Clone.GetComponent<MeshFilter>().mesh.vertices;
		CuttPieceTwoVerts = Clone.GetComponent<MeshFilter>().mesh.vertices;
		CuttPieceOneFaces = Clone.GetComponent<MeshFilter>().mesh.triangles;
		CuttPieceTwoFaces = Clone.GetComponent<MeshFilter>().mesh.triangles;
		
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
		
		//Now we know which verticies are selected and how to distinguish both islands thanks to the organized paper vert grid
		// found previously
		
		//Now, we create two lists of verticies to determine which vertices belong to which 'island'
		island1 = new List<Vector2>();
		island2 = new List<Vector2>();
		
		//The following is used to switch in between both new meshes for data transfer after cutt/tear
		addingToPieceOne = true;
		bool currentlyInTransition = false;
		
		//int testNum = 0;
		int numEdgeTearVerts = 0;
		
		

		int startingVertIndice = 0;
		
		
		//We looop through the sorted paperGrid to create two new mesh based of tearLine
		for(int iterator = 0; iterator < paperGrid.Values.Count; iterator++)//each(List<Vector3> tempList in paperGrid.Values)
		{
			
			//If the fist node in list is in tearLine verts, increase number of tear verts long edge count
			//AND if the tear line does NOT include the vertice directly below

			bool haveHitTearLine = false;
			Vector3 rowTearStartPos = Vector3.zero;
			Vector3 endPosTearStartPos = Vector3.zero;
			bool readyToDeterminShape = false;
			
			bool addingToislandOneROWCheck = addingToPieceOne;
			for(int jtor = 0; jtor < paperGrid.Values.ElementAt(iterator).Count; jtor++)
			{
				if(jtor == 0)
				{
					addingToPieceOne = true;
					if (!tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator)[jtor]))
					{
						if(iterator - 1 >= 0 && tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator - 1)[jtor]))//TODO
						{
							numEdgeTearVerts++;
						}
					}
					if(numEdgeTearVerts % 2 == 0)// && addingToPieceOne)
					{
						Debug.LogWarning("NUMBER OF EDGE TEAR VERTICES HAS CHANGED, true now!!!! itor = " + iterator.ToString());
						addingToPieceOne = true;
					}
					else if(numEdgeTearVerts % 2 == 1)// && !addingToPieceOne)
					{
						Debug.LogWarning("NUMBER OF EDGE TEAR VERTICES HAS CHANGED, false now!!!! itor = " + iterator.ToString());
						addingToPieceOne = false;
					}
				}
				
				
				if(tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator)[jtor]) && !haveHitTearLine)
				{
					haveHitTearLine = true;
					rowTearStartPos = paperGrid.Values.ElementAt(iterator)[jtor];
					startingVertIndice = jtor;
					readyToDeterminShape = false;
					Debug.LogWarning("Current vert = " + paperGrid.Values.ElementAt(iterator)[jtor].ToString() + " has hit tearvert in row = " + iterator.ToString());
				}
				
				if(haveHitTearLine)
				{
					
					if((iterator + 1) <= (paperGrid.Values.Count - 1) && !tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator + 1)[jtor]))
					{
						haveHitTearLine = false;
						readyToDeterminShape = true;
						//endPosTearStartPos = paperGrid.Values.ElementAt(iterator + 1)[jtor];
					}

					
					endPosTearStartPos = paperGrid.Values.ElementAt(iterator)[jtor];
				}
				
				//In the following, the x==y and y==x already
				TraverseGridAddToNewPiece2(paperGrid.Values.ElementAt(iterator)[jtor], new Vector2(paperGrid.Values.ElementAt(iterator)[jtor].y, paperGrid.Values.ElementAt(iterator)[jtor].x), currentlyInTransition, 
					jtor, paperGrid.Values.ElementAt(iterator), rowTearStartPos, endPosTearStartPos, readyToDeterminShape, startingVertIndice);
			}
			
		}
		
	
		
		Clone_2.GetComponent<MeshRenderer>().enabled = true;
		
		island1Indicies = new int[CuttPieceOneFaces.Length];
		island2Indicies = new int[CuttPieceTwoFaces.Length];
		
		indexor1 = 0;
		indexor2 = 0;
		
		//Now we have the associates vertices stored within island1 and island 2
		//We can now determine the new faces for each mesh by FIRST finding the indices for each vertice
		for(int itor = 0; itor < CuttPieceOneFaces.Length; itor += 3)
		{
			//Debug.Log("CHECKING WHAT TYPE OF CUTT");
			if(SimpleCutt)
			{
				TestFaceChecking(itor);
			}
			else
			{
				if(TestingMouseTime)
				{
					CreatNewCuttPaperMeshPieces(itor);
				}
				else
				{
					CompareIslandSweeps(itor);
				}
			}
			
		}
		//Debug.Log("Number of polyGons added to new Island mesh #2 = " +  indexor2.ToString());
		
		
		Clone.GetComponent<MeshFilter>().mesh.triangles = island1Indicies;
		Clone.GetComponent<MeshCollider>().sharedMesh = Clone.GetComponent<MeshFilter>().mesh;	
		
		Clone_2.GetComponent<MeshFilter>().mesh.triangles = island2Indicies;	
		Clone_2.GetComponent<MeshCollider>().sharedMesh = Clone_2.GetComponent<MeshFilter>().mesh;
		
		
		//int dist1 = island2Indicies.Length;
		//int dist2 = island1Indicies.Length;
		
		if(Clone_2.GetComponent<MeshFilter>().mesh.triangles.Length < Clone.GetComponent<MeshFilter>().mesh.triangles.Length)
		{
			Clone_2.name = "paper_CuttPieceOfPaper";
			Clone.name = "paper_LargerPiece";
		}
		else
		{
			Clone.name = "paper_CuttPieceOfPaper";
			Clone_2.name = "paper_LargerPiece";
		}
		
		doneCalculatingCuttLine = false;
	}
	
	
	/// <summary>
	/// Cutts the paper object along cutt verts ('tearLine').
	/// </summary>
	private void CuttPaperObjectAlongTearVerts()
	{
		//Debug.Log("Just entered cutt paper along tear line (Verts)");
		CuttPieceOneVerts = Clone.GetComponent<MeshFilter>().mesh.vertices;
		CuttPieceTwoVerts = Clone.GetComponent<MeshFilter>().mesh.vertices;
		CuttPieceOneFaces = Clone.GetComponent<MeshFilter>().mesh.triangles;
		CuttPieceTwoFaces = Clone.GetComponent<MeshFilter>().mesh.triangles;
		
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
		
		//Now we know which verticies are selected and how to distinguish both islands thanks to the organized paper vert grid
		// found previously
		
		//Now, we create two lists of verticies to determine which vertices belong to which 'island'
		island1 = new List<Vector2>();
		island2 = new List<Vector2>();
		
		//The following is used to switch in between both new meshes for data transfer after cutt/tear
		addingToPieceOne = true;
		bool currentlyInTransition = false;
		
		//int testNum = 0;
		int numEdgeTearVerts = 0;
		
		if(!TestingMouseTime)
		{
			//We looop through the sorted paperGrid to create two new mesh based of tearLine
			foreach(List<Vector3> tempList in paperGrid.Values)//(int index = 0; index < paperGrid.Values.Count; index++)//
			{
				//Debug.LogWarning("PaperGrid Ordering::::::::>>>>>>>>>>>" + tempList[0].y.ToString() + "   " + tempList[1].y.ToString() + "   " + tempList[2].y.ToString());
				//List<Vector3> tempList = paperGrid.Values[index];
				if (tearLine.ContainsKey(tempList[0]) && !tearLine.ContainsKey(new Vector3(tempList[0].x, tempList[0].y - MESH_VERT_OFFSET, tempList[0].z)))// || 
					//tearLine.ContainsKey(tempList[tempList.Count - 1])) 
				{
					
					numEdgeTearVerts++;
					//if (numEdgeTearVerts % 2 == 0) addingToPieceOne = true;
					//else addingToPieceOne = false;
				}
				
				if (numEdgeTearVerts % 2 == 0) 
				{
					addingToPieceOne = true;
				}
				else
				{
					addingToPieceOne = false;
				}
				
				
				for(int jtor = 0; jtor < tempList.Count; jtor++)
				{
					//In the following, the x==y and y==x already
					TraverseGridAddToNewPiece(new Vector2(tempList[jtor].y, tempList[jtor].x), currentlyInTransition, jtor, tempList, true);
					
				}
				//TESTINGnumOfTornVerts += tempList.Count;
			}
		}
		else if(TestingMouseTime)
		{
			int startingVertIndice = 0;
			
			
			//We looop through the sorted paperGrid to create two new mesh based of tearLine
			for(int iterator = 0; iterator < paperGrid.Values.Count; iterator++)//each(List<Vector3> tempList in paperGrid.Values)
			{
				
				//If the fist node in list is in tearLine verts, increase number of tear verts long edge count
				//AND if the tear line does NOT include the vertice directly below
				
				/*
				addingToPieceOne = true;
				
				if (!tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator)[0]))
					//!tearLine.ContainsKey(new Vector3(paperGrid.Values.ElementAt(iterator)[0].x, 
					//paperGrid.Values.ElementAt(iterator)[0].y - MESH_VERT_OFFSET, paperGrid.Values.ElementAt(iterator)[0].z)))
				{
					if(iterator - 1 >= 0 && tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator - 1)[0]))//TODO
					{
						//The following bool check is needed because of the way in with user input is being tranformered into tear vertices -> J.C.
						//if(iterator + 1 >= paperGrid.Values.Count - 1 && !tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator + 1)[0]))//TODO
						{
							//numEdgeTearVerts++;

						}
					}
				}
				
				if(numEdgeTearVerts % 2 == 0)// && addingToPieceOne) // (!tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator)[0]))//
				{
					Debug.LogWarning("NUMBER OF EDGE TEAR VERTICES HAS CHANGED, true now!!!! itor = " + iterator.ToString());
					addingToPieceOne = true;
				}
				else  if(numEdgeTearVerts % 2 == 1)// && !addingToPieceOne)// if (iterator - 1 >= 0 && tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator - 1)[0]))
				{
					Debug.LogWarning("NUMBER OF EDGE TEAR VERTICES HAS CHANGED, false now!!!! itor = " + iterator.ToString());
					addingToPieceOne = false;
				}
				*/
				
				
				
				/*
				else
				{
					if(numEdgeTearVerts % 2 == 0)// && addingToPieceOne) // (!tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator)[0]))//
					{
						Debug.LogWarning("NUMBER OF EDGE TEAR VERTICES HAS CHANGED, true now!!!! itor = " + iterator.ToString());
						addingToPieceOne = true;
					}
					else  if(numEdgeTearVerts % 2 == 1)// && !addingToPieceOne)// if (iterator - 1 >= 0 && tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator - 1)[0]))
					{
						Debug.LogWarning("NUMBER OF EDGE TEAR VERTICES HAS CHANGED, false now!!!! itor = " + iterator.ToString());
						addingToPieceOne = false;
					}
				}*/
				
				
				
				/*
				if (iterator >= 1) 
				{
					if(tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator - 1)[0]))
						//!tearLine.ContainsKey(new Vector3(paperGrid.Values.ElementAt(iterator)[0].x, 
						//paperGrid.Values.ElementAt(iterator)[0].y - MESH_VERT_OFFSET, paperGrid.Values.ElementAt(iterator)[0].z)))
					{
						if(!tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator)[0]))//TODO
						{
							numEdgeTearVerts++;
						}
					}
				}
				*/
				
				/*
				Debug.LogWarning("NUMBER OF EDGE TEAR VERTICES = " + numEdgeTearVerts.ToString());
				
				if(numEdgeTearVerts % 2 == 0 && !addingToPieceOne) // (!tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator)[0]))//
				{
					Debug.LogWarning("NUMBER OF EDGE TEAR VERTICES HAS CHANGED, true now!!!! itor = " + iterator.ToString());
					addingToPieceOne = true;
				}
				else if(numEdgeTearVerts % 2 == 1 && addingToPieceOne)// if (iterator - 1 >= 0 && tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator - 1)[0]))
				{
					Debug.LogWarning("NUMBER OF EDGE TEAR VERTICES HAS CHANGED, false now!!!! itor = " + iterator.ToString());
					addingToPieceOne = false;
				}
				*/
				
				
				/*
				
				//if(tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator)[0]))
				{
				//	addingToPieceOne = false;
				}
				//else
				{
				//	addingToPieceOne = true;
				}
				*/
				bool haveHitTearLine = false;
				Vector3 rowTearStartPos = Vector3.zero;
				Vector3 endPosTearStartPos = Vector3.zero;
				bool readyToDeterminShape = false;
				
				bool addingToislandOneROWCheck = addingToPieceOne;
				for(int jtor = 0; jtor < paperGrid.Values.ElementAt(iterator).Count; jtor++)
				{
					if(jtor == 0)
					{
						addingToPieceOne = true;
						if (!tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator)[jtor]))
						{
							if(iterator - 1 >= 0 && tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator - 1)[jtor]))//TODO
							{
								numEdgeTearVerts++;
							}
						}
						if(numEdgeTearVerts % 2 == 0)// && addingToPieceOne)
						{
							Debug.LogWarning("NUMBER OF EDGE TEAR VERTICES HAS CHANGED, true now!!!! itor = " + iterator.ToString());
							addingToPieceOne = true;
						}
						else if(numEdgeTearVerts % 2 == 1)// && !addingToPieceOne)
						{
							Debug.LogWarning("NUMBER OF EDGE TEAR VERTICES HAS CHANGED, false now!!!! itor = " + iterator.ToString());
							addingToPieceOne = false;
						}
					}
					
					
					if(tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator)[jtor]) && !haveHitTearLine)
					{
						haveHitTearLine = true;
						rowTearStartPos = paperGrid.Values.ElementAt(iterator)[jtor];
						startingVertIndice = jtor;
						readyToDeterminShape = false;
						Debug.LogWarning("Current vert = " + paperGrid.Values.ElementAt(iterator)[jtor].ToString() + " has hit tearvert in row = " + iterator.ToString());
					}
					
					if(haveHitTearLine)
					{
						
						if((iterator + 1) <= (paperGrid.Values.Count - 1) && !tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator + 1)[jtor]))
						{
							haveHitTearLine = false;
							readyToDeterminShape = true;
							//endPosTearStartPos = paperGrid.Values.ElementAt(iterator + 1)[jtor];
						}
						//if(!tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator)[jtor]))
						//{
						//	haveHitTearLine = false;
						//	readyToDeterminShape = true;
						//}
						//else
						//{
						//	endPosTearStartPos = paperGrid.Values.ElementAt(iterator)[jtor];
						//}
						
						endPosTearStartPos = paperGrid.Values.ElementAt(iterator)[jtor];
					}
					
					//In the following, the x==y and y==x already
					TraverseGridAddToNewPiece2(paperGrid.Values.ElementAt(iterator)[jtor], new Vector2(paperGrid.Values.ElementAt(iterator)[jtor].y, paperGrid.Values.ElementAt(iterator)[jtor].x), currentlyInTransition, 
						jtor, paperGrid.Values.ElementAt(iterator), rowTearStartPos, endPosTearStartPos, readyToDeterminShape, startingVertIndice);
				}
				
				/*
				if (tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator)[0]))
					//!tearLine.ContainsKey(new Vector3(paperGrid.Values.ElementAt(iterator)[0].x, 
					//paperGrid.Values.ElementAt(iterator)[0].y - MESH_VERT_OFFSET, paperGrid.Values.ElementAt(iterator)[0].z)))
				{
					if(!tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator + 1)[0]))//TODO
					{
						numEdgeTearVerts++;
					}
				}*/
			}
		}
		
		island1_Vertical = new List<Vector2>();
		island2_Vertical = new List<Vector2>();
		
		if(!SimpleCutt && !TestingMouseTime)
		{
			//reset flags
			numEdgeTearVerts = 0;
			currentlyInTransition = false;
			
			
			if(island1.Contains(new Vector2(paperGrid[paperGrid.Keys.ElementAt(0)][paperGrid[paperGrid.Keys.ElementAt(0)].Count - 1].y, 
				paperGrid[paperGrid.Keys.ElementAt(0)][paperGrid[paperGrid.Keys.ElementAt(0)].Count - 1].x)))
			
			{
				addingToPieceOne = true;
			}
			else
			{
				addingToPieceOne = false;
			}
			
			//We looop through the sorted paperGrid to create two new mesh based of tearLine
			foreach(List<Vector3> tempList in paperGrid.Values)//(int index = 0; index < paperGrid.Values.Count; index++)//
			{
				if (tearLine.ContainsKey(tempList[tempList.Count - 1]) && !tearLine.ContainsKey(
					new Vector3(tempList[tempList.Count - 1].x, tempList[tempList.Count - 1].y - MESH_VERT_OFFSET, tempList[tempList.Count - 1].z)))
				{
					numEdgeTearVerts++;
				}
				if (numEdgeTearVerts % 2 == 0) addingToPieceOne = true;
				else addingToPieceOne = false;
				
				
				for(int jtor = tempList.Count - 1; jtor >= 0; jtor--)
				{
					//In the following, the x==y and y==x already
					TraverseGridAddToNewPiece(new Vector2(tempList[jtor].y, tempList[jtor].x), currentlyInTransition, jtor, tempList, false);
					
				}
			}
			
			island1_2 = new List<Vector2>();
			island2_2 = new List<Vector2>();
			
			island1_2_2 = new List<Vector2>();
			island2_2_2 = new List<Vector2>();
			
			//reset flags
			numEdgeTearVerts = 0;
			currentlyInTransition = false;
			if(island1.Contains(new Vector2(paperGrid_Vertical[paperGrid_Vertical.Keys.ElementAt(0)][0].y, paperGrid_Vertical[paperGrid_Vertical.Keys.ElementAt(0)][0].x)))
			{
				addingToPieceOne = true;
			}
			else
			{
				addingToPieceOne = false;
			}
			//We looop through the sorted paperGrid to create two new mesh based of tearLine
			foreach(List<Vector3> tempList in paperGrid_Vertical.Values)
			{
				if (tearLine.ContainsKey(tempList[0]) && !tearLine.ContainsKey(new Vector3(tempList[0].x - MESH_VERT_OFFSET, tempList[0].y, tempList[0].z)))
				{
					numEdgeTearVerts++;
				}
				if (numEdgeTearVerts % 2 == 0) addingToPieceOne = true;
				else addingToPieceOne = false;
				
				for(int jtor = 0; jtor < tempList.Count; jtor++)
				{
					//In the following, the x==y and y==x already
					TraverseGridAddToNewPieceVertically(new Vector2(tempList[jtor].y, tempList[jtor].x), currentlyInTransition, jtor, tempList, true);
				}
			}
			
			
			//reset flags
			numEdgeTearVerts = 0;
			currentlyInTransition = false;
			if(island1_2.Contains(new Vector2(paperGrid_Vertical[paperGrid_Vertical.Keys.ElementAt(paperGrid_Vertical.Keys.Count - 1)][paperGrid_Vertical[paperGrid_Vertical.Keys.ElementAt(paperGrid_Vertical.Keys.Count - 1)].Count - 1].y,
				paperGrid_Vertical[paperGrid_Vertical.Keys.ElementAt(paperGrid_Vertical.Keys.Count - 1)][paperGrid_Vertical[paperGrid_Vertical.Keys.ElementAt(paperGrid_Vertical.Keys.Count - 1)].Count - 1].x)))
			{
				addingToPieceOne = true;
			}
			else
			{
				addingToPieceOne = false;
			}
			//We looop through the sorted paperGrid to create two new mesh based of tearLine
			foreach(List<Vector3> tempList in paperGrid_Vertical.Values)
			{
				if (tearLine.ContainsKey(tempList[tempList.Count - 1]) && !tearLine.ContainsKey(new Vector3(tempList[tempList.Count - 1].x - MESH_VERT_OFFSET,
					tempList[tempList.Count - 1].y, tempList[tempList.Count - 1].z)))
				{
					numEdgeTearVerts++;
				}
				if (numEdgeTearVerts % 2 == 0) addingToPieceOne = true;
				else addingToPieceOne = false;
				
				for(int jtor = tempList.Count - 1; jtor >= 0; jtor--)
				{
					//In the following, the x==y and y==x already
					TraverseGridAddToNewPieceVertically(new Vector2(tempList[jtor].y, tempList[jtor].x), currentlyInTransition, jtor, tempList, false);
				}
			}
		}
		else if(!TestingMouseTime)
		{
		
		
			//reset flags
			numEdgeTearVerts = 0;
			currentlyInTransition = false;
		
		
			if(island1.Contains(new Vector2(paperGrid_Vertical[paperGrid_Vertical.Keys.ElementAt(0)][0].y, paperGrid_Vertical[paperGrid_Vertical.Keys.ElementAt(0)][0].x)))
			{
				addingToPieceOne = true;
			}
			else
			{
				addingToPieceOne = false;
			}
		
		
			//We looop through the sorted paperGrid to create two new mesh based of tearLine
			foreach(List<Vector3> tempList in paperGrid_Vertical.Values)//(int index = 0; index < paperGrid.Values.Count; index++)//
			{
				
				//List<Vector3> tempList = paperGrid.Values[index];
				if (tearLine.ContainsKey(tempList[0]) && !tearLine.ContainsKey(new Vector3(tempList[0].x - MESH_VERT_OFFSET, tempList[0].y, tempList[0].z)))// || 
				//tearLine.ContainsKey(tempList[tempList.Count - 1])) 
				{
					
					numEdgeTearVerts++;
					//if (numEdgeTearVerts % 2 == 0) addingToPieceOne = true;
					//else addingToPieceOne = false;
				}
				if (numEdgeTearVerts % 2 == 0) addingToPieceOne = true;
				else addingToPieceOne = false;
				
				
				for(int jtor = 0; jtor < tempList.Count; jtor++)
				{
					//In the following, the x==y and y==x already
					TraverseGridAddToNewPiece(new Vector2(tempList[jtor].y, tempList[jtor].x), currentlyInTransition, jtor, tempList, false);
					
				}
				//TESTINGnumOfTornVerts += tempList.Count;
			}
		
		}
		
		//Debug.Log("Starting Position papergrid = " + paperGrid[0].ElementAt(0).ToString());
		//Debug.Log("Starting Position papergrid_Vertical = " + paperGrid_Vertical[0].ElementAt(0).ToString());
		
		//Debug.Log("Number of TORN VERTS 2 = " + TESTINGnumOfTornVerts.ToString());
		//Debug.Log("Number of TORN **EDGE** VERTS 2 = " + numEdgeTearVerts.ToString());
		//Debug.Log("Number of vertices ADDED TO ISLANDS = " + testNum.ToString());
		
		Clone_2.GetComponent<MeshRenderer>().enabled = true;
		
		island1Indicies = new int[CuttPieceOneFaces.Length];
		island2Indicies = new int[CuttPieceTwoFaces.Length];
		
		indexor1 = 0;
		indexor2 = 0;
		
		//Now we have the associates vertices stored within island1 and island 2
		//We can now determine the new faces for each mesh by FIRST finding the indices for each vertice
		for(int itor = 0; itor < CuttPieceOneFaces.Length; itor += 3)
		{
			//Debug.Log("CHECKING WHAT TYPE OF CUTT");
			if(SimpleCutt)
			{
				TestFaceChecking(itor);
			}
			else
			{
				if(TestingMouseTime)
				{
					CreatNewCuttPaperMeshPieces(itor);
				}
				else
				{
					CompareIslandSweeps(itor);
				}
			}
			
		}
		//Debug.Log("Number of polyGons added to new Island mesh #2 = " +  indexor2.ToString());
		
		
		Clone.GetComponent<MeshFilter>().mesh.triangles = island1Indicies;
		Clone.GetComponent<MeshCollider>().sharedMesh = Clone.GetComponent<MeshFilter>().mesh;	
		
		Clone_2.GetComponent<MeshFilter>().mesh.triangles = island2Indicies;	
		Clone_2.GetComponent<MeshCollider>().sharedMesh = Clone_2.GetComponent<MeshFilter>().mesh;
		
		
		//int dist1 = island2Indicies.Length;
		//int dist2 = island1Indicies.Length;
		
		if(Clone_2.GetComponent<MeshFilter>().mesh.triangles.Length < Clone.GetComponent<MeshFilter>().mesh.triangles.Length)
		{
			Clone_2.name = "paper_CuttPieceOfPaper";
			Clone.name = "paper_LargerPiece";
		}
		else
		{
			Clone.name = "paper_CuttPieceOfPaper";
			Clone_2.name = "paper_LargerPiece";
		}
		
		doneCalculatingCuttLine = false;
	}
	
	public bool SimpleCutt;
	
	/// <summary>
	/// Compares the islands found in multiple passed through grid to determine actual cuttLine and new two islands.
	/// </summary>
	/// <param name='itor'>
	/// Itor - index into CuttPieceOneFaces
	/// </param>
	private void CompareIslandSweeps(int index)
	{
		if( island2.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index]].y, CuttPieceOneVerts[CuttPieceOneFaces[index]].x)) && 
			island2.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].x)) && 
			island2.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].x)) &&
			island2_Vertical.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index]].y, CuttPieceOneVerts[CuttPieceOneFaces[index]].x)) && 
			island2_Vertical.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].x)) && 
			island2_Vertical.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].x)) &&
			
			island1_2.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index]].y, CuttPieceOneVerts[CuttPieceOneFaces[index]].x)) && 
			island1_2.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].x)) && 
			island1_2.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].x)) &&
			island1_2_2.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index]].y, CuttPieceOneVerts[CuttPieceOneFaces[index]].x)) && 
			island1_2_2.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].x)) && 
			island1_2_2.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].x))
			)
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
	
	private bool horizontalCutt = false;
	private bool verticalCutt = false;
	private bool topToTopCutt = false;
	private bool rightToRightCutt = false;
	private bool bottomToBottomCutt = false;
	private bool leftToLeftCutt = false;
	private bool topRightCutt = false;
	private bool topLeftCutt = false;
	private bool botRightCutt = false;
	private bool botLeftCutt = false;
	private Vector3 startPos;
	private Vector3 endPos;
		
	/// <summary>
	/// Tests the face checking.
	/// </summary>
	/// <param name='index'>
	/// Index.
	/// </param>
	private void TestFaceChecking(int index)
	{

		if ((startPos.x == WIDTH_MAX && endPos.x == WIDTH_MIN) || (startPos.x == WIDTH_MIN && endPos.x == WIDTH_MAX) || horizontalCutt)
		{
			//Debug.Log("HORIZONTAL CUTT TRIGGERED");
			//Horizontal-like cut
			HorizontalCutt(index);
			horizontalCutt = true;
		}
		else if ((startPos.y == HEIGHT_MAX && endPos.y == HEIGHT_MIN) || (startPos.y == HEIGHT_MIN && endPos.y == HEIGHT_MAX) || verticalCutt)
		{ 
			//Debug.Log("VERTICAL CUTT TRIGGERED");
			//Vertical-like cut
			VerticalCutt(index);
			verticalCutt = true;
		}
		else if ((startPos.x == WIDTH_MIN && endPos.y == HEIGHT_MAX) || (startPos.y == HEIGHT_MAX && endPos.x == WIDTH_MIN) || topRightCutt)
		{
			//Debug.Log("TOP RIGHT CUTT TRIGGERED");
			//Top Right Corner-like Cutt
			TopRightCutt(index);
			topRightCutt = true;
		}
		else if ((startPos.x == WIDTH_MAX && endPos.y == HEIGHT_MAX) || (startPos.y == HEIGHT_MAX && endPos.x == WIDTH_MAX) || topLeftCutt)
		{
			
			//Debug.Log("TOP LEFT CUTT TRIGGERED");
			//Top Left Corner-like Cutt
			TopLeftCutt(index);
			topLeftCutt = true;
		}
		else if ((startPos.x == WIDTH_MAX && endPos.y == HEIGHT_MIN) || (startPos.y == HEIGHT_MIN && endPos.x == WIDTH_MAX) || botLeftCutt)
		{
			//Debug.Log("BOTTOM LEFT CUTT TRIGGERED");
			//Bottom Left Corner-like Cutt
			BottomLeftCutt(index);
			botLeftCutt = true;
		}
		else if ((startPos.x == WIDTH_MIN && endPos.y == HEIGHT_MIN) || (startPos.y == HEIGHT_MIN && endPos.x == WIDTH_MIN) || botRightCutt)
		{
			//Debug.Log("BOTTOM RIGHT CUTT TRIGGERED");
			//Bottom Right Corner-like Cutt
			BottomRightCutt(index);
			botRightCutt = true;
		}
		else if ((startPos.y == endPos.y && endPos.y <= HEIGHT_MIN + 0.250f && endPos.y >= HEIGHT_MIN - 0.250f) || bottomToBottomCutt)
		{
			///Debug.Log("BOTTOM - to - BOTTOM CUTT TRIGGERED");
			//Bottom Right Corner-like Cutt
			BottomToBottomCutt(index);
			bottomToBottomCutt = true;
		}
		else if ((startPos.y == endPos.y && endPos.y == HEIGHT_MAX) || topToTopCutt)
		{
			//Debug.Log("TOP - to - TOP CUTT TRIGGERED");
			//Bottom Right Corner-like Cutt
			TopToTopCutt(index);
			topToTopCutt = true;
		}
		else if ((startPos.x == endPos.x && endPos.x == WIDTH_MAX) || leftToLeftCutt)
		{
			//Debug.Log("LEFT - to - LEFT CUTT TRIGGERED");
			//Bottom Right Corner-like Cutt
			LeftToLeftCutt(index);
			leftToLeftCutt = true;
		}
		else if ((startPos.x == endPos.x && endPos.x == WIDTH_MIN) || rightToRightCutt)
		{
			//Debug.Log("RIGHT - to - RIGHT CUTT TRIGGERED");
			//Bottom Right Corner-like Cutt
			RightToRightCutt(index);
			rightToRightCutt = true;
		}
		else
		{
			Debug.LogError("I DONT KNOW WHAT TYPE OF TEAR I AM :-(");
		}
	}
	
	private void HorizontalCutt(int index)
	{
		if( island2.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index]].y, CuttPieceOneVerts[CuttPieceOneFaces[index]].x)) && 
			island2.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].x)) && 
			island2.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].x)) &&
			island1_Vertical.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index]].y, CuttPieceOneVerts[CuttPieceOneFaces[index]].x)) && 
			island1_Vertical.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].x)) && 
			island1_Vertical.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].x)))
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
	
	private void VerticalCutt(int index)
	{
		if( island1.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index]].y, CuttPieceOneVerts[CuttPieceOneFaces[index]].x)) && 
			island1.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].x)) && 
			island1.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].x)))
			
			//island1_Vertical.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index]].y, CuttPieceOneVerts[CuttPieceOneFaces[index]].x)) && 
			//island1_Vertical.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].x)) && 
			//island1_Vertical.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].x)))
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
	
	private void TopLeftCutt(int index)
	{
		if( island1.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index]].y, CuttPieceOneVerts[CuttPieceOneFaces[index]].x)) && 
			island1.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].x)) && 
			island1.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].x)))// &&
			//island1_Vertical.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index]].y, CuttPieceOneVerts[CuttPieceOneFaces[index]].x)) && 
			//island1_Vertical.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].x)) && 
			//island1_Vertical.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].x)))
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
	private void TopRightCutt(int index)
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
	
	private void BottomRightCutt(int index)
	{
		if( island2_Vertical.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index]].y, CuttPieceOneVerts[CuttPieceOneFaces[index]].x)) && 
			island2_Vertical.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].x)) && 
			island2_Vertical.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].x)))
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
	
	private void BottomLeftCutt(int index)
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
	
	private void BottomToBottomCutt(int index)
	{
		if( island2.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index]].y, CuttPieceOneVerts[CuttPieceOneFaces[index]].x)) && 
			island2.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].x)) && 
			island2.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].x)) &&
			island2_Vertical.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index]].y, CuttPieceOneVerts[CuttPieceOneFaces[index]].x)) && 
			island2_Vertical.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].x)) && 
			island2_Vertical.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].x)))
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
	private void TopToTopCutt(int index)
	{
		if( island2.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index]].y, CuttPieceOneVerts[CuttPieceOneFaces[index]].x)) && 
			island2.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].x)) && 
			island2.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].x)) &&
			island1_Vertical.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index]].y, CuttPieceOneVerts[CuttPieceOneFaces[index]].x)) && 
			island1_Vertical.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].x)) && 
			island1_Vertical.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].x)))// &&
			//!island1.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index]].y, CuttPieceOneVerts[CuttPieceOneFaces[index]].x)) && 
			//!island1.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].x)) && 
			//!island1.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].x)) &&
			//!island2_Vertical.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index]].y, CuttPieceOneVerts[CuttPieceOneFaces[index]].x)) && 
			//!island2_Vertical.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].x)) && 
			//!island2_Vertical.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].x)))
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
	private void RightToRightCutt(int index)
	{
		if( island2.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index]].y, CuttPieceOneVerts[CuttPieceOneFaces[index]].x)) && 
			island2.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].x)) && 
			island2.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].x)) &&
			island2_Vertical.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index]].y, CuttPieceOneVerts[CuttPieceOneFaces[index]].x)) && 
			island2_Vertical.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 1]].x)) && 
			island2_Vertical.Contains(new Vector2(CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].y, CuttPieceOneVerts[CuttPieceOneFaces[index + 2]].x)))
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
	private void LeftToLeftCutt(int index)
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
	
	/// <summary>
	/// This function turns tearLine into a line created by vertices, instead of mulitple creating faces
	/// </summary>
	private void UpdateTearInputLine()
	{
		//TestDist3FastInput();
		
		TurnInputIntoTornVertCurve();
		
		//ConnectEdgeTearVerts();
		
		return;
		
		/*
		//TODO
		for(int jtor = 0; jtor < Clone.GetComponent<MeshFilter>().mesh.triangles.Length; jtor += 3)
		{
			if(tearLine.ContainsKey(Clone.GetComponent<MeshFilter>().mesh.vertices[Clone.GetComponent<MeshFilter>().mesh.triangles[jtor]]) &&
				tearLine.ContainsKey(Clone.GetComponent<MeshFilter>().mesh.vertices[Clone.GetComponent<MeshFilter>().mesh.triangles[jtor + 1]]) &&
				tearLine.ContainsKey(Clone.GetComponent<MeshFilter>().mesh.vertices[Clone.GetComponent<MeshFilter>().mesh.triangles[jtor + 2]]))
			{
				int rand = UnityEngine.Random.Range(0,2);
				
				if(rand == 0)
				{
					tearLine.Remove(Clone.GetComponent<MeshFilter>().mesh.vertices[Clone.GetComponent<MeshFilter>().mesh.triangles[jtor]]);
				}
				else if(rand == 1)
				{
					tearLine.Remove(Clone.GetComponent<MeshFilter>().mesh.vertices[Clone.GetComponent<MeshFilter>().mesh.triangles[jtor + 1]]);
				}
				else if(rand == 2)
				{
					tearLine.Remove(Clone.GetComponent<MeshFilter>().mesh.vertices[Clone.GetComponent<MeshFilter>().mesh.triangles[jtor + 2]]);
				}
			}
		}*/
		
	}
	
	/// <summary>
	/// Connects the edge tear verts so that there are no gaps in tear verts, colid tear reagion along edge
	/// </summary>
	private void ConnectEdgeTearVerts()
	{
		//We check each edge tfor tear region completion
		for(int iterator = 0; iterator < paperGrid.Values.Count; iterator++)
		{
			if(iterator + 2 < paperGrid.Values.Count)
			{
				if (tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator)[0]) &&
					!tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator + 1)[0]) &&
					tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator + 2)[0]))
				{
					++tearLineTimer;
					int index = 0;
					for(int itor = 0; itor < newMesh.Length; itor++)
					{
						if(paperGrid.Values.ElementAt(iterator + 1)[0] == Clone.GetComponent<MeshFilter>().mesh.vertices[itor])
						{
							index = itor;
						}
					}
					tearLine.Add(paperGrid.Values.ElementAt(iterator + 1)[0], index);
					paperGrid_1[paperGrid.Values.ElementAt(iterator + 1)[0]] = true;
						
					//This maps a 'time' to the tear vertice
					tearLineTime.Add (paperGrid.Values.ElementAt(iterator + 1)[0], (float)tearLineTimer);
					tearLinePositionTime.Add ((float)tearLineTimer, paperGrid.Values.ElementAt(iterator + 1)[0]);
				}
			}
			
			if(iterator + 2 < paperGrid.Values.Count)
			{
				if (tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator)[paperGrid.Values.ElementAt(iterator).Count - 1]) &&
					!tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator + 1)[paperGrid.Values.ElementAt(iterator).Count - 1]) &&
					tearLine.ContainsKey(paperGrid.Values.ElementAt(iterator + 2)[paperGrid.Values.ElementAt(iterator).Count - 1]))
				{
					++tearLineTimer;
					int index = 0;
					for(int itor = 0; itor < newMesh.Length; itor++)
					{
						if(paperGrid.Values.ElementAt(iterator + 1)[paperGrid.Values.ElementAt(iterator).Count - 1] == Clone.GetComponent<MeshFilter>().mesh.vertices[itor])
						{
							index = itor;
						}
					}
					tearLine.Add(paperGrid.Values.ElementAt(iterator + 1)[paperGrid.Values.ElementAt(iterator).Count - 1], index);
					paperGrid_1[paperGrid.Values.ElementAt(iterator + 1)[paperGrid.Values.ElementAt(iterator).Count - 1]] = true;
						
					//This maps a 'time' to the tear vertice
					tearLineTime.Add (paperGrid.Values.ElementAt(iterator + 1)[paperGrid.Values.ElementAt(iterator).Count - 1], (float)tearLineTimer);
					tearLinePositionTime.Add ((float)tearLineTimer, paperGrid.Values.ElementAt(iterator + 1)[paperGrid.Values.ElementAt(iterator).Count - 1]);
				}
			}
			
			if(iterator + 2 < paperGrid.Values.Count)
			{
				if (tearLine.ContainsKey(paperGrid.Values.ElementAt(0)[iterator]) &&
					!tearLine.ContainsKey(paperGrid.Values.ElementAt(0)[iterator + 1]) &&
					tearLine.ContainsKey(paperGrid.Values.ElementAt(0)[iterator + 2]))
				{
					++tearLineTimer;
					int index = 0;
					for(int itor = 0; itor < newMesh.Length; itor++)
					{
						if(paperGrid.Values.ElementAt(0)[iterator + 1] == Clone.GetComponent<MeshFilter>().mesh.vertices[itor])
						{
							index = itor;
						}
					}
					tearLine.Add(paperGrid.Values.ElementAt(0)[iterator + 1], index);
					paperGrid_1[paperGrid.Values.ElementAt(0)[iterator + 1]] = true;
						
					//This maps a 'time' to the tear vertice
					tearLineTime.Add (paperGrid.Values.ElementAt(0)[iterator + 1], (float)tearLineTimer);
					tearLinePositionTime.Add ((float)tearLineTimer, paperGrid.Values.ElementAt(0)[iterator + 1]);
				}
			}
			
			if(iterator + 2 < paperGrid.Values.Count)
			{
				if (tearLine.ContainsKey(paperGrid.Values.ElementAt(paperGrid.Values.Count - 1)[iterator]) &&
					!tearLine.ContainsKey(paperGrid.Values.ElementAt(paperGrid.Values.Count - 1)[iterator + 1]) &&
					tearLine.ContainsKey(paperGrid.Values.ElementAt(paperGrid.Values.Count - 1)[iterator + 2]))
				{
					++tearLineTimer;
					int index = 0;
					for(int itor = 0; itor < newMesh.Length; itor++)
					{
						if(paperGrid.Values.ElementAt(paperGrid.Values.Count - 1)[iterator + 1] == Clone.GetComponent<MeshFilter>().mesh.vertices[itor])
						{
							index = itor;
						}
					}
					tearLine.Add(paperGrid.Values.ElementAt(paperGrid.Values.Count - 1)[iterator + 1], index);
					paperGrid_1[paperGrid.Values.ElementAt(paperGrid.Values.Count - 1)[iterator + 1]] = true;
						
					//This maps a 'time' to the tear vertice
					tearLineTime.Add (paperGrid.Values.ElementAt(paperGrid.Values.Count - 1)[iterator + 1], (float)tearLineTimer);
					tearLinePositionTime.Add ((float)tearLineTimer, paperGrid.Values.ElementAt(paperGrid.Values.Count - 1)[iterator + 1]);
				}
			}
		}
				
		

	}
	
	/// <summary>
	/// This function takes in the list of mouse input to create the tear line
	/// </summary>
	private void TestDist3FastInput()
	{
		Vector3 previousTornVert = Clone.GetComponent<MeshFilter>().mesh.vertices[0];
		int numberOfVerts = 0;
		foreach(Vector3 mousePos in mouseTearPositions)
		{
			//Loop through mesh vertices
			for (int itor = 0; itor < newMesh.Length; itor++)
			{
				//Find the screen poition of the vertex
				screenPoint = Camera.main.WorldToScreenPoint(Clone.GetComponent<MeshFilter>().mesh.vertices[itor]);
				
				//Get the current position of the mouse in the scene (world) with the same z-depth as the vertex being examined
	    		Vector3 curPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, screenPoint.z));
				
				//Now find the distance between the world mouse position and the vertex world position for distance comparison
				float dist = Vector3.Distance(curPosition, Clone.GetComponent<MeshFilter>().mesh.vertices[itor]);
				
				float distTwo = Vector3.Distance(curPosition, previousTornVert);
				
				if(dist < 0) dist *= -1;
				
				//Muliply the distance to widen the difference between the mouse and all mesh vertices
				dist *= 10000;
				
				//Debug.LogWarning("Checking Mouse Down + " + itor.ToString() + "  ---- Dist = " + dist.ToString());
				 
				//Check is the distance is less than 1500 (arbitrarily found through trial and error)
				if( dist < 1500 && !tearLine.ContainsKey(Clone.GetComponent<MeshFilter>().mesh.vertices[itor]))// && NeighborVert(Clone.GetComponent<MeshFilter>().mesh.vertices[itor]))
				{	
					tearLineTimer++;
					numberOfVerts++;
					//Debug.LogWarning("Adding a vert with itor = " + itor.ToString());
					//Debug.Log("New Tear vert = " + Clone.GetComponent<MeshFilter>().mesh.vertices[itor].ToString());
					tearLine.Add(Clone.GetComponent<MeshFilter>().mesh.vertices[itor], itor);
					paperGrid_1[Clone.GetComponent<MeshFilter>().mesh.vertices[itor]] = true;
						
					//This maps a 'time' to the tear vertice
					tearLineTime.Add (Clone.GetComponent<MeshFilter>().mesh.vertices[itor], (float)tearLineTimer);
					tearLinePositionTime.Add ((float)tearLineTimer, Clone.GetComponent<MeshFilter>().mesh.vertices[itor]);
					previousTornVert = Clone.GetComponent<MeshFilter>().mesh.vertices[itor];
				}
					
			}
		}
		//Debug.Log("NUMBER OF TORN VERTS = " + numberOfVerts.ToString());
	}
	
	/// <summary>
	/// Turns the input into torn vert curve.
	/// </summary>
	private void TurnInputIntoTornVertCurve()
	{
		Vector3 screenDepth = Camera.main.WorldToScreenPoint(Clone.GetComponent<MeshFilter>().mesh.vertices[0]);
		List<Vector3> newMouseTearWorldPos = new List<Vector3>();
		//First the mouseTearPositions needs to be translated into world positions
		for(int indexr = 0; indexr < mouseTearPositions.Count - 1; indexr++)
		{
			Vector3 curPosition = Camera.main.ScreenToWorldPoint(
				new Vector3(mouseTearPositions[indexr].x, mouseTearPositions[indexr].y, screenDepth.z));
			newMouseTearWorldPos.Add (curPosition);
		}
		mouseTearPositions = newMouseTearWorldPos;
		
		Vector3 previousTornVert = Clone.GetComponent<MeshFilter>().mesh.vertices[0];
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
			if(jtor == 0 || mouseMoveDistX >= MESH_VERT_OFFSET || mouseMoveDistY >= MESH_VERT_OFFSET)
			{
				Debug.LogWarning("Mouse Has Moved the offset amount to add new vert");
				//Now, we know here we need to find the vert closest to the mouse position to add to the
				// tearLine
				
				float distToClosestRow = MESH_VERT_OFFSET * 20;
				int rowNum = 0;
				
				//First, loop through grid to find row with closest y-component
				for(int itor = 0; itor < paperGrid.Keys.Count - 1; itor++)
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
				//Now, we loop through grid to find vert in row with closest x-component
				for(int ktor = 0; ktor < paperGrid[paperGrid.Keys.ElementAt(rowNum)].Count - 1; ktor++)
				{
					float tempDist2 =  paperGrid[paperGrid.Keys.ElementAt(rowNum)][ktor].y - mouseTearPositions.ElementAt(jtor).y;
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
		
		
		Debug.LogWarning("Number of verts torn = " + numberOfVerts.ToString());
	}
	
	private int TESTINGnumOfTornVerts = 0;
	
	/// <summary>
	/// Traverses the grid to add new information to splitt paper object mesh
	/// </summary>
	/// <returns>
	/// The true if we are still adding to the first cutt piece, false if the second
	/// </returns>
	/// <param name='pieceOne'>
	/// If set to <c>true</c> piece one if we are currently adding to the first object
	/// </param>
	private void TraverseGridAddToNewPiece(Vector2 pos, bool currentlyInTransition, int listIndice, List<Vector3> tempList, bool checkingRow)
	{
		//Debug.Log ("Currently Entering traverseGrid with pos = " + rowPos.ToString());
		if(paperGrid_1[pos])
		{
			currentlyInTransition = true;
			TESTINGnumOfTornVerts++;
		}
		
		if(addingToPieceOne && !currentlyInTransition)
		{
			//island1
			//CuttPieceOneFaces
			//CuttPieceOneVerts
			if(checkingRow)
			{
				island1.Add (pos);
			}
			else
			{
				island1_Vertical.Add (pos);
			}
		}
		else if(!addingToPieceOne && !currentlyInTransition)
		{
			//island2
			//CuttPieceTwoFaces
			//CuttPieceTwoVerts
			if(checkingRow)
			{
				island2.Add (pos);
			}
			else
			{
				island2_Vertical.Add (pos);
			}
		}
		
		//Here we add the vertice to both lists for new both new meshes
		else if(currentlyInTransition)
		{
			//island1
			//CuttPieceOneFaces
			//CuttPieceOneVerts
			//island2
			//CuttPieceTwoFaces
			//CuttPieceTwoVerts
			if(checkingRow)
			{
				island1.Add (pos);
				island2.Add (pos);
			
				//Reset flags for next function iteration through this function
				//if(listIndice != tempList.Count - 1)Debug.LogWarning(" Testing " + paperGrid_1[tempList[listIndice + 1]].ToString());
				if(listIndice != tempList.Count - 1)
				{
					//if((paperGrid_1.ContainsKey(new Vector2(rowPos.x, rowPos.y + 0.1f)) && !paperGrid_1[new Vector2(rowPos.x, rowPos.y + 0.1f)])
						//|| !paperGrid_1.ContainsKey(new Vector2(rowPos.x, rowPos.y + 0.1f))
						//)
					//if(listIndice != tempList.Count - 1)
					//if(!paperGrid_1[new Vector2(tempList[listIndice + 1].x, tempList[listIndice + 1].y)])
					if(!tearLine.ContainsKey(tempList[listIndice + 1]))
					{
					
						if(addingToPieceOne) addingToPieceOne = false;
						else addingToPieceOne = true;
					}
				}
				else
				{
					//if(addingToPieceOne) addingToPieceOne = false;
					//else addingToPieceOne = true;
				}
			
				currentlyInTransition = false;
			} 
			else
			{
				//Checking Colomn
				
				island1_Vertical.Add (pos);
				island2_Vertical.Add (pos);
				
				if(!SimpleCutt)
				{
					if(listIndice != 0)//tempList.Count - 1
					{
						if(!tearLine.ContainsKey(tempList[listIndice - 1]))// + 1
						{
							if(addingToPieceOne) addingToPieceOne = false;
							else addingToPieceOne = true;
						}
					}
				}
				else
				{
					if(listIndice != tempList.Count - 1)//tempList.Count - 1
					{
						if(!tearLine.ContainsKey(tempList[listIndice + 1]))// + 1
						{
							if(addingToPieceOne) addingToPieceOne = false;
							else addingToPieceOne = true;
						}
					}	
				}
			
				currentlyInTransition = false;
			}
		}
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
	private void TraverseGridAddToNewPiece2(Vector3 origPos, Vector2 pos, bool currentlyInTransition, int listIndice, 
		List<Vector3> tempList, Vector3 rowTearStartPos, Vector3 endPosTearStartPos, bool readyToDeterminShape,
		int startingVertIndice)
	{
		if(paperGrid_1[pos])//tearLine.ContainsKey(endPosTearStartPos)) 
		{
			currentlyInTransition = true;
			TESTINGnumOfTornVerts++;
		}
		
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
			island1.Add (pos);
			island2.Add (pos);
			
			/*
			if(listIndice != tempList.Count - 1)
			{
				if(addingToPieceOne) addingToPieceOne = false;
				else addingToPieceOne = true;
			}*/
			///*
			//Reset flags for next function iteration through this function
			if(listIndice != tempList.Count - 1)
			{
				//if(!tearLine.ContainsKey(tempList[listIndice + 1]))
				{
				//{
					//Debug.Log("****S-Like shape ==== " + listIndice.ToString() + " , " + TearLineMovingInSLikeShape(rowTearStartPos, endPosTearStartPos, listIndice, startingVertIndice, tempList));
					
					//Make sure the tearLine is not makeing a 'U-like' turn
					if((TearLineMovingInSLikeShape(rowTearStartPos, endPosTearStartPos, listIndice, startingVertIndice, tempList) && readyToDeterminShape)) 
						//|| !readyToDeterminShape
						//)
						//Debug.LogWarning("************************************S-Like SHAPE TRUE!***************************************");
						if(addingToPieceOne) addingToPieceOne = false;
						else addingToPieceOne = true;
					}
					//Else a Uturn, therefore, we dont change which island mesh we are adding to
				//}
				
			}
			//*/
		
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
	private bool TearLineMovingInSLikeShape(Vector3 rowTearStartPos, Vector3 endPosTearStartPos, int listIndice, int startingVertIndice, List<Vector3> tempList)
	{
		//return true;//
		
		
		
		
		
		
		
		
		
		
		
		
		//Find the time associated with rowTearStartPos && endPosTearStartPos, then find y components for the
		// tear vertex with time greater than endPos and time less than start pos to setermine the change in times
		//Then we will determine what shape is being made based off this time change checking
		
		bool returnVal = false;
		
		int endTime = (int)tearLineTime[endPosTearStartPos];
		int startTime = (int)tearLineTime[rowTearStartPos];
		bool timeSwitched = false;
		
		if(endTime < startTime)
		{
			timeSwitched = true;
			Debug.LogError("TIME IS SWITCHED!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
		}
		
		float StartChangeInY = ChangeInHeightTearVerticeDir(rowTearStartPos, true, listIndice, startingVertIndice, tempList, timeSwitched);
		float EndChangeInY = ChangeInHeightTearVerticeDir(endPosTearStartPos, false, listIndice, startingVertIndice, tempList, timeSwitched);
		
		//Debug.LogWarning("StartChangeInY = " + StartChangeInY.ToString());
		//Debug.LogWarning("EndChangeInY = " + EndChangeInY.ToString());
		
		if(StartChangeInY == rowTearStartPos.y && EndChangeInY == rowTearStartPos.y)
		{
			Debug.LogError("S SHAPE DETERMINATION FAILURE!!!!!!!!!!!!!!!!!!!!!!!!    ;-(");
			return true;//
		}
		else if((StartChangeInY == rowTearStartPos.y && EndChangeInY > rowTearStartPos.y) 
			|| (StartChangeInY > rowTearStartPos.y && EndChangeInY == rowTearStartPos.y))
		{
			return true;//
		}
		else if(((StartChangeInY < rowTearStartPos.y && EndChangeInY > rowTearStartPos.y) 
			|| (StartChangeInY > rowTearStartPos.y && EndChangeInY < rowTearStartPos.y))
			
			//||
			//((StartChangeInY == rowTearStartPos.y && EndChangeInY > rowTearStartPos.y) 
			//||
			//(StartChangeInY > rowTearStartPos.y && EndChangeInY == rowTearStartPos.y))
			)
		{
			return false;//	
		}
		else
		{
			return true;//
		}
		/*
		if((StartChangeInY > 0 && EndChangeInY > 0) || (StartChangeInY < 0 && EndChangeInY < 0)
			
			||
			(StartChangeInY < 0 && EndChangeInY == 0) || (StartChangeInY == 0 && EndChangeInY < 0) ||
			(StartChangeInY == 0 && EndChangeInY > 0) || (StartChangeInY > 0 && EndChangeInY == 0) 
			|| (StartChangeInY == 0 && EndChangeInY == 0))  
		{
			Debug.LogWarning("S-Like SHAPE TRUE!_0");
			returnVal = true;
		}
		else
		if(
			//(
			(StartChangeInY < 0 && EndChangeInY > 0) || (StartChangeInY > 0 && EndChangeInY < 0) 
			
			//||
			//(StartChangeInY < 0 && EndChangeInY == 0) || (StartChangeInY == 0 && EndChangeInY < 0) ||
			//(StartChangeInY == 0 && EndChangeInY > 0) || (StartChangeInY > 0 && EndChangeInY == 0) 
			//) 
			
			&& 
			rowTearStartPos.x != WIDTH_MAX && rowTearStartPos.x != WIDTH_MIN && 
			rowTearStartPos.y != HEIGHT_MIN && rowTearStartPos.y != HEIGHT_MAX
			)
		{
			Debug.LogWarning("S-Like SHAPE FALSE!_______________________________1");
			returnVal = false;
		}
		else if(StartChangeInY == 0 || EndChangeInY == 0 //|| rowTearStartPos.x == WIDTH_MAX || rowTearStartPos.x == WIDTH_MIN
			//|| rowTearStartPos.y == HEIGHT_MIN || rowTearStartPos.y == HEIGHT_MAX
			)
		{
			if(StartChangeInY == 0 || EndChangeInY == 0)
			{
				Debug.LogWarning("S-Like SHAPE TRUE!_1");
				Debug.LogError("NEXT VERTICE IN TEARLINE LIST HAS SAME HEIGHT!!!!");
			}
			returnVal = true;
		}
		else
		{
			Debug.LogWarning("S-Like SHAPE TRUE!_1");
			returnVal = true;	
		}*/
		
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
	private float  ChangeInHeightTearVerticeDir(Vector3 tornVertPos, bool startVertFlag, int listIndice, int startingVertIndice, List<Vector3> tempList, bool timeSwitched)
	{
		float otherTornVertPos = 0.0f;
		float returnVal = 0.0f;
		
		int currentTime = (int)tearLineTime[tornVertPos];
		
		int index = 1;
		
		int numberOfVertsToCompare = 4;
		
		if(timeSwitched) index *= -1;
		
		if(startVertFlag && tearLinePositionTime.ContainsKey(currentTime - index))
		{
			//if((startingVertIndice - index) < tempList.Count && (startingVertIndice - index) >= 0)
			//{
			//	tornVertPos = tempList[startingVertIndice - index];
			//}
			//else
			//{
				tornVertPos = tempList[startingVertIndice];
			//}
			//Get an average;
			int division = 1;
			otherTornVertPos = tearLinePositionTime[currentTime].y;
			for(int vertCount = 1; vertCount < numberOfVertsToCompare; vertCount++)
			{
				if(tearLinePositionTime.ContainsKey(currentTime - (vertCount * index)))
				{
					division++;
					otherTornVertPos += tearLinePositionTime[currentTime - (vertCount * index)].y;
				}
			}
			otherTornVertPos /= division;
			
			returnVal = otherTornVertPos;//tornVertPos.y - otherTornVertPos;
		}
		else //if(tearLinePositionTime.ContainsKey(currentTime + index))
		{
			//if((listIndice + index) < tempList.Count && (listIndice + index) >= 0)
			//{
			//	tornVertPos = tempList[listIndice + index];
			//}
			//else
			//{
				tornVertPos = tempList[listIndice];
			//}
			//Get an average;
			int division = 1;
			otherTornVertPos = tearLinePositionTime[currentTime].y;
			for(int vertCount = 1; vertCount < numberOfVertsToCompare; vertCount++)
			{
				if(tearLinePositionTime.ContainsKey(currentTime + (vertCount * index)))
				{
					division++;
					otherTornVertPos += tearLinePositionTime[currentTime + (vertCount * index)].y;
				}
			}
			otherTornVertPos /= division;
			
			returnVal = otherTornVertPos;//otherTornVertPos - tornVertPos.y;
		}
		
		return returnVal;
	}
	
	
	/// <summary>
	/// Traverses the grid add to new piece vertically.
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
	/// <param name='downSweep'>
	/// Down sweep.
	/// </param>
	private void TraverseGridAddToNewPieceVertically(Vector2 pos, bool currentlyInTransition, int listIndice, List<Vector3> tempList, bool downSweep)
	{
		//Debug.Log ("Currently Entering traverseGrid with pos = " + rowPos.ToString());
		if(paperGrid_1[pos])
		{
			currentlyInTransition = true;
			TESTINGnumOfTornVerts++;
		}
		
		if(addingToPieceOne && !currentlyInTransition)
		{
			//island1
			//CuttPieceOneFaces
			//CuttPieceOneVerts
			if(downSweep)
			{
				island1_2.Add (pos);
			}
			else
			{
				island1_2_2.Add (pos);
			}

		}
		else if(!addingToPieceOne && !currentlyInTransition)
		{
			//island2
			//CuttPieceTwoFaces
			//CuttPieceTwoVerts
			if(downSweep)
			{
				island2_2.Add (pos);
			}
			else
			{
				island2_2_2.Add (pos);
			}
			
		}
		
		//Here we add the vertice to both lists for new both new meshes
		else if(currentlyInTransition)
		{
			//island1
			//CuttPieceOneFaces
			//CuttPieceOneVerts
			//island2
			//CuttPieceTwoFaces
			//CuttPieceTwoVerts
			
			if(downSweep)
			{
				island1_2.Add (pos);
				island2_2.Add (pos);
			}
			else
			{
				island1_2_2.Add (pos);
				island2_2_2.Add (pos);
			}
		
			//Reset flags for next function iteration through this function
			//if(listIndice != tempList.Count - 1)Debug.LogWarning(" Testing " + paperGrid_1[tempList[listIndice + 1]].ToString());
			if(listIndice != tempList.Count - 1)
			{
				//if((paperGrid_1.ContainsKey(new Vector2(rowPos.x, rowPos.y + 0.1f)) && !paperGrid_1[new Vector2(rowPos.x, rowPos.y + 0.1f)])
					//|| !paperGrid_1.ContainsKey(new Vector2(rowPos.x, rowPos.y + 0.1f))
					//)
				//if(listIndice != tempList.Count - 1)
				//if(!paperGrid_1[new Vector2(tempList[listIndice + 1].x, tempList[listIndice + 1].y)])
				if(!tearLine.ContainsKey(tempList[listIndice + 1]))
				{
				
					if(addingToPieceOne) addingToPieceOne = false;
					else addingToPieceOne = true;
				}
			}
			else
			{
				//if(addingToPieceOne) addingToPieceOne = false;
				//else addingToPieceOne = true;
			}
		
			currentlyInTransition = false;
			
			
		}
	}
	
	/// <summary>
	/// Tears then reassigns new mesh for cloned paper world object...
	/// </summary>
	private void TearPaper()
	{
		//...pseudocode...
		//Check if user is trying to tear edge vertice:
		//
		//	A) if NO ->	 
		//				- dont allow tear,
		//
		//	B) if YES ->   
		//				- save original paper mesh state
		// 				- start 'tear sequence'
		//
		//					A- Tear sequence:
		//						A	- Start to add to list of vertices touched by user.
		//						B	- DO NOT allow a vertex to be added to this list if:
		//
		//							a - Same vertice already exists within list 
		//							b - If vertice is the third vertex within an existing mesh triangle face ( This
		//						        prevents the user from selecting anything other than a jagged curve!!!!!!!!!! NO LOOPS!!)
		//							c - Vertice HAS TO be the:
		//
		//								1) IFF on the edge of the world &&
		//								2) IFF an interior vertice ELSE USER INPUT OR SPEED OF MOUSE IF TOOO FAST! :-(....DOUBLE CHECK THIS!!!
		//							d - What if user input is too fast??? Well, we need a way to connext the vertices inbetween spaced ones collected from the
		//								user input
		//
		//						C - Once Tear line is found, a duplication of object needs to be performed, OR!!!!! These 'Island' vertices need to be
		//				 	  	    distinguished for further manipulation. The most likely solution will be to create a new object with the new sub section 
		//				  	  		of the original mesh. This will invold making sure the correect vertices, faces, normals, and UV coordinates are
		//				  	  		appropriatly mapped to the new mesh. Alongside this, distinguishing what is an 'island' needs to be performed.
		//				  	  		There are MANY types of tears which can occur. That are a lot, but we only care about several key factors:
		//
		//							a - First, we need to determine how many edged this cut connected with. There are Three possible options:
		//
		//								1) Same Edge - ...
		//								2) Adjacent Edge (Both Left and Right Adjacent Edges) - ...
		//								3) Opposite Edge - ...
		//
		//							b - Next We create or add to TWO global storages containing 1) CuttPiece#1 && 2) CuttPiece#2
		//							c - We then iterate through the mesh and add the vertices to each of these storages. The key here
		//						    	is EVERY EDGE VERTEX switches which list being currently added to. This works for any tear
		//						    	(except a loop tear, which is a LAMO tear) Then, we have distinguished which vertices beling to which
		//						    	'island' therefore we can now determine which vertices make up which faces which in turn contruct the
		//						    	islands.
		//							d - Find these associated verices, make new object, seperate the island meshes, and check with mesh
		//						    	contains the least amount of faces in order to distinguish which is the smallest for the player to move 
		//						    	next.
		//						D - Once this is done, the new island (mesh pieces) need their world max and min values reset for the next tear's
		//							edge detection for a new paper tear
		//							
		//				  
		//				  
		//				  
		//
		
		//We check to see if the player is touching an edge
		if(PlayerTouchingEdge(true) || cuttInProgress)//PlayerTouchingEdge()
		{
			//currentlyCalculatingCuttLine = true;
			
			//The following line has been put into place for testing purposes
			//CuttWorld();
			mouseTearPositions.Add(Input.mousePosition);
			//testDist_2();
			
			
		}
	}
	
	/// <summary>
	/// The mouse tear positions on screen
	/// </summary>
	List<Vector3> mouseTearPositions;
	
	/// <summary>
	/// Creates the tear vert line determined by user input
	/// </summary>
	/// <param name='UserPos'>
	/// User input position
	/// </param>
	//private void CreateTearVertLine(Vector3 UserPos)
	//{
	//	testDist_2();
	//					---> Currently not in use -> J.C.
	//}
	private bool haveStoredStartPos = false;
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
	
	private bool testInputMouseToMaxMinOfPaper()
	{
		//Loop through mesh vertices
		for (var itor = 0; itor < newMesh.Length; itor++)
		{
			//Find the screen poition of the vertex
			screenPoint = Camera.main.WorldToScreenPoint(Clone.GetComponent<MeshFilter>().mesh.vertices[itor]);
			
			//Get the current position of the mouse in the scene (world) with the same z-depth as the vertex being examined
    		Vector3 curPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
			
			//Now find the distance between the world mouse position and the vertex world position for distance comparison

			
			//Check is the distance is less than 1500 (arbitrarily found through trial and error)
			if(curPosition.x >= WIDTH_MAX ||
				curPosition.x <= WIDTH_MIN ||
				curPosition.y >= HEIGHT_MAX ||
				curPosition.y <= HEIGHT_MIN)
			{
				return true;
			}
		}
		return false;
	}
	
	/// <summary>
	/// Cutts the cloned paper world mesh
	/// </summary>
	private void CuttPaperWorldMesh()
	{
		//CURRENTLY, not in use, instead CutttWorld -> J.C.
	}
	
	/// <summary>
	/// Creates the new paper world clone and hides the original mesh renderer
	/// This function also sets flags handling which object is being manipulated
	/// </summary>
	private void CreateNewPaperWorld()
	{
		//Assign the clone to be the new instance
		Clone = (GameObject) Instantiate(newPaper, this.transform.position, this.transform.rotation);
		
		Clone_2 = (GameObject) Instantiate(newPaper, this.transform.position, this.transform.rotation);
		Clone_2.GetComponent<Test_Script_Cutt>().CloneObject = true;
		Clone_2.GetComponent<MeshRenderer>().enabled = false;
		
		//Turn off the original object's meshRenderer to hide
		this.GetComponent<MeshRenderer>().enabled = false;
		
		//Set flag true to start deformations
		Clone.GetComponent<Test_Script_Cutt>().CloneObject = true;
	}
	
	/// <summary>
	/// Tests the distance between a vertex and the mouse scene position to flag cutting
	/// </summary>
	private void testDist()
	{
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
			
			//Debug.LogWarning("Checking Mouse Down + " + itor.ToString() + "  ---- Dist = " + dist.ToString());
			
			//Check is the distance is less than 1500 (arbitrarily found through trial and error)
			if(dist < 1000)
			{
				//Flag the index into the mesh for further manipulation
   				Index = itor;
				
				//Flag that a vertex has been found within the mouse threshold for cutting
				NeedToDeleteFace = true;
				
				//Break here because we only care about one vertex to cutt at a time
				break;
			}
		}
	}
	
	/// <summary>
	/// Tests the distance between a vertex and the mouse scene position to flag cutting
	/// </summary>
	private void testDist_2()
	{
		//Loop through mesh vertices
		for (var itor = 0; itor < newMesh.Length; itor++)
		{
			//Find the screen poition of the vertex
			screenPoint = Camera.main.WorldToScreenPoint(Clone.GetComponent<MeshFilter>().mesh.vertices[itor]);
			
			//Get the current position of the mouse in the scene (world) with the same z-depth as the vertex being examined
    		Vector3 curPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
			
			//Now find the distance between the world mouse position and the vertex world position for distance comparison
			float dist = Vector3.Distance(curPosition, Clone.GetComponent<MeshFilter>().mesh.vertices[itor]);
			
			if(dist < 0) dist *= -1;
			
			//Muliply the distance to widen the difference between the mouse and all mesh vertices
			dist *= 10000;
			
			//Debug.LogWarning("Checking Mouse Down + " + itor.ToString() + "  ---- Dist = " + dist.ToString());
			
			//Check is the distance is less than 1500 (arbitrarily found through trial and error)
			if(dist < 1000 && !tearLine.ContainsKey(Clone.GetComponent<MeshFilter>().mesh.vertices[itor]) && NeighborVert(Clone.GetComponent<MeshFilter>().mesh.vertices[itor]))
			{
				//Debug.LogWarning("Adding a vert with itor = " + itor.ToString());
				//Debug.Log("New Tear vert = " + Clone.GetComponent<MeshFilter>().mesh.vertices[itor].ToString());
				tearLine.Add(Clone.GetComponent<MeshFilter>().mesh.vertices[itor], itor);
				paperGrid_1[Clone.GetComponent<MeshFilter>().mesh.vertices[itor]] = true;
			}
		}
	}
	
	/// <summary>
	/// This function checks if the vertex being added to the cutt line is a neighboring vertex
	/// </summary>
	/// <param name='index'>
	/// vertPos represents position of the vertex trying to be added to cut line
	/// </param>
	private bool NeighborVert(Vector3 vertPos)
	{
		bool returnVal = false;
		
		//Immedialy we return true if we are checking an 'edge' vertice
		if(vertPos.y == HEIGHT_MAX || 
			vertPos.y == HEIGHT_MIN ||
			vertPos.x == WIDTH_MAX ||
			vertPos.x == WIDTH_MIN)
		{
			return true;//Because we are on an edge, may want to optomize in future
		}
		
		float test = 0.1f;
		//Positions storing the neighboring vertices
		Vector3 TopNeighbor = new Vector3(vertPos.x, vertPos.y + test, vertPos.z);
		Vector3 BotNeighbor = new Vector3(vertPos.x, vertPos.y - test, vertPos.z);
		Vector3 LeftNeighbor = new Vector3(vertPos.x - test, vertPos.y, vertPos.z);
		Vector3 RightNeighbor = new Vector3(vertPos.x + test, vertPos.y, vertPos.z);
		Vector3 TopLeftNeighbor = new Vector3(vertPos.x - test, vertPos.y + test, vertPos.z);
		Vector3 TopRightNeighbor = new Vector3(vertPos.x + test, vertPos.y + test, vertPos.z);
		Vector3 BotLeftNeighbor = new Vector3(vertPos.x - test, vertPos.y - test, vertPos.z);
		Vector3 BotRightNeighbor = new Vector3(vertPos.x + test, vertPos.y - test, vertPos.z);
		
		
		//paperGrid stores the gridden vertice positions, therefore we can map out the neighborings
		//paperGrid
		
		//If no neighbors are within tearline list, we add this vertPos, and the closesest neighbor to the list!
		//If one neighbor is within tearline, we add to list
		//if two or more, we ignore and return false
		
		int NumNeighborsInTearList = 0;
		
		if(tearLine.ContainsKey(TopNeighbor))
		{
			++NumNeighborsInTearList;
		}
		if(tearLine.ContainsKey(BotNeighbor))
		{
			++NumNeighborsInTearList;
		}
		if(tearLine.ContainsKey(LeftNeighbor))
		{
			++NumNeighborsInTearList;
		}
		if(tearLine.ContainsKey(RightNeighbor))
		{
			++NumNeighborsInTearList;
		}
		if(tearLine.ContainsKey(TopLeftNeighbor))
		{
			++NumNeighborsInTearList;
		}
		if(tearLine.ContainsKey(TopRightNeighbor))
		{
			++NumNeighborsInTearList;
		}
		if(tearLine.ContainsKey(BotLeftNeighbor))
		{
			++NumNeighborsInTearList;
		}
		if(tearLine.ContainsKey(BotRightNeighbor))
		{
			++NumNeighborsInTearList;
		}
		
		if(NumNeighborsInTearList == 0)
		{
			returnVal = true;
			
			/*
			Vector3 newPos = ClosestNeightbor(vertPos, TopNeighbor, 
				BotNeighbor, LeftNeighbor, RightNeighbor, 
				TopLeftNeighbor, TopRightNeighbor, 
				BotLeftNeighbor, BotRightNeighbor);
			
			int index = 0;
			for(int ktor = 0; ktor < Clone.GetComponent<MeshFilter>().mesh.vertices.Length; ktor++)
			{
				if(Clone.GetComponent<MeshFilter>().mesh.vertices[ktor] == newPos)
				{
					index = ktor;
					break;
				}
			}
			tearLine.Add(newPos, index);
			*/
		}
		if(NumNeighborsInTearList == 1)
		{
			returnVal = true;
		}
		
		if(NumNeighborsInTearList >= 2)
		{
			returnVal = false;	
		}
		
		return returnVal;
		
	}
	
	/// <summary>
	/// Closests the neightbor.
	/// </summary>
	/// <param name='vertPos'>
	/// Center vert position.
	/// </param>
	/// <param name='TopNeighbor'>
	/// Top neighbor vert position.
	/// </param>
	/// <param name='BotNeighbor'>
	/// Bot neighbor vert position.
	/// </param>
	/// <param name='LeftNeighbor'>
	/// Left neighbor vert position.
	/// </param>
	/// <param name='RightNeighbor'>
	/// Right neighbor vert position.
	/// </param>
	/// <param name='TopLeftNeighbor'>
	/// Top left neighbor vert position.
	/// </param>
	/// <param name='TopRightNeighbor'>
	/// Top right neighbor vert position.
	/// </param>
	/// <param name='BotLeftNeighbor'>
	/// Bot left neighbor vert position.
	/// </param>
	/// <param name='BotRightNeighbor'>
	/// Bot right neighbor vert position.
	/// </param>
	private Vector3 ClosestNeightbor(Vector3 CenterVertPos, Vector3 TopNeighbor, 
		Vector3 BotNeighbor, Vector3 LeftNeighbor, 
		Vector3 RightNeighbor, Vector3 TopLeftNeighbor, 
		Vector3 TopRightNeighbor, Vector3 BotLeftNeighbor,
		Vector3 BotRightNeighbor)
	{
		float distToPrigin = 100000;
		Vector3 ClostestTornVert = Vector3.zero;
		
		
		foreach(Vector3 vec in tearLine.Keys)
		{
			float tempDist = Vector3.Distance(vec, CenterVertPos);
			if(tempDist < distToPrigin)
			{
				distToPrigin = tempDist;
				ClostestTornVert = vec;
			}
		}
		
		//Now we have a pointer to the closest torn vert
		
		float distToClosestTornVert = 1000;
		Vector3 neighborToBeTornAlso = Vector3.zero;
		
		float tempDist2 = Vector3.Distance(TopNeighbor, ClostestTornVert);
		tempDist2 *= -1;
		if(tempDist2 < distToClosestTornVert)
		{
			neighborToBeTornAlso = TopNeighbor;
			distToClosestTornVert = tempDist2;
		}
		float tempDist3 = Vector3.Distance(BotNeighbor, ClostestTornVert);
		tempDist3 *= -1;
		if(tempDist3 < distToClosestTornVert)
		{
			neighborToBeTornAlso = BotNeighbor;
			distToClosestTornVert = tempDist3;
		}
		float tempDist4 = Vector3.Distance(LeftNeighbor, ClostestTornVert);
		tempDist4 *= -1;
		if(tempDist4 < distToClosestTornVert)
		{
			neighborToBeTornAlso = LeftNeighbor;
			distToClosestTornVert = tempDist4;
		}
		float tempDist5 = Vector3.Distance(RightNeighbor, ClostestTornVert);
		tempDist5 *= -1;
		if(tempDist5 < distToClosestTornVert)
		{
			neighborToBeTornAlso = RightNeighbor;
			distToClosestTornVert = tempDist5;
		}
		float tempDist6 = Vector3.Distance(TopLeftNeighbor, ClostestTornVert);
		tempDist6 *= -1;
		if(tempDist6 < distToClosestTornVert)
		{
			neighborToBeTornAlso = TopLeftNeighbor;
			distToClosestTornVert = tempDist6;
		}
		float tempDist7 = Vector3.Distance(TopRightNeighbor, ClostestTornVert);
		tempDist7 *= -1;
		if(tempDist7 < distToClosestTornVert)
		{
			neighborToBeTornAlso = TopRightNeighbor;
			distToClosestTornVert = tempDist7;
		}
		float tempDist8 = Vector3.Distance(BotLeftNeighbor, ClostestTornVert);
		tempDist8 *= -1;
		if(tempDist8 < distToClosestTornVert)
		{
			neighborToBeTornAlso = BotLeftNeighbor;
			distToClosestTornVert = tempDist8;
		}
		float tempDist9 = Vector3.Distance(BotRightNeighbor, ClostestTornVert);
		tempDist9 *= -1;
		if(tempDist9 < distToClosestTornVert)
		{
			neighborToBeTornAlso = BotRightNeighbor;
			distToClosestTornVert = tempDist9;
		}
		
		return neighborToBeTornAlso;
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
	/// Cutts then reassigns new mesh for cloned paper world object
	/// </summary>
	private void CuttWorld()
	{
		screenPoint = Camera.main.WorldToScreenPoint(transform.position);
		
		Vector3[] newMeshPos = new Vector3[Clone.GetComponent<MeshFilter>().mesh.vertices.Length];
		
		int indexer = 0;
		
		if(!NeedToDeleteFace) testDist();
		
		if(NeedToDeleteFace)
		{
			GameObject clone = null;
			int[] oldTriangles = null;
			
			newMesh = Clone.GetComponent<MeshFilter>().mesh.vertices;
			oldTriangles = Clone.GetComponent<MeshFilter>().mesh.triangles;

			
			//Debug.LogWarning("Just about to delete a vert");
			NeedToDeleteFace = false;
		
			int itor = 0;
			foreach(Vector3 vert in newMesh)
			{
				if(indexer != Index)
				{
					newMeshPos[indexer] = newMesh[itor];
					itor++;
				}
				indexer++;
			}
			//Now the verticies have been calculated

			//Now we need to delete the triangles associated with this vertex
		
			int Length = 0;
			
			for(int jtor = 0; jtor < oldTriangles.Length; jtor++)
			{
				if(oldTriangles[jtor] == Index)
				{
					Length++;
					break;
				}
			}
			
			int triangleCount = 1;
			
			int[] indexIntoTriangles = new int[Length * 3];
			
			//int[] indexIntroTrianglesPlacement = new int[Length*2];
			
			int indexInto = 0;
			
			for(int jtor = 0; jtor < oldTriangles.Length; jtor++)
			{
				if(oldTriangles[jtor] == Index)
				{
					indexIntoTriangles[indexInto] = jtor;
					//indexIntroTrianglesPlacement[indexInto] = triangleCount;
					
					if(triangleCount == 1)
					{
						
						indexIntoTriangles[indexInto + 1] = jtor + 1;
						indexIntoTriangles[indexInto + 2] = jtor + 2;
					}
					else if(triangleCount == 2)
					{
						indexIntoTriangles[indexInto + 1] = jtor + 1;
						indexIntoTriangles[indexInto + 2] = jtor - 1;
					}
					else if(triangleCount == 3)
					{
						indexIntoTriangles[indexInto + 1] = jtor - 2;
						indexIntoTriangles[indexInto + 2] = jtor - 1;
					}
					indexInto += 3;
					
					++triangleCount;
					if(triangleCount == 4) triangleCount = 1;
					break;
				}
				
				++triangleCount;
				
				if(triangleCount == 4) triangleCount = 1;
			}
			
			
			int[] newTriangles = new int[oldTriangles.Length - indexIntoTriangles.Length];
			
			int newCount = 0;
			
			//Now, indexIntoTriangles contains all of the indecies into oldTiangles which needs to be disgarded for new mesh
			
			for(int jjtr = 0; jjtr < oldTriangles.Length; jjtr++)
			{
				bool test = false;
				
				for(int tester = 0; tester < indexIntoTriangles.Length; tester++)
				{
					if(jjtr == indexIntoTriangles[tester])
					{
						test = true;
					}
				}
				
				if(!test)
				{
					newTriangles[newCount] = oldTriangles[jjtr];
					newCount++;
				}
			}

			Clone.GetComponent<MeshFilter>().mesh.triangles = newTriangles;
			
			Clone.GetComponent<MeshCollider>().sharedMesh = Clone.GetComponent<MeshFilter>().mesh;		
		}
	}
	
	#endregion
}
