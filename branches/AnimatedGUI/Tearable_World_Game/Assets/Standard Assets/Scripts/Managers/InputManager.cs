// <summary>
/// 
/// FILE: Input Manager
/// 
/// DESCRIPTION:
/// 	This class is used to handle the mobile device input to trigger events in game and in UI navigation
/// 	This class mainly only talks to other managers
/// 
/// AUTHORS: TearableWorld Team (Crumpled Up Game Engineers)
/// 			-> John Crocker, Justin Telmo, ... (ADD YOUR NAME HERE!)
/// 
/// </summary>
using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{
	
	#region Fields
	
	
	#endregion
	
	#region Methods

	/// <summary>
	/// Use this for initialization
	/// </summary>
	public void Start () 
	{
	
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	public void Update () 
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if(GameStateManager.gameStateManagerRef.inGame)
			{
			GameStateManager.gameStateManagerRef.PauseGame();
			}
		}
	}
	
	
	/// <summary>
	/// Gets the current screen being displayed
	/// </summary>
	public void GetScreen()
	{
		//TODO	
	}
	
	#endregion
	
}
