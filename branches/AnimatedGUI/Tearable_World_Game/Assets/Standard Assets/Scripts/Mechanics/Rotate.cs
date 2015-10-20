// <summary>
/// 
/// FILE: Rotate
/// 
/// DESCRIPTION:
/// 	This class is used to 'rotate' the world, which is really just
/// 	changing the direction of gravity affecting the player.
/// 
/// AUTHORS: TearableWorld Team (Crumpled Up Game Engineers)
/// 			-> John Crocker,... (ADD YOUR NAME HERE!)
/// 
/// </summary>
using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour 
{	
	#region Fields
	
	/// <summary>
	/// The paper world represents the current game world paper object
	/// being modified by the player
	/// </summary>
	public GameObject paperWorld;
	
	/// <summary>
	/// The curr rotation represents the toatl current roation (from beginning
	///  of level) the current player has talleyed.
	/// </summary>
	private float currRotation;
	
	/// <summary>
	/// The rotation limit represents the maximum amount of rotation degrees the
	/// player has (this can be used to place a limit on the amount
	/// of rotations the player can perform in a given level)
	/// </summary>
	private float rotationLimit;
	
	/// <summary>
	/// The roation imcrement represents the amount of rotation occurs at once,
	/// this, as of now, is 90 degrees!
	/// </summary>
	private const float ROTATION_INCREMENT = 90.0f;
	
	#endregion
	
	#region Methods

	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Start () 
	{
		//TODO
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	void Update () 
	{
		//TODO
	}
	
	public void RotatePaper()
	{
		//TODO
		//paperWorld....
	}
	
	#endregion
	
}