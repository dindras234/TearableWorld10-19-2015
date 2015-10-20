using UnityEngine;
using System.Collections;

public class OptionsInGame : TearableScreen{
	SoundManager					soundRef;
	Rect[]							buttons;// = new Rect[totalNumOptions];
	bool escDown;
	// Use this for initialization
	public override void Start(){
		base.Start();
		escDown = false;
		styles = (GUISkin)Resources.Load("stylesLevel");
		
		// Set the menu proportions.
		START_POS = Vector2.zero;
		int WIDTH_SCALE = 160;
		
		NUM_ROWS = 5;
		NUM_COLS = 1;
		
		MENU_BOX_WIDTH = MENU_OPTION_WIDTH + MENU_BOX_BUFFER;
		MENU_BOX_HEIGHT = (MENU_OPTION_HEIGHT * NUM_ROWS) + (MENU_OPTION_DELTA * (NUM_ROWS - 1) + (2 * MENU_BOX_BUFFER));
		if(MENU_BOX_WIDTH < Screen.width*0.8F){
			MENU_BOX_WIDTH = Screen.width*0.8F;
		}
		if(MENU_BOX_HEIGHT < Screen.height*0.9F){
			MENU_BOX_HEIGHT = Screen.height*0.9F;
		}
		
		MENU_OPTION_DELTA = 10;
		MENU_BOX_BUFFER = 50;
		MENU_TOP_BUFFER = 20;
		
		MENU_OPTION_WIDTH = (int)(MENU_BOX_WIDTH - MENU_BOX_BUFFER)/NUM_COLS;
		MENU_OPTION_WIDTH -= WIDTH_SCALE;
		MENU_OPTION_HEIGHT = (int)(MENU_BOX_HEIGHT - (MENU_BOX_BUFFER+MENU_BOX_BUFFER) - MENU_OPTION_DELTA)/(NUM_ROWS + 1);
		
		gameObject.GetComponent<SoundManager>();
		soundRef = gameObject.GetComponent<SoundManager>();
		buttons = new Rect[NUM_ROWS];
		
		for(int i = 0; i < NUM_ROWS; i++){
			Rect currRect = new Rect(
					(START_POS.x * 0.125F) + (MENU_BOX_BUFFER * 0.125F) + (MENU_BOX_WIDTH * 0.125F) + (WIDTH_SCALE/2),
					(START_POS.y * 0.5F) + (MENU_BOX_BUFFER * 0.5F) + (MENU_OPTION_HEIGHT * (i + 1)) + (MENU_OPTION_DELTA * (i + 1)),
					MENU_OPTION_WIDTH,
					MENU_OPTION_HEIGHT);
			// Quick fix for Main Menu button in Options menu
			if (i==4) { currRect.y -= 10; }
			buttons[i] = currRect;
		}
		
		CreateGUIBox();
		CreateGUIButtons();
		CreateSliders();
		CreateLabels();
	}

	public override void OnEnable(){
		base.OnEnable();
	}

	public override void CreateGUIBox(){
		menuBoxList.Clear();
		menuBoxList.Add(new GuiBox(new Rect(
			   (START_POS.x * 0.5F) - (MENU_BOX_WIDTH * 0.5F),
			   (START_POS.y * 0.5F) - (MENU_BOX_HEIGHT * 0.5F),
			   MENU_BOX_WIDTH,
			   MENU_BOX_HEIGHT), "Options"));
		//menuBoxList.Add(new GuiBox(buttons[0], "SFX Volume"));
		//menuBoxList.Add(new GuiBox(buttons[2], "Music Volume"));
	}

	public override void CreateGUIButtons(){
		menuButtonList.Clear();
		ButtonToScreenDict.Clear();
		menuButtonList.Add(new GuiButton(buttons[4], "Back", "Button"));
		ButtonToScreenDict.Add(menuButtonList[menuButtonList.Count - 1], (int)ScreenAreas.Pause);
	}

	// Creates the sliders used for volume control.
	public override void CreateSliders(){
		sliderList.Clear();
				sliderList.Add(new GuiSlider(buttons[1], soundRef.sfxVol, 0));
				sliderList.Add(new GuiSlider(buttons[3], soundRef.musicVol, 1));
	}
	
	public override void CreateLabels(){
		menuLabelList.Clear();
		menuLabelList.Add(new GuiLabel(buttons[0], " "));
		menuLabelList.Add(new GuiLabel(buttons[2], " "));
	}
	
	// Updates the position of GUI elements based on screen size.
	public override void Update(){
		base.Update();
		//code to allow you to escape back to previous screen
#if UNITY_STANDALONE
		if(Input.GetKeyDown(KeyCode.Escape)) escDown = true;
		if(escDown && Input.GetKeyUp(KeyCode.Escape))
		{
			escDown = false;
			gameStateManagerRef.GetScreenManager().DisplayScreen(ScreenAreas.Pause);
		}
#endif
	}
}
