using System;
using UnityEngine;
using DG.Tweening;

public enum MoveDirection
{
    None,
    FromLeft,
    FromRight,
    FromTop,
    FromBottom,
    CustomOffset
}

public class BaseUI : MonoBehaviour
{
    [Header("UI Animation Settings")]
    public float fadeDuration = 0.25f;
    public float scaleDuration = 0.25f;
    public float startScale = 0.8f;

    [Header("Move Animation")]
    public MoveDirection moveDirection = MoveDirection.None;
    public Vector2 customOffset = new Vector2(0, -50);
    public float moveDuration = 0.25f;

    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected RectTransform rect;               
    protected Vector2 originalAnchoredPos;      
    protected Vector3 originalScale;
    protected Action afterShow;

    protected virtual void Awake()
    {
        originalScale = rect.localScale;
        originalAnchoredPos = rect.anchoredPosition;
    }

    Vector2 GetStartOffset()
    {
        switch (moveDirection)
        {
            case MoveDirection.FromLeft: return new Vector2(-Screen.width, 0);
            case MoveDirection.FromRight: return new Vector2(Screen.width, 0);
            case MoveDirection.FromTop: return new Vector2(0, Screen.height);
            case MoveDirection.FromBottom: return new Vector2(0, -Screen.height);
            case MoveDirection.CustomOffset: return customOffset;
            default: return Vector2.zero;
        }
    }

    public virtual void Show()
    {
        rect.gameObject.SetActive(true);
        canvasGroup.alpha = 0;
        rect.localScale = originalScale * startScale;

        Vector2 startOffset = GetStartOffset();
        rect.anchoredPosition = originalAnchoredPos + startOffset;

        Sequence seq = DOTween.Sequence();
        seq.Join(canvasGroup.DOFade(1, fadeDuration));
        seq.Join(rect.DOScale(originalScale, scaleDuration).SetEase(Ease.OutBack));
        if (moveDirection != MoveDirection.None)
            seq.Join(rect.DOAnchorPos(originalAnchoredPos, moveDuration).SetEase(Ease.OutCubic));

        seq.OnComplete(() => afterShow?.Invoke()).SetUpdate(true);
    }

    public virtual void Hide(Action afterHide = null)
    {
        Vector2 startOffset = GetStartOffset();

        Sequence seq = DOTween.Sequence();
        seq.Join(canvasGroup.DOFade(0, fadeDuration));
        seq.Join(rect.DOScale(originalScale * startScale, scaleDuration).SetEase(Ease.InBack));
        if (moveDirection != MoveDirection.None)
            seq.Join(rect.DOAnchorPos(originalAnchoredPos + startOffset, moveDuration).SetEase(Ease.InCubic));

        seq.OnComplete(() =>
        {
            rect.gameObject.SetActive(false);
            afterHide?.Invoke();
        }).SetUpdate(true);
    }
}
