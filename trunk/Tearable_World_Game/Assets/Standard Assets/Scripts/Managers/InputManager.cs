/* 
	FILE: Input Manager
	
	DESCRIPTION:
		This class is used to handle the mobile device input to trigger events in game and in UI navigation.
		This class mainly only talks to other managers.
	
	AUTHORS: TearableWorld Team (Crumpled Up Game Engineers)
		-> Dominic Arcamone, Justin Telmo, Tom Dubiner ... (ADD YOUR NAME HERE!)
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class InputManager : MonoBehaviour{
	#region Fields
	public enum PressState{
		NONE,
		UP,
		DOWN,
		JUST_PRESSED,
		JUST_RELEASED
	}

	// Types of touch/mouse input the player can perform.
	public enum InputType{
		// Both keyboard and touch device input.
		NONE,
		// Single touch on the screen at doesn't move from the start.
		TAP,
		// Fast input down and up across the screen.
		SWIPE,
		// Player input is moving but constantly down on the screen.
		DRAG,

		// Solely tablet input.
		MULTITOUCH_2,
		MULTITOUCH_3,
		MULTITOUCH_4,
		MULTITOUCH_5,
		MULTITOUCH_OTHER
	}

	public enum ScreenSide{
		LEFT,
		RIGHT,
		NONE
	}

	public enum FingerGesture{
		MOVE,
		TEAR,
		FOLD,
		TOGGLETORNPIECE,
		NONE
	}
	//These were added to create toggles for the various mechanics.
	/// <summary>
	/// When true the player can control the movements of the player
	/// </summary>
	public bool moveMode;
	/// <summary>
	/// When true the player can use the tear mechanic.
	/// </summary>
	public bool tearMode;
	/// <summary>
	/// When true the player can use the fold mechanic.
	/// </summary>
	public bool foldMode;
	
	
	public bool 						pickUpTear = false;
	public Vector3						initPlayerNonGroundedPos = new Vector3();
	public float						initPlayerNonGroundedZRot = 0;

	// Time in milliseconds that triggers the player's idle movement if no input is recieved.
	int									idleTriggerLimit = 4000;
	
	// Flag for whether or not the player is able to trigger the idle animation.
	public bool							currentlyIdling = false,
	// Boolean to check for if the player is dead.
										isDead = false,
	// Checks for whether or not the player is colliding horizontally.
										hasHorizontalCollision = true,
	// Pair of booleans determining if the player is sliding up or down. Should have a different animation depending on
	//	which is being triggered. Should be impossible to set both at the same time.
										isSlidingUp = false, isSlidingDown = false;

	GameObject							paperObject, foldBorder,
										unfoldBorder, unfoldBlocker, moveBorder, tearBorder,
										tornPiece,
										menuButton, restartButton;
	
	UnfoldCollision						unfoldCollisionRef;

	private bool						allLoaded = false;
	
	// The touch controller that returns the type of touch input.
	TouchController						touchController;

	// Tear Paper script.
	public TearManager					tearManagerRef;
	// Fold paper script.
	public Fold							foldRef;
	
	// Actions keys for player input, touch controls for tablets will needed to be added to this.
	KeyCode								wasdRight, wasdLeft,
										arrowLeft, arrowRight,
										keyJump;

	// List for all watches in order to limit time being ellapsed or global pause.
	List<Stopwatch>						watchList = new List<Stopwatch>();

	// Watches that will trigger events for player movement and idleness.
	Stopwatch							keyDownWatch,
										playerBottomCollisionWatch,
	// Last time the player released and just pressed on screen.
										releaseWatch, justPressedWatch;
	
	// Stopwatch that checks for when the player is idling.
	public Stopwatch					idleKeyWatch,
	// Watch that determines how long the player has been idle.
										idlePlayerWatch;
	
	// Couples with lastReleaseWatch to determine a double tap.
	private int							releasedCount = 0,
	// Couples with release count to determine double tap.
										justPressedCount = 0;
	
	// Time between two taps that signify a double tap for paper placement
	int									DOUBLE_TAP_TIME_LIMIT = 500,
	// Time in ms between just pressed and double tap limit.
										CARRY_OVER_LIMIT = 150,
	// Time in milliseconds that no two inputs that are the same will be accepted.
										TAP_INPUT_BUFFER = 25,
	// Ellapsed time before watches pause.
										ELAPSED_WATCH_LIMIT = 10000,
	// Time in ms that stand animation won't be played after no input is supplied. Allows walk to cycle frames
	//	when player is just tapping movement for player.
										INPUT_BUFFER_LIMIT = 250,
	// Amount in pixels needed to perform a correct swipe.
										MIN_SWIPE_DIST = 50;
	
	// Time limit in milliseconds that notifies the player just released mouse or finger input.
	float								JUST_HIT_INPUT_LIMIT = 100;
	
	// Bottom the maximum height the user can touch the screen to control the player.
	Vector2								bottomTouchLimitPos,
	// Horizontal limits that the bounds input to player movement.
										leftSideTouchLimitPos, rightSideTouchLimitPos;
	
	// Animation manager reference object.
	private AnimationManager			animManagerRef;
	// The game state manager reference.
	private GameStateManager			gameStateManagerRef;
	// The screen manager reference.
	private ScreenManager				screenManagerRef;
	// The sound manager reference.
	private SoundManager				soundManagerRef;
	// The world collision reference.
	private WorldCollision				worldCollisionRef;
	
	// The player object.
	private GameObject					playerObject;
	
	// The controller reference.
	private TWCharacterController		controllerRef;

	// Player's input type regardless of platform.
	private InputManager.InputType		currentPlayerInputType;
	// Player's previous input type regardless of platform, helps us determine if the player just released.
	private InputManager.InputType		previousPlayerInputType;
	// Returns if the input is pressed or released. Set dependent upon 'currentPlayerInputType'.
	private InputManager.PressState		currentPlayerPressState;
	
	// Local resolution to check against screen manager.
	private Vector2						currentScreenSize = new Vector2(),
	// The current resolution.
										currentResolution;
	
	bool								movingTornPiece, tornPieceInitiated;
	private bool						jumpInitiated = false;
	ScreenSide							currentDirection;
	
	// Press states to simulate just releases and presses.
	private PressState					currPressState = PressState.NONE,
										prevPressState = PressState.NONE;

	public bool PlayerJumping(){ return jumpInitiated; }
	public bool PlayerIdling(){ return currentlyIdling; }

	public void SetPlayerIdling(bool value){ currentlyIdling = value; }

	FingerGesture						fingerGesture = FingerGesture.NONE;
	public FingerGesture GetFingerGesture(){ return fingerGesture; }

	bool								inputInit = false;

	int									JUST_PRESSED_LIMIT = 100;

	Stopwatch							swipeWatch = new Stopwatch();
	int									SWIPE_LIMIT = 400;
	Vector2								justPressedFingerPos = new Vector2(-1000, -1000),
										justReleasedFingerPos = new Vector2(),
										lastDownFingerPos = new Vector2();

	public Stopwatch					foldGestureWatch = new Stopwatch();
	Stopwatch							lastValidInputWatch = new Stopwatch(),
										bufferWatch = new Stopwatch();
	
	public AnimationManager.AnimationDirection
										currDirection = AnimationManager.AnimationDirection.RIGHT;
	int									MAX_FINGER_SEPERATION_DIST = 5,
										SWIPE_ANGLE_RESET_VALUE = -1;
	public float						fingerSwipeAngle = -1;
	bool								pressSet = false;
	
	// If a finger is within this pixel distance, the player will neither move or jump.
	float								PLAYER_VOID_MOVEMENT_DIST = 200;
	/// <summary>
	/// This is the deltaPosition of a touch divided by deltaTime. 
	/// So it is pos/time.
	/// </summary>
	float								DELTA_SWIPE_DIFF = 300f;
	// Watch that ellapses when a finger has been in one spot for an extended amount of time.
	Stopwatch							samePosWatch = new Stopwatch(),
	// Watch that samples every so often for the current finger position. It's inefficient to be checking current finger pos
	//	against a potential idle finger every tick.
										samplePosWatch = new Stopwatch();

	// Time in ms that samplePosWatch will relapse and re-assign (sample) idle finger pos.
	int									SAME_POS_SAMPLE_LIMIT = 100,
	// Time in ms that justPressedFingerPos has to be in relatively the same spot before being reset to the current finger pos.
	//	Allows for swipe to occur when holding finger down for an extended amount of time.
										SAME_POS_TIME_LIMIT = 350,
	// The limit in pixels a finger is considered not to be in the same position before resetting justPressedFingerPos.
										SAME_POS_DIST_VARIANCE = 20;
	
	Vector2								fingerIdlePos = TouchController.NULL_VECTOR;

	bool								releaseSet = false,
										swipeInputDetected = false;

	float								timeLastInputHeld = 0;

	ScreenSide							lastSidePressed = ScreenSide.NONE,
										justPressedSide = ScreenSide.NONE,
										justReleasedSide = ScreenSide.NONE,

										lastKeyboardSide = ScreenSide.RIGHT;
	#endregion

	#region Methods
	// Wrapper function for touchControllers.ReturnTouchType that returns InputType instead of TouchType.
	//	Used only for ReturnPressStateFunction below and the actual finialized placement of torn object in the tear script.
	public InputType ReturnInputType(){
		if(gameStateManagerRef.inGame){
			if(gameStateManagerRef.OnMobileDevice()){
				if(touchController){
					/***** DETERMINES TOUCH TYPE *****/
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

				/*else{
					UnityEngine.Debug.LogError("TOUCH CONTROLLER IS NULL");
				}*/
			}
		}
		return InputType.NONE;
	}
	
	// Returns the current touch input.
	public PressState GetcurrPressState(){ return currPressState; }
	// Returns the previous touch input.
	public PressState GetprevPressState(){ return prevPressState; }
	// Returns the ellapsed time the user has finger/mouse down.
	public long GetKeyDownTime(){ return keyDownWatch.ElapsedMilliseconds; }
	// Returns the ellapsed time the user has had no input applied to the game.
	public long GetIdleTime(){ return idleKeyWatch.ElapsedMilliseconds; }
	
	// Double tap boolean. Three since the initial release of the cut is counted.
	public bool DoubleTap(){
		if(fingerGesture == FingerGesture.TOGGLETORNPIECE){
			fingerGesture = FingerGesture.NONE;
			return true;
		}
		else{
			return false;
		}
		/*return (releasedCount >= 2 && 
				releaseWatch.ElapsedMilliseconds <= DOUBLE_TAP_TIME_LIMIT);*/
	}
	public bool SingleTap(){
		return (justPressedCount == 1);
	}
	
	// Checks if player tearing.
	public bool CheckIfPlayerTearing(){
		if(fingerGesture == FingerGesture.TEAR){ return true; }
		return false;
	}
	
	// Method to be called when releasing needed specifically for tearing.
	private void DoubleTapRelease(){
		if(tearManagerRef != null){
			if(releaseWatch.ElapsedMilliseconds <= DOUBLE_TAP_TIME_LIMIT){
				releaseWatch.Reset();
				releaseWatch.Start();

				if(releasedCount < 2){
					releasedCount++;
				}
			}
			
			if(tearManagerRef.GetRotatingPiece()){
				UnityEngine.Debug.Log("tearManagerRef.GetRotatingPiece");
				tearManagerRef.SetRotatingPiece(false);
			}

			//UnityEngine.Debug.Log("RELEASE COUNT " + releasedCount);
			// + "\nRELEASE COUNT " + justReleasedCount);
		}
		else{
			UnityEngine.Debug.Log("TEAR MANAGER IS NULL");
		}
	}

	// Method to be called when just pressing.
	private void OnJustPressed(InputType tempPressState){
		inputInit = true;

		OnInitialInput(tempPressState);

		if(justPressedWatch.ElapsedMilliseconds.Equals(0)){
			justPressedWatch.Start();
		}

		// Init input accepted so start buffer timers in order for multiples of the same feedback from being read.
		bufferWatch.Reset();
		bufferWatch.Stop();

		// Reset the timer that detects how long no valid input has been detected. This is different than idle watch
		//	since idle looks for any input period
		lastValidInputWatch.Reset();
		lastValidInputWatch.Stop();
	}

	// Logic needed when release input is detected.
	private void OnRelease(){
		//UnityEngine.Debug.Log("BUFFER " + bufferWatch.ElapsedMilliseconds);
		//UnityEngine.Debug.Log("PREV " + prevPressState);

		/*
			With configuring our own 'just pressed' logic, the player can skip press states
				ie Normal Press State Flow):
				JUST_PRESSED -> DOWN -> JUST_RELEASED -> UP
				instead, if released fast enough you can have this instead:
				JUST_PRESSED -> JUST_RELEASED -> UP
				skipping the down state completely.
				
				so with the above said, a previous press state for just release could either be
				just pressed or the expected down
				
				so if just pressed or down &&
				bufferWatch has started and not in the buffer zone 
				accept new input
		*/
		if(prevPressState.Equals(PressState.JUST_PRESSED) || prevPressState.Equals(PressState.UP) &&
				(bufferWatch.ElapsedMilliseconds == 0 || bufferWatch.ElapsedMilliseconds > TAP_INPUT_BUFFER)){
			// Start buffer watch if it has not already started.
			if(bufferWatch.ElapsedMilliseconds == 0){
				bufferWatch.Start();
			}
			// If beyond buffer threshold, restart watch.
			else if (bufferWatch.ElapsedMilliseconds > TAP_INPUT_BUFFER){
				bufferWatch.Reset();
				bufferWatch.Start();
			}

			inputInit = false;

			// Trigger the OnRelease function used right now for double taps.
			DoubleTapRelease();
		}
		
		// Stop the swipe watch from to determine amount of time the player pressed and released the touch device.  
		//	Used primarily for detecting jump input.
		swipeWatch.Stop();

		justPressedWatch.Reset();
		justPressedWatch.Stop();
	   
		if(!foldRef.currentlyFolding &&
				!tearManagerRef.GetMovingPiece() &&
				fingerGesture != FingerGesture.NONE){
			//UnityEngine.Debug.Log("GESTURE IS NONE");
			fingerGesture = FingerGesture.NONE;
		}
	}

	private void DetermineInitGesture(){
		if(gameStateManagerRef.OnMobileDevice() && 
			fingerGesture == FingerGesture.NONE){
			if(touchController.GetLastFingerPosition() != TouchController.NULL_VECTOR){
				if(((!foldRef.isFolded && worldCollisionRef.PointInsideObject(foldBorder, touchController.GetLastFingerPosition())
						&& !worldCollisionRef.PointInsideObject(moveBorder, touchController.GetLastFingerPosition())) 
						|| (foldRef.isFolded && worldCollisionRef.PointInsideObject( unfoldBorder, touchController.GetLastFingerPosition())
						&& !worldCollisionRef.PointInsideObject( unfoldBlocker, touchController.GetLastFingerPosition())))
						&& !(unfoldCollisionRef.getOverPlayer() && !foldRef.currentlyFolding) && foldMode){
					//UnityEngine.Debug.Log("GESTURE IS FOLD");
					fingerGesture = FingerGesture.FOLD;
				}
				else if(tearManagerRef.HaveTornOnce && worldCollisionRef.PointInsideObject(tornPiece, touchController.GetLastFingerPosition())
						&& !movingTornPiece && tearMode){
					movingTornPiece = true;
					fingerGesture = FingerGesture.TOGGLETORNPIECE;
				}
				else if(
					
						//This selection of code was commented out for controls by Douglas Weller
					/*worldCollisionRef.PointInsideObject(moveBorder, touchController.GetLastFingerPosition())
						|| worldCollisionRef.PointInsideObject(unfoldBlocker, touchController.GetLastFingerPosition())
						|| (foldRef.isFolded && !worldCollisionRef.PointInsideObject( unfoldBorder, touchController.GetLastFingerPosition())
						&& worldCollisionRef.PointInsideObject(foldBorder, touchController.GetLastFingerPosition()))
						|| (unfoldCollisionRef.getOverPlayer() && !foldRef.currentlyFolding
						&& (worldCollisionRef.PointInsideObject(foldBorder, touchController.GetLastFingerPosition())
						|| worldCollisionRef.PointInsideObject( unfoldBorder, touchController.GetLastFingerPosition())))*/
					
					
					moveMode){
					//UnityEngine.Debug.Log("GESTURE IS MOVE");
					fingerGesture = FingerGesture.MOVE;
				}
				else if(worldCollisionRef.PointInsideObject(tearBorder, touchController.GetLastFingerPosition())
						&& !worldCollisionRef.PointInsideObject(menuButton, touchController.GetLastFingerPosition())
						&& !worldCollisionRef.PointInsideObject(restartButton, touchController.GetLastFingerPosition()) &&
						tearMode){
					//UnityEngine.Debug.Log("GESTURE IS TEAR");
					fingerGesture = FingerGesture.TEAR;
					//UnityEngine.Debug.Log("tearmode");
				}
			}  
		}
	}
	// Method to be called when mouse or single finger is down. Ensure the previous mouse state is just pressed
	//	to ensure the initial input position is not calclulated more than once.
	private void OnDown(){}

	// Logic for post just released
	private void OnUp(){
		// if lastValidInputWatch has not started, do so
		if(lastValidInputWatch.ElapsedMilliseconds == 0){
			lastValidInputWatch.Start();
		}

		// Read description of CARRY_OVER_LIMIT, needed for double tap.  Buffer time between two taps is larger
		//	than buffer time between just released and up so we need carry over.
		if(lastValidInputWatch.ElapsedMilliseconds > 0 && lastValidInputWatch.ElapsedMilliseconds < CARRY_OVER_LIMIT){
			// Continue calling OnRelease for full double tap threshold time limit.
			if(prevPressState.Equals(PressState.JUST_PRESSED) || prevPressState.Equals(PressState.DOWN)){
				DoubleTapRelease();
			}
		}

		// Reset finger touch positions
	   
		//justPressedFingerPos = TouchController.NULL_VECTOR;
		//justReleasedFingerPos = TouchController.NULL_VECTOR;

		// Reset initFingerGesture to be nothing 
		//fingerGesture = FingerGesture.NONE;
	}
	
	// Function that's called at the top of ReturnPressState to interpret initial press.
	private void OnInitialInput(InputType tempPressState){
		if(fingerGesture == FingerGesture.NONE){
			DetermineInitGesture();
		}
		if(swipeInputDetected){
			swipeWatch.Reset();
			swipeWatch.Start();
		}
	}
	
	/*
		Returns pressed states of player input (Up, down, just hit down, just hit up, none).
			Works for both PC and Android.
			With configuring our own 'just pressed' logic, the player can skip press states
			ie Normal Press State Flow):
			JUST_PRESSED -> DOWN -> JUST_RELEASED -> UP
			instead if released fast enough you can have this instead:
			JUST_PRESSED -> JUST_RELEASED -> UP
			skipping the down state completely.
			SHOULD ONLY BE CALLED ONCE IN UPDATE
	*/
	private PressState ReturnPressState(){
		if(gameStateManagerRef.inGame){
			// Get a temp state so we don't constantly call the function.
			InputType tempPressState = ReturnInputType();

			//UnityEngine.Debug.Log("INPUT TYPE " + tempPressState);

			// Looking for fold finger gesture
			if(!tempPressState.Equals(InputType.NONE)){
				OnInitialInput(tempPressState);
			}

			// If the mouse immediate down flag is raised on the pc.
			if(Input.GetMouseButtonDown(0) ||
					// Or if the mouse is down on the pc and not using unity remote or tablet.
					(((Input.GetMouseButton(0) && !gameStateManagerRef.OnMobileDevice()) ||
					tempPressState.Equals(InputType.TAP) ||
					tempPressState.Equals(InputType.DRAG) ||
					tempPressState.Equals(InputType.SWIPE)) &&
					// And has not been triggered for max length of time.
					keyDownWatch.ElapsedMilliseconds > 0 &&
					keyDownWatch.ElapsedMilliseconds <= JUST_HIT_INPUT_LIMIT)){
				
				// Setup variables needed for gesture interpretations
				OnJustPressed(tempPressState);

				return PressState.JUST_PRESSED;
			}

			// If left mouse button is down OR touch device input is equal to a tap or drag and we're past the just pressed
			//	threshold return a DOWN pressState and constantly set lastDownFingerPos to use for valid wipe distance.
			else if((Input.GetMouseButton(0) ||
					tempPressState.Equals(InputType.TAP) ||
					tempPressState.Equals(InputType.DRAG)) &&
					keyDownWatch.ElapsedMilliseconds > JUST_HIT_INPUT_LIMIT &&
					idleKeyWatch.ElapsedMilliseconds <= 0){
				// setup variables needed for gesture interpretation
				OnDown();

				return PressState.DOWN;
			}

			// If the left mouse button was just released OR no input is being supplied to the tablet within just hit threshold
			//	return JUST RELEASED state.
			else if(Input.GetMouseButtonUp(0) ||
					(tempPressState.Equals(InputType.NONE) && 
					idleKeyWatch.ElapsedMilliseconds > 0 && 
					idleKeyWatch.ElapsedMilliseconds <= JUST_HIT_INPUT_LIMIT && 
					inputInit &&
					gameStateManagerRef.OnMobileDevice())){
				// setup varaibles needed for gesture interpretation
				//if(movingTornPiece)
				OnRelease();
//				UnityEngine.Debug.Log("PressState.JUST_RELEASED");
				return PressState.JUST_RELEASED;
			}

			// If  no left mouse input is detecting (used for paper translation) OR if on tablet device and no input is detected AND
			//	we've been idling for more than our threshold limit return UP pressState.
			else if(!Input.GetMouseButton(0) ||
					(gameStateManagerRef.OnMobileDevice() &&
					tempPressState.Equals(InputType.NONE) && 
					idleKeyWatch.ElapsedMilliseconds > JUST_HIT_INPUT_LIMIT)){
				// setup variables needed for gesture interpretation
				OnUp();

				return PressState.UP;
			}
		}

		return PressState.NONE;

		/** SAVED PRINT STATEMENTS FOR DEBUGGING USE **/
		//UnityEngine.Debug.Log("TIME " + swipeWatch.ElapsedMilliseconds.ToString());
		//UnityEngine.Debug.Log("END POS " + touchController.GetLastFingerPosition());
		//UnityEngine.Debug.Log("JUST RELEASED");
		//UnityEngine.Debug.Log("START POS " + touchController.GetLastFingerPosition());
		//UnityEngine.Debug.Log("JUST PRESSED");
		//UnityEngine.Debug.Log("UP idle watch " + idleWatch.ElapsedMilliseconds);
		//UnityEngine.Debug.Log("UP");
		//UnityEngine.Debug.Log("DOWN");
		//UnityEngine.Debug.Log("idle watch " + idleWatch.ElapsedMilliseconds + "\nkey watch " + keyDownWatch.ElapsedMilliseconds);
	}
	
	// Use this for initialization.
	public void Start(){
		// Keeping GameObject component grabbing consistent among classes. - J.T.
		GameObject mainObject = GameObject.FindGameObjectsWithTag("MainObject")[0];

		if(GameObject.FindGameObjectsWithTag("MainObject").Length > 1){
			GameObject[] mainObjectList = GameObject.FindGameObjectsWithTag("MainObject");
			for(int i = 0; i < mainObjectList.Length; ++i){
				if(mainObjectList[i].GetComponent<GameStateManager>().objectSaved){
					mainObject = mainObjectList[i];
				}
			}
		}
		
		gameStateManagerRef = mainObject.GetComponent <GameStateManager>();
		gameStateManagerRef.EnsureGameScriptsAdded();
		gameStateManagerRef.EnsureCoreScriptsAdded();
		screenManagerRef = mainObject.GetComponent<ScreenManager>();
		animManagerRef = mainObject.GetComponent<AnimationManager>();
		worldCollisionRef = mainObject.GetComponent<WorldCollision>();
		touchController = gameObject.GetComponent<TouchController>();
		soundManagerRef = gameStateManagerRef.GetSoundManager();
		paperObject = GameObject.FindGameObjectWithTag("background");
		tearBorder = GameObject.FindGameObjectWithTag("DeadSpace");
		foldBorder = GameObject.FindGameObjectWithTag("foldborder");
		unfoldBorder = GameObject.FindGameObjectWithTag("unfoldborder");
		unfoldBlocker = GameObject.FindGameObjectWithTag("RayTraceBlocker");
		moveBorder = GameObject.FindGameObjectWithTag("insideBorder");
		menuButton = GameObject.Find("MenuButton_Prefab");
		restartButton = GameObject.Find("RestartButton_Prefab");
		moveMode = true;
		tearMode = false;
		foldMode = false;
		//Keeping old code here in case something gets broken. - J.T.
		// Ensures all necessary scripts are added for the MainObject.
		/*
		gameStateManagerRef = gameObject.GetComponent<GameStateManager>();
		gameStateManagerRef.EnsureCoreScriptsAdded();
		gameStateManagerRef.EnsureGameScriptsAdded();
		screenManagerRef = gameObject.GetComponent<ScreenManager>();
		animManagerRef = gameObject.GetComponent<AnimationManager>();
		worldCollisionRef = gameObject.GetComponent<WorldCollision>();
		touchController = gameObject.GetComponent<TouchController>();
		*/
		idleTriggerLimit = Random.Range (1000, 3000);
		keyDownWatch = new Stopwatch();
		idleKeyWatch = new Stopwatch();
		releaseWatch = new Stopwatch();
		justPressedWatch = new Stopwatch();
		playerBottomCollisionWatch = new Stopwatch();
		idlePlayerWatch = new Stopwatch();

		watchList.Add(keyDownWatch);
		watchList.Add(idleKeyWatch);
		watchList.Add(releaseWatch);
		watchList.Add(justPressedWatch);
		watchList.Add(playerBottomCollisionWatch);
		watchList.Add(idlePlayerWatch);

		wasdRight = KeyCode.D;
		wasdLeft = KeyCode.A;
		arrowRight = KeyCode.RightArrow;
		arrowLeft = KeyCode.LeftArrow;
		keyJump = KeyCode.Space;

		hasHorizontalCollision = false;
		currentDirection = ScreenSide.NONE;
		tornPieceInitiated = false;
		movingTornPiece = false;
	}
	
	public void OnEnable(){
		Start();
	}
	
	// Resets the positions of variables that are screen dependent (which is any GUI element).
	public void ResetScreenSizeDependents(){
		leftSideTouchLimitPos = new Vector2(screenManagerRef.GetScreenSize().x / 4, screenManagerRef.GetScreenSize().y);
		rightSideTouchLimitPos = new Vector2(3 * screenManagerRef.GetScreenSize().x / 4, screenManagerRef.GetScreenSize().y);
	}

	public Vector2 FirstPressPos(){ return justPressedFingerPos; }
	public Vector2 LastPressPos(){ return justReleasedFingerPos; }

	private void DetectFirstInput(){
		// Set the initial just pressed finger position to determine if swipe distance is valid.
		if(!touchController.GetLastFingerPosition().Equals(TouchController.NULL_VECTOR) && 
				((justPressedFingerPos == TouchController.NULL_VECTOR &&
				currPressState == PressState.JUST_PRESSED) ||
				samePosWatch.ElapsedMilliseconds > SAME_POS_TIME_LIMIT)){
			pressSet = true;
			releaseSet = false;
		   
			justPressedFingerPos = touchController.GetLastFingerPosition();
			justPressedSide = ReturnSidePressed(justPressedFingerPos);

			//UnityEngine.Debug.Log("JUST PRESS POS " + justPressedFingerPos);
		}
	}

	private void DetectBetweenInput(){
		if(touchController.GetLastFingerPosition() != TouchController.NULL_VECTOR){
			// If the sample pos watch hasn't been started, then do so.
			if(samplePosWatch.ElapsedMilliseconds == 0){
				samplePosWatch.Start();
				fingerIdlePos = touchController.GetLastFingerPosition();
			}
			// If it has started, then determine if we can re-sample a new position for finger idle pos.
			else if (samplePosWatch.ElapsedMilliseconds > SAME_POS_SAMPLE_LIMIT){
				fingerIdlePos = touchController.GetLastFingerPosition();
				samplePosWatch.Reset();
				samplePosWatch.Start();
			}

			// With a new sample of the idle pos, determine if the sampled pos and the current finger pos are relatively the same.
			if(Vector2.Distance(fingerIdlePos, touchController.GetLastFingerPosition()) < SAME_POS_DIST_VARIANCE){
				// If same pos watch hasn't started then do so.
				if (samePosWatch.ElapsedMilliseconds == 0){
					samePosWatch.Start();
				}
				// Else if the watch has already been counting and has breached our arbitrary limit, then reset justPressedFingerPos.
				else if(samePosWatch.ElapsedMilliseconds > SAME_POS_TIME_LIMIT){
					//UnityEngine.Debug.Log("FINGER DOWN POS RESET");
					justPressedFingerPos = touchController.GetLastFingerPosition();
					samePosWatch.Reset();
					samplePosWatch.Stop();
				}
			}
			// Otherwise, if the sampled position is nowhere close to our current finger pos, then reset the respective watch.
			//	TODO: Make sure the sample period is short enough to not allow a finger to back track to the same pos. - D.A.
			else{
				//justPressedFingerPos = touchController.GetLastFingerPosition();
				samePosWatch.Reset();
				samePosWatch.Stop();
			}
		}
	}

	private void DetectLastInput(){
		if(!touchController.GetLastFingerPosition().Equals(TouchController.NULL_VECTOR)){
			justReleasedFingerPos = touchController.GetLastFingerPosition();
			justReleasedSide = ReturnSidePressed(justReleasedFingerPos);
			//UnityEngine.Debug.Log("JUST RELEASED POS " + justReleasedFingerPos);

			if(jumpInitiated 
				//This selection of code was commented out for controls by Douglas Weller
				/*---->&&
					Vector2.Distance(touchController.GetLastFingerPosition(),
					Camera.mainCamera.WorldToScreenPoint(playerObject.transform.position)) > (PLAYER_VOID_MOVEMENT_DIST * 0.5F)*/){
				float degree = Mathf.Atan2(justReleasedFingerPos.y - justPressedFingerPos.y,
						justReleasedFingerPos.x - justPressedFingerPos.x);
				fingerSwipeAngle = degree * 180 / Mathf.PI;
				// atan2 returns values from -180 to 180
				if(fingerSwipeAngle < 0){
					fingerSwipeAngle = 360 + fingerSwipeAngle;
				}

			}
			else if(controllerRef.currentPlayerState == TWCharacterController.playerStates.isGrounded){
				fingerSwipeAngle = 0;
			}
			//UnityEngine.Debug.Log("ANGLE " + fingerSwipeAngle);
		}
		else if (currPressState == PressState.JUST_RELEASED){
				// && currPressState == PressState.UP)
				// && (prevPressState == PressState.DOWN
				// || prevPressState == PressState.JUST_PRESSED)){
			//UnityEngine.Debug.Log("RELEASE " + justReleasedFingerPos);
			//UnityEngine.Debug.Log("PRESS SIDE: " + justPressedSide);
			//UnityEngine.Debug.Log("RELEASE SIDE: " + justReleasedSide);
			//UnityEngine.Debug.Log("TOUCH TYPE " + touchController.ReturnTouchType());
			//UnityEngine.Debug.Log("DIST " + Vector2.Distance(justReleasedFingerPos, justPressedFingerPos));
			//UnityEngine.Debug.Log("TIME " + swipeWatch.ElapsedMilliseconds);
			//UnityEngine.Debug.Log("SWIPE " + IsSwipeInput());
			releaseSet = true;
		}
	}
	
	// Initializes any references that were not set in Start().
	private void InitRemainingRefs(){
		//if(!allLoaded){
			if(!paperObject){
				paperObject = GameObject.FindGameObjectWithTag("background");
			}

			if(!tearBorder){
				tearBorder = GameObject.FindGameObjectWithTag("DeadSpace");
				foldBorder = GameObject.FindGameObjectWithTag("foldborder");
				unfoldBorder = GameObject.FindGameObjectWithTag("unfoldborder");
				unfoldBlocker = GameObject.FindGameObjectWithTag("RayTraceBlocker");
				moveBorder = GameObject.FindGameObjectWithTag("insideBorder");
				
				menuButton = GameObject.Find("MenuButton_Prefab");
				restartButton = GameObject.Find("RestartButton_Prefab");
			}
	
			if(GameObject.FindGameObjectWithTag("TearManager")){
				tearManagerRef = GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>();
			}
	
			if(GameObject.FindGameObjectWithTag("Player")){
				playerObject = GameObject.FindGameObjectWithTag("Player");
				unfoldCollisionRef = playerObject.GetComponent<UnfoldCollision>();
				// JUSTIN, CHAR CONTR IS ATTACHED TO THE PLAYER, NOT MAIN OBJECT - D.A.
				controllerRef = GameObject.FindGameObjectWithTag("Player").GetComponent<TWCharacterController>();
			}
	
			if(GameObject.FindGameObjectWithTag("FoldObject")){
				foldRef = GameObject.FindGameObjectWithTag("FoldObject").GetComponent<Fold>();
			}
			if(paperObject && tearManagerRef && playerObject && foldRef){
				allLoaded = true;
			}
		//}
	}
	
	// Returns a screen side used to determine which direction to the move player based on current finger position and
	//	current tablet orientation.
	private ScreenSide ReturnSidePressed(Vector2 fingerPos){
		if(gameStateManagerRef.OnMobileDevice() && fingerPos != TouchController.NULL_VECTOR){
			switch(screenManagerRef.CurrentDeviceOrientation()){
				// TODO DOM: RESET TOUCH LIMITS TO BE HALVES OF PAPER
				// landscape left
				default:
					if(fingerPos.x >= Camera.mainCamera.WorldToScreenPoint(playerObject.transform.position).x){
						return ScreenSide.RIGHT;
					}
					else{
						return ScreenSide.LEFT;
					}
					break;
				
				case DeviceOrientation.LandscapeRight:
					if(fingerPos.x >= Camera.mainCamera.WorldToScreenPoint(playerObject.transform.position).x){
						return ScreenSide.LEFT;
					}
					else{
						return ScreenSide.RIGHT;
					}
					break;	

				case DeviceOrientation.PortraitUpsideDown:
					if(fingerPos.y >= Camera.mainCamera.WorldToScreenPoint(playerObject.transform.position).y){
						return ScreenSide.LEFT;
					}
					else{
						return ScreenSide.RIGHT;
					}
					break;

				case DeviceOrientation.Portrait:
					if(fingerPos.y >= Camera.mainCamera.WorldToScreenPoint(playerObject.transform.position).y){
						return ScreenSide.RIGHT;
					}
					else{
						return ScreenSide.LEFT;
					}
					break;
			}
		}
		return ScreenSide.NONE;
	}

	// Resets x and y position limits when the player re-orients the tablet.
	private void SetTouchPositionLimits(){
		currentScreenSize = screenManagerRef.GetScreenSize();

		switch(screenManagerRef.CurrentDeviceOrientation()){
			default:
				bottomTouchLimitPos = new Vector2(screenManagerRef.GetScreenSize().x, screenManagerRef.GetScreenSize().y);
				leftSideTouchLimitPos = new Vector2(screenManagerRef.GetScreenSize().x / 2, screenManagerRef.GetScreenSize().y);
				rightSideTouchLimitPos = new Vector2(screenManagerRef.GetScreenSize().x / 2, screenManagerRef.GetScreenSize().y);
				break;

			case DeviceOrientation.LandscapeRight:
				bottomTouchLimitPos = new Vector2(screenManagerRef.GetScreenSize().x, screenManagerRef.GetScreenSize().y);
				leftSideTouchLimitPos = new Vector2(screenManagerRef.GetScreenSize().x / 2, screenManagerRef.GetScreenSize().y);
				rightSideTouchLimitPos = new Vector2(screenManagerRef.GetScreenSize().x / 2, screenManagerRef.GetScreenSize().y);
				break;

			case DeviceOrientation.Portrait:
				bottomTouchLimitPos = new Vector2(screenManagerRef.GetScreenSize().x / 2, screenManagerRef.GetScreenSize().y);
				leftSideTouchLimitPos = new Vector2(screenManagerRef.GetScreenSize().x, screenManagerRef.GetScreenSize().y / 2);
				rightSideTouchLimitPos = new Vector2(screenManagerRef.GetScreenSize().x, screenManagerRef.GetScreenSize().y / 2);
				break;

			case DeviceOrientation.PortraitUpsideDown:
				bottomTouchLimitPos = new Vector2(screenManagerRef.GetScreenSize().x, screenManagerRef.GetScreenSize().y / 2);
				leftSideTouchLimitPos = new Vector2(screenManagerRef.GetScreenSize().x / 2, screenManagerRef.GetScreenSize().y);
				rightSideTouchLimitPos = new Vector2(screenManagerRef.GetScreenSize().x / 2, screenManagerRef.GetScreenSize().y);
				break;
		}
	}

	// Ensures the screen size and resolution are what they need to be as well as resets touch positions when the screen is re-oriented.
	private void HandleScreenChanges(){
		if(currentScreenSize != screenManagerRef.GetScreenSize()){
			currentScreenSize = screenManagerRef.GetScreenSize();
			ResetScreenSizeDependents();
		}

		if(screenManagerRef.DeviceOrientationSwitched || bottomTouchLimitPos == Vector2.zero){
			SetTouchPositionLimits();
		}
	}

	// Sets the player's animations direction based on last finger pos.
	private void SetPlayerDirection(){
		// Set the direction of the animation to the side the player is touching.
		if(!touchController.GetLastFingerPosition().Equals(TouchController.NULL_VECTOR) || !gameStateManagerRef.OnMobileDevice()){
			currDirection = PlayerScreenSideTouched();
		}

		if(!animManagerRef.GetAnimationDirection().Equals(currDirection)){
			animManagerRef.SetDirection(currDirection);
		}
	}

	// Resets fields used for detecting idle input and differentiating between moving and jumping.
	private void ResetTouchFields()
    {
        if (controllerRef.getGrounded())
        {
            fingerSwipeAngle = SWIPE_ANGLE_RESET_VALUE;
        }

		releaseSet = false;
		pressSet = false;
		lastSidePressed = ScreenSide.NONE;
		fingerIdlePos = TouchController.NULL_VECTOR;
		justPressedFingerPos = TouchController.NULL_VECTOR;
		justReleasedFingerPos = TouchController.NULL_VECTOR;
		justPressedSide = ScreenSide.NONE;
		justReleasedSide = justPressedSide;
	}

	private void UpdateInputWatches(){
		if(prevPressState == PressState.UP &&
				currPressState == PressState.UP &&
				releaseWatch.ElapsedMilliseconds > 0 &&
				releaseWatch.ElapsedMilliseconds > DOUBLE_TAP_TIME_LIMIT){
			releasedCount = 0;
			releaseWatch.Reset();
			releaseWatch.Stop();
		}

		// Starts the watch if any of the actions keys are pressed.
		if(PlayerMoveHorizontalTriggered(ScreenSide.LEFT) ||
				PlayerMoveHorizontalTriggered(ScreenSide.RIGHT) ||
				currPressState.Equals(PressState.DOWN) ||
				currPressState.Equals(PressState.JUST_PRESSED) ||
				jumpInitiated){
			if(keyDownWatch.ElapsedMilliseconds.Equals(0)){
				keyDownWatch.Start();
			}

			if(!idleKeyWatch.ElapsedMilliseconds.Equals(0)){
				idlePlayerWatch.Reset();
				idleKeyWatch.Reset();
				idlePlayerWatch.Stop();
				idleKeyWatch.Stop();
			}
		}
		// Else reset the watch and start idle timer.
		else{
			if(!keyDownWatch.ElapsedMilliseconds.Equals(0)){
				timeLastInputHeld = keyDownWatch.ElapsedMilliseconds;
				keyDownWatch.Stop();
				keyDownWatch.Reset();
			}

			if(idleKeyWatch.ElapsedMilliseconds.Equals(0)){
				idleKeyWatch.Start();
			}

			if(idlePlayerWatch.ElapsedMilliseconds.Equals(0)){
				idlePlayerWatch.Start();
			}
		}
	}

	private void UpdateAnimations(){
		// UnityEngine.Debug.Log("STATE " + controllerRef.currentPlayerState);
		if(idlePlayerWatch.ElapsedMilliseconds > idleTriggerLimit &&
				controllerRef.currentPlayerState != TWCharacterController.playerStates.falling &&
				controllerRef.currentPlayerState == TWCharacterController.playerStates.isGrounded &&
				!controllerRef.playerIsDead &&  
				!jumpInitiated){
			if(!animManagerRef.GetCurrentAnimationType().Equals(AnimationManager.AnimationType.IDLE)){
				//If not already idling and stopwatch reaches the limit, reset the flag and assign a new IDLE_TRIGGER_LIMIT.
				animManagerRef.TriggerAnimation(AnimationManager.AnimationType.IDLE);
				idleTriggerLimit = Random.Range(4000, 8000);
			}

			//UnityEngine.Debug.Log("IDLING");
		}

		// We need frames for the death animation before this code gets implemented. - J.T.
		else if(controllerRef.playerIsDead && !animManagerRef.DeathComplete()){
			if(!animManagerRef.GetCurrentAnimationType().Equals(AnimationManager.AnimationType.DEATH)){
				animManagerRef.TriggerAnimation(AnimationManager.AnimationType.DEATH);
			}
			// UnityEngine.Debug.Log("DEATH");
		}

		/*
			DOUG, Animation already has a reference for CharacterController, controllerRef.
			I replaced it below for you - D.A.
			douglas - added check to see if character is rising, only then should jump animation run.
		*/
		else if(jumpInitiated){ // && controllerRef.rising){
			// Trigger jump movement in animation manager.
			if(!animManagerRef.GetCurrentAnimationType().Equals(AnimationManager.AnimationType.JUMP) ||
					(hasHorizontalCollision)){
				animManagerRef.TriggerAnimation(AnimationManager.AnimationType.JUMP);
			}

			if(animManagerRef.GetCurrentAnimationType().Equals(AnimationManager.AnimationType.JUMP)
					&& !soundManagerRef.IsAudioPlaying("jump", "SFX")
					&& animManagerRef.GetCurrentFrame().Equals(1)){
				soundManagerRef.PlayAudio("jump", "SFX");
				//unityengine.debug.log("jumping sound!");
			}
			if(!animManagerRef.GetCurrentAnimationType().Equals(AnimationManager.AnimationType.JUMP)){
				soundManagerRef.StopSound("jump", "SFX");
			}

			if(hasHorizontalCollision){
				hasHorizontalCollision = false;
				playerBottomCollisionWatch.Reset();
				playerBottomCollisionWatch.Start();
			}
			//UnityEngine.Debug.Log("JUMP");
		}
		else if(!hasHorizontalCollision || controllerRef.currentPlayerState == TWCharacterController.playerStates.falling){
			//UnityEngine.Debug.Log("FALLING");
			// UnityEngine.Debug.Log("ROT " + initPlayerNonGroundedZRot);
			float fallBuff = 0.25f;
			if(initPlayerNonGroundedZRot >= 0 && initPlayerNonGroundedZRot < 50){
				if(playerObject.transform.position.y + fallBuff < initPlayerNonGroundedPos.y){
					if(!animManagerRef.GetCurrentAnimationType().Equals(AnimationManager.AnimationType.FALL)){
						animManagerRef.TriggerAnimation(AnimationManager.AnimationType.FALL);
					}
					
					//UnityEngine.Debug.Log("FALLING");
				}
			}

			if(initPlayerNonGroundedZRot > 85 && initPlayerNonGroundedZRot < 95){
				if(playerObject.transform.position.x > initPlayerNonGroundedPos.x + fallBuff){
					if(!animManagerRef.GetCurrentAnimationType().Equals(AnimationManager.AnimationType.FALL)){
						animManagerRef.TriggerAnimation(AnimationManager.AnimationType.FALL);
					}
					
					//UnityEngine.Debug.Log("FALLING");
				}
			}

			if(initPlayerNonGroundedZRot > 175 && initPlayerNonGroundedZRot < 185){
				if(playerObject.transform.position.y > initPlayerNonGroundedPos.y + fallBuff){
					if(!animManagerRef.GetCurrentAnimationType().Equals(AnimationManager.AnimationType.FALL)){
						animManagerRef.TriggerAnimation(AnimationManager.AnimationType.FALL);
					}
					
					//UnityEngine.Debug.Log("FALLING");
				}
			}
			
			if(initPlayerNonGroundedZRot > 265 && initPlayerNonGroundedZRot < 275){
				if(playerObject.transform.position.x + fallBuff < initPlayerNonGroundedPos.x){
					if(!animManagerRef.GetCurrentAnimationType().Equals(AnimationManager.AnimationType.FALL)){
						animManagerRef.TriggerAnimation(AnimationManager.AnimationType.FALL);
					}
				}
			}
		}
		else if((fingerGesture == FingerGesture.MOVE || !gameStateManagerRef.OnMobileDevice()) &&
				PlayerMoveHorizontalTriggered(ReturnSidePressed(touchController.GetLastFingerPosition())) &&
				!foldRef.currentlyFolding &&
				!tearManagerRef.GetMovingPiece()){
			
			if(!animManagerRef.GetCurrentAnimationType().Equals(AnimationManager.AnimationType.WALK)){
				animManagerRef.TriggerAnimation(AnimationManager.AnimationType.WALK);
			}

			if(animManagerRef.GetCurrentAnimationType().Equals(AnimationManager.AnimationType.WALK)
					&& (animManagerRef.GetCurrentFrame().Equals(2)
					|| animManagerRef.GetCurrentFrame().Equals(5)
					|| animManagerRef.GetCurrentFrame().Equals(6))){
				soundManagerRef.PlayAudio("walk_L", "SFX");
				//UnityEngine.Debug.Log("playing walking");
			}
			if(!animManagerRef.GetCurrentAnimationType().Equals(AnimationManager.AnimationType.WALK)){
				soundManagerRef.StopSound("walk_L", "SFX");
			}

			//UnityEngine.Debug.Log("WALKING");
		}
		else if(controllerRef.getGrounded()){ //idleKeyWatch.ElapsedMilliseconds > INPUT_BUFFER_LIMIT && controllerRef.isGrounded){
			//UnityEngine.Debug.Log("STANDING STILL");
			if(!animManagerRef.GetCurrentAnimationType().Equals(AnimationManager.AnimationType.STAND)){
				animManagerRef.TriggerAnimation(AnimationManager.AnimationType.STAND);
			}
		}
		else{
			//UnityEngine.Debug.Log("NO ANIMS TO BE TRIGGERED");
		}
	}
	
	// Sets global variables needed to determine specific touch controls. Such globals need to be only updated once per
	//	update and nowhere else.
	private void SetTouchGlobals(){
		// This is the only place where the function ReturnPressState should be called so we can have consistent prev and curr states.
		prevPressState = currPressState;
		currPressState = ReturnPressState();

		//UnityEngine.Debug.Log("******************************");
		//UnityEngine.Debug.Log("PRESS STATE " + currPressState);
		//UnityEngine.Debug.Log("PREV " + prevPressState.ToString());
		//UnityEngine.Debug.Log("CURR " + currPressState.ToString());
		//UnityEngine.Debug.Log("GEST " + touchController.ReturnTouchType());

		// Set global boolean for swipe detection. Set here so it is used only once.
		swipeInputDetected = IsSwipeInput();
		//UnityEngine.Debug.Log("H");
		//UnityEngine.Debug.Log("SWIPE " + swipeInputDetected);

		// Boolean used throughout the class to determine if player has initiated jump. We call this once so we dont have
		//	conflicting states New, if vector swipe overrides any call to PlayerVerticalMovementTriggered().
		jumpInitiated = PlayerVerticalMovementTriggered();
	}
	
	// Update is called once per frame.
//	public void Update(){
//		// Below Debug Log call is to check for random idle timings.
//		//UnityEngine.Debug.Log("TIMER COUNT " + releaseWatch.ElapsedMilliseconds);
//		//UnityEngine.Debug.Log("idleWatch: " + idleKeyWatch.ElapsedMilliseconds); // + '\n' + "idleTime: " + IDLE_TRIGGER_LIMIT);
//
//    }


	/// <summary>
	/// Update is called once per frame
	/// </summary>
	public void Update()
    {
        /*Below Debug Log call is to check for random idle timings*/
       // UnityEngine.Debug.Log("TIMER COUNT " + releaseWatch.ElapsedMilliseconds);
        //UnityEngine.Debug.Log("idleWatch: " + idleKeyWatch.ElapsedMilliseconds); // + '\n' + "idleTime: " + IDLE_TRIGGER_LIMIT);

        //UnityEngine.Debug.Log("SIZE " + Screen.width.ToString() + ", " + Screen.height.ToString());
        //if (!touchController.GetLastFingerPosition().Equals(TouchController.NULL_VECTOR))
      //  {
            //UnityEngine.Debug.Log("TYPE " + touchController.ReturnTouchType());
        //    UnityEngine.Debug.Log("DIST " + Vector2.Distance(touchController.GetLastFingerPosition(), Camera.mainCamera.WorldToScreenPoint(playerObject.transform.position)));
        //}

		//UnityEngine.Debug.Log("LAST " + justReleasedFingerPos);
		//UnityEngine.Debug.Log("ANGLE " + fingerSwipeAngle);
		/*if(playerObject){
			UnityEngine.Debug.Log("PLAYER " + Camera.mainCamera.WorldToScreenPoint(playerObject.transform.position));
		}*/
		//UnityEngine.Debug.Log("FINGER POS " + touchController.GetLastFingerPosition());

		if(gameStateManagerRef.inGame){
			// Initialize any left over managers, objects, refs, etc.
			InitRemainingRefs();

			// Could have used any of the managers but chose tearManagerRef, but this ensures no code gets executed until
			//	everything is initialized
			if(tearManagerRef){
				if(touchController.ReturnTouchType() == TouchType.NONE){
					movingTornPiece = false;
					currentDirection = ScreenSide.NONE;
				}
				if(tearManagerRef.HaveTornOnce && !tornPieceInitiated){
					tornPiece = GameObject.Find("paper_CuttPieceOfPaper");
					tornPieceInitiated = true;
				}
				// Determine if screen orientation or size has changed and re-assign touch positions/limits.
				HandleScreenChanges();

				// Sets touch globals such current/previous press state swipe detection and jump detectoin.
				SetTouchGlobals();
				
				// Detect first last and in between states/pos' for touch input positions.
				DetectFirstInput();
				DetectBetweenInput();
				DetectLastInput();

				// After horizontal and vertical movements are set determine player animation direction
				SetPlayerDirection();

				if(currPressState == PressState.UP && releaseSet){
					ResetTouchFields();
				}

				// Update idle and keydown watches.
				UpdateInputWatches();

				// Limits time ellapsed on watches.
				LimitWatches();

				// Update and set new animations to be played in Animation Manager.
				UpdateAnimations();
			}
			/*else{
				UnityEngine.Debug.Log("MANAGERS ARE NULL");
			}*/
		}
	}

	private void LimitWatches(){
		for(int i = 0; i < watchList.Count; ++i){
			if(watchList[i].IsRunning){
				if(watchList[i].ElapsedMilliseconds > ELAPSED_WATCH_LIMIT){
					watchList[i].Stop();
				}
			}
		}
	}
	
	// Sets the direction in which the animation will play. Returns the screen side touched.
	public AnimationManager.AnimationDirection PlayerScreenSideTouched(){
		if(gameStateManagerRef.inGame){
			if(!gameStateManagerRef.OnMobileDevice()){
				if(lastKeyboardSide.Equals(ScreenSide.RIGHT)){
					return AnimationManager.AnimationDirection.RIGHT;
				}
				else{
					return AnimationManager.AnimationDirection.LEFT;
				}
			}
			else{
				ScreenSide side = ReturnSidePressed(touchController.GetLastFingerPosition());
				
				if(side == ScreenSide.LEFT){//!controllerRef.MovingRight()){//currentDirection ==  ScreenSide.RIGHT){
					return AnimationManager.AnimationDirection.RIGHT;
				}
				else{
					return AnimationManager.AnimationDirection.LEFT;
				}
			}
		}

		return AnimationManager.AnimationDirection.RIGHT;
	}
	
	// Returns true if finger input is something similar to a swipe, if not then swipe itself.
	private bool IsSwipeInput(){
		if(justReleasedFingerPos == TouchController.NULL_VECTOR ||
				justPressedFingerPos == TouchController.NULL_VECTOR){
			return false;
		}
		return
			(touchController.ReturnTouchType() == TouchType.SWIPE ||
			// Or something similar to a swipe.
			((touchController.ReturnTouchType() == TouchType.DRAG) &&
			// And we swiped the minimum distance.
			Vector2.Distance(justReleasedFingerPos, justPressedFingerPos) >= MIN_SWIPE_DIST &&
			currPressState.Equals(PressState.JUST_PRESSED)));
	}
	
	// Returns true if the player is attempting to jump.
	//	SHOULD ONLY BE CALLED ONCE IN UPDATE. USE THE 'jumpInitiated' bool if you wish to know the following.
	public bool PlayerVerticalMovementTriggered(){
		// if still rising from our jump, continue return true.
		if(controllerRef.getRising()){
			return true;
		}

		// If the player is performing one of the mechanics on the mobile device, then immediately return false.
		if((foldRef.currentlyFolding ||
				tearManagerRef.GetMovingPiece()) &&
				gameStateManagerRef.OnMobileDevice()){
			//UnityEngine.Debug.Log("*** FALSE ****");
			return false;
		}
		bool swipeUp = false;
		if(Input.touches.Length > 0)
		{
			Touch touch = Input.touches[0];
			UnityEngine.Debug.Log(touch.deltaPosition.x/touch.deltaTime + " " + touch.deltaPosition.y/touch.deltaTime);
		switch(screenManagerRef.CurrentDeviceOrientation()){
		default:
				//UnityEngine.Debug.Log(touch.deltaPosition.y/touch.deltaTime+ " " + DELTA_SWIPE_DIFF);

			if(//timeLastInputHeld <= SWIPE_LIMIT &&
			   touch.deltaPosition.y/touch.deltaTime > DELTA_SWIPE_DIFF)
				{
				swipeUp = true;
				}
			break;
		case DeviceOrientation.Portrait:
			if(//timeLastInputHeld <= SWIPE_LIMIT &&
			   touch.deltaPosition.x/touch.deltaTime < -DELTA_SWIPE_DIFF)
				swipeUp = true;
			break;
		case DeviceOrientation.LandscapeRight:
				//UnityEngine.Debug.Log(touch.deltaPosition.y/touch.deltaTime + " " + DELTA_SWIPE_DIFF);
			if(//timeLastInputHeld <= SWIPE_LIMIT &&
			   touch.deltaPosition.y/touch.deltaTime < -DELTA_SWIPE_DIFF)
				swipeUp = true;
			break;
		case DeviceOrientation.PortraitUpsideDown:
			if(//timeLastInputHeld <= SWIPE_LIMIT &&
			   touch.deltaPosition.x/touch.deltaTime > DELTA_SWIPE_DIFF)
				swipeUp = true;
			break;
			}
		}
		// Now, check for correct jump parameters.
		if(
				// A swipe was recognized.
				swipeInputDetected && swipeUp &&
				
				// A finger is within a arbitrary swipe distance of player.
				//((timeLastInputHeld > SWIPE_LIMIT && 
			
			
				//This selection of code was commented out for controls by Douglas Weller
			/*	Vector2.Distance(justPressedFingerPos,
						Camera.mainCamera.WorldToScreenPoint(playerObject.transform.position)) < PLAYER_VOID_MOVEMENT_DIST && */
			
			
			//) ||
				//(timeLastInputHeld <= SWIPE_LIMIT && 
				//timeLastInputHeld > 0 &&
				//Vector2.Distance(touchController.GetLastFingerPosition(),
						//Camera.mainCamera.WorldToScreenPoint(playerObject.transform.position)) < PLAYER_VOID_MOVEMENT_DIST)) &&
	
				// And the average time to swipe was met.
				//swipeWatch.ElapsedMilliseconds > SWIPE_LIMIT &&
	
				// The pressed side is the same as the released side had to implement this since swipe can be returned if player
				//	quickly presses on one side of the screen and then immediately the other.
				//justPressedSide == justReleasedSide &&
	
				justPressedFingerPos != TouchController.NULL_VECTOR &&
				justReleasedFingerPos != TouchController.NULL_VECTOR &&
	
				// If the finger gesture suggests no fold or tear.
				fingerGesture.Equals(FingerGesture.MOVE) ||
				
				// Or keyboard input was applied.
				Input.GetKey(keyJump)){
					return true;
				}
		return false;
	}
	
	// Returns -1, 0, 1 depending on left, no input, right screen touch for moving the player horizontally.
	public int GetHorizontalTouchMovement(){
		if(foldRef){
			if(gameStateManagerRef.OnMobileDevice()){
				if(touchController.GetLastFingerPosition() == TouchController.NULL_VECTOR){
					return 0;
				}
			}
			if(fingerGesture == FingerGesture.MOVE &&
					!tearManagerRef.GetMovingPiece() &&
					!foldRef.currentlyFolding){
				
				
			/*switch(screenManagerRef.CurrentDeviceOrientation()){
				// TODO DOM: RESET TOUCH LIMITS TO BE HALVES OF PAPER
				// landscape left
				case DeviceOrientation.LandscapeLeft:
					if(touchController.getDisplacement() >= 0.3F){
						currentDirection = ScreenSide.LEFT;
						return -1;
					}
					else if(touchController.getDisplacement() <= -0.3F){
						currentDirection = ScreenSide.RIGHT;
						return 1;
					}
					break;
				case DeviceOrientation.LandscapeRight:
					if(touchController.getDisplacement() >= 0.3F){
						currentDirection = ScreenSide.RIGHT;
						return 1;
					}
					else if(touchController.getDisplacement() <= -0.3F){
						currentDirection = ScreenSide.LEFT;
						return -1;
					}
					break;

				case DeviceOrientation.PortraitUpsideDown:
					if(touchController.getDisplacement() >= 0.3F){
						currentDirection = ScreenSide.RIGHT;
						return 1;
					}
					else if(touchController.getDisplacement() <= -0.3F){
						currentDirection = ScreenSide.LEFT;
						return -1;
					}
					break;

				case DeviceOrientation.Portrait:
					if(touchController.getDisplacement() >= 0.3F){
						currentDirection = ScreenSide.LEFT;
						return -1;
					}
					else if(touchController.getDisplacement() <= -0.3F){
						currentDirection = ScreenSide.RIGHT;
						return 1;
					}
					break;
			}*/
				
				
				
				
				if((currentDirection == ScreenSide.NONE  || currentDirection == ScreenSide.LEFT)
						&& ReturnSidePressed(touchController.GetLastFingerPosition()) == ScreenSide.LEFT){
					return -1;
				}
				else if((currentDirection == ScreenSide.NONE || currentDirection == ScreenSide.RIGHT)
						&& ReturnSidePressed(touchController.GetLastFingerPosition()) == ScreenSide.RIGHT){
					return 1;
				}
				else{
					return 0;
				}
				// And did the player hit the viable horizontal section of the screen.
				/*if(/*touchController.GetLastFingerPosition().y <= bottomTouchLimitPos.y &&
						touchController.GetLastFingerPosition().x <= bottomTouchLimitPos.x &&
						ReturnSidePressed(touchController.GetLastFingerPosition()) == ScreenSide.LEFT){
					//UnityEngine.Debug.Log("NEG");
					return -1;
				}
				else if(touchController.GetLastFingerPosition().y <= bottomTouchLimitPos.y &&
						touchController.GetLastFingerPosition().x <= bottomTouchLimitPos.x &&
						ReturnSidePressed(touchController.GetLastFingerPosition()) == ScreenSide.RIGHT){
					//UnityEngine.Debug.Log("POS");
					return 1;
				}*/
			}
		}

		//UnityEngine.Debug.Log("NONE");
		return 0;
	}
	
	// Returns true if the player is attempting to move the player left or right on screen.
	private bool PlayerMoveHorizontalTriggered(ScreenSide side){
		if((!tearManagerRef.GetMovingPiece() && fingerGesture == FingerGesture.MOVE &&
				!foldRef.currentlyFolding &&
				(!tearManagerRef.PlayerCurrentlyTearing 
			
			//This selection of code was commented out for controls by Douglas Weller
			/*||
				!worldCollisionRef.PointInsideObject(paperObject, touchController.GetLastFingerPosition())*/
			
			)) ||
				!gameStateManagerRef.OnMobileDevice()){
			if((Input.GetKey(wasdLeft) || Input.GetKey(arrowLeft)) ||
					(Input.GetKey(wasdRight) || Input.GetKey(arrowRight))){
				if(Input.GetKey(wasdLeft) || Input.GetKey(arrowLeft)){
					lastKeyboardSide = ScreenSide.RIGHT;
				}
				else if((Input.GetKey(wasdLeft) && Input.GetKey(wasdRight)) ||
						(Input.GetKey(arrowLeft) && Input.GetKey(arrowRight))){
					lastKeyboardSide = ScreenSide.RIGHT;
				}
				else{
					lastKeyboardSide = ScreenSide.LEFT;
				}
				//UnityEngine.Debug.Log("WATCH " + justPressedWatch.ElapsedMilliseconds);
				//UnityEngine.Debug.Log("WALK");
				//UnityEngine.Debug.Log("TRUE");
				return true;
			}
			else{
				if(touchController.GetLastFingerPosition().Equals(TouchController.NULL_VECTOR)){
					//UnityEngine.Debug.Log("FALSE");
					return false;
				}
		else{
					//if(
							// did the player actually trigger a keyboard input
							/*(touchController.ReturnTouchType() == TouchType.TAP ||
							touchController.ReturnTouchType() == TouchType.DRAG) &&*/
						/*
							// and did the player hit the viable horizontal section of the screen
							((side.Equals(ScreenSide.LEFT) && ReturnSidePressed(touchController.GetLastFingerPosition()) == ScreenSide.LEFT) ||
							((side.Equals(ScreenSide.RIGHT) && ReturnSidePressed(touchController.GetLastFingerPosition()) == ScreenSide.RIGHT))) &&
						
							// and did the player hit the viable vertical section of the screen
							touchController.GetLastFingerPosition().x <= bottomTouchLimitPos.x &&
							touchController.GetLastFingerPosition().y <= bottomTouchLimitPos.y)*/{
						lastSidePressed = side;
						//UnityEngine.Debug.Log("TRUE");
						//UnityEngine.Debug.Log("MOVING " + side.ToString());
						return true;
					}

					//UnityEngine.Debug.Log("FALSE");
					return false;
			}
			}
		}

		/*else{
			UnityEngine.Debug.Log("FALSE");
		}*/
		return false;
	}
	
	// Gets the current screen being displayed.
	public TearableScreen GetScreen(){
		return screenManagerRef.GetCurrentScreen();
	}
	 
	//public bool getToggleTornPiece(){}
	#endregion
}
