/*
	Class: GameStateManager
	Function:
		This vital class is used to control the game states of TearableWorld.
	
	Authored by: Tearable World (Crumpled Up Games' Engineers)
		-> Dominic Arcamone, John Crocker, Justin Telmo, Tom Dubiner, Audrey Fabian, Tonton Rue,
 *      -> Douglas Weller, Shawn Hampton, Joseph Allington
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Reflection;

public class GameStateManager : MonoBehaviour{
	public enum LevelScenes{
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
		Level_20 = 20,
		Level_21 = 21,
		Level_22 = 22,
		Level_23 = 23,
		Level_24 = 24,
		Level_25 = 25
	}
	#region Fields
	/** ENTIRE CODE BASE SINGLETONS **/
	public GameStateManager				gameStateManagerRef;
	private ScreenManager				screenManagerRef;
	private InputManager				inputManagerRef;
	private SoundManager				soundManagerRef;
	private StatisticManager			statManagerRef;
	private AnimationManager			animManagerRef;
	private WorldCollision				worldCollisionRef;
	private TouchController				touchControllerRef;

    
    //Stopwatch for keeping track of current level's time.
    private Stopwatch                   currLevelStopwatch;

	// List of all the core managers in the game (GameState, Screen, Input, Sound).
	private List<MonoBehaviour>			CoreManagerList = new List<MonoBehaviour>(),
	// List of all the game only managers in the game (Anim, Stat, Also added WorldCollision and TouchController).
										GameManagerList = new List<MonoBehaviour>();
	
	// The levels beaten in this save profile.
	public bool[]						levelsBeaten = new bool[Enum.GetNames(typeof(LevelScenes)).Length];

	// Set this if you wish to run unity remote on the pc.
	public bool 						unityRemote = false,
	// Toggled within screen manager.
										fullScreen = false,
	// The in game flag keeps track of whether or not the player is currently in the game state.
										inGame = false,
	
	// The in UI flag keeps track of whether or not the player is currently in the UI state of the game (not in game pause UI).
										inUI = false,
	
	// The is paused flag keeps track of whether or not the player is in game and has paused the game session.
										isPaused = false,

	// A boolean that will delete multiple instances of Main Object  if carried over from one scene to the next.
										objectSaved = false;
	
	private bool 						loading = false, begunLoading = false;
	private int 						currLevel;
	
	// A boolean that determines if developer started game with a level and not through UI.
	//	If the player started game with a level and entered UI, then this bool gets set to true.
	public bool							gameStartedFromUI = false,
	// A boolean to complement the above. If this is check to true, then it will disregard menus and assume the game is being
	//	started from within a level.
										startInGame = false,
	// The have played intro animation.
										HavePlayedIntroAnimation = false;

	// Number of game levels, will need to be changed as we pump out more levels.
	public int 					FINAL_LEVEL_NUM = Enum.GetNames(typeof(LevelScenes)).Length;
	
	List<string> 						menuMusicSounds, menuSFXSounds,
										levelMusicSounds, levelSFXSounds;
	GameObject 							mainMenuGUI;	
	#endregion

	#region Methods
	public bool OnMobileDevice(){
	//	if(SystemInfo.deviceType == DeviceType.Handheld) return true;
	//	else return false;
		return (Application.platform.Equals(RuntimePlatform.Android) || unityRemote ||
			Application.platform.Equals(RuntimePlatform.IPhonePlayer));
	}
	
	// Use this for initialization
	public void Start(){
		if(GameObject.FindGameObjectsWithTag("MainObject").Length > 1){
			if(!objectSaved){
				DestroyImmediate(gameObject);
			}
			return;
		}
		else{
			if(!gameObject.activeSelf){
				gameObject.SetActive(true);
			}
		}
		DeclareAudioNames();
		CoreManagerList.Add(gameStateManagerRef);
		CoreManagerList.Add(screenManagerRef);
		CoreManagerList.Add(inputManagerRef);
		CoreManagerList.Add(soundManagerRef);
		
		GameManagerList.Add(animManagerRef);
		GameManagerList.Add(statManagerRef);
		GameManagerList.Add(touchControllerRef);

        currLevelStopwatch = new Stopwatch();

		Application.targetFrameRate = 30;
		// Use this forprinting the current level number and compare with LevelScenes enum
		//UnityEngine.Debug.Log("LEVEL NUM " + Application.loadedLevel.ToString());

		if(Application.loadedLevel.Equals((int)LevelScenes.UserInterface) && !startInGame){
			// Set screen booleans accordingly.
			inUI = true;
			inGame = false;
			isPaused = false;
			gameStartedFromUI = true;
		}
		else{
			inUI = false;
			inGame = true;
			isPaused = false;
			gameStartedFromUI = false;
		}

		// Declare singletons and add script to the MainObject.
		AssignCoreSingletons();
		//menuSounds.Add(
		
		if(!HavePlayedIntroAnimation)
		{
			HavePlayedIntroAnimation = true;
			//EnterIntroScene();
		}
	}
	
	private void DeclareAudioNames(){
		menuMusicSounds = new List<string>();
		levelMusicSounds = new List<string>();
		menuSFXSounds = new List<string>();
		levelSFXSounds = new List<string>();
		menuMusicSounds.Add("menu01");
		levelSFXSounds.Add("jump");
		levelSFXSounds.Add("land");
		levelSFXSounds.Add("deathWoosh");
		levelSFXSounds.Add("DeathWimper");
		levelSFXSounds.Add("PaperRotate");
		levelSFXSounds.Add("paperFold1");
		levelSFXSounds.Add("PaperTear-notchopped");
		levelSFXSounds.Add("TearEffect");
		levelSFXSounds.Add("walk_L");
		levelSFXSounds.Add("walk_R");
		levelSFXSounds.Add("doorOpen");
		levelMusicSounds.Add("RedemptionByAshleyAlyse");
	}
	
	// Update this instance.
	public void Update(){
		if(Input.touchCount > 0) unityRemote = true;
		if(inGame){
			if(Time.timeScale != 1.0F){ // && screenManagerRef.GetCurrentScreenArea() != ScreenAreas.LevelComplete)
				Time.timeScale = 1.0F;
			}

			if(gameObject.GetComponent<Pause>()){
				if(gameObject.GetComponent<Pause>().enabled){
					gameObject.GetComponent<Pause>().enabled = false;
				}
			}

			if(gameObject.GetComponent<MainMenu>()){
				if(gameObject.GetComponent<MainMenu>().enabled){
					gameObject.GetComponent<MainMenu>().enabled = false;
				}
			}
		}
		else if(inUI){
			EnsureGameScriptsDisabled();

			if (screenManagerRef.GetCurrentScreenArea() != ScreenAreas.LevelSelect){
				foreach (GameObject levelButton in GameObject.FindGameObjectsWithTag("levelButton")){
					if (levelButton.GetComponent<GUITexture>()){
						levelButton.GetComponent<GUITexture>().enabled = false;
					}
				}
			}
		}
		/*if(Application.isLoadingLevel){
			Debug.Log(currLevel);
			if(!loading){ Debug.Log("NOT LOADING"); }
		}*/
		if(loading && !begunLoading){
			begunLoading = true;
			//MeshFilter temp = gameObject.AddComponent<MeshFilter>();
			//temp.mesh =
		}
		else if(loading && begunLoading){
			loading = false;
			begunLoading = false;
			/*inGame = true;
			inUI = false;
			isPaused = false;
			objectSaved = true;*/
			DontDestroyOnLoad(gameObject);
			Application.LoadLevel(currLevel);
			//StartCoroutine(PracticeFunc(currLevel));
			//StartCoroutine(LoadingAsyncLevel(currLevel));
		}
		else{
			if(!mainMenuGUI)
			{
				if(Application.loadedLevelName == "Game_Scene")
				{
					GameObject tempObj = GameObject.FindGameObjectWithTag("MainMenuGUI");
					if(tempObj) 
					{
						mainMenuGUI = tempObj;
					}
				}
			}
		}
//        UnityEngine.Debug.Log("Level " + GetCurrLevel() + ": " + currLevelStopwatch.Elapsed);
	}

	public TouchController GetTouchController(){ return touchControllerRef; }
	public InputManager GetInputManager(){ return inputManagerRef; }
	public AnimationManager GetAnimationManager(){ return animManagerRef; }
	public ScreenManager GetScreenManager(){ return screenManagerRef; }
	public SoundManager GetSoundManager(){ return soundManagerRef; }
	public StatisticManager GetStatisticManager(){ return statManagerRef; }
	public WorldCollision GetWorldCollision(){ return worldCollisionRef; }
	
	// Ensures the overall MainObject has the core singletons added and ready to be used (Input, Animation, Screen, Statistic, Sound).
	//	Should only be called ifyou're referencing the MainObject!
	public void EnsureCoreScriptsAdded(){
		if(!gameObject.GetComponent<GameStateManager>()){
			gameObject.AddComponent<GameStateManager>();
			gameObject.GetComponent<GameStateManager>().HavePlayedIntroAnimation = true;
		}
		if(!gameObject.GetComponent<InputManager>()){
			gameObject.AddComponent<InputManager>();
		}
		if(!gameObject.GetComponent<ScreenManager>()){
			gameObject.AddComponent<ScreenManager>();
		}
		if(!gameObject.GetComponent<SoundManager>()){
			gameObject.AddComponent<SoundManager>();
		}
		gameObject.GetComponent<GameStateManager>().enabled = true;
		gameObject.GetComponent<ScreenManager>().enabled = true;
		gameObject.GetComponent<SoundManager>().enabled = true;
	}

	public void EnsureCoreScriptsDisabled(bool reset){
		if(!gameObject.GetComponent<GameStateManager>()){
			gameObject.AddComponent<GameStateManager>();
			gameObject.GetComponent<GameStateManager>().HavePlayedIntroAnimation = true;
		}
		if(!gameObject.GetComponent<InputManager>()){
			gameObject.AddComponent<InputManager>();
		}
		if(!gameObject.GetComponent<ScreenManager>()){
			gameObject.AddComponent<ScreenManager>();
		}
	  
		if(!gameObject.GetComponent<SoundManager>()){
			gameObject.AddComponent<SoundManager>();
		}
		// If resetting scripts between levels, don't disable the main script.
		if(!reset){
			gameObject.GetComponent<GameStateManager>().enabled = false;
		}
		
		gameObject.GetComponent<ScreenManager>().enabled = false;
		gameObject.GetComponent<SoundManager>().enabled = false;
	}

	// Ensures game needed scripts are added to the overall 'MainObject'. Should only be called if you're referencing the MainObject!
	public void EnsureGameScriptsAdded(){
		if(!gameObject.GetComponent<AnimationManager>()){
			gameObject.AddComponent<AnimationManager>();
		}
		if(!gameObject.GetComponent<PlayerManager>()){
			gameObject.AddComponent<PlayerManager>();
		}
		if(!gameObject.GetComponent<TouchController>()){
			gameObject.AddComponent<TouchController>();
		}
		if(!gameObject.GetComponent<WorldCollision>()){
			gameObject.AddComponent<WorldCollision>();
		}
		if(!gameObject.GetComponent<InputManager>()){
			gameObject.AddComponent<InputManager>();
		}
		gameObject.GetComponent<AnimationManager>().enabled = true;
		gameObject.GetComponent<PlayerManager>().enabled = true;
		gameObject.GetComponent<TouchController>().enabled = true;
		gameObject.GetComponent<WorldCollision>().enabled = true;
		gameObject.GetComponent<InputManager>().enabled = true;

		if(!animManagerRef){
			animManagerRef = gameObject.GetComponent<AnimationManager>();
		}
		if(!touchControllerRef){
			touchControllerRef = gameObject.GetComponent<TouchController>();
		}
		if(!worldCollisionRef){
			worldCollisionRef = gameObject.GetComponent<WorldCollision>();
//			UnityEngine.Debug.Log("TRUE");
		}
	}

	// Ensures scripts necessary forgame play are disabled when leaving the gamespace.
	public void EnsureGameScriptsDisabled(){
		if(gameObject.GetComponent<AnimationManager>()){
			gameObject.GetComponent<AnimationManager>().enabled = false;
		}
		if(gameObject.GetComponent<PlayerManager>()){
			gameObject.GetComponent<PlayerManager>().enabled = false;
		}
		if(gameObject.GetComponent<TouchController>()){
			gameObject.GetComponent<TouchController>().enabled = false;
		}
		if(gameObject.GetComponent<WorldCollision>()){
			gameObject.GetComponent<WorldCollision>().enabled = false;
		}
		if(gameObject.GetComponent<InputManager>()){
			gameObject.GetComponent<InputManager>().enabled = false;
		}
	}
	
	// Ensures that a script is added to the overall 'MainObject'.
	public void EnsureScriptAdded(string script){
		if(script == "AnimationManager"){
			if(!gameObject.GetComponent<AnimationManager>()){
				gameObject.AddComponent<AnimationManager>();
			}
			gameObject.GetComponent<AnimationManager>().enabled = true;
		}
		
		else if(script == "MainMenu"){
			if(!gameObject.GetComponent<MainMenu>()){
				gameObject.AddComponent<MainMenu>();
			}
			if(mainMenuGUI) mainMenuGUI.SetActive(true);
			gameObject.GetComponent<MainMenu>().enabled = true;
		}
		else if(script == "LevelComplete"){
			if(!gameObject.GetComponent<LevelComplete>()){
				gameObject.AddComponent<LevelComplete>();
			}
			gameObject.GetComponent<LevelComplete>().enabled = true;
		}
		else if(script == "LevelSelect"){
			if(!gameObject.GetComponent<LevelSelect>()){
				gameObject.AddComponent<LevelSelect>();
			}
			if(mainMenuGUI)	mainMenuGUI.SetActive(false);
			gameObject.GetComponent<LevelSelect>().enabled = true;
		}
		else if(script == "LevelSelectB"){
			if(!gameObject.GetComponent<LevelSelectB>()){
				gameObject.AddComponent<LevelSelectB>();
			}
			if(mainMenuGUI) mainMenuGUI.SetActive(false);
			gameObject.GetComponent<LevelSelectB>().enabled = true;
		}
		else if(script == "LevelSelectC"){
			if(!gameObject.GetComponent<LevelSelectC>()){
				gameObject.AddComponent<LevelSelectC>();
			}
			if(mainMenuGUI) mainMenuGUI.SetActive(false);
			gameObject.GetComponent<LevelSelectC>().enabled = true;
		}
		else if(script == "Options"){
			if(!gameObject.GetComponent<Options>()){
				gameObject.AddComponent<Options>();
			}
			if(mainMenuGUI) mainMenuGUI.SetActive(false);
			gameObject.GetComponent<Options>().enabled = true;
		}
		else if(script == "OptionsInGame"){
			if(!gameObject.GetComponent<OptionsInGame>()){
				gameObject.AddComponent<OptionsInGame>();
			}
			gameObject.GetComponent<OptionsInGame>().enabled = true;
		}
		else if(script == "Pause"){
			if(!gameObject.GetComponent<Pause>()){
				gameObject.AddComponent<Pause>();
			}
			gameObject.GetComponent<Pause>().enabled = true;
		}

		else if(script == "TouchController"){
			if(!gameObject.GetComponent<TouchController>()){
				gameObject.AddComponent<TouchController>();
			}
			gameObject.GetComponent<TouchController>().enabled = true;
		}

		else if(script == "WorldCollision"){
			if(!gameObject.GetComponent<WorldCollision>()){
				gameObject.AddComponent<WorldCollision>();
			}
			gameObject.GetComponent<WorldCollision>().enabled = true;
		}
	}

	// Ensures any script on the MainObject that is no longer needed is disabled.
	public void EnsureScriptDisabled(string script){
		if(gameObject.GetComponent(script.GetType())){
			gameObject.AddComponent(script.GetType());
		}
	}
	
	// Enters the state of the game is used to load a given level by it's associated level number.
	public void EnterGameState(int levelNum){
		//UnityEngine.Debug.Log("LEVEL WON");

		// If entering a level from within another level reset main scripts.
		if(!Application.loadedLevel.Equals((int)LevelScenes.UserInterface)){
			//Debug.Log("RESET");
			ResetManagers();
		}

		// If the Player Manager script hasn't already been added, do so.
		EnsureCoreScriptsAdded();
		EnsureGameScriptsAdded();
		
		
		// This will need to be changed when we make more levels.
		if(Application.loadedLevel.Equals(FINAL_LEVEL_NUM) && !isPaused){
			//UnityEngine.Debug.Log("BALLSSSS");
			EnterMainMenu();
		}
		// Ensuring that the level to be loaded is valid and not the current level unless restarting
		else if(Application.levelCount >= levelNum){
			if(Application.levelCount != levelNum){
				Screen.SetResolution((int)(Screen.currentResolution.width*0.9F),
						(int)(Screen.currentResolution.height*0.9F), gameStateManagerRef.fullScreen);
				screenManagerRef.SetScreenResolution(new Vector2(Screen.currentResolution.width, Screen.currentResolution.height));
				if(soundManagerRef.IsAudioPlaying("menu01", "MUSIC")){
					soundManagerRef.StopSound("menu01", "MUSIC");
					soundManagerRef.LoadAudioAssets(levelMusicSounds, levelSFXSounds);
					soundManagerRef.PlayAudio("RedemptionByAshleyAlyse", "MUSIC");
				}
			}

			screenManagerRef.GetCurrentScreen().enabled = false;
			screenManagerRef.SetCurrentScreen(ScreenAreas.InGame);
			screenManagerRef.DisplayScreen(ScreenAreas.ContinueGame);
			
			inUI = false;
			inGame = true;
			isPaused = false;
			objectSaved = true;
			DontDestroyOnLoad(gameObject);
			//Application.LoadLevel(levelNum);
			LoadToLevel(levelNum);
		}
	}
	
	// Enters the intro scene -> J.C.
	public void EnterIntroScene(){
		UnityEngine.Debug.Log("currently EnterIntroScene");
		HavePlayedIntroAnimation = true;
		gameStateManagerRef.HavePlayedIntroAnimation = true;
		Application.LoadLevel(19);
	}

	public void EnterMainMenu(){
		// Needs to get reset when entering UI so all functionality returns to normal.
		gameStartedFromUI = true;
		Screen.SetResolution(Screen.width, Screen.height, gameStateManagerRef.fullScreen);
		screenManagerRef.SetScreenResolution(new Vector2(Screen.width, Screen.height));
		//Debug.Log("CHEKC");
		soundManagerRef.LoadAudioAssets(menuMusicSounds, menuSFXSounds);
		soundManagerRef.StopAll();
		soundManagerRef.PlayAudio("menu01", "MUSIC");
		DontDestroyOnLoad(gameObject);
		inUI = true;
		inGame = false;
		isPaused = false;
		screenManagerRef.GetCurrentScreen().enabled = false;
		screenManagerRef.SetCurrentScreen(ScreenAreas.MainMenu);
		
		Application.LoadLevel((int)LevelScenes.UserInterface);
	}
	
	public bool ForceOnlyOneIntro = false;
	
	// Resets managers manually by calling their Start functions. Can't find a more elegant/efficient way of doing this.
	public void ResetManagers(){
		screenManagerRef = null;
		inputManagerRef = null;
		soundManagerRef = null;
		animManagerRef = null;

		// Disable scripts and then re-add them which automatically calls their Start().
		EnsureCoreScriptsDisabled(true);
		EnsureCoreScriptsAdded();
		AssignCoreSingletons();
	}
	
	// Pauses the game when needed/requested by the player. Right now this only pauses input and sound.
	public void PauseGame(){
		isPaused = true;
		inUI = false;
		inGame = false;

		inputManagerRef.enabled = false;
		soundManagerRef.enabled = false;
		animManagerRef.enabled = false;
		// time.timescale pausing the in game when set to 0
		Time.timeScale = 0;
	  
		screenManagerRef.DisplayScreen(ScreenAreas.Pause);
	}
	
	// Continues the game after the player paused while in game
	public void ContinueGame(){
		isPaused = false;
		inUI = false;
		inGame = true;
		
		// time.timescale set to 1 resets the game back to normal speed.
		Time.timeScale = 1;

		inputManagerRef.enabled = true;
		soundManagerRef.enabled = true;
		animManagerRef.enabled = true;
	}
	
	// Function is called from within LevelComplete's ONGUI() ethod and updates all managers as necessary. Right now the only
	//	thing needed to do is call StatManager to actually save the score.
	public void LevelWon(int score, string name){

        currLevelStopwatch.Stop();
		//if(score >= statManagerRef.LevelScoresDict[Application.loadedLevel]){
			statManagerRef.SaveGame(Application.loadedLevel, name, score);
			levelsBeaten[Application.loadedLevel] = true;
		//}eStateManagerRef.curr

		if(Application.loadedLevel > gameStateManagerRef.FINAL_LEVEL_NUM){
			EnterMainMenu();
		}
		else{
			EnterGameState(Application.loadedLevel + 1);
		}
	}
	
	
	// Initializes the singletons that are needed for the initial start up of the game. We do not call InitPlayerManager here,
	//	we will when we enter the main game space.
	public void AssignCoreSingletons(){
		AssignGameStateManager();
		AssignSoundMananger();
		AssignScreenMananger();
		AssignStatisticMananger();
		AssignInputMananger();
		AssignAnimationManager();
		AssignTouchController();
	}
	
	#region Initialize Managers
	public void AssignGameStateManager(){
		if(!gameObject.GetComponent<GameStateManager>()){
			gameObject.AddComponent<GameStateManager>();
			gameObject.GetComponent<GameStateManager>().HavePlayedIntroAnimation = true;
		}
		gameStateManagerRef = gameObject.GetComponent<GameStateManager>();
		gameStateManagerRef.HavePlayedIntroAnimation = false;
	}


	public void AssignTouchController(){
		if(!gameObject.GetComponent<TouchController>()){
			gameObject.AddComponent<TouchController>();
		}
		touchControllerRef = gameObject.GetComponent<TouchController>();
	}
	
	
	// Initialize the sound mananger.
	public void AssignSoundMananger(){
		if(gameObject.GetComponent("SoundManager") == null){
		gameObject.AddComponent<SoundManager>();
		soundManagerRef = gameObject.GetComponent("SoundManager") as SoundManager;
		soundManagerRef.LoadAudioAssets(menuMusicSounds, menuSFXSounds);
		soundManagerRef.PlayAudio("menu01", "MUSIC");
		}
		soundManagerRef = gameObject.GetComponent("SoundManager") as SoundManager;

	}
	// Initialize the input mananger.
	public void AssignInputMananger(){
		if(gameObject.GetComponent("InputManager") == null){
			gameObject.AddComponent<InputManager>();
		}

		inputManagerRef = gameObject.GetComponent("InputManager") as InputManager;
	}
	
	// Initialize the screen mananger.
	public void AssignScreenMananger(){
		if(gameObject.GetComponent("ScreenManager") == null){
			gameObject.AddComponent<ScreenManager>();
		}

		screenManagerRef = gameObject.GetComponent("ScreenManager") as ScreenManager;
	}
	
	// Initialize the statistic mananger.
	public void AssignStatisticMananger(){
		// Stat ref should only assigned once.
		if(gameObject.GetComponent("StatisticManager") == null){
			gameObject.AddComponent<StatisticManager>();
		}
		
		statManagerRef = gameObject.GetComponent("StatisticManager") as StatisticManager;
	}
	
	public void AssignAnimationManager(){
		if(gameObject.GetComponent ("AnimationManager") == null){
			gameObject.AddComponent<AnimationManager>();
		}

		animManagerRef = gameObject.GetComponent("AnimationManager") as AnimationManager;
	}
	#endregion
	#endregion
	
	public void LoadToLevel(int level){

		loading = true;
		currLevel = level;
        currLevelStopwatch.Start();
		//Application.LoadLevelAsync(level);
		//Application.LoadLevel(currLevel);
		
		Application.LoadLevel ("Loading_Level");
	}


    //Wrapper function for accessing the current level number
    //outside of GameStateManager. - J.T.
    public int GetCurrLevel()
    {
        return currLevel;
    }

}
