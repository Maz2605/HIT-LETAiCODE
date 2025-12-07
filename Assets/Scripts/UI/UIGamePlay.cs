using System;
using System.Collections.Generic;
using DesignPattern.Obsever;
using UnityEngine;
using DG.Tweening;

public class UIGamePlay : BaseUI
{
    public List<GameObject> _timeItem;

    [Header("Animation Settings")]
    public float usedScale = 0f;
    public float recoverScale = 1.0f;
    public float animDuration = 0.25f;

    [Header("Blink Settings")]
    public float blinkMin = 0.15f;
    public float blinkMax = 1f;
    public float blinkDuration = 0.6f;

    private Tween currentBlinkTween;
    public GameObject currentBlinkObj;

    protected override void Awake()
    {
        base.Awake();
    }
    public override void Show()
    {
        rect.gameObject.SetActive(true);
        UIManager.Instance.GetUI<LevelSelectUI>("LevelSelectUI")?.Hide();
    }

    public override void Hide(System.Action onComplete = null)
    {
        rect.gameObject.SetActive(false);
        onComplete?.Invoke();
    }
    public void UseTimeItem(int index)
    {
        if (!IsValid(index)) return;

        var item = _timeItem[index];
        item.SetActive(true);
        Debug.Log("XXX");
        item.transform.DOScale(usedScale, animDuration)
            .SetEase(Ease.OutBack).OnComplete(() => item.SetActive(false));
    }

    public void RecoverTimeItem(int index)
    {
        if (!IsValid(index)) return;

        var item = _timeItem[index];
        item.SetActive(true);
        item.transform.DOScale(recoverScale, animDuration)
            .SetEase(Ease.OutElastic);
    }
    public void BlinkTimeItem()
    {
        StartBlink();
    }
    public void StartBlink()
    {
        StopBlink();
        currentBlinkObj.gameObject.SetActive(true);
        CanvasGroup cg = currentBlinkObj.GetComponent<CanvasGroup>();
        if (cg == null) cg = currentBlinkObj.AddComponent<CanvasGroup>();

        currentBlinkTween = cg
            .DOFade(blinkMin, blinkDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
    public void StopBlink()
    {
        if (currentBlinkTween != null)
        {
            currentBlinkTween.Kill();
            currentBlinkTween = null;
        }

        if (currentBlinkObj != null)
        {
            var cg = currentBlinkObj.GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = 1f;
        }
        currentBlinkObj.gameObject.SetActive(false);
    }
    public void ResetAllItems()
    {
        StopBlink();

        foreach (var item in _timeItem)
        {
            if (item == null) continue;

            item.SetActive(true);
            item.transform.localScale = Vector3.one;

            var cg = item.GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = 1f;
        }
    }

    private bool IsValid(int index)
    {
        return _timeItem != null && index >= 0 && index < _timeItem.Count;
    }
}
