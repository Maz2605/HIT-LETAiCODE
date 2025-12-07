using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("Level Prefabs")]
    public GameObject[] levelPrefabs;

    private GameObject currentLevelInstance;
    public int CurrentLevelIndex { get; private set; } = 0;
    public Vector3 CurrentPosLevel { get; set; }

    [SerializeField] private PlayerController _playerPrefab;
    [SerializeField] private PlayerController _currentPlayerPrefab;

    protected override void Awake()
    {
        base.Awake();
        KeepAlive(true);
    }

    #region LEVEL LOAD

    public void LoadLevel(int index)
    {
        if (index < 0 || index >= levelPrefabs.Length)
        {
            Debug.LogError($"GameManager: Level index {index} invalid!");
            return;
        }

        CurrentLevelIndex = index;

        if (currentLevelInstance != null)
            Destroy(currentLevelInstance);

        currentLevelInstance = Instantiate(levelPrefabs[index]);

        Debug.Log($"Loaded Level {index + 1}");

        UIManager.Instance.GetUI<UIGamePlay>("UIGamePlay")?.Show();
    }

    public void RestartLevel()
    {
        AnimationTranslate.Instance.StartLoading(() =>
        {
            if (currentLevelInstance != null)
                Destroy(currentLevelInstance);

            LoadLevel(CurrentLevelIndex);
        });

        AnimationTranslate.Instance.EndLoading();
    }

    public void NextLevel()
    {
        int next = CurrentLevelIndex + 1;

        AnimationTranslate.Instance.StartLoading(() =>
        {
            if (currentLevelInstance != null)
                Destroy(currentLevelInstance);

            if (next >= levelPrefabs.Length)
            {
                Debug.Log("Hết level → về Level Select");

                UIManager.Instance.GetUI<UIGamePlay>("UIGamePlay")?.Hide();
            }
            else
            {
                LoadLevel(next);
            }
        });

        AnimationTranslate.Instance.EndLoading(() =>
        {
            if (next >= levelPrefabs.Length)
            {
                UIManager.Instance.GetUI<LevelSelectUI>("LevelSelectUI")?.Show();
            }
        });
    }

    #endregion
    private void SpawnPlayer()
    {
        if (_currentPlayerPrefab != null) Destroy(_currentPlayerPrefab.gameObject);

        _currentPlayerPrefab = Instantiate(_playerPrefab, Vector3.zero, Quaternion.identity);

        _currentPlayerPrefab.ResetPlayerFull();

        var rec = _currentPlayerPrefab.GetComponent<InputRecorder>();
        if (rec != null) rec.HardReset();
        ResetCloneManager();
    }

    private void ClearPlayer()
    {
        if (_currentPlayerPrefab != null)
        {
            Destroy(_currentPlayerPrefab.gameObject);
            _currentPlayerPrefab = null;
        }
    }

    private void ResetCloneManager()
    {
        if (CloneManager.Instance != null)
            CloneManager.Instance.ResetAllClones();
    }


    #region PAUSE FUNCTIONS (Thêm mới cho UIPause)

    public void PauseGame()
    {
        Time.timeScale = 0;

        var uiPause = UIManager.Instance.GetUI<UIGame.UIPause>("UIPause");

        uiPause.SetActionContinue(() =>
        {
            ResumeGame();
        });

        uiPause.SetActionRestart(() =>
        {
            ResumeGame();
            RestartLevel();
        });

        uiPause.SetActionGiveUp(() =>
        {
            GiveUpLevel();
        });

        uiPause.ShowDisplay(true, "LEVEL " + (CurrentLevelIndex + 1));
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
    }

  
    public void GiveUpLevel()
    {
        Time.timeScale = 1;

        AnimationTranslate.Instance.StartLoading(() =>
        {
            if (currentLevelInstance != null)
                Destroy(currentLevelInstance);

            UIManager.Instance.GetUI<UIGamePlay>("UIGamePlay")?.Hide();
        });

        AnimationTranslate.Instance.EndLoading(() =>
        {
            UIManager.Instance.GetUI<LevelSelectUI>("LevelSelectUI")?.Show();
        });
    }

    #endregion

    #region STAR CALC & RESULT

    public int CalculateStars(float playTime)
    {
        if (playTime <= 120f) return 3;   
        if (playTime <= 180f) return 2;  
        return 1;
    }

    public void OnLevelCompleted(float time)
    {
        int level = CurrentLevelIndex + 1;
        int stars = CalculateStars(time);

        LevelSave.SaveLevelStars(level, stars);
        LevelSave.SaveLevelTime(level, time);
        LevelSave.UnlockNextLevel(level);

        var resultUI = UIManager.Instance.GetUI<UIResult>("UIResult");

        resultUI.SetHomeAction(() =>
        {
            AnimationTranslate.Instance.StartLoading(() =>
            {
                if (currentLevelInstance != null)
                    Destroy(currentLevelInstance);

                UIManager.Instance.GetUI<UIGamePlay>("UIGamePlay")?.Hide();
            });

            AnimationTranslate.Instance.EndLoading(() =>
            {
                UIManager.Instance.GetUI<LevelSelectUI>("LevelSelectUI")?.Show();
            });
        });

        resultUI.SetNextAction(() =>
        {
            NextLevel();
        });

        resultUI.ShowResult(true, stars, time, $"LEVEL {level}");
    }

    public void OnLevelFailed(float time)
    {
        int level = CurrentLevelIndex + 1;

        var resultUI = UIManager.Instance.GetUI<UIResult>("UIResult");

        resultUI.SetHomeAction(() =>
        {
            AnimationTranslate.Instance.StartLoading(() =>
            {
                if (currentLevelInstance != null)
                    Destroy(currentLevelInstance);

                UIManager.Instance.GetUI<UIGamePlay>("UIGamePlay")?.Hide();
            });

            AnimationTranslate.Instance.EndLoading(() =>
            {
                UIManager.Instance.GetUI<LevelSelectUI>("LevelSelectUI")?.Show();
            });
        });

        resultUI.ShowResult(false, 0, time, $"LEVEL {level}");
    }

    #endregion
}
