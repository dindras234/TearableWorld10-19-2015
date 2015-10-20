// <summary>
/// 
/// FILE: Player Manager
/// 
/// DESCRIPTION:
/// 	This file is used to manage the TearableWorld_Player
/// 
/// AUTHORS: TearableWorld Team (Crumpled Up Game Engineers)
/// 			-> John Crocker, Justin Telmo, Ben Dover... (ADD YOUR NAME HERE!)
/// 
/// </summary>
using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class PlayerManager : MonoBehaviour
{
	
	#region Fields
	
	/// <summary>
	/// This represents the player in game (stick figure)
	/// </summary>
	//public static Tearable_Player player;

    /// <summary>
    /// Stopwatch for keeping track of overall game time.
    /// -J.T.
    /// </summary>
    private static Stopwatch timeInGame = new Stopwatch();


    /// <summary>
    /// A reference for GameStateManager. Used for keeping
    /// track of whether or not the player is in game.
    /// - J.T.
    /// </summary>
    private static GameStateManager gameStateManagerRef;


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
        /*
         * Need this to get a GameStateManager reference.
         */
        GameObject mainObject = GameObject.FindGameObjectsWithTag("MainObject")[0];
        timeInGame.Start();
        if (GameObject.FindGameObjectsWithTag("MainObject").Length > 1)
        {
            GameObject[] mainObjectList = GameObject.FindGameObjectsWithTag("MainObject");
            for (int i = 0; i < mainObjectList.Length; ++i)
            {
                if (mainObjectList[i].GetComponent<GameStateManager>().objectSaved)
                {
                    mainObject = mainObjectList[i];
                }
            }
        }
        gameStateManagerRef = mainObject.GetComponent<GameStateManager>();


	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	public void Update () 
	{
//        UnityEngine.Debug.Log("Game Time: " + timeInGame.Elapsed);
        if (!gameStateManagerRef.inGame)
        {
            timeInGame.Stop();
        }
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