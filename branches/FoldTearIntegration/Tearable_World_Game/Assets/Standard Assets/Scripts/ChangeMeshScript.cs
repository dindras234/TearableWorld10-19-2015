using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class is attached to a GameObject with a mesh.  When called it will find the background(currently
/// set to wolfbackground 1 but using tags it will soon be set to the tag 'background')
/// and go through the vertices in the mesh.  It will find the ones that are inside the area of the background.
/// If they are, they will raycast.  If that raycast hits something it will add it to a list.
/// Then it will go through each object hit and check the vertices that hit something.
/// Using this it will remove the triangles of the objects that were hit using the indices that were
/// close to where the object was hit
/// 
/// Needs a GameObject with a tag called 'background' to work properly.
/// Created by Douglas Weller
/// </summary>
public class ChangeMeshScript : MonoBehaviour {
	#region variables
	#region bounds
	/// <summary>
	/// The the max X value in, world space, of the background.
	/// </summary>
		public float backgroundObjMaxX;
	/// <summary>
	/// The the max Y value in, world space, of the background.
	/// </summary>
		public float backgroundObjMaxY;
	/// <summary>
	/// The the min X value in, world space, of the background.
	/// </summary>
		public float backgroundObjMinX;
	/// <summary>
	/// The the min Y value in, world space, of the background.
	/// </summary>
		public float backgroundObjMinY;
	#endregion
	#region for raycast
	/// <summary>
	/// The direction and distance the raycast will be sent to determine what vertices to be removed.
	/// </summary>
		private Vector3 dir;
	/// <summary>
	/// The is where information regarding raycast will be returned.
	/// </summary>
		private RaycastHit hit;
	#endregion
	#region Used For RevertChange
	/// <summary>
	/// The list of objects that get their meshes changed.
	/// </summary>
		private List<GameObject> changedObjects;
	/// <summary>
	/// List of the triangle array for each object that was changed.
	/// </summary>
		private List<int[]> changedObjectsTriangles;
	#endregion

	#endregion
	// Use this for initialization
	void Start () {
		//This is finding the transform of the background, atm its only wolfbackground 1 but can easilly be changed
		//to something else using tags
		if(GameObject.FindGameObjectWithTag("background") != null)
		{
			Transform backgroundTransform = GameObject.FindGameObjectWithTag("background").transform;
			//This sets the bounds for the background
			Bounds backgroundBounds = backgroundTransform.GetComponent<MeshFilter>().mesh.bounds;
			//This code multiplies the bounds of the background by the localScale, then adds them to the localPosition of the background transform
			//After that it uses TransformPoint to set them to world space.
			if(backgroundTransform.rotation.y == Quaternion.Euler(0f, 180f,0f).y)
			{
				backgroundObjMaxX = backgroundTransform.TransformPoint((backgroundTransform.localPosition + (backgroundTransform.localScale.x * backgroundBounds.min))).x;
	   			backgroundObjMinX = backgroundTransform.TransformPoint((backgroundTransform.localPosition + (backgroundTransform.localScale.x * backgroundBounds.max))).x;
			}
			else if(backgroundTransform.rotation.y == Quaternion.Euler(0f, 0f,0f).y)
			{
				backgroundObjMaxX = backgroundTransform.TransformPoint((backgroundTransform.localPosition + (backgroundTransform.localScale.x * backgroundBounds.max))).x;
	   			backgroundObjMinX = backgroundTransform.TransformPoint((backgroundTransform.localPosition + (backgroundTransform.localScale.x * backgroundBounds.min))).x;
			}
			backgroundObjMaxY = backgroundTransform.TransformPoint((backgroundTransform.localPosition + (backgroundTransform.localScale.y * backgroundBounds.max))).y;
			backgroundObjMinY = backgroundTransform.TransformPoint((backgroundTransform.localPosition + (backgroundTransform.localScale.y * backgroundBounds.min))).y;
		}
		else
		{
			Debug.Log("ERROR NO OBJECT WITH TAG 'background'. Bounds and min/max are now set to arbitary values."); 
			//setting the bounds to arbitrary values
			backgroundObjMaxX = 24.0f;
			backgroundObjMinX = -24.3f;
			backgroundObjMaxY = 24.0f;
			backgroundObjMinY = -24.3f;
		}		
		changedObjects = new List<GameObject>(); 
		changedObjectsTriangles = new List<int[]>();
	}
	
	// Update is called once per frame
	void Update () {
    }
	
	/// <summary>
	/// Changes the mesh.
	/// </summary>
	/// <param name='meshPoints'>
	/// These is the vertices of the mesh that determine what vertices for other meshes should be removed.
	/// </param>
	/// <param name='tran'>
	/// This is the transform of the GameObject that is being used to determine what vertices should be removed.
	/// </param>
	/// <param name='radiusx'>
	/// This is half the size of the x value for the square that is used to find vertices near the vertices of meshPoints.
	/// </param>
	/// <param name='radiusy'>
	/// This is half the size of the y value for the square that is used to find vertices near the vertices of meshPoints.
	/// </param>
	public void ChangeMesh(Vector3[] meshPoints, Transform tran, float radiusx, float radiusy)
	{	
		changedObjects.Clear();
		changedObjectsTriangles.Clear();
		//The distance is determined by the integer value, and Vdirection from the Vector3.forward
		// Vector3.forward is towards the z axis.
	 	dir = tran.TransformDirection(tran.forward*3);
		//This is a list of objects that got hit by raycast from fold mesh.
		//List<GameObject> objThatGotHit = new List<GameObject>();
		GameObject[] platformsInWorld;
		platformsInWorld = GameObject.FindGameObjectsWithTag("Platform");
		//This is a list of vertices from meshPoints that hit something during the raycast
		List<Vector3> vertWithinBoundsList = new List<Vector3>();
		//This is the current world space location for the current vertice checking whether to raycast
		Vector3 worldPosofVert;
		
		#region check whether to raycast and what vertice hit something
		//This loops through each vertice in meshPoints and determines what objects are beneath it.
		//it also adds each vertice that has a raycast hit to verticesThatHitList, so the amount that need 
		//to be checked later is smaller
		for(int j = 0; j < meshPoints.Length; j++)
		{
			//determines the world space coordinate of meshPoints[j] using that mesh GameObject's transform
			worldPosofVert = tran.TransformPoint(meshPoints[j]);
			//This checks to see if that coordinate is within the background
			if( worldPosofVert.x < backgroundObjMaxX && worldPosofVert.x > backgroundObjMinX && worldPosofVert.y < backgroundObjMaxY && worldPosofVert.y > backgroundObjMinY)
			{
						//This will be a list that is checked for what positions to delete
						vertWithinBoundsList.Add(worldPosofVert);
			}
		}
		#endregion
		#region cycling through game objects for triangle deletion
		Vector3[] objVert;	// This is the array that holds the vertices from the mesh that is currently being changed.
		Mesh objMesh; 	// This is the Mesh for the GameObjects that are being affected by ChangeMeshScript.
		List<int> removeI = new List<int>();	// List of indices that will be used to remove triangles.
		
		//After cycling through the array of vertices from meshPoints we cycle through the gameObjects that were hit
		foreach(GameObject obj in platformsInWorld){
			//set objMesh to the mesh of the current object
			changedObjects.Add(obj);	// add the object to ChangedObjects so you can keep track of objects changed
			objMesh = obj.GetComponent<MeshFilter>().mesh;
			//double checks that there is in fact a mesh on the object.  If there isn't it skips this object and sends out a 
			//debug.log
			if(objMesh == null)
			{
				Debug.Log(obj.name + " has NO MESHFILTER.  So cannot delete mesh. Moving on.");
			}
			else
			{
				changedObjectsTriangles.Add(objMesh.triangles); //add the mesh for that object to ChangedObjectMesh so you can keep track of original
				//set objVert to the vertices of the current object
				objVert = objMesh.vertices;
				//reset removeI so it is fresh for new object
				removeI.Clear();
				//sets verticesThatHitList to an array 
				Vector3[] vertWithinBoundsArray = vertWithinBoundsList.ToArray();
				//Length of verticesThathitArray, so we don't have to call it again every 
				//time we want to check length
				int verWithinBoundsArrayLen = vertWithinBoundsArray.Length;
				
				// for loop that checks for vertices in which to use deletion on
				//This loops through verticesThatHit and then checks the object mesh to see
				//if any of these are close
				for(int j = 0; j < verWithinBoundsArrayLen; j++) 
				{
					int i = 0;
					//This is the vertice currently being checked, set to world space
					Vector3 theCheckedVertice;
					//this is the vertice from verticesThatHitArray that is currently being checked
					Vector3 theOriginOfSquareCheck;
					//This cycles through the verts of the object currently being checked
			        while (i < objVert.Length) {			
						//set theCheckedVertice to world space using transformpoint
						theCheckedVertice = obj.transform.TransformPoint(objVert[i]);
						theOriginOfSquareCheck = vertWithinBoundsArray[j];
						//This creates a square around theOriginofSquareCheck so the vertices don't have to
						//hit exactly on, if theCheckedVertice is within range then code goes on to remove it
						if(   (theCheckedVertice.y < theOriginOfSquareCheck.y + radiusy)
							&&(theCheckedVertice.y > theOriginOfSquareCheck.y - radiusy)
							&&(theCheckedVertice.x < theOriginOfSquareCheck.x + radiusx)
							&&(theCheckedVertice.x > theOriginOfSquareCheck.x - radiusx))
						{
							//check if this indice has been added to be removed already
							if(!removeI.Contains(i))
							{
								//if it hasn't add it
									removeI.Add(i);
							}
						}
			            i++;
					}
				}
				//call calc triangle giving it removeI
				CalcTriangles(removeI, objMesh);
				//set the objects sharedMesh to objMesh, which was change in calcTriangles
				if(obj.GetComponent<MeshCollider>() != null)
				{
				obj.GetComponent<MeshCollider>().sharedMesh = null;//set to null so it will accept new mesh
				obj.GetComponent<MeshCollider>().sharedMesh = objMesh;//the new mesh
				}
				else
				{
					obj.GetComponent<MeshFilter>().mesh = null;
					obj.GetComponent<MeshFilter>().mesh = objMesh;
				}
			}
		}
		#endregion
}
	/// <summary>
	/// Calculates the triangles for tempMesh.
	/// </summary>
	/// <param name='indicesToDelete'>
	/// Delete, list of verts that are to be removed
	/// </param>
	public void CalcTriangles(List<int> indicesToDelete, Mesh triObjMesh)
		{
			//this is the current set of triangles before and after update
			int[] triangles = triObjMesh.triangles;
			//This is a list of the new triangles
			List<int> newTrianglesList = new List<int>();
			//This is a bool that checks whether triangle should be removed or not
			bool dontRemoveTriangle;
			//this cycles through the array of triangles in triangles
			//it uses i+3 because a triangle is always 3 points, so triangle
			//is a array of sets of 3 points
			for(int i = 0; i < triangles.Length; i = i+3)
			{
				//set to true to begin with
				dontRemoveTriangle = true;
				//this cycles through the indices in indicesTodelete
				foreach(int j in indicesToDelete)
				{			
					//it checks if any pointi n the triangle is that current indice
					if((triangles[i] == j || triangles[i+1] == j || triangles[i+2] == j))
							dontRemoveTriangle = false;	//if it is it sets dontRemoveTriangle to false so it won't be added
				}
				//if dontRemoveTriangle was never set to false it adds the current triangle to 
				//the newTrianglesList
				if(dontRemoveTriangle)
				{
					newTrianglesList.Add(triangles[i]);
					newTrianglesList.Add(triangles[i+1]);
					newTrianglesList.Add (triangles[i+2]);
	
				}
			}
			//set newTrianglesList to an array and make triangles equal to it
			triangles = newTrianglesList.ToArray();
			//set the triangle for the objMesh equal to newtriangles
			triObjMesh.triangles = triangles;

		}
	/// <summary>
	/// Reverts the changes that were made by ChangeMesh.
	/// It will set all meshes that were changed back to their
	/// originals.
	/// </summary>
	public void RevertChanges()
	{
		GameObject[] tempChangedObjects = changedObjects.ToArray(); //An array that holds all the objects that were changed
		int[][] tempChangedObjectsMesh = changedObjectsTriangles.ToArray(); //an array that holds all the original triangles
		//makes sure that this code only runs if their is an equal amount of objects to triangle arrays
		if(tempChangedObjects.Length == tempChangedObjectsMesh.Length)
		{
			//loops through each object to reset triangles
			for(int i = 0; i < tempChangedObjects.Length; i++)
			{
				GameObject currentObj = tempChangedObjects[i]; //current object being changed
				//mesh that will hold triangles, but first set to current sharedMesh for the MeshCollider
				Mesh tempObjMesh = currentObj.GetComponent<MeshCollider>().sharedMesh; 
				//makes sure that its not trying to revert to a set of triangles that is smaller than current set
				//the original should have the most amount of triangles
				if(currentObj.GetComponent<MeshCollider>().sharedMesh.triangles.Length < tempChangedObjectsMesh[i].Length)
				{
					tempObjMesh.triangles = tempChangedObjectsMesh[i]; //set the triangles in the temporary mesh
					currentObj.GetComponent<MeshCollider>().sharedMesh = null;//set sharedmesh to null so it will accept new mesh
					currentObj.GetComponent<MeshCollider>().sharedMesh = tempObjMesh; //set new mesh
				}
			}
		}
		else
			Debug.Log("MESH REVERT FAILURE!");
	}
}
