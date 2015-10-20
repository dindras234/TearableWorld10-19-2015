using UnityEngine;
using System.Collections;

public class Pause : TearableScreen{
	bool escDown;
	// Use this for initialization
	public override void Start(){
		escDown = false;
		base.Start();
		
		NUM_ROWS = 4;
		NUM_COLS = 1;
		
		// Set the menu proportions.
		//START_POS = Vector2.zero;
		
		MENU_BOX_WIDTH = MENU_OPTION_WIDTH + MENU_BOX_BUFFER;
		MENU_BOX_HEIGHT = (MENU_OPTION_HEIGHT * NUM_ROWS) + (MENU_OPTION_DELTA * (NUM_ROWS - 1) + (2 * MENU_BOX_BUFFER));
		if(MENU_BOX_WIDTH < Screen.width*0.5F){
			MENU_BOX_WIDTH = Screen.width*0.5F;
		}
		if(MENU_BOX_HEIGHT < Screen.height*0.9F){
			MENU_BOX_HEIGHT = Screen.height*0.9F;
		}
		
		MENU_OPTION_DELTA = 10;
		MENU_BOX_BUFFER = 50;
		MENU_TOP_BUFFER = 10;
		
		MENU_OPTION_WIDTH = (int)(MENU_BOX_WIDTH - (MENU_BOX_BUFFER+MENU_BOX_BUFFER) - MENU_OPTION_DELTA)/NUM_COLS;
		MENU_OPTION_HEIGHT = (int)(MENU_BOX_HEIGHT - (MENU_TOP_BUFFER+MENU_TOP_BUFFER) - MENU_OPTION_DELTA)/(NUM_ROWS + 1);
	}

	public override void CreateSliders(){}

	public override void CreateGUIButtons(){
		// Calculate location for and make the Menu button.
		Rect currentRect = new Rect(
			(START_POS.x * 0.5F) + (MENU_BOX_BUFFER * 1.0F) - (MENU_BOX_WIDTH * 0.5F) +
				(MENU_OPTION_WIDTH * NUM_COLS * 0.2F),
			(START_POS.y * 0.5F) + (MENU_TOP_BUFFER * 0.5F) - (MENU_BOX_HEIGHT * 0.5F) +
				(MENU_OPTION_HEIGHT * 1) + (MENU_OPTION_DELTA * 1.0F),
			(MENU_OPTION_WIDTH * NUM_COLS * 0.6F),
			MENU_OPTION_HEIGHT);
		menuButtonList.Add(new GuiButton(currentRect, "Main Menu", "Button"));
		ButtonToScreenDict.Add(menuButtonList[menuButtonList.Count - 1], (int)ScreenAreas.MainMenu);
		
		
		currentRect = new Rect(
			(START_POS.x * 0.5F) + (MENU_BOX_BUFFER * 1.0F) - (MENU_BOX_WIDTH * 0.5F) +
				(MENU_OPTION_WIDTH * NUM_COLS * 0.2F),
			(START_POS.y * 0.5F) + (MENU_TOP_BUFFER * 0.5F) - (MENU_BOX_HEIGHT * 0.5F) +
				(MENU_OPTION_HEIGHT * 2) + (MENU_OPTION_DELTA * 1.0F),
			(MENU_OPTION_WIDTH * NUM_COLS * 0.6F),
			MENU_OPTION_HEIGHT);
		menuButtonList.Add(new GuiButton(currentRect, "Options", "Button"));
		ButtonToScreenDict.Add(menuButtonList[menuButtonList.Count -1], (int)ScreenAreas.OptionsInGame);
		
		
		currentRect = new Rect(
			(START_POS.x * 0.5F) + (MENU_BOX_BUFFER * 1.0F) - (MENU_BOX_WIDTH * 0.5F) +
				(MENU_OPTION_WIDTH * NUM_COLS * 0.2F),
			(START_POS.y * 0.5F) + (MENU_TOP_BUFFER * 0.5F) - (MENU_BOX_HEIGHT * 0.5F) +
				(MENU_OPTION_HEIGHT * 3) + (MENU_OPTION_DELTA * NUM_ROWS),
			(MENU_OPTION_WIDTH * NUM_COLS * 0.6F),
			MENU_OPTION_HEIGHT);
		menuButtonList.Add(new GuiButton(currentRect, "Continue", "Button"));
		ButtonToScreenDict.Add(menuButtonList[menuButtonList.Count - 1], (int)ScreenAreas.ContinueGame);

		/*menuButtonList.Add(new GuiButton(buttons[2], "Beat Level", "Button"));
		ButtonToScreenDict.Add(menuButtonList[menuButtonList.Count - 1], (int)ScreenAreas.LevelComplete);*/
	}

	public override void OnEnable(){
		base.OnEnable();
	}

	public override void CreateGUIBox(){
		menuBoxList.Add(new GuiBox(new Rect(
				START_POS.x * 0.5F - MENU_BOX_WIDTH * 0.5F,
				START_POS.y * 0.5F - MENU_BOX_HEIGHT * 0.5F,
				MENU_BOX_WIDTH,
				MENU_BOX_HEIGHT), "PAUSE"));
	}
	public override void CreateLabels (){}
	// Updates the position of GUI elements based on screen size.
	public override void Update(){
		base.Update();
		//code to allow you to escape back to previous screen
#if UNITY_STANDALONE
		if(Input.GetKeyDown(KeyCode.Escape)) escDown = true;
		if(escDown && Input.GetKeyUp(KeyCode.Escape))
		{
			escDown = false;
			gameStateManagerRef.GetScreenManager().DisplayScreen(ScreenAreas.ContinueGame);
		}
#endif
	}
}
