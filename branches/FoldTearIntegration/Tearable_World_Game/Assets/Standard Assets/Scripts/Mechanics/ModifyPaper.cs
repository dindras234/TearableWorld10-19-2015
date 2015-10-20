// <summary>
/// 
/// FILE: Modify Paper
/// 
/// DESCRIPTION:
/// 	This abstract class is used to represent mechanics used to 
/// 	distort the game world (paper).
/// 
/// AUTHORS: TearableWorld Team (Crumpled Up Game Engineers)
/// 			-> John Crocker,... (ADD YOUR NAME HERE!)
/// 
/// </summary>
using UnityEngine;
using System.Collections;

public abstract class ModifyPaper : MonoBehaviour 
{
	#region Fields
	
	/// <summary>
	/// The begin mod represents the point a modification against the current
	/// paper world begins
	/// </summary>
	Vector2 beginMod;
	
	/// <summary>
	/// The end mod represents the point a modification against the current
	/// paper world begins
	/// </summary>
	Vector2 endMod;
	
	/// <summary>
	/// The mod line represents the line of which the modifcation is occuring
	/// </summary>
	Vector2 modLine;
	
	/// <summary>
	/// The current paper world game object represents the current digital
	/// paper level the player is interacting with
	/// </summary>
	GameObject currentPaperWorld;
	
	#endregion
	
	#region Methods
	
	/// <summary>
	/// Performs a modification against the game world paper object
	/// </summary>
	/// <param name='paperWorld'>
	/// Paper world represents the paper game world which this
	/// function will modify
	/// </param>
	/// <param name='modLine'>
	/// Mod line represents the line of which the modifcation is occuring
	/// about
	/// </param>
	public abstract void PerformMod(GameObject paperWorld, Vector2 modLine);

	#endregion
	
}
