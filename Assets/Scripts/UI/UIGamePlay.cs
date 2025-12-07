using UnityEngine;

public class UIGamePlay : BaseUI
{
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
}