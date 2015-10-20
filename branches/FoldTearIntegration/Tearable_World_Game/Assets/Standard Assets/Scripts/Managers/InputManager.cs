// <summary>
/// 
/// FILE: Input Manager
/// 
/// DESCRIPTION:
/// 	This class is used to handle the mobile device input to trigger events in game and in UI navigation
/// 	This class mainly only talks to other managers
/// 
/// AUTHORS: TearableWorld Team (Crumpled Up Game Engineers)
/// 			-> John Crocker, Justin Telmo, Tom Dubiner, ... (ADD YOUR NAME HERE!)
/// 
/// </summary>
using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class InputManager : MonoBehaviour
{
	#region Fields

	public enum PressState
    {
		NONE,
		UP,
		DOWN,
		JUST_PRESSED,
		JUST_RELEASED
	}

	/// <summary>
	/// Types of touch/mouse input the player can perform
	/// </summary>
	public enum InputType
    {
		// both keyboard and touch device input
		NONE,

		TAP,	// single touch on the screen at doesn't move from the start

		SWIPE,	// fast input down and up across the screen

		DRAG,	// player input is moving but constantly down on the screen 

		// solely tablet input
		MULTITOUCH_2,
		MULTITOUCH_3,
		MULTITOUCH_4,
		MULTITOUCH_5,
		MULTITOUCH_OTHER
	}

	public enum ScreenSide
    {
		LEFT,
		RIGHT,
		NONE
	}
	


	/// <summary>
	/// Time in milliseconds that triggers the player's idle movement if no input is recieved.
	/// </summary>
	const int IDLE_TRIGGER_LIMIT = 1000;
	
	/// <summary>
	/// Flag for whether or not the player is able to trigger the idle animation.
	/// </summary>
	bool canIdle = false;

	
	/// <summary>
	/// The touch controller that returns the type of touch input
	/// </summary>
	TouchController touchController;

    /// <summary>
    /// Tear Paper script
    /// </summary>
    public Demo_SingleTear tearScript;

	/// <summary>
	/// Actions keys for player input, touch controls for tablets will needed to be added to this
	/// </summary>
	KeyCode moveRight, moveLeft, jump;

	/// <summary>
	/// Watches that will trigger events for player movement and idleness
	/// </summary>
	Stopwatch keyDownWatch, idleWatch;

    /// <summary>
    /// Last time the player released and just pressed
    /// on screen
    /// </summary>
    Stopwatch lastReleaseWatch, lastJustPressedWatch;

    /// <summary>
    /// Couples with lastReleaseWatch to determine
    /// a double tap
    /// </summary>
    private int justReleasedCount = 0;

    /// <summary>
    /// Couples with release count
    /// to determine double tap
    /// </summary>
    private int justPressedCount = 0;

    /// <summary>
    /// Time between two taps that 
    /// signify a double tap for
    /// paper placement
    /// </summary>
    int DOUBLE_TAP_TIME_LIMIT = 250;

	/// <summary>
	/// Time limit in milliseconds that notifies the player just released mouse or finger input
	/// </summary>
	float JUST_RELEASED_INPUT_LIMIT = 100;

	/// <summary>
	/// bottom the maximum height the user can touch the screen to control the player
	/// </summary>
	float bottomTouchLimitPos;

	/// <summary>
	/// Horizontal limits that the bounds input to player movement
	/// </summary>
	float leftSideTouchLimitPos, rightSideTouchLimitPos;

	/// <summary>
	/// Animation manager reference object
	/// </summary>
	private AnimationManager animationManagerRef;

	private GameStateManager gameStateManagerRef;

	private ScreenManager screenManagerRef;

    private GameObject playerObject;

	/// <summary>
	/// Player's input type regardless of platform
	/// </summary>
	private InputManager.InputType currentPlayerInputType;

	/// <summary>
	/// Player's previous input type regardless of platform, helps us determine if the player just released
	/// </summary>
	private InputManager.InputType previousPlayerInputType;

	/// <summary>
	/// Returns if the input is pressed or released. Set dependent upon 'currentPlayerInputType'.
	/// </summary>
	private InputManager.PressState currentPlayerPressState;

    /// <summary>
    /// local resolution to check against screen manager
    /// </summary>
    private Vector2 currentScreenSize = new Vector2();
	
	private Vector2 currentResolution;
	#endregion

	#region Methods

	/// <summary>
	/// Press states to simulate just releases and presses
	/// </summary>
	private PressState currPressState = PressState.NONE;
	private PressState prevPressState = PressState.NONE;

	/// <summary>
	/// Wrapper function for touchControllers.ReturnTouchType that returns InputType instead of TouchType.
	///		Used only for ReturnPressStateFunction below and the actual finialized placement of torn object in the tear script.
	/// </summary>
	/// <returns></returns>
	public InputType ReturnInputType()
    {
		if(gameStateManagerRef.inGame)
        {
			if(gameStateManagerRef.OnMobileDevice())
            {
				if(touchController)
                {
					switch(touchController.ReturnTouchType()){
						case TouchType.DRAG:
							return InputType.DRAG;

						case TouchType.TAP:
							return InputType.TAP;

						case TouchType.SWIPE:
							return InputType.SWIPE;

						case TouchType.MULTITOUCH_2:
							return InputType.MULTITOUCH_2;

						case TouchType.MULTITOUCH_3:
							return InputType.MULTITOUCH_3;

						case TouchType.MULTITOUCH_4:
							return InputType.MULTITOUCH_4;

						case TouchType.MULTITOUCH_5:
							return InputType.MULTITOUCH_5;

						case TouchType.MULTITOUCH_OTHER:
							return InputType.MULTITOUCH_OTHER;

						default:
							return InputType.NONE;
					}
				}

				else
                {
					UnityEngine.Debug.LogError("TOUCH CONTROLLER IS NULL");
				}
			}
		}
		return InputType.NONE;
	}

	/// <summary>
	/// Returns the current touch input
	/// </summary>
	/// <returns></returns>
	public PressState GetcurrPressState()
    {
		return currPressState;
	}

	/// <summary>
	/// Returns the previous touch input
	/// </summary>
	/// <returns></returns>
	public PressState GetprevPressState()
    {
		return prevPressState;
	}

	/// <summary>
	/// Returns the ellapsed time the user has finger/mouse down
	/// </summary>
	/// <returns></returns>
	public long GetKeyDownTime()
    {
		return keyDownWatch.ElapsedMilliseconds;
	}

	/// <summary>
	/// Returns the ellapsed time the user has had no input applied to the game
	/// </summary>
	/// <returns></returns>
	public long GetIdleTime()
    {
		return idleWatch.ElapsedMilliseconds;
	}

    /// <summary>
    /// Double tap boolean 
    /// Three since the initial
    /// release of the cut is counted
    /// </summary>
    public bool DoubleTap()
    {
        return (justReleasedCount >= 2);
    }

    public bool SingleTap()
    {
        return (justPressedCount == 1);
    }


    /// <summary>
    /// Method to be called when releasing
    /// </summary>
    private void OnRelease()
    {
        if (tearScript != null)
        {
            if (tearScript.GetMovingPiece())
            {
                //UnityEngine.Debug.Log("TIMER COUNT " + lastReleaseWatch.ElapsedMilliseconds + "\nRELEASE COUNT " + justReleasedCount);

                if (lastReleaseWatch.ElapsedMilliseconds.Equals(0) || lastReleaseWatch.ElapsedMilliseconds > DOUBLE_TAP_TIME_LIMIT)
                {
                    lastReleaseWatch.Reset();
                    lastReleaseWatch.Start();
                    justReleasedCount = 1;
                }

                else if (lastReleaseWatch.ElapsedMilliseconds > 0 && lastReleaseWatch.ElapsedMilliseconds < DOUBLE_TAP_TIME_LIMIT)
                    justReleasedCount++;
            }

            if (tearScript.GetRotatingPiece())
            {
                tearScript.SetRotatingPiece(false);
            }


            if (PointInsideObject(playerObject, touchController.GetLastFingerPosition()))
            {
                playerSelected = !playerSelected;
            }
        }
    }

    /// <summary>
    /// Method to be called when just pressing
    /// </summary>
    private void OnJustPressed()
    {
        //UnityEngine.Debug.Log("RELEASE COUNT " + justReleasedCount);
        if (tearScript != null)
        {
            if (tearScript.GetMovingPiece())
            {
                if (lastJustPressedWatch.ElapsedMilliseconds.Equals(0) || lastJustPressedWatch.ElapsedMilliseconds > DOUBLE_TAP_TIME_LIMIT)
                {
                    lastJustPressedWatch.Reset();
                    lastJustPressedWatch.Start();
                    justPressedCount = 1;
                }

                else if (lastJustPressedWatch.ElapsedMilliseconds > 0 && lastJustPressedWatch.ElapsedMilliseconds < DOUBLE_TAP_TIME_LIMIT)
                    justPressedCount++;
            }
        }
    }

    Vector2 initDownPressPos = new Vector2();
    public void SetInitDownPressPos(Vector2 vec)
    {
        //UnityEngine.Debug.Log("CHANGE");
        initDownPressPos = vec;
    }

    public Vector2 GetInitDownPressPos()
    {
        
        return initDownPressPos;
    }

    /// <summary>
    /// Method to be called when
    /// mouse or single finger is down.
    /// Ensure the previous mouse state
    /// is just pressed to ensure the initial
    /// input position is not calclulated more
    /// than once
    /// </summary>
    private void OnDown()
    {
        initDownPressPos = touchController.GetLastFingerPosition();
    }

    Vector2 lastPressPos = new Vector2();

	/// <summary>
	/// Returns pressed states of player input (Up, down, just hit down, just hit up, none)
	/// Works for both PC and Android.
	/// SHOULD ONLY BE CALLED ONCE IN UPDATE
	/// </summary>
	/// <returns></returns>
	private PressState ReturnPressState()
    {
		//UnityEngine.Debug.Log("idle watch " + idleWatch.ElapsedMilliseconds + "\nkey watch " + keyDownWatch.ElapsedMilliseconds);
		if(gameStateManagerRef.inGame)
        {
			// get a temp state so we don't constantly call the function
			InputType tempPressState = ReturnInputType();

            // if the mouse immediate down flag is raised on the pc
			if(Input.GetMouseButtonDown(0) ||

                // or if the mouse is down on the pc and not using unity remote or tablet
                // or on unity remote or tablet 
                (((Input.GetMouseButton(0) && !gameStateManagerRef.OnMobileDevice()) ||
                tempPressState.Equals(InputType.TAP) ||
				tempPressState.Equals(InputType.DRAG) ||
				tempPressState.Equals(InputType.SWIPE)) &&

                // and has not been triggered for max length of time
				keyDownWatch.ElapsedMilliseconds > 0 &&
				keyDownWatch.ElapsedMilliseconds <= JUST_RELEASED_INPUT_LIMIT))
            {

                //UnityEngine.Debug.Log("JUST PRESSED");
				return PressState.JUST_PRESSED;
			}

			else if((Input.GetMouseButton(0) ||
					tempPressState.Equals(InputType.TAP) ||
					tempPressState.Equals(InputType.DRAG)) &&
					keyDownWatch.ElapsedMilliseconds > JUST_RELEASED_INPUT_LIMIT &&
					idleWatch.ElapsedMilliseconds <= 0)
            {
                //UnityEngine.Debug.Log("DOWN");
                lastPressPos = touchController.GetLastFingerPosition();
				return PressState.DOWN;
			}

			else if(Input.GetMouseButtonUp(0) ||
					(tempPressState.Equals(InputType.NONE) && 
					idleWatch.ElapsedMilliseconds > 0 && 
					idleWatch.ElapsedMilliseconds <= JUST_RELEASED_INPUT_LIMIT && 
                    gameStateManagerRef.OnMobileDevice()))
            {
                if (prevPressState.Equals(PressState.JUST_PRESSED) || prevPressState.Equals(PressState.DOWN))
                    OnRelease();

                //UnityEngine.Debug.Log("JUST RELEASED");
				return PressState.JUST_RELEASED;
			}

			else if(!Input.GetMouseButton(0) ||
					(gameStateManagerRef.OnMobileDevice() &&
					tempPressState.Equals(InputType.NONE) && 
					idleWatch.ElapsedMilliseconds > JUST_RELEASED_INPUT_LIMIT))
            {
				//UnityEngine.Debug.Log("UP idle watch " + idleWatch.ElapsedMilliseconds);
                //UnityEngine.Debug.Log("UP");
				return PressState.UP;
			}
			
		}
		return PressState.NONE;
	}
	/// <summary>
	/// Use this for initialization
	/// </summary>
	public void Start()
    {

        gameObject.AddComponent<TouchController>();
        screenManagerRef = gameObject.GetComponent<ScreenManager>();
        gameStateManagerRef = gameObject.GetComponent<GameStateManager>();
        screenManagerRef = gameObject.GetComponent<ScreenManager>();
        animationManagerRef = gameObject.GetComponent<AnimationManager>();
        touchController = gameObject.GetComponent<TouchController>();
       

		keyDownWatch = new Stopwatch();
		idleWatch = new Stopwatch();
        lastReleaseWatch = new Stopwatch();
        lastJustPressedWatch = new Stopwatch();
		moveRight = KeyCode.D;
		moveLeft = KeyCode.A;
		jump = KeyCode.Space;

	}

	/// <summary>
	/// Resets the positions of variables that are screen dependent
    /// (which is any GUI element)
	/// </summary>
	public void ResetResolutionDependents()
    {
		leftSideTouchLimitPos = (screenManagerRef.GetScreenSize().x / 4);
        rightSideTouchLimitPos = (3 * screenManagerRef.GetScreenSize().x / 4);
	}

    /// <summary>
    /// Method that determines if a point is within
    /// the bounds of a game object
    /// </summary>
    /// <param name="meshObject"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    private bool PointInsideObject(GameObject gameObject, Vector2 point)
    {
        //UnityEngine.Debug.Log("FINGER POS " + touchController.GetLastFingerPosition().ToString());

        return (gameObject.collider.bounds.Contains(new Vector3(Camera.mainCamera.ScreenToWorldPoint(point).x,
                                                                     Camera.mainCamera.ScreenToWorldPoint(point).y,
                                                                     gameObject.collider.bounds.center.z)));
    }



    private bool playerSelected = false;

    public bool PlayerSelected()
    {
        return playerSelected;
    }

    

	/// <summary>
	/// Update is called once per frame
	/// </summary>
	public void Update()
    {
        //UnityEngine.Debug.Log("SIZE " + Screen.width.ToString() + ", " + Screen.height.ToString());

        if (gameStateManagerRef.inGame)
        {
            // saves a boolean for init update
            // when the game first starts
            if (bottomTouchLimitPos == 0)
            {
                // Here, we need to use Screen height, width instead of Screen Resolution
                // for whatever reason, FLAG
                currentScreenSize = screenManagerRef.GetScreenSize();
                bottomTouchLimitPos = screenManagerRef.GetScreenSize().x - (2 * screenManagerRef.GetScreenSize().x / 3);
                leftSideTouchLimitPos = (screenManagerRef.GetScreenSize().x / 4);
                rightSideTouchLimitPos = (3 * screenManagerRef.GetScreenSize().x / 4);

                //UnityEngine.Debug.Log("RIGHT " + rightSideTouchLimitPos);
                //UnityEngine.Debug.Log("BOTTOM " + bottomTouchLimitPos);
                //UnityEngine.Debug.Log("RES " + Screen.width + ", " + Screen.height);
            }

            if (GameObject.FindGameObjectWithTag("TearScript"))
            {
                tearScript = GameObject.FindGameObjectWithTag("TearScript").GetComponent<Demo_SingleTear>();
            }

            if (GameObject.FindGameObjectWithTag("Player") != null)
            {
                playerObject = GameObject.FindGameObjectWithTag("Player");
            }
        }


        AnimationManager.AnimationDirection currDirection = PlayerScreenSideTouched();
		
        if (currentScreenSize != screenManagerRef.GetScreenSize())
        {
            currentScreenSize = screenManagerRef.GetScreenSize();
			ResetResolutionDependents();
		}
		

		if(Input.GetKey(KeyCode.Escape) || touchController.ReturnTouchType().Equals(TouchType.MULTITOUCH_3))
        {
			if(gameStateManagerRef.inGame)
            {
				gameStateManagerRef.PauseGame();
			}
		}

		if(gameStateManagerRef.inGame)
        {
			// this is the only place where the function ReturnPressState
			//	should be called so we can have consistent prev and curr states
			prevPressState = currPressState;
			currPressState = ReturnPressState();
			
			

			if(currDirection != AnimationManager.AnimationDirection.NONE)
            {
				//UnityEngine.Debug.Log (currSideTouched);
				if(!animationManagerRef.GetAnimationDirection().Equals (currDirection))
				{
					animationManagerRef.SetDirection (currDirection);
				}
			}
			
			// starts the watch if any of the actions keys are pressed
			if(PlayerMoveHorizontalTriggered(ScreenSide.LEFT) || 
			   PlayerMoveHorizontalTriggered(ScreenSide.RIGHT) ||
			   currPressState.Equals(PressState.DOWN) ||
               currPressState.Equals(PressState.JUST_PRESSED)
					|| PlayerVerticalMovementTriggered())
			{
				if(keyDownWatch.ElapsedMilliseconds.Equals(0))
                {
					keyDownWatch.Start();
				}
				if (!idleWatch.ElapsedMilliseconds.Equals(0))
                {
					idleWatch.Reset();
					idleWatch.Stop();
				}
			}
			
			// else reset the watch and start idle timer
			else
            {
				if(!keyDownWatch.ElapsedMilliseconds.Equals(0))
                {
					keyDownWatch.Reset();
					keyDownWatch.Stop();
				}
				if(idleWatch.ElapsedMilliseconds.Equals(0))
                {
					idleWatch.Start();
				}
			}
				
			if(idleWatch.ElapsedMilliseconds > IDLE_TRIGGER_LIMIT)
            {
				if(!animationManagerRef.GetCurrentAnimationType().Equals(AnimationManager.AnimationType.IDLE))
                {
					//UnityEngine.Debug.Log("I'M GETTING HIT 2");
					animationManagerRef.TriggerAnimation(AnimationManager.AnimationType.IDLE);		
				}
			}


            else if (PlayerVerticalMovementTriggered())
            {
                // trigger jump movement in animation manager
                if (!animationManagerRef.GetCurrentAnimationType().Equals(AnimationManager.AnimationType.JUMP))
                    animationManagerRef.TriggerAnimation(AnimationManager.AnimationType.JUMP);
            }
			
			else if(PlayerMoveHorizontalTriggered(GetCurrScreenSide ()))
			{
				if(!animationManagerRef.GetCurrentAnimationType().Equals(AnimationManager.AnimationType.WALK))
                {
					animationManagerRef.TriggerAnimation(AnimationManager.AnimationType.WALK);
				}
			}

			
			//Should trigger for case where player holds both left and right down.
			else if(PlayerMoveHorizontalTriggered(ScreenSide.LEFT) && PlayerMoveHorizontalTriggered(ScreenSide.RIGHT))
            {
				animationManagerRef.SetDirection (AnimationManager.AnimationDirection.RIGHT);
				animationManagerRef.TriggerAnimation (AnimationManager.AnimationType.STAND);
			}
			
			
			else
            {
				//UnityEngine.Debug.Log ("Yell at me~!");
				animationManagerRef.TriggerAnimation (AnimationManager.AnimationType.STAND);
			}
		}
	}
	
	
	/// <summary>
	/// Checks which side of the screen was touched. Used to abstractly determine animation direction.
	/// </summary>
	/// <returns>
	/// The screen side touched.
	/// </returns>
	public AnimationManager.AnimationDirection PlayerScreenSideTouched()
    {
		if(PlayerMoveHorizontalTriggered(ScreenSide.LEFT))
        { 
            return AnimationManager.AnimationDirection.LEFT; 
        }

		else if(PlayerMoveHorizontalTriggered(ScreenSide.RIGHT))
        { 
            return AnimationManager.AnimationDirection.RIGHT; 
        }
		else
        { 
            return AnimationManager.AnimationDirection.NONE; 
        }
	}
	
	public ScreenSide GetCurrScreenSide()
    {
		if(PlayerMoveHorizontalTriggered(ScreenSide.LEFT))
        { 
            return ScreenSide.LEFT; 
        }

		else if(PlayerMoveHorizontalTriggered(ScreenSide.RIGHT))
        { 
            return ScreenSide.RIGHT; 
        }
		else
        { 
            return ScreenSide.NONE; 
        }
	}
	
	/// <summary>
	/// returns true if the player is attempting to jump
	/// </summary>
	/// <returns></returns>
	public bool PlayerVerticalMovementTriggered()
    {
        if ((touchController.ReturnTouchType() == TouchType.SWIPE || Input.GetKey(jump)))
        {
            //UnityEngine.Debug.Log("SWIPE");
            if (tearScript != null)
            {
                if (!tearScript.GetMovingPiece() && !tearScript.GetRotatingPiece())
                {
                    return true;
                }

                else
                {
                    return false;
                }
            }

            else
            {
                return true;
            }
        }

        return false;
	}

	/// <summary>
	/// Returns -1, 0, 1 depending on left, no input, right screen touch for moving the player horizontally
	/// </summary>
	/// <returns></returns>
	public int GetHorizontalTouchMovement()
    {
		//if(touchController.GetFingerPositions().Count > 0){
			// did the player actually trigger a keyboard input
			if(currPressState.Equals(PressState.JUST_PRESSED) ||
               currPressState.Equals(PressState.DOWN))
            {
				// and did the player hit the viable horizontal section of the screen
                if (touchController.GetLastFingerPosition().y <= bottomTouchLimitPos &&
                        touchController.GetLastFingerPosition().x <= leftSideTouchLimitPos &&
                        touchController.GetLastFingerPosition().x <= rightSideTouchLimitPos)
                {
                    return -1;
                }

                else
                {
                    //UnityEngine.Debug.Log("FINGER " + touchController.GetLastFingerPosition().ToString());
                    //UnityEngine.Debug.Log("RIGHT " + rightSideTouchLimitPos);
                    //UnityEngine.Debug.Log("BOTTOM " + bottomTouchLimitPos);
                }
				
				if(touchController.GetLastFingerPosition().y <= bottomTouchLimitPos &&
						touchController.GetLastFingerPosition().x >= rightSideTouchLimitPos)
                {
					return 1;
				}
			}
			//else{
			//	UnityEngine.Debug.Log("NOT TOUCH OR DRAG " + touchController.ReturnTouchType().ToString());
			//}
		//}
		return 0;
	}

	/// <summary>
	/// Returns true if the player is attempting to move the player left or right on screen
	/// </summary>
	/// <param name="side"></param>
	/// <returns></returns>
	private bool PlayerMoveHorizontalTriggered(ScreenSide side)
    {
        if (gameStateManagerRef.inGame)
        {
            if (tearScript != null)
            {
                if (tearScript.GetMovingPiece() || tearScript.GetRotatingPiece())
                {
                    return false;
                }
            }
        }

		if((Input.GetKey(moveLeft) && side.Equals(ScreenSide.LEFT)) ||
		   (Input.GetKey(moveRight) && side.Equals(ScreenSide.RIGHT)))
        {
            
			return true;
		}

		else
        {
			if(touchController.GetLastFingerPosition().Equals(TouchController.nullVector))
            {
				return false;
			}

			else
            {

                if
                    // did the player actually trigger a keyboard input
                ((touchController.ReturnTouchType() == TouchType.TAP ||
                    touchController.ReturnTouchType() == TouchType.DRAG) &&

                    // and did the player hit the viable horizontal section of the screen
                    ((side.Equals(ScreenSide.LEFT) &&
                    touchController.GetLastFingerPosition().x <= leftSideTouchLimitPos) ||
                    (side.Equals(ScreenSide.RIGHT) &&
                    touchController.GetLastFingerPosition().x >= rightSideTouchLimitPos)) &&

                    // and did the player hit the viable vertical section of the screen
                    touchController.GetLastFingerPosition().y <= bottomTouchLimitPos)
                {
                    //UnityEngine.Debug.Log("MOVING " + side.ToString());
                    return true;
                }

                return false;
			}
		}
	}

	/// <summary>
	/// Gets the current screen being displayed
	/// </summary>
	public void GetScreen()
    {
		//TODO	
	}
	#endregion
}
