using UnityEngine;

public static class SaveSystem
{
    public static void SaveFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }

    public static float LoadFloat(string key, float defaultValue = 0f)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }
    public static void SaveInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
    }

    public static int LoadInt(string key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public static void SaveString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
    }

    public static string LoadString(string key, string defaultValue = "")
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }
    public static void SaveBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static bool LoadBool(string key, bool defaultValue = false)
    {
        return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
    }
    public static void Delete(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }

    public static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }
}
public static class SaveKeys
{
    public const string MUSIC = "Setting_Music";
    public const string SOUND = "Setting_Sound";

    public const string COIN = "Player_Coin";
    public const string DIAMOND = "Player_Diamond";

    public static string LevelStar(int level) => $"Level_{level}_Star";
    public static string LevelTime(int level) => $"Level_{level}_Time";

    public const string HIGHEST_LEVEL = "Player_Highest_Level";
}
public static class LevelSave
{
    public static void SaveLevelTime(int level, float time)
    {
        float best = SaveSystem.LoadFloat(SaveKeys.LevelTime(level), 0);

        if (best == 0 || time < best)
            SaveSystem.SaveFloat(SaveKeys.LevelTime(level), time);
    }

    public static float LoadLevelTime(int level)
    {
        return SaveSystem.LoadFloat(SaveKeys.LevelTime(level), 0);
    }


    public static void SaveLevelStars(int level, int stars)
    {
        int old = SaveSystem.LoadInt(SaveKeys.LevelStar(level), 0);

        if (stars > old)
            SaveSystem.SaveInt(SaveKeys.LevelStar(level), stars);
    }

    public static int LoadLevelStars(int level)
    {
        return SaveSystem.LoadInt(SaveKeys.LevelStar(level), 0);
    }


    public static int GetHighestUnlockedLevel()
    {
        return SaveSystem.LoadInt(SaveKeys.HIGHEST_LEVEL, 1);
    }

    public static void UnlockNextLevel(int finishedLevel)
    {
        int highest = GetHighestUnlockedLevel();

        if (finishedLevel + 1 > highest)
        {
            SaveSystem.SaveInt(SaveKeys.HIGHEST_LEVEL, finishedLevel + 1);
        }
    }

    public static bool IsLevelUnlocked(int level)
    {
        return level <= GetHighestUnlockedLevel();
    }
}

public static class SettingData
{
    public static bool Music
    {
        get => SaveSystem.LoadBool(SaveKeys.MUSIC, true);
        set => SaveSystem.SaveBool(SaveKeys.MUSIC, value);
    }

    public static bool Sound
    {
        get => SaveSystem.LoadBool(SaveKeys.SOUND, true);
        set => SaveSystem.SaveBool(SaveKeys.SOUND, value);
    }
}
