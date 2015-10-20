// <summary>
/// 
/// FILE: Screen Manager
/// 
/// DESCRIPTION:
/// 	This class is used to manage the different screens within game which 
/// 	are ran during different game states
/// 
/// AUTHORS: TearableWorld Team (Crumpled Up Game Engineers)
/// 			-> John Crocker, Justin Telmo, ... (ADD YOUR NAME HERE!)
/// 
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ScreenAreas
{
	None = -2,
	Quit = -1,
	MainMenu = 0,
	LevelSelect = 1,
	Options = 2,
	Pause = 3,
	NewGame = 4,
	InGame = 5,
	ContinueGame = 6
}

public class ScreenManager : MonoBehaviour
{
	#region Fields
	/// <summary>
	/// The camera for the game
	/// </summary>
	public Camera gameCamera;
	
	/// <summary>
	/// The active Tearable screen.
	/// </summary>
	public TearableScreen activeScreen;
	
	/// <summary>
	/// The current screen area.
	/// </summary>
	public ScreenAreas currentScreenArea = ScreenAreas.None;
	
	#endregion
	
	#region Methods

	/// <summary>
	/// Use this for initialization
	/// </summary>
	public void Start () 
	{
		// start screen is always the first screen to 
		// be displayed so might as well load it here without any
		// fancy footwork
		if (currentScreenArea.Equals(ScreenAreas.None))
		{
			gameObject.AddComponent<MainMenu>();
			activeScreen = gameObject.GetComponent("MainMenu") as MainMenu;
			currentScreenArea = ScreenAreas.MainMenu;
		}
	}
	
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	public void Update () 
	{
		//DisplayScreen (currentScreen.index);
		///TODO: Questions;
		///What should a screen do on every update? 
		///Wait for an input? Run an animation?
		
	}
	
		
	
	/// <summary>
	/// Displays the screen corresponding to the screen with the provided
	/// index
	/// </summary>
	/// <param name='screen'>
	/// Screen represents the number index corresponding to which screen out
	/// of all game screens needs to be currently displayed.
	/// JOE, Any new screens to be made in the future should be added to the switch
	/// statement below
	/// </param>
	public void DisplayScreen(ScreenAreas screenArea)
	{
		
		// ensure we're not switching to the same area
		// if we're selecting a level, then the enums associated
		// with the button are from GameStateManager not ScreenManager
		if (!screenArea.Equals(currentScreenArea) && !screenArea.Equals(ScreenAreas.None))
		{
			switch(screenArea)
			{
				case ScreenAreas.MainMenu:
					if (gameObject.GetComponent("MainMenu") == null)
						gameObject.AddComponent<MainMenu>();
					activeScreen.enabled = false;
					activeScreen = gameObject.GetComponent("MainMenu") as MainMenu;
					currentScreenArea = ScreenAreas.MainMenu;
				
					if (GameStateManager.gameStateManagerRef.isPaused)
					{
						GameStateManager.gameStateManagerRef.EnterMainMenu();
					}
				
					break;
				
				case ScreenAreas.Options:
					if (gameObject.GetComponent("Options") == null)
						gameObject.AddComponent<Options>();
					activeScreen.enabled = false;
					activeScreen = gameObject.GetComponent("Options") as Options;
					currentScreenArea = ScreenAreas.Options;
					break;
				
				case ScreenAreas.LevelSelect:
					gameObject.AddComponent<LevelSelect>();
					activeScreen.enabled = false;
					activeScreen = gameObject.GetComponent("LevelSelect") as LevelSelect;
					currentScreenArea = ScreenAreas.LevelSelect;
					break;
				
				case ScreenAreas.Pause:
					if (gameObject.GetComponent("Pause") == null)
						gameObject.AddComponent<Pause>();
					activeScreen.enabled = false;
					activeScreen = gameObject.GetComponent("Pause") as Pause;
					currentScreenArea = ScreenAreas.Pause;
					break;
				
				case ScreenAreas.ContinueGame:
					activeScreen.enabled = false;
		            currentScreenArea = ScreenAreas.InGame;
					GameStateManager.gameStateManagerRef.ContinueGame();
					break;
				
				case ScreenAreas.Quit:
					Application.Quit();
					break;
				
				default:
					break;
				
			}
			
			// ensure active screen script 
			// is enabled since we re-use
			// scripts and make sure this
			// we're not just continuing after
			// a pause
			if (!activeScreen.enabled && !screenArea.Equals(ScreenAreas.ContinueGame))
			{
				activeScreen.enabled = true;
			}
		}
	}
	
	#endregion
	
}
