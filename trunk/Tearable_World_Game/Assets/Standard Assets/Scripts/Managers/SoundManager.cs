using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// Sound manager for Tearable World.  Uses AudioSources and attaches them to one object.
/// Has all of the basic controls and allows for two types, SFX and MUSIC.
/// 
/// Created by Douglas Weller - 2/1/13-present
/// </summary>
public class SoundManager : MonoBehaviour {
	#region Variables
	#region Sound Variables
	/// <summary>
	/// A pre-declared audiosource, so you don't have to declare one throughout the code
	/// constantly.
	/// </summary>
	AudioSource audioSourceTemp;	
	//list of sounds fading in and out
    private List<AudioSource> fadeInSounds;
    private List<AudioSource> fadeOutSounds;
	
	/// <summary>
	/// Dictionary filled with the music audiosources, so you
	/// can look for and play it based on the string of the sound name
	/// </summary>
	private Dictionary<string, AudioSource> myMusicSounds;
	/// <summary>
	/// Dictionary filled with the sfx audiosources, so you
	/// can look for and play it based on the string of the sound name	/// </summary>
    private Dictionary<string, AudioSource> mySFXSounds;
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
	public float musicVol = 1.0f;
	/// <summary>
	/// This is the current sfx vol.
	/// </summary>
	public float sfxVol = 1.0f;
	/// <summary>
	/// List of sfx clips that have already been added, used when
	/// you call LoadAudioAssets.
	/// </summary>

	public List<string> alreadyHereSFXClips;
	/// <summary>
	/// List of music clips that have already been added, used when
	/// you call LoadAudioAssets.
	/// </summary>
	public List<string> alreadyHereMusicClips;

//    private AnimationManager animManagerRef;
//    private InputManager inputManagerRef;
//    private GameStateManager gameStateManagerRef;
//    private TWCharacterController controllerRef;
    #endregion


	#endregion


	#region Basic Functions
	// Use this for initialization
	void Start () 
    {
		//DontDestroyOnLoad(gameObject);
	}	
	// Update is called once per frame
	void Awake() {
		if(gameObject.GetComponent<AudioListener>() == null)
			gameObject.AddComponent<AudioListener>();
		myMusicSounds = new Dictionary<string, AudioSource>();
        mySFXSounds = new Dictionary<string, AudioSource>();
        fadeInSounds = new List<AudioSource>();
        fadeOutSounds = new List<AudioSource>();
		alreadyHereSFXClips = new List<string>();
		alreadyHereMusicClips = new List<string>();
	}
	void Update () {
		//All fade code written by Justin Telmo, team member.
        if (fadeOutSounds.Count > 0)
        {
            UnityEngine.Debug.Log(fadeOutSounds.First<AudioSource>().name);
            foreach (AudioSource audio in fadeOutSounds)
            {
                if (audio.volume > 0.1)
                {
                    audio.volume -= 0.1f * Time.deltaTime;
                }
            }
        }
        if (fadeInSounds.Count > 0)
        {
            foreach (AudioSource audio in fadeInSounds)
            {
                while (audio.volume < 1)
                {
                    audio.volume += 0.1f * Time.deltaTime;
                }
            }
        }
	}
	#endregion
	
	#region Load
	/// <summary>
	/// Load all sounds in music and sfx folders to empty sfx and music arrays
	/// as audioclips, then declare size of audiosource arrays for
	/// music and sfx based on amount of audio pulled
	/// </summary>
	public void LoadAllAudioAssets()
	{

	 	List<AudioClip> tempClips = Resources.LoadAll ("Audio/Music", typeof(AudioClip)).Cast<AudioClip>().ToList();
		foreach(AudioClip ac in tempClips)
			AddMusicSound(ac);
		tempClips = Resources.LoadAll ("Audio/SFX", typeof(AudioClip)).Cast<AudioClip>().ToList();
		foreach(AudioClip ac in tempClips)
			AddSFXSound(ac);
	}
	/// <summary>
	/// Loads the the list of given strings for music and sfx
	/// </summary>
	/// <param name='musList'>
	/// List of string names of songs to be loaded
	/// </param>
	/// <param name='sfxList'>
	/// List of SFX names to be loaded
	/// </param>
	public void LoadAudioAssets(List<string> musList, List<string> sfxList)
	{
		AudioClip audioClipTemp;
		RemoveSounds(musList, sfxList);
		foreach(string s in musList)
		{
			if(!alreadyHereMusicClips.Contains(s))
			{
				audioClipTemp = Resources.Load("Audio/Music/"+s, typeof(AudioClip)) as AudioClip;
				if(audioClipTemp)
				{
					AddMusicSound(audioClipTemp);
				}
			}
		}
		foreach(string s in sfxList)
		{
			if(!alreadyHereSFXClips.Contains(s))
			{
				audioClipTemp = Resources.Load("Audio/SFX/"+s, typeof(AudioClip)) as AudioClip;
				if(audioClipTemp)
				{
					AddSFXSound(audioClipTemp);
				}
			}
		}

		audioClipTemp = null;
		audioSourceTemp = null;
		alreadyHereSFXClips.Clear();
		alreadyHereMusicClips.Clear();
		Resources.UnloadUnusedAssets();
	}
	
	private void RemoveSounds(List<string> musList, List<string> sfxList)
	{
		alreadyHereSFXClips.Clear();
		alreadyHereMusicClips.Clear();
		//list of the names of audioclips to be removed from current music and sfx lists
		List<string> toRemove = new List<string>();
		//look through each sound name in myMusicSounds
		//and determine whether it should stay or not
		//This is both to find music that is already there
		//as well as dealing with songs that aren't in the list of 
		//new songs that need to be removed
		foreach(string cn in myMusicSounds.Keys)
		{
			bool chosen = false;
			for(int i = 0; i < musList.Count; i++)
				if(cn == musList[i]) chosen = true;
			if(!chosen) toRemove.Add(cn);
			else alreadyHereMusicClips.Add(cn);
		}
		//now look through the sounds to remove, and 
		//double check that it in fact exists in the music list
		//then remove it
		foreach(string sndName in toRemove)
		{
			//checks that the sound is there
			if(myMusicSounds.TryGetValue(sndName, out audioSourceTemp))
			{
				audioSourceTemp.Stop(); //stops the sound
				fadeInSounds.Remove(audioSourceTemp); //begins removing references to it
				myMusicSounds.Remove(sndName);
				Destroy(audioSourceTemp);//destroy the object audiosource
			}
		}
		//clear the list to check for sfx
		toRemove.Clear();

		//look through each sound name in mySFXSounds
		//and determine whether it should stay or not

		foreach(string cn in mySFXSounds.Keys)
		{
			bool chosen = false;
			for(int i = 0; i < sfxList.Count; i++)
				if(cn == sfxList[i]) chosen = true;
			if(!chosen) toRemove.Add(cn);
			else alreadyHereSFXClips.Add(cn);
		}
		//now look through the sounds to remove, and 
		//double check that it in fact exists in the music list
		//then remove it

		foreach(string sdName in toRemove)
		{
			if(mySFXSounds.TryGetValue(sdName, out audioSourceTemp))
			{
				//Debug.Log(audioSourceTemp.clip.name);
				audioSourceTemp.Stop();
				fadeInSounds.Remove(audioSourceTemp);
				mySFXSounds.Remove(sdName);
				Destroy(audioSourceTemp); //destroy the object audiosource
			}
		}
	}
	#endregion
    #region Add Sounds
	/// <summary>
	/// Adds the AudioClip given to the AudioSource array based on type.
	/// Type is either "MUSIC" or "SFX"
	/// 
	/// </summary>
	/// <param name='clip'>
	/// The clip to be added to list of music audiosources
	/// </param>
	/// <param name='type'>
	/// Type.
	/// </param>
	private void AddMusicSound(AudioClip clip)
	{
		if(!myMusicSounds.ContainsKey(clip.name))//double check the sound hasn't already been added
		{
			AudioSource temp;
			temp = gameObject.AddComponent<AudioSource>(); //add an audiosource to main game object
            temp.name = clip.name; //make name of audio source same as the clip
			temp.clip = clip; //set clip
			temp.volume = musicVol; //set the volume to whatever musicvol currently is
			temp.loop = true; //makes the music loop
			myMusicSounds.Add(clip.name, temp);
		}
            
	}
    private void AddSFXSound(AudioClip clip)
    {
		if(!mySFXSounds.ContainsKey(clip.name))
		{
	        AudioSource temp;
	        temp = gameObject.AddComponent<AudioSource>();//add an audiosource to main game object
	        temp.clip = clip; //set clip
	        temp.volume = sfxVol; //make the sfx set to the current sfx vol
	        temp.loop = false; //makes the sfx not loop
	        temp.name = clip.name;//make name of audio source same as the clip
	        mySFXSounds.Add(clip.name, temp);
		}
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
	public void SetSpecificSoundVol(float vol, string sndName)
	{
		//checks if the sound is in music list
        if (myMusicSounds.TryGetValue(sndName, out audioSourceTemp))
        {
            audioSourceTemp.volume = vol; //if it is it set it to a new vol.
            
            return;
        }
        else //otherwise it checks the sfx sounds
        {
			
            if (mySFXSounds.TryGetValue(sndName, out audioSourceTemp))
            {
                audioSourceTemp.volume = vol; //if sounds there it set it to new vol.
                return;
            }
        }
		//well send out error if the sound isn't there, but it won't stop the game from running
        Debug.LogError("No Music Sound Called " + sndName + " found.");
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
        foreach (AudioSource m in mySFXSounds.Values)
            m.volume = vol;
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
		foreach(AudioSource m in myMusicSounds.Values)
			m.volume = vol;
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
		//sfxSound.mute = true;
		//for(int i = 0; i < sfxLength; i++)
		//	sfxSound[i].mute = true;
		foreach(AudioSource m in myMusicSounds.Values)
			m.mute = true;
		foreach(AudioSource m in mySFXSounds.Values)
			m.mute = true;
	}
	/// <summary>
	/// Unmutes all the sounds.
	/// </summary>
	/// <returns>
	/// The mute all.
	/// </returns>
	public void UnMuteAll()
	{
		//sfxSound.mute = false;
		//for(int i = 0; i < sfxLength; i++)
		//	sfxSound[i].mute = false;
		foreach(AudioSource m in myMusicSounds.Values)
			m.mute = false;
		foreach(AudioSource m in mySFXSounds.Values)
			m.mute = false;

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
	public void PlayAudio(string sndName, string type)
	{
		if(type == "MUSIC")//checks if its a music clip we want to play
		{
			if(myMusicSounds.TryGetValue(sndName, out audioSourceTemp))
			{
				audioSourceTemp.Play(); // if it exists play it
				return;
			}
		}
		else if(type == "SFX")//checks if its a music clip we want to play
		{
			if(mySFXSounds.TryGetValue(sndName, out audioSourceTemp))
			{
				audioSourceTemp.Play();// if it exists play it
				return;
			}
		}
		//log error if the song doesn't exist in the specified area
			Debug.Log ("Failed to play " + sndName + " of type: " + type);
	}

    /// <summary>
    /// Plays an audio clip and throws it out there for 
    /// a one-time use.  This means that the sound can be played
    /// multiple times without worrying about it stopping the previous version.
    /// THe problem is you can't control it anymore, so the sound will play until
    /// it ends (which for music is never).
    /// </summary>
    /// <param name="sndName"></param>
    /// <param name="type"></param>
    public void PlayAudioOneShot(string sndName, string type)
    {
        if(type == "SFX")
		{
			if(mySFXSounds.TryGetValue(sndName, out audioSourceTemp))
			{
                audioSourceTemp.PlayOneShot(audioSourceTemp.clip);
				return;
			}
		}
		else if(type == "MUSIC")
		{
			if(myMusicSounds.TryGetValue(sndName, out audioSourceTemp))
			{
                audioSourceTemp.PlayOneShot(audioSourceTemp.clip);
				return;
			}
		}
			Debug.Log ("Failed to play one shot " + sndName + " of type: " + type);
    }
	/// <summary>
	/// Determines whether this sound is playing currently
	/// </summary>
	/// <param name='sndName'>
	/// The name of the sound we want to check
	/// </param>
	/// <param name='type'>
	/// String of type "SFX" or "MUSIC"
	/// </param>
    public bool IsAudioPlaying(string sndName, string type)
    {
        //UnityEngine.Debug.Log("Calling isAudioPlaying");
        if (type == "MUSIC")
		{
			if(myMusicSounds.TryGetValue(sndName, out audioSourceTemp))
			{
               if(audioSourceTemp.isPlaying) return true;
               else return false;
			}
		}
        else if (type == "SFX")
        {
            if (mySFXSounds.TryGetValue(sndName, out audioSourceTemp))
            {
                if (audioSourceTemp.isPlaying) return true;
                else return false;
            }
        }
      return false;
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
		if(type == "MUSIC")
		{
			if(myMusicSounds.TryGetValue(sndName, out audioSourceTemp))
			{
               
				audioSourceTemp.Stop();
				return;
			}
		}
		else if(type == "SFX")
		{
            if(mySFXSounds.TryGetValue(sndName, out audioSourceTemp))
			{
                UnityEngine.Debug.Log("We stopped: " + sndName);
				audioSourceTemp.Stop();
				return;
			}
			//Debug.LogError("cannot stop sfx");
			return;
		}
		Debug.LogError("SoundManager: " + sndName + " sound not located for StopSound of type " +type);
	}
	/// <summary>
	/// Stops all sounds, so when sound plays again it will start from beginning.
	/// </summary>
	public void StopAll()
	{
		foreach(AudioSource m in myMusicSounds.Values)
			m.Stop();
        foreach (AudioSource m in mySFXSounds.Values)
            m.Stop();
	}

	#endregion
	#region Pause
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
			if(myMusicSounds.TryGetValue(sndName, out audioSourceTemp))
			{
				audioSourceTemp.Pause();
				return;
			}
		}
		else if(type == "SFX")
		{
            if (mySFXSounds.TryGetValue(sndName, out audioSourceTemp))
            {
                audioSourceTemp.Pause();
                return;
            }
			return;
		}
		Debug.LogError("SoundManager: " + sndName + " sound not located for PauseSound of type " + type);
	}
	/// <summary>
	/// Pauses all music in musicSounds, so when music play they will begin from where they were paused.
	/// </summary>
	public void PauseAllSounds()
	{
		foreach(AudioSource m in myMusicSounds.Values)
			m.Pause();
        foreach (AudioSource m in mySFXSounds.Values)
            m.Pause();
		//Debug.Log ("SoundManager: all music paused");
	}
	#endregion

    #region Fades

    /// <summary>
    /// Fades a given sound effect or song out.
    /// Created by Justin Telmo, team member
    /// </summary>
    /// <param name="sndName"></param>
    /// <param name="type"></param>
    public void FadeOut(string sndName, string type)
    {
        if (type == "MUSIC")
        {
            if (myMusicSounds.TryGetValue(sndName, out audioSourceTemp))
            {
                fadeOutSounds.Add(audioSourceTemp);
            }
        }
        if (type == "SFX")
        {
            if (mySFXSounds.TryGetValue(sndName, out audioSourceTemp))
            {
                fadeOutSounds.Add(audioSourceTemp);
            }
        }
		Debug.Log("Failed to begin fadeout of " + sndName + " of type " + type);
    }

    /// <summary>
    /// Fades a given sound effect or song in from zero.
    /// made by justin telmo, team member
    /// </summary>
    /// <param name="sndName"></param>
    /// <param name="type"></param>
    public void FadeIn(string sndName, string type)
    {
        if (type == "MUSIC")
        {
            if (myMusicSounds.TryGetValue(sndName, out audioSourceTemp))
            {
                fadeInSounds.Add(audioSourceTemp);
            }
        }
        if (type == "SFX")
        {
            if (mySFXSounds.TryGetValue(sndName, out audioSourceTemp))
            {
                fadeInSounds.Add(audioSourceTemp);
            }
        }
		Debug.Log("Failed to begin fadein of " + sndName + " of type " + type);

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
			if(myMusicSounds.TryGetValue(sndName, out audioSourceTemp))
			{
				audioSourceTemp.pitch = pitch;
				return;
			}
		}
		//check type
		else if(type == "SFX")
		{
            if (mySFXSounds.TryGetValue(sndName, out audioSourceTemp))
            {
                audioSourceTemp.pitch = pitch;
                return;
            }
			return;
		}
			Debug.LogError("error setting pitch of " +sndName+ " of type " + type);
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
			if(myMusicSounds.TryGetValue(sndName, out audioSourceTemp))
			{
				return audioSourceTemp.pitch;
			}
		}
		else if(type == "SFX")//check type
		{
            if (mySFXSounds.TryGetValue(sndName, out audioSourceTemp))
            {
                return audioSourceTemp.pitch;
            }
		}
			Debug.LogError("Unable to get sndpitch of " + sndName+ " of type " + type);
			return 0.0f;
	}

	#endregion
}