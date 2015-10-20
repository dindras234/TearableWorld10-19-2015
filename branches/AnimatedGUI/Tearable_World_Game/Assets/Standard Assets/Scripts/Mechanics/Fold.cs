using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fold : MonoBehaviour
{
	private Vector3 origin;
	private Vector3 originalPosition;
	private Quaternion originalRotation;
	TouchController touchController;
	GameObject[] reference;
	GameObject[] reference2;
	TouchType touchType;
	TouchType prevTouchType;
	List<Vector2> fingerList;
	bool firstTouch = false;
	int stop; 
	float originX, originY;
	public Transform prefab;
	// Use this for initialization
	void Start ()
	{

		originalRotation = this.transform.rotation;
		originalPosition = new Vector3(0,0,-1);
		reference = GameObject.FindGameObjectsWithTag("TOUCHCONTROLLER");
		reference2 = GameObject.FindGameObjectsWithTag("GLOBALVARIABLES");
		touchController = reference[0].GetComponent<TouchController>();
		fingerList = new List<Vector2>();
		stop = 0;
		prefab = this.transform;

	}
	
	// Update is called once per frame
	void Update ()
	{
		touchType = touchController.ReturnTouchType();
		Debug.Log(touchType.ToString());
		fingerList = touchController.GetFingerPositions();
		Vector3 fingerPosition1 = new Vector3();
		Vector3 fingerPosition2 = new Vector3();
		Vector3 avgFingerPos = new Vector3();
		if(fingerList.Count == 2){
			fingerPosition1 = Camera.main.ScreenToWorldPoint(fingerList[0]);
			fingerPosition2 = Camera.main.ScreenToWorldPoint(fingerList[1]);
			avgFingerPos.x = (fingerPosition1.x+fingerPosition2.x)/2;
			avgFingerPos.y = (fingerPosition1.y+fingerPosition2.y)/2;
			avgFingerPos.z = -1;
			/*Debug.Log(" 1 X: " + fingerPosition1.x + " 1 Y: " + fingerPosition1.y+ " 1 Z: " + fingerPosition1.z);
			Debug.Log(" 2 X: " + fingerPosition2.x + " 2 Y: " + fingerPosition2.y + " 2 Z: " + fingerPosition2.z);
			Debug.Log(" avg X: " + avgFingerPos.x + " avg Y: " + avgFingerPos.y + " avg Z: " + avgFingerPos.z);*/
		}
		if(touchType == TouchType.NONE)
		{
			firstTouch = false;
			if(prevTouchType == TouchType.MULTITOUCH_2){
				stop++;
			}
		}
		else if(touchType == TouchType.MULTITOUCH_2 && stop < 1){
			if(!firstTouch)
			{
				this.transform.position = originalPosition;
				foreach(Transform child in transform)
				{
					originX = avgFingerPos.x;
					originY = avgFingerPos.y;
				}
				firstTouch = true;
			}
			
			float fingerPosX = avgFingerPos.x - originX;
			float fingerPosY = avgFingerPos.y - originY;
				
			if (fingerPosX == 0 && fingerPosY == 0)
			{
				return;
			}
		
			DoFoldTransforms(fingerPosX, fingerPosY, originX, originY);
		}
		else if(stop == 1){
			DoTear();
			Instantiate(prefab, originalPosition, originalRotation);
			stop++;
			GlobalVariables.zFoldLayer -= 0.2f;
		}
		prevTouchType = touchType;
	}
	
	void DoFoldTransforms(float fingerPosX, float fingerPosY, float originX, float originY ){
		this.transform.rotation = originalRotation;
		float angleInDegrees = Mathf.Rad2Deg * (2 * Mathf.Atan2(fingerPosY, fingerPosX));
		this.transform.position = (new Vector3(fingerPosX + originX, fingerPosY+ originY, 0));
		this.transform.Rotate(Vector3.forward, angleInDegrees);
		this.transform.Translate(new Vector3(0, 0, 0));
		this.transform.Translate (new Vector3(originX, -originY, GlobalVariables.zFoldLayer));
	}
	
	void DoTear(){
		
	}
}