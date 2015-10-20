using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// This is where all the magic happens for every screen
/// this abstract class pretty much does all the logic involved
/// for selecting a menu option and calling ScreenManager's 
/// DisplayScreen function when a new screen is to appear
/// </summary>
public abstract class TearableScreen : MonoBehaviour 
{
	public GUISkin styles = (GUISkin)Resources.Load ("styles");

	/// <summary>
	/// Standard width of any menu option
	/// </summary>
	protected const int MENU_OPTION_WIDTH = 100;
	
	/// <summary>
	/// standard height of any menu option
	/// </summary>
	protected const int MENU_OPTION_HEIGHT = 25;
	
	/// <summary>
	/// Vertical/Horizontal distance between two options to select from4
	/// </summary>
	protected const int MENU_OPTION_DELTA = 45; 
	
	/// <summary>
	/// These are not const since their dimensions
	/// depend on the total number of menu options
	/// to choose from and their corresponding sizes
	/// This will be set in the OnGui function
	/// </summary>
	protected int MENU_BOX_WIDTH, MENU_BOX_HEIGHT;
	
	/// <summary>
	/// Used for both menu box height and width, gives a little
	/// dimension to the box so that icons lie within them
	/// </summary>
	protected const int MENU_BOX_BUFFER = 20;
	
	/// <summary>
	/// Not really needed but helps readability
	/// to determine how big our menu option needs to be
	/// </summary>
	protected int totalNumOptions = 0;
	
	/// <summary>
	/// List of menu gui boxes to be displayed
	/// had to make my own class 'GuiBox' instead of 
	/// of GUI.Box since it is a function that displays a gui element
	/// </summary>
	protected List<GuiBox> menuBoxList = new List<GuiBox>();
	
	/// <summary>
	/// List of menu gui buttons to be displayed
	/// had to make my own class 'GuiButton' instead of 
	/// of GUI.Button since it is a function that displays a gui element
	/// </summary>
	protected List<GuiButton> menuButtonList = new List<GuiButton>();
	
	/// <summary>
	/// Dictionary that maps a gui button to a corresponding screen.  
	/// This should be used to return an int for the DisplayScreen function
	/// in ScreenManager.cs
	/// </summary>
	public Dictionary<GuiButton, int> ButtonToScreenDict = new Dictionary<GuiButton, int>();
	
	/// <summary>
	/// Default ctor to be inherited with every TearableScreen class.
	/// </summary>
	public TearableScreen()
	{
		
	}
	int count = 0;
	/// <summary>
	/// Raises the GUI event. Needs to be implemented
	/// by the active screen.
	/// JOE, we'll need to add logic to accept touch input
	/// these built in GUI boxes don't accept touch input as
	/// a mappable selection so we'll have to map where a finger
	/// touches and check if it lies within the area of a single button
	/// </summary>
	public virtual void OnGUI() 
	{
		GUI.skin = styles;
		
		// draw all menu boxes for this screen 
		// typically there should only be one
		for(int i = 0; i < menuBoxList.Count; ++i)
		{
			GUI.Box(menuBoxList[i].rect, menuBoxList[i].str);
		}
		
		// draw all the menu buttons for this screen
		for(int i = 0; i < menuButtonList.Count; ++i)
		{
			// JOE, this is where we pass in the next screen to be displayed
			if (GUI.Button(menuButtonList[i].rect, menuButtonList[i].str, menuButtonList[i].style))
			{
				
				// If the player wishes to enter the game state 
				// through the level select screen, alert GameStateManager
				if (GameStateManager.screenManagerRef.currentScreenArea.Equals(ScreenAreas.LevelSelect))
				{
					GameStateManager.gameStateManagerRef.EnterGameState(ButtonToScreenDict[menuButtonList[i]]);
				}					
				
				// else if the player chose new game from the main menu
				else if (ButtonToScreenDict[menuButtonList[i]].Equals((int)ScreenAreas.NewGame))
				{
					GameStateManager.gameStateManagerRef.EnterGameState((int)GameStateManager.LevelScenes.Level_1);
				}
				
				// otherwise the player is not attempting to enter the game space
				else
				{
					GameStateManager.screenManagerRef.DisplayScreen((ScreenAreas)ButtonToScreenDict[menuButtonList[i]]);
				}
			}
		}
	}
	
	// Use this for initialization
	public virtual void Start () 
	{

	}
	

	// Update is called once per frame
	void Update () 
	{
		// do not use foreach loops, they slow down performance
		// use for loops instead
	}
} 
// End TearableScreen Abstract class

/** NOTICE: WRAPPER CLASSES BELOW **/

/// <summary>
/// GUI box. Created this class so we 
/// could have a list of GuiBoxes to display
/// </summary>
public class GuiBox
{
	public Rect rect;
	public string str;
	public string style;
	
	public GuiBox(Rect rect, string str)
	{
		this.rect = rect;
		this.str = str;
		this.style = "";
	}

	public GuiBox(Rect rect, string str, string style)
	{
		this.rect = rect;
		this.str = str;
		this.style = style;
	}
}

public class GuiButton
{
	public Rect rect;
	public string str;
	public string style;
	
	public GuiButton(Rect rect, string str)
	{
		this.rect = rect;
		this.str = str;
		this.style = "Button";
	}
	
	public GuiButton(Rect rect, string str, string style)
	{
		this.rect = rect;
		this.str = str;
		this.style = style;
	}
}
