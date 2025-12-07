using UnityEngine;

public class UIGamePlay : BaseUI
{
    protected override void Awake()
    {
        base.Awake();
    }


    public override void Show()
    {
        gameObject.SetActive(true);
    }

    public override void Hide(System.Action onComplete = null)
    {
        gameObject.SetActive(false);
        onComplete?.Invoke();
    }
}