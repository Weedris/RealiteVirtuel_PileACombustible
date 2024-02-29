/*
* 2022-02-18
* SoundManager
* by Enzo Moulian
*
* Inspired by Tarodev
* Link to the video : 
* https://www.youtube.com/watch?v=tEsuLTpz_DU
*
* Usage : Commercial and non-commercial use
* Credits appreciated 
* 
* How to set up the Sound Manager :
* 1) Go on your 1st scene avec create an empty gameObject
* 2) Attach this script to this empty
* 3) Create 2 childrens empty gameObjects on the initial gameObject
* 4) Attach an AudioSource component for each childrens
* 5) Link parent's "BgmSource" field and "SfxSource" fields with the AudioSource component from childrens
* 6) Modify "BgmType" and "SfxType" enumerator in this script to create as much BGM and SFX as you want in your project
* 7) In Unity Editor, don't forget to match "L_bgms" length and "L_sfxs" length with "BgmType" length and "SfxType" length
* 8) Check if you have an AudioListener component in your scene (like on your Camera); otherwhise, no sound will be catched
* 9) Your SoundManager is ready !
*
* For each element of your bgm and sfx list ("L_bgms" and "L_sfxs"), choose the type of your sound and attach an audio clip
* To call a sound in your scripts, use "SoundManager.Instance" and chosse the method you want to call
*/

using System.Collections.Generic;
using UnityEngine;

public enum BgmType
{
	BGM_Elcto,
	BGM_Jazz
}

public enum SfxType
{
	GoodAnswer,
	BadAnswer,
	GrabbedObject,
	endAssembly
}

public class SoundManager : MonoBehaviour
{
	[SerializeField] private Settings settings;
	public static SoundManager Instance;
	[SerializeField]
	private AudioSource bgmSource;
	[SerializeField]
	private AudioSource sfxSource;
	[System.Serializable]
	public struct Bgm
	{
		public BgmType type;
		public AudioClip clip;
	}

	[System.Serializable]
	public struct Sfx
	{
		public SfxType type;
		public AudioClip clip;
	}

	[Header("Background Musics in project")]
	public List<Bgm> l_bgms = new(new Bgm[System.Enum.GetNames(typeof(BgmType)).Length]);
	[Header("Sound Effects in project")]
	public List<Sfx> l_sfxs = new(new Sfx[System.Enum.GetNames(typeof(SfxType)).Length]);
	private Dictionary<BgmType, AudioClip> d_AllBgm = new();
	private Dictionary<SfxType, AudioClip> d_AllSfx = new();

	void Awake()
	{
		// singleton
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
			Destroy(gameObject);

		// initialize dictionaries
		foreach (var bgm in l_bgms)
			d_AllBgm.Add(bgm.type, bgm.clip);

		foreach (var sfx in l_sfxs)
			d_AllSfx.Add(sfx.type, sfx.clip);
	}

	/// <summary>Plays the bgm attached to an given BgmType</summary>
	/// <remarks>The current BGM played is stopped when this method is called</remarks>
	/// <param name="bgm">BGM type from BgmType enumerator of SoundManager file</param>
	public void PlayBGM(BgmType bgm)
	{
		StopCurrentBgm();
		bgmSource.clip = d_AllBgm[bgm];
		bgmSource.Play();
	}

	/// <summary>Plays the sfx attached to an given SfxType</summary>
	/// <param name="sfx">SFX type from SfxType enumerator of SoundManager file</param>
	public void PlaySFX(SfxType sfx)
	{
		sfxSource.clip = d_AllSfx[sfx];
		sfxSource.Play();
	}

	/// <summary>Change the volume of all AudioListener in the scene</summary>
	/// <param name="value">Sound volume between 0.0 and 1.0</param>
	public void ChangeMasterVolume(float value)
	{
		AudioListener.volume = value;
	}

	/// <summary>Change the volume of the BGM AudioSource</summary>
	/// <param name="value">Sound volume between 0.0 and 1.0</param>
	public void ChangeBgmVolume(float value)
	{
		settings.BgmLevel = value;
		bgmSource.volume = value;
	}

	/// <summary>Change the volume of the SFX AudioSource</summary>
	/// <param name="value">Sound volume between 0.0 and 1.0</param>
	public void ChangeSfxVolume(float value)
	{
		settings.SfxLevel = value;
		sfxSource.volume = value;
	}

	/// <summary>Mute the BGM AudioSource</summary>
	/// <param name="muted">Choose if the BGM AudioSource is muted or not</param>
	public void MuteBgm(bool muted)
	{
		bgmSource.mute = muted;
	}

	/// <summary>Mute the SFX AudioSource</summary>
	/// <param name="muted">Choose if the SFX AudioSource is muted or not</param>
	public void MuteSfx(bool muted)
	{
		sfxSource.mute = muted;
	}

	/// <summary>Mute the BGM AudioSource if it's muted and vice versa</summary>
	/// <remarks>This method is created to be use on a Toggle Button in the UI element</remarks>
	public void ToggleBgm()
	{
		bgmSource.mute ^= true;
	}

	/// <summary>Mute the SFX AudioSource if it's muted and vice versa</summary>
	/// <remarks>This method is created to be use on a Toggle Button in the UI element</remarks>
	public void ToggleSfx()
	{
		sfxSource.mute ^= true;
	}

	/// <summary>Loop the sound of the BGM AudioSource</summary>
	/// <param name="loop">Choose if the BGM AudioSource is looping its sound or not</param>
	public void LoopBgm(bool loop)
	{
		bgmSource.loop = loop;
	}

	/// <summary>Loop the sound of the SFX AudioSource</summary>
	/// <param name="loop">Choose if the SFX AudioSource is looping its sound or not</param>
	public void LoopSfx(bool loop)
	{
		sfxSource.loop = loop;
	}

	/// <summary>Pause the sound of the BGM AudioSource</summary>
	/// <param name="pause">Choose if you want to pause or upause the sound of the BGM AudioSource</param>
	public void PauseBgm(bool pause)
	{
		if (pause)
			bgmSource.Pause();
		else
			bgmSource.UnPause();
	}

	/// <summary>Pause the sound of the SFX AudioSource</summary>
	/// <param name="pause">Choose if you want to pause or upause the sound of the SFX AudioSource</param>
	public void PauseSfx(bool pause)
	{
		if (pause)
			sfxSource.Pause();
		else
			sfxSource.UnPause();
	}

	/// <summary>Stop the current sound of the BGM AudioSource</summary>
	public void StopCurrentBgm()
	{
		bgmSource.Stop();
	}

	/// <summary>Stop the current sound of the SFX AudioSource</summary>
	public void StopCurrentSfx()
	{
		sfxSource.Stop();
	}
}