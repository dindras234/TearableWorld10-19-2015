using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FoldT : MonoBehaviour
{
	private Vector3 origin;
	private Quaternion originalRotation;
	TouchController touchController;
	GameObject[] reference;
	TouchType touchType;
	List<Vector2> fingerList;
	bool firstTouch = false;
	float originX, originY;
	
	// Use this for initialization
	void Start ()
	{
		//origin = new Vector3(1, -0.75f, 0);
		originalRotation = this.transform.rotation;
		
		//reference = GameObject.FindGameObjectsWithTag("TOUCHCONTROLLER");

		touchController = GameObject.FindGameObjectWithTag("Player").GetComponent<TouchController>();
		fingerList = new List<Vector2>();
		//touchType = TouchType.NONE;
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 fingerPosition = new Vector3();
		fingerPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//		float mouseX = fingerPosition.x;
//		float mouseY = fingerPosition.y;
//		float foldX = mouseY*mouseY/2/mouseX + mouseX/2;
//		float foldY = mouseX*mouseX/2/mouseY + mouseY/2;
//		
		
//		touchType = touchController.ReturnTouchType();
		//Debug.Log(touchType.ToString());
//		if(touchType == TouchType.NONE)
//		{
//			firstTouch = false;
//		}
		if(Input.GetMouseButtonUp(0))
		{
			firstTouch = false;
		}
		
//		if(touchType == TouchType.DRAG || touchType == TouchType.SWIPE )
//		{
//			fingerList = touchController.GetFingerPositions();
//			
//			if(fingerList.Count > 0)
//			{
				//Vector3 test = new Vector3(fingerList[0].x, fingerList[0].y, 0);
				//fingerPosition = Camera.main.ScreenToWorldPoint(test);
				//if(Input.GetMouseButtonDown(0))
				//{

					if(!firstTouch)
					{
						foreach(Transform child in transform)
						{
							//Vector3 newVec = new Vector3 (fingerPosition.x -1, fingerPosition.y + .75f, 0);
							
							originX = fingerPosition.x;
							originY = fingerPosition.y;
							//originalRotation = this.transform.rotation;
							//Vector3 newVec = new Vector3 (originX, originY, 0);
							//child.position = newVec;
						
						}
						firstTouch = true;
					}
					
					float mouseX = fingerPosition.x - originX;
					float mouseY = fingerPosition.y - originY;
						
					if (mouseX == 0 && mouseY == 0)
					{
						return;
					}
					
					//float foldX = (tempfpy*tempfpy/(2*tempfpx)) + (tempfpx/2);
					//float foldY = (tempfpx*tempfpx/(2*tempfpy)) + (tempfpy/2);
					//this.transform.position = origin;
					this.transform.rotation = originalRotation;
					
					float angleInDegrees = Mathf.Rad2Deg * (2 * Mathf.Atan2(mouseY, mouseX));
					
					//float angleInDegrees = Mathf.Rad2Deg * Mathf.Atan2(fingerPosition.y, fingerPosition.x - foldX);
					this.transform.position = (new Vector3(mouseX + originX, mouseY + originY, 0));
					//this.transform.position = new Vector3(fingerPosition.x, fingerPosition.y, 0);
					this.transform.Rotate(Vector3.forward, angleInDegrees);
					//this.transform.localScale = new Vector3(-1, 1, 1);
					//this.transform.Translate(new Vector3(0, 0, 0));
					this.transform.Translate (new Vector3(originX, -originY, 0));
			
				//}
				//Debug.Log (fingerPosition.x + " , " + fingerPosition.y + " Deg: " + angleInDegrees);
			//}
//			else{
//				Debug.Log("count <=0");
//			}
//		}
	}
}