using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fold : MonoBehaviour
{	
	public Transform prefab;
	public List<Vector3> tearPoints;

	private Vector3 origin;
	private Vector3 originalPosition;
	private Quaternion originalRotation;
	
	Vector3 avgFingerPos;

	GameObject backsideReference;
	GameObject cameraReference;
	GameObject[] backsideCollisionReference;
	//GVariables GlobalVariables;
	
	TouchController touchController;
	TouchType touchType;
	TouchType prevTouchType;
	
	List<Vector2> fingerList;
	
	bool firstTouch = false;
	bool prevMousestate;
	bool currMouseState;
	//int stop; 
	bool isFolded;
	float originX, originY;
	float fingerPosX, fingerPosY;
	float zLayerTmp;

	ChangeMeshScript script;
	FoldCollision foldCollide;
	// Use this for initialization
	void Start ()
	{
		
		originalRotation = this.transform.rotation;
		//originalPosition = new Vector3(0,0,-1);
		originalPosition = this.transform.position;
		touchController = GameObject.FindGameObjectWithTag("MainObject").GetComponent<TouchController>();
		
		fingerList = new List<Vector2>();
		tearPoints = new List<Vector3>();
		
	//	stop = 0;
		
		prefab = this.transform;
		
		zLayerTmp = GVariables.zFoldLayer;
		
		backsideReference = this.transform.FindChild("backside").gameObject;
		foldCollide = GameObject.Find("Collision").GetComponent<FoldCollision>();
		backsideCollisionReference = GameObject.FindGameObjectsWithTag("FoldPlatform");
		cameraReference = GameObject.Find("Main Camera");
		
		script = gameObject.GetComponent<ChangeMeshScript>();
		
		prevMousestate = false;
		currMouseState = false;
		firstTouch = false;
		isFolded = false;
	}
	
	
	// Update is called once per frame
	void Update ()
	{
		bool inputFound = false;
		if(Input.GetMouseButton(1) && GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().PlayerMovingPlatformState)
		{
			inputFound = false;
		}
		else if(Input.GetMouseButton(1) && !GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().PlayerMovingPlatformState)
		{
			inputFound = true;
		}
		
		
		touchType = touchController.ReturnTouchType();
		//Debug.Log(touchType.ToString());
		fingerList = touchController.GetFingerPositions();
		Vector3 fingerPosition1 = new Vector3();
		Vector3 fingerPosition2 = new Vector3();
		avgFingerPos = new Vector3();
		if(fingerList.Count == 2)
		{
			fingerPosition1 = Camera.main.ScreenToWorldPoint(fingerList[0]);
			fingerPosition2 = Camera.main.ScreenToWorldPoint(fingerList[1]);
			avgFingerPos.x = (fingerPosition1.x+fingerPosition2.x)/2;
			avgFingerPos.y = (fingerPosition1.y+fingerPosition2.y)/2;
			avgFingerPos.z = -1;
			/*Debug.Log(" 1 X: " + fingerPosition1.x + " 1 Y: " + fingerPosition1.y+ " 1 Z: " + fingerPosition1.z);
			Debug.Log(" 2 X: " + fingerPosition2.x + " 2 Y: " + fingerPosition2.y + " 2 Z: " + fingerPosition2.z);
			Debug.Log(" avg X: " + avgFingerPos.x + " avg Y: " + avgFingerPos.y + " avg Z: " + avgFingerPos.z);*/
		}
		else if(inputFound){
            avgFingerPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		    avgFingerPos.z = -1;
			currMouseState = true;
			//Debug.Log ("mouse down: " + avgFingerPos.ToString());
		}
		if(!inputFound){
			currMouseState = false;
			//Debug.Log("mouse up");
		}
		if(touchType == TouchType.NONE && !inputFound)
		{
			
			//firstTouch = false;
			if((prevTouchType == TouchType.MULTITOUCH_2 || prevMousestate) && firstTouch)
			{
				Debug.Log("mouse up and prev mouse state");
				//stop++;
				//tearPoints = calcTearPoints();
				script.ChangeMesh(backsideReference.GetComponent<MeshFilter>().mesh.vertices, backsideReference.transform, 0.15f, 0.15f);
				this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, .8f);
				foreach(GameObject iterate in backsideCollisionReference)
				{
					iterate.transform.Translate (new Vector3(0,0,0.8f));
					//iterate.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);
				}
				firstTouch = false;
				isFolded = true;
			}
		}
		else if((touchType == TouchType.MULTITOUCH_2 || inputFound)/*&& stop < 1*/)
		{
			//if(!firstTouch && ((onFoldBorder(avgFingerPos) && !isFolded)|| (onUnfoldBorder(avgFingerPos) && isFolded)))
			if(!firstTouch)
			{
				if(onFoldBorder(avgFingerPos) && !isFolded)
				{
					this.transform.position = originalPosition;
					foreach(Transform child in transform)
					{
						originX = avgFingerPos.x;
						originY = avgFingerPos.y;
					}
					firstTouch = true;
				}
				else if(onUnfoldBorder(avgFingerPos) && isFolded)
				{
					firstTouch = true;
					script.RevertChanges();
				}
			}
			if(firstTouch)
			{
				fingerPosX = avgFingerPos.x - originX;
				fingerPosY = avgFingerPos.y - originY;
					
				if (fingerPosX == 0 && fingerPosY == 0)
				{
					return;
				}
			
				DoFoldTransforms(fingerPosX, fingerPosY, originX, originY);
			}
			//script.ChangeMesh(backsideReference.GetComponent<MeshFilter>().mesh.vertices, backsideReference.transform);
		}
		/*else if(stop == 1)
		{
			//Instantiate(prefab, originalPosition, originalRotation);
			stop++;
			//Debug.Log("Does this happen?");
			//GVariables.zFoldLayer -= 0.1f;
			
		}*/
		prevTouchType = touchType;
		prevMousestate = currMouseState;
	}
	
	void DoFoldTransforms(float fingerPosX, float fingerPosY, float originX, float originY )
	{
		if(!foldCollide.getHitting())
		{
			this.transform.rotation = originalRotation;
			float angleInDegrees = Mathf.Rad2Deg * (2 * Mathf.Atan2(fingerPosY, fingerPosX));
			this.transform.position = (new Vector3(fingerPosX + originX, fingerPosY+ originY, 0));
			this.transform.Rotate(Vector3.forward, angleInDegrees);
			this.transform.Translate(new Vector3(0, 0, 0));
			//this.transform.Translate (new Vector3(originX, -originY, zLayerTmp));
			this.transform.Translate (new Vector3(originX, -originY, zLayerTmp));
		}
	}
	
	
	// This method will calculate where the points are needed to create a tear for multifolding
//	List<Vector3> calcTearPoints()
//	{
//		List<Vector3> tmpPoints = new List<Vector3>();
//		
//		// Calculate slope for x and y
//		float nX = (avgFingerPos.x + originX)/2;
//		float nY = (avgFingerPos.y + originY)/2;
//		
//		// Calculate the distance of the tear in the X and the Y from the top left
//		// corner of the paper with respect to the mouse and the original click
//		float foldX = ((nY * (avgFingerPos.y - originY)) / (avgFingerPos.x - originX)) + nX;
//		float foldY = ((nX * (avgFingerPos.x - originX)) / (avgFingerPos.y - originY)) + nY;
//		
//		// insert the distances into 2 different Vector 3 spaces
//		Vector3 tmpFoldX = new Vector3(foldX, 0, 0); // y should be the distance from the center of the paper to the top edge
//		Vector3 tmpFoldY = new Vector3(0, foldY, 0); // x should be the distance from the center of the paper to the left edge
//		
//		// add the two vector spaces into the list
//		tmpPoints.Add(tmpFoldX);
//		tmpPoints.Add(tmpFoldY);
//		
//		//GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//	//	cube.AddComponent<Rigidbody>();
//		//cube.tranform.position = tmpFoldX;
//		//cube.transform.position = tmpFoldX;
//		//GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
//	//	cube2.AddComponent<Rigidbody>();
////		cube2.tranform.position = tmpFoldY;
//		//cube2.transform.position = tmpFoldY;
//		
//		return tmpPoints;
//	}
	
	public bool onFoldBorder(Vector3 raytrace)
	{
		List<GameObject> objThatGotHit = new List<GameObject>();
		RaycastHit hit;
		Vector3 dir = cameraReference.transform.TransformDirection(Vector3.forward);
		if(Physics.Raycast(raytrace, dir, out hit,Mathf.Infinity ))
		{
			//it only cares aobut a raycast if the object is a type "platform" or type "removable"
			if(hit.transform.gameObject.tag=="foldborder")
			{
				return true;
			}
		}
		return false;
	}
	
	public bool onUnfoldBorder(Vector3 raytrace)
	{
		List<GameObject> objThatGotHit = new List<GameObject>();
		RaycastHit hit;
		Vector3 dir = cameraReference.transform.TransformDirection(Vector3.forward);
		if(Physics.Raycast(raytrace, dir, out hit,Mathf.Infinity ))
		{
			//it only cares aobut a raycast if the object is a type "platform" or type "removable"
			if(hit.transform.gameObject.tag=="unfoldborder")
			{
				return true;
			}
		}
		return false;
		
	}

}