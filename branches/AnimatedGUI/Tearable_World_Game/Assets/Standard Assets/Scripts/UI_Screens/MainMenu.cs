using UnityEngine;
using System.Collections;
using System.Collections.Generic;


class MainMenu : TearableScreen
{
	
	// menu options to be created
	Rect newGameRect, selectLevelRect, optionsRect, quitRect;
	
	
	//ctor
	public MainMenu() : base()
	{
		
	}
	
	
	// Use this for initialization
    public override void Start ()
	{
		
		totalNumOptions = 0;
	
		newGameRect = new Rect(Screen.width/2f, 
								Screen.height/2f + MENU_OPTION_DELTA * (totalNumOptions + 1), 
								MENU_OPTION_WIDTH, 
								MENU_OPTION_HEIGHT); 
		totalNumOptions++;
		
		selectLevelRect = new Rect(Screen.width/2f, 
									Screen.height/2f + MENU_OPTION_DELTA * (totalNumOptions + 1), 
									MENU_OPTION_WIDTH, 
									MENU_OPTION_HEIGHT); 
		totalNumOptions++;
		
		optionsRect = new Rect(Screen.width/2f, 
								Screen.height/2f + MENU_OPTION_DELTA * (totalNumOptions + 1), 
								MENU_OPTION_WIDTH, 
								MENU_OPTION_HEIGHT);
		
		totalNumOptions++;
		
		quitRect = new Rect(Screen.width/2f, 
								Screen.height/2f + MENU_OPTION_DELTA * (totalNumOptions + 1), 
								MENU_OPTION_WIDTH, 
								MENU_OPTION_HEIGHT);
		
		totalNumOptions++;
		
		// these has to be reset for every screen
		// you don't have to use them but it beats hard coding
		// and helps for readability
		MENU_BOX_WIDTH = MENU_OPTION_WIDTH + MENU_BOX_BUFFER;
		MENU_BOX_HEIGHT = (MENU_OPTION_HEIGHT * totalNumOptions) + 
						  (MENU_OPTION_DELTA * (totalNumOptions - 1)) + MENU_BOX_BUFFER;
		
		
		menuBoxList.Add (new GuiBox(new Rect(Screen.width/2f - MENU_BOX_BUFFER/2f, 
							Screen.height/2f - MENU_BOX_BUFFER/2f, 
							MENU_BOX_WIDTH, 
							MENU_BOX_HEIGHT), "MAIN MENU"));
		
		
		menuButtonList.Add(new GuiButton(newGameRect, "New Game", "Button"));
		ButtonToScreenDict.Add (menuButtonList[menuButtonList.Count - 1], (int)ScreenAreas.NewGame);
		
		menuButtonList.Add(new GuiButton(selectLevelRect, "SelectLevel", "Button"));
		ButtonToScreenDict.Add (menuButtonList[menuButtonList.Count - 1], (int)ScreenAreas.LevelSelect);
		
		menuButtonList.Add(new GuiButton(optionsRect, "Options", "Button"));
		ButtonToScreenDict.Add (menuButtonList[menuButtonList.Count - 1], (int)ScreenAreas.Options);
		
		menuButtonList.Add(new GuiButton(quitRect, "Quit", "Button"));
		ButtonToScreenDict.Add (menuButtonList[menuButtonList.Count - 1], (int)ScreenAreas.Quit);
		
		
		base.Start ();
	}
	
	
	
	void Update () 
	{
		
	}
}
