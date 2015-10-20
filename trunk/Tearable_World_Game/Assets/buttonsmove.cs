using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class buttonsmove : MonoBehaviour{
	
	Transform leftTouchButton;
	Transform rightTouchButton;
	
	// Use this for initialization
	void Start () {
		leftTouchButton = transform.Find("left touch");
		rightTouchButton = transform.Find ("right touch");
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.deviceOrientation == DeviceOrientation.LandscapeLeft){
			leftTouchButton.localPosition = new Vector3(-6.5F, -4.8F, 0.0F);
			rightTouchButton.localPosition = new Vector3(6.5F, -4.8F,0.0F);
			leftTouchButton.localRotation = Quaternion.Euler(0.0F, 0.0F, 0.0F);
			rightTouchButton.rotation = Quaternion.Euler(0.0F, 0.0F, 0.0F);

		}
		else if(Input.deviceOrientation == DeviceOrientation.LandscapeRight){
			leftTouchButton.localPosition = new Vector3(6.5f, 4.8F, 0.0F);
			rightTouchButton.localPosition = new Vector3(-6.5f, 4.8F, 0.0F);
			leftTouchButton.localRotation = Quaternion.Euler(0.0F, 0.0F, 180.0F);
			rightTouchButton.localRotation = Quaternion.Euler(0.0F, 0.0F, 180.0F);
		}
		else if(Input.deviceOrientation == DeviceOrientation.Portrait){
			leftTouchButton.localPosition = new Vector3(6.5f, 4.8F, 0.0F);
			rightTouchButton.localPosition = new Vector3(6.5F, -4.8F, 0.0F);
			leftTouchButton.localRotation = Quaternion.Euler(0.0F, 0.0F, 90.0F);
			rightTouchButton.localRotation = Quaternion.Euler(0.0F, 0.0F, 90.0F);
		}
		else if(Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown){
			leftTouchButton.localPosition = new Vector3(-6.5F, -4.8F, 0.0F);
			rightTouchButton.localPosition = new Vector3(-6.5F, 4.8F, 0.0F);
			leftTouchButton.localRotation = Quaternion.Euler(0.0F, 0.0F, 270.0F);
			rightTouchButton.localRotation = Quaternion.Euler(0.0F, 0.0F, 270.0F);
		}
	}
}
