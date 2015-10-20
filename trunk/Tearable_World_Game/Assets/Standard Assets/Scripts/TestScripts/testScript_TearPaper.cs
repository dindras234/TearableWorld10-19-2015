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

public class testScript_TearPaper : MonoBehaviour {
	
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
	
	#endregion
	
	#region Private Fields
	/// <summary>
	/// The screen point is used to store the position being 
	/// compared to an input screen position
	/// </summary>
	private Vector3 screenPoint;
	
	/// <summary>
	/// The offset is used for 'picking' an object up in world and moving with user input
	/// </summary>
	private Vector3 offset;
	
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
	/// This flag is used to determine whether or not a face needs to 
	/// be deleted (this is riggered when the user cuts the paper
	/// </summary>
	private bool NeedToDeleteFace;
	
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
		if(!CloneObject)CreateNewPaperWorld();
		
	}
	
	/// <summary>
	/// Update this instance, called once per frame
	/// </summary>
	private void Update () 
	{
	
	}
	
	/// <summary>
	/// Raises the mouse drag event.
	/// </summary>
	void OnMouseDrag()
	{
		screenPoint = Camera.main.WorldToScreenPoint(transform.position);
			
		offset = transform.position - Camera.main.ScreenToWorldPoint(
        		new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
		
		//CuttPaperWorldMesh();
		CuttWorld();
	}
	
	/// <summary>
	/// Cutts the cloned paper world mesh
	/// </summary>
	private void CuttPaperWorldMesh()
	{
		
	}
	
	/// <summary>
	/// Creates the new paper world clone and hides the original mesh renderer
	/// This function also sets flags handling which object is being manipulated
	/// </summary>
	private void CreateNewPaperWorld()
	{
		//Assign the clone to be the new instance
		Clone = (GameObject)Instantiate(newPaper, this.transform.position, this.transform.rotation);
		
		this.GetComponent<MeshRenderer>().enabled = false;
		
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
			if(dist < 1500)
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
	/// Cutts then reassigns new mesh for cloned paper world object
	/// </summary>
	private void CuttWorld()
	{
		
		Vector3[] newMeshPos = new Vector3[Clone.GetComponent<MeshFilter>().mesh.vertices.Length];
		
		int indexer = 0;
		
		testDist();
		
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


/*using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class testScript_TearPaper : MonoBehaviour 
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
	/// Cutt this instance represents whether or not this instance should
	/// be cutt.
	/// </summary>
	public bool cuttThisInstance;
	
	/// <summary>
	/// The picking object flags whether or not we are picking objects out of the scene
	/// or trying to cutt the paper
	/// </summary>
	public bool PickingObject;
	
	#endregion
	
	#region Private Fields
	/// <summary>
	/// The screen point is used to store the position being 
	/// compared to an input screen position
	/// </summary>
	private Vector3 screenPoint;
	
	/// <summary>
	/// The offset is used for 'picking' an object up in world and moving with user input
	/// </summary>
	private Vector3 offset;
	
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
	/// The pick flag is updated once 'PickingObject' is set from unityGUI
	/// </summary>
	private bool pickFlag;
	
	/// <summary>
	/// This flag is used to determine whether or not a face needs to 
	/// be deleted (this is riggered when the user cuts the paper
	/// </summary>
	private bool NeedToDeleteFace;
	
	#endregion
	

 
	// Use this for initialization
	void Start () 
	{
		//Initially, the user is not trying to cutt the paper object
		NeedToDeleteFace = false;
		
		//Initially, we set the paper's screen position to be zero
		screenPoint = new Vector3(0.0f, 0.0f, 0.0f);
		
		//Initialize the newMesh to be the same length of the previous
		newMesh = new Vector3[mesh.vertices.Length];
		
		//Set the local variable to keep track of script mode determined in onMouseDown()
		pickFlag = PickingObject;
		
		//Initially, we set this to true to trigger cloning
		cuttThisInstance = true;
	}
	/// <summary>
	/// Update this instance, called once per frame
	/// </summary>
	void Update () 
	{
		
	}
	
	/// <summary>
	/// Raises the mouse down event.
	/// </summary>
	void OnMouseDown()
	{
		
	}
	
	/// <summary>
	/// Cutts then creates new world mesh
	/// </summary>
	private void CuttAndCreateWorld()
	{
		Vector3[] newMeshPos = new Vector3[mesh.vertices.Length];
		
		int indexer = 0;

		
		testDist_1();
		
		if(NeedToDeleteFace)
		{
			
			GameObject clone = (GameObject)Instantiate(newPaper, this.transform.position, this.transform.rotation);
			Clone = clone;
			
			newMesh = clone.GetComponent<MeshFilter>().mesh.vertices;
			int[] oldTriangles  = clone.GetComponent<MeshFilter>().mesh.triangles;
			
			
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
				}
			}
			
			int triangleCount = 1;
			int[] indexIntoTriangles = new int[Length * 3];
			
			int indexInto = 0;
			
			for(int jtor = 0; jtor < oldTriangles.Length; jtor++)
			{
				if(oldTriangles[jtor] == Index)
				{
					indexIntoTriangles[indexInto] = jtor;
					
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
				}
				
				++triangleCount;
				if(triangleCount == 4) triangleCount = 1;
			}
			
			//Now indexIntroTrianglesPlacement holds the vertex index within it's local triangle, either 1,2 or 3
			//Now indexIntroTriangles holds indecies into triangles which contain the target destroyed vertex
			
			
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

			
			clone.GetComponent<MeshFilter>().mesh.triangles = newTriangles;

			clone.GetComponent<testScript_TearPaper>().CloneObject = true;
			clone.GetComponent<testScript_TearPaper>().cuttThisInstance = true;
			CloneObject = true;
			GetComponent<MeshRenderer>().enabled = false;
			cuttThisInstance = false;
				
			
			
		}
	}
	
	/// <summary>
	/// Tests the distance between a vertex and the mouse scene position to flag cutting
	/// </summary>
	private void testDist_1()
	{
		//Loop through mesh vertices
		for (var itor = 0; itor < newMesh.Length; itor++)
		{
			//Find the screen poition of the vertex
			screenPoint = Camera.main.WorldToScreenPoint(mesh.vertices[itor]);
			
			//Get the current position of the mouse in the scene (world) with the same z-depth as the vertex being examined
    		Vector3 curPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
			
			//Now find the distance between the world mouse position and the vertex world position for distance comparison
			float dist = Vector3.Distance(curPosition, mesh.vertices[itor]);
			
			//Muliply the distance to widen the difference between the mouse and all mesh vertices
			dist *= 10000;
			
			//Debug.LogWarning("Checking Mouse Down + " + itor.ToString() + "  ---- Dist = " + dist.ToString());
			
			//Check is the distance is less than 1500 (arbitrarily found through trial and error)
			if(dist < 1500)
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
	/// Testing the method used for getting the distance between the mouse and a specific vertex located on the
	/// world paper mesh
	/// </summary>
	private void testDist()
	{
		for (var itor = 0; itor < newMesh.Length; itor++)
		{
			
			screenPoint = Camera.main.WorldToScreenPoint(Clone.GetComponent<MeshFilter>().mesh.vertices[itor]);
			
    		Vector3 curPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
			
			float dist = Vector3.Distance(curPosition, Clone.GetComponent<MeshFilter>().mesh.vertices[itor]);
			
			//dist *= 10000;
			
			//Debug.LogWarning("Checking Mouse Down + " + itor + "  ---- Dist = " + dist.ToString());
			
			if(dist < 0.3)
			{
   				//offset = new Vector3(newMesh[itor].x, newMesh[itor].y, newMesh[itor].z) - Camera.main.ScreenToWorldPoint(new 
					//Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
				
				Index = itor;
				NeedToDeleteFace = true;
				
				
				break;
			}
		}
	}
	
	/// <summary>
	/// Cutts then reassigns new mesh for cloned paper world object
	/// </summary>
	private void CuttWorld()
	{
		Vector3[] newMeshPos = new Vector3[Clone.GetComponent<MeshFilter>().mesh.vertices.Length];
		int indexer = 0;

		
		testDist_1();
		
		
		
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


	void OnMouseDrag()
	{
		
		if(pickFlag)
		{
			Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

    		Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
    		transform.position = curPosition;
		}
		else
		{
			screenPoint = Camera.main.WorldToScreenPoint(transform.position);
			offset = transform.position - Camera.main.ScreenToWorldPoint(
        			new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
		
			if(!pickFlag)
			{
				//Debug.Log("CUTT WORLD");
				
				if(CloneObject)
				{
					CuttWorld();	
				}
				else if(!CloneObject && cuttThisInstance)
				{
					CuttAndCreateWorld();
				}

			}
		}
		/*
    	Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

    	Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
    	
		newMesh.vertices[Index].x = curPosition.x;
		newMesh.vertices[Index].y = curPosition.y;
		newMesh.vertices[Index].z = curPosition.z;
		//mesh.vertices.SetValue(null, Index);
		
		mesh.SetTriangles = newMesh;
		
		
		
		var mesh : Mesh = GetComponent(MeshFilter).mesh;
		var vertices : Vector3[] = mesh.vertices;
		var normals : Vector3[] = mesh.normals;

		for (var i = 0; i < vertices.Length; i++)
    		vertices[i] += normals[i] * Mathf.Sin(Time.time);

		mesh.vertices = vertices;
	}		
}
*/