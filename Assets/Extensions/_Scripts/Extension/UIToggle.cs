using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Scripts.Extension
{
    public class UIToggle : MonoBehaviour, IPointerClickHandler
    {
        public Toggle toggle;
        public RectTransform switchHandle;
        
        [Header("Position")]
        public Vector2 onPosition;
        public Vector2 offPosition;
        
        public List<TargetChange> targets;

        public UnityEvent switchOff;
        public UnityEvent switchOn;

        private void Awake()
        {
            toggle = GetComponent<Toggle>();
        }

        public virtual void SwitchChange()
        {
            if (toggle.isOn)
            {

                switchHandle.DOAnchorPos(onPosition, 0.3f).SetEase(Ease.OutSine)
                    .OnPlay(() =>
                    {
                        switchOn?.Invoke();
                        foreach (var target in targets)
                        {
                            target.SwitchChange(toggle.isOn);
                        }
                    })
                    .OnComplete(() =>
                    {

                    });
            }
            else
            {

                switchHandle.DOAnchorPos(offPosition, 0.3f).SetEase(Ease.OutSine)
                    .OnPlay(() =>
                    {
                        switchOff?.Invoke();
                        foreach (var target in targets)
                        {
                            target.SwitchChange(toggle.isOn);
                        }
                    })
                    .OnComplete(() =>
                    {

                    });
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (toggle.isOn)
            {
                /*
                AudioManager.instance.PlaySoundToggleClickOn();
            */
            }
            else
            {
                /*
                AudioManager.instance.PlaySoundToggleClickOff();
            */
            }
        }
    }

    [System.Serializable]
    public class TargetChange
    {
        public ToggleChangeStyle style;
        public TextMeshProUGUI targetText;
        public Image targetImage;

        public Color onColor;
        public Color offColor;
        
        public Sprite onSprite;
        public Sprite offSprite;

        public void SwitchChange(bool isOn)
        {
            if (style == ToggleChangeStyle.Color)
            {
                if (targetImage != null)
                {
                    if (isOn)
                    {
                        targetImage.DOColor(onColor, 0.3f);
                    }
                    else
                    {
                        targetImage.DOColor(offColor, 0.3f);
                    }
                }

                if (targetText != null)
                {
                    if (isOn)
                    {
                        targetText.DOColor(onColor, 0.3f);
                    }
                    else
                    {
                        targetText.DOColor(offColor, 0.3f);
                    }
                }
            }
            else if (style == ToggleChangeStyle.Sprite)
            {
                if (targetImage != null)
                {
                    if (isOn)
                    {
                        targetImage.sprite = onSprite;
                    }
                    else
                    {
                        targetImage.sprite = offSprite;
                    }
                }
            }
        }
    }

    [System.Serializable]
    public enum ToggleChangeStyle
    {
        Color = 0,
        Sprite = 1,
    }
}
