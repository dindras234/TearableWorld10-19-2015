using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelComplete : TearableScreen{
	public bool 						keyBoardEnabled;
	GameObject							blackFadeObject, endLevelObject,
										levelCompleteTextObject, swipeToContinueTextObject;
	private GVariables					globalVariables;
	
	private float						blackFadeAlpha = 0;

	private const int					BUTTON_WIDTH = 50, BUTTON_HEIGHT = 50,
										NUM_LETTERS = 25;
	private Dictionary<int, char>		IntToCharDict;
	private const string				ENTER_SCORE_STRING = "Enter Score", ENTER_NAME_STRING = "Enter Name",
										SHIFT_SYMBOL = "Shift", DELTE_SYMBOL = "Delete", ENTER_SYMBOL = "Enter";
	private string						scoreString = "High Score", nameString = "Enter Name";
	
	// Single string that will switch between user name and score data.
	string								currString;

	// bool that lets us know if the player's name has been entered so the second time the user hits enter on the keyboard,
	//	we know we are ready to progress to the next level.
	private bool						nameEntered = false,
	// Bool that lets the player know whether or not they are inputting capitalized letters.
										isCapitals = true;
	
	// Rectangle for player's text input.
	Rect								textRect;
	bool escDown;
	// Too lazy to figure out how to parse an integer to an ascii character so I just made a dictionary that does the same thing.
	private void SetupDictionary(){
		IntToCharDict = new Dictionary<int, char>();
		IntToCharDict.Add(0, 'A');
		IntToCharDict.Add(1, 'B');
		IntToCharDict.Add(2, 'C');
		IntToCharDict.Add(3, 'D');
		IntToCharDict.Add(4, 'E');
		IntToCharDict.Add(5, 'F');
		IntToCharDict.Add(6, 'G');
		IntToCharDict.Add(7, 'H');
		IntToCharDict.Add(8, 'I');
		IntToCharDict.Add(9, 'J');
		IntToCharDict.Add(10, 'K');
		IntToCharDict.Add(11, 'L');
		IntToCharDict.Add(12, 'M');
		IntToCharDict.Add(13, 'N');
		IntToCharDict.Add(14, 'O');
		IntToCharDict.Add(15, 'P');
		IntToCharDict.Add(16, 'Q');
		IntToCharDict.Add(17, 'R');
		IntToCharDict.Add(18, 'S');
		IntToCharDict.Add(19, 'T');
		IntToCharDict.Add(20, 'U');
		IntToCharDict.Add(21, 'V');
		IntToCharDict.Add(22, 'W');
		IntToCharDict.Add(23, 'X');
		IntToCharDict.Add(24, 'Y');
		IntToCharDict.Add(25, 'Z');
	}

	// Use this for initialization.
	public override void Start(){
		base.Start();
		escDown = false;
		GameObject mainObject = GameObject.FindGameObjectsWithTag("MainObject")[0];
		gameStateManagerRef = mainObject.GetComponent<GameStateManager>();
		if(GameObject.FindGameObjectsWithTag("MainObject").Length > 1){
			GameObject[] mainObjectList = GameObject.FindGameObjectsWithTag("MainObject");
			for(int i = 0; i < mainObjectList.Length; ++i){
				if(mainObjectList[i].GetComponent<GameStateManager>().objectSaved){
					mainObject = mainObjectList[i];
					gameStateManagerRef = mainObject.GetComponent<GameStateManager>();
				}
			}
		}
		
		// Get the global variables reference.
		GameObject gVar = GameObject.FindGameObjectsWithTag("globalVariables")[0];
		globalVariables = gVar.GetComponent<GVariables>();

		if(IntToCharDict == null){
			SetupDictionary();
		}

		BUTTON_DELTA = new Vector2(60, 60);
		NUM_COLS = 10;
		NUM_ROWS = 3;

		START_POS = new Vector2(
				START_POS.x / 2f - (((BUTTON_WIDTH + (BUTTON_DELTA.x - BUTTON_WIDTH)) * NUM_COLS) / 2f),
				START_POS.y / 2f);

		scoreString = ENTER_SCORE_STRING;
		nameString = ENTER_NAME_STRING;
		currString = nameString;

		CreateGUIButtons();
		CreateGUIBox();

		gameStateManagerRef.gameObject.GetComponent<InputManager>().enabled = false;
		gameStateManagerRef.gameObject.GetComponent<AnimationManager>().enabled = false;

		endLevelObject = GameObject.FindGameObjectWithTag("EndLevelObject");
		levelCompleteTextObject = GameObject.FindGameObjectWithTag("LevelCompleteText");
		swipeToContinueTextObject = GameObject.FindGameObjectWithTag("SwipeToContinueText");

		blackFadeObject = GameObject.FindGameObjectWithTag("BlackFade");
		blackFadeObject.GetComponent<GUITexture>().enabled = false;
		blackFadeObject.GetComponent<Camera>().enabled = true;
		blackFadeObject.GetComponent<GUILayer>().enabled = true;

		blackFadeAlpha = 0;
	}

	// Function that adds the special keys that right now include shift, delte, and enter.
	private void AddUniqueKeys(int currentNumOption){
		Rect deleteRect = new Rect(START_POS.x,
				START_POS.y + (BUTTON_DELTA.y * (NUM_ROWS - 1)),
				BUTTON_WIDTH,
				BUTTON_HEIGHT);

		Rect shiftRect = new Rect(START_POS.x + BUTTON_DELTA.x,
				START_POS.y + (BUTTON_DELTA.y * (NUM_ROWS - 1)),
				BUTTON_WIDTH,
				BUTTON_HEIGHT);

		Rect enterRect = new Rect(START_POS.x + (BUTTON_DELTA.x * (NUM_COLS - 2) + 5),
				START_POS.y + (BUTTON_DELTA.y * (NUM_ROWS - 1)),
				BUTTON_WIDTH * 2,
				BUTTON_HEIGHT);

		textRect = new Rect(START_POS.x + 50,
				START_POS.y - 150,
				BUTTON_WIDTH * NUM_COLS,
				100);


		menuButtonList.Add(new GuiButton(deleteRect, DELTE_SYMBOL));
		currentNumOption++;

		menuButtonList.Add(new GuiButton(shiftRect, SHIFT_SYMBOL));
		currentNumOption++;

		menuButtonList.Add(new GuiButton(enterRect, ENTER_SYMBOL));
		currentNumOption++;

		ButtonToScreenDict.Add(menuButtonList[menuButtonList.Count - 1], -1);
	}

	// Adds a row of numeric characters 0 - 9 right beneath the alpha keys.
	private void AddNumberKeys(){
		for(int i = 0; i < 10; ++i){
			Rect rectangle = new Rect(START_POS.x + (BUTTON_DELTA.x * (i)),
					START_POS.y + (BUTTON_DELTA.y * NUM_ROWS),
					BUTTON_WIDTH,
					BUTTON_HEIGHT);

			menuButtonList.Add(new GuiButton(rectangle, i.ToString()));
			currentNumOption++;
			ButtonToScreenDict.Add(menuButtonList[menuButtonList.Count - 1], -1);
		}
	}	

	// Updates the position of GUI elements based on screen size.
	public override void Update(){
		base.Update();
		//allows esc to continue on
#if UNITY_STANDALONE
		if(Input.GetKeyDown(KeyCode.Escape)) escDown = true;
#endif
		if(endLevelObject){
			//Debug.Log("ALPHA " + blackFadeAlpha);
			if(blackFadeAlpha < 255 && !endLevelObject.GetComponent<GUITexture>().enabled){
				blackFadeAlpha += 2;
				blackFadeObject.GetComponent<GUITexture>().enabled = true;
				blackFadeObject.GetComponent<GUITexture>().color = new Color(0, 0, 0, blackFadeAlpha / 255.0F);
				// Debug.Log("ONE HIT ");
			}
			else{
				if(!endLevelObject.GetComponent<GUITexture>().enabled){
					endLevelObject.GetComponent<EndLevelAnimate>().SetZero();
					endLevelObject.GetComponent<GUITexture>().enabled = true;
					endLevelObject.GetComponent<Camera>().enabled = true;
					endLevelObject.GetComponent<Camera>().depth = 0;
					endLevelObject.GetComponent<Camera>().cullingMask = 0xffff;
					endLevelObject.GetComponent<GUILayer>().enabled = true;
					blackFadeAlpha = 0;
				}

				endLevelObject.GetComponent<GUITexture>().color = new Color(
						endLevelObject.GetComponent<GUITexture>().color.r,
						endLevelObject.GetComponent<GUITexture>().color.g,
						endLevelObject.GetComponent<GUITexture>().color.b,
						blackFadeAlpha / 255.0F);
				
				if(blackFadeAlpha < 255){
					blackFadeAlpha += 2;
				}
			}

			// If a swipe or left mouse button down is detected, then proceed to next level.
			if(endLevelObject.GetComponent<GUITexture>().enabled && blackFadeAlpha >= 10){
				if(levelCompleteTextObject){
					//levelCompleteTextObject.GetComponent<GUIText>().enabled = true;
				}
				if(swipeToContinueTextObject){
					//swipeToContinueTextObject.GetComponent<GUIText>().enabled = true;
				}
				if(gameStateManagerRef.GetTouchController().ReturnTouchType() == TouchType.TAP ||
						Input.GetMouseButtonDown(0) ||(escDown && Input.GetKeyUp(KeyCode.Escape))){
					escDown = false;
					gameStateManagerRef.GetAnimationManager().AssignLevelTextures(true);
					gameStateManagerRef.LevelWon(globalVariables.coins, "Player Name");
				}
			}
		}
	}

	public override void OnGUI(){
		if(keyBoardEnabled){
			// Draw all menu boxes for this screen typically there should only be one.
			for(int i = 0; i < menuBoxList.Count; ++i){
				GUI.Box(menuBoxList[i].rect, menuBoxList[i].str);
			}

			// Draw all the menu buttons for this screen.
			for(int i = 0; i < menuButtonList.Count; ++i){
				if(GUI.Button(menuButtonList[i].rect, menuButtonList[i].str)){
					if((currString.Equals(ENTER_SCORE_STRING) && nameEntered) ||
							(currString.Equals(ENTER_NAME_STRING) && !nameEntered)){
						currString = "";
					}
					switch (menuButtonList[i].str){
						case DELTE_SYMBOL:
							if(currString.Length > 0)
								currString = currString.Substring(0, currString.Length - 1);
							break;
						
						case SHIFT_SYMBOL:
							// Get any character entered and either capitalize or lowercase depending on which state the menu is in.
							if(isCapitals){
								for(int j = 0; j < NUM_LETTERS; ++j){
									menuButtonList[j].str = menuButtonList[j].str.ToLower();
								}
								isCapitals = false;
							}
		
							else{
								for(int j = 0; j < NUM_LETTERS; ++j){
									menuButtonList[j].str = menuButtonList[j].str.ToUpper();
								}
								isCapitals = true;
							}
							break;
						
						case ENTER_SYMBOL:
							// ensuring the scoreString consists of only numbers
							try{
								if(nameEntered){
									scoreString = currString;
									int.Parse(scoreString);
									gameStateManagerRef.LevelWon(int.Parse(scoreString), nameString);
									nameString = ENTER_NAME_STRING;
									scoreString = ENTER_SCORE_STRING;
									currString = nameString;
									nameEntered = false;
								}
								else{
									nameString = currString;
									currString = scoreString;
									nameEntered = true;
								}
							}
							catch{
								Debug.LogError("Invalid Score String");
							}
							break;
						
						default:
							// Makes sure that the input character corresponds to the correct case.
							if(isCapitals){
								currString += menuButtonList[i].str.ToUpper();
							}
							else{ currString += menuButtonList[i].str.ToLower(); }
							break;
					}
				}
			}
			// Remember, this screen appears on top of the play space so we need screen resolution here, not screen size
			//	like normal GUI elements.
			START_POS = gameStateManagerRef.GetScreenManager().GetScreenSize();

			currString = GUI.TextField(textRect, currString, 25);
		}
	}

	public override void OnEnable(){
		//base.OnEnable();
		Start();
	}

	public override void CreateGUIBox(){
		// Add our containing box
		menuBoxList.Add(new GuiBox(new Rect(
				START_POS.x - MENU_BOX_BUFFER,
				START_POS.y - MENU_BOX_BUFFER,
				MENU_BOX_BUFFER * 2 + BUTTON_WIDTH + (BUTTON_DELTA.x * (NUM_COLS - 1)),
				MENU_BOX_BUFFER * 2 + BUTTON_WIDTH + (BUTTON_DELTA.y * (NUM_ROWS))),
				"KEYBOARD"));
	}

	public override void CreateGUIButtons(){
		currentNumOption = 0;
		
		for(int r = 0; r < NUM_ROWS; r++){
			for(int c = (r == NUM_ROWS - 1) ? 2 : 0; c < NUM_COLS; c++){
				Rect rectangle = new Rect(START_POS.x + (BUTTON_DELTA.x * (c)),
										  START_POS.y + (BUTTON_DELTA.y * (r)),
										  BUTTON_WIDTH,
										  BUTTON_HEIGHT);

				menuButtonList.Add(new GuiButton(rectangle, IntToCharDict[currentNumOption].ToString()));
				currentNumOption++;
				ButtonToScreenDict.Add(menuButtonList[menuButtonList.Count - 1], -1);

				// We dont have NUM_ROWS * NUM_COLS (30) keys only 26 for the alphabet, so cut the for loop prematurely
				//	and add our unique keys within the structure of keys we already have and the numeric keys underneath that.
				if(currentNumOption >= IntToCharDict.Count){
					AddUniqueKeys(currentNumOption);
					AddNumberKeys();
					break;
				}
			}
		}
	}

	public override void CreateSliders(){}
	
	public override void CreateLabels(){}
}
