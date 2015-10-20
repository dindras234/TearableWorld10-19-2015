using UnityEngine;
using System.Collections;

public class PlaySound : MonoBehaviour {
	#region Variables
	#region Sound Variables
	public AudioSource[] sfxSounds = new AudioSource[20];
	public AudioSource[] musicSounds = new AudioSource[10];
	/// <summary>
	/// The amount of songs in the the musicSounds array
	/// </summary>
	private int musicLength = 0;
	/// <summary>
	/// The amount of sfx in the sfxSounds array
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
	#region AudioClip's
	public AudioClip clipAge;
	public AudioClip clipSmile;
	public AudioClip clipBark;
	#endregion
	#endregion
	#region Basic Functions
	// Use this for initialization
	void Start () {
		//DontDestroyOnLoad(gameObject);
	}	
	// Update is called once per frame
	void Awake() {
		Debug.Log ("SoundManager is Awake");
		DontDestroyOnLoad(this.gameObject);

		AddAllSounds();
	}
	void Update () {
	}
	#endregion
	#region Add Sounds
	private void AddAllSounds()
	{
		//declaring music sounds
		AddSound(clipAge, "MUSIC");
		//declaring sfx sounds
		AddSound(clipSmile, "SFX");
		AddSound (clipBark, "SFX");
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
		if(type == "MUSIC")
		{
			if(musicLength < musicSounds.Length)
			{
				temp = gameObject.AddComponent<AudioSource>();
				temp.clip = clip;
				temp.volume = musicVol;
				temp.loop = true;
				musicSounds[musicLength] = temp;
				musicLength++;
				Debug.Log ("MUSIC " + temp.clip.name + " added");
				return;
			}
		}
		else if(type == "SFX")
		{
			if(sfxLength < sfxSounds.Length)
			{
				temp = gameObject.AddComponent<AudioSource>();
				temp.clip = clip;
				temp.volume = sfxVol;
				sfxSounds[sfxLength] = temp;
				sfxLength++;
				Debug.Log ("SFX " + temp.clip.name + " added");
				return;
			}
		}
		Debug.Log("failed to AddSound " + clip.name + " of type " + type);
	}
	/// <summary>
	/// Adds a SFX audiosource to sfxSounds array.
	/// </summary>
	/// <param name='source'>
	/// Source.
	/// </param>
	public void AddSFX(AudioSource source)
	{
		for(int i = 0; i < sfxSounds.Length; i++)
			if(sfxSounds[i] == null)
			{	
				sfxSounds[i] = source;
				Debug.Log ("sfx " + sfxSounds[i].clip.name + "added");
				break;
			}
	}	
	/// <summary>
	/// Adds music audiosource to musicSounds array.
	/// </summary>
	/// <param name='source'>
	/// Source.
	/// </param>
	public void AddMusic(AudioSource source)
	{
		for(int i = 0; i < musicSounds.Length; i++)
			if(musicSounds[i] == null)
			{	
				musicSounds[i] = source;
				Debug.Log ("music " + sfxSounds[i].clip.name + "added");
				break;
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
	public void SetSpecificSndVol(float vol, string sndName, string type)
	{
		if(type == "MUSIC")
			SetVolForSpecificSnd(vol, sndName, musicSounds, musicLength);
		else if(type == "SFX")
			SetVolForSpecificSnd(vol, sndName, sfxSounds, sfxLength);
		else {
			Debug.Log ("unknown type of sound: " + type);
			return;
		}
		Debug.Log ("set " + sndName + " vol to " + vol);
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
		bool sndFound = false;
		for(int i = 0; i < len; i++)
		{
			if(s[i].clip.name == sndName)
			{
				s[i].volume = vol;
				sndFound = true;
				Debug.Log ("setting sound: " + sndName + " to: " + vol);
				break;
			}
		}
		if(!sndFound) Debug.Log ("sound not found for SetSPecificSoundVol");
	}
	/// <summary>
	/// Sets the SFX vol.
	/// </summary>
	/// <param name='vol'>
	/// Vol.
	/// </param>
	public void SetSFXVol(float vol)
	{
		sfxVol = vol;
		for(int i = 0; i < sfxLength; i++)
			sfxSounds[i].volume = vol;
		Debug.Log("setting sfx vol to: " + vol);

	}
	/// <summary>
	/// Sets the music vol.
	/// </summary>
	/// <param name='vol'>
	/// Vol.
	/// </param>
	public void SetMusicVol(float vol)
	{
	musicVol = vol;
	for(int i = 0; i < musicLength; i++)
			musicSounds[i].volume = vol;
		Debug.Log ("setting music vol to: " + vol);
		
	}
	/// <summary>
	/// Returns musicVol.
	/// </summary>
	/// <returns>
	/// The music vol.
	/// </returns>
	public float GetMusicVol()
	{
		return musicVol;
	}
	/// <summary>
	/// Returns sfxVol.
	/// </summary>
	/// <returns>
	/// The SFX vol.
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
			Debug.Log ("Unknown type of sound: " + type);
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
		bool sndFound = false;
		for(int i = 0; i < len; i++)
		{
			if(s[i].clip.name == sndName)
			{
				if(!s[i].isPlaying)
				{
					s[i].Play();
					sndFound = true;
					Debug.Log ("playing sound: " + s[i].clip.name);
					break;
				}
			}
		}
		if(!sndFound) Debug.Log ("Sound not found for PlayAudio");
	}
	/// <summary>
	/// Plays all sounds in sfxSounds and musicSounds.
	/// </summary>
	public void PlayAll()
	{
		for(int i = 0; i < sfxLength; i++)
			sfxSounds[i].Play();
		for(int i = 0; i < musicLength; i++)
			musicSounds[i].Play();
		Debug.Log ("all sounds playing");
	}
	#endregion
	#region Stop
	/// <summary>
	/// Stops specific sound, determined by AudioSource
	/// Needs to know type, either "MUSIC" or "SFX"
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
					sndFound = true;
					Debug.Log("stopping music: " +musicSounds[i].clip.name + " through: " + musicSounds[i].name);
					break;
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
					sndFound = true;
					Debug.Log("stopping sfx: " +sfxSounds[i].clip.name + " through: " + sfxSounds[i].name);
					break;
				}
			}
		}
		if(!sndFound) Debug.Log("sound not located for StopSound");
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
		Debug.Log ("all sounds stopped");
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
		Debug.Log ("all sounds paused");
	}
	/// <summary>
	/// Pauses specific sound, determined by AudioSource.  Needs to know type 
	/// either "MUSIC" or "SFX"
	/// </summary>
	/// <param name='source'>
	/// Source.
	/// </param>
	public void PauseSound(string sndName, string type)//change string sndname to audiosource source if reverting to old code
	{
		bool sndFound = false;
		if(type == "MUSIC")
		{
			for(int i = 0; i < musicLength; i++)
			{
				if(musicSounds[i].clip.name == sndName)
				{
					musicSounds[i].Pause();
					sndFound = true;
					Debug.Log("stopping music: " +musicSounds[i].clip.name + " through: " + musicSounds[i].name);
					break;
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
					sndFound = true;
					Debug.Log("stopping sfx: " +sfxSounds[i].clip.name + " through: " + sfxSounds[i].name);
					break;
				}
			}
		}
		if(!sndFound) Debug.Log("sound not located for PauseSound");
	}
	/// <summary>
	/// Pauses all SFX in sfxSounds,  so when SFX play they will begin from where they were paused.
	/// </summary>
	public void PauseAllSFX()
	{
		for(int i = 0; i < sfxLength; i++)
			sfxSounds[i].Pause();
		Debug.Log ("all sfx paused");
	}
	/// <summary>
	/// Pauses all music in musicSounds, so when music play they will begin from where they were paused.
	/// </summary>
	public void PauseAllMusic()
	{
		for(int i = 0; i < musicLength; i++)
			musicSounds[i].Pause();
		Debug.Log ("all music paused");
	}
	#endregion
	#endregion
	#region Pitch
	/// <summary>
	/// This sets the pitch for a specific sound.
	/// The sndName is the clip name you want to change.
	/// Types are "MUSIC" or "SFX"
	/// </summary>
	public void SetSndPitch(string sndName, float pitch, string type)
	{
		if(type == "MUSIC")
		{
			for(int i = 0; i < musicLength; i++)
			{
				if(musicSounds[i].clip.name == sndName)
				{
					musicSounds[i].pitch = pitch;
					Debug.Log ("Setting " + sndName + " pitch to " + pitch);
				}
			}
		}
		else if(type == "SFX")
		{
			for(int i = 0; i < sfxLength; i++)
			{
				if(sfxSounds[i].clip.name == sndName)
				{
					sfxSounds[i].pitch = pitch;
					Debug.Log ("Setting " + sndName +" pitch to " + pitch);
				}
			}
		}
		else
		{
			Debug.Log("unknown type: " + type);
			Debug.Log ("no sound's pitch set");
		}
	}
	/// <summary>
	/// Returns the pitch of the clip that has the same name as sndName, that is in type "MUSIC" or "SFX"
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
		if(type == "MUSIC")
		{
			for(int i = 0; i < musicLength; i++)
			{
				if(musicSounds[i].clip.name == sndName)
					return musicSounds[i].pitch;
			}
		}
		else if(type == "SFX")
		{
			for(int i = 0; i < sfxLength; i++)
			{
				if(sfxSounds[i].clip.name == sndName)
					return sfxSounds[i].pitch;
			}
		}
			Debug.Log("unknown type: " + type);
			return 0.0f;
	}
	#endregion

}