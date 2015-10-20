/// <summary>
/// 
/// Class: Game state manager
/// 
/// Function:
/// 	This vital class is used to control the game states of TearableWorld.
/// 
/// Author: Tearable World(Crumpled Up Games' Engineers)
/// 		-> John Crocker, Justin Telmo, ... (ADD YOUR NAME HERE!!!)
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class GameStateManager : MonoBehaviour
{
	
	public enum LevelScenes
	{
		UserInterface = 0,
		Level_1 = 1,
		Level_2 = 2,
		Level_3 = 3,
		Level_4 = 4,
		Level_5 = 5,
		Level_6 = 6,
		Level_7 = 7,
		Level_8 = 8,
		Level_9 = 9,
		Level_10 = 10,
		Level_11 = 11,
		Level_12 = 12,
		Level_13 = 13,
		Level_14 = 14,
		Level_15 = 15,
		Level_16 = 16,
		Level_17 = 17,
		Level_18 = 18,
		Level_19 = 19,
		Level_20 = 20
	}
	
	#region Fields
	
	/** ENTIRE CODE BASE SINGLETONS **/
	public static GameStateManager gameStateManagerRef;
	public static ScreenManager screenManagerRef;
	public static InputManager inputManagerRef;
	public static SoundManager soundManagerRef;
	public static StatisticManager statManagerRef;
	public static PlayerManager playerManagerRef;
	
	/// <summary>
	/// The in game flag keeps track of whether or not the player is currently
	/// in the game state
	/// </summary>
	public bool inGame;
	
	/// <summary>
	/// The in UI flag keeps track of whether or not the player is currently
	/// in the UI state of the game (not in game pause UI)
	/// </summary>
	public bool inUI;
	
	/// <summary>
	/// The is paused flag keeps track of whether or not the player is in game
	/// and has paused the game session
	/// </summary>
	public bool isPaused;
	
	#endregion
	
	#region Methods
	
	/// <summary>
	/// Use this for initialization
	/// </summary>
	public void Start () 
	{
		// set screen booleans accordinly
		inUI = true;
		inGame = false;
		isPaused = false;
		
		gameStateManagerRef = gameObject.GetComponent("GameStateManager") as GameStateManager;
		
		// declare singletons
		InitCoreSingletons();
		
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	public void Update () 
	{
	
	}
	
	
	/// <summary>
	/// Enters the state of the game is used to load a given level by it's
	/// associated level number
	/// </summary>
	/// <param name='currentLevel'>
	/// Level represents the level number of the level to load
	/// </param>
	public void EnterGameState(int levelNum)
	{
		// if the Player Manager script hasn't already been
		// added, do so
		if (gameObject.GetComponent("PlayerManager") == null)
		{
			gameObject.AddComponent<PlayerManager>();
			playerManagerRef = gameObject.GetComponent("PlayerManager") as PlayerManager;
		}
		
		if (!Application.loadedLevel.Equals((LevelScenes) levelNum) && Application.levelCount >= levelNum)
		{
			inUI = false;
			inGame = true;
			screenManagerRef.activeScreen.enabled = false;
			screenManagerRef.currentScreenArea = ScreenAreas.InGame;
			DontDestroyOnLoad(gameObject);
			Application.LoadLevel(levelNum);
	
		}
	}
	
	public void EnterMainMenu()
	{
		DestroyObject(gameObject);
		inUI = true;
		inGame = false;
		isPaused = false;
		playerManagerRef.enabled = false;
		Application.LoadLevel((int)LevelScenes.UserInterface);
	}
	
	/// <summary>
	/// Pauses the game when needed/requested by the player.
	/// Right now this only pauses input and sound
	/// </summary>
	public void PauseGame()
	{
		isPaused = true;
		inUI = false;
		inGame = false;
		
		inputManagerRef.enabled = false;
		soundManagerRef.enabled = false;
		
		// time.timescale pausing the in game when set to 0
		Time.timeScale = 0;
		
		screenManagerRef.DisplayScreen(ScreenAreas.Pause);
	}
	/// <summary>
	/// Restarts the level.
	/// </summary>

	public static void RestartLevel()
	{
		
	}
	
	/// <summary>
	/// Continues the game after the 
	/// player paused while in game
	/// </summary>
	public void ContinueGame()
	{
		isPaused = false;
		inUI = false;
		inGame = true;
		
		// time.timescale set to 1 resets the game back to normal speed.
		Time.timeScale = 1;
		
		inputManagerRef.enabled = true;
		soundManagerRef.enabled = true;
	}
	
	/// <summary>
	/// Inits the singletons that are needed
	/// for the initial start up of the game.
	/// We do not call InitPlayerManager here,
	/// we will when we enter the main game space
	/// </summary>
	public void InitCoreSingletons()
	{
		InitSoundMananger();
		InitScreenMananger();
		InitPlayerMananger();
		InitStatisticMananger();
		InitInputMananger();
	}
	
	#region Initialize Managers
	
	/// <summary>
	/// Initialize the sound mananger.
	/// </summary>
	public void InitSoundMananger()
	{
		if (gameObject.GetComponent("SoundManager") == null)
		{
			gameObject.AddComponent<SoundManager>();
			soundManagerRef = gameObject.GetComponent("SoundManager") as SoundManager;
		}
	}
		
	/// <summary>
	/// Initialize the player mananger.
	/// </summary>
	public void InitPlayerMananger()
	{
		if (gameObject.GetComponent("PlayerManager") == null)
		{
			gameObject.AddComponent<PlayerManager>();
			playerManagerRef = gameObject.GetComponent("PlayerManager") as PlayerManager;
		}
	}
		
	/// <summary>
	/// Initialize the input mananger.
	/// </summary>
	public void InitInputMananger()
	{
		if (gameObject.GetComponent("InputManager") == null)
		{
			gameObject.AddComponent<InputManager>();
			inputManagerRef = gameObject.GetComponent("InputManager") as InputManager;
		}	
	}
		
	/// <summary>
	/// Initialize the screen mananger.
	/// </summary>
	public void InitScreenMananger()
	{
		if (gameObject.GetComponent("ScreenManager") == null)
		{
			gameObject.AddComponent<ScreenManager>();
			screenManagerRef = gameObject.GetComponent("ScreenManager") as ScreenManager;
		}	
	}
		
	/// <summary>
	/// Initialize the statistic mananger.
	/// </summary>
	public void InitStatisticMananger()
	{
		if (gameObject.GetComponent("StatisticManager") == null)
		{
			gameObject.AddComponent<StatisticManager>();
			statManagerRef = gameObject.GetComponent("StatisticManager") as StatisticManager;
		}	
	}
	
	
	//End of Initialize Managers region
	#endregion
	
	
	//End of 'Methods' region
	#endregion
	
}
