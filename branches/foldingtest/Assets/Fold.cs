using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fold : MonoBehaviour
{
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
	void Start ()
	{

		originalRotation = this.transform.rotation;
		
		reference = GameObject.FindGameObjectsWithTag("TOUCHCONTROLLER");

		touchController = reference[0].GetComponent<TouchController>();
		fingerList = new List<Vector2>();

	}
	
	// Update is called once per frame
	void Update ()
	{
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
			Debug.Log(" 1 X: " + fingerPosition1.x + " 1 Y: " + fingerPosition1.y+ " 1 Z: " + fingerPosition1.z);
			Debug.Log(" 2 X: " + fingerPosition2.x + " 2 Y: " + fingerPosition2.y + " 2 Z: " + fingerPosition2.z);
			Debug.Log(" avg X: " + avgFingerPos.x + " avg Y: " + avgFingerPos.y + " avg Z: " + avgFingerPos.z);
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
					
					float angleInDegrees = Mathf.Rad2Deg * (2 * Mathf.Atan2(mouseY, mouseX));
					
					
					this.transform.position = (new Vector3(mouseX + originX, mouseY + originY, 0));
					
					this.transform.Rotate(Vector3.forward, angleInDegrees);
					this.transform.Translate(new Vector3(-2, 0, 0));
					this.transform.Translate (new Vector3(originX, -originY, 0));
		}
	}
}