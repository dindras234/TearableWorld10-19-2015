using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoverUp : MonoBehaviour {
	
	public Transform prefab;
	
	private Vector3 origin;
	private Vector3 originalPosition;
	private Quaternion originalRotation;
	
	GameObject[] reference;
	GameObject reference2;
	GameObject reference3;
	//GVariables GlobalVariables;
	
	TouchController touchController;
	TouchType touchType;
	TouchType prevTouchType;
	
	List<Vector2> fingerList;
	
	bool firstTouch = false;
	bool currMouseState;
	bool prevMouseState;
	
	float originX, originY;	
	float zLayerTmp;
	
	ChangeMeshScript script;
	FoldCollision foldCollide;
	Fold foldReference;
	bool isFolded;
	// Use this for initialization
	void Start () {

        // NOTICE : DOM
		
		originalPosition = new Vector3(0,0,-3);
		originalRotation = this.transform.rotation;
		
		touchController = GameObject.FindGameObjectWithTag("MainObject").GetComponent<TouchController>();
		foldCollide = GameObject.Find("Collision").GetComponent<FoldCollision>();
		foldReference = GameObject.Find("backsidepivot").GetComponent<Fold>();
		fingerList = new List<Vector2>();
		
		
		prefab = this.transform;
		
		zLayerTmp = GVariables.zCoverLayer;
		
		reference3 = this.transform.FindChild("coverup").gameObject;
		script = gameObject.GetComponent<ChangeMeshScript>();
		
		prevMouseState = false;
		currMouseState = false;
		firstTouch = false;
		isFolded = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		bool inputFound = false;
		if(Input.GetMouseButton(1) && GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().PlayerMovingPlatformState && inputFound)
		{
			inputFound = false;
		}
		else if(Input.GetMouseButton(1) && !GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().PlayerMovingPlatformState && !inputFound)
		{
			inputFound = true;
		}
		
		
		touchType = touchController.ReturnTouchType();
		fingerList = touchController.GetFingerPositions();
		Vector3 fingerPosition1 = new Vector3();
		Vector3 fingerPosition2 = new Vector3();
		Vector3 avgFingerPos = new Vector3();
		if(fingerList.Count == 2){
			fingerPosition1 = Camera.main.ScreenToWorldPoint(fingerList[0]);
			fingerPosition2 = Camera.main.ScreenToWorldPoint(fingerList[1]);
			avgFingerPos.x = (fingerPosition1.x+fingerPosition2.x)/2;
			avgFingerPos.y = (fingerPosition1.y+fingerPosition2.y)/2;
			avgFingerPos.z = -3;
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
			if((prevTouchType == TouchType.MULTITOUCH_2|| prevMouseState) && firstTouch){
				script.ChangeMesh(reference3.GetComponent<MeshFilter>().mesh.vertices,
					reference3.transform,
					.15f, .15f);
				this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, zLayerTmp + 1);
				firstTouch = false;
				isFolded = true;
			}
		}
		else if((touchType == TouchType.MULTITOUCH_2 || inputFound)){
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
				}
			}
			if(firstTouch){
				float mouseX = avgFingerPos.x - originX;
				float mouseY = avgFingerPos.y - originY;
			
				if (mouseX == 0 && mouseY == 0)
				{
					return;
				}
				
				if(!foldCollide.getHitting())
				{
					this.transform.rotation = originalRotation;
					float angleInDegrees = Mathf.Rad2Deg * Mathf.Atan2(mouseY, mouseX);
					this.transform.position = new Vector3(originX + mouseX/2, originY + mouseY/2, zLayerTmp);
					this.transform.Rotate(Vector3.forward, angleInDegrees);
				}
			}
		}
		/*else if(stop == 1){
			//Instantiate(prefab, new Vector3(originalPosition.x, originalPosition.y, originalPosition.z + 0.1f), originalRotation);
			stop++;
			//GVariables.zCoverLayer -= 0.1f;
		}*/
		prevTouchType = touchType;
		prevMouseState = currMouseState;
	}
}
