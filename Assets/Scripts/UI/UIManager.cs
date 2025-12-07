using System.Collections.Generic;
using UnityEngine;
using UIGame;

public class UIManager : Singleton<UIManager>
{
    private readonly Dictionary<string, BaseUI> uiDict = new Dictionary<string, BaseUI>();

    protected override void Awake()
    {
        KeepAlive(false);
        base.Awake();

        // Tự động tìm tất cả UI trong con
        BaseUI[] uis = GetComponentsInChildren<BaseUI>(true);

        foreach (var ui in uis)
        {
            if (ui == null) continue;

            string key = ui.gameObject.name;

            if (!uiDict.ContainsKey(key))
                uiDict.Add(key, ui);
        }
    }


    public void ShowPause(string levelText = "")
    {
        if (uiDict.TryGetValue("UIPause", out BaseUI ui) && ui is UIPause pauseUI)
        {
            pauseUI.ShowDisplay(true, levelText);
        }
    }

    public void ShowSetting(bool enable)
    {

        if (enable && uiDict.TryGetValue("UIPause", out BaseUI pauseUI) && pauseUI is UIPause pause)
            pause.ShowDisplay(false);

        if (uiDict.TryGetValue("UISetting", out BaseUI ui) && ui is UISetting settingUI)
        {
            if (enable)
            {
                settingUI.SetActionClosed(() =>
                {
                    if (uiDict.TryGetValue("UIPause", out BaseUI pu) && pu is UIPause p)
                        p.ShowDisplay(true);
                });

                settingUI.ShowDisplay(true);
            }
            else
            {
                settingUI.ShowDisplay(false);
            }
        }
    }
    public void HideUI(string name)
    {
        if (uiDict.TryGetValue(name, out BaseUI ui))
            ui.Hide();
    }

    /// <summary>
    /// Lấy UI theo tên.
    /// </summary>
    public T GetUI<T>(string name) where T : BaseUI
    {
        if (uiDict.TryGetValue(name, out BaseUI ui) && ui is T t)
            return t;

        return null;
    }
}
