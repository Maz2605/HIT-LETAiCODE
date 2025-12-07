using System;
using DG.Tweening;
using UnityEngine;

public class AnimationTranslate : Singleton<AnimationTranslate>
{
    [SerializeField] private GameObject loading;
    [SerializeField] private SpriteMask spriteMask;
    [SerializeField] private SpriteRenderer smile;

    [SerializeField] private float duration;

    private Action extraEvent;

    public Action ExtraEvent
    {
        set => extraEvent = value;
    }

    public bool IsActive { get; private set; } = false;
    public void DisplayLoading(bool enable, Action onClosed = null)
    {
        if (enable)
        {
            smile.transform.localScale = Vector3.zero;
            spriteMask.transform.localScale = Vector3.zero;

            IsActive = true;
            loading.SetActive(true);
            var sequence = DOTween.Sequence();

            sequence.Append(smile.transform.DOScale(Vector3.one * 8f, duration).SetEase(Ease.OutQuart));
        }
        else
        {
            spriteMask.transform.localScale = Vector3.zero;
            smile.transform.localScale = Vector3.one * 8f;

            spriteMask.transform.DOScale(Vector3.one * 8f, duration).SetEase(Ease.InQuart).OnComplete(() =>
            {
                onClosed?.Invoke();

                IsActive = false;
                loading.SetActive(false);
            });
        }
    }

    public void Loadingg(Action onLoading = null, Action onClosed = null)
    {
        DisplayLoading(true);

        DOVirtual.DelayedCall(duration, () => { onLoading?.Invoke(); });

        DOVirtual.DelayedCall(3 * duration, () =>
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
        DOVirtual.DelayedCall(duration, () =>
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