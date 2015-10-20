using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
	This is where all the magic happens for every screen this abstract class pretty much does all the logic involved
		for selecting a menu option and calling ScreenManager's DisplayScreen function when a new screen is to appear.
*/
public abstract class TearableScreen : MonoBehaviour{
	public GUISkin						styles = (GUISkin)Resources.Load("styles");
	public GUISkin						styles_level = (GUISkin)Resources.Load ("stylesLevel");

	// The start position of each menu.
	protected Vector2					START_POS = Vector2.zero,
	// The buffer distance between each button.
										BUTTON_DELTA = Vector2.zero;

	// Standard width and height of any menu option.
	protected int 						MENU_OPTION_WIDTH = 200,
										MENU_OPTION_HEIGHT = 25,
	// Number of rows and columns for this menu. Needs to be overridden by child class.
				 						NUM_ROWS = 0, NUM_COLS = 0,
	// Vertical/Horizontal distance between two options to select from.
										MENU_OPTION_DELTA = 20;

	// These are not const since their dimensions depend on the total number of menu options to choose from and their
	//	corresponding sizes. This will be set in the OnGui function.
	protected float						MENU_BOX_WIDTH = 0,
										MENU_BOX_HEIGHT = 0,
	// Used for both menu box height and width, gives a little dimension to the box so that icons lie within them.
										MENU_BOX_BUFFER = 50,
										MENU_TOP_BUFFER = 10;

	// Not really needed but helps readability to determine how big our menu option needs to be.
	protected int						currentNumOption = 0;

	// List of menu gui boxes to be displayed had to make my own class 'GuiBox' instead of of GUI.Box since it is a
	//	function that displays a gui element
	protected List<GuiBox>				menuBoxList = new List<GuiBox>();

	// List of menu gui buttons to be displayed had to make my own class 'GuiButton' instead of of GUI.Button since
	//	it is a function that displays a gui element.
	protected List<GuiButton>			menuButtonList = new List<GuiButton>();

	// List of GUI Volume Sliders to be displayed whenever the player opens a screen that contains them.
	protected List<GuiSlider>			sliderList = new List<GuiSlider>();

	protected List<GuiLabel>			menuLabelList = new List<GuiLabel>();
	
	// Dictionary that maps a gui button to a corresponding screen. This should be used to return an int for the
	//	DisplayScreen function in ScreenManager.cs.
	public Dictionary<GuiButton, int>	ButtonToScreenDict = new Dictionary<GuiButton, int>();

	// Internal reference for screen size that needs to be constantly checked against actual screen size. If the
	//	two are different then we need to dynamically change each GUI position since their position is determined
	//	from the screen size.
	private Vector2						currentScreenSize = new Vector2();

	// A boolean that determines if screen size has changed in order to reposition GUI elements.
	private bool						screenSizeChange = false;

	public GameStateManager				gameStateManagerRef;
	public float						menuTargetHeight,
										menuTargetDelay = 12F;  // Larger numbers mean slower expansion of submenus!
	public List<GuiButton>				toAdd = new List<GuiButton>();
	public GuiButton					root;
	public bool							resizing = false;
	public int							coinCountFontSize = 14;

	public GuiButton					testButton;
	
	int									count = 0;
	
	// Default ctor to be inherited with every TearableScreen class.
	public TearableScreen(){}

	// Raises the GUI event. Needs to be implemented by the active screen.
	public virtual void OnGUI(){
		GUI.skin = styles;

		if(screenSizeChange && !gameStateManagerRef.inGame){
			menuBoxList.Clear();
			menuButtonList.Clear();
			CreateGUIBox();
			CreateGUIButtons();
			CreateSliders();
		}
		
		// Draw all menu boxes for this screen typically there should only be one.
		for(int i = 0; i < menuBoxList.Count; ++i){
			GUI.Box(menuBoxList[i].rect, menuBoxList[i].str);
		}

		// Draw all the menu buttons for this screen.
		for(int i = 0; i < menuButtonList.Count; ++i){
			GuiButton currButton = menuButtonList[i];
			// JOE, this is where we pass in the next screen to be displayed
			if(GUI.Button(currButton.rect, currButton.str, currButton.style)){
				// Load level!
				if(ButtonToScreenDict[currButton] >= 0){
					//Debug.Log("Ha!");
					if (!gameStateManagerRef.inGame){
						gameStateManagerRef.EnterGameState(ButtonToScreenDict[currButton]);
					}
				}
				// Else if the player chose new game from the main menu
				else if(ButtonToScreenDict[currButton].Equals((int)ScreenAreas.NewGame)){
				   gameStateManagerRef.EnterGameState((int)GameStateManager.LevelScenes.Level_1);
				}
				// Otherwise the player is not attempting to enter the game space
				else{
					gameStateManagerRef.GetScreenManager().DisplayScreen((ScreenAreas)ButtonToScreenDict[currButton]);
					//Debug.Log("Secret Number: " + ButtonToScreenDict[currButton]);
				}
			}
		}
		// Draw all menu labels for this screen
		for(int i = 0; i < sliderList.Count; ++i){
		   sliderList[i].hSliderValue = GUI.HorizontalSlider(sliderList[i].rect, sliderList[i].hSliderValue, 0.0f, 1.0f);
			if(sliderList[i].changingMusic){
				GUI.skin.horizontalSliderThumb.normal.background = (Texture2D)Resources.Load ("2DImages/UI/slider_handle_2");
				GUI.skin.horizontalSliderThumb.hover.background = (Texture2D)Resources.Load ("2DImages/UI/slider_handle_2");
				GUI.skin.horizontalSliderThumb.active.background = (Texture2D)Resources.Load ("2DImages/UI/slider_handle_2");
				gameStateManagerRef.GetSoundManager().SetMusicVol(sliderList[i].hSliderValue);
			}
			else if(sliderList[i].changingSFX){
				GUI.skin.horizontalSliderThumb.normal.background = (Texture2D)Resources.Load ("2DImages/UI/slider_handle");
				GUI.skin.horizontalSliderThumb.hover.background = (Texture2D)Resources.Load ("2DImages/UI/slider_handle");
				GUI.skin.horizontalSliderThumb.active.background = (Texture2D)Resources.Load ("2DImages/UI/slider_handle");
				gameStateManagerRef.GetSoundManager().SetSFXVol(sliderList[i].hSliderValue);
			}
		}
		// Draw all menu labels for this screen
		for(int i = 0; i < menuLabelList.Count; ++i){
			GUI.Label(menuLabelList[i].rect, menuLabelList[i].str);
		}
		if(screenSizeChange){
			screenSizeChange = false;
		}
	}

	public virtual void OnEnable(){
		if(!gameStateManagerRef){
			GameObject mainObject = GameObject.FindGameObjectsWithTag("MainObject")[0];

			if(GameObject.FindGameObjectsWithTag("MainObject").Length > 1){
				GameObject[] mainObjectList = GameObject.FindGameObjectsWithTag("MainObject");
				for(int i = 0; i < mainObjectList.Length; ++i){
					if(mainObjectList[i].GetComponent<GameStateManager>().objectSaved){
						mainObject = mainObjectList[i];
					}
				}
			}

			gameStateManagerRef = mainObject.GetComponent<GameStateManager>();
		}
	}

	// Use this for initialization
	public virtual void Start(){
		GameObject mainObject = GameObject.FindGameObjectsWithTag("MainObject")[0];

		if(GameObject.FindGameObjectsWithTag("MainObject").Length > 1){
			GameObject[] mainObjectList = GameObject.FindGameObjectsWithTag("MainObject");
			for(int i = 0; i < mainObjectList.Length; ++i){
				if(mainObjectList[i].GetComponent<GameStateManager>().objectSaved){
					mainObject = mainObjectList[i];
				}
			}
		}

		gameStateManagerRef = mainObject.GetComponent<GameStateManager>();
		gameStateManagerRef.EnsureCoreScriptsAdded();
		
		// GUI elements depend on screen size, NOT RESOLUTION
		START_POS = gameStateManagerRef.GetScreenManager().GetScreenSize();
	}

	// Update is called once per frame
	public virtual void Update(){
		if(currentScreenSize != new Vector2(Screen.width, Screen.height))
		{
			// GUI elements depend on screen size
			START_POS = gameStateManagerRef.GetScreenManager().GetScreenSize();
			currentScreenSize = new Vector2(Screen.width, Screen.height);
			screenSizeChange = true;
		}
	}

	// Method that needs to be overriden if the screen size dynamically changes.
	public abstract void CreateGUIButtons();

	// Method that needs to be overriden if the screen size dynamically changes.
	public abstract void CreateGUIBox();

	// Method that needs to be overriden if the screen size dynamically changes.
	public abstract void CreateSliders();
	
	public abstract void CreateLabels();
}
// End TearableScreen Abstract class

/** NOTICE: WRAPPER CLASSES BELOW **/

// GUI box. Created this class so we could have a list of GuiBoxes to display.
public class GuiBox{
	public Rect rect;
	public string str;
	public string style;

	public GuiBox(Rect rect, string str){
		this.rect = rect;
		this.str = str;
		this.style = "";
	}

	public GuiBox(Rect rect, string str, string style){
		this.rect = rect;
		this.str = str;
		this.style = style;
	}
}

public class GuiButton{
	public Rect rect;
	public string str;
	public string style;

	public GuiButton(Rect rect, string str){
		this.rect = rect;
		this.str = str;
		this.style = "Button";
	}

	public GuiButton(Rect rect, string str, string style){
		this.rect = rect;
		this.str = str;
		this.style = style;
	}
}

public class GuiSlider{
	public Rect rect;
	public float hSliderValue;
	public string style;
	public bool changingMusic;
	public bool changingSFX;
	
	// Initializes a new instance of the <see cref="GuiSlider"/> class.
	/* Input:
		'rect': The rectangle this slider will occupy.
		'hSliderValue': The maximum value for this slider.
		'affecting': 1 is for music, 0 is for SFX, anything after that can be for other things we may want to change.
	*/
	public GuiSlider(Rect rect, float hSliderValue, int affecting){
		changingSFX = false;
		changingMusic = false;
		this.rect = rect;
		this.hSliderValue = hSliderValue;
		this.style = "Button";
		if(affecting == 0){
			changingSFX = true;
		}
		else if(affecting == 1){
			changingMusic = true;
		}
	}

	public GuiSlider(Rect rect, float hSliderValue, string style, int affecting){
		changingSFX = false;
		changingMusic = false;
		this.rect = rect;
		this.hSliderValue = hSliderValue;
		this.style = style;
		if(affecting == 0){
			changingSFX = true;
		}
		else if(affecting == 1){
			changingMusic = true;
		}
	}

	/*public void OnGUI(){
		hSliderValue = GUI.HorizontalSlider(rect, hSliderValue, 0.0F, 1.0F);
	}*/
}

public class GuiLabel{
	public Rect rect;
	public string str;
	public string style;
	public GuiLabel(Rect rect, string str){
		this.rect = rect;
		this.str = str;
		this.style = "Button";
	}
	public GuiLabel(Rect rect, string str, string style){
		this.rect = rect;
		this.str = str;
		this.style = style;
	}
}