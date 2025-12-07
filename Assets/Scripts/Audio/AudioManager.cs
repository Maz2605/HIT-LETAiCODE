using System.Collections;
using UnityEngine;
using DesignPattern;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Background Music")]
    public AudioSource bgmSource;
    public AudioClip bgmClip;

    [Header("Sound Effects")]
    public AudioClip soundWin;
    public AudioClip soundLose;
    public AudioClip soundCollectCoin;
    public AudioClip soundClick;

    [Header("Player Sounds")]
    public AudioClip soundDie;
    public AudioClip soundSkillOn;
    public AudioClip soundSkillOff;
    public AudioClip soundDash;
    public AudioClip soundMove;  // chạy loop

    [SerializeField] private AudioSource sfxSource;
    private AudioSource movementSource;   

    protected override void Awake()
    {
        base.KeepAlive(true);
        base.Awake();


        movementSource = gameObject.AddComponent<AudioSource>();
        movementSource.loop = true;
        movementSource.playOnAwake = false;

        float volume = SaveSystem.LoadFloat("VolumnSound", 1f);

        bgmSource.volume = volume;
        sfxSource.volume = volume;
        movementSource.volume = volume * 0.7f; 

        bgmSource.mute = !SettingData.Music;
        sfxSource.mute = !SettingData.Sound;
        movementSource.mute = !SettingData.Sound;
    }

    private void Start()
    {
        /*PlayBGM(bgmClip);*/
    }

    // =============================
    // PLAY BGM
    // =============================
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;

        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
        }

        bgmSource.clip = clip;
        bgmSource.Play();
    }

    // =============================
    // CHANGE BGM (FADE)
    // =============================
    public void ChangeBGM(AudioClip newClip, float fadeTime = 0.5f)
    {
        StopAllCoroutines();
        StartCoroutine(FadeChangeBGM(newClip, fadeTime));
    }

    private IEnumerator FadeChangeBGM(AudioClip newClip, float time)
    {
        float startVolume = bgmSource.volume;

        // Fade out
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / time;
            bgmSource.volume = Mathf.Lerp(startVolume, 0, t);
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.clip = newClip;
        bgmSource.Play();

        // Fade in
        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / time;
            bgmSource.volume = Mathf.Lerp(0, startVolume, t);
            yield return null;
        }
    }

    // =============================
    // SOUND EFFECTS
    // =============================
    public void PlayWin() => PlaySFX(soundWin);
    public void PlayLose() => PlaySFX(soundLose);
    public void PlayCollectCoin() => PlaySFX(soundCollectCoin);
    public void PlayClick() => PlaySFX(soundClick);

    public void PlayDie() => PlaySFX(soundDie);
    public void PlaySkillOn() => PlaySFX(soundSkillOn);
    public void PlaySkillOff() => PlaySFX(soundSkillOff);
    public void PlayDash() => PlaySFX(soundDash);

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    // =============================
    // MOVEMENT SOUND (loop)
    // =============================
    public void PlayMovement(bool isMoving)
    {
        if (soundMove == null) return;

        if (isMoving)
        {
            if (!movementSource.isPlaying)
            {
                movementSource.clip = soundMove;
                movementSource.Play();
            }
        }
        else
        {
            if (movementSource.isPlaying)
                movementSource.Stop();
        }
    }


    public void SetValue(float value)
    {
        SaveSystem.SaveFloat("VolumnSound", value);

        bgmSource.volume = value;
        sfxSource.volume = value;
        movementSource.volume = value * 0.7f;
    }

    public void SetMusic(bool on)
    {
        SettingData.Music = on;
        bgmSource.mute = !on;
    }

    public void SetSound(bool on)
    {
        SettingData.Sound = on;
        sfxSource.mute = !on;
        movementSource.mute = !on;
    }
}
