using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LevelButton : MonoBehaviour
{
    [Header("Level")]
    public int levelIndex;

    [Header("UI")]
    public Image icon;
    public TextMeshProUGUI levelText;
    public GameObject focus;

    [Header("Sprites")]
    public Sprite unlockedSprite;
    public Sprite lockedSprite;

    [Header("State")]
    public bool isLocked = false;

    public UnityEvent<int> onSelect;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            onSelect?.Invoke(levelIndex);
        });
        levelText.text = levelIndex.ToString();
        RefreshVisual();
    }

    public void SetLocked(bool locked)
    {
        isLocked = locked;
        RefreshVisual();
    }

    public void SetFocus(bool active)
    {
        focus.SetActive(!isLocked && active);
    }

    void RefreshVisual()
    {
        if (icon != null)
            icon.sprite = isLocked ? lockedSprite : unlockedSprite;

        if (levelText != null)
            levelText.color = isLocked 
                ? new Color(1, 1, 1, 0.4f)  
                : Color.white;
    }
}