using UnityEngine;
using System.Collections;

public class PlaySound : MonoBehaviour {
	public AudioSource[] sfxSounds = new AudioSource[20];
	public AudioSource[] musicSounds = new AudioSource[10];

	
	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(gameObject);
	}	
	
	void Awake() {
	}
	
	// Update is called once per frame
	void Update () {
		/*if(Input.GetMouseButtonDown(0))
			PlayAudio (gameObject.GetComponent(AudioSource), "SFX");
		if(Input.GetMouseButtonDown (1))
			stopSound (gameObject.GetComponent(AudioSource), "SFX");*/
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
				Debug.Log ("sfx added");
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
				Debug.Log ("music added");
				break;
			}	
	}
	/// <summary>
	/// Plays specified audiosource
	/// Needs to know type, either "MUSIC" or "SFX"
	/// </summary>
	/// <param name='source'>
	/// Source.
	/// </param>
	public void PlayAudio(string sndName, string type)
	{
		source.Play();
		for(int i = 0; i < 3; i++)
		{
			if(sfxSounds[i] == source)
				Debug.Log(source.name + " " + sfxSounds[i].name);
		}
		if(type == "MUSIC")
		{
			for(int i = 0; i < musicSounds.Length; i++)
			{
				if(musicSounds[i].clip.name == sndName)
				{
					if(!musicSounds[i].isPlaying)
					{
						musicSounds[i].Play();
						Debug.Log("playing music: " +musicSounds[i].clip.name + " through: " + musicSounds[i].name);
						break;
					}
				}
			}
		}
		else if(type == "SFX")
		{
			for(int i = 0; i < sfxSounds.Length; i++)
			{
				if(sfxSounds[i].clip.name == sndName)
				{
					if(!sfxSounds[i].isPlaying)
					{
						sfxSounds[i].Play ();
						Debug.Log("playing sfx: " +musicSounds[i].clip.name + " through: " + musicSounds[i].name);
						break;
					}
				}
			}
		}

	}
	
	/// <summary>
	/// Sets the SFX vol.
	/// </summary>
	/// <param name='vol'>
	/// Vol.
	/// </param>
	public void SetSFXVol(float vol)
	{
		foreach(AudioSource s in sfxSounds)
			s.volume = vol;
		Debug.Log("sfx vol: " + vol);

	}
	
	/// <summary>
	/// Sets the music vol.
	/// </summary>
	/// <param name='vol'>
	/// Vol.
	/// </param>
	public void setMusicVol(float vol)
	{
		foreach(AudioSource s in musicSounds)
			s.volume = vol;
		Debug.Log ("music vol: " + vol);
		
	}
	
	/// <summary>
	/// Stops specific sound, determined by AudioSource
	/// Needs to know type, either "MUSIC" or "SFX"
	/// </summary>
	/// <param name='source'>
	/// Source.
	/// </param>
	public void stopSound(string sndName, string type)
	{
		if(type == "MUSIC")
		{
			for(int i = 0; i < musicSounds.Length; i++)
			{
				if(musicSounds[i].clip.name == sndName)
				{
					musicSounds[i].Stop();
					Debug.Log("stopping music: " +musicSounds[i].clip.name + " through: " + musicSounds[i].name);
					break;
				}
			}
		}
		else if(type == "SFX")
		{
			for(int i = 0; i < sfxSounds.Length; i++)
			{
				if(sfxSounds[i].clip.name == sndName)
				{
					sfxSounds[i].Stop ();
					Debug.Log("stopping sfx: " +musicSounds[i].clip.name + " through: " + musicSounds[i].name);
					break;
				}
			}
		}
	}
	
	/// <summary>
	/// Pauses specific sound, determined by AudioSource.  Needs to know type 
	/// either "MUSIC" or "SFX"
	/// </summary>
	/// <param name='source'>
	/// Source.
	/// </param>
	public void PauseSound(string sndName, string type)
	{
		if(type == "MUSIC")
		{
			for(int i = 0; i < musicSounds.Length; i++)
			{
				if(musicSounds[i].clip.name == sndName)
				{
					musicSounds[i].Pause();
					Debug.Log("stopping music: " +musicSounds[i].clip.name + " through: " + musicSounds[i].name);
					break;
				}
			}
		}
		else if(type == "SFX")
		{
			for(int i = 0; i < sfxSounds.Length; i++)
			{
				if(sfxSounds[i].clip.name == sndName)
				{
					sfxSounds[i].Pause();
					Debug.Log("stopping sfx: " +musicSounds[i].clip.name + " through: " + musicSounds[i].name);
					break;
				}
			}
		}
	}
	/// <summary>
	/// Stops all sounds.
	/// </summary>
	public void StopAll()
	{
		foreach(AudioSource s in sfxSounds)
			s.Stop();
		Debug.Log ("sfx stopped");
		foreach(AudioSource s in musicSounds)
			s.Stop();
		Debug.Log ("music stopped");
	}
		/// <summary>
	/// Pauses all sounds.
	/// </summary>
	public void PauseAll()
	{
		foreach(AudioSource s in sfxSounds)
			s.Pause();
		Debug.Log ("all sfx stopped");
		foreach(AudioSource s in musicSounds)
			s.Pause();
		Debug.Log ("all music stopped");
	}
}
