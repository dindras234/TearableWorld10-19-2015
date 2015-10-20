using UnityEngine;
using System.Linq;
using System.Collections;

public class SoundManager : MonoBehaviour {
	#region Variables
	#region Sound Variables
	#region AudioClip's
	private AudioClip[] sfxClips;
	private AudioClip[] musicClips;
	#endregion
	private AudioSource[] sfxSounds;
	private AudioSource[] musicSounds;
	/// <summary>
	/// The next open musicSounds array point.
	/// </summary>
	private int musicLength = 0;
	/// <summary>
	/// The next open point in array of SFX.
	/// </summary>
	private int sfxLength = 0;
	/// <summary>
	/// This is the current music vol.
	/// </summary>
	private float musicVol = 1.0f;
	/// <summary>
	/// This is the current sfx vol.
	/// </summary>
	private float sfxVol = 1.0f;
	#endregion
	#endregion
	#region Basic Functions
	// Use this for initialization
	void Start () {
		//DontDestroyOnLoad(gameObject);
	}	
	// Update is called once per frame
	void Awake() {
		gameObject.AddComponent<AudioListener>();
		LoadAudioAssets();
		//Debug.Log ("SoundManager is Awake");
		//DontDestroyOnLoad(this.gameObject);
		AddAllSounds();
	}
	void Update () {
	}
	#endregion
	
	#region Load
	/// <summary>
	/// Loads the sound assets in Sounds folder,
	/// </summary>
	private void LoadAudioAssets()
	{
		//Load all sounds in music and sfx folders to empty sfx and music arrays
		//as audioclips, then declare size of audiosource arrays for
		//music and sfx based on amount of audio pulled
	 	musicClips = Resources.LoadAll ("Audio/Music", typeof(AudioClip)).Cast<AudioClip>().ToArray();
		musicSounds = new AudioSource[musicClips.Length];
		
		sfxClips = Resources.LoadAll ("Audio/SFX", typeof(AudioClip)).Cast<AudioClip>().ToArray();
		sfxSounds = new AudioSource[sfxClips.Length];
		//Debug.Log("All audio clips loaded");
	}
	#endregion
	#region Add Sounds
	/// <summary>
	/// This is where you add all the sounds you want to add to sound manager.
	/// </summary>
	private void AddAllSounds()
	{
		//declaring music sounds
		foreach(AudioClip m in musicClips)
			AddSound (m, "MUSIC");
		//declaring sfx sounds
		foreach(AudioClip s in sfxClips)
			AddSound(s, "SFX");
		//Debug.Log("All Sounds added.");
	}
	/// <summary>
	/// Adds the AudioClip given to the AudioSource array based on type.
	/// Type is either "MUSIC" or "SFX"
	/// 
	/// </summary>
	/// <param name='clip'>
	/// Clip.
	/// </param>
	/// <param name='type'>
	/// Type.
	/// </param>
	private void AddSound(AudioClip clip, string type)
	{
		AudioSource temp;
		//Check where the new sound clip is either a SFX or Music
		if(type == "MUSIC")
		{
			//Checking to see if musicLength is still smaller than the array of music
			if(musicLength < musicSounds.Length)
			{
				musicSounds[musicLength] = gameObject.AddComponent<AudioSource>(); //adds an audiosource to the gameObject
				musicSounds[musicLength].clip = clip; //set clip
				musicSounds[musicLength].volume = musicVol; 
				musicSounds[musicLength].loop = true; //makes the music loop
				//musicSounds[musicLength] = temp; //sets next open point in array to the new audiosource
				musicLength++; //increment musicLength, which represents the next open point in array.
				//Debug.Log ("MUSIC: " + clip.name + " added");
				return;
			}
		}
		//Check where the new sound clip is either a SFX or Music
		else if(type == "SFX")
		{
			//Checking to see if musicLength is still smaller than the array of music
			if(sfxLength < sfxSounds.Length)
			{
				sfxSounds[sfxLength] = gameObject.AddComponent<AudioSource>();//adds an audiosource to the gameObject
				sfxSounds[sfxLength].clip = clip; //set clip
				sfxSounds[sfxLength].volume = sfxVol;
				//sfxSounds[sfxLength] = temp; //sets the next open point in array to new audio source
				sfxLength++; //increment the next open point in array
				//Debug.Log ("SFX: " + clip.name + " added");
				return;
			}
		}
		Debug.Log("failed to AddSound " + clip.name + " of type " + type);
	}
	#endregion
	#region Volume
	/// <summary>
	/// Checks the type of sound "SFX" or "MUSIC".
	/// sndName is the name of the clip you want to stop.
	/// It then calls SetVolForSpecificSnd.
	/// </summary>
	/// <param name='vol'>
	/// Vol.
	/// </param>
	/// <param name='sndName'>
	/// Snd name.
	/// </param>
	/// <param name='type'>
	/// Type.
	/// </param>
	public void SetSpecificSndVol(float vol, string sndName, string type)
	{
		if(type == "MUSIC")//check type
			SetVolForSpecificSnd(vol, sndName, musicSounds, musicLength);
		else if(type == "SFX")//check type
			SetVolForSpecificSnd(vol, sndName, sfxSounds, sfxLength);
		else {
			Debug.Log ("unknown type of sound: " + type);
			return;
		}
		//Debug.Log ("set " + sndName + " vol to " + vol);
	}
	/// <summary>
	/// Sets the vol for a specific sound.  
	/// Vol is the Volume.
	/// sndName is the clip name of the sound.
	/// s is the AudioSource array type(musicSounds or sfxSounds)
	/// len is the amount of sounds in above array(musicLength or sfxLength)
	/// </summary>
	/// <param name='vol'>
	/// Vol.
	/// </param>
	/// <param name='sndName'>
	/// Snd name.
	/// </param>
	/// <param name='s'>
	/// S.
	/// </param>
	/// <param name='len'>
	/// Length.
	/// </param>
	private void SetVolForSpecificSnd(float vol, string sndName, AudioSource[] s, int len)
	{
		for(int i = 0; i < len; i++)
		{
			//check if the sfx with same name is located
			if(s[i].clip.name == sndName)
			{
				s[i].volume = vol;
				//Debug.Log ("setting sound: " + sndName + " to: " + vol);
				return;
			}
		}
		Debug.Log ("SoundManager: " + sndName +" not found for SetSPecificSoundVol");
	}
	/// <summary>
	/// Sets the SFX vol.
	/// </summary>
	/// <param name='vol'>
	/// Vol.
	/// </param>
	public void SetSFXVol(float vol)
	{
		sfxVol = vol; //set SFX vol for all new sfx added.
		//loop through SFX and set volume for each one to vol
		for(int i = 0; i < sfxLength; i++)
			sfxSounds[i].volume = vol;
		//Debug.Log("setting sfx vol to: " + vol);

	}
	/// <summary>
	/// Sets the music vol.
	/// </summary>
	/// <param name='vol'>
	/// Vol.
	/// </param>
	public void SetMusicVol(float vol)
	{
	musicVol = vol;//set music vol for all new music added
	//loop through music and set volume for each one to vol
	for(int i = 0; i < musicLength; i++)
			musicSounds[i].volume = vol;
		//Debug.Log ("setting music vol to: " + vol);
		
	}
	/// <summary>
	/// Returns the current music volume.
	/// </summary>
	/// <returns>
	/// The current music volume.
	/// </returns>
	public float GetMusicVol()
	{
		return musicVol;
	}
	/// <summary>
	/// Returns the current sound effects volume.
	/// </summary>
	/// <returns>
	/// The current sound effects voluem.
	/// </returns>
	public float GetSFXVol()
	{
		return sfxVol;
	}
	#region Mute
	/// <summary>
	/// Mutes all sounds.
	/// </summary>
	/// <returns>
	/// The all.
	/// </returns>
	public void MuteAll()
	{
		for(int i = 0; i < sfxLength; i++)
			sfxSounds[i].mute = true;
		for(int i = 0; i < musicLength; i++)
			musicSounds[i].mute = true;
	}
	/// <summary>
	/// Unmutes all the sounds.
	/// </summary>
	/// <returns>
	/// The mute all.
	/// </returns>
	public void UnMuteAll()
	{
		for(int i = 0; i < sfxLength; i++)
			sfxSounds[i].mute = false;
		for(int i = 0; i < musicLength; i++)
			musicSounds[i].mute = false;
	}
	#endregion
	#endregion
	#region Stop, Pause, Play
	#region Play
	/// <summary>
	/// Plays specified audiosource
	/// Needs to know type, either "MUSIC" or "SFX"
	/// </summary>
	/// <param name='source'>
	/// Source.
	/// </param>
	public void PlayAudio(string sndName, string type) //change string sndname to audiosource source if reverting to old code
	{
		if(type == "MUSIC")
			PlaySFXorMusic(musicSounds, musicLength, sndName);
		else if(type == "SFX")
			PlaySFXorMusic(sfxSounds, sfxLength, sndName);
		else
			Debug.Log ("SoundManager: Unknown type of sound: " + type);
	}
	/// <summary>
	/// Plays a specific sound.
	/// s is the AudioSource array the sound is located in(sfxSounds or musicSounds)
	/// len is the amount of sounds in the above array (sfxLength or musicLength)
	/// sndName is the name of the clip.
	/// </summary>
	/// <param name='s'>
	/// S.
	/// </param>
	/// <param name='len'>
	/// Length.
	/// </param>
	/// <param name='sndName'>
	/// Snd name.
	/// </param>
	private void PlaySFXorMusic(AudioSource[] s, int len, string sndName)
	{
		for(int i = 0; i < len; i++) //loop through array of s until you reach end of it (len)
		{
			if(s[i].clip.name == sndName) //if you find sound with audiosource with same clip name as sndName
			{
				if(!s[i].isPlaying) //and its not already playing
				{
					s[i].Play();    //play it, make it known the sound was found and send message saying it worked
					//Debug.Log (" SoundManager: playing sound: " + s[i].clip.name);
					return;
				}
			}
		}
		//Sound was not located so send out message, letting people know
		Debug.Log ("SoundManager " + sndName + " not found for PlayAudio");
	}
	#endregion
	#region Stop
	/// <summary>
	/// Stops specific sound, determined by AudioSource
	/// Needs to know type, either "MUSIC" or "SFX"
	/// Stop means that the music is stopped.  When you play it again it will start from the beginning.
	/// </summary>
	/// <param name='source'>
	/// Source.
	/// </param>
	public void StopSound(string sndName, string type)//change string sndname to audiosource source if reverting to old code
	{
		bool sndFound = false;
		if(type == "MUSIC")
		{
			for(int i = 0; i < musicLength; i++)
			{
				if(musicSounds[i].clip.name == sndName)
				{
					musicSounds[i].Stop();
					//Debug.Log("stopping music: " +musicSounds[i].clip.name + " through: " + musicSounds[i].name);
					return;
				}
			}
		}
		else if(type == "SFX")
		{
			for(int i = 0; i < sfxLength; i++)
			{
				if(sfxSounds[i].clip.name == sndName)
				{
					sfxSounds[i].Stop ();
					//Debug.Log("stopping sfx: " +sfxSounds[i].clip.name + " through: " + sfxSounds[i].name);
					return;
				}
			}
		}
		Debug.Log("SoundManager: " + sndName + " sound not located for StopSound");
	}
	/// <summary>
	/// Stops all sounds, so when sound plays again it will start from beginning.
	/// </summary>
	public void StopAll()
	{
		for(int i = 0; i < sfxLength; i++)
			sfxSounds[i].Stop();
		for(int i = 0; i < musicLength; i++)
			musicSounds[i].Stop();
		//Debug.Log ("SoundManager: all sounds stopped");
	}
	#endregion
	#region Pause
	/// <summary>
	/// Pauses all sounds, so when sound plays it will begin from where it was paused.
	/// </summary>
	public void PauseAll()
	{
		PauseAllSFX();
		PauseAllMusic();
		//Debug.Log ("SoundManager: all sounds paused");
	}
	/// <summary>
	/// Pauses specific sound, determined by AudioSource.  Needs to know type 
	/// either "MUSIC" or "SFX"
	/// Pause means that after being paused, the song will continue from that point
	/// when played again.
	/// </summary>
	/// <param name='source'>
	/// Source.
	/// </param>
	public void PauseSound(string sndName, string type)//change string sndname to audiosource source if reverting to old code
	{
		if(type == "MUSIC")
		{
			for(int i = 0; i < musicLength; i++)
			{
				if(musicSounds[i].clip.name == sndName)
				{
					musicSounds[i].Pause();
					//Debug.Log("SoundManager: stopping music: " +musicSounds[i].clip.name + " through: " + musicSounds[i].name);
					return;
				}
			}
		}
		else if(type == "SFX")
		{
			for(int i = 0; i < sfxLength; i++)
			{
				if(sfxSounds[i].clip.name == sndName)
				{
					sfxSounds[i].Pause();
					//Debug.Log("SoundManager: stopping sfx: " +sfxSounds[i].clip.name + " through: " + sfxSounds[i].name);
					return;
				}
			}
		}
		Debug.Log("SoundManager: " + sndName + " sound not located for PauseSound");
	}
	/// <summary>
	/// Pauses all SFX in sfxSounds,  so when SFX play they will begin from where they were paused.
	/// </summary>
	public void PauseAllSFX()
	{
		for(int i = 0; i < sfxLength; i++)
			sfxSounds[i].Pause();
		//Debug.Log ("SoundManager: all sfx paused");
	}
	/// <summary>
	/// Pauses all music in musicSounds, so when music play they will begin from where they were paused.
	/// </summary>
	public void PauseAllMusic()
	{
		for(int i = 0; i < musicLength; i++)
			musicSounds[i].Pause();
		//Debug.Log ("SoundManager: all music paused");
	}
	#endregion
	#endregion
	#region Pitch
	/// <summary>
	/// This sets the pitch for a specific sound.
	/// Pitch is the speed at which the sound plays, so 1 is regular, 2 is double, .5 is half.
	/// The sndName is the clip name you want to change.
	/// Types are "MUSIC" or "SFX"
	/// </summary>
	public void SetSndPitch(string sndName, float pitch, string type)
	{
		//check type
		if(type == "MUSIC")
		{
			//loop through music array
			for(int i = 0; i < musicLength; i++)
			{
				//if clip name is the same as sndName
				if(musicSounds[i].clip.name == sndName)
				{
					//set pitch
					musicSounds[i].pitch = pitch;
					//Debug.Log ("SoundManager: Setting " + sndName + " pitch to " + pitch);
				}
			}
		}
		//check type
		else if(type == "SFX")
		{
			//loop through the music array
			for(int i = 0; i < sfxLength; i++)
			{
				//if clip name is the same as sndName
				if(sfxSounds[i].clip.name == sndName)
				{
					//set pitch
					sfxSounds[i].pitch = pitch;
					//Debug.Log ("SoundManager: Setting " + sndName +" pitch to " + pitch);
				}
			}
		}
		else //if the type is not known
		{
			Debug.Log("SoundManager: unknown type: " + type);
			Debug.Log ("SoundManager: no sound's pitch set");
		}
	}
	/// <summary>
	/// Returns the pitch of the clip that has the same name as sndName, that is in type "MUSIC" or "SFX"
	/// Pitch is the speed at which the sound plays, so 1 is regular, 2 is double, .5 is half.
	/// </summary>
	/// <returns>
	/// The snd pitch.
	/// </returns>
	/// <param name='sndName'>
	/// Snd name.
	/// </param>
	/// <param name='type'>
	/// Type.
	/// </param>
	public float GetSndPitch(string sndName, string type)
	{
		if(type == "MUSIC") //check type
		{
			//loop through music array
			for(int i = 0; i < musicLength; i++)
			{
				//if current audiosource.clip.name is the same as sndName
				if(musicSounds[i].clip.name == sndName)
					return musicSounds[i].pitch; //return pitch
			}
		}
		else if(type == "SFX")//check type
		{
			//loop through sfx array
			for(int i = 0; i < sfxLength; i++)
			{
				//if current audiosource.clip name is the same as sndName
				if(sfxSounds[i].clip.name == sndName)
					return sfxSounds[i].pitch; //return its pitch
			}
		}
		//if it gets to this point, state that is unknown type and return 0 for pitch.
			Debug.Log("SoundManager: unknown type: " + type);
			return 0.0f;
	}
	#endregion
}