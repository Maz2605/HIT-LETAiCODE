using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelInfoPopup : BaseUI
{
    public TextMeshProUGUI levelTitle;
    public TextMeshProUGUI playTimeText;
    public List<GameObject> _stars;

    public bool IsShowing => isShowing;
    bool isShowing = false;

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void ShowInfo(int levelIndex)
    {
        afterShow = () =>
        {
            bool unlocked = LevelSave.IsLevelUnlocked(levelIndex);

            levelTitle.text = "LEVEL " + levelIndex;

            foreach (var s in _stars) s.SetActive(false);

            if (!unlocked)
            {
                playTimeText.text = "--:--";
            }
            else
            {
                float time = LevelSave.LoadLevelTime(levelIndex);
                int stars = LevelSave.LoadLevelStars(levelIndex);

                playTimeText.text = (time <= 0) ? "--:--" : $"{time:0.00}";
                foreach (var s in _stars)
                {
                    s.SetActive(false);
                }
                for (int i = 0; i < stars && i < _stars.Count; i++)
                    _stars[i].SetActive(true);
            }
        };
        if (isShowing)
        {
            Hide(() =>
            {
                afterShow?.Invoke();
                Show();
                isShowing = true;
            });
        }
        else
        {
            Show();
            isShowing = true;
        }
    }

    public override void Hide()
    {
        base.Hide();
        isShowing = false;
    }

    public override void Hide(System.Action afterHide)
    {
        base.Hide(afterHide);
        isShowing = false;
    }
}