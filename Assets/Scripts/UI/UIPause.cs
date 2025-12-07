using System;
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

        private Action _actionContinue;
        private Action _actionRestart;
        private Action _actionGiveUp;

        protected override void Awake()
        {
            base.Awake();
            RegisterButtonListeners();
        }

        private void RegisterButtonListeners()
        {
            if (btnContinue != null) btnContinue.onClick.AddListener(Continue);
            if (btnRestart != null) btnRestart.onClick.AddListener(Restart);
            if (btnGiveUp != null) btnGiveUp.onClick.AddListener(GiveUp);
            if (btnSetting != null) btnSetting.onClick.AddListener(OpenSetting);
        }

        public void ShowDisplay(bool enable, string levelText = "", Action onShow = null, Action onClosed = null)
        {
            if (enable)
            {
                if (txtLevel != null)
                    txtLevel.text = levelText;

                Time.timeScale = 0;
                afterShow = onShow;
                Show();
            }
            else
            {
                Time.timeScale = 1;
                Hide(onClosed);
            }
        }

        #region Set Action
        public void SetActionContinue(Action action) => _actionContinue = action;
        public void SetActionRestart(Action action) => _actionRestart = action;
        public void SetActionGiveUp(Action action) => _actionGiveUp = action;
        #endregion

        #region Button Methods
        private void Continue() => ShowDisplay(false, null, _actionContinue);
        private void Restart() => ShowDisplay(false, null, _actionRestart);
        private void GiveUp() => ShowDisplay(false, null, _actionGiveUp);

        private void OpenSetting()
        {
            ShowDisplay(false);
            UIManager.Instance.ShowSetting(true);
        }
        #endregion
    }
}
