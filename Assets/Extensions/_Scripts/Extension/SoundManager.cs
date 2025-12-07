using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.Extension
{
	public enum ESetting
	{
		SOUND, MUSIC, HAPTIC, NOTIFI
	}

	public delegate void OnMusicChanged(bool music);

	public class SoundManager : Singleton<SoundManager>
	{
		[SerializeField] AudioClip clickClip;

		[SerializeField] AudioSource originAS;

		public bool music
		{
			get { return PlayerPrefs.GetInt("music") == 0; }
			private set { PlayerPrefs.SetInt("music", value ? 0 : 1); }
		}
		public bool sfx
		{
			get { return PlayerPrefs.GetInt("sfx") == 0; }
			private set { PlayerPrefs.SetInt("sfx", value ? 0 : 1); }
		}
		public bool vibration
		{
			get { return PlayerPrefs.GetInt("vibration") == 0; }
			private set { PlayerPrefs.SetInt("vibration", value ? 0 : 1); }
		}

		public bool notifi
		{
			get { return PlayerPrefs.GetInt("notifi") == 0; }
			private set { PlayerPrefs.SetInt("notifi", value ? 0 : 1); }
		}


		public UnityAction musicOnOff, sfxOnOff, vibrationOnOff;
		public OnMusicChanged onMusicChanged;


		protected override void Awake()
		{
			base.KeepAlive(false);
			asPool.Add(originAS);
		}

		public void SetMusic(bool value)
		{
			music = value;
			onMusicChanged(music);

			if (musicOnOff != null)
				musicOnOff();
		}
		public void SetSfx(bool value)
		{
			sfx = value;
			if (sfxOnOff != null)
				sfxOnOff();
		}
		public void SetVibration(bool value)
		{
			vibration = value;

			if (vibrationOnOff != null)
				vibrationOnOff();
		}

		public void SetNotifi(bool value)
		{
			notifi = value;
		}


		public void Vibrate(int miliseconds)
		{
			if (!vibration) return;
#if !UNITY_STANDALONE_WIN
			/*
			Vibration.Vibrate(miliseconds);
*/
#endif
		}



		public AudioSource PlaySfx(AudioClip clip, float delay = 0, float volume = 1f, bool loop = false)
		{
			if (clip == null) return null;
			if (!sfx) return null;

			var audioSource = GetAudioSource();
			audioSource.clip = clip;
			audioSource.time = 0f;
			audioSource.loop = loop;
			audioSource.spatialBlend = 0f;
			audioSource.volume = volume;
			// audioSource.pitch = Random.Range(.95f, 1.05f);
			if (delay > 0)
				audioSource.PlayDelayed(delay);
			else
				audioSource.Play();
			return audioSource;
		}



		public AudioSource PlaySfxAPosition(AudioClip clip, Vector2 position, float volume = 1f, bool loop = false, float maxDistance = 0f, float delay = 0f)
		{
			if (!sfx) return null;

			var audioSource = GetAudioSource();
			audioSource.clip = clip;
			audioSource.time = 0f;
			audioSource.loop = loop;
			audioSource.spatialBlend = 1f;
			audioSource.volume = volume;
			// if (maxDistance == 0f)
			//     audioSource.maxDistance = GamePlay.instance.gameData.defaultSoundDistance;
			// else
			audioSource.maxDistance = maxDistance;
			audioSource.transform.position = position;

			if (delay > 0)
				audioSource.PlayDelayed(delay);
			else
				audioSource.Play();
			return audioSource;
		}

		public void _PlaySfx(AudioClip clip)
		{
			PlaySfx(clip);
		}

		public void PlaySfxs(params AudioClip[] clips)
		{
			for (int i = 0; i < clips.Length; i++)
			{
				PlaySfx(clips[i]);
			}
		}

		public AudioSource PlayRndSfx(params AudioClip[] clips)
		{
			if (clips == null || clips.Length == 0) return null;
			return PlaySfx(clips[Random.Range(0, clips.Length)]);
		}


		public AudioSource PlayRndSfx(float delay, float volume, params AudioClip[] clips)
		{
			if (clips == null || clips.Length == 0) return null;
			return PlaySfx(clips[Random.Range(0, clips.Length)], delay, volume);
		}

		public AudioSource PlayRndSfxAtPosition(Vector3 position, bool loop, float maxDistance = 0f, params AudioClip[] clips)
		{
			return PlaySfxAPosition(clips[Random.Range(0, clips.Length)], position, 1f, loop, maxDistance);
		}

		///////////////////////////////////////////////////////////////////

		public void PlayBtnClick()
		{
			PlaySfx(clickClip);
		}

		public void ClearLoopSfxs()
		{
			for (int i = 0; i < asPool.Count; i++)
			{
				if (asPool[i].loop)
					asPool[i].Stop();
			}
		}



		/////////////////////////////////////////////////////////////
		List<AudioSource> asPool = new List<AudioSource>();
		public AudioSource GetAudioSource()
		{
			for (int i = 0; i < asPool.Count; i++)
			{
				if (!asPool[i].isPlaying)
					return asPool[i];
			}

			AudioSource newAS = Instantiate(originAS, originAS.transform.parent);
			asPool.Add(newAS);
			return newAS;
		}


	}
}