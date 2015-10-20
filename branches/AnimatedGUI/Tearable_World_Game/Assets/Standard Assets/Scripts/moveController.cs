using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// moveController class:
///    A general movement class in case we decide to do 
///    AI-controlled enemies.
/// 
/// Fields:
///    public int swipeLength - detects swipe length,
///       for character acceleration
///    public int swipeVariance - detects swipe variance
///    public int timeToSwipe - determines how long player 
///       has swiped, for acceleration
///    
///    private int swipeText;
///    private TouchInfo[] touchInfoArray - Array for storing
///       all touch info.
///    private int activeTouch = -1 - purely for determining
///       which touch is active.
/// 
/// 
/// Methods:
///    void Start() - Gets a reference to GUIText component,
///       then determine whether 
///    void Update() - Looks through every phase and determines 
///       which direction a swipe is in
///    void SwipeComplete(string msg, Touch touch) - 
///       determines whether or not a touch is completed,
///       then outputs a message for the designer to be aware
///    void Reset(Touch touch) - Resets the active touch.
/// 
/// 
/// </summary>

public class TouchInfo{
	public Vector2 touchPos;
	public bool swipeComplete;
	public float swipeTime;
}


public class moveController : MonoBehaviour
{
	public int swipeLength;
	public int swipeVariance;
	public int timeToSwipe;
	
	private GUIText swipeText;
	private TouchInfo[] touchInfoArray;
	private int activeTouch = -1;
	
	void Start(){
		//swipeText = (GUIText) GetComponent(typeof(GUIText));
		swipeText = GetComponent<GUIText>();
		touchInfoArray = new TouchInfo[5];
	}
	
	void Update(){
	
		if(Input.touchCount > 0 && Input.touchCount < 6){
			foreach(Touch touch in Input.touches){
				if(touchInfoArray[touch.fingerId] == null){
					touchInfoArray[touch.fingerId] = new TouchInfo();
				}
				if(touch.phase == TouchPhase.Began){
					touchInfoArray[touch.fingerId].touchPos = touch.position;
					touchInfoArray[touch.fingerId].swipeTime = Time.time;
				}
				
				//Check swipe variance
				if(touch.position.y > 
					(touchInfoArray[touch.fingerId].touchPos.y
					+ swipeVariance)){
					touchInfoArray[touch.fingerId].touchPos = touch.position;
				}
				
				if(touch.position.y < 
					(touchInfoArray[touch.fingerId].touchPos.y
					+ swipeVariance)){
					touchInfoArray[touch.fingerId].touchPos = touch.position;
				}
				
				//Swipe right
				if((touch.position.x > 
					touchInfoArray[touch.fingerId].touchPos.x
					+ swipeLength) && 
					!touchInfoArray[touch.fingerId].swipeComplete
					&& activeTouch == -1)
				{
					SwipeComplete("swipe right ", touch);
				}
				
				//Swipe left
				if((touch.position.x < 
					touchInfoArray[touch.fingerId].touchPos.x
					- swipeLength) && 
					!touchInfoArray[touch.fingerId].swipeComplete
					&& activeTouch == -1)
				{
					SwipeComplete("swipe left ", touch);
				}
				
				if(touch.fingerId == activeTouch && touch.phase ==
					TouchPhase.Ended)
				{
					//Debug.log("Ending " + touch.fingerId);
					
					foreach(Touch touchReset in Input.touches){
					   touchInfoArray[touch.fingerId].touchPos
							= touchReset.position;
					}
					touchInfoArray[touch.fingerId].swipeComplete = false;
					activeTouch = -1;
				}
			}
		}
	}
	
	void SwipeComplete(string msg, Touch touch){
		Reset(touch);
		
		if(timeToSwipe == 0.0f || (timeToSwipe > 0.0f &&
			(Time.time - 
			touchInfoArray[touch.fingerId].swipeTime)
			<= timeToSwipe))
		{
			msg = "Derp";
			//eh
		}
	}
	
	void Reset(Touch touch){
		activeTouch = touch.fingerId;
		touchInfoArray[touch.fingerId].swipeComplete = true;
	}
}

