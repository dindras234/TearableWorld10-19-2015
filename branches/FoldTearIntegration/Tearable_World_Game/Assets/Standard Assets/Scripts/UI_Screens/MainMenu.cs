using UnityEngine;
using System.Collections;
using System.Collections.Generic;


class MainMenu : TearableScreen
{
    // menu options to be created
    //Rect newGameRect, selectLevelRect, optionsRect, testRect, quitRect;

    int totalNumOptions = 5;

    // Constructor
    public MainMenu() : base()
    {

    }
	
    // Use this for initialization
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
    /// abstract function that is called on creation
    /// and dynamically if screen size changes
    /// </summary>
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

        menuButtonList.Add(new GuiButton(buttons[0], "New Game", "Button"));
        ButtonToScreenDict.Add(menuButtonList[menuButtonList.Count - 1], (int)ScreenAreas.NewGame);

        menuButtonList.Add(new GuiButton(buttons[1], "Select Level", "Button"));
        ButtonToScreenDict.Add(menuButtonList[menuButtonList.Count - 1], (int)ScreenAreas.LevelSelect);

        menuButtonList.Add(new GuiButton(buttons[2], "Options", "Button"));
        ButtonToScreenDict.Add(menuButtonList[menuButtonList.Count - 1], (int)ScreenAreas.Options);

        menuButtonList.Add(new GuiButton(buttons[3], "Test", "Button"));
        ButtonToScreenDict.Add(menuButtonList[menuButtonList.Count - 1], (int)ScreenAreas.Test);

        menuButtonList.Add(new GuiButton(buttons[4], "Quit", "Button"));
        ButtonToScreenDict.Add(menuButtonList[menuButtonList.Count - 1], (int)ScreenAreas.Quit);
    }

    /// <summary>
    /// abstract function that is called on creation
    /// and dynamically if screen size changes
    /// </summary>
    public override void CreateGUIBox()
    {
        // FILL DEM CONTAINERS (i.e. titles & destinations)
        menuBoxList.Add(new GuiBox(
                            new Rect(
                            START_POS.x / 2f - MENU_BOX_WIDTH / 2f,
                            START_POS.y / 2f - MENU_BOX_HEIGHT / 2f,
                            MENU_BOX_WIDTH,
                            MENU_BOX_HEIGHT), "Tearable World"));
    }

    /// <summary>
    /// Updates the position of GUI elements
    /// based on screen size
    /// </summary>
     public override void  Update()
    {
 	     base.Update();
    }
}
