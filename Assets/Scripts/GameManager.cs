using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("Level Prefabs")] 
    public GameObject[] levelPrefabs;
    public float PlayTime { get; private set; }
    private bool isTiming = false;
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

    private void Start()
    {
        AudioManager.Instance.PlayBGM(AudioManager.Instance.bgmClip);
    }

    #region LEVEL LOAD

    public void LoadLevel(int index)
    {
        if (index < 0 || index >= levelPrefabs.Length)
        {
            Debug.LogError($"GameManager: Level index {index} invalid!");
            return;
        }
        PlayTime = 0;
        isTiming = true;

        CurrentLevelIndex = index;

        if (currentLevelInstance != null)
            Destroy(currentLevelInstance);

        currentLevelInstance = Instantiate(levelPrefabs[index]);
        var lv = currentLevelInstance.GetComponent<Level>();
        ClearPlayer();
        
        SpawnPlayer(lv.spawnAtOrigin);
        lv.SetUpdateFollow(_currentPlayerPrefab);

        Debug.Log($"Loaded Level {index + 1}");

        UIManager.Instance.GetUI<UIGamePlay>("UIGamePlay")?.Show();
    }
    private void Update()
    {
        if (!isTiming) return;

        PlayTime += Time.deltaTime;
    }

    public void RestartLevel()
    {
        AnimationTranslate.Instance.StartLoading(() =>
        {
            if (currentLevelInstance != null)
                Destroy(currentLevelInstance);

            PlayTime = 0;
            isTiming = true;

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

    private void SpawnPlayer(Transform pos)
    {
        if (_currentPlayerPrefab != null) 
            Destroy(_currentPlayerPrefab.gameObject);

        _currentPlayerPrefab = Instantiate(
            _playerPrefab, 
            pos.position + Vector3.up * 5f, 
            Quaternion.identity
        );

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

    #region PAUSE FUNCTIONS (Dùng UIPause mới)

    public void PauseGame()
    {
        Time.timeScale = 0;

        var uiPause = UIManager.Instance.GetUI<UIGame.UIPause>("UIPause");

        uiPause.ShowDisplay(true, $"LEVEL {CurrentLevelIndex + 1}");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        isTiming = true;
    }

    public void GiveUpLevel()
    {
        Time.timeScale = 1;
        isTiming = false;
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

    public void OnLevelCompleted()
    {
        isTiming = false;
        int level = CurrentLevelIndex + 1;
        int stars = CalculateStars(PlayTime);

        LevelSave.SaveLevelStars(level, stars);
        LevelSave.SaveLevelTime(level, PlayTime);
        LevelSave.UnlockNextLevel(level);

        var resultUI = UIManager.Instance.GetUI<UIResult>("UIResult");

        resultUI.SetHomeAction(() =>
        {
            AnimationTranslate.Instance.StartLoading(() =>
            {
                if (currentLevelInstance != null)
                    Destroy(currentLevelInstance);
                if (_currentPlayerPrefab != null)
                {
                    Destroy(_currentPlayerPrefab);
                }
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

        resultUI.ShowResult(true, stars, PlayTime, $"LEVEL {level}");
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
