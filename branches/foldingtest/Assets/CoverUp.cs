using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoverUp : MonoBehaviour {
	
	private Vector3 origin;
	private Quaternion originalRotation;
	TouchController touchController;
	GameObject[] reference;
	TouchType touchType;
	List<Vector2> fingerList;
	bool firstTouch = false;
	float originX, originY;	
	int stop = 0;
	// Use this for initialization
	void Start () {
		
		reference = GameObject.FindGameObjectsWithTag("TOUCHCONTROLLER");

		touchController = reference[0].GetComponent<TouchController>();
		originalRotation = this.transform.rotation;
		fingerList = new List<Vector2>();
	}
	
	// Update is called once per frame
	void Update () {
		
		
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
			avgFingerPos.z = (fingerPosition1.z+fingerPosition2.z)/2;
		}
		if(touchType == TouchType.NONE)
		{
			firstTouch = false;
		}
		else if(touchType == TouchType.MULTITOUCH_2){
				if(!firstTouch)
				{
					foreach(Transform child in transform)
					{
						
						originX = avgFingerPos.x;
						originY = avgFingerPos.y;
					
					}
					firstTouch = true;
				}
				
				float mouseX = avgFingerPos.x - originX;
				float mouseY = avgFingerPos.y - originY;
					
				if (mouseX == 0 && mouseY == 0)
				{
					return;
				}
				
		
				this.transform.rotation = originalRotation;
				
				float angleInDegrees = Mathf.Rad2Deg * Mathf.Atan2(mouseY, mouseX);
				
				this.transform.position = new Vector3(originX + mouseX/2, originY + mouseY/2, 0);
				
				this.transform.Rotate(Vector3.forward, angleInDegrees);
		}

	}
}
