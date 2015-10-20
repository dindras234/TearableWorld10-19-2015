using UnityEngine;
using System.Collections;

public class LevelSelectC : TearableScreen{
	// Buttons. Important to make global to the class so we can easily place score/name next to level buttons.
	//Rect[] levelButtons = new Rect[Level buttons + Menu button + Page Flip buttons]
	Rect[]							levelButtons = new Rect[1+1+1];
	Texture2D						coinsZero, coinsOne, coinsTwo, coinsThree,
									lockedLevel, completeLevel;
	
	int								totalCoins = 0,
									levelReached = 1;
	
	public enum levelDisplay{
		nameOnly = 0,
		completion = 1,
		coins = 2
	};
	public levelDisplay				display = levelDisplay.coins;
	bool escDown;
	public override void Start(){
		base.Start();
		styles = (GUISkin)Resources.Load("stylesLevel");
		escDown = false;
		// Set the menu proportions.
		START_POS = Vector2.zero;
		
		NUM_ROWS = 2;
		NUM_COLS = 1;
		
		MENU_BOX_WIDTH = MENU_OPTION_WIDTH + MENU_BOX_BUFFER;
		MENU_BOX_HEIGHT = (MENU_OPTION_HEIGHT * NUM_ROWS) + (MENU_OPTION_DELTA * (NUM_ROWS - 1) + (2 * MENU_BOX_BUFFER));
		if(MENU_BOX_WIDTH < Screen.width*0.8F){
			MENU_BOX_WIDTH = Screen.width*0.8F;
		}
		if(MENU_BOX_HEIGHT < Screen.height*0.9F){
			MENU_BOX_HEIGHT = Screen.height*0.9F;
		}
		
		MENU_OPTION_DELTA = 0;
		MENU_BOX_BUFFER = 10;
		MENU_TOP_BUFFER = 10;
		
		MENU_OPTION_WIDTH = (int)(MENU_BOX_WIDTH - (MENU_BOX_BUFFER+MENU_BOX_BUFFER) - MENU_OPTION_DELTA)/NUM_COLS;
		MENU_OPTION_HEIGHT = (int)(MENU_BOX_HEIGHT - (MENU_TOP_BUFFER+MENU_TOP_BUFFER) - MENU_OPTION_DELTA)/(NUM_ROWS + 2);
		
		coinsZero = Resources.Load("2DImages/Other/coinsZero") as Texture2D;
		coinsOne = Resources.Load("2DImages/Other/coinsOne") as Texture2D;
		coinsTwo = Resources.Load("2DImages/Other/coinsTwo") as Texture2D;
		coinsThree = Resources.Load("2DImages/Other/coinsThree") as Texture2D;
		
		lockedLevel = Resources.Load("2DImages/Other/Lock") as Texture2D;
		completeLevel = Resources.Load("2DImages/Other/Checkmark") as Texture2D;
		
		gameStateManagerRef.GetStatisticManager().LoadSavedGames();
		
		levelReached = gameStateManagerRef.GetStatisticManager().LevelScoresDict.Count + 1;
		
		totalCoins = 0;
		for(int lv = 1; lv < levelReached; ++lv){
			totalCoins += gameStateManagerRef.GetStatisticManager().LevelScoresDict[lv];
		}
		
		CreateGUIBox();
		CreateGUIButtons();
	}

	public override void OnEnable(){
		base.OnEnable();
		levelReached = gameStateManagerRef.GetStatisticManager().LevelScoresDict.Count + 1;
		CreateGUIBox();
		CreateGUIButtons();
		//if(gameStateManagerRef.GetStatisticManager()){
			gameStateManagerRef.GetStatisticManager().LoadSavedGames();
		//}
	}

	// Updates the position of GUI elements based on screen size.
	public override void Update(){
		base.Update();
		//allows the esc button to back up
#if UNITY_STANDALONE
		if(Input.GetKeyDown(KeyCode.Escape)) escDown = true;
		if(escDown && Input.GetKeyUp(KeyCode.Escape))
		{
			escDown = false;
			gameStateManagerRef.GetScreenManager().DisplayScreen(ScreenAreas.LevelSelectB);
		}
#endif
	}

	public override void CreateGUIBox(){
		menuBoxList.Add(new GuiBox(new Rect(
				(START_POS.x * 0.5F) - (MENU_BOX_WIDTH * 0.5F),
				(START_POS.y * 0.5F) - (MENU_BOX_HEIGHT * 0.5F),
				MENU_BOX_WIDTH,
				MENU_BOX_HEIGHT), "Level Select"));
	}

	public override void CreateGUIButtons(){
		// Calculate level buttons locations.
		for(int i = 0; i < (NUM_ROWS-1); i++){
			for(int j = 0; j < NUM_COLS; j++){
				Rect currRect = new Rect(
						(START_POS.x * 0.5F) + (MENU_BOX_BUFFER * 1.0F) - (MENU_BOX_WIDTH * 0.5F) +
							(MENU_OPTION_WIDTH * j) + (MENU_OPTION_DELTA * j),
						(START_POS.y * 0.5F) + (MENU_TOP_BUFFER * 0.5F) - (MENU_BOX_HEIGHT * 0.5F) +
							(MENU_OPTION_HEIGHT * (i+1)) + (MENU_OPTION_DELTA * (i+1)),
						MENU_OPTION_WIDTH,
						MENU_OPTION_HEIGHT);
				levelButtons[j + i*NUM_COLS] = currRect;
			}
		}
		ResetGUIButtons();
	}
	
	public void ResetGUIButtons(){
		levelReached = gameStateManagerRef.GetStatisticManager().LevelScoresDict.Count + 1;
		
		totalCoins = 0;
		for(int lv = 1; lv < levelReached; ++lv){
			totalCoins += gameStateManagerRef.GetStatisticManager().LevelScoresDict[lv];
		}
		
		// Remake level buttons.
		ButtonToScreenDict.Clear();
		menuButtonList.Clear();
		#region LevelButtons
		int buttonDestination = (int)(ScreenAreas.LevelSelectC);
		
		menuButtonList.Add(new GuiButton(levelButtons[0], "Door Of Air", "Button"));
		if(levelReached >= 25){ buttonDestination = 25; }
		ButtonToScreenDict.Add(menuButtonList[menuButtonList.Count - 1], buttonDestination);
		buttonDestination = (int)(ScreenAreas.LevelSelectC);
		#endregion
		
		// Calculate location for and make the Menu button.
		levelButtons[1] = new Rect(
				(START_POS.x * 0.5F) + (MENU_BOX_BUFFER * 1.0F) - (MENU_BOX_WIDTH * 0.5F) +
					(MENU_OPTION_WIDTH * NUM_COLS * 0.2F),
				(START_POS.y * 0.5F) + (MENU_TOP_BUFFER * 0.5F) - (MENU_BOX_HEIGHT * 0.5F) +
					(MENU_OPTION_HEIGHT * NUM_ROWS) + (MENU_OPTION_DELTA * NUM_ROWS),
				(MENU_OPTION_WIDTH * NUM_COLS * 0.6F),
				MENU_OPTION_HEIGHT);
		menuButtonList.Add(new GuiButton(levelButtons[1], "Main Menu", "Button"));
		ButtonToScreenDict.Add(menuButtonList[menuButtonList.Count - 1], (int)ScreenAreas.MainMenu);
		
		// Calculate location for and make the Prev. Page button.
		levelButtons[2] = new Rect(
				(START_POS.x * 0.5F) + (MENU_BOX_BUFFER * 1.0F) - (MENU_BOX_WIDTH * 0.5F),
				(START_POS.y * 0.5F) + (MENU_TOP_BUFFER * 0.5F) - (MENU_BOX_HEIGHT * 0.5F) +
					(MENU_OPTION_HEIGHT * NUM_ROWS) + (MENU_OPTION_DELTA * NUM_ROWS),
				(MENU_OPTION_WIDTH * NUM_COLS * 0.2F),
				MENU_OPTION_HEIGHT);
		menuButtonList.Add(new GuiButton(levelButtons[2], "<--", "Button"));
		ButtonToScreenDict.Add(menuButtonList[menuButtonList.Count - 1], (int)ScreenAreas.LevelSelectB);
	}

	public override void OnGUI(){
		base.OnGUI();
		//Tkae out comments if you add coins - Douglas
		// Drawing of high scores/names for each level.
		//gameStateManagerRef.GetScreenManager().savedDataGUIStyle.fontSize = coinCountFontSize;
		
	//	int currentCoins = 0;
		Texture currentImage = coinsZero;
		Rect currentRect;
		// NOTICE: Level numbers start at 1, not 0.
		for(int index = 25; index <= 25; ++index){
			/*if(levelReached > index){
				currentCoins = gameStateManagerRef.GetStatisticManager().LevelScoresDict[index];
			}*/
			currentRect = menuButtonList[index - 1 - 24].rect;
			
			// Draw coins found for each individual level.
			/*if((display == levelDisplay.coins) && (levelReached > index)){
				switch(currentCoins){
					case 1:
						currentImage = coinsOne;
						break;
					case 2:
						currentImage = coinsTwo;
						break;
					case 3:
						currentImage = coinsThree;
						break;
					default:
						currentImage = coinsZero;
						break;
				}
				GUI.DrawTexture(new Rect(
						currentRect.xMin + (currentRect.width * 0.25F),
						currentRect.yMin,
						currentRect.width * 0.5F,
						currentRect.height * 0.5F),
						currentImage);
			}
			// Draw checkmarks as appropriate.
			else*/ if((display == levelDisplay.completion) && (levelReached > index)){
				currentImage = completeLevel;
				GUI.DrawTexture(new Rect(
						currentRect.xMin + (currentRect.width * 0.4375F),
						currentRect.yMin,
						currentRect.width * 0.125F,
						currentRect.height * 0.5F),
						currentImage);
			}
			// Draw locks as appropriate.
			if(levelReached < index){
				currentImage = lockedLevel;
				GUI.DrawTexture(new Rect(
						currentRect.xMin + (currentRect.width * 0.4375F),
						currentRect.yMin,
						currentRect.width * 0.125F,
						currentRect.height * 0.5F),
						currentImage);
			}
		}
		/*GUI.Label(new Rect(
				(START_POS.x * 0.5F) + (MENU_BOX_WIDTH * 0.3F),
				(START_POS.y * 0.5F) - (MENU_BOX_HEIGHT * 0.45F),
				MENU_BOX_WIDTH,
				MENU_BOX_HEIGHT),
				"Coins: " + totalCoins + " / " + (gameStateManagerRef.FINAL_LEVEL_NUM)*3,
				gameStateManagerRef.GetScreenManager().savedDataGUIStyle);*/
	}
	public override void CreateSliders(){}
	public override void CreateLabels(){}
}
