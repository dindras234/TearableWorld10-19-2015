using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoverUp2 : MonoBehaviour {
	
	private Vector3 origin;
	private Quaternion originalRotation;
	TouchController touchController;
	GameObject[] reference;
	TouchType touchType;
	List<Vector2> fingerList;
	bool firstTouch = false;
	float originX, originY;	
	// Use this for initialization
	void Start () {
		
		reference = GameObject.FindGameObjectsWithTag("TOUCHCONTROLLER");

		touchController = reference[0].GetComponent<TouchController>();
		originalRotation = this.transform.rotation;
		fingerList = new List<Vector2>();
	}
	
	// Update is called once per frame
	void Update () {
		
		
		Vector3 fingerPosition = new Vector3();
		fingerPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		if(Input.GetMouseButtonUp(0))
		{
			firstTouch = false;
		}
		

				if(!firstTouch)
				{
					foreach(Transform child in transform)
					{
						
						originX = fingerPosition.x;
						originY = fingerPosition.y;
					
					}
					firstTouch = true;
				}
				
				float mouseX = fingerPosition.x - originX;
				float mouseY = fingerPosition.y - originY;
					
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
