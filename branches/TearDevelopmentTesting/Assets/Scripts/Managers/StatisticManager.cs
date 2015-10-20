/// <summary>
/// 
/// FILE: Statistic manager
/// 
/// DESCRIPTION:
/// 	This file is used to keep track of the player's statistics in game for achievements,
/// 	saving, and loading game sessions
/// 
/// AUTHORS: Tearable World Team (Crumpled Up Games' Engineers)
/// 			-> John Crocker, Justin Telmo, ... (ADD YOUR NAME HERE!!!!)
/// 
/// </summary>
/// 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatisticManager : MonoBehaviour
{
	
	#region Fields
	
	/// <summary>
	/// This represents the player's saved game, where the needed data is stored for
	/// launching a reloaded game session
	/// </summary>
	//private SavedData savedGame;
	
	/// <summary>
	/// The number levels completed represents the current number
	/// of levels the player has compeleted
	/// </summary>
	private int numLevelsCompleted;
	
	/// <summary>
	/// This is used to flag whether or not a player is currently trying to 
	/// resume a saved game session
	/// </summary>
	private bool resumeSession;
	
	/// <summary>
	/// This represents the total number of levels Tearable world has to offer
	/// the player
	/// </summary>
	private const int TOTAL_NUM_LEVELS = 1;
	
	/// <summary>
	/// This represents current high score for the only level above
	/// TODO: This will change once the player has gained points in game,
	/// updating the levelScores dictionary accordinly
	/// </summary>
	private const int LEVEL_HIGH_SCORE = 1;
	
	/// <summary>
	/// This dictionary is used to keep track of the highest score for each
	/// level the player has achieved
	/// </summary>
	private Dictionary<int, int> levelScores;
	
	#endregion
	
	#region Methods
	
	/// <summary>
	/// Use this for initialization
	/// </summary>
	private void Start () 
	{
		//Initialize number of levels completed by player to zer, ONLY if 
		//there currently is no saved data for the player to resume
		//if(savedGame == null && resumeSession)
		//{
		//	numLevelsCompleted = 0;
		//}
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	private void Update () 
	{
		//TODO
	}
	
	/// <summary>
	/// Saves the game and all stats needed for next relaunch, TODO: IMPLEMENT!
	/// </summary>
	/// <param name='currentLevel'>
	/// Current level represents the current level being saved
	/// </param>
	/// <param name='score'>
	/// Score represents the score of the player to be saved
	/// </param>
	public void SaveGame(int currentLevel, int score)
	{
		//TODO IMPLEMENT THIS METHOD
	}
	
	/// <summary>
	/// Loads the save game for the player
	/// </summary>
	public void LoadSaveGame()
	{
		//TODO
		
		//As of now, I created the static class 'SavedData' to store the information
		//needed to pull from here to load a saved game session for the player
		
		//currentGame = SavedGame
	}
	
	#endregion
}
