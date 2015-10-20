using UnityEngine;
using System.Collections;

public class LevelSelect : TearableScreen
{
    /*
    // FULL LEVEL SELECT
	
    private const int NUM_ROWS = 4;
    private const int NUM_COLS = 5;
    private const int LEVEL_OPTION_HEIGHT = 75;
    private const int LEVEL_OPTION_WIDTH = 75;
	
	
	
    private Vector2 START_POS;
	
    public LevelSelect() : base()
    {
        // divide by 12,10 are just arbitrary numbers
        // you can tweak these to your liking Joe
        START_POS = new Vector2(START_POS.x / 12,
                                START_POS.y / 10);
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
                                0, //START_POS.y - LEVEL_OPTION_HEIGHT*3,
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
    */

    int totalNumOptions = 4;

    public override void Start()
    {
        // START
        base.Start();

        // SET UP CONTAINERS (i.e. boxes & buttons)
        MENU_BOX_WIDTH = MENU_OPTION_WIDTH + MENU_BOX_BUFFER;
        MENU_BOX_HEIGHT = (MENU_OPTION_HEIGHT * totalNumOptions) + (MENU_OPTION_DELTA * (totalNumOptions - 1) + (2 * MENU_BOX_BUFFER));


        CreateGUIBox();
        CreateGUIButtons();

    }

    /// <summary>
    /// Updates the position of GUI elements
    /// based on screen size
    /// </summary>
    public override void Update()
    {
        base.Update();
    }

    public override void CreateGUIBox()
    {
        menuBoxList.Add(new GuiBox(
                           new Rect(
                           START_POS.x / 2f - MENU_BOX_WIDTH / 2f,
                           START_POS.y / 2f - MENU_BOX_HEIGHT / 2f,
                           MENU_BOX_WIDTH,
                           MENU_BOX_HEIGHT), "Level Select"));
    }

    public override void CreateGUIButtons()
    {
        Rect[] buttons = new Rect[totalNumOptions];

        for (int i = 0; i < totalNumOptions; i++)
        {
            Rect currRect = new Rect(START_POS.x / 2f + MENU_BOX_BUFFER / 2f - MENU_BOX_WIDTH / 2f,
                START_POS.y / 2f + MENU_BOX_BUFFER / 2f - MENU_BOX_HEIGHT / 2f + (MENU_OPTION_HEIGHT * (i + 1)) + (MENU_OPTION_DELTA * (i + 1)),
                MENU_OPTION_WIDTH,
                MENU_OPTION_HEIGHT);
            buttons[i] = currRect;
        }

        menuButtonList.Add(new GuiButton(buttons[0], "Level One", "Button"));
        ButtonToScreenDict.Add(menuButtonList[menuButtonList.Count - 1], 1);

        menuButtonList.Add(new GuiButton(buttons[1], "Level Two", "Button"));
        ButtonToScreenDict.Add(menuButtonList[menuButtonList.Count - 1], 2);

        menuButtonList.Add(new GuiButton(buttons[2], "Level Three", "Button"));
        ButtonToScreenDict.Add(menuButtonList[menuButtonList.Count - 1], 3);

        menuButtonList.Add(new GuiButton(buttons[3], "Main Menu", "Button"));
        ButtonToScreenDict.Add(menuButtonList[menuButtonList.Count - 1], (int)ScreenAreas.MainMenu);
    }

}
