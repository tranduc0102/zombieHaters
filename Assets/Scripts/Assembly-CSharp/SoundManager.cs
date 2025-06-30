using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
	private AudioSource audioSource;

	[SerializeField]
	private AudioSource musicSource;

	[SerializeField]
	private AudioSource soundSource;

	[SerializeField]
	private AudioSource stepsLoopSource;

	[HideInInspector]
	public bool musicIsMuted;

	[HideInInspector]
	public bool soundIsMuted;

	[HideInInspector]
	public float musicVolume;

	[HideInInspector]
	public float soundVolume;

	public List<MusicInfo> menuMusic;

	public List<MusicInfo> inGameMusic;

	[SerializeField]
	private AudioClip healSound;

	[SerializeField]
	private AudioClip buffSound;

	[Range(0f, 10f)]
	[SerializeField]
	private float buffPlayDelay;

	public AudioClip clickSound;

	public AudioClip upgradeSound;

	public AudioClip claimSound;

	public AudioClip gameOverSound;

	public AudioClip newHeroOpened;

	public SusrvivorTakeDamage survivorsTakeDamage;

	public Coroutine musicCor;

	private bool healSoundPlaying;

	private bool buffSoundPlayed;

	public static SoundManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			Instance = this;
		}
	}

	public void GetSavedInfo()
	{
		if (!PlayerPrefs.HasKey(StaticConstants.MusicMuted))
		{
			PlayerPrefs.SetInt(StaticConstants.MusicMuted, 0);
			musicIsMuted = false;
			PlayerPrefs.SetInt(StaticConstants.SoundMuted, 0);
			soundIsMuted = false;
			PlayerPrefs.SetFloat(StaticConstants.MusicVolume, 1f);
			musicVolume = 1f;
			musicSource.volume = 1f;
			PlayerPrefs.SetFloat(StaticConstants.SoundVolume, 1f);
			soundVolume = 1f;
			soundSource.volume = 1f;
		}
		else
		{
			musicIsMuted = PlayerPrefs.GetInt(StaticConstants.MusicMuted) == 1;
			soundIsMuted = PlayerPrefs.GetInt(StaticConstants.SoundMuted) == 1;
			musicVolume = PlayerPrefs.GetFloat(StaticConstants.MusicVolume);
			soundVolume = PlayerPrefs.GetFloat(StaticConstants.SoundVolume);
		}
		DataLoader.gui.SetMusic(musicIsMuted);
		DataLoader.gui.SetSound(soundIsMuted);
		soundSource.mute = soundIsMuted;
		musicSource.mute = musicIsMuted;
		stepsLoopSource.mute = soundIsMuted;
		PlayRandomMusic();
	}

	public void MuteMusic()
	{
		musicIsMuted = !musicIsMuted;
		PlayerPrefs.SetInt(StaticConstants.MusicMuted, musicIsMuted ? 1 : 0);
		PlayerPrefs.Save();
		DataLoader.gui.SetMusic(musicIsMuted);
		musicSource.mute = musicIsMuted;
	}

	public void MuteSound()
	{
		soundIsMuted = !soundIsMuted;
		PlayerPrefs.SetInt(StaticConstants.SoundMuted, soundIsMuted ? 1 : 0);
		PlayerPrefs.Save();
		DataLoader.gui.SetSound(soundIsMuted);
		soundSource.mute = soundIsMuted;
		stepsLoopSource.mute = soundIsMuted;
	}

	public void PlayRandomMusic()
	{
		if (menuMusic.Count > 0)
		{
			PlayMusic(menuMusic[Random.Range(0, menuMusic.Count)].clip);
		}
	}

	public void PlayMusic(AudioClip clip)
	{
		if (clip != null)
		{
			if (musicCor != null)
			{
				StopCoroutine(musicCor);
			}
			musicCor = StartCoroutine(SmoothMusic(clip));
		}
	}

	private IEnumerator SmoothMusic(AudioClip clip)
	{
		int repeatCount = 0;
		while (true)
		{
			float t = clip.length;
			musicSource.volume = 0f;
			musicSource.clip = clip;
			float currentMaxVolume = GetMaxVolume(clip);
			musicSource.Play();
			while (musicSource.volume < currentMaxVolume)
			{
				musicSource.volume += 0.02f;
				yield return null;
			}
			yield return new WaitForSeconds(clip.length - 2.5f);
			while (musicSource.volume > 0f)
			{
				musicSource.volume -= 0.02f;
				yield return null;
			}
			if (menuMusic.Count > 1)
			{
				string lastClipName2 = clip.name;
				repeatCount++;
				int rand = Random.Range(0, menuMusic.Count);
				clip = menuMusic[rand].clip;
				if (lastClipName2 != clip.name)
				{
					lastClipName2 = clip.name;
					repeatCount = 0;
				}
				else if (repeatCount >= 3)
				{
					int num = Random.Range(0, 2) * 2 - 1;
					clip = ((rand + num < 0 || rand + num >= menuMusic.Count) ? menuMusic[rand - num].clip : menuMusic[rand + num].clip);
				}
			}
		}
	}

	public void PlayClickSound()
	{
		PlaySound(clickSound);
	}

	public void PlayHealSound()
	{
		if (!healSoundPlaying)
		{
			PlaySound(healSound);
			healSoundPlaying = true;
			Invoke("SetHealSoundPlayed", healSound.length);
		}
	}

	public void SetHealSoundPlayed()
	{
		healSoundPlaying = false;
	}

	public void PlayBuffSound()
	{
		if (!buffSoundPlayed)
		{
			PlaySound(buffSound);
			buffSoundPlayed = true;
			Invoke("SetBuffSoundPlayed", buffPlayDelay);
		}
	}

	public void SetBuffSoundPlayed()
	{
		buffSoundPlayed = false;
	}

	public void PlaySurvivorTakeDamage(SaveData.HeroData.HeroType type)
	{
		if (type == SaveData.HeroData.HeroType.MEDIC || type == SaveData.HeroData.HeroType.COOK || type == SaveData.HeroData.HeroType.PISTOL)
		{
			PlaySound(survivorsTakeDamage.femaleSounds[Random.Range(0, survivorsTakeDamage.femaleSounds.Length)]);
		}
		else
		{
			PlaySound(survivorsTakeDamage.maleSounds[Random.Range(0, survivorsTakeDamage.maleSounds.Length)]);
		}
	}

	public void PlaySurvivorTakeDamageAtPoint(SaveData.HeroData.HeroType type, Vector3 position)
	{
		if (type == SaveData.HeroData.HeroType.MEDIC || type == SaveData.HeroData.HeroType.COOK || type == SaveData.HeroData.HeroType.PISTOL)
		{
			AudioSource.PlayClipAtPoint(survivorsTakeDamage.femaleSounds[Random.Range(0, survivorsTakeDamage.femaleSounds.Length)], position);
		}
		else
		{
			AudioSource.PlayClipAtPoint(survivorsTakeDamage.maleSounds[Random.Range(0, survivorsTakeDamage.maleSounds.Length)], position);
		}
	}

	public void PlayStepsSound(bool play)
	{
		if (play)
		{
			if (!stepsLoopSource.isPlaying)
			{
				stepsLoopSource.Play();
			}
		}
		else
		{
			stepsLoopSource.Stop();
		}
	}

	public void PlaySound(AudioClip clip, float volume = -1f)
	{
		if (clip != null && !soundIsMuted && !soundSource.mute)
		{
			soundSource.PlayOneShot(clip, (volume != -1f) ? volume : soundVolume);
		}
	}

	private float GetMaxVolume(AudioClip clip)
	{
		List<MusicInfo> list = menuMusic;
		list.AddRange(inGameMusic);
		foreach (MusicInfo item in list)
		{
			if (item.clip == clip)
			{
				return item.maxVolume;
			}
		}
		return 0f;
	}

	public void MuteAll()
	{
		musicSource.mute = true;
		soundSource.mute = true;
		soundIsMuted = true;
		stepsLoopSource.mute = true;
	}

	public void UnMuteAll()
	{
		musicSource.mute = musicIsMuted;
		soundIsMuted = PlayerPrefs.GetInt(StaticConstants.SoundMuted) == 1;
		soundSource.mute = soundIsMuted;
		stepsLoopSource.mute = soundIsMuted;
	}
}
