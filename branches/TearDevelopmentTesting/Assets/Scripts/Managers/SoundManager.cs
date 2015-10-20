// <summary>
/// 
/// FILE: Sound Manager
/// 
/// DESCRIPTION:
/// 	This file is used to keep track and manage the sound for the
/// 	tearable world game application
/// 
/// AUTHORS: TearableWorld Team (Crumpled Up Game Engineers)
/// 			-> John Crocker, Justin Telmo, ...(ADD YOUR NAME HERE!)
/// 
/// </summary>
using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
	
	#region Fields
	
	/// <summary>
	/// The primary clip currently being played
	/// </summary>
	public AudioClip primaryClip;
	
	/// <summary>
	/// The secodary clip currently being played
	/// </summary>
	public AudioClip secodaryClip;
	
	/// <summary>
	/// The effects clip currently being played
	/// </summary>
	public AudioClip effectsClip;
	
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
	/// Plays the sound clip specified
	/// </summary>
	/// <param name='clip'>
	/// Clip represents the audioClip currently trying to be played
	/// </param>
	/// <param name='loop'>
	/// Loop represents whether or not the AudioClip is to loop or not
	/// </param>
	public void PlaySound(AudioClip clip, bool loop)
	{
		//TODO
	}
	
	/// <summary>
	/// Updates the volume of a specified audioClip
	/// </summary>
	/// <param name='clip'>
	/// Clip represents the current audioClip needing adjustment
	/// </param>
	/// <param name='soundObj'>
	/// Sound object is used to check an AudioClip against an object to ensure
	/// multiple object playing the same clip do not interfear
	/// </param>
	/// <param name='vol'>
	/// Vol represents the new volume level fo the audio clip
	/// </param>
	public void UpdateVolume(AudioClip clip, GameObject soundObj, int vol)
	{
		//TODO
	}
	
	#endregion
	
}
