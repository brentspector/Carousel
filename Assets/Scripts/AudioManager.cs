/***************************************************************************************** 
 * File:    PCScene.cs
 * Summary: Handles process for PC scene
 *****************************************************************************************/
#region Using
using UnityEngine;
using System.Collections;
#endregion

public class AudioManager : MonoBehaviour 
{
	#region Variables
	public static AudioManager instance = null;	//The reference for the singleton AudioManager

	public AudioSource sfxSource;			//Plays SFX
	public AudioSource musicSource;			//Plays background music
	public AudioClip introMusic;			//Music for the intro screen
	public AudioClip mainMenuMusic;			//Music for main menu
	public AudioClip battleMusic;			//Music for battle scene
	public AudioClip changeSFX;				//SFX for a selection change
	public AudioClip selectionSFX;			//SFX for a confirmed selection
	public float lowPitchRange = 0.95f;		//Low end of change spectrum
	public float highPitchRange = 1.05f;	//High end of change spectrum
	#endregion

	#region Methods
	/***************************************
	 * Name: Awake
	 * Initialize the AudioManager
	 ***************************************/
	void Awake()
	{
		//Set instance if one does not exist
		if (instance == null)
		{
			instance = this;
		} //end if

		//Destroy instance if one already exists
		else if (instance != this)
		{
			Destroy(this);
		} //end else if

		//Protect AudioManager from destruction between scenes
		DontDestroyOnLoad(gameObject);
	} //end Awake

	/***************************************
	 * Name: MuteMusic
	 * Silence the background music
	 ***************************************/
	public void MuteMusic()
	{
		musicSource.mute = !musicSource.mute; 
	} //end MuteMusic

	/***************************************
	 * Name: IsMusicMuted
	 * Return whether background music is 
	 * muted
	 ***************************************/
	public bool IsMusicMuted()
	{
		return musicSource.mute;
	} //end IsMusicMuted

	/***************************************
	 * Name: MuteSFX
	 * Silence sound effects
	 ***************************************/
	public void MuteSFX()
	{
		sfxSource.mute = !sfxSource.mute;
	} //end MuteSFX

	/***************************************
	 * Name: IsSFXMuted
	 * Return whether SFX are muted
	 ***************************************/
	public bool IsSFXMuted()
	{
		return sfxSource.mute;
	} //end IsMusicMuted

	/***************************************
	 * Name: MusicVolume
	 * Adjust volume of background music
	 ***************************************/
	public void MusicVolume(float vol)
	{
		musicSource.volume = vol;
	} //end MusicVolume

	/***************************************
	 * Name: MusicLevel
	 * Return the volume of the music
	 ***************************************/
	public float MusicLevel()
	{
		return musicSource.volume;
	} //end MusicLevel

	/***************************************
	 * Name: SFXVolume
	 * Adjust the volume of SFX
	 ***************************************/
	public void SFXVolume(float vol)
	{
		sfxSource.volume = vol;
	} //end SFXVolume

	/***************************************
	 * Name: SFXLevel
	 * Return the volume of the SFX
	 ***************************************/
	public float SFXLevel()
	{
		return sfxSource.volume;
	} //end SFXLevel

	/***************************************
	 * Name: PlaySingle
	 * Plays a specific SFX
	 ***************************************/
	public void PlaySingle(AudioClip clip)
	{
		sfxSource.pitch = Random.Range(lowPitchRange, highPitchRange);
		sfxSource.clip = clip;
		sfxSource.Play();
	} // end PlaySingle

	/***************************************
	 * Name: PlayIntro
	 * Plays the introduction music
	 ***************************************/
	public void PlayIntro()
	{
		StartCoroutine(CrossFadeMusic(introMusic));
	} // end PlayIntro

	/***************************************
	 * Name: PlayMainMenu
	 * Plays the main menu music
	 ***************************************/
	public void PlayMainMenu()
	{
		StartCoroutine(CrossFadeMusic(mainMenuMusic));
	} // end PlayMainMenu

	/***************************************
	 * Name: PlayBattle
	 * Plays the battle scene music
	 ***************************************/
	public void PlayBattle()
	{
		StartCoroutine(CrossFadeMusic(battleMusic));
	} // end PlayBattle

	/***************************************
	 * Name: PlayChange
	 * Plays the change SFX
	 ***************************************/
	public void PlayChange()
	{
		PlaySingle(changeSFX);
	} // end PlayChange

	/***************************************
	 * Name: PlaySelect
	 * Plays the select SFX
	 ***************************************/
	public void PlaySelect()
	{
		PlaySingle(selectionSFX);
	} // end PlaySelect

	/***************************************
	 * Name: CrossFadeMusic
	 * Cross fades music clips
	 ***************************************/
	IEnumerator CrossFadeMusic(AudioClip newClip)
	{
		float timeTaken = 0f;
		while (musicSource.volume > 0.2f)
		{
			musicSource.volume = Mathf.Lerp(musicSource.volume, 0.2f, timeTaken);
			timeTaken += Time.deltaTime;
			yield return null;
		} //end while
		musicSource.Stop();
		musicSource.clip = newClip;
		musicSource.Play();
		timeTaken = 0f;
		while (musicSource.volume <0.98f)
		{
			musicSource.volume = Mathf.Lerp(musicSource.volume, 1f, timeTaken);
			timeTaken += Time.deltaTime;
			yield return null;
		} //end while
		musicSource.volume = 1f;
	} //end CrossFadeMusic(AudioClip newClip) 
	#endregion
} //end class AudioManager
