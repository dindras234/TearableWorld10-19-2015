using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollisionPivot : MonoBehaviour
{	
	public Transform prefab;
	public List<Vector3> tearPoints;

	private Vector3 origin;
	private Vector3 originalPosition;
	private Quaternion originalRotation;
	
	Vector3 avgFingerPos;

	GameObject[] reference;
	GameObject reference2;
	
	Component collisionReference;
	
	TouchController touchController;
	TouchType touchType;
	TouchType prevTouchType;
	
	List<Vector2> fingerList;
	
	bool firstTouch = false;
	bool prevMousestate;
	bool currMouseState;
	
	float originX, originY;
	float fingerPosX, fingerPosY;
	float zLayerTmp;

	ChangeMeshScript script;
	FoldCollision foldCollide;
	Fold foldReference;
	bool isFolded;
	// Use this for initialization
	void Start ()
	{
		collisionReference = GameObject.Find ("Collision").GetComponent<BoxCollider>();
		originalRotation = this.transform.rotation;
		//originalPosition = new Vector3(0,0,-1);
		originalPosition = this.transform.position;
		touchController = GameObject.FindGameObjectWithTag("MainObject").GetComponent<TouchController>();
		foldReference = GameObject.Find("backsidepivot").GetComponent<Fold>();
		
		fingerList = new List<Vector2>();
		tearPoints = new List<Vector3>();
		
		prefab = this.transform;
		
		zLayerTmp = GVariables.zFoldLayer;
		
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
				this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, zLayerTmp + 1);
				//GameObject.Find("Collision").collider.enabled = false;
				//Destroy(gameObject);
				collisionReference.collider.enabled = false;
				
				firstTouch = false;
				isFolded = true;
			}
		}
		else if(touchType == TouchType.MULTITOUCH_2 || inputFound)
		{
			if(!firstTouch)
			{
				if(foldReference.onFoldBorder(avgFingerPos) && !isFolded)
				{
					this.transform.position = originalPosition;
					foreach(Transform child in transform)
					{
						originX = avgFingerPos.x;
						originY = avgFingerPos.y;
					}
					firstTouch = true;
				}
				else if(foldReference.onUnfoldBorder(avgFingerPos) && isFolded)
				{
					firstTouch = true;
					collisionReference.collider.enabled = true;
				}
			}
			if(firstTouch){
				fingerPosX = avgFingerPos.x - originX;
				fingerPosY = avgFingerPos.y - originY;
					
				if (fingerPosX == 0 && fingerPosY == 0)
				{
					return;
				}
			
				DoFoldTransforms(fingerPosX, fingerPosY, originX, originY);
			}
		}
//		else if(stop == 1)
//		{
//			Instantiate(prefab, originalPosition, originalRotation);
//			stop++;
//			Debug.Log("you are stupid");
//			GVariables.zFoldLayer -= 0.1f;
//			
//			
//		}
		prevTouchType = touchType;
		prevMousestate = currMouseState;
	}
	
	void DoFoldTransforms(float fingerPosX, float fingerPosY, float originX, float originY )
	{
		this.transform.rotation = originalRotation;
		float angleInDegrees = Mathf.Rad2Deg * (2 * Mathf.Atan2(fingerPosY, fingerPosX));
		this.transform.position = (new Vector3(fingerPosX + originX, fingerPosY+ originY, 0));
		this.transform.Rotate(Vector3.forward, angleInDegrees);
		this.transform.Translate(new Vector3(0, 0, 0));
		this.transform.Translate (new Vector3(originX, -originY, zLayerTmp));
	}
}