/* 
	FILE: ScreenManager
	
	DESCRIPTION:
		This class is used to manage the different screens within game which are ran during different game states.
	
	AUTHORS: TearableWorld Team (Crumpled Up Game Engineers)
		-> John Crocker, Justin Telmo, Tom Dubiner ... (ADD YOUR NAME HERE!)
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ScreenAreas{
	// WARNING: Do NOT make any enum here with a number >= 0
	// ONLY NEGATIVE NUMBERS, PLEASE.
	// This keeps TearableScreen from getting confused.
	None = -1,
	Quit = -2,
	// Level Select has multiple values because there are multiple pages of levels.
	LevelSelect = -3,
	LevelSelectB = -4,
	LevelSelectC = -5,
	
	MainMenu = -6,
	Options = -7,
	OptionsInGame = -8,
	Pause = -9,
	NewGame = -10,
	InGame = -11,
	ContinueGame = -12,
	Test = -13,
	LevelComplete = -14,
	RestartLevel = -15
}

public class ScreenManager : MonoBehaviour{
	#region Fields
	// The camera for the game.
	public Camera						gameCamera;

	// The active Tearable screen.
	private TearableScreen				activeScreen;

	// The current screen area.
	private ScreenAreas					currentScreenArea = ScreenAreas.None;
	
	// Reference to the GameStateManager. Used to properly return to the main menu from in-game.
	private GameStateManager			gameStateManagerRef;

	private StatisticManager			statManagerRef;

	private DeviceOrientation			deviceOrientation;

	private ScreenOrientation			screenOrientation;

	private Vector2						screenResolution = new Vector2(),
										screenSize = new Vector2();

	// Custom GUI to be manipulated.
	public GUIStyle						savedDataGUIStyle = new GUIStyle();
	#endregion

	#region Methods
	// Use this for initialization.
	public void Start(){
		// Ensures all necessary scripts are added for the MainObject
		gameStateManagerRef = gameObject.GetComponent<GameStateManager>();
		gameStateManagerRef.EnsureCoreScriptsAdded();
		gameStateManagerRef.EnsureScriptAdded("MainMenu");
	   
		if(gameStateManagerRef.gameStartedFromUI){
			activeScreen = gameObject.GetComponent("MainMenu") as MainMenu;
			currentScreenArea = ScreenAreas.MainMenu;
		}
		else{
			// This will not be the active screen, but active screen needs to be initialized.
			activeScreen = gameObject.GetComponent("MainMenu") as MainMenu;
			activeScreen.enabled = false;
			currentScreenArea = ScreenAreas.InGame;
		}

		screenOrientation = ScreenOrientation.Landscape;
		deviceOrientation = DeviceOrientation.LandscapeLeft;

		if(Application.platform.Equals(RuntimePlatform.Android)){
			#if UNITY_ANDROID
			using(AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"),
					metricsClass = new AndroidJavaClass("android.util.DisplayMetrics")){
				using(AndroidJavaObject metricsInstance = new AndroidJavaObject("android.util.DisplayMetrics"),
						activityInstance = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity"),
						windowManagerInstance = activityInstance.Call<AndroidJavaObject>("getWindowManager"),
						displayInstance = windowManagerInstance.Call<AndroidJavaObject>("getDefaultDisplay")){
					displayInstance.Call("getMetrics", metricsInstance);
					screenResolution.y = metricsInstance.Get<int>("heightPixels");
					screenResolution.x = metricsInstance.Get<int>("widthPixels");
					screenSize.x = Screen.width;
					screenSize.y = Screen.height;
					Screen.SetResolution((int)screenResolution.x, (int)screenResolution.y, true);
					deviceOrientation = Input.deviceOrientation;
				}
			}
			#endif
		}
		else{
			// Overall screen resolution must be set initially to screen size for UI, then set to screen resolution while in game.
			screenSize = new Vector2(Screen.width, Screen.height);
			screenResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

			if(gameStateManagerRef.inUI){
				Screen.SetResolution((int)screenSize.x, (int)screenSize.y, gameStateManagerRef.fullScreen);
			}

			// Developer started in a level.
			else{
				Screen.SetResolution((int)screenResolution.x, (int)screenResolution.y, gameStateManagerRef.fullScreen);
			}
		}
	}
			
	// Accessor function to return the active screen.
	public ScreenAreas GetCurrentScreenArea(){ return currentScreenArea; }
	public TearableScreen GetCurrentScreen(){ return activeScreen; }
	public Vector2 GetScreenSize(){ return screenSize; }
	public Vector2 GetScreenResolution(){ return screenResolution; }
	
	public void SetCurrentScreen(ScreenAreas area){
		if(area.Equals(ScreenAreas.InGame)){
			currentScreenArea = area;
		}
		else{
			DisplayScreen(area);
		}
	}
	public void SetScreenResolution(Vector2 vec){
		screenResolution = vec;
	}
	
	public bool DeviceOrientationSwitched{
		get;
		set;
	}
	public DeviceOrientation CurrentDeviceOrientation(){
		return deviceOrientation;
	}

	// Should only be called by TWCharacterController when manually flipping orientation of the game.
	public void SetDeviceOrientation(DeviceOrientation orientation){
		deviceOrientation = orientation;
		DeviceOrientationSwitched = true;
		//UnityEngine.Debug.Log("ORIENTATION " + orientation);
	}

	// Update is called once per frame.
	public void Update(){
		if(Application.platform.Equals(RuntimePlatform.Android)){
			if(deviceOrientation != Input.deviceOrientation &&
					(Input.deviceOrientation == DeviceOrientation.LandscapeLeft ||
					Input.deviceOrientation == DeviceOrientation.LandscapeRight ||
					Input.deviceOrientation == DeviceOrientation.Portrait ||
					Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)){
				deviceOrientation = Input.deviceOrientation;
				DeviceOrientationSwitched = true;
			}
			else if(DeviceOrientationSwitched){
				DeviceOrientationSwitched = false;
			}
		}

		// Screen size can dynamically change at any time via the GUI.
		if(screenSize != new Vector2(Screen.width, Screen.height)){
			screenSize = new Vector2(Screen.width, Screen.height);
		}

		// Not sure if needed but not taking a chance.
		if(screenResolution != new Vector2(Screen.currentResolution.width, Screen.currentResolution.height)){
			screenResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
		}

		if(screenOrientation != Screen.orientation){
			screenOrientation = Screen.orientation;
		}
	}

	// Displays the screen corresponding to the screen with the provided index.
	public void DisplayScreen(ScreenAreas screenArea){
		// Ensure we're not switching to the same area if we're selecting a level, then the enums associated
		//	with the button are from GameStateManager not ScreenManager.
		if(!screenArea.Equals(currentScreenArea) && !screenArea.Equals(ScreenAreas.None)){
			switch(screenArea){
				case ScreenAreas.RestartLevel:
					gameStateManagerRef.EnterGameState(Application.loadedLevel);
					break;

				// Switch to the main menu.
				case ScreenAreas.MainMenu:
					gameStateManagerRef.EnsureScriptAdded("MainMenu");
					activeScreen.enabled = false;
					activeScreen = gameObject.GetComponent<MainMenu>();
					currentScreenArea = ScreenAreas.MainMenu;
					
					if(gameStateManagerRef.isPaused){
						gameStateManagerRef.EnterMainMenu();
					}
					
					break;

				// Switch to the options screen.
				case ScreenAreas.Options:
					gameStateManagerRef.EnsureScriptAdded("Options");
					activeScreen.enabled = false;
					activeScreen = gameObject.GetComponent<Options>();
					currentScreenArea = ScreenAreas.Options;
					break;
				case ScreenAreas.OptionsInGame:
					gameStateManagerRef.EnsureScriptAdded("OptionsInGame");
					activeScreen.enabled = false;
					activeScreen = gameObject.GetComponent<OptionsInGame>();
					currentScreenArea = ScreenAreas.OptionsInGame;
					break;
				
				// Switch to the level select screen.
				case ScreenAreas.LevelSelect:
					gameStateManagerRef.EnsureScriptAdded("LevelSelect");
					activeScreen.enabled = false;
					activeScreen = gameObject.GetComponent<LevelSelect>();
					currentScreenArea = ScreenAreas.LevelSelect;
					break;
				// Switch to the second page of the level select screen.
				case ScreenAreas.LevelSelectB:
					gameStateManagerRef.EnsureScriptAdded("LevelSelectB");
					activeScreen.enabled = false;
					activeScreen = gameObject.GetComponent<LevelSelectB>();
					currentScreenArea = ScreenAreas.LevelSelectB;
					break;
				// Switch to the third page of the level select screen.
				case ScreenAreas.LevelSelectC:
					gameStateManagerRef.EnsureScriptAdded("LevelSelectC");
					activeScreen.enabled = false;
					activeScreen = gameObject.GetComponent<LevelSelectC>();
					currentScreenArea = ScreenAreas.LevelSelectC;
					break;
				
				// Switch to the pause screen.
				case ScreenAreas.Pause:
					gameStateManagerRef.EnsureScriptAdded("Pause");
					activeScreen.enabled = false;
					activeScreen = gameObject.GetComponent<Pause>();
					currentScreenArea = ScreenAreas.Pause;
					break;
				
				// Switch back to the previous in-game screen.
				case ScreenAreas.ContinueGame:
					activeScreen.enabled = false;
					currentScreenArea = ScreenAreas.InGame;
					gameStateManagerRef.ContinueGame();
					break;

				// Switch to the main menu.
				case ScreenAreas.LevelComplete:
					Time.timeScale = 0;
					activeScreen.enabled = false;
					gameStateManagerRef.EnsureScriptAdded("LevelComplete");
					activeScreen = gameObject.GetComponent <LevelComplete>();
					currentScreenArea = ScreenAreas.LevelComplete;
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
			if(!activeScreen.enabled && 
					!screenArea.Equals(ScreenAreas.ContinueGame) && 
					!screenArea.Equals(ScreenAreas.RestartLevel) && 
					!screenArea.Equals(ScreenAreas.LevelComplete)){
				activeScreen.enabled = true;
				activeScreen.Start();
			}
		}
	}
	#endregion
}
