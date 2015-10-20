using UnityEngine;
using System.Collections;

public class Options : TearableScreen 
{
	// menu options to be created
	Rect masterVolumeRect, backgroundVolumeRect, effectsVolumeRect, backRect;
	
	// Use this for initialization
	public override void Start ()
	{
		totalNumOptions = 0;
	
		masterVolumeRect = new Rect(Screen.width/2f, 
								Screen.height/2f + MENU_OPTION_DELTA * (totalNumOptions + 1), 
								MENU_OPTION_WIDTH, 
								MENU_OPTION_HEIGHT);
		
		totalNumOptions++;
		
		backgroundVolumeRect = new Rect(Screen.width/2f, 
								Screen.height/2f + MENU_OPTION_DELTA * (totalNumOptions + 1), 
								MENU_OPTION_WIDTH, 
								MENU_OPTION_HEIGHT); 
		totalNumOptions++;
	
		effectsVolumeRect = new Rect(Screen.width/2f, 
								Screen.height/2f + MENU_OPTION_DELTA * (totalNumOptions + 1), 
								MENU_OPTION_WIDTH, 
								MENU_OPTION_HEIGHT); 
		totalNumOptions++;
		
		backRect = new Rect(Screen.width/2f, 
								Screen.height/2f + MENU_OPTION_DELTA * (totalNumOptions + 1), 
								MENU_OPTION_WIDTH, 
								MENU_OPTION_HEIGHT); 
		totalNumOptions++;
		
		MENU_BOX_WIDTH = MENU_OPTION_WIDTH + MENU_BOX_BUFFER;
		MENU_BOX_HEIGHT = (MENU_OPTION_HEIGHT * totalNumOptions) + 
						  (MENU_OPTION_DELTA * (totalNumOptions - 1));
		
		menuBoxList.Add (new GuiBox(
							new Rect(
							Screen.width/2f - MENU_BOX_BUFFER/2f, 
							Screen.height/2f - MENU_BOX_BUFFER/2f, 
							MENU_BOX_WIDTH, 
							MENU_BOX_HEIGHT), "OPTIONS MENU"));
		
		menuButtonList.Add(new GuiButton(masterVolumeRect, "Master Volume", "Button"));
		ButtonToScreenDict.Add (menuButtonList[menuButtonList.Count - 1], (int)ScreenAreas.None);
		
		menuButtonList.Add(new GuiButton(backgroundVolumeRect, "Background Volume", "Button"));
		ButtonToScreenDict.Add (menuButtonList[menuButtonList.Count - 1], (int)ScreenAreas.None);
		
		menuButtonList.Add(new GuiButton(effectsVolumeRect, "Effects Volume", "Button"));
		ButtonToScreenDict.Add (menuButtonList[menuButtonList.Count - 1], (int)ScreenAreas.None);
		
		menuButtonList.Add(new GuiButton(backRect, "Main Menu", "Button"));
		ButtonToScreenDict.Add (menuButtonList[menuButtonList.Count - 1], (int)ScreenAreas.MainMenu);
		
		base.Start();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
