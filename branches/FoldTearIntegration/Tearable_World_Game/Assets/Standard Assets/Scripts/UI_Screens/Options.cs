using UnityEngine;
using System.Collections;

public class Options : TearableScreen{
    // Use this for initialization

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

    public override void CreateGUIBox()
    {
        menuBoxList.Add(new GuiBox(
                           new Rect(
                           START_POS.x / 2f - MENU_BOX_WIDTH / 2f,
                           START_POS.y / 2f - MENU_BOX_HEIGHT / 2f,
                           MENU_BOX_WIDTH,
                           MENU_BOX_HEIGHT), "Options"));
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

        menuButtonList.Add(new GuiButton(buttons[0], "Master Volume", "Button"));
        ButtonToScreenDict.Add(menuButtonList[menuButtonList.Count - 1], (int)ScreenAreas.None);

        menuButtonList.Add(new GuiButton(buttons[1], "Music Volume", "Button"));
        ButtonToScreenDict.Add(menuButtonList[menuButtonList.Count - 1], (int)ScreenAreas.None);

        menuButtonList.Add(new GuiButton(buttons[2], "Effects Volume", "Button"));
        ButtonToScreenDict.Add(menuButtonList[menuButtonList.Count - 1], (int)ScreenAreas.None);

        menuButtonList.Add(new GuiButton(buttons[3], "Main Menu", "Button"));
        ButtonToScreenDict.Add(menuButtonList[menuButtonList.Count - 1], (int)ScreenAreas.MainMenu);
    }

    /// <summary>
    /// Updates the position of GUI elements
    /// based on screen size
    /// </summary>
    public override void Update()
    {
        base.Update();
    }
}
