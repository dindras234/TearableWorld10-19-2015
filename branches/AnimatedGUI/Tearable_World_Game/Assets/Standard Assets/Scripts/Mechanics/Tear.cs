/// <summary>
/// 
/// FILE: Tear - mechanic
/// 
/// DESCRIPTION:
/// 	This class represents the logic needed to tear the digital world for the
/// 	player to interact with to solve the in game puzzle challenges
/// 
/// AUTHORS: TearableWorld Team (Crumpled Up Game Engineers)
/// 			-> John Crocker,... (ADD YOUR NAME HERE!)
/// 
/// </summary>
using UnityEngine;
using System.Collections;

public class Tear : ModifyPaper
{
	
	#region Fields
	
	#endregion
	
	#region Methods

	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Start () 
	{
	
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	void Update () 
	{
	
	}
	
	/// <summary>
	/// Performs the tear modification on the paperWorld
	/// </summary>
	/// <param name='PaperWorld'>
	/// Paper world is the actual digital piece of paper representing the game
	/// background and gameObject the player is interacting with && on
	/// </param>
	/// <param name='tearLine'>
	/// Tear line represents the line of which the tear is occuring along
	/// </param>
	public override void PerformMod(GameObject PaperWorld, Vector2 tearLine)
	{
		//TODO
	}
	
	#endregion
	
}
