using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class was created originally to change the faces of meshes that were covered by a fold.
/// Originally it figured out what vertices were covered, then began removing all faces that accessed that vertice.
/// Since we have changed to removing the meshcollider from the object instead, it now just checks if a point is covered
/// and once it finds one it removes the mesh colliders and moves on to the next object.
/// 
/// It can deal with objects with different tags to differentiate what you want to do.
/// Created by Douglas Weller
/// </summary>
public class ChangeMeshScript : MonoBehaviour {
	#region variables
	#region bounds
	/// <summary>
	/// predeclared variable for the max X point in a objects bounding box
	/// </summary>
		public Vector3 foldMaxX;
	/// <summary>
	/// predeclared variable for the min X point in a objects bounding box
	/// </summary>
		public Vector3 foldMaxY;
	/// <summary>
	/// predeclared variable for the max y point in a objects bounding box
	/// </summary>
		public Vector3 foldMinX;
	/// <summary>
	/// predeclared variable for the min y point in a objects bounding box
	/// </summary>
		public Vector3 foldMinY;

		private	Vector3 boundsPoint1;
		private Vector3 boundsPoint2;
		private Vector3 boundsPoint3;
		private Vector3 boundsPoint4;
	/// <summary>
	/// This is the current max x point on the coverup rectangle
	/// </summary>
		public Vector3 coverMaxX;
	/// <summary>
	/// This is the current min x point on the coverup rectangle
	/// </summary>
		public Vector3 coverMinX;
	/// <summary>
	/// This is the current max y point on the coverup rectangle
	/// </summary>
		public Vector3 coverMaxY;
	/// <summary>
	/// This is the current min y point on the coverup rectangle
	/// </summary>
		public Vector3 coverMinY;
	/// <summary>
	/// Dictionary with the type of platform in the world
	/// along with array of them
	/// </summary>
		private Dictionary<string,GameObject[]> platFormsInWorld;
	#endregion
	#region Used For RevertChange
	/// <summary>
	/// The list of objects that get their meshes changed.
	/// </summary>
	//	public List<GameObject> changedObjects;
	/// <summary>
	/// List of the triangle array for each object that was changed.
	/// </summary>
	//	private List<int[]> changedObjectsTriangles;
		public Dictionary<GameObject, int[]> changedObjs;
		public Dictionary<GameObject, int[]> origMeshCollider;
		public Dictionary<GameObject, int[]> origMeshColliderTri;
	#endregion

	#endregion
	// Use this for initialization
	void Start () {
		platFormsInWorld = new Dictionary<string, GameObject[]>();
		origMeshCollider = new Dictionary<GameObject, int[]>();
		changedObjs = new Dictionary<GameObject, int[]>();
		GrabPlatformsInWorld("FoldPlatform");
		GrabPlatformsInWorld("Platform");				
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	private void Update () 
	{
    }
	
	/// <summary>
	/// Changes the mesh with the square, then double checks that the point
	/// isn't in the missing triangles.
	/// </summary>
	/// <param name='tran'>
	/// The transform of the object thats over platforms
	/// </param>
	/// <param name='target'>
	/// Tthe tag of target platforms we want to remove
	/// </param>
	/// <param name='misTri'>
	/// array of triangles no longer used by the object
	/// </param>
	public void ChangeMeshWithSquareIgnoreMissingTriangles(Transform tran, string target, int[] misTri)
	{	
		List<Vector3> misTriLoc = new List<Vector3>();
		//find bounds
		Vector3[] verts = tran.GetComponent<MeshFilter>().mesh.vertices;
		Bounds foldBounds = tran.GetComponent<MeshFilter>().mesh.bounds;
		boundsPoint1 = tran.TransformPoint(foldBounds.max);
		boundsPoint2 = tran.TransformPoint(foldBounds.min);
		boundsPoint3 = tran.TransformPoint(new Vector3(foldBounds.min.x, foldBounds.max.y, foldBounds.max.z));
		boundsPoint4 = tran.TransformPoint(new Vector3(foldBounds.max.x, foldBounds.min.y, foldBounds.min.z));
		List<Vector3> possible = new List<Vector3>();
		possible.Add(boundsPoint1);
		possible.Add(boundsPoint2);
		possible.Add(boundsPoint3);
		possible.Add(boundsPoint4);
		foldMaxX = FindLargestX(possible);//point with the largest x value
		possible.Remove(foldMaxX); //removes it from possible so the next point can't have one point be max x and max y or min y
		foldMinX = FindSmallestX(possible); //point with smallest x
		possible.Remove(foldMinX); //removes it from possibles
		foldMaxY = FindLargestY(possible);
		possible.Remove(foldMaxY);
		foldMinY = FindSmallestY(possible);
		//This is a list of all the objects in the world that are platforms.
		GameObject[] targetPlatInWorld;
		Vector3[] backsideVerts = tran.GetComponent<MeshFilter>().mesh.vertices;
		if(platFormsInWorld.TryGetValue(target, out targetPlatInWorld))
		{
		//This is a list of vertices from meshPoints that hit something during the raycast
		//This is the current world space location for the current vertice checking whether to raycast
		Vector3[] objVert;	// This is the array that holds the vertices from the mesh that is currently being changed.
		Mesh objMesh; 	// This is the Mesh for the GameObjects that are being affected by ChangeMeshScript.
			//set the current missing triangle to their world space location at this moment
		for(int i = 0; i < misTri.Length; i = i+ 3)
		{
			misTriLoc.Add(tran.TransformPoint(verts[misTri[i]]));
			misTriLoc.Add(tran.TransformPoint(verts[misTri[i+1]]));
			misTriLoc.Add(tran.TransformPoint(verts[misTri[i+2]]));	
		}
		//After cycling through the array of vertices from meshPoints we cycle through the gameObjects that were hit
		foreach(GameObject obj in targetPlatInWorld){
				//set objMesh to the mesh of the current object, meshcollider and meshfilter are always the same
				//but the meshfilter will always be there and unchanged, so this is the basis for objmesh
			objMesh = obj.GetComponent<MeshFilter>().mesh;
			//double checks that there is in fact a mesh on the object.  If there isn't it skips this object and sends out a 
			//debug.log
			if(objMesh == null)
			{
				Debug.Log(obj.name + " has NO MESHFILTER.  So cannot delete mesh. Moving on.");
			}
			else
			{
				//set objVert to the vertices of the current object
				objVert = objMesh.vertices;
				//reset removeI so it is fresh for new object
					//This is the vertice currently being checked, set to world space
					Vector3 theCheckedVertice;
					//This cycles through the verts of the object currently being checked
					int i = 0;
			        while (i < objVert.Length) {	
						//set theCheckedVertice to world space using transformpoint
						theCheckedVertice = obj.transform.TransformPoint(objVert[i]);
						//This checks if the point is within the 2 triangles that make up the rectangle which is the fold.
						
					if(PointInTriangle(theCheckedVertice, foldMinX, foldMaxY, foldMaxX) || 
						PointInTriangle(theCheckedVertice, foldMinX, foldMinY, foldMaxX))
					{
						//check if the point is also inside the missing triangles
						bool inMissing = false;
						for(int k = 0; k < misTriLoc.Count; k = k + 3)
						{
								//if it is then set inMissing to true so we ignore it
							if(PointInTriangle(theCheckedVertice, misTriLoc[k], misTriLoc[k+1],
							misTriLoc[k+2]))
							{
								inMissing = true;
								break;
							}
						}
							//if it is not in there, its time to remove his meshcollider
							if(!inMissing)
						{
								if(!changedObjs.ContainsKey(obj))
								{
									changedObjs.Add(obj, objMesh.triangles);
									Destroy(obj.GetComponent<MeshCollider>());
								}
									break;
						}
					}
			            i++;
					}

			}
		}
		}
	}
	/// <summary>
	/// Changes the platforms with the basic square of the fold
	/// </summary>
	/// <param name='tran'>
	/// The transform of object doing deleting platforms
	/// </param>
	/// <param name='target'>
	/// the tag of the platform you want deleted
	/// </param>
	public void ChangeWithBasicMeshSquare(Transform tran, string target)
	{
		//grab bounds of object
		Bounds foldBounds = tran.GetComponent<MeshFilter>().mesh.bounds;
		boundsPoint1 = tran.TransformPoint(foldBounds.max);
		boundsPoint2 = tran.TransformPoint(foldBounds.min);
		boundsPoint3 = tran.TransformPoint(new Vector3(foldBounds.min.x, foldBounds.max.y, foldBounds.max.z));
		boundsPoint4 = tran.TransformPoint(new Vector3(foldBounds.max.x, foldBounds.min.y, foldBounds.min.z));
		//this will be a list of all the points that make triangle, and they will be removed as we learn where they reside
		List<Vector3> possible = new List<Vector3>();
		possible.Add(boundsPoint1);
		possible.Add(boundsPoint2);
		possible.Add(boundsPoint3);
		possible.Add(boundsPoint4);
		foldMaxX = FindLargestX(possible);//point with the largest x value
		possible.Remove(foldMaxX); //removes it from possible so the next point can't have one point be max x and max y or min y
		foldMinX = FindSmallestX(possible); //point with smallest x
		possible.Remove(foldMinX); //removes it from possibles
		foldMaxY = FindLargestY(possible);
		possible.Remove(foldMaxY);
		foldMinY = FindSmallestY(possible);
		//the fold cover will never have points removed
		//but players should be killed if they are inside of its radius
		//so whenever a fold happens you check if this is the foldcover
		//and if it is, you save these values so the player can check if he's inside
		//the triangles that make up the square.
		if(tran.gameObject.tag == "FoldCover")
		{
			coverMaxX = foldMaxX;
			coverMinX = foldMinX;
			coverMaxY = foldMaxY;
			coverMinY = foldMinY;
		}
		//Now do the actual deletion
		ChangeMeshWithSquare(tran, target);
	}
	/// <summary>
	/// Removes the meshcollider of platforms underneath the rectangular object which is a fold.
	/// But, it allows it to be smaller or bigger than it actually is
	/// </summary>
	/// <param name='tran'>
	/// The transform of the object that will be covering platforms.
	/// </param>
	/// <param name='target'>
	/// The tag of the platforms we are trying to delete
	/// </param>
	/// <param name='percentSize'>
	/// The percentage you want removed or added from rectangle, such as 0.05(for rectangle 95% as big or
	/// 1 for rectangle 2x as big
	/// </param>
	public void ChangeWithSmallerMeshSquare(Transform tran, string target, float percentSize)
	{
		
		Bounds foldBounds = tran.GetComponent<MeshFilter>().mesh.bounds;
		//Change the bounds based on the ppercent size you want
		float xChange = (foldBounds.max.x - foldBounds.min.x) * percentSize;
		float yChange = (foldBounds.max.y - foldBounds.min.y) * percentSize;
		//Debug.Log(xChange + " " + yChange);
		//get an pointfrom each of the bounds to make the rectangle
		boundsPoint1 = tran.TransformPoint(new Vector3(foldBounds.max.x-xChange, foldBounds.max.y-yChange, foldBounds.max.z));
		boundsPoint2 = tran.TransformPoint(new Vector3(foldBounds.min.x+xChange, foldBounds.min.y+yChange, foldBounds.min.z));
		boundsPoint3 = tran.TransformPoint(new Vector3(foldBounds.min.x+xChange, foldBounds.max.y-yChange, foldBounds.max.z));
		boundsPoint4 = tran.TransformPoint(new Vector3(foldBounds.max.x-xChange, foldBounds.min.y+yChange, foldBounds.min.z));
		//this will be a list of all the points that make triangle, and they will be removed as we learn where they reside
		List<Vector3> possible = new List<Vector3>();
		possible.Add(boundsPoint1);
		possible.Add(boundsPoint2);
		possible.Add(boundsPoint3);
		possible.Add(boundsPoint4);
		foldMaxX = FindLargestX(possible);//point with the largest x value
		possible.Remove(foldMaxX); //removes it from possible so the next point can't have one point be max x and max y or min y
		foldMinX = FindSmallestX(possible); //point with smallest x
		possible.Remove(foldMinX); //removes it from possibles
		foldMaxY = FindLargestY(possible);
		possible.Remove(foldMaxY);
		foldMinY = FindSmallestY(possible);
		//do deletion
		ChangeMeshWithSquare(tran, target);
	}
	
	/// <summary>
	/// Changes the mesh.
	/// This is the transform of the GameObject that is being used to determine what vertices should be removed.
	/// </param>
	private void ChangeMeshWithSquare(Transform tran, string target)
	{	
		//This is a list of all the objects in the world that are platforms.
		GameObject[] targetPlatInWorld;
		
		if(platFormsInWorld.TryGetValue(target, out targetPlatInWorld))
		{
			#region cycling through game objects for triangle deletion
			Vector3[] objVert;	// This is the array that holds the vertices from the mesh that is currently being changed.
			Mesh objMesh; 	// This is the Mesh for the GameObjects that are being affected by ChangeMeshScript. 			
			//After cycling through the array of vertices from meshPoints we cycle through the gameObjects that were hit
			foreach(GameObject obj in targetPlatInWorld){
				//set objMesh to the mesh of the current object, meshcollider and meshfilter are always the same
				//but the meshfilter will always be there and unchanged, so this is the basis for objmesh
				objMesh = obj.GetComponent<MeshFilter>().mesh;
				//double checks that there is in fact a mesh on the object.  If there isn't it skips this object and sends out a 
				if(objMesh == null)
				{
					Debug.Log(obj.name + " has NO MESHFILTER.  So cannot delete mesh. Moving on.");
				}
				else
				{
					//set objVert to the vertices of the current object
					objVert = objMesh.vertices;
						int i = 0;
						//This is the vertice currently being checked, set to world space
						Vector3 theCheckedVertice;
						//This cycles through the verts of the object currently being checked
				        while (i < objVert.Length) {			
							//set theCheckedVertice to world space using transformpoint
							theCheckedVertice = obj.transform.TransformPoint(objVert[i]);
							//This checks if the point is within the 2 triangles that make up the rectangle which is the fold.
						if(PointInTriangle(theCheckedVertice, foldMinX, foldMaxY, foldMaxX) || 
							PointInTriangle(theCheckedVertice, foldMinX, foldMinY, foldMaxX))
							{
							//make sure the gameobject hasn't already been added
						if(!changedObjs.ContainsKey(obj))
							changedObjs.Add(obj, objMesh.triangles);
							Destroy(obj.GetComponent<MeshCollider>());//now remove the meshcollider
								break;
							}
				            i++;
						}
					}
				}
			}
		}
		#endregion
	/// <summary>
	/// This deletes the meshcolliders of the platforms that have been 'torn out' by a tear, on the backside.
	/// This function should only be called once, at some point after the tear.  Its not right after tear
	/// because it would make the tear lag more noticeable
	/// </summary>
	/// <param name='missingTri'>
	/// List of triangles the backside no longer uses
	/// </param>
	/// <param name='tran'>
	/// The transform of the object with the missing triangles.
	/// </param>
	/// <param name='target'>
	/// The tag the platforms we are removing will have.
	/// </param>
	public void DeletePlatformsFromMissingTriangles(int[] missingTri, Transform tran, string target)
	{
		List<Vector3> missingTriangles = new List<Vector3>();
		Vector3[] verts = tran.GetComponent<MeshFilter>().mesh.vertices;//MeshCollider>().sharedMesh.vertices;
		bool triExists = false;
		//return missingTriangles;
		//This is creating a list filled with the current world coordinites of the triangles that
		//should removed since that part of the backside is gone.
		for(int i = 0; i < missingTri.Length; i = i+3)
		{
			missingTriangles.Add(tran.TransformPoint(verts[missingTri[i]]));
			missingTriangles.Add(tran.TransformPoint(verts[missingTri[i+1]]));
			missingTriangles.Add(tran.TransformPoint(verts[missingTri[i+2]]));
		}
		Vector3 pointA;
		Vector3 pointB;
		Vector3 pointC;
		GameObject[] tarPlatInWorld;
		//Checks the dictionary for the target, if it finds it it then sends the list
		//of gameobjects to tarPlatInWorld
		if(platFormsInWorld.TryGetValue(target, out tarPlatInWorld))
		{
		Vector3[] objVert;	// This is the array that holds the vertices from the mesh that is currently being changed.
		Mesh objMesh; 	// This is the Mesh for the GameObjects that are being affected by ChangeMeshScript.
		bool ifShouldBeRemoved;	
		Vector3 theCheckedVertice;
		foreach(GameObject obj in tarPlatInWorld)
		{	
				objMesh = obj.GetComponent<MeshFilter>().mesh;
				int[] chngObjTri;
				objVert = objMesh.vertices;
				ifShouldBeRemoved = false;
				for(int i = 0; i < missingTriangles.Count; i = i + 3)
				{
					pointA = missingTriangles[i];
					pointB = missingTriangles[i+1];
					pointC = missingTriangles[i+2];
					objMesh = obj.GetComponent<MeshFilter>().mesh;
					if(objMesh != null)
					{
						for(int j = 0; j < objVert.Length; j++)
						{
							theCheckedVertice = obj.transform.TransformPoint(objVert[j]);
							//checks if the vert is inside this a triangle thats been removed
							if(PointInTriangle(theCheckedVertice, pointA, pointB, pointC))
							{
								//if it has then break out out cause we don't need to check anymore
								ifShouldBeRemoved = true;
								break;
							//	obj.GetComponent<MeshCollider>().enabled = false;
							//	removeI.Add(j);
							}
						}
						//since one vert was found within a triangle, the whole plat meshcollider
						//is removed
						if(ifShouldBeRemoved)
						{
							Destroy(obj.GetComponent<MeshCollider>());
							//obj.SetActive(false);
							//obj.GetComponent<MeshCollider>().enabled = false;
							break;
						}
					}
				}
		}
		}
	}
	/// <summary>
	/// Calculates the new set of triangles for a mesh.  
	/// It determines what vertices should not longer be used and removes any triangle
	/// that uses it.
	/// 
	/// Code currently not in use since we changed from using one mesh per platform, 
	/// to have each platform made up of independant triangles.
	/// </summary>
	/// <param name='indicesToDelete'>
	/// Delete, list of verts that are to be removed
	/// </param>
	private int[] CalcTriangles(List<int> indicesToDelete, int[] triObjTri)
		{
			//Debug.Log(indicesToDelete.Count);
			//this is the current set of triangles before and after update
			int[] triangles = triObjTri;
			//This is a list of the new triangles
			List<int> newTrianglesList = new List<int>();
			//This is a bool that checks whether triangle should be removed or not
			bool dontRemoveTriangle;
			//this cycles through the array of triangles in triangles
			//it uses i+3 because a triangle is always 3 points, so triangle
			//is a array of sets of 3 points
			int triLength = triangles.Length;
			for(int i = 0; i < triLength; i = i+3)
			{
			//Debug.Log(triangles[i] + " " + triangles[i+1] + " " + triangles[i+2]);
				//set to true to begin with
				dontRemoveTriangle = true;
				//this cycles through the indices in indicesTodelete
				foreach(int j in indicesToDelete)
				{			
					//it checks if any pointi n the triangle is that current indice
					if((triangles[i] == j || triangles[i+1] == j || triangles[i+2] == j))
					{
						dontRemoveTriangle = false;	//if it is it sets dontRemoveTriangle to false so it won't be added
						break;
					}
				}
				//if dontRemoveTriangle was never set to false it adds the current triangle to 
				//the newTrianglesList
				if(dontRemoveTriangle)
				{
					newTrianglesList.Add(triangles[i]);
					newTrianglesList.Add(triangles[i+1]);
					newTrianglesList.Add (triangles[i+2]);
					//Debug.Log("CHECK2");
	
				}
			}
			//set newTrianglesList to an array and make triangles equal to it
			triangles = newTrianglesList.ToArray();
			return triangles;
			//set the triangle for the objMesh equal to newtriangles
			//triObjMesh.triangles = triangles;
			//return triObjMesh;
		}


	#region Find Min and Max Vector3
	/// <summary>
	/// Looks through array of vector3's for the one with max x.
	/// </summary>
	/// <returns>
	/// The largest x vector3.
	/// </returns>
	/// <param name='possibleMax'>
	/// Array filled with the Vector3's we're looking for.
	/// </param>
	private Vector3 FindLargestX(List<Vector3> possibleMax)
	{
		Vector3 largest;
		largest = possibleMax[0];
		foreach( Vector3 currentV in possibleMax)
		{
			if(largest.x < currentV.x)
				largest = currentV;
		}
		return largest;
	}
	/// <summary>
	/// Looks through array of vector3's for the one with max y.
	/// </summary>
	/// <returns>
	/// The largest y vector3.
	/// </returns>
	/// <param name='possibleMax'>
	/// Array filled with the Vector3's we're looking for.
	/// </param>
		private Vector3 FindLargestY(List<Vector3> possibleMax)
	{
		Vector3 largest;
		largest = possibleMax[0];
		foreach( Vector3 currentV in possibleMax)
		{
			if(largest.y < currentV.y)
				largest = currentV;
		}
		return largest;
	}
		/// <summary>
	/// Looks through array of vector3's for the one with minimum x.
	/// </summary>
	/// <returns>
	/// The smallest x.
	/// </returns>
	/// <param name='possibleMin'>
	/// Array filled with the Vector3's we're looking for.
	/// </param>
	private Vector3 FindSmallestX(List<Vector3> possibleMin)
	{
		Vector3 smallest;
		smallest = possibleMin[0];
		foreach( Vector3 currentV in possibleMin)
		{
			if(smallest.x > currentV.x)
				smallest = currentV;
		}
		return smallest;
	}
	
	/// <summary>
	/// Looks through array of vector3's for the one with minimum y.
	/// </summary>
	/// <returns>
	/// The smallest y.
	/// </returns>
	/// <param name='possibleMin'>
	/// Array filled with the Vector3's we're looking for.
	/// </param>
		private Vector3 FindSmallestY(List<Vector3> possibleMin)
	{
		Vector3 smallest;
		smallest = possibleMin[0];
		foreach( Vector3 currentV in possibleMin)
		{
			if(smallest.y > currentV.y)
				smallest = currentV;
		}
		return smallest;
	}
	#endregion
	#region Calculate Triangle area, and if point is in it
	//Function/Algorithm/Equation found from http://www.blackpawn.com/texts/pointinpoly/
	private bool SameSide(Vector3 p1, Vector3 p2, Vector3 a,Vector3 b)
	{
	    Vector3 cp1 = Vector3.Cross(b-a, p1-a);
		Vector3 cp2 = Vector3.Cross(b-a, p2-a);
	    if(Vector3.Dot(cp1, cp2) >= 0) return true;
	    else return false;
	}
	
	//Function/Algorithm/Equation found from http://www.blackpawn.com/texts/pointinpoly/
	/// <summary>
	/// Checks if the Vector3 point p, is somewhere in the triangle formed by the 3 points
	/// a, b, and c.
	/// </summary>
    private bool PointInTriangle(Vector3 p, Vector3 a,Vector3 b,Vector3 c)
	{
		// Compute vectors        
		Vector3 v0 = c - a;
		Vector3 v1 = b - a;
		Vector3 v2 = p - a;
		// Compute dot products
		float dot00 = Vector3.Dot(v0, v0);
		float dot01 = Vector3.Dot(v0, v1);
		float dot02 = Vector3.Dot(v0, v2);
		float dot11 = Vector3.Dot(v1, v1);
		float dot12 = Vector3.Dot(v1, v2);
		
		// Compute barycentric coordinates
		float invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
		float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
		float v = (dot00 * dot12 - dot01 * dot02) * invDenom;
		
		if((u >= 0) && (v >= 0) && (u + v < 1))
			return true;
		else return false;
	}
	#endregion
	#region Grab and Revert World
	/// <summary>
	/// Grabs the targeted platforms in world.  These are currently "FoldPlatforms" and 
	/// "Platforms", fold platforms are the ones that exist on the back side of the paper.
	/// </summary>
	/// <param name='target'>
	/// Tthe tag that the platform you want added is named.
	/// </param>
	public void GrabPlatformsInWorld(string target)
	{
		GameObject[] tempArray;
	//Do this so when the script hits the Awake function it will be able to replace the old platforms with new ones
		if(platFormsInWorld.ContainsKey(target))
		{
			tempArray = GameObject.FindGameObjectsWithTag(target);
			platFormsInWorld.Remove(target);
			platFormsInWorld.Add(target, tempArray);
		}
		else
		{
			tempArray = GameObject.FindGameObjectsWithTag(target);
			platFormsInWorld.Add(target, tempArray);
		}
	}
		/// <summary>
	/// Reverts the changes that were made by ChangeMesh
	/// It will set all meshes that are in changedObj back to have meshcolliders.
	/// </summary>
	public void RevertChanges()
	{
			//loops through each object to reset triangles
			foreach(GameObject currentObj in changedObjs.Keys)
			{
				currentObj.AddComponent<MeshCollider>();
			}
		changedObjs.Clear();
	}
	/// <summary>
	/// Reverts the world platforms back to how they originally were.  Since tear and fold both work by
	/// removing meshcolliders, you just need to add them back in.
	/// </summary>
	public void RevertWorld()
	{
		foreach(string tar in platFormsInWorld.Keys)
		{
			foreach(GameObject g in platFormsInWorld[tar])
			{
			if(g != null)
			{
				if(!g.GetComponent<MeshCollider>()) g.AddComponent<MeshCollider>();
			}
			}
		}
		changedObjs.Clear();
	}
	#endregion
}


