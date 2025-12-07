using System;
using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Scripts.Extension
{
    public class SceneLoader : Singleton<SceneLoader>
    {
        [SerializeField] private Slider loading;
        [SerializeField] private TextMeshProUGUI loadingText;
        [SerializeField] private GameObject tapToPlayButton;

        public float timer;

        private string sceneName
        {
            get => PlayerPrefs.GetString("sceneName", "Gameplay");
            set => PlayerPrefs.SetString("sceneName", value);
        }

        protected override void Awake()
        {
            base.KeepAlive(true);
            base.Awake();
        }

        private void Start()
        {
            if (tapToPlayButton)
                tapToPlayButton.SetActive(false);   

            IntoGamePlay();
        }

        public void IntoGamePlay()
        {
            LoadScene(sceneName);
        }

        public void LoadScene(string sceneName)
        {
            var scene = SceneManager.LoadSceneAsync(sceneName);
            if (scene != null)
            {
                scene.allowSceneActivation = false;

                loading.value = 0f;
                if (loadingText != null)
                    loadingText.text = "0%";

                loading
                    .DOValue(1f, timer)
                    .SetEase(Ease.Linear)
                    .OnUpdate(() =>
                    {
                        if (loadingText != null)
                        {
                            float percent = loading.value * 100f;
                            loadingText.text = $"{percent:0}%";
                        }
                    })
                    .OnComplete(() =>
                    {
                        if (loadingText != null)
                            loadingText.text = "100%";

                        loading.transform.parent.gameObject.SetActive(false);

                        if (tapToPlayButton != null)
                            tapToPlayButton.SetActive(true);

                        tapToPlayButton.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            tapToPlayButton.SetActive(false);

                            AnimationTranslate.Instance.StartLoading(() =>
                            {
                                AnimationTranslate.Instance.DisplayLoading(false);
                                scene.allowSceneActivation = true;
                            });
                        });
                    });
            }
        }
    }
}
