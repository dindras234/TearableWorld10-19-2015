using UnityEngine;
using System.Collections;

public class LevelSelect : TearableScreen 
{
	private const int NUM_ROWS = 4;
	private const int NUM_COLS = 5;
	private const int LEVEL_OPTION_HEIGHT = 75;
	private const int LEVEL_OPTION_WIDTH = 75;
	
	
	
	private Vector2 START_POS;
	
	public LevelSelect() : base()
	{
		// divide by 12,10 are just arbitrary numbers
		// you can tweak these to your liking Joe
		START_POS = new Vector2(Screen.currentResolution.width / 12,
								Screen.currentResolution.height / 10);
	}
	
	// Use this for initialization
	public override void Start ()
	{
		totalNumOptions = 0;
		
		for(int r = 0; r < NUM_ROWS; r++)
		{
			for(int c = 0; c < NUM_COLS; c++)
			{
				Rect rectangle = new Rect(START_POS.x * (c + 1), 
										  START_POS.y * (r + 1), 
										  LEVEL_OPTION_WIDTH, 
										  LEVEL_OPTION_HEIGHT); 
				
				totalNumOptions++;
				
				menuButtonList.Add(new GuiButton(rectangle, "Level " + totalNumOptions.ToString(), "Button"));
				// JOE, this will obviously need to be changed if
				// we make each level its own scene
				ButtonToScreenDict.Add (menuButtonList[menuButtonList.Count - 1], totalNumOptions);
			}
		}
		
		menuButtonList.Add (new GuiButton(
								new Rect(
								0,
								Screen.currentResolution.height - LEVEL_OPTION_HEIGHT*3,
								LEVEL_OPTION_WIDTH,
								LEVEL_OPTION_HEIGHT), 
								"Back"));
		ButtonToScreenDict.Add (menuButtonList[menuButtonList.Count - 1], 
												(int)ScreenAreas.MainMenu);
		
		
		menuBoxList.Add (new GuiBox(new Rect(
							START_POS.x - MENU_OPTION_DELTA, 
							START_POS.y - MENU_OPTION_DELTA,
							(START_POS.x * NUM_COLS),
							(START_POS.y * NUM_ROWS) + MENU_OPTION_DELTA), 
							"TEARABLE LEVELS")); // the extra delta above
												 // accounts for the space
												 // taken up by the title of 
												 // of the box
		
		base.Start();
	}
					
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
