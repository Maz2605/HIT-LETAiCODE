using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UIToggle : MonoBehaviour
    {
        public Toggle toggle;
        public GameObject switchHandle;
        public Image switchBackground;

        public Sprite switchOn;
        public Sprite switchOff;

        private void Awake()
        {
            toggle = GetComponent<Toggle>();
        }
        
        public virtual void SwitchChange()
        {
            if (toggle.isOn)
            {
                /*
                AudioManager.Instance.PlaySoundToggleClickOn();
                */
                switchHandle.transform.DOLocalMoveX(50, 0.3f).SetEase(Ease.OutSine).OnPlay(() => {
                    switchBackground.sprite = switchOn;
                });
            } else
            {
                /*
                AudioManager.Instance.PlaySoundToggleClickOff();
                */

                switchHandle.transform.DOLocalMoveX(-50, 0.3f).SetEase(Ease.OutSine).OnPlay(() =>
                {
                    switchBackground.sprite = switchOff;
                });

            }
        }
    }
}
