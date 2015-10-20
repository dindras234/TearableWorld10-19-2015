using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Touch type.
/// </summary>
public enum TouchType 
{
	TAP,
	DRAG,
	SWIPE,
	MULTITOUCH_2,
	MULTITOUCH_3,
	MULTITOUCH_4,
	MULTITOUCH_5,
	MULTITOUCH_OTHER,
	NONE
}

/// <summary>
/// Touch controller.
/// </summary>
public class TouchController : MonoBehaviour 
{	
	List<Vector2> fingerPositions; //Keeps track of the postions for each finger
	int touchCount;  //keeps track of which finger in the loop we are on.
	float startTime;  //time when the touch starts
	float currentTime;  //keeps track of how long it has been since the touch started.
	bool isMultiTouch;  //boolean to see if the touch is multitouch
	TouchType touchType = TouchType.NONE;
	const float JUMP_SPEED = 5.0f; //Jump speed by default
	
	// Use this for initialization
	void Start () 
	{
		//initializes the list of finger positions
		fingerPositions = new List<Vector2>();
	}
	
	// Update is called once per frame
	void Update () 
	{ 
		//both lines here are placeholders for testing
		TouchType touch = ReturnTouchType();
		//Debug.Log (touch.ToString ());
		
		//TODO: Have the selected mesh be highlighted,
		//then have controls only work while selected.
		
		
		if(touch == TouchType.TAP)
		{
			//Get the list of fingers' positions
			//Only do this if there is one finger dragging.
			//Check if the distance between the finger's current position and
			//the mesh is more than zero.
			Debug.Log (fingerPositions.ToString ());
			
			//If finger is on the left hand of the screen, move left
			if(fingerPositions.Count == 1 && (fingerPositions[0].x) > Screen.width / 2){
				Move (new Vector3(-1, 0, 0));

			}
			//If finger is on the right hand side, move right
			else if(fingerPositions.Count == 1 & (fingerPositions[0].x) < Screen.width/2){
				Move (new Vector3(1, 0, 0));
			}
			//Else don't move.
			else Move (new Vector3(0,0,0));
			
		}
		
		//At this point, dragging at all will make a jump occur.
		//TODO: Make jump work by swiping specifically in a vertical upwards direction
		if(touch == TouchType.DRAG)
		{
			Jump();
		}
		
		// if(blah != TouchType.NONE){
			// Debug.Log(blah.ToString());
			// List<Vector2> lol = GetFingerPositions();
			// for(int i = 0; i < lol.Count; i++){
				// Debug.Log ("Finger " + i + " position: " + lol[i].x + " , " + lol[i].y);
			// }
		// }
	}
	
	/// <summary>
	/// Jump functionality.
	/// 
	/// Utilizies Physics.gravity to apply gravity automatically.
	/// </summary>
	public void Jump()
	{
		transform.Translate (Vector3.up*JUMP_SPEED*Time.deltaTime);
		Physics.gravity = new Vector3(0, -JUMP_SPEED, 0); //Unity has a PHYSICS PACKAGE THIS IS SO NICE.
	}
	
	/// <summary>
	/// Move the specified direction.
	/// </summary>
	/// <param name='direction'>
	/// Vector2 defining the direction of the swipe.
	/// Should be fleshed out to better work with code.
	/// </param>
	public void Move(Vector3 direction){
	
		if(direction.x < Screen.width/2)
		{
			this.transform.Translate (1, 0, 0);
		}
		else this.transform.Translate (-1, 0, 0);
	}
	
	#region ReturnTouchType ()
	/// <summary>
	/// Returns the type of the touch.
	/// How to use:
	/// Call in an update function that is used every frame to get
	/// data for the touch input.
	/// 
	/// Reurn types:
	/// TAP: Happens when the player touches the screen and holds finger in same place
	/// 	Every touch starts as a tap but can be changed to something else.
	/// SWIPE: if player moves finger on the screen for less than .2 seconds
	/// DRAG: if player moves finger on the screen for more than or equal to .2 seconds
	/// NONE: if the player has not touched or has just ended a touch
	/// MULTITOUCH #: the number after the MULTITOUCH represents how many fingers have been put down
	/// MULTITOUCH OTHER: if the number of fingers is not 2-5
	/// 
	/// </summary>
	/// <returns>
	/// The touch type.
	/// </returns>
	TouchType ReturnTouchType ()
	{
		touchCount = 0;  //resets the touchCount back to zero so it will be accurate.
		fingerPositions.Clear();  //Clears the List of finger positions.
		//Begin loop for checking touches
		foreach(Touch touch in Input.touches)
		{
			
			fingerPositions.Add(touch.position);
			switch(touch.phase)
			{
				case TouchPhase.Began:
					startTime = Time.time;
					isMultiTouch = false;
					if(Input.touchCount > 1)
					{
						isMultiTouch = true;
					
					// switch statement for checking how many fingers on screen for multitouch
						switch(Input.touchCount)
						{
							case 2:
								touchType = TouchType.MULTITOUCH_2;
								break;
							case 3:
								touchType = TouchType.MULTITOUCH_3;
								break;
							case 4:
								touchType = TouchType.MULTITOUCH_4;
								break;
							case 5:
								touchType = TouchType.MULTITOUCH_5;
								break;
							default:
								touchType = TouchType.MULTITOUCH_OTHER;
								break;
						}
					}
				// if it's not a multitouch it becomes a tap
					else
					{
						touchType = TouchType.TAP;
					}
					break;
				
				//if finger moved, check how long the finger has been on the screen to determine if it is a drag or swipe
				case TouchPhase.Moved:
					currentTime = Time.time - startTime;
					if(currentTime > .2 && !isMultiTouch)
					{
						touchType = TouchType.DRAG;
					}
					else if(!isMultiTouch)
					{
						touchType = TouchType.SWIPE;
					}
					break;
				
				// if finger moved and becomes stationary it becomes a drag instead
				case TouchPhase.Stationary:
					if (touchType == TouchType.SWIPE)
					{
						touchType = TouchType.DRAG;
					}
					break;
				
				// when finger is lifted from screen it becomes NONE again
				case TouchPhase.Ended:
					touchType = TouchType.NONE;
					break;
				
				// if program force quits or is facepalm then it becomes NONE
				case TouchPhase.Canceled:
					touchType = TouchType.NONE;
					break;
			}
			touchCount++;
			//Debug.Log(touchType.ToString()); // debug line for testing purposes
		}
		return touchType;
	}
	#endregion
	
	#region GetFingerPositions()
	/// <summary>
	/// Gets the finger positions.
	/// </summary>
	/// <returns>
	/// The finger positions.
	/// </returns>
	List<Vector2> GetFingerPositions()
	{
		return fingerPositions;
	}
	#endregion
}
