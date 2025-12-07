using System;
using UnityEngine;
using UnityEngine.UI;

namespace UIGame
{
    public class UISetting : BaseUI
    {
        [Header("Slider Sound FX")]
        [SerializeField] private Slider _sliderSoundFX;
        [SerializeField] private Image iconSound;
        public Sprite iconOnSoundFX;
        public Sprite iconOffSoundFX;

        [Header("Slider Music")]
        [SerializeField] private Slider _sliderMusic;
        [SerializeField] private Image iconMusic;
        public Sprite iconOnMusic;
        public Sprite iconOffMusic;

        private Action _actionClosed;
        private bool _isInit = false;

        private void Init()
        {
            if (_isInit) return;
            _sliderSoundFX.onValueChanged.AddListener(OnSoundValueChanged);
            _sliderMusic.onValueChanged.AddListener(OnMusicValueChanged);
            _isInit = true;
        }

        private void LoadSettingUI()
        {
            bool sound = SettingData.Sound;
            bool music = SettingData.Music;

            _sliderSoundFX.value = sound ? 1 : 0;
            _sliderMusic.value = music ? 1 : 0;

            iconSound.sprite = sound ? iconOnSoundFX : iconOffSoundFX;
            iconMusic.sprite = music ? iconOnMusic : iconOffMusic;
        }

        private void OnSoundValueChanged(float value)
        {
            bool isOn = value > 0.01f;
            SettingData.Sound = isOn;
            iconSound.sprite = isOn ? iconOnSoundFX : iconOffSoundFX;

            AudioManager.Instance?.SetValue(value);
        }

        private void OnMusicValueChanged(float value)
        {
            bool isOn = value > 0.01f;
            SettingData.Music = isOn;
            iconMusic.sprite = isOn ? iconOnMusic : iconOffMusic;

            if (AudioManager.Instance != null)
                AudioManager.Instance.bgmSource.volume = value;
        }

        public void SetActionClosed(Action action) => _actionClosed = action;

        public void Close() => ShowDisplay(false);

        public void ShowDisplay(bool enable, Action onShow = null, Action onClosed = null)
        {
            if (enable)
            {
                Init();
                LoadSettingUI();
                Time.timeScale = 0;
                onShow?.Invoke();
                Show();
            }
            else
            {
                Time.timeScale = 1;
                Hide(onClosed ?? (() => _actionClosed?.Invoke()));
            }
        }
    }
}
