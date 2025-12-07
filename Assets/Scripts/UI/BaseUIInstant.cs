using System;
using UnityEngine;

public class BaseUIInstant : BaseUI
{
    protected override void Awake()
    {
    }

    public override void Show()
    {
        rect.gameObject.SetActive(true);
        afterShow?.Invoke();
    }

    public override void Hide(Action afterHide = null)
    {
        rect.gameObject.SetActive(false);
        afterHide?.Invoke();
    }
}