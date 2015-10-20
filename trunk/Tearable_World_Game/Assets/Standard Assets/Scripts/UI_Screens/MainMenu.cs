using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class MainMenu : TearableScreen{
	// Constructor
	public MainMenu() : base(){}
	
	// Use this for initialization.
	public override void Start(){
		base.Start();
		
		// Set the menu proportions.
		//START_POS = Vector2.zero;
		
		NUM_ROWS = 4;
		NUM_COLS = 1;
		
		MENU_BOX_WIDTH = MENU_OPTION_WIDTH + MENU_BOX_BUFFER;
		MENU_BOX_HEIGHT = (MENU_OPTION_HEIGHT * NUM_ROWS) + (MENU_OPTION_DELTA * (NUM_ROWS - 1) + (2 * MENU_BOX_BUFFER));
		if(MENU_BOX_WIDTH < Screen.width*0.5F){
			MENU_BOX_WIDTH = Screen.width*0.5F;
		}
		if(MENU_BOX_HEIGHT < Screen.height*0.9F){
			MENU_BOX_HEIGHT = Screen.height*0.7F;
		}
		
		MENU_OPTION_DELTA = 10;
		MENU_BOX_BUFFER = 100;
		MENU_TOP_BUFFER = 50;
		
		MENU_OPTION_WIDTH = (int)(MENU_BOX_WIDTH - MENU_BOX_BUFFER)/NUM_COLS;
		MENU_OPTION_HEIGHT = (int)(MENU_BOX_HEIGHT - (MENU_BOX_BUFFER+MENU_BOX_BUFFER) - MENU_OPTION_DELTA)/(NUM_ROWS + 1);
		
		// Set up containers (i.e. boxes & buttons).
		CreateGUIBox();
		CreateGUIButtons();
	}

	public override void OnEnable()
    {
		base.OnEnable();
	}

	// Abstract function that is called on creation and dynamically if screen size changes.
	public override void CreateGUIButtons()
    {
		Rect[] buttons = new Rect[NUM_COLS*NUM_ROWS];

		for(int i = 0; i < NUM_ROWS; i++){
			Rect currRect = new Rect(
					START_POS.x * 0.5F + MENU_BOX_BUFFER * 0.5F - MENU_BOX_WIDTH * 0.5F,
					START_POS.y * 0.5F + MENU_BOX_BUFFER * 0.1F - MENU_BOX_HEIGHT * 0.1F + (MENU_OPTION_HEIGHT * (i + 1)) + (MENU_OPTION_DELTA * (i + 1)),
					MENU_OPTION_WIDTH,
					MENU_OPTION_HEIGHT);
			buttons[i] = currRect;
		}

		menuButtonList.Add(new GuiButton(buttons[0], "New Game", "Button"));
		ButtonToScreenDict.Add(menuButtonList[menuButtonList.Count - 1], (int)ScreenAreas.NewGame);

		menuButtonList.Add(new GuiButton(buttons[1], "Select Level", "Button"));
		ButtonToScreenDict.Add(menuButtonList[menuButtonList.Count - 1], (int)ScreenAreas.LevelSelect);

		menuButtonList.Add(new GuiButton(buttons[2], "Options", "Button"));
		ButtonToScreenDict.Add(menuButtonList[menuButtonList.Count - 1], (int)ScreenAreas.Options);

		/*menuButtonList.Add(new GuiButton(buttons[3], "Test", "Button"));
		ButtonToScreenDict.Add(menuButtonList[menuButtonList.Count - 1], (int)ScreenAreas.Test);*/

		menuButtonList.Add(new GuiButton(buttons[3], "Quit", "Button"));
		ButtonToScreenDict.Add(menuButtonList[menuButtonList.Count - 1], (int)ScreenAreas.Quit);
	}

	// Abstract function that is called on creation and dynamically if screen size changes.
	public override void CreateGUIBox(){
		// FILL DEM CONTAINERS (i.e. titles & destinations)
		menuBoxList.Add(new GuiBox(new Rect(
				START_POS.x * 0.5F - MENU_BOX_WIDTH * 0.5F,
				START_POS.y * 0.82F - MENU_BOX_HEIGHT * 0.5F,
				MENU_BOX_WIDTH,
				MENU_BOX_HEIGHT), ""));
	}

	public override void CreateSliders(){}
	public override void CreateLabels(){}
	
	// Updates the position of GUI elements based on screen size.
	public override void  Update(){
 		 base.Update();
	}
}
