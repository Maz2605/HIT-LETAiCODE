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
            else
                Debug.LogWarning($"[UIManager] Duplicate UI name found: {key}. Skipped.");
        }

        Debug.Log($"[UIManager] Loaded {uiDict.Count} UI(s):");
        foreach (var kvp in uiDict)
        {
            Debug.Log($"UI Name: {kvp.Key}, Active: {kvp.Value.gameObject.activeSelf}");
        }
    }


    public void ShowPause(string levelText = "")
    {
        if (uiDict.TryGetValue("UIPause", out BaseUI ui) && ui is UIPause pauseUI)
        {
            Debug.Log($"[UIManager] Showing UIPause with levelText: {levelText}");
            pauseUI.ShowDisplay(true, levelText);
        }
        else
        {
            Debug.LogWarning("[UIManager] UIPause not found in dictionary!");
        }
    }

    public void ShowSetting(bool enable)
    {
        Debug.Log($"[UIManager] ShowSetting: {enable}");

        // Tắt UIPause khi mở UISetting
        if (enable && uiDict.TryGetValue("UIPause", out BaseUI pauseUI) && pauseUI is UIPause pause)
            pause.ShowDisplay(false);

        if (uiDict.TryGetValue("UISetting", out BaseUI ui) && ui is UISetting settingUI)
        {
            if (enable)
            {
                // Khi đóng Setting → tự bật lại Pause
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
        else
        {
            Debug.LogWarning("[UIManager] UISetting not found!");
        }
    }


    /// <summary>
    /// Ẩn UI theo tên.
    /// </summary>
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
