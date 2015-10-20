using UnityEngine;
using System.Collections;

public class TearableSoundManager : MonoBehaviour {
	
	public void PlaySound(string sName)
	{
		for(int i = 0; i < sounds.Length; i++)
		{
			if(sounds[i].name == sName)
			{
				sounds[i].Play();
				break;
			}
		}
	}
	
	private static GameObject smObject;
	private AudioSource[] sounds;
	private AudioSource[] sfxSounds;
	private AudioSource[] musicSounds;
	private float sfxVol;
	private float musicVol;
	private static bool create = false;
	public void AddSFX(AudioSource sfx)
	{
	}
	/// <summary>
	/// Sets the SFX vol.  The int given is the new vol for SFX.
	/// </summary>
	/// <param name='vol'>
	/// Vol.
	/// </param>
	public void setSFXVol(float vol)
	{
		sfxVol = vol;
		foreach(AudioSource s in sfxSounds)
		{
			s.volume = vol;
		}
	}
	/// <summary>
	/// Sets the music vol.  The int given is the new vol for music
	/// </summary>
	/// <param name='vol'>
	/// Vol.
	/// </param>
		public void setMusicVol(int vol)
	{
		musicVol = vol;
		foreach(AudioSource s in musicSounds)
		{
			s.volume = vol;
		}
	}
	
	public void PlayClip(AudioClip clip, GameObject obj, string type, float pitch)
	{
		AudioSource audioSource = obj.AddComponent<AudioSource>();
		audioSource.clip = clip;
		if(type == "SFX")
		{
			audioSource.volume = sfxVol;
			for(int i = 0; i < sfxSounds.Length; i++)
			{
				if(sfxSounds[i] == null)
				{
					sfxSounds[i] = audioSource;
					break;
				}
			}
		}
		else if(type == "MUSIC")
			audioSource.volume = musicVol;
		audioSource.pitch = pitch;
		audioSource.Play();
	}
	
	TearableSoundManager() 
	{
	}
	// Use this for initialization
	void Awake () {
		sfxSounds = new AudioSource[20];
		musicSounds = new AudioSource[10];
		DontDestroyOnLoad(smObject);
		for(int i = 0; i < sounds.Length; i++)
		{
		}
		sfxVol = 1.0f;
		musicVol = 1.0f;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

