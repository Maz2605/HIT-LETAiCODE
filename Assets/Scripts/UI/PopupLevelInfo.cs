using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelInfoPopup : BaseUI
{
    [Header("UI Elements")]
    public TextMeshProUGUI levelTitle;
    public TextMeshProUGUI playTimeText;
    public List<GameObject> _stars;

    [Header("Play Button")]
    public Button playButton;
    public Image playButtonImage;
    public Sprite spriteUnlocked;
    public Sprite spriteLocked;

    private int _currentLevelIndex;
    private bool _isShowing = false;

    public bool IsShowing => _isShowing;

    private void Start()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayClicked);
    }
    public void ShowInfo(int levelIndex)
    {
        _currentLevelIndex = levelIndex;

        afterShow = () =>
        {
            bool unlocked = LevelSave.IsLevelUnlocked(_currentLevelIndex);

            if (levelTitle != null)
                levelTitle.text = "LEVEL " + _currentLevelIndex;

            foreach (var s in _stars)
                s.SetActive(false);

            if (unlocked)
            {
                float time = LevelSave.LoadLevelTime(_currentLevelIndex);
                int stars = LevelSave.LoadLevelStars(_currentLevelIndex);

                playTimeText.text = (time <= 0) ? "--:--" : $"{time:0.00}";

                for (int i = 0; i < stars && i < _stars.Count; i++)
                    _stars[i].SetActive(true);
            }
            else
            {
                playTimeText.text = "--:--";
            }

            UpdatePlayButton(unlocked);
        };

        if (_isShowing)
        {
            Hide(() =>
            {
                Show();
                _isShowing = true;
            });
        }
        else
        {
            Show();
            _isShowing = true;
        }
    }

    private void UpdatePlayButton(bool unlocked)
    {
        if (playButton == null || playButtonImage == null)
            return;

        playButton.interactable = unlocked;
        playButtonImage.sprite = unlocked ? spriteUnlocked : spriteLocked;
    }

    private void OnPlayClicked()
    {
        if (!LevelSave.IsLevelUnlocked(_currentLevelIndex))
            return;

        Hide();

        AnimationTranslate.Instance.StartLoading(() =>
        {
            GameManager.Instance.LoadLevel(_currentLevelIndex - 1); 
        });

        AnimationTranslate.Instance.EndLoading();
    }

    public override void Show()
    {
        base.Show();
        _isShowing = true;
    }

    public void Hide()
    {
        base.Hide();
        _isShowing = false;
    }

    public override void Hide(System.Action afterHide)
    {
        base.Hide(afterHide);
        _isShowing = false;
    }
}
