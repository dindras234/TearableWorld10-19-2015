// <summary>
/// 
/// FILE: Screen Manager
/// 
/// DESCRIPTION:
/// 	This class is used to manage the different screens within game which 
/// 	are ran during different game states
/// 
/// AUTHORS: TearableWorld Team (Crumpled Up Game Engineers)
/// 			-> John Crocker, Justin Telmo, Tom Dubiner, ... (ADD YOUR NAME HERE!)
/// 
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ScreenAreas{
	// WARNING: Do NOT make any enum here with a number >= 0
	// ONLY NEGATIVE NUMBERS, PLEASE
	// This keeps TearableScreen from getting confused.
	None = -1,
	Quit = -2,
	LevelSelect = -3,
	MainMenu = -4,
	Options = -5,
	Pause = -6,
	NewGame = -7,
	InGame = -8,
	ContinueGame = -9,
	Test = -10,
	BeatLevel = -11
}

public class ScreenManager : MonoBehaviour{
	#region Fields
	/// <summary>
	/// The camera for the game
	/// </summary>
	public Camera gameCamera;

	/// <summary>
	/// The active Tearable screen.
	/// </summary>
	private TearableScreen activeScreen;

	/// <summary>
	/// The current screen area.
	/// </summary>
	private ScreenAreas currentScreenArea = ScreenAreas.None;
	
	// Reference to the GameStateManager. Used to properly return to the main menu from in-game.
	private GameStateManager gameStateManagerRef;

    private DeviceOrientation deviceOrientation;

    private ScreenOrientation screenOrientation;

    private Vector2 screenResolution = new Vector2();

    private Vector2 screenSize = new Vector2();

	#endregion

	#region Methods

	/// <summary>
	/// Use this for initialization
	/// </summary>
    public void Start()
    {
        // Initialize the reference to the GameStateManager so that is actually points to it.
        gameStateManagerRef = gameObject.GetComponent<GameStateManager>();
        // start screen is always the first screen to be displayed so might as well load it here without any fancy footwork
        if (currentScreenArea.Equals(ScreenAreas.None))
        {
            gameObject.AddComponent<MainMenu>();
            activeScreen = gameObject.GetComponent("MainMenu") as MainMenu;
            currentScreenArea = ScreenAreas.MainMenu;
            screenOrientation = ScreenOrientation.Landscape;
            deviceOrientation = DeviceOrientation.LandscapeLeft;

            if (Application.platform.Equals(RuntimePlatform.Android))
            {
                using (AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"),
                      metricsClass = new AndroidJavaClass("android.util.DisplayMetrics"))
                {
                    using (AndroidJavaObject metricsInstance = new AndroidJavaObject("android.util.DisplayMetrics"),
                            activityInstance = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity"),
                            windowManagerInstance = activityInstance.Call<AndroidJavaObject>("getWindowManager"),
                            displayInstance = windowManagerInstance.Call<AndroidJavaObject>("getDefaultDisplay"))
                    {
                        displayInstance.Call("getMetrics", metricsInstance);
                        screenResolution.y = metricsInstance.Get<int>("heightPixels");
                        screenResolution.x = metricsInstance.Get<int>("widthPixels");
                        screenSize.x = Screen.width;
                        screenSize.y = Screen.height;
                        Screen.SetResolution((int)screenResolution.x, (int)screenResolution.y, true);
                        deviceOrientation = Input.deviceOrientation;
                    }
                }
            }

            else
            {
                // Overall screen resolution must be set initially to screen size for UI,
                // then set to screen resolution while in game
                screenSize = new Vector2(Screen.width, Screen.height);
                screenResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
                Screen.SetResolution((int)screenSize.x, (int)screenSize.y, gameStateManagerRef.fullScreen);
            }
        }
    }
            
	/// <summary>
	/// Accessor function to return the active screen
	/// </summary>
	/// <returns></returns>
	public ScreenAreas GetCurrentScreenArea()
    {
		return currentScreenArea;
	}

	public TearableScreen GetCurrentScreen()
    {
		return activeScreen;
	}

	public void SetCurrentScreen(ScreenAreas area)
    {
		if(area.Equals(ScreenAreas.InGame))
        {
			currentScreenArea = area;
		}
		else
        {
			DisplayScreen(area);
		}
	}

    public Vector2 GetScreenSize()
    {
        return screenSize;
    }

    public Vector2 GetScreenResolution()
    {
        return screenResolution;
    }

    public void SetScreenResolution(Vector2 vec)
    {
        screenResolution = vec;
    }

	/// <summary>
	/// Update is called once per frame
	/// </summary>
	public void Update()
    {
        if (Application.platform.Equals(RuntimePlatform.Android))
        {
            if (deviceOrientation != Input.deviceOrientation)
            {
                deviceOrientation = Input.deviceOrientation;
                screenResolution = new Vector2(screenResolution.y, screenResolution.x);
            }
        }

        // screen size can dynamically change at any time via the GUI
        if (screenSize != new Vector2(Screen.width, Screen.height))
        {
            screenSize = new Vector2(Screen.width, Screen.height);
        }

        // not sure if needed but not taking a chance
        if (screenResolution != new Vector2(Screen.currentResolution.width, Screen.currentResolution.height))
        {
            screenResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        }

        if (screenOrientation != Screen.orientation)
            screenOrientation = Screen.orientation;
	}

	/// <summary>
	/// Displays the screen corresponding to the screen with the provided index
	/// </summary>
	/// <param name='screen'>
	/// Screen represents the number index corresponding to which screen out f all game screens needs to be currently displayed.
	/// JOE, Any new screens to be made in the future should be added to the switch statement below
	/// </param>
	public void DisplayScreen(ScreenAreas screenArea){
		// Ensure we're not switching to the same area if we're selecting a level, then the enums associated
		//	with the button are from GameStateManager not ScreenManager.
		if(!screenArea.Equals(currentScreenArea) && !screenArea.Equals(ScreenAreas.None)){
			switch(screenArea){
				// Test?
				case ScreenAreas.Test:
					GameStateManager.statManagerRef.SaveGame(1, 343);
					break;
				
				// Switch to the main menu.
				case ScreenAreas.MainMenu:
					if(gameObject.GetComponent("MainMenu") == null){
						gameObject.AddComponent<MainMenu>();
					}
					activeScreen.enabled = false;
					activeScreen = gameObject.GetComponent("MainMenu") as MainMenu;
					currentScreenArea = ScreenAreas.MainMenu;
					
					if(gameStateManagerRef.isPaused){
						gameStateManagerRef.EnterMainMenu();
					}
					
					break;

				// Switch to the options screen.
				case ScreenAreas.Options:
					if(gameObject.GetComponent("Options") == null){
						gameObject.AddComponent<Options>();
					}
					activeScreen.enabled = false;
					activeScreen = gameObject.GetComponent("Options") as Options;
					currentScreenArea = ScreenAreas.Options;
					break;
				
				// Switch to the level select screen.
				case ScreenAreas.LevelSelect:
					if(gameObject.GetComponent("LevelSelect") == null){
						gameObject.AddComponent<LevelSelect>();
					}
					activeScreen.enabled = false;
					activeScreen = gameObject.GetComponent("LevelSelect") as LevelSelect;
					currentScreenArea = ScreenAreas.LevelSelect;
					break;
				
				// Switch to the pause screen.
				case ScreenAreas.Pause:
					if(gameObject.GetComponent("Pause") == null){
						gameObject.AddComponent<Pause>();
					}
					activeScreen.enabled = false;
					activeScreen = gameObject.GetComponent("Pause") as Pause;
					currentScreenArea = ScreenAreas.Pause;
					break;
				
				// Switch back to the previous in-game screen.
				case ScreenAreas.ContinueGame:
					activeScreen.enabled = false;
					currentScreenArea = ScreenAreas.InGame;
					gameStateManagerRef.ContinueGame();
					break;

				// Switch to the main menu.
				case ScreenAreas.BeatLevel:
					if(gameObject.GetComponent("LevelComplete") == null){
						gameObject.AddComponent<LevelComplete>();
					}
					activeScreen.enabled = false;
					activeScreen = gameObject.GetComponent("LevelComplete") as LevelComplete;
					currentScreenArea = ScreenAreas.BeatLevel;
					break;
				
				// Exit this game. That's terrible.
				case ScreenAreas.Quit:
					Application.Quit();
					break;
				
				// We shouldn't get to this one. EVER.
				default:
					break;
			}

			// Ensure active screen script is enabled since we re-use scripts and make sure this we're not
			//	just continuing after a pause.
			if(!activeScreen.enabled && !screenArea.Equals(ScreenAreas.ContinueGame)){
				activeScreen.enabled = true;
			}
		}
	}
	#endregion
}
