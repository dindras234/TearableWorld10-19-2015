// <summary>
/// 
/// FILE: Player Manager
/// 
/// DESCRIPTION:
/// 	This file is used to manage the TearableWorld_Player
/// 
/// AUTHORS: TearableWorld Team (Crumpled Up Game Engineers)
/// 			-> John Crocker, Justin Telmo, ... (ADD YOUR NAME HERE!)
/// 
/// </summary>
using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
	
	#region Fields
	
	/// <summary>
	/// This represents the player in game (stick figure)
	/// </summary>
	//public static Tearable_Player player;
	
	/// <summary>
	/// The number of lives the player has in game before 'game over'
	/// </summary>
	public static int lives;
	
	/// <summary>
	/// The current player score
	/// </summary>
	public static int score;
	
	/// <summary>
	/// The number folds the player has performed 
	/// (GLOBAL OR LOCAL TO LEVEL, UNDETERMINED YET)
	/// </summary>
	public static int numFolds;
	
	/// <summary>
	/// The number tears the player has performed 
	/// (GLOBAL OR LOCAL TO LEVEL, UNDETERMINED YET)
	/// </summary>
	public static int numTears;
	
	/// <summary>
	/// The number rotate the player has performed 
	/// (GLOBAL OR LOCAL TO LEVEL, UNDETERMINED YET)
	/// </summary>
	public static int numRotate;
	
	#endregion
	
	#region Methods

	/// <summary>
	/// Use this for initialization
	/// </summary>
	public void Start () 
	{
		//TODO
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	public void Update () 
	{
		//TODO
	}
	
	/// <summary>
	/// This is executed when the player wins a level
	/// </summary>
	/// <param name='currentLevel'>
	/// Current level number representing which level the player is on
	/// within entire game
	/// </param>
	/// <param name='levelScore'>
	/// Level score represents the players score once completing the level
	/// This will be used to update highScores if necessary from within
	/// StatisticsManager
	/// </param>
	public void PlayerWinsLevel(int currentLevel, int levelScore)
	{
		//TODO
	}
	
	#endregion
	
}