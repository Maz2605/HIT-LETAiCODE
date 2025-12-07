using System;
using DG.Tweening;
using UnityEngine;

public class AnimationTranslate : Singleton<AnimationTranslate>
{
    [SerializeField] private GameObject loading;
    [SerializeField] private SpriteMask spriteMask;
    [SerializeField] private SpriteRenderer smile;

    [SerializeField] private float duration = 0.6f;

    private Action extraEvent;

    private Tween currentTween;
    public bool IsActive { get; private set; } = false;


    public Action ExtraEvent
    {
        set => extraEvent = value;
    }

    public void DisplayLoading(bool enable, Action onClosed = null)
    {
        currentTween?.Kill();

        if (enable)
        {
            IsActive = true;
            loading.SetActive(true);

            smile.transform.localScale = Vector3.zero;
            spriteMask.transform.localScale = Vector3.zero;

            Sequence seq = DOTween.Sequence();

            seq.Append(
                smile.transform.DOScale(20f, duration).SetEase(Ease.OutBack)
            );

            currentTween = seq;
        }
        else
        {
            Sequence seq = DOTween.Sequence();

            seq.Append(
                spriteMask.transform.DOScale(20f, duration)
                .SetEase(Ease.InBack)
            );

            seq.OnComplete(() =>
            {
                loading.SetActive(false);
                IsActive = false;
                onClosed?.Invoke();
            });

            currentTween = seq;
        }
    }


    public void Loading(Action onLoading = null, Action onClosed = null)
    {
        DisplayLoading(true);

        DOVirtual.DelayedCall(duration, () => { onLoading?.Invoke(); });

        DOVirtual.DelayedCall(duration * 3, () =>
        {
            DisplayLoading(false, () =>
            {
                onClosed?.Invoke();
                extraEvent?.Invoke();
                extraEvent = null;
            });
        });
    }



    public void StartLoading(Action onLoading = null)
    {
        DisplayLoading(true);
        DOVirtual.DelayedCall(duration, () => { onLoading?.Invoke(); });
    }


    public void EndLoading(Action onClosed = null)
    {
        DOVirtual.DelayedCall(0.1f, () =>
        {
            DisplayLoading(false, () =>
            {
                onClosed?.Invoke();
                extraEvent?.Invoke();
                extraEvent = null;
            });
        });
    }
}
