using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIResult : BaseUI
{
    [Header("Star Objects")]
    public GameObject star1;
    public GameObject star2;
    public GameObject star3;

    [Header("Texts")]
    public TextMeshProUGUI txtTime;
    public TextMeshProUGUI txtTitle;

    [Header("Buttons")]
    public Button btnHome;
    public Button btnNext;

    private Action _callbackHome;
    private Action _callbackNext;

    protected override void Awake()
    {
        base.Awake();
        btnHome?.onClick.AddListener(OnHomeClicked);
        btnNext?.onClick.AddListener(OnNextClicked);
    }

    public void SetHomeAction(Action action)
    {
        _callbackHome = action;
    }

    public void SetNextAction(Action action)
    {
        _callbackNext = action;
    }

    /// <summary>
    /// Hiển thị màn result
    /// </summary>
    public void ShowResult(bool isWin, int starCount, float playTime, string title)
    {
        txtTime.text = FormatTime(playTime);
        txtTitle.text = isWin ? $"{title} - YOU WIN!" : $"{title} - YOU LOSE!";

        // Nút next chỉ hiện nếu win
        btnNext.gameObject.SetActive(isWin);

        // Hiển thị sao nếu win
        if (isWin)
            ShowStars(starCount);
        else
            HideStars();

        Time.timeScale = 0;
        Show();
    }


    void ShowStars(int starCount)
    {
        GameObject[] stars = { star1, star2, star3 };

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].SetActive(false);
            stars[i].transform.localScale = Vector3.zero;
        }

        for (int i = 0; i < starCount && i < 3; i++)
        {
            stars[i].SetActive(true);
            stars[i].transform.DOScale(1f, 0.35f)
                .SetEase(Ease.OutBack)
                .SetDelay(i * 0.15f);
        }
    }

    void HideStars()
    {
        star1.SetActive(false);
        star2.SetActive(false);
        star3.SetActive(false);
    }

    string FormatTime(float t)
    {
        int minutes = (int)(t / 60);
        int seconds = (int)(t % 60);
        return $"{minutes:00}:{seconds:00}";
    }

    private void OnHomeClicked()
    {
        Hide(() =>
        {
            Time.timeScale = 1;

            AnimationTranslate.Instance.StartLoading(() =>
            {
                _callbackHome?.Invoke();
            });

            AnimationTranslate.Instance.EndLoading(() =>
            {
                UIManager.Instance
                    .GetUI<LevelSelectUI>("LevelSelectUI")
                    .Show();
            });
        });
    }

    private void OnNextClicked()
    {
        Hide(() =>
        {
            Time.timeScale = 1;

            AnimationTranslate.Instance.StartLoading(() =>
            {
                _callbackNext?.Invoke();
            });

            AnimationTranslate.Instance.EndLoading(() =>
            {
                // Không mở LevelSelect → load luôn next level
            });
        });
    }
}
