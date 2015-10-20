using UnityEngine;
using System.Collections;

public class WristMovement : MonoBehaviour 
{
	public float Rotation = 20.0f;
	public float RotationSpeed = 0.1f;
	
	private float minRot;
	private float maxRot;
	
	private bool currentlyStretching = false;
	
	// Use this for initialization
	private void Start () 
	{
		minRot = transform.rotation.eulerAngles.z - Rotation;
		maxRot = transform.rotation.eulerAngles.z + Rotation;
		
		//Debug.LogError("maxRot = " + maxRot.ToString());
		//Debug.LogError("minRot = " + minRot.ToString());
	}
	
	// Update is called once per frame
	private void Update () 
	{
		
		if(!GameObject.FindGameObjectWithTag("AnimationManager").GetComponent<IntroSceneAnimationManager>().AnimationDone)
		{
			if(currentlyStretching)
			{
				if(transform.rotation.eulerAngles.z > maxRot)
				{
					currentlyStretching = false;
				}
				
				transform.RotateAround(new Vector3(0, 0, 1), RotationSpeed);
			}
			else
			{
				if(transform.rotation.eulerAngles.z < minRot)
				{
					currentlyStretching = true;
				}
				
				transform.RotateAround(new Vector3(0, 0, 1), -RotationSpeed);
			}
		}
	}
}
