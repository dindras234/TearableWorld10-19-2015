using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class buttonsmove : MonoBehaviour {
	
	Transform leftTouchButton;
	Transform rightTouchButton;
	Transform inventory;
	Inventory inventoryItems;
	
	// Use this for initialization
	void Start () {
		leftTouchButton = transform.Find("left touch");
		rightTouchButton = transform.Find ("right touch");
		inventory = transform.Find ("inventory");
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
		{
			leftTouchButton.localPosition = new Vector3(-6.515384f, -4.789911f, 0.0f);
			rightTouchButton.localPosition = new Vector3(6.515384f, -4.789911f,0.0f);
			leftTouchButton.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
			rightTouchButton.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
			inventory.localPosition = (leftTouchButton.localPosition + rightTouchButton.localPosition)/2;
			inventory.localRotation = Quaternion.Euler (0.0f, 0.0f, 0.0f);

		}
		else if(Input.deviceOrientation == DeviceOrientation.LandscapeRight)
		{
			leftTouchButton.localPosition = new Vector3(6.515384f, 4.789911f, 0.0f);
			rightTouchButton.localPosition = new Vector3(-6.515384f, 4.789911f, 0.0f);
			inventory.localPosition = (leftTouchButton.localPosition + rightTouchButton.localPosition)/2;
			leftTouchButton.localRotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);
			rightTouchButton.localRotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);
			inventory.localRotation = Quaternion.Euler (0.0f, 0.0f, 180.0f);
		}
		else if(Input.deviceOrientation == DeviceOrientation.Portrait)
		{
			leftTouchButton.localPosition = new Vector3(6.515384f, 4.789911f, 0.0f);
			rightTouchButton.localPosition = new Vector3(6.515384f, -4.789911f, 0.0f);
			inventory.localPosition = (leftTouchButton.localPosition + rightTouchButton.localPosition)/2;
			leftTouchButton.localRotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
			rightTouchButton.localRotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
			inventory.localRotation = Quaternion.Euler (0.0f, 0.0f, 90.0f);


		}
		else if(Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)
		{
			leftTouchButton.localPosition = new Vector3(-6.515384f, -4.789911f, 0.0f);
			rightTouchButton.localPosition = new Vector3(-6.515384f, 4.789911f, 0.0f);
			inventory.localPosition = (leftTouchButton.localPosition + rightTouchButton.localPosition)/2;
			leftTouchButton.localRotation = Quaternion.Euler(0.0f, 0.0f, 270.0f);
			rightTouchButton.localRotation = Quaternion.Euler(0.0f, 0.0f, 270.0f);
			inventory.localRotation = Quaternion.Euler (0.0f, 0.0f, 270.0f);
		}

	}
}
