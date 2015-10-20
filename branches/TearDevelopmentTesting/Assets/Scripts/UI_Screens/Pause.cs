using UnityEngine;
using System.Collections;

public class Pause : TearableScreen 
{
	private Rect continueRect, mainMenuRect;
	
	// Use this for initialization
	public override void Start ()
	{
		totalNumOptions = 0;
	
		continueRect = new Rect(Screen.width/2f, 
								Screen.height/2f + MENU_OPTION_DELTA * (totalNumOptions + 1), 
								MENU_OPTION_WIDTH, 
								MENU_OPTION_HEIGHT);
		
		totalNumOptions++;
		
		mainMenuRect = new Rect(Screen.width/2f, 
								Screen.height/2f + MENU_OPTION_DELTA * (totalNumOptions + 1), 
								MENU_OPTION_WIDTH, 
								MENU_OPTION_HEIGHT); 
		totalNumOptions++;
		
		MENU_BOX_WIDTH = MENU_OPTION_WIDTH + MENU_BOX_BUFFER;
		MENU_BOX_HEIGHT = (MENU_OPTION_HEIGHT + MENU_OPTION_DELTA) * totalNumOptions;
		
		menuBoxList.Add (new GuiBox(
							new Rect(
							Screen.width/2f - MENU_BOX_BUFFER/2f, 
							Screen.height/2f - MENU_BOX_BUFFER/4f, 
							MENU_BOX_WIDTH, 
							MENU_BOX_HEIGHT), "TEARABLE PAUSE"));
		
		menuButtonList.Add(new GuiButton(mainMenuRect, "Main Menu", "Button"));
		ButtonToScreenDict.Add (menuButtonList[menuButtonList.Count - 1], (int)ScreenAreas.MainMenu);
		
		menuButtonList.Add(new GuiButton(continueRect, "Continue", "Button"));
		ButtonToScreenDict.Add (menuButtonList[menuButtonList.Count - 1], (int)ScreenAreas.ContinueGame);
		
		base.Start();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
