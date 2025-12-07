using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIGame
{
    public class UIPause : BaseUI
    {
        [Header("Buttons")]
        [SerializeField] private Button btnContinue;
        [SerializeField] private Button btnRestart;
        [SerializeField] private Button btnGiveUp;
        [SerializeField] private Button btnSetting;

        [Header("Level Info")]
        [SerializeField] private TextMeshProUGUI txtLevel;

        protected override void Awake()
        {
            base.Awake();
            RegisterButtonListeners();
        }

        private void RegisterButtonListeners()
        {
            btnContinue?.onClick.AddListener(OnContinue);
            btnRestart?.onClick.AddListener(OnRestart);
            btnGiveUp?.onClick.AddListener(OnGiveUp);
            btnSetting?.onClick.AddListener(OnSetting);
        }

        public void ShowDisplay(bool enable, string levelText = "")
        {
            if (enable)
            {
                if (txtLevel != null)
                    txtLevel.text = levelText;

                Time.timeScale = 0;
                Show();
            }
            else
            {
                Time.timeScale = 1;
                Hide();
            }
        }

        #region Button Actions — gọi GameManager trực tiếp

        private void OnContinue()
        {
            ShowDisplay(false);
            GameManager.Instance.ResumeGame();
        }

        private void OnRestart()
        {
            ShowDisplay(false);
            GameManager.Instance.ResumeGame();
            GameManager.Instance.RestartLevel();
        }

        private void OnGiveUp()
        {
            ShowDisplay(false);
            GameManager.Instance.GiveUpLevel();
        }

        private void OnSetting()
        {
            ShowDisplay(false);
            UIManager.Instance.ShowSetting(true);
        }

        #endregion
    }
}