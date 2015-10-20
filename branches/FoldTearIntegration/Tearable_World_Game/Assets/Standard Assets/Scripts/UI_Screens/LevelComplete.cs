using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelComplete : TearableScreen
{
    private const int BUTTON_WIDTH = 50;
    private const int BUTTON_HEIGHT = 50;
    private Dictionary<int, char> IntToCharDict;
    private string startString = "High Score";
    private string scoreString = "High Score";
    private const string SHIFT_SYMBOL = "shift";
    private const string DELTE_SYMBOL = "delete";
    private const string ENTER_SYMBOL = "enter";

    /// <summary>
    /// Too lazy to figure out how to parse
    /// an integer to an ascii character so
    /// I just made a dictionary thtat does
    /// the same thing
    /// </summary>
    private void SetupDictionary()
    {
        IntToCharDict = new Dictionary<int, char>();
        IntToCharDict.Add(0, 'A');
        IntToCharDict.Add(1, 'B');
        IntToCharDict.Add(2, 'C');
        IntToCharDict.Add(3, 'D');
        IntToCharDict.Add(4, 'E');
        IntToCharDict.Add(5, 'F');
        IntToCharDict.Add(6, 'G');
        IntToCharDict.Add(7, 'H');
        IntToCharDict.Add(8, 'I');
        IntToCharDict.Add(9, 'J');
        IntToCharDict.Add(10, 'K');
        IntToCharDict.Add(11, 'L');
        IntToCharDict.Add(12, 'M');
        IntToCharDict.Add(13, 'N');
        IntToCharDict.Add(14, 'O');
        IntToCharDict.Add(15, 'P');
        IntToCharDict.Add(16, 'Q');
        IntToCharDict.Add(17, 'R');
        IntToCharDict.Add(18, 'S');
        IntToCharDict.Add(19, 'T');
        IntToCharDict.Add(20, 'U');
        IntToCharDict.Add(21, 'V');
        IntToCharDict.Add(22, 'W');
        IntToCharDict.Add(23, 'X');
        IntToCharDict.Add(24, 'Y');
        IntToCharDict.Add(25, 'Z');
    }

    // Use this for initialization
    void Start()
    {
        base.Start();
        if (IntToCharDict == null)
            SetupDictionary();


        BUTTON_DELTA = new Vector2(60, 60);
        NUM_COLS = 10;
        NUM_ROWS = 3;
        START_POS = new Vector2(START_POS.x / 2f -
                                (((BUTTON_WIDTH + (BUTTON_DELTA.x - BUTTON_WIDTH)) * NUM_COLS) / 2f),
                                START_POS.y / 2f);
        currentNumOption = 0;

        for (int r = 0; r < NUM_ROWS; r++)
        {
            for (int c = (r == NUM_ROWS - 1) ? 2 : 0; c < NUM_COLS; c++)
            {
                Rect rectangle = new Rect(START_POS.x + (BUTTON_DELTA.x * (c)),
                                          START_POS.y + (BUTTON_DELTA.y * (r)),
                                          BUTTON_WIDTH,
                                          BUTTON_HEIGHT);



                menuButtonList.Add(new GuiButton(rectangle, IntToCharDict[currentNumOption].ToString()));
                currentNumOption++;
                ButtonToScreenDict.Add(menuButtonList[menuButtonList.Count - 1], -1);

                // we dont have NUM_ROWS * NUM_COLS (30) keys
                // only 26 for the alphabet, so cut the for loop prematurely
                // and add our unique keys within the structure of keys
                // we already have and the numeric keys underneath that
                if (currentNumOption >= IntToCharDict.Count)
                {
                    AddUniqueKeys(currentNumOption);
                    AddNumberKeys();
                    break;
                }
            }
        }


        // Add our containing box
        menuBoxList.Add(new GuiBox(new Rect(
                                          START_POS.x - MENU_BOX_BUFFER,
                                          START_POS.y - MENU_BOX_BUFFER,
                                          MENU_BOX_BUFFER * 2 + BUTTON_WIDTH + (BUTTON_DELTA.x * (NUM_COLS - 1)),
                                          MENU_BOX_BUFFER * 2 + BUTTON_WIDTH + (BUTTON_DELTA.y * (NUM_ROWS))
                                          ),
                            "KEYBOARD"));

    }


    /// <summary>
    /// Function that adds the special keys that right
    /// now include shift, delte, and enter
    /// </summary>
    /// <param name="currentNumOption"></param>
    private void AddUniqueKeys(int currentNumOption)
    {
        Rect deleteRect = new Rect(START_POS.x,
                                          START_POS.y + (BUTTON_DELTA.y * (NUM_ROWS - 1)),
                                          BUTTON_WIDTH,
                                          BUTTON_HEIGHT);

        Rect shiftRect = new Rect(START_POS.x + BUTTON_DELTA.x,
                              START_POS.y + (BUTTON_DELTA.y * (NUM_ROWS - 1)),
                              BUTTON_WIDTH,
                              BUTTON_HEIGHT);

        Rect enterRect = new Rect(START_POS.x + (BUTTON_DELTA.x * (NUM_COLS - 2) + 5),
                              START_POS.y + (BUTTON_DELTA.y * (NUM_ROWS - 1)),
                              BUTTON_WIDTH * 2,
                              BUTTON_HEIGHT);


        menuButtonList.Add(new GuiButton(deleteRect, DELTE_SYMBOL));
        currentNumOption++;

        menuButtonList.Add(new GuiButton(shiftRect, SHIFT_SYMBOL));
        currentNumOption++;

        menuButtonList.Add(new GuiButton(enterRect, ENTER_SYMBOL));
        currentNumOption++;

        ButtonToScreenDict.Add(menuButtonList[menuButtonList.Count - 1], -1);
    }

    /// <summary>
    /// Adds a row of numeric characters 0 - 9
    /// right beneath the alpha keys
    /// </summary>
    private void AddNumberKeys()
    {
        for (int i = 0; i < 10; ++i)
        {
            Rect rectangle = new Rect(START_POS.x + (BUTTON_DELTA.x * (i)),
                                        START_POS.y + (BUTTON_DELTA.y * NUM_ROWS),
                                        BUTTON_WIDTH,
                                        BUTTON_HEIGHT);

            menuButtonList.Add(new GuiButton(rectangle, i.ToString()));
            currentNumOption++;
            ButtonToScreenDict.Add(menuButtonList[menuButtonList.Count - 1], -1);
        }
    }

    /// <summary>
    /// Updates the position of GUI elements
    /// based on screen size
    /// </summary>
    public override void Update()
    {
        base.Update();
    }

    public override void OnGUI()
    {
        // draw all menu boxes for this screen 
        // typically there should only be one
        for (int i = 0; i < menuBoxList.Count; ++i)
        {
            GUI.Box(menuBoxList[i].rect, menuBoxList[i].str);
        }

        // draw all the menu buttons for this screen
        for (int i = 0; i < menuButtonList.Count; ++i)
        {
            if (GUI.Button(menuButtonList[i].rect, menuButtonList[i].str))
            {
                if (scoreString.Equals("High Score"))
                    scoreString = "";

                switch (menuButtonList[i].str)
                {
                    case DELTE_SYMBOL:
                        if (scoreString.Length > 0)
                            scoreString = scoreString.Substring(0, scoreString.Length - 1);
                        break;

                    // I'll get to this later
                    case SHIFT_SYMBOL:
                        break;

                    case ENTER_SYMBOL:
                        GameStateManager.statManagerRef.SaveGame(1, int.Parse(scoreString));
                        GameStateManager.gameStateManagerRef.EnterMainMenu();
                        break;

                    default:
                        scoreString += menuButtonList[i].str;
                        break;
                }
            }
        }

        scoreString = GUI.TextField(new Rect(
                                   START_POS.x + 50,
                                   START_POS.y - 150,
                                   BUTTON_WIDTH * NUM_COLS,
                                   100),
                                   scoreString,
                                   25);
    }

    public override void CreateGUIBox()
    {
        throw new System.NotImplementedException();
    }

    public override void CreateGUIButtons()
    {
        throw new System.NotImplementedException();
    }
}
