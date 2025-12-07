using UnityEngine;

public class LevelSelectUI : BaseUIInstant
{
    public LevelInfoPopup infoPopup;
    public LevelButton[] levelButtons;

    int currentFocusIndex = -1;
    int currentSelectedLevel = -1;

    void Start()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            LevelButton btn = levelButtons[i];

            btn.onSelect.AddListener(OnSelectLevel);

            bool unlocked = LevelSave.IsLevelUnlocked(btn.levelIndex);
            btn.SetLocked(!unlocked);

            btn.SetFocus(false);
        }
    }

    void OnSelectLevel(int levelIndex)
    {
        bool unlocked = LevelSave.IsLevelUnlocked(levelIndex);

        if (currentSelectedLevel == levelIndex && infoPopup.IsShowing)
        {
            infoPopup.Hide();

            if (currentFocusIndex >= 0)
                levelButtons[currentFocusIndex].SetFocus(false);

            currentSelectedLevel = -1;
            currentFocusIndex = -1;
            return;
        }

        currentSelectedLevel = levelIndex;

        int btnIndex = FindButtonIndex(levelIndex);
        if (btnIndex == -1) return;

        if (currentFocusIndex >= 0)
            levelButtons[currentFocusIndex].SetFocus(false);

        currentFocusIndex = btnIndex;
        levelButtons[currentFocusIndex].SetFocus(unlocked);

        infoPopup.ShowInfo(levelIndex);
    }

    int FindButtonIndex(int levelIndex)
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i].levelIndex == levelIndex)
                return i;
        }
        return -1;
    }
}